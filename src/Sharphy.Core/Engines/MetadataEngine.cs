using Mono.Cecil;

namespace Sharphy.Core.Engines;

/// <summary>
/// Engine analisis metadata ECMA-335 berbasis Mono.Cecil.
/// </summary>
public static class MetadataEngine
{
    public static string InspectMetadata(string assemblyPath)
    {
        if (!File.Exists(assemblyPath))
        {
            throw new FileNotFoundException($"Assembly file was not found at '{assemblyPath}'.", assemblyPath);
        }

        var readerParams = new ReaderParameters { ReadSymbols = false };
        using var assembly = AssemblyDefinition.ReadAssembly(assemblyPath, readerParams);

        var sw = new StringWriter();
        sw.WriteLine($"=== ECMA-335 Assembly Metadata: {assembly.Name.Name} ===");
        sw.WriteLine($"Version: {assembly.Name.Version}");
        sw.WriteLine($"CLR Architecture: {assembly.MainModule.Architecture}");
        sw.WriteLine($"Runtime Version: {assembly.MainModule.RuntimeVersion}");
        sw.WriteLine($"Kind: {assembly.MainModule.Kind}");
        sw.WriteLine();

        sw.WriteLine("--- Defined Types ---");
        foreach (var type in assembly.MainModule.Types.OrderBy(t => t.FullName))
        {
            if (type.Name.StartsWith("<")) continue; // Skip compiler-generated special types

            sw.WriteLine($"[Type] {type.FullName} (Base: {type.BaseType?.FullName ?? "None"})");
            foreach (var m in type.Methods.Where(m => !m.IsGetter && !m.IsSetter))
            {
                var vis = m.IsPublic ? "public" : (m.IsPrivate ? "private" : "internal");
                sw.WriteLine($"  └─ [Method] {vis} {m.ReturnType.Name} {m.Name}({string.Join(", ", m.Parameters.Select(p => $"{p.ParameterType.Name} {p.Name}"))})");
            }
        }

        if (assembly.MainModule.AssemblyReferences.Count > 0)
        {
            sw.WriteLine();
            sw.WriteLine("--- Referenced Assemblies ---");
            foreach (var r in assembly.MainModule.AssemblyReferences)
            {
                sw.WriteLine($"  • {r.FullName}");
            }
        }

        return sw.ToString();
    }
}
