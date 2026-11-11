using Mono.Cecil;

namespace Sharphy.Core.Engines;

/// <summary>
/// Shared Mono.Cecil helpers for safe assembly loading, atomic persistence, nested type discovery, and identifier sanitization.
/// </summary>
internal static class CecilHelpers
{
    public static AssemblyDefinition ReadAssemblyReadOnly(string assemblyPath)
    {
        if (!File.Exists(assemblyPath))
        {
            throw new FileNotFoundException($"Assembly file was not found at '{assemblyPath}'.", assemblyPath);
        }

        return AssemblyDefinition.ReadAssembly(assemblyPath, new ReaderParameters
        {
            ReadWrite = false,
            ReadSymbols = false
        });
    }

    public static void WriteAssemblyAvoidingLock(AssemblyDefinition assembly, string sourcePath, string outputPath)
    {
        var fullSource = Path.GetFullPath(sourcePath);
        var fullOutput = Path.GetFullPath(outputPath);

        var outDir = Path.GetDirectoryName(fullOutput);
        if (!string.IsNullOrEmpty(outDir) && !Directory.Exists(outDir))
        {
            Directory.CreateDirectory(outDir);
        }

        if (string.Equals(fullSource, fullOutput, StringComparison.OrdinalIgnoreCase))
        {
            var tempPath = Path.Combine(outDir ?? Path.GetTempPath(), $".sharphy_tmp_{Guid.NewGuid():N}_{Path.GetFileName(fullOutput)}");
            try
            {
                assembly.Write(tempPath);
                assembly.Dispose();
                File.Move(tempPath, fullOutput, overwrite: true);
            }
            catch
            {
                if (File.Exists(tempPath))
                {
                    try { File.Delete(tempPath); } catch { }
                }
                throw;
            }
        }
        else
        {
            assembly.Write(fullOutput);
        }
    }

    public static IEnumerable<TypeDefinition> EnumerateAllTypes(ModuleDefinition module, bool includeModuleType = false)
    {
        foreach (var type in module.Types)
        {
            if (ShouldSkipType(type, includeModuleType)) continue;

            yield return type;

            foreach (var nested in EnumerateNestedTypes(type, includeModuleType))
            {
                yield return nested;
            }
        }
    }

    public static TypeDefinition? FindType(ModuleDefinition module, string typeName)
    {
        var normalizedQuery = typeName.Replace('+', '/');

        foreach (var type in EnumerateAllTypes(module, includeModuleType: true))
        {
            if (type.FullName.Equals(normalizedQuery, StringComparison.OrdinalIgnoreCase) ||
                type.FullName.Replace('/', '.').Equals(typeName, StringComparison.OrdinalIgnoreCase) ||
                type.Name.Equals(typeName, StringComparison.OrdinalIgnoreCase))
            {
                return type;
            }
        }

        return null;
    }

    public static string SanitizeCSharpIdentifier(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return "_";

        Span<char> buffer = stackalloc char[raw.Length];
        for (int i = 0; i < raw.Length; i++)
        {
            char c = raw[i];
            buffer[i] = char.IsLetterOrDigit(c) || c == '_' ? c : '_';
        }

        var result = new string(buffer);
        return char.IsDigit(result[0]) ? "_" + result : result;
    }

    private static IEnumerable<TypeDefinition> EnumerateNestedTypes(TypeDefinition parent, bool includeModuleType)
    {
        if (!parent.HasNestedTypes) yield break;

        foreach (var nested in parent.NestedTypes)
        {
            if (ShouldSkipType(nested, includeModuleType)) continue;

            yield return nested;

            foreach (var deep in EnumerateNestedTypes(nested, includeModuleType))
            {
                yield return deep;
            }
        }
    }

    private static bool ShouldSkipType(TypeDefinition type, bool includeModuleType)
    {
        if (includeModuleType && string.Equals(type.Name, "<Module>", StringComparison.Ordinal))
        {
            return false;
        }

        return type.Name.StartsWith("<", StringComparison.Ordinal);
    }
}
