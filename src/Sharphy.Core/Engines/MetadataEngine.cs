using Mono.Cecil;

namespace Sharphy.Core.Engines;

/// <summary>
/// Provides ECMA-335 metadata hierarchy and type reflection powered by Mono.Cecil.
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
        var pInvokeEntries = new List<(string DeclaringType, MethodDefinition Method, PInvokeInfo Info)>();

        foreach (var type in assembly.MainModule.Types.OrderBy(t => t.FullName))
        {
            if (type.Name.StartsWith("<")) continue; // Skip compiler-generated special types

            sw.WriteLine($"[Type] {type.FullName} (Base: {type.BaseType?.FullName ?? "None"})");
            foreach (var m in type.Methods.Where(m => !m.IsGetter && !m.IsSetter))
            {
                var vis = m.IsPublic ? "public" : (m.IsPrivate ? "private" : "internal");
                sw.WriteLine($"  └─ [Method] {vis} {m.ReturnType.Name} {m.Name}({string.Join(", ", m.Parameters.Select(p => $"{p.ParameterType.Name} {p.Name}"))})");

                if (m.HasPInvokeInfo && m.PInvokeInfo != null)
                {
                    pInvokeEntries.Add((type.FullName, m, m.PInvokeInfo));
                }
            }

            CollectNestedPInvokes(type, pInvokeEntries);
        }

        if (pInvokeEntries.Count > 0)
        {
            sw.WriteLine();
            sw.WriteLine("--- Unmanaged P/Invoke Imports ---");
            foreach (var (declaringType, method, info) in pInvokeEntries.Take(25))
            {
                var dllName = string.IsNullOrWhiteSpace(info.Module?.Name) ? "<unknown>" : info.Module.Name;
                var entryPoint = string.IsNullOrWhiteSpace(info.EntryPoint) ? method.Name : info.EntryPoint;
                var callConv = FormatCallingConvention(info);
                var charSet = FormatCharSet(info);
                var lastErr = info.SupportsLastError ? ", SetLastError=true" : string.Empty;
                sw.WriteLine($"  • {declaringType}::{method.Name} -> [{dllName}!{entryPoint}] ({callConv}, {charSet}{lastErr})");
            }

            if (pInvokeEntries.Count > 25)
            {
                sw.WriteLine($"  ... ({pInvokeEntries.Count - 25} additional P/Invoke imports omitted for token efficiency)");
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

    private static void CollectNestedPInvokes(
        TypeDefinition parentType,
        List<(string DeclaringType, MethodDefinition Method, PInvokeInfo Info)> entries)
    {
        if (!parentType.HasNestedTypes) return;

        foreach (var nested in parentType.NestedTypes)
        {
            if (nested.Name.StartsWith("<")) continue;

            foreach (var m in nested.Methods)
            {
                if (m.HasPInvokeInfo && m.PInvokeInfo != null)
                {
                    entries.Add((nested.FullName, m, m.PInvokeInfo));
                }
            }

            CollectNestedPInvokes(nested, entries);
        }
    }

    private static string FormatCallingConvention(PInvokeInfo info)
    {
        if (info.IsCallConvCdecl) return "Cdecl";
        if (info.IsCallConvStdCall) return "StdCall";
        if (info.IsCallConvThiscall) return "ThisCall";
        if (info.IsCallConvFastcall) return "FastCall";
        return "Winapi";
    }

    private static string FormatCharSet(PInvokeInfo info)
    {
        if (info.IsCharSetUnicode) return "Unicode";
        if (info.IsCharSetAnsi) return "Ansi";
        if (info.IsCharSetAuto) return "Auto";
        return "CharSetNotSpec";
    }
}
