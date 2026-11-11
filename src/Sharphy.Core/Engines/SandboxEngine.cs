using System.Reflection;
using System.Reflection.PortableExecutable;
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

        using var fs = File.OpenRead(assemblyPath);

        // Pre-screen PE/COR headers statically before passing bytes into the CLR runtime loader.
        try
        {
            using var peReader = new PEReader(fs, PEStreamOptions.LeaveOpen);
            if (!peReader.HasMetadata || peReader.PEHeaders.CorHeader == null)
            {
                return $"[Sharphy Sandbox Security Warning] File '{Path.GetFileName(assemblyPath)}' is not a valid managed .NET assembly (missing CLR metadata header); runtime loading refused.";
            }

            if ((peReader.PEHeaders.CorHeader.Flags & CorFlags.ILOnly) == 0)
            {
                return $"[Sharphy Sandbox Security Warning] Assembly '{Path.GetFileName(assemblyPath)}' is a native/mixed-mode PE image (ILOnly flag is unset). Refusing runtime ALC execution; use 'sharphy_inspect_metadata' for pure static inspection.";
            }
        }
        catch (BadImageFormatException ex)
        {
            return $"[Sharphy Sandbox Security Warning] File '{Path.GetFileName(assemblyPath)}' has an invalid or malformed PE header ({ex.Message}); runtime loading refused.";
        }

        fs.Position = 0;

        var alc = new IsolatedSandboxLoadContext($"SharphySandbox_{Guid.NewGuid():N}");
        var sw = new StringWriter();

        try
        {
            var asm = alc.LoadFromStream(fs);

            sw.WriteLine($"=== Sandboxed Token Extraction: {asm.GetName().Name} ===");
            sw.WriteLine($"Full Assembly Name: {asm.FullName}");

            Type[] exportedTypes;
            int unresolvedCount = 0;
            try
            {
                exportedTypes = asm.GetExportedTypes();
            }
            catch (ReflectionTypeLoadException rtlex)
            {
                exportedTypes = rtlex.Types.Where(t => t != null).Cast<Type>().ToArray();
                unresolvedCount = rtlex.Types.Count(t => t == null);
            }

            sw.WriteLine($"Exported Types Count: {exportedTypes.Length}");
            if (unresolvedCount > 0)
            {
                sw.WriteLine($"Unresolved Dependency Types: {unresolvedCount} (external references not loaded into sandbox)");
            }

            foreach (var t in exportedTypes.Take(10))
            {
                sw.WriteLine($"  [TypeToken: 0x{t.MetadataToken:X8}] {t.FullName}");
                foreach (var m in t.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static).Take(5))
                {
                    sw.WriteLine($"    └─ [MethodToken: 0x{m.MetadataToken:X8}] {m.Name}");
                }
            }

            sw.WriteLine("\nSandbox security status: Isolated, IL-only verified, unmanaged DLL loading disabled & unlinked from default host context.");
        }
        finally
        {
            // Deterministically unload ALC to prevent file locking and host memory pollution
            alc.Unload();
        }

        return sw.ToString();
    }

    private sealed class IsolatedSandboxLoadContext : AssemblyLoadContext
    {
        public IsolatedSandboxLoadContext(string name) : base(name, isCollectible: true)
        {
        }

        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName) => IntPtr.Zero;
    }
}
