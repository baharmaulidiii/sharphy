using System.Runtime.InteropServices;

namespace MemoryLeakDemo;

/// <summary>
/// Reproducible heap and interop testbed modeling LOH allocations, pinned handles, and unmanaged P/Invoke boundaries.
/// </summary>
public static class Program
{
    private static readonly List<byte[]> RetainedLohBuffers = new();
    private static readonly List<GCHandle> PinnedHandles = new();

    public static void Main(string[] args)
    {
        var count = args.Length > 0 && int.TryParse(args[0], out var parsed) ? Math.Clamp(parsed, 1, 64) : 2;
        AllocatePinnedLohBuffers(count);

        if (OperatingSystem.IsWindows())
        {
            var ticks = NativeMethods.GetTickCount64();
            Console.WriteLine($"MemoryLeakDemo allocated {RetainedLohBuffers.Count} LOH buffers (Win32 Tick: {ticks}).");
        }
        else
        {
            Console.WriteLine($"MemoryLeakDemo allocated {RetainedLohBuffers.Count} LOH buffers.");
        }

        ReleasePinnedHandles();
    }

    public static void AllocatePinnedLohBuffers(int bufferCount)
    {
        const int lohBufferSize = 90_000;
        for (int i = 0; i < bufferCount; i++)
        {
            var buffer = GC.AllocateUninitializedArray<byte>(lohBufferSize, pinned: true);
            buffer[0] = (byte)(i & 0xFF);
            RetainedLohBuffers.Add(buffer);
            PinnedHandles.Add(GCHandle.Alloc(new byte[4096], GCHandleType.Pinned));
        }
    }

    public static void ReleasePinnedHandles()
    {
        foreach (var handle in PinnedHandles)
        {
            if (handle.IsAllocated)
            {
                handle.Free();
            }
        }
        PinnedHandles.Clear();
        RetainedLohBuffers.Clear();
    }
}

internal static class NativeMethods
{
    [DllImport("kernel32.dll", EntryPoint = "GetTickCount64", SetLastError = true)]
    internal static extern ulong GetTickCount64();
}
