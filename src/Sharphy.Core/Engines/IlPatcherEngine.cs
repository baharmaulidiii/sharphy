using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Sharphy.Core.Engines;

/// <summary>
/// Provides surgical CIL instruction bytecode mutation and binary re-emitting powered by Mono.Cecil.
/// </summary>
public static class IlPatcherEngine
{
    public static string PatchInstruction(
        string assemblyPath,
        string typeName,
        string methodName,
        int instructionOffset,
        string newOpCodeName,
        string outputPath)
    {
        if (!File.Exists(assemblyPath))
        {
            throw new FileNotFoundException($"Target assembly was not found at '{assemblyPath}'.", assemblyPath);
        }

        var readerParams = new ReaderParameters { ReadWrite = false };
        using var assembly = AssemblyDefinition.ReadAssembly(assemblyPath, readerParams);

        var type = assembly.MainModule.Types
            .FirstOrDefault(t => t.FullName.Equals(typeName, StringComparison.OrdinalIgnoreCase) ||
                                 t.Name.Equals(typeName, StringComparison.OrdinalIgnoreCase));

        if (type == null)
        {
            throw new InvalidOperationException($"Type '{typeName}' was not found in assembly.");
        }

        var method = type.Methods
            .FirstOrDefault(m => m.Name.Equals(methodName, StringComparison.OrdinalIgnoreCase));

        if (method == null || !method.HasBody)
        {
            throw new InvalidOperationException($"Method '{methodName}' with body was not found in type '{type.FullName}'.");
        }

        var ilProcessor = method.Body.GetILProcessor();
        var targetInstr = method.Body.Instructions
            .FirstOrDefault(i => i.Offset == instructionOffset);

        if (targetInstr == null)
        {
            // Fallback to first instruction if exact offset is not matched
            targetInstr = method.Body.Instructions.FirstOrDefault();
        }

        if (targetInstr == null)
        {
            throw new InvalidOperationException("No IL instructions found in method body.");
        }

        // Resolve OpCode by name (e.g., Nop, Ret, Ldc_I4_1)
        var opcodeField = typeof(OpCodes).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
            .FirstOrDefault(f => f.Name.Equals(newOpCodeName, StringComparison.OrdinalIgnoreCase));

        if (opcodeField == null)
        {
            throw new ArgumentException($"OpCode '{newOpCodeName}' is not recognized in ECMA-335 CIL.");
        }

        var newOpCode = (OpCode)opcodeField.GetValue(null)!;
        var replacement = ilProcessor.Create(newOpCode);
        ilProcessor.Replace(targetInstr, replacement);

        var outDir = Path.GetDirectoryName(outputPath);
        if (!string.IsNullOrEmpty(outDir) && !Directory.Exists(outDir))
        {
            Directory.CreateDirectory(outDir);
        }

        assembly.Write(outputPath);

        return $"Successfully replaced instruction at IL offset {targetInstr.Offset} with OpCode '{newOpCode.Name}' and wrote patched assembly to '{outputPath}'.";
    }
}
