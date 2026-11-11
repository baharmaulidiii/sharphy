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
        using var assembly = CecilHelpers.ReadAssemblyReadOnly(assemblyPath);

        var sw = new StringWriter();
        sw.WriteLine("=== Sharphy Deobfuscation & Anti-Tamper Forensics ===");
        sw.WriteLine($"Source Binary: {Path.GetFileName(assemblyPath)}");
        sw.WriteLine($"Destination  : {Path.GetFileName(outputPath)}");
        sw.WriteLine();

        int suspiciousMethods = 0;
        int strippedNops = 0;
        int detectedSwitchFlattening = 0;
        int detectedDecryptors = 0;

        sw.WriteLine("--- Method Control Flow Graph (CFG) Analysis ---");

        foreach (var type in CecilHelpers.EnumerateAllTypes(assembly.MainModule, includeModuleType: true))
        {
            foreach (var method in type.Methods.Where(m => m.HasBody))
            {
                var body = method.Body;
                var instructions = body.Instructions;

                // 1. Detect CFG Flattening (Switch state-machine dispatcher loop)
                if (IsControlFlowFlatteningDispatcher(body))
                {
                    detectedSwitchFlattening++;
                    var switchCount = instructions.Count(i => i.OpCode == OpCodes.Switch);
                    sw.WriteLine($"  [!] Control Flow Flattening dispatcher detected in '{type.Name}::{method.Name}' (Switch statements: {switchCount})");
                }

                // 2. Detect and safely strip excessive dead NOP sleds
                var nopCount = instructions.Count(i => i.OpCode == OpCodes.Nop);
                if (nopCount > 10)
                {
                    var removed = StripRedundantNops(body);
                    strippedNops += removed;
                    sw.WriteLine($"  [i] Stripped {removed}/{nopCount} redundant NOP instructions in '{type.Name}::{method.Name}'");
                }

                // 3. Detect String Encryption or Dynamic Proxy Method Invocation
                var hasDynamicInvoke = instructions.Any(i => i.OpCode == OpCodes.Calli ||
                    (i.Operand is MethodReference mr &&
                     ((mr.DeclaringType?.FullName is "System.Reflection.MethodBase" or "System.Reflection.MethodInfo" && mr.Name == "Invoke") ||
                      (mr.DeclaringType?.FullName == "System.Delegate" && mr.Name == "DynamicInvoke"))));

                if (hasDynamicInvoke)
                {
                    suspiciousMethods++;
                    sw.WriteLine($"  [⚠️] Reflection proxy delegate / dynamic invocation found in '{type.Name}::{method.Name}'");
                }

                // 4. Detect Pattern-Matching String Decryptors (XOR / ROT / BitShift / Base64)
                var decryptorKind = ClassifyDecryptorPattern(instructions);
                if (decryptorKind != null)
                {
                    detectedDecryptors++;
                    sw.WriteLine($"  [🔐] Pattern-matched {decryptorKind} decryptor routine found in '{type.Name}::{method.Name}' (Token: 0x{method.MetadataToken.ToInt32():X8})");
                }
            }
        }

        sw.WriteLine();
        sw.WriteLine("--- Obfuscation Summary & Metrics ---");
        sw.WriteLine($"CFG Switch Flattening Instances : {detectedSwitchFlattening}");
        sw.WriteLine($"Excessive NOP / Junk Instructions: {strippedNops}");
        sw.WriteLine($"Reflection/Dynamic Proxy Calls   : {suspiciousMethods}");
        sw.WriteLine($"Pattern-Matched String Decryptors: {detectedDecryptors}");

        // Save processed assembly copy to destination outputPath (creating missing parent directories safely)
        CecilHelpers.WriteAssemblyAvoidingLock(assembly, assemblyPath, outputPath);
        sw.WriteLine($"Cleaned/Processed binary written to: {outputPath}");

        return sw.ToString();
    }

    /// <summary>
    /// Detects Control Flow Flattening (CFF) state-machine switch dispatchers where a local state variable
    /// drives a switch table enclosed by a back-edge dispatcher loop (or multiple switch dispatchers).
    /// </summary>
    public static bool IsControlFlowFlatteningDispatcher(MethodBody body)
    {
        if (body?.Instructions == null || body.Instructions.Count < 4) return false;

        var instructions = body.Instructions;
        var indexByInstruction = BuildInstructionIndexMap(instructions);
        int switchCount = 0;

        for (int i = 0; i < instructions.Count; i++)
        {
            var ins = instructions[i];
            if (ins.OpCode != OpCodes.Switch) continue;

            switchCount++;
            if (switchCount > 1) return true;

            bool hasStateLoad = false;
            int windowStart = Math.Max(0, i - 6);
            for (int k = windowStart; k < i; k++)
            {
                if (IsLocalLoad(instructions[k].OpCode.Code))
                {
                    hasStateLoad = true;
                    break;
                }
            }

            bool hasDispatcherBackEdge = false;
            for (int j = i + 1; j < instructions.Count; j++)
            {
                if (instructions[j].Operand is Instruction target &&
                    indexByInstruction.TryGetValue(target, out var targetIdx) &&
                    targetIdx <= i &&
                    IsBranchOpCode(instructions[j].OpCode))
                {
                    hasDispatcherBackEdge = true;
                    break;
                }
            }

            var targetCount = ins.Operand is Instruction[] targets ? targets.Length : 0;
            if (hasStateLoad && hasDispatcherBackEdge && targetCount >= 2)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Safely removes redundant NOP instructions that are not branch targets or exception handler boundaries,
    /// then recalculates sequential instruction offsets.
    /// </summary>
    public static int StripRedundantNops(MethodBody body)
    {
        if (body?.Instructions == null || body.Instructions.Count == 0) return 0;

        var protectedInstructions = new HashSet<Instruction>();
        foreach (var ins in body.Instructions)
        {
            if (ins.Operand is Instruction singleTarget)
            {
                protectedInstructions.Add(singleTarget);
            }
            else if (ins.Operand is Instruction[] switchTargets)
            {
                foreach (var t in switchTargets)
                {
                    protectedInstructions.Add(t);
                }
            }
        }

        if (body.HasExceptionHandlers)
        {
            foreach (var eh in body.ExceptionHandlers)
            {
                if (eh.TryStart != null) protectedInstructions.Add(eh.TryStart);
                if (eh.TryEnd != null) protectedInstructions.Add(eh.TryEnd);
                if (eh.HandlerStart != null) protectedInstructions.Add(eh.HandlerStart);
                if (eh.HandlerEnd != null) protectedInstructions.Add(eh.HandlerEnd);
                if (eh.FilterStart != null) protectedInstructions.Add(eh.FilterStart);
            }
        }

        var toRemove = body.Instructions
            .Where(i => i.OpCode == OpCodes.Nop && !protectedInstructions.Contains(i))
            .ToList();

        if (toRemove.Count == 0) return 0;

        var il = body.GetILProcessor();
        foreach (var nop in toRemove)
        {
            il.Remove(nop);
        }

        int offset = 0;
        foreach (var ins in body.Instructions)
        {
            ins.Offset = offset;
            offset += ins.GetSize();
        }

        return toRemove.Count;
    }

    /// <summary>
    /// Detects signature byte/string decryption patterns via CIL opcode sequences.
    /// </summary>
    public static bool IsStringDecryptorLoop(Mono.Collections.Generic.Collection<Instruction> instructions)
    {
        return ClassifyDecryptorPattern(instructions) != null;
    }

    /// <summary>
    /// Classifies obfuscated string/payload decryption routines into XOR/Bitwise, BitShift,
    /// ROT/Arithmetic, or Base64/Lookup patterns using ECMA-335 CIL opcodes.
    /// </summary>
    public static string? ClassifyDecryptorPattern(Mono.Collections.Generic.Collection<Instruction> instructions)
    {
        if (instructions == null || instructions.Count < 3) return null;

        var indexByInstruction = BuildInstructionIndexMap(instructions);
        bool hasLdelem = false;
        bool hasXorOrNot = false;
        bool hasAnd = false;
        bool hasBitShift = false;
        bool hasOr = false;
        bool hasArithmetic = false;
        bool hasRemainder = false;
        bool hasStelemOrConv = false;
        bool hasLoopBranch = false;
        bool hasBase64Call = false;
        bool hasMask63 = false;

        for (int i = 0; i < instructions.Count; i++)
        {
            var ins = instructions[i];
            var op = ins.OpCode;

            if (op == OpCodes.Ldelem_U1 || op == OpCodes.Ldelem_I1 || op == OpCodes.Ldelem_U2 || op == OpCodes.Ldelem_I2 || op == OpCodes.Ldelema)
            {
                hasLdelem = true;
            }
            else if (op == OpCodes.Xor || op == OpCodes.Not)
            {
                hasXorOrNot = true;
            }
            else if (op == OpCodes.And)
            {
                hasAnd = true;
            }
            else if (op == OpCodes.Shl || op == OpCodes.Shr || op == OpCodes.Shr_Un)
            {
                hasBitShift = true;
            }
            else if (op == OpCodes.Or)
            {
                hasOr = true;
            }
            else if (op == OpCodes.Add || op == OpCodes.Sub)
            {
                hasArithmetic = true;
            }
            else if (op == OpCodes.Rem || op == OpCodes.Rem_Un)
            {
                hasRemainder = true;
            }
            else if (op == OpCodes.Stelem_I1 || op == OpCodes.Stelem_I2 || op == OpCodes.Stelem_Any || op == OpCodes.Conv_U1 || op == OpCodes.Conv_I1 || op == OpCodes.Conv_U2)
            {
                hasStelemOrConv = true;
            }
            else if (op == OpCodes.Ldc_I4_S && ins.Operand is sbyte sb && sb == 63)
            {
                hasMask63 = true;
            }
            else if (op == OpCodes.Ldc_I4 && ins.Operand is int imm && imm == 63)
            {
                hasMask63 = true;
            }
            else if ((op == OpCodes.Call || op == OpCodes.Callvirt) &&
                     ins.Operand is MethodReference mr &&
                     mr.DeclaringType?.FullName == "System.Convert" &&
                     (mr.Name == "FromBase64String" || mr.Name == "FromBase64CharArray"))
            {
                hasBase64Call = true;
            }

            if (ins.Operand is Instruction target &&
                indexByInstruction.TryGetValue(target, out var targetIdx) &&
                targetIdx < i &&
                IsLoopBackBranch(op))
            {
                hasLoopBranch = true;
            }
        }

        if (hasLdelem && hasXorOrNot && (hasStelemOrConv || hasLoopBranch))
        {
            return "XOR/Bitwise";
        }

        if (hasLdelem && hasAnd && (hasStelemOrConv || hasLoopBranch) && !hasBitShift)
        {
            return "XOR/Bitwise";
        }

        if (hasLdelem && hasBitShift && (hasOr || hasStelemOrConv) && (hasLoopBranch || hasStelemOrConv))
        {
            return "BitShift";
        }

        if (hasLdelem && hasArithmetic && hasLoopBranch && (hasRemainder || hasStelemOrConv))
        {
            return "ROT/Arithmetic";
        }

        if (hasBase64Call || (hasLdelem && hasMask63 && hasBitShift && (hasLoopBranch || hasOr)))
        {
            return "Base64/Lookup";
        }

        return null;
    }

    private static Dictionary<Instruction, int> BuildInstructionIndexMap(Mono.Collections.Generic.Collection<Instruction> instructions)
    {
        var map = new Dictionary<Instruction, int>(instructions.Count);
        for (int i = 0; i < instructions.Count; i++)
        {
            map[instructions[i]] = i;
        }
        return map;
    }

    private static bool IsLocalLoad(Code code) =>
        code == Code.Ldloc || code == Code.Ldloc_S ||
        code == Code.Ldloc_0 || code == Code.Ldloc_1 ||
        code == Code.Ldloc_2 || code == Code.Ldloc_3 ||
        code == Code.Ldloca || code == Code.Ldloca_S;

    private static bool IsBranchOpCode(OpCode op) =>
        op == OpCodes.Br || op == OpCodes.Br_S ||
        op == OpCodes.Brtrue || op == OpCodes.Brtrue_S ||
        op == OpCodes.Brfalse || op == OpCodes.Brfalse_S ||
        op == OpCodes.Beq || op == OpCodes.Beq_S ||
        op == OpCodes.Bne_Un || op == OpCodes.Bne_Un_S ||
        op == OpCodes.Blt || op == OpCodes.Blt_S ||
        op == OpCodes.Blt_Un || op == OpCodes.Blt_Un_S ||
        op == OpCodes.Ble || op == OpCodes.Ble_S ||
        op == OpCodes.Bgt || op == OpCodes.Bgt_S ||
        op == OpCodes.Bge || op == OpCodes.Bge_S;

    private static bool IsLoopBackBranch(OpCode op) =>
        op == OpCodes.Bne_Un || op == OpCodes.Bne_Un_S ||
        op == OpCodes.Blt || op == OpCodes.Blt_S ||
        op == OpCodes.Blt_Un || op == OpCodes.Blt_Un_S ||
        op == OpCodes.Br || op == OpCodes.Br_S ||
        op == OpCodes.Brtrue || op == OpCodes.Brtrue_S;
}
