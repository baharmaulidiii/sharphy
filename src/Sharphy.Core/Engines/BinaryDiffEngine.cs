using Mono.Cecil;

namespace Sharphy.Core.Engines;

/// <summary>
/// Provides ECMA-335 binary assembly diffing to detect structural API breaking changes and internal CIL logic shifts.
/// </summary>
public static class BinaryDiffEngine
{
    public static string CompareAssemblies(string oldAssemblyPath, string newAssemblyPath)
    {
        if (!File.Exists(oldAssemblyPath))
        {
            throw new FileNotFoundException($"Baseline assembly was not found at '{oldAssemblyPath}'.", oldAssemblyPath);
        }

        if (!File.Exists(newAssemblyPath))
        {
            throw new FileNotFoundException($"Updated assembly was not found at '{newAssemblyPath}'.", newAssemblyPath);
        }

        using var oldAssembly = AssemblyDefinition.ReadAssembly(oldAssemblyPath);
        using var newAssembly = AssemblyDefinition.ReadAssembly(newAssemblyPath);

        var sw = new StringWriter();
        sw.WriteLine($"=== Sharphy ECMA-335 Binary Diff Analysis ===");
        sw.WriteLine($"Baseline: {Path.GetFileName(oldAssemblyPath)} (v{oldAssembly.Name.Version})");
        sw.WriteLine($"Updated : {Path.GetFileName(newAssemblyPath)} (v{newAssembly.Name.Version})");
        sw.WriteLine();

        var oldTypes = oldAssembly.MainModule.Types.ToDictionary(t => t.FullName, t => t);
        var newTypes = newAssembly.MainModule.Types.ToDictionary(t => t.FullName, t => t);

        var removedTypes = oldTypes.Keys.Except(newTypes.Keys).Where(k => !k.StartsWith("<")).ToList();
        var addedTypes = newTypes.Keys.Except(oldTypes.Keys).Where(k => !k.StartsWith("<")).ToList();
        var commonTypes = oldTypes.Keys.Intersect(newTypes.Keys).Where(k => !k.StartsWith("<")).ToList();

        sw.WriteLine("--- Type Hierarchy Changes ---");
        sw.WriteLine($"Added Types   : {addedTypes.Count}");
        foreach (var t in addedTypes)
        {
            sw.WriteLine($"  [+] {t}");
        }

        sw.WriteLine($"Removed Types : {removedTypes.Count}");
        foreach (var t in removedTypes)
        {
            sw.WriteLine($"  [-] {t} (⚠️ Potential Breaking Change)");
        }

        sw.WriteLine();
        sw.WriteLine("--- Method Signatures & Body Modifications ---");

        int changedMethodsCount = 0;
        int breakingChangesCount = removedTypes.Count;

        foreach (var typeName in commonTypes)
        {
            var oldT = oldTypes[typeName];
            var newT = newTypes[typeName];

            var oldMethods = oldT.Methods.ToDictionary(m => GetMethodSignatureKey(m), m => m);
            var newMethods = newT.Methods.ToDictionary(m => GetMethodSignatureKey(m), m => m);

            var removedMethods = oldMethods.Keys.Except(newMethods.Keys).ToList();
            var addedMethods = newMethods.Keys.Except(oldMethods.Keys).ToList();
            var matchedMethods = oldMethods.Keys.Intersect(newMethods.Keys).ToList();

            foreach (var rem in removedMethods)
            {
                var m = oldMethods[rem];
                if (m.IsPublic || m.IsFamily)
                {
                    sw.WriteLine($"  [-] {typeName}::{rem} (⚠️ Public/Protected Method Removed)");
                    breakingChangesCount++;
                }
            }

            foreach (var add in addedMethods)
            {
                var m = newMethods[add];
                if (m.IsPublic || m.IsFamily)
                {
                    sw.WriteLine($"  [+] {typeName}::{add} (New Public/Protected API)");
                }
            }

            foreach (var key in matchedMethods)
            {
                var oldM = oldMethods[key];
                var newM = newMethods[key];

                if (oldM.HasBody && newM.HasBody)
                {
                    if (oldM.Body.Instructions.Count != newM.Body.Instructions.Count ||
                        oldM.Body.CodeSize != newM.Body.CodeSize)
                    {
                        changedMethodsCount++;
                        sw.WriteLine($"  [~] {typeName}::{oldM.Name} (CIL size: {oldM.Body.CodeSize}B -> {newM.Body.CodeSize}B, Ops: {oldM.Body.Instructions.Count} -> {newM.Body.Instructions.Count})");
                    }
                }
            }
        }

        sw.WriteLine();
        sw.WriteLine("--- Compatibility Summary ---");
        sw.WriteLine($"Total Methods with Modified CIL: {changedMethodsCount}");
        sw.WriteLine($"Breaking Changes Detected     : {breakingChangesCount}");
        sw.WriteLine(breakingChangesCount > 0
            ? "Status: ⚠️ BREAKING CHANGES DETECTED - Requires SemVer Major/Minor bump."
            : "Status: ✅ FULLY COMPATIBLE - No public breaking API removals detected.");

        return sw.ToString();
    }

    private static string GetMethodSignatureKey(MethodDefinition m)
    {
        var paramTypes = string.Join(",", m.Parameters.Select(p => p.ParameterType.FullName));
        return $"{m.ReturnType.FullName} {m.Name}({paramTypes})";
    }
}
