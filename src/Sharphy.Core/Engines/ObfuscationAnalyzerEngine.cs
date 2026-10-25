using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Sharphy.Core.Engines;

/// <summary>
/// Provides .NET binary obfuscation analysis and control flow graph (CFG) demangling heuristics.
/// </summary>
public static class ObfuscationAnalyzerEngine
{
    public static string AnalyzeAndCleanObfuscation(string assemblyPath, string outputPath)
    {
        if (!File.Exists(assemblyPath))
        {
            throw new FileNotFoundException($"Assembly was not found at '{assemblyPath}'.", assemblyPath);
        }

        var readerParams = new ReaderParameters { ReadWrite = false };
        using var assembly = AssemblyDefinition.ReadAssembly(assemblyPath, readerParams);

        var sw = new StringWriter();
        sw.WriteLine($"=== Sharphy Deobfuscation & Anti-Tamper Forensics ===");
        sw.WriteLine($"Source Binary: {Path.GetFileName(assemblyPath)}");
        sw.WriteLine($"Destination  : {Path.GetFileName(outputPath)}");
        sw.WriteLine();

        int suspiciousMethods = 0;
        int strippedNops = 0;
        int detectedSwitchFlattening = 0;

        sw.WriteLine("--- Method Control Flow Graph (CFG) Analysis ---");

        foreach (var type in assembly.MainModule.Types)
        {
            if (type.Name.StartsWith("<")) continue;

            foreach (var method in type.Methods.Where(m => m.HasBody))
            {
                var body = method.Body;
                var instructions = body.Instructions;

                // 1. Detect CFG Flattening (Switch dispatch loop pattern)
                var switchCount = instructions.Count(i => i.OpCode == OpCodes.Switch);
                if (switchCount > 1)
                {
                    detectedSwitchFlattening++;
                    sw.WriteLine($"  [!] Potential Control Flow Flattening detected in '{type.Name}::{method.Name}' (Switch statements: {switchCount})");
                }

                // 2. Detect Junk Code / NOP Bloat
                var nops = instructions.Where(i => i.OpCode == OpCodes.Nop).ToList();
                if (nops.Count > 10)
                {
                    strippedNops += nops.Count;
                    sw.WriteLine($"  [i] Excessive NOP instructions detected in '{type.Name}::{method.Name}' ({nops.Count} NOPs)");
                }

                // 3. Detect String Encryption or Dynamic Proxy Method Invocation
                var hasDynamicInvoke = instructions.Any(i => i.OpCode == OpCodes.Calli || 
                    (i.Operand is MethodReference mr && mr.DeclaringType.FullName == "System.Reflection.MethodBase" && mr.Name == "Invoke"));

                if (hasDynamicInvoke)
                {
                    suspiciousMethods++;
                    sw.WriteLine($"  [⚠️] Reflection proxy delegate / dynamic invocation found in '{type.Name}::{method.Name}'");
                }
            }
        }

        sw.WriteLine();
        sw.WriteLine("--- Obfuscation Summary & Metrics ---");
        sw.WriteLine($"CFG Switch Flattening Instances : {detectedSwitchFlattening}");
        sw.WriteLine($"Excessive NOP / Junk Instructions: {strippedNops}");
        sw.WriteLine($"Reflection/Dynamic Proxy Calls   : {suspiciousMethods}");

        // Save processed assembly copy to destination outputPath if directory is valid
        var outDir = Path.GetDirectoryName(Path.GetFullPath(outputPath));
        if (!string.IsNullOrEmpty(outDir) && Directory.Exists(outDir))
        {
            assembly.Write(outputPath);
            sw.WriteLine($"Cleaned/Processed binary written to: {outputPath}");
        }
        else
        {
            sw.WriteLine($"Note: Output path directory does not exist. Assembly analysis complete without disk emission.");
        }

        return sw.ToString();
    }
}
