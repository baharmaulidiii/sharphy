using Microsoft.Diagnostics.Runtime;

namespace Sharphy.Core.Engines;

/// <summary>
/// Provides crash dump telemetry and managed heap forensic inspection powered by Microsoft ClrMD.
/// </summary>
public static class DumpForensicsEngine
{
    private const int MaxNativeModulesInReport = 15;

    private static readonly string[] SyncBlockingSignatures =
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

    public static string AnalyzeCrashDump(string dumpPath)
    {
        if (!File.Exists(dumpPath))
        {
            return $"[Sharphy Dump Forensics] Target crash dump file was not found at '{dumpPath}'. Make sure to supply an existing .dmp or CoreCLR ELF dump.";
        }

        if (!TryOpenDumpRuntime(
            dumpPath,
            "[Sharphy Dump Forensics]",
            out var target,
            out var runtime,
            out var errorReport,
            includeNativeFallback: true))
        {
            return errorReport!;
        }

        using (target)
        using (runtime)
        {
            var sw = new StringWriter();
            sw.WriteLine($"=== Root Cause Analysis (RCA) Report: {Path.GetFileName(dumpPath)} ===");
            sw.WriteLine($"CLR Flavor: {runtime!.ClrInfo.Flavor}");
            sw.WriteLine($"CLR Version: {runtime.ClrInfo.Version}");
            sw.WriteLine($"Server GC: {SafeRead(() => runtime.Heap.IsServer.ToString(), "Unavailable (partial dump)")}");
            sw.WriteLine($"Managed Threads Count: {runtime.Threads.Length}");
            sw.WriteLine();

            WriteThreadsAndExceptions(sw, runtime);
            WriteThreadPoolTriage(sw, runtime);
            WriteUnmanagedModulesSummary(sw, target!);

            return sw.ToString();
        }
    }

    public static string InspectHeap(string dumpPath, int topObjects = 20)
    {
        if (!File.Exists(dumpPath))
        {
            return $"[Sharphy Managed Heap] Target dump file was not found at '{dumpPath}'.";
        }

        if (!TryOpenDumpRuntime(dumpPath, "[Sharphy Managed Heap]", out var target, out var runtime, out var errorReport))
        {
            return errorReport!;
        }

        using (target)
        using (runtime)
        {
            var heap = runtime!.Heap;
            if (!heap.CanWalkHeap)
            {
                return "[Sharphy Managed Heap Warning] Managed heap is corrupted or incomplete in this dump (missing heap segments); walking heap aborted.";
            }

            try
            {
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
                catch (Exception ex) when (ex is ClrDiagnosticsException or InvalidOperationException)
                {
                    sw.WriteLine($"[Heap Telemetry Warning] Partial dump prevented full fragmentation telemetry: {ex.Message}");
                }

                return sw.ToString();
            }
            catch (Exception ex) when (ex is ClrDiagnosticsException or InvalidOperationException)
            {
                return $"[Sharphy Managed Heap Warning] Heap traversal aborted due to partial or corrupted dump memory: {ex.Message}";
            }
        }
    }

    public static string DetectDeadlocks(string dumpPath)
    {
        if (!File.Exists(dumpPath))
        {
            return $"[Sharphy Deadlock Detector] Target dump file was not found at '{dumpPath}'.";
        }

        if (!TryOpenDumpRuntime(dumpPath, "[Sharphy Deadlock Detector]", out var target, out var runtime, out var errorReport))
        {
            return errorReport!;
        }

        using (target)
        using (runtime)
        {
            var sw = new StringWriter();
            sw.WriteLine($"=== Synchronization & Deadlock Traversal: {Path.GetFileName(dumpPath)} ===");

            if (!runtime!.Heap.CanWalkHeap)
            {
                sw.WriteLine("[Deadlock Detector Warning] Managed heap cannot be walked in this minidump; SyncBlock traversal skipped.");
                return sw.ToString();
            }

            try
            {
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
            }
            catch (Exception ex) when (ex is ClrDiagnosticsException or InvalidOperationException)
            {
                sw.WriteLine($"[Deadlock Detector Warning] SyncBlock enumeration failed due to incomplete dump memory: {ex.Message}");
            }

            return sw.ToString();
        }
    }

    private static bool TryOpenDumpRuntime(
        string dumpPath,
        string toolTag,
        out DataTarget? target,
        out ClrRuntime? runtime,
        out string? errorReport,
        bool includeNativeFallback = false)
    {
        target = null;
        runtime = null;
        errorReport = null;

        FileStream? dumpStream = null;
        try
        {
            var fileInfo = new FileInfo(dumpPath);
            if (fileInfo.Length == 0)
            {
                errorReport = $"{toolTag} Warning: Dump file '{Path.GetFileName(dumpPath)}' is empty (0 bytes) and cannot be parsed.";
                return false;
            }

            // Own the FileStream explicitly so failed LoadDump calls on truncated dumps never leak OS file locks.
            dumpStream = new FileStream(dumpPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
            target = DataTarget.LoadDump(dumpPath, dumpStream, new CacheOptions(), leaveOpen: false);
            dumpStream = null; // Ownership transferred to DataTarget
        }
        catch (Exception ex) when (ex is ClrDiagnosticsException or InvalidDataException or IOException or UnauthorizedAccessException or NotSupportedException or BadImageFormatException)
        {
            dumpStream?.Dispose();
            target?.Dispose();
            target = null;
            errorReport = $"{toolTag} Warning: Unable to load dump '{Path.GetFileName(dumpPath)}' (truncated, corrupted, or unsupported format): {ex.Message}";
            return false;
        }

        var clrInfo = target.ClrVersions.FirstOrDefault();
        if (clrInfo == null)
        {
            var sw = new StringWriter();
            sw.WriteLine($"{toolTag} Warning: No supported CLR runtime found inside '{Path.GetFileName(dumpPath)}' (native-only or partial minidump).");
            if (includeNativeFallback)
            {
                WriteUnmanagedModulesSummary(sw, target);
            }

            target.Dispose();
            target = null;
            errorReport = sw.ToString();
            return false;
        }

        try
        {
            runtime = clrInfo.CreateRuntime();
            return true;
        }
        catch (Exception ex) when (ex is ClrDiagnosticsException or InvalidOperationException or FileNotFoundException or IOException)
        {
            var sw = new StringWriter();
            sw.WriteLine($"{toolTag} Warning: CLR runtime initialization failed for '{Path.GetFileName(dumpPath)}' (missing DAC or truncated CLR metadata): {ex.Message}");
            if (includeNativeFallback)
            {
                WriteUnmanagedModulesSummary(sw, target);
            }

            target.Dispose();
            target = null;
            errorReport = sw.ToString();
            return false;
        }
    }

    private static void WriteThreadsAndExceptions(StringWriter sw, ClrRuntime runtime)
    {
        sw.WriteLine("--- Threads & Exceptions ---");
        foreach (var thread in runtime.Threads.Where(t => t.IsAlive))
        {
            sw.WriteLine($"[Thread #{thread.ManagedThreadId}] OS Thread ID: 0x{thread.OSThreadId:X4}");
            try
            {
                var ex = thread.CurrentException;
                if (ex != null)
                {
                    sw.WriteLine($"  🔥 Crashing Exception: {ex.Type?.Name ?? "UnknownException"}: {ex.Message ?? "<no message>"}");
                    sw.WriteLine($"  HRESULT: 0x{ex.HResult:X8}");
                }
            }
            catch (ClrDiagnosticsException ex)
            {
                sw.WriteLine($"  [Partial Dump Warning] Could not read thread exception record: {ex.Message}");
            }
        }
    }

    private static void WriteThreadPoolTriage(StringWriter sw, ClrRuntime runtime)
    {
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
            var hybridThreads = new List<(int ManagedThreadId, string RuntimeFrame)>();
            var partialStackThreads = 0;

            foreach (var thread in runtime.Threads.Where(t => t.IsAlive))
            {
                try
                {
                    string? firstRuntimeFrame = null;
                    bool recordedBlocked = false;

                    foreach (var frame in thread.EnumerateStackTrace())
                    {
                        var display = frame.FrameName ?? frame.Method?.Signature ?? string.Empty;

                        if (firstRuntimeFrame == null && frame.Kind == ClrStackFrameKind.Runtime && !string.IsNullOrWhiteSpace(display))
                        {
                            firstRuntimeFrame = display;
                        }

                        if (!recordedBlocked)
                        {
                            var matchedSig = SyncBlockingSignatures.FirstOrDefault(sig =>
                                display.Contains(sig, StringComparison.OrdinalIgnoreCase));
                            if (matchedSig != null)
                            {
                                blockedThreads.Add((thread.ManagedThreadId, matchedSig, display));
                                recordedBlocked = true;
                            }
                        }
                    }

                    if (firstRuntimeFrame != null)
                    {
                        hybridThreads.Add((thread.ManagedThreadId, firstRuntimeFrame));
                    }
                }
                catch (ClrDiagnosticsException)
                {
                    partialStackThreads++;
                }
            }

            if (partialStackThreads > 0)
            {
                sw.WriteLine($"[Partial Stack Warning] Stack memory unavailable for {partialStackThreads} managed thread(s).");
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

            if (hybridThreads.Count > 0)
            {
                sw.WriteLine($"[Hybrid Interop Transitions] {hybridThreads.Count} thread(s) contain native/CLR transition frames:");
                foreach (var (threadId, runtimeFrame) in hybridThreads.Take(10))
                {
                    sw.WriteLine($"  - Thread #{threadId}: {runtimeFrame}");
                }
            }
        }
        catch (Exception ex) when (ex is ClrDiagnosticsException or InvalidOperationException)
        {
            sw.WriteLine($"[ThreadPool Inspection Warning] Telemetry evaluation skipped: {ex.Message}");
        }
    }

    private static void WriteUnmanagedModulesSummary(StringWriter sw, DataTarget target)
    {
        sw.WriteLine();
        sw.WriteLine("--- Unmanaged & Hybrid Native Modules ---");
        try
        {
            var nativeModules = target.EnumerateModules()
                .Where(m => !m.IsManaged)
                .OrderBy(m => m.ImageBase)
                .ToList();

            sw.WriteLine($"Native Modules Loaded: {nativeModules.Count}");
            foreach (var mod in nativeModules.Take(MaxNativeModulesInReport))
            {
                var name = string.IsNullOrWhiteSpace(mod.FileName) ? "<unknown>" : Path.GetFileName(mod.FileName);
                sw.WriteLine($"  - {name} (Base: 0x{mod.ImageBase:X}, Size: 0x{mod.ImageSize:X}, Kind: {mod.Kind})");
            }

            if (nativeModules.Count > MaxNativeModulesInReport)
            {
                sw.WriteLine($"  ... ({nativeModules.Count - MaxNativeModulesInReport} additional native modules omitted for token efficiency)");
            }
        }
        catch (Exception ex) when (ex is ClrDiagnosticsException or InvalidOperationException)
        {
            sw.WriteLine($"[Native Module Warning] Unmanaged module enumeration unavailable: {ex.Message}");
        }
    }

    private static string SafeRead(Func<string> getter, string fallback)
    {
        try
        {
            return getter();
        }
        catch (ClrDiagnosticsException)
        {
            return fallback;
        }
    }
}
