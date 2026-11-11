using System.Globalization;
using Microsoft.Diagnostics.Runtime;

namespace Sharphy.Core.Engines;

/// <summary>
/// Provides crash dump telemetry and managed heap forensic inspection powered by Microsoft ClrMD.
/// </summary>
public static class DumpForensicsEngine
{
    private const int MaxNativeModulesInReport = 15;
    private const int MaxThreadsInReport = 30;
    private const int MaxExceptionFramesInReport = 15;

    private static readonly string[] SyncBlockingSignatures =
    {
        "System.Threading.Tasks.Task.Wait",
        "System.Runtime.CompilerServices.TaskAwaiter.GetResult",
        "System.Runtime.CompilerServices.TaskAwaiter`1.GetResult",
        "System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter.GetResult",
        "System.Runtime.CompilerServices.ConfiguredTaskAwaitable`1.ConfiguredTaskAwaiter.GetResult",
        "System.Runtime.CompilerServices.ValueTaskAwaiter.GetResult",
        "System.Runtime.CompilerServices.ValueTaskAwaiter`1.GetResult",
        "System.Threading.Monitor.Enter",
        "System.Threading.Monitor.Wait",
        "System.Threading.ManualResetEventSlim.Wait",
        "System.Threading.SemaphoreSlim.Wait",
        "System.Threading.ReaderWriterLockSlim.Enter",
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
        topObjects = Math.Clamp(topObjects, 1, 500);

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
                sw.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0,-60} | {1,10} | {2,15}", "Type Name", "Count", "Total Bytes"));
                sw.WriteLine(new string('-', 92));

                foreach (var item in stats)
                {
                    sw.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0,-60} | {1,10} | {2,15:N0}", item.TypeName, item.Count, item.TotalSize));
                }

                // GC Heap Fragmentation, LOH/POH Compaction Risk & Pinning Telemetry
                sw.WriteLine();
                sw.WriteLine("--- Heap Fragmentation & LOH/POH Compaction Telemetry ---");
                try
                {
                    long totalHeapSize = 0;
                    long totalFreeSize = 0;
                    long lohTotalSize = 0;
                    long lohFreeSize = 0;
                    long pohTotalSize = 0;
                    long pohFreeSize = 0;
                    long gen2TotalSize = 0;
                    long gen2FreeSize = 0;

                    foreach (var segment in heap.Segments)
                    {
                        var segLen = (long)segment.Length;
                        var segFree = segment.EnumerateObjects()
                            .Where(o => o.IsValid && o.IsFree)
                            .Sum(o => (long)o.Size);

                        totalHeapSize += segLen;
                        totalFreeSize += segFree;

                        if (segment.Kind == GCSegmentKind.Large)
                        {
                            lohTotalSize += segLen;
                            lohFreeSize += segFree;
                        }
                        else if (segment.Kind == GCSegmentKind.Pinned || segment.IsPinned)
                        {
                            pohTotalSize += segLen;
                            pohFreeSize += segFree;
                        }
                        else if (segment.Kind == GCSegmentKind.Generation2)
                        {
                            gen2TotalSize += segLen;
                            gen2FreeSize += segFree;
                        }
                    }

                    double fragmentationPct = totalHeapSize > 0 ? ((double)totalFreeSize / totalHeapSize) * 100.0 : 0.0;
                    double lohFragPct = lohTotalSize > 0 ? ((double)lohFreeSize / lohTotalSize) * 100.0 : 0.0;
                    double pohFragPct = pohTotalSize > 0 ? ((double)pohFreeSize / pohTotalSize) * 100.0 : 0.0;
                    double gen2FragPct = gen2TotalSize > 0 ? ((double)gen2FreeSize / gen2TotalSize) * 100.0 : 0.0;

                    sw.WriteLine(string.Format(CultureInfo.InvariantCulture, "Total Heap Size: {0:N0} bytes | Free: {1:N0} bytes ({2:F2}%)", totalHeapSize, totalFreeSize, fragmentationPct));
                    sw.WriteLine(string.Format(CultureInfo.InvariantCulture, "LOH Size: {0:N0} bytes (Fragmentation: {1:F2}%)", lohTotalSize, lohFragPct));
                    sw.WriteLine(string.Format(CultureInfo.InvariantCulture, "POH Size: {0:N0} bytes (Fragmentation: {1:F2}%)", pohTotalSize, pohFragPct));
                    if (gen2TotalSize > 0)
                    {
                        sw.WriteLine(string.Format(CultureInfo.InvariantCulture, "Gen2 Size: {0:N0} bytes (Fragmentation: {1:F2}%)", gen2TotalSize, gen2FragPct));
                    }

                    // Pinned & AsyncPinned handles enumeration
                    var pinnedHandles = runtime.EnumerateHandles()
                        .Where(h => h.HandleKind == ClrHandleKind.Pinned || h.HandleKind == ClrHandleKind.AsyncPinned)
                        .ToList();

                    int ephemeralPinnedCount = 0;
                    int lohPinnedCount = 0;
                    int pohPinnedCount = 0;
                    int gen2PinnedCount = 0;

                    foreach (var handle in pinnedHandles)
                    {
                        var targetAddr = handle.Object.IsValid && handle.Object.Address != 0
                            ? handle.Object.Address
                            : handle.Address;
                        var seg = heap.GetSegmentByAddress(targetAddr);
                        if (seg == null) continue;

                        if (seg.Kind == GCSegmentKind.Large)
                        {
                            lohPinnedCount++;
                        }
                        else if (seg.Kind == GCSegmentKind.Pinned || seg.IsPinned)
                        {
                            pohPinnedCount++;
                        }
                        else if (seg.Kind == GCSegmentKind.Generation2 ||
                                 (seg.Kind == GCSegmentKind.Ephemeral &&
                                  targetAddr >= seg.Generation2.Start &&
                                  targetAddr < seg.Generation2.End))
                        {
                            gen2PinnedCount++;
                        }
                        else
                        {
                            ephemeralPinnedCount++;
                        }
                    }

                    var (riskScore, riskSeverity) = ComputeCompactionRiskScore(lohFragPct, pohFragPct, lohPinnedCount, gen2PinnedCount);
                    sw.WriteLine($"Compaction Resistance Score: {riskScore}/100 ({riskSeverity})");
                    sw.WriteLine($"Pinned Handles: {pinnedHandles.Count} total (Gen0/Gen1: {ephemeralPinnedCount}, Gen2: {gen2PinnedCount}, LOH: {lohPinnedCount}, POH: {pohPinnedCount})");

                    if (ephemeralPinnedCount > 0)
                    {
                        sw.WriteLine($"  ⚠️ {ephemeralPinnedCount} ephemeral (Gen0/Gen1) pinned handle(s) detected; pinning young-generation objects blocks generational compaction.");
                    }

                    if (fragmentationPct > 30.0 || riskScore >= 50)
                    {
                        sw.WriteLine("  ⚠️ Elevated GC heap fragmentation or pinning resistance detected. May cause OutOfMemoryException during large allocations.");
                    }

                    if (pinnedHandles.Count > 0)
                    {
                        sw.WriteLine("Pinned Objects (First 10):");
                        foreach (var handle in pinnedHandles.Take(10))
                        {
                            var targetObj = handle.Object;
                            var typeName = targetObj.IsValid ? (targetObj.Type?.Name ?? "Unknown") : "Invalid";
                            var lookupAddr = targetObj.IsValid && targetObj.Address != 0 ? targetObj.Address : handle.Address;
                            var seg = heap.GetSegmentByAddress(lookupAddr);
                            var genStr = seg != null ? seg.Kind.ToString() : "UnknownGen";
                            sw.WriteLine($"  - [{handle.HandleKind} 0x{handle.Address:X}] Object: 0x{targetObj.Address:X} ({typeName}) in {genStr}");
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
                var threadByAddress = runtime.Threads
                    .Where(t => t.Address != 0)
                    .GroupBy(t => t.Address)
                    .ToDictionary(g => g.Key, g => g.First());

                var heldSyncBlocks = syncBlocks.Where(sb => sb.IsMonitorHeld).ToList();
                sw.WriteLine($"Active SyncBlocks in CLR: {syncBlocks.Count} (Monitor-Held: {heldSyncBlocks.Count})");

                var locksByObject = new Dictionary<ulong, (int OwnerThreadId, string ObjectType, SyncBlock Block)>();
                foreach (var sb in heldSyncBlocks)
                {
                    if (sb.HoldingThreadAddress != 0 && threadByAddress.TryGetValue(sb.HoldingThreadAddress, out var owner))
                    {
                        var obj = runtime.Heap.GetObject(sb.Object);
                        var typeName = obj.IsValid ? (obj.Type?.Name ?? "System.Object") : "UnknownObject";
                        locksByObject[sb.Object] = (owner.ManagedThreadId, typeName, sb);
                    }
                }

                var waitForGraph = new Dictionary<int, List<int>>();
                var waitEdges = new List<(int WaiterThreadId, int OwnerThreadId, ulong ObjectAddress, string ObjectType)>();
                var waitSignatures = new[]
                {
                    "System.Threading.Monitor.Enter",
                    "System.Threading.Monitor.ReliableEnter",
                    "System.Threading.Monitor.Wait",
                    "AwareLock::Enter",
                    "HelperMethodFrame_1OBJ",
                    "JIT_MonEnter",
                    "JIT_MonReliableEnter",
                    "System.Threading.WaitHandle.WaitOne"
                };

                foreach (var thread in runtime.Threads.Where(t => t.IsAlive))
                {
                    bool isWaitingOnSync = false;
                    try
                    {
                        foreach (var frame in thread.EnumerateStackTrace())
                        {
                            var display = frame.FrameName ?? frame.Method?.Signature ?? string.Empty;
                            if (waitSignatures.Any(sig => display.Contains(sig, StringComparison.OrdinalIgnoreCase)))
                            {
                                isWaitingOnSync = true;
                                break;
                            }
                        }

                        if (!isWaitingOnSync || locksByObject.Count == 0)
                        {
                            continue;
                        }

                        var candidateEdges = new List<(int OwnerThreadId, ulong ObjectAddress, string ObjectType, int WaitingCount)>();
                        foreach (var root in thread.EnumerateStackRoots())
                        {
                            var addr = root.Object.Address;
                            if (addr != 0 && locksByObject.TryGetValue(addr, out var held) && held.OwnerThreadId != thread.ManagedThreadId)
                            {
                                candidateEdges.Add((held.OwnerThreadId, addr, held.ObjectType, held.Block.WaitingThreadCount));
                            }
                        }

                        // Prefer SyncBlocks that explicitly report waiting threads when available
                        var preferredEdges = candidateEdges.Any(e => e.WaitingCount > 0)
                            ? candidateEdges.Where(e => e.WaitingCount > 0)
                            : candidateEdges;

                        foreach (var held in preferredEdges)
                        {
                            if (!waitForGraph.TryGetValue(thread.ManagedThreadId, out var owners))
                            {
                                owners = new List<int>();
                                waitForGraph[thread.ManagedThreadId] = owners;
                            }

                            if (!owners.Contains(held.OwnerThreadId))
                            {
                                owners.Add(held.OwnerThreadId);
                                waitEdges.Add((thread.ManagedThreadId, held.OwnerThreadId, held.ObjectAddress, held.ObjectType));
                            }
                        }
                    }
                    catch (ClrDiagnosticsException)
                    {
                        // Partial stack memory on a thread should not abort global WFG traversal.
                    }
                }

                sw.WriteLine();
                sw.WriteLine("--- Wait-For Graph (WFG) & Cycle Analysis ---");
                var wfgReadOnly = waitForGraph.ToDictionary(kvp => kvp.Key, kvp => (IReadOnlyList<int>)kvp.Value);
                var cycles = FindDeadlockCycles(wfgReadOnly);

                if (cycles.Count > 0)
                {
                    sw.WriteLine($"[🚨 DEADLOCK CYCLE DETECTED] Identified {cycles.Count} circular Wait-For cycle(s):");
                    foreach (var cycle in cycles)
                    {
                        sw.WriteLine($"  - Cycle: {string.Join(" -> ", cycle.Select(id => $"Thread #{id}"))}");
                    }
                }
                else if (waitEdges.Count > 0)
                {
                    sw.WriteLine($"[⚠️ Lock Contention Detected] {waitEdges.Count} Wait-For edge(s) without circular cycle:");
                    foreach (var edge in waitEdges.Take(15))
                    {
                        sw.WriteLine($"  - Thread #{edge.WaiterThreadId} waiting on Thread #{edge.OwnerThreadId} (Object 0x{edge.ObjectAddress:X} [{edge.ObjectType}])");
                    }
                }
                else if (heldSyncBlocks.Count == 0)
                {
                    sw.WriteLine("No active lock contention or deadlocks detected across threads.");
                }
                else
                {
                    sw.WriteLine("Monitor locks held without circular deadlock cycles.");
                }

                if (heldSyncBlocks.Count > 0)
                {
                    sw.WriteLine();
                    sw.WriteLine("--- Held Monitor SyncBlocks (Top 15) ---");
                    foreach (var sb in heldSyncBlocks.Take(15))
                    {
                        var ownerStr = sb.HoldingThreadAddress != 0 && threadByAddress.TryGetValue(sb.HoldingThreadAddress, out var owner)
                            ? $"Thread #{owner.ManagedThreadId}"
                            : $"0x{sb.HoldingThreadAddress:X}";
                        var obj = runtime.Heap.GetObject(sb.Object);
                        var typeName = obj.IsValid ? (obj.Type?.Name ?? "System.Object") : "UnknownObject";
                        sw.WriteLine($"  - Object 0x{sb.Object:X} ({typeName}) | Owner: {ownerStr} | Recursion: {sb.RecursionCount} | Waiters: {sb.WaitingThreadCount}");
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

    /// <summary>
    /// Traverses a directed Wait-For Graph (WaiterThreadId -> OwnerThreadIds) using DFS
    /// and returns unique normalized circular deadlock cycles.
    /// </summary>
    public static IReadOnlyList<IReadOnlyList<int>> FindDeadlockCycles(IReadOnlyDictionary<int, IReadOnlyList<int>> waitForGraph)
    {
        var cycles = new List<IReadOnlyList<int>>();
        var seenSignatures = new HashSet<string>(StringComparer.Ordinal);
        var state = new Dictionary<int, int>(); // 0/absent = unvisited, 1 = visiting, 2 = visited
        var stack = new List<int>();

        foreach (var startNode in waitForGraph.Keys.OrderBy(k => k))
        {
            if (state.GetValueOrDefault(startNode) == 0)
            {
                DfsFindCycles(startNode, waitForGraph, state, stack, cycles, seenSignatures);
            }
        }

        return cycles;
    }

    private static void DfsFindCycles(
        int node,
        IReadOnlyDictionary<int, IReadOnlyList<int>> graph,
        Dictionary<int, int> state,
        List<int> stack,
        List<IReadOnlyList<int>> cycles,
        HashSet<string> seenSignatures)
    {
        state[node] = 1;
        stack.Add(node);

        if (graph.TryGetValue(node, out var neighbors))
        {
            foreach (var next in neighbors)
            {
                var nextState = state.GetValueOrDefault(next);
                if (nextState == 1)
                {
                    var idx = stack.IndexOf(next);
                    if (idx >= 0)
                    {
                        var rawCycle = stack.Skip(idx).ToList();
                        var minIdx = 0;
                        for (int i = 1; i < rawCycle.Count; i++)
                        {
                            if (rawCycle[i] < rawCycle[minIdx]) minIdx = i;
                        }

                        var normalized = new List<int>(rawCycle.Count + 1);
                        for (int i = 0; i < rawCycle.Count; i++)
                        {
                            normalized.Add(rawCycle[(minIdx + i) % rawCycle.Count]);
                        }
                        normalized.Add(normalized[0]);

                        var signature = string.Join("->", normalized);
                        if (seenSignatures.Add(signature))
                        {
                            cycles.Add(normalized);
                        }
                    }
                }
                else if (nextState == 0 && graph.ContainsKey(next))
                {
                    DfsFindCycles(next, graph, state, stack, cycles, seenSignatures);
                }
            }
        }

        stack.RemoveAt(stack.Count - 1);
        state[node] = 2;
    }

    /// <summary>
    /// Computes a bounded 0-100 GC compaction resistance risk score from LOH/POH fragmentation and pinned handles.
    /// </summary>
    public static (int Score, string Severity) ComputeCompactionRiskScore(
        double lohFragmentationPct,
        double pohFragmentationPct,
        int lohPinnedCount,
        int gen2PinnedCount)
    {
        var clampedLohFrag = Math.Clamp(lohFragmentationPct, 0.0, 100.0);
        var clampedPohFrag = Math.Clamp(pohFragmentationPct, 0.0, 100.0);

        double rawScore =
            Math.Min(40.0, clampedLohFrag * 0.8) +
            Math.Min(20.0, clampedPohFrag * 0.4) +
            Math.Min(25.0, Math.Max(0, lohPinnedCount) * 5.0) +
            Math.Min(15.0, Math.Max(0, gen2PinnedCount) * 3.0);

        int score = Math.Clamp((int)Math.Round(rawScore, MidpointRounding.AwayFromZero), 0, 100);
        string severity = score switch
        {
            >= 75 => "CRITICAL",
            >= 50 => "HIGH",
            >= 25 => "MODERATE",
            _ => "LOW"
        };

        return (score, severity);
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
        var aliveThreads = runtime.Threads.Where(t => t.IsAlive).ToList();
        var orderedThreads = aliveThreads
            .OrderByDescending(t => SafeRead(() => t.CurrentException != null ? "1" : "0", "0") == "1")
            .ThenBy(t => t.ManagedThreadId)
            .ToList();

        int printedThreads = 0;
        foreach (var thread in orderedThreads)
        {
            ClrException? ex = null;
            string? exceptionReadError = null;
            try
            {
                ex = thread.CurrentException;
            }
            catch (ClrDiagnosticsException readEx)
            {
                exceptionReadError = readEx.Message;
            }

            if (printedThreads >= MaxThreadsInReport && ex == null)
            {
                continue;
            }

            sw.WriteLine($"[Thread #{thread.ManagedThreadId}] OS Thread ID: 0x{thread.OSThreadId:X4}");
            printedThreads++;

            if (exceptionReadError != null)
            {
                sw.WriteLine($"  [Partial Dump Warning] Could not read thread exception record: {exceptionReadError}");
                continue;
            }

            if (ex != null)
            {
                sw.WriteLine($"  🔥 Crashing Exception: {ex.Type?.Name ?? "UnknownException"}: {ex.Message ?? "<no message>"}");
                sw.WriteLine($"  HRESULT: 0x{ex.HResult:X8}");

                try
                {
                    var frames = thread.EnumerateStackTrace()
                        .Select(f => f.Method?.Signature ?? f.FrameName)
                        .Where(s => !string.IsNullOrWhiteSpace(s))
                        .Take(MaxExceptionFramesInReport)
                        .ToList();

                    if (frames.Count > 0)
                    {
                        sw.WriteLine("  Crashing Call Stack:");
                        foreach (var frame in frames)
                        {
                            sw.WriteLine($"    at {frame}");
                        }
                    }
                }
                catch (ClrDiagnosticsException stackEx)
                {
                    sw.WriteLine($"  [Partial Stack Warning] Could not unwind full exception stack: {stackEx.Message}");
                }
            }
        }

        if (orderedThreads.Count > printedThreads)
        {
            sw.WriteLine($"  ... ({orderedThreads.Count - printedThreads} additional alive threads omitted for token efficiency)");
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

                    if (!recordedBlocked && firstRuntimeFrame != null &&
                        (firstRuntimeFrame.Contains("HelperMethodFrame_1OBJ", StringComparison.OrdinalIgnoreCase) ||
                         firstRuntimeFrame.Contains("JIT_MonEnter", StringComparison.OrdinalIgnoreCase) ||
                         firstRuntimeFrame.Contains("JIT_MonReliableEnter", StringComparison.OrdinalIgnoreCase)))
                    {
                        blockedThreads.Add((thread.ManagedThreadId, "System.Threading.Monitor.Enter", firstRuntimeFrame));
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
