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
        using var assembly = CecilHelpers.ReadAssemblyReadOnly(assemblyPath);

        var type = CecilHelpers.FindType(assembly.MainModule, typeName);
        if (type == null)
        {
            throw new InvalidOperationException($"Type '{typeName}' was not found in assembly.");
        }

        var method = type.Methods
            .FirstOrDefault(m => m.Name.Equals(methodName, StringComparison.OrdinalIgnoreCase) && m.HasBody)
            ?? type.Methods.FirstOrDefault(m => m.Name.Equals(methodName, StringComparison.OrdinalIgnoreCase));

        if (method == null || !method.HasBody || method.Body.Instructions.Count == 0)
        {
            throw new InvalidOperationException($"Method '{methodName}' with body was not found in type '{type.FullName}'.");
        }

        var targetInstr = method.Body.Instructions
            .FirstOrDefault(i => i.Offset == instructionOffset);

        if (targetInstr == null)
        {
            var maxOffset = method.Body.Instructions[^1].Offset;
            throw new ArgumentOutOfRangeException(nameof(instructionOffset),
                $"Instruction offset IL_{instructionOffset:x4} ({instructionOffset}) was not found in '{type.FullName}::{method.Name}' (valid offsets: 0..{maxOffset}).");
        }

        // Resolve OpCode by name (e.g., Nop, Ldc_I4_1, Ret)
        var normalizedOpCode = newOpCodeName.Replace('.', '_');
        var opcodeField = typeof(OpCodes).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
            .FirstOrDefault(f => f.Name.Equals(normalizedOpCode, StringComparison.OrdinalIgnoreCase));

        if (opcodeField == null)
        {
            throw new ArgumentException($"OpCode '{newOpCodeName}' is not recognized in ECMA-335 CIL.");
        }

        var newOpCode = (OpCode)opcodeField.GetValue(null)!;
        if (newOpCode.OperandType != OperandType.InlineNone)
        {
            throw new ArgumentException($"OpCode '{newOpCode.Name}' requires an operand ({newOpCode.OperandType}) and cannot replace an instruction without operand metadata.");
        }

        // Mutate the Instruction instance in place so existing branch targets and ExceptionHandler bounds stay valid.
        var originalOffset = targetInstr.Offset;
        targetInstr.OpCode = newOpCode;
        targetInstr.Operand = null;

        int runningOffset = 0;
        foreach (var ins in method.Body.Instructions)
        {
            ins.Offset = runningOffset;
            runningOffset += ins.GetSize();
        }

        CecilHelpers.WriteAssemblyAvoidingLock(assembly, assemblyPath, outputPath);

        return $"Successfully replaced instruction at IL offset {originalOffset} with OpCode '{newOpCode.Name}' and wrote patched assembly to '{outputPath}'.";
    }
}
