using Microsoft.Diagnostics.Runtime;

namespace Sharphy.Core.Engines;

/// <summary>
/// Engine analisis forensik crash dump dan managed heap inspeksi berbasis ClrMD.
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
