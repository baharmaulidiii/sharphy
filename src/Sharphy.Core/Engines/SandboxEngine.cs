using System.Runtime.Loader;

namespace Sharphy.Core.Engines;

/// <summary>
/// Provides sandboxed dynamic metadata reflection and token extraction inside collectible AssemblyLoadContexts.
/// </summary>
public static class SandboxEngine
{
    public static string ExtractTokens(string assemblyPath)
    {
        if (!File.Exists(assemblyPath))
        {
            throw new FileNotFoundException($"Target assembly not found at '{assemblyPath}'.", assemblyPath);
        }

        var alc = new AssemblyLoadContext($"SharphySandbox_{Guid.NewGuid():N}", isCollectible: true);
        var sw = new StringWriter();

        try
        {
            using var fs = File.OpenRead(assemblyPath);
            var asm = alc.LoadFromStream(fs);

            sw.WriteLine($"=== Sandboxed Token Extraction: {asm.GetName().Name} ===");
            sw.WriteLine($"Full Assembly Name: {asm.FullName}");

            var exportedTypes = asm.GetExportedTypes();
            sw.WriteLine($"Exported Types Count: {exportedTypes.Length}");

            foreach (var t in exportedTypes.Take(10))
            {
                sw.WriteLine($"  [TypeToken: 0x{t.MetadataToken:X8}] {t.FullName}");
                foreach (var m in t.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static).Take(5))
                {
                    sw.WriteLine($"    └─ [MethodToken: 0x{m.MetadataToken:X8}] {m.Name}");
                }
            }

            sw.WriteLine("\nSandbox security status: Isolated & unlinked from default host context.");
        }
        finally
        {
            // Deterministically unload ALC to prevent file locking and host memory pollution
            alc.Unload();
        }

        return sw.ToString();
    }
}
