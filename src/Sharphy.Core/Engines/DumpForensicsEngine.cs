using Microsoft.Diagnostics.Runtime;

namespace Sharphy.Core.Engines;

/// <summary>
/// Provides crash dump telemetry and managed heap forensic inspection powered by Microsoft ClrMD.
/// </summary>
public static class DumpForensicsEngine
{
    public static string AnalyzeCrashDump(string dumpPath)
    {
        if (!File.Exists(dumpPath))
        {
            return $"[Sharphy Dump Forensics] Target crash dump file was not found at '{dumpPath}'. Make sure to supply an existing .dmp or CoreCLR ELF dump.";
        }

        using var target = DataTarget.LoadDump(dumpPath);
        var runtime = target.ClrVersions.FirstOrDefault()?.CreateRuntime();

        if (runtime == null)
        {
            return $"[Sharphy Dump Forensics] No supported CLR runtime found inside the dump: {dumpPath}.";
        }

        var sw = new StringWriter();
        sw.WriteLine($"=== Root Cause Analysis (RCA) Report: {Path.GetFileName(dumpPath)} ===");
        sw.WriteLine($"CLR Flavor: {runtime.ClrInfo.Flavor}");
        sw.WriteLine($"CLR Version: {runtime.ClrInfo.Version}");
        sw.WriteLine($"Server GC: {runtime.Heap.IsServer}");
        sw.WriteLine($"Managed Threads Count: {runtime.Threads.Length}");
        sw.WriteLine();

        sw.WriteLine("--- Threads & Exceptions ---");
        foreach (var thread in runtime.Threads.Where(t => t.IsAlive))
        {
            sw.WriteLine($"[Thread #{thread.ManagedThreadId}] OS Thread ID: 0x{thread.OSThreadId:X4}");
            if (thread.CurrentException != null)
            {
                sw.WriteLine($"  🔥 Crashing Exception: {thread.CurrentException.Type.Name}: {thread.CurrentException.Message}");
                sw.WriteLine($"  HRESULT: 0x{thread.CurrentException.HResult:X8}");
            }
        }

        // ThreadPool Starvation & Sync-over-Async Heuristic Analysis
        sw.WriteLine();
        sw.WriteLine("--- ThreadPool Health & Sync-over-Async Triage ---");
        try
        {
            var tp = runtime.ThreadPool;
            if (tp != null)
            {
                sw.WriteLine($"ThreadPool Active Workers: {tp.ActiveWorkerThreads} (Idle: {tp.IdleWorkerThreads}, Retired: {tp.RetiredWorkerThreads})");
                sw.WriteLine($"Min / Max Worker Threads: {tp.MinThreads} / {tp.MaxThreads}");
                sw.WriteLine($"CPU Utilization: {tp.CpuUtilization}%");
            }

            var blockedThreads = new List<(int ManagedThreadId, string BlockingPattern, string Frame)>();
            var syncSignatures = new[]
            {
                "System.Threading.Tasks.Task.Wait",
                "System.Threading.Tasks.Task.GetResult",
                "System.Threading.Tasks.Task`1.GetResult",
                "System.Threading.Tasks.ValueTask.GetResult",
                "System.Threading.Monitor.Enter",
                "System.Threading.Monitor.Wait",
                "System.Threading.ManualResetEventSlim.Wait",
                "System.Threading.WaitHandle.WaitOne"
            };

            foreach (var thread in runtime.Threads.Where(t => t.IsAlive))
            {
                foreach (var frame in thread.EnumerateStackTrace())
                {
                    var display = frame.FrameName ?? frame.Method?.Signature ?? string.Empty;
                    var matchedSig = syncSignatures.FirstOrDefault(sig => display.Contains(sig, StringComparison.OrdinalIgnoreCase));
                    if (matchedSig != null)
                    {
                        blockedThreads.Add((thread.ManagedThreadId, matchedSig, display));
                        break;
                    }
                }
            }

            if (blockedThreads.Count > 0)
            {
                sw.WriteLine($"[⚠️ THREADPOOL STARVATION DETECTED] Identified {blockedThreads.Count} thread(s) blocked in synchronous waits:");
                foreach (var (threadId, pattern, frame) in blockedThreads)
                {
                    sw.WriteLine($"  - Thread #{threadId}: Blocked on '{pattern}'");
                    sw.WriteLine($"    Frame: {frame}");
                }
            }
            else
            {
                sw.WriteLine("ThreadPool Health: ✅ Normal. No sync-over-async blocking patterns detected across managed threads.");
            }
        }
        catch (Exception ex)
        {
            sw.WriteLine($"[ThreadPool Inspection Notice] Telemetry evaluation skipped: {ex.Message}");
        }

        return sw.ToString();
    }

    public static string InspectHeap(string dumpPath, int topObjects = 20)
    {
        if (!File.Exists(dumpPath))
        {
            return $"[Sharphy Managed Heap] Target dump file was not found at '{dumpPath}'.";
        }

        using var target = DataTarget.LoadDump(dumpPath);
        var runtime = target.ClrVersions.FirstOrDefault()?.CreateRuntime();

        if (runtime == null)
        {
            return $"[Sharphy Managed Heap] No CLR runtime found inside dump: {dumpPath}.";
        }

        var heap = runtime.Heap;
        if (!heap.CanWalkHeap)
        {
            return "[Sharphy Managed Heap] Managed heap is corrupted or incomplete in this dump; walking heap aborted.";
        }

        var stats = heap.EnumerateObjects()
            .Where(obj => obj.IsValid)
            .GroupBy(obj => obj.Type?.Name ?? "Unknown")
            .Select(g => new { TypeName = g.Key, Count = g.Count(), TotalSize = g.Sum(o => (long)o.Size) })
            .OrderByDescending(s => s.TotalSize)
            .Take(topObjects)
            .ToList();

        var sw = new StringWriter();
        sw.WriteLine($"=== Managed Heap Object Inspection (Top {topObjects}) ===");
        sw.WriteLine($"Total Segments: {heap.Segments.Length}");
        sw.WriteLine();
        sw.WriteLine(string.Format("{0,-60} | {1,10} | {2,15}", "Type Name", "Count", "Total Bytes"));
        sw.WriteLine(new string('-', 92));

        foreach (var item in stats)
        {
            sw.WriteLine(string.Format("{0,-60} | {1,10} | {2,15:N0}", item.TypeName, item.Count, item.TotalSize));
        }

        // GC Heap Fragmentation & Pinning Telemetry
        sw.WriteLine();
        sw.WriteLine("--- Heap Fragmentation & Pinning Telemetry ---");
        try
        {
            long totalHeapSize = 0;
            long totalFreeSize = 0;

            foreach (var segment in heap.Segments)
            {
                totalHeapSize += (long)segment.Length;
                totalFreeSize += (long)segment.EnumerateObjects()
                    .Where(o => o.IsValid && o.IsFree)
                    .Sum(o => (long)o.Size);
            }

            double fragmentationPct = totalHeapSize > 0 ? ((double)totalFreeSize / totalHeapSize) * 100.0 : 0.0;
            sw.WriteLine($"Total Heap Size: {totalHeapSize:N0} bytes");
            sw.WriteLine($"Free Object Space: {totalFreeSize:N0} bytes");
            sw.WriteLine($"Heap Fragmentation: {fragmentationPct:F2}%");

            if (fragmentationPct > 30.0)
            {
                sw.WriteLine("  ⚠️ High GC Heap Fragmentation detected (> 30%). May lead to OutOfMemoryException during large allocations.");
            }

            // Pinned handles enumeration
            var pinnedHandles = runtime.EnumerateHandles()
                .Where(h => h.HandleKind == ClrHandleKind.Pinned)
                .ToList();

            sw.WriteLine($"Pinned GC Handles Count: {pinnedHandles.Count}");
            if (pinnedHandles.Count > 0)
            {
                sw.WriteLine("Pinned Objects (First 10):");
                foreach (var handle in pinnedHandles.Take(10))
                {
                    var targetObj = handle.Object;
                    var typeName = targetObj.IsValid ? (targetObj.Type?.Name ?? "Unknown") : "Invalid";
                    var seg = heap.GetSegmentByAddress(handle.Address);
                    var genStr = seg != null ? seg.Kind.ToString() : "UnknownGen";
                    sw.WriteLine($"  - [0x{handle.Address:X}] Object: 0x{targetObj.Address:X} ({typeName}) in {genStr}");
                }
            }
            else
            {
                sw.WriteLine("  No pinned handles detected.");
            }
        }
        catch (Exception ex)
        {
            sw.WriteLine($"[Heap Telemetry Notice] Telemetry evaluation skipped: {ex.Message}");
        }

        return sw.ToString();
    }

    public static string DetectDeadlocks(string dumpPath)
    {
        if (!File.Exists(dumpPath))
        {
            return $"[Sharphy Deadlock Detector] Target dump file was not found at '{dumpPath}'.";
        }

        using var target = DataTarget.LoadDump(dumpPath);
        var runtime = target.ClrVersions.FirstOrDefault()?.CreateRuntime();

        if (runtime == null)
        {
            return $"[Sharphy Deadlock Detector] No CLR runtime found inside dump: {dumpPath}.";
        }

        var sw = new StringWriter();
        sw.WriteLine($"=== Synchronization & Deadlock Traversal: {Path.GetFileName(dumpPath)} ===");

        var syncBlocks = runtime.Heap.EnumerateSyncBlocks().ToList();
        sw.WriteLine($"Active SyncBlocks in CLR: {syncBlocks.Count}");

        if (syncBlocks.Count == 0)
        {
            sw.WriteLine("No active lock contention or deadlocks detected across threads.");
        }
        else
        {
            foreach (var sb in syncBlocks)
            {
                sw.WriteLine($"SyncBlock at Object 0x{sb.Object:X}");
            }
        }

        return sw.ToString();
    }
}
