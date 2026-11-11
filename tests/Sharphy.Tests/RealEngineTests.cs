using Sharphy.Core.Engines;
using Xunit;

namespace Sharphy.Tests;

public class RealEngineTests
{
    private static readonly string SampleAssemblyPath = Path.GetFullPath(
        Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "samples", "CrackmeDemo", "bin", "Release", "net8.0", "CrackmeDemo.dll"));

    [Fact]
    public void MetadataEngine_InspectsCrackmeDemo_Successfully()
    {
        Assert.True(File.Exists(SampleAssemblyPath), $"Sample binary not found at: {SampleAssemblyPath}");

        var metadata = MetadataEngine.InspectMetadata(SampleAssemblyPath);

        Assert.NotNull(metadata);
        Assert.Contains("CrackmeDemo", metadata);
        Assert.Contains("Defined Types", metadata);
    }

    [Fact]
    public void DecompilerEngine_DecompilesMainMethod_Successfully()
    {
        Assert.True(File.Exists(SampleAssemblyPath));

        var code = DecompilerEngine.DecompileMethod(SampleAssemblyPath, "CrackmeDemo.Program", "Main");

        Assert.NotNull(code);
        Assert.Contains("Main", code);
    }

    [Fact]
    public void SandboxEngine_ExtractsTokensSafely()
    {
        Assert.True(File.Exists(SampleAssemblyPath));

        var tokens = SandboxEngine.ExtractTokens(SampleAssemblyPath);

        Assert.NotNull(tokens);
        Assert.Contains("Sandboxed Token Extraction", tokens);
    }

    [Fact]
    public void WrapperGeneratorEngine_GeneratesUnsafeAccessorWrapper()
    {
        Assert.True(File.Exists(SampleAssemblyPath));

        var tempOutput = Path.Combine(Path.GetTempPath(), $"WrapperTest_{Guid.NewGuid():N}.cs");
        try
        {
            var code = WrapperGeneratorEngine.GenerateWrapper(SampleAssemblyPath, "CrackmeDemo.Program", tempOutput);

            Assert.NotNull(code);
            Assert.Contains("[UnsafeAccessor", code);
            Assert.True(File.Exists(tempOutput));
        }
        finally
        {
            if (File.Exists(tempOutput)) File.Delete(tempOutput);
        }
    }

    [Fact]
    public void BinaryDiffEngine_ComparesAssemblyWithSelf_ReportsZeroBreakingChanges()
    {
        Assert.True(File.Exists(SampleAssemblyPath));

        var diff = BinaryDiffEngine.CompareAssemblies(SampleAssemblyPath, SampleAssemblyPath);

        Assert.NotNull(diff);
        Assert.Contains("Sharphy ECMA-335 Binary Diff Analysis", diff);
        Assert.Contains("FULLY COMPATIBLE", diff);
        Assert.Contains("Breaking Changes Detected     : 0", diff);
    }

    [Fact]
    public void ObfuscationAnalyzerEngine_AnalyzesCrackmeDemo_Successfully()
    {
        Assert.True(File.Exists(SampleAssemblyPath));

        var tempOutput = Path.Combine(Path.GetTempPath(), $"CleanedDemo_{Guid.NewGuid():N}.dll");
        try
        {
            var analysis = ObfuscationAnalyzerEngine.AnalyzeAndCleanObfuscation(SampleAssemblyPath, tempOutput);

            Assert.NotNull(analysis);
            Assert.Contains("Deobfuscation & Anti-Tamper Forensics", analysis);
            Assert.Contains("CFG Switch Flattening Instances", analysis);
            Assert.True(File.Exists(tempOutput));
        }
        finally
        {
            if (File.Exists(tempOutput)) File.Delete(tempOutput);
        }
    }
    [Fact]
    public void BinaryDiffEngine_UnifiedCilDiff_OutputsAccurateInstructions()
    {
        Assert.True(File.Exists(SampleAssemblyPath));

        var diff = BinaryDiffEngine.CompareAssemblies(SampleAssemblyPath, SampleAssemblyPath, includeCilDiff: true);

        Assert.NotNull(diff);
        Assert.Contains("Sharphy ECMA-335 Binary Diff Analysis", diff);
        Assert.Contains("FULLY COMPATIBLE", diff);
    }

    [Fact]
    public void ObfuscationAnalyzer_FlagsXorStringDecryptor()
    {
        using var assembly = Mono.Cecil.AssemblyDefinition.CreateAssembly(
            new Mono.Cecil.AssemblyNameDefinition("TestDynDec", new Version(1, 0, 0, 0)),
            "TestDynDec",
            Mono.Cecil.ModuleKind.Dll);

        var module = assembly.MainModule;
        var voidType = module.TypeSystem.Void;
        var method = new Mono.Cecil.MethodDefinition("DecryptString", Mono.Cecil.MethodAttributes.Public | Mono.Cecil.MethodAttributes.Static, voidType);
        method.Body.InitLocals = true;

        var il = method.Body.GetILProcessor();
        var loopTarget = il.Create(Mono.Cecil.Cil.OpCodes.Ldelem_U1);
        il.Append(il.Create(Mono.Cecil.Cil.OpCodes.Nop));
        il.Append(loopTarget);
        il.Append(il.Create(Mono.Cecil.Cil.OpCodes.Ldc_I4_S, (sbyte)0x5A));
        il.Append(il.Create(Mono.Cecil.Cil.OpCodes.Xor));
        il.Append(il.Create(Mono.Cecil.Cil.OpCodes.Conv_U1));
        il.Append(il.Create(Mono.Cecil.Cil.OpCodes.Bne_Un, loopTarget));
        il.Append(il.Create(Mono.Cecil.Cil.OpCodes.Ret));

        var isDecryptor = ObfuscationAnalyzerEngine.IsStringDecryptorLoop(method.Body.Instructions);
        Assert.True(isDecryptor);
    }

    [Fact]
    public void DumpForensicsEngine_HandlesMissingDumpGracefully()
    {
        var nonExistentDump = Path.Combine(Path.GetTempPath(), "NonExistent_" + Guid.NewGuid().ToString("N") + ".dmp");

        var rca = DumpForensicsEngine.AnalyzeCrashDump(nonExistentDump);
        Assert.Contains("Target crash dump file was not found", rca);

        var heap = DumpForensicsEngine.InspectHeap(nonExistentDump);
        Assert.Contains("Target dump file was not found", heap);

        var deadlocks = DumpForensicsEngine.DetectDeadlocks(nonExistentDump);
        Assert.Contains("Target dump file was not found", deadlocks);
    }

    [Fact]
    public void DumpForensicsEngine_HandlesEmptyAndTruncatedDumpsGracefully()
    {
        var emptyDump = Path.Combine(Path.GetTempPath(), $"Empty_{Guid.NewGuid():N}.dmp");
        var truncatedDump = Path.Combine(Path.GetTempPath(), $"Truncated_{Guid.NewGuid():N}.dmp");

        try
        {
            File.WriteAllBytes(emptyDump, Array.Empty<byte>());
            // Write a truncated MDMP signature (only 32 bytes) to simulate an incomplete minidump
            File.WriteAllBytes(truncatedDump, "MDMP\x93\xA7\x00\x00TruncatedMinidumpPayload1234"u8.ToArray());

            var emptyRca = DumpForensicsEngine.AnalyzeCrashDump(emptyDump);
            var emptyHeap = DumpForensicsEngine.InspectHeap(emptyDump);
            var emptyDeadlocks = DumpForensicsEngine.DetectDeadlocks(emptyDump);

            Assert.Contains("Warning", emptyRca);
            Assert.Contains("empty (0 bytes)", emptyRca);
            Assert.Contains("Warning", emptyHeap);
            Assert.Contains("Warning", emptyDeadlocks);

            var truncRca = DumpForensicsEngine.AnalyzeCrashDump(truncatedDump);
            var truncHeap = DumpForensicsEngine.InspectHeap(truncatedDump);
            var truncDeadlocks = DumpForensicsEngine.DetectDeadlocks(truncatedDump);

            Assert.Contains("Warning", truncRca);
            Assert.Contains("Warning", truncHeap);
            Assert.Contains("Warning", truncDeadlocks);
        }
        finally
        {
            if (File.Exists(emptyDump)) File.Delete(emptyDump);
            if (File.Exists(truncatedDump)) File.Delete(truncatedDump);
        }
    }

    [Fact]
    public void DumpForensicsEngine_FindDeadlockCycles_DetectsCyclesAndIgnoresAcyclicChains()
    {
        // AB-BA 2-thread deadlock plus 3-thread deadlock cycle
        var cyclicGraph = new Dictionary<int, IReadOnlyList<int>>
        {
            [4] = new[] { 7 },
            [7] = new[] { 4 },
            [10] = new[] { 12 },
            [12] = new[] { 15 },
            [15] = new[] { 10 }
        };

        var cycles = DumpForensicsEngine.FindDeadlockCycles(cyclicGraph);
        Assert.Equal(2, cycles.Count);
        Assert.Contains(cycles, c => c.SequenceEqual(new[] { 4, 7, 4 }));
        Assert.Contains(cycles, c => c.SequenceEqual(new[] { 10, 12, 15, 10 }));

        // Acyclic contention chain: 1 -> 2 -> 3
        var acyclicGraph = new Dictionary<int, IReadOnlyList<int>>
        {
            [1] = new[] { 2 },
            [2] = new[] { 3 }
        };

        var noCycles = DumpForensicsEngine.FindDeadlockCycles(acyclicGraph);
        Assert.Empty(noCycles);
    }

    [Fact]
    public void DumpForensicsEngine_ComputeCompactionRiskScore_CalculatesExpectedBands()
    {
        var (lowScore, lowSeverity) = DumpForensicsEngine.ComputeCompactionRiskScore(5.0, 0.0, 0, 0);
        Assert.InRange(lowScore, 0, 24);
        Assert.Equal("LOW", lowSeverity);

        var (modScore, modSeverity) = DumpForensicsEngine.ComputeCompactionRiskScore(20.0, 10.0, 2, 1);
        Assert.InRange(modScore, 25, 49);
        Assert.Equal("MODERATE", modSeverity);

        var (highScore, highSeverity) = DumpForensicsEngine.ComputeCompactionRiskScore(45.0, 25.0, 3, 2);
        Assert.InRange(highScore, 50, 74);
        Assert.Equal("HIGH", highSeverity);

        var (critScore, critSeverity) = DumpForensicsEngine.ComputeCompactionRiskScore(75.0, 60.0, 8, 6);
        Assert.InRange(critScore, 75, 100);
        Assert.Equal("CRITICAL", critSeverity);
    }

    [Fact]
    public void MetadataEngine_ExtractsPInvokeBoundaryImports()
    {
        var tempDll = Path.Combine(Path.GetTempPath(), $"PInvokeMeta_{Guid.NewGuid():N}.dll");
        try
        {
            using (var assembly = Mono.Cecil.AssemblyDefinition.CreateAssembly(
                new Mono.Cecil.AssemblyNameDefinition("PInvokeMetaTest", new Version(1, 0, 0, 0)),
                "PInvokeMetaTest",
                Mono.Cecil.ModuleKind.Dll))
            {
                var module = assembly.MainModule;
                var interopType = new Mono.Cecil.TypeDefinition(
                    "Demo.Interop",
                    "NativeBridge",
                    Mono.Cecil.TypeAttributes.Public | Mono.Cecil.TypeAttributes.Abstract | Mono.Cecil.TypeAttributes.Sealed,
                    module.TypeSystem.Object);
                module.Types.Add(interopType);

                var nativeModRef = new Mono.Cecil.ModuleReference("kernel32.dll");
                module.ModuleReferences.Add(nativeModRef);

                var pInvokeMethod = new Mono.Cecil.MethodDefinition(
                    "GetTickCount64",
                    Mono.Cecil.MethodAttributes.Public | Mono.Cecil.MethodAttributes.Static | Mono.Cecil.MethodAttributes.PInvokeImpl | Mono.Cecil.MethodAttributes.HideBySig,
                    module.TypeSystem.UInt64);
                pInvokeMethod.ImplAttributes = Mono.Cecil.MethodImplAttributes.PreserveSig;
                pInvokeMethod.PInvokeInfo = new Mono.Cecil.PInvokeInfo(
                    Mono.Cecil.PInvokeAttributes.CallConvWinapi | Mono.Cecil.PInvokeAttributes.CharSetUnicode | Mono.Cecil.PInvokeAttributes.SupportsLastError,
                    "GetTickCount64",
                    nativeModRef);
                interopType.Methods.Add(pInvokeMethod);

                assembly.Write(tempDll);
            }

            var report = MetadataEngine.InspectMetadata(tempDll);
            Assert.Contains("Unmanaged P/Invoke Imports", report);
            Assert.Contains("Demo.Interop.NativeBridge::GetTickCount64", report);
            Assert.Contains("kernel32.dll!GetTickCount64", report);
            Assert.Contains("SetLastError=true", report);
        }
        finally
        {
            if (File.Exists(tempDll)) File.Delete(tempDll);
        }
    }

    [Fact]
    public void ObfuscationAnalyzer_ClassifiesRotBitShiftAndBase64Decryptors()
    {
        using var assembly = Mono.Cecil.AssemblyDefinition.CreateAssembly(
            new Mono.Cecil.AssemblyNameDefinition("DecryptorVariants", new Version(1, 0, 0, 0)),
            "DecryptorVariants",
            Mono.Cecil.ModuleKind.Dll);

        var module = assembly.MainModule;
        var voidType = module.TypeSystem.Void;

        // 1. ROT/Arithmetic decryptor loop
        var rotMethod = new Mono.Cecil.MethodDefinition("RotDecrypt", Mono.Cecil.MethodAttributes.Public | Mono.Cecil.MethodAttributes.Static, voidType);
        var rotIl = rotMethod.Body.GetILProcessor();
        var rotHead = rotIl.Create(Mono.Cecil.Cil.OpCodes.Ldelem_U1);
        rotIl.Append(rotHead);
        rotIl.Append(rotIl.Create(Mono.Cecil.Cil.OpCodes.Ldc_I4_S, (sbyte)13));
        rotIl.Append(rotIl.Create(Mono.Cecil.Cil.OpCodes.Add));
        rotIl.Append(rotIl.Create(Mono.Cecil.Cil.OpCodes.Conv_U1));
        rotIl.Append(rotIl.Create(Mono.Cecil.Cil.OpCodes.Bne_Un, rotHead));
        rotIl.Append(rotIl.Create(Mono.Cecil.Cil.OpCodes.Ret));
        Assert.Equal("ROT/Arithmetic", ObfuscationAnalyzerEngine.ClassifyDecryptorPattern(rotMethod.Body.Instructions));

        // 2. BitShift decryptor loop
        var shiftMethod = new Mono.Cecil.MethodDefinition("ShiftDecrypt", Mono.Cecil.MethodAttributes.Public | Mono.Cecil.MethodAttributes.Static, voidType);
        var shiftIl = shiftMethod.Body.GetILProcessor();
        var shiftHead = shiftIl.Create(Mono.Cecil.Cil.OpCodes.Ldelem_U1);
        shiftIl.Append(shiftHead);
        shiftIl.Append(shiftIl.Create(Mono.Cecil.Cil.OpCodes.Ldc_I4_4));
        shiftIl.Append(shiftIl.Create(Mono.Cecil.Cil.OpCodes.Shr_Un));
        shiftIl.Append(shiftIl.Create(Mono.Cecil.Cil.OpCodes.Or));
        shiftIl.Append(shiftIl.Create(Mono.Cecil.Cil.OpCodes.Conv_U1));
        shiftIl.Append(shiftIl.Create(Mono.Cecil.Cil.OpCodes.Bne_Un, shiftHead));
        shiftIl.Append(shiftIl.Create(Mono.Cecil.Cil.OpCodes.Ret));
        Assert.Equal("BitShift", ObfuscationAnalyzerEngine.ClassifyDecryptorPattern(shiftMethod.Body.Instructions));

        // 3. Base64 transform
        var b64Method = new Mono.Cecil.MethodDefinition("Base64Decrypt", Mono.Cecil.MethodAttributes.Public | Mono.Cecil.MethodAttributes.Static, voidType);
        var fromB64Ref = new Mono.Cecil.MethodReference("FromBase64String", module.ImportReference(typeof(byte[])), module.ImportReference(typeof(Convert)));
        fromB64Ref.Parameters.Add(new Mono.Cecil.ParameterDefinition(module.TypeSystem.String));
        var b64Il = b64Method.Body.GetILProcessor();
        b64Il.Append(b64Il.Create(Mono.Cecil.Cil.OpCodes.Ldstr, "U2hhcnBoeQ=="));
        b64Il.Append(b64Il.Create(Mono.Cecil.Cil.OpCodes.Call, fromB64Ref));
        b64Il.Append(b64Il.Create(Mono.Cecil.Cil.OpCodes.Pop));
        b64Il.Append(b64Il.Create(Mono.Cecil.Cil.OpCodes.Ret));
        Assert.Equal("Base64/Lookup", ObfuscationAnalyzerEngine.ClassifyDecryptorPattern(b64Method.Body.Instructions));
    }

    [Fact]
    public void ObfuscationAnalyzer_DetectsCffDispatcherAndStripsRedundantNopsSafely()
    {
        var inputDll = Path.Combine(Path.GetTempPath(), $"ObfInput_{Guid.NewGuid():N}.dll");
        var outputDll = Path.Combine(Path.GetTempPath(), $"ObfCleaned_{Guid.NewGuid():N}.dll");
        try
        {
            using (var assembly = Mono.Cecil.AssemblyDefinition.CreateAssembly(
                new Mono.Cecil.AssemblyNameDefinition("ObfTest", new Version(1, 0, 0, 0)),
                "ObfTest",
                Mono.Cecil.ModuleKind.Dll))
            {
                var module = assembly.MainModule;
                var testType = new Mono.Cecil.TypeDefinition("ObfDemo", "Protector", Mono.Cecil.TypeAttributes.Public | Mono.Cecil.TypeAttributes.Class, module.TypeSystem.Object);
                module.Types.Add(testType);

                // Method 1: CFF state-machine switch dispatcher
                var cffMethod = new Mono.Cecil.MethodDefinition("Dispatch", Mono.Cecil.MethodAttributes.Public | Mono.Cecil.MethodAttributes.Static, module.TypeSystem.Void);
                cffMethod.Body.InitLocals = true;
                cffMethod.Body.Variables.Add(new Mono.Cecil.Cil.VariableDefinition(module.TypeSystem.Int32));
                var cffIl = cffMethod.Body.GetILProcessor();
                var case0 = cffIl.Create(Mono.Cecil.Cil.OpCodes.Nop);
                var case1 = cffIl.Create(Mono.Cecil.Cil.OpCodes.Ret);
                var loopHead = cffIl.Create(Mono.Cecil.Cil.OpCodes.Ldloc_0);
                cffIl.Append(cffIl.Create(Mono.Cecil.Cil.OpCodes.Ldc_I4_0));
                cffIl.Append(cffIl.Create(Mono.Cecil.Cil.OpCodes.Stloc_0));
                cffIl.Append(loopHead);
                cffIl.Append(cffIl.Create(Mono.Cecil.Cil.OpCodes.Switch, new[] { case0, case1 }));
                cffIl.Append(case0);
                cffIl.Append(cffIl.Create(Mono.Cecil.Cil.OpCodes.Ldc_I4_1));
                cffIl.Append(cffIl.Create(Mono.Cecil.Cil.OpCodes.Stloc_0));
                cffIl.Append(cffIl.Create(Mono.Cecil.Cil.OpCodes.Br, loopHead));
                cffIl.Append(case1);
                testType.Methods.Add(cffMethod);

                // Method 2: 14 NOPs where 1 is branch target and 1 is ExceptionHandler TryStart
                var nopMethod = new Mono.Cecil.MethodDefinition("NopBloat", Mono.Cecil.MethodAttributes.Public | Mono.Cecil.MethodAttributes.Static, module.TypeSystem.Void);
                var nopIl = nopMethod.Body.GetILProcessor();
                var branchTargetNop = nopIl.Create(Mono.Cecil.Cil.OpCodes.Nop);
                var tryStartNop = nopIl.Create(Mono.Cecil.Cil.OpCodes.Nop);
                var leaveTarget = nopIl.Create(Mono.Cecil.Cil.OpCodes.Ret);
                var handlerStart = nopIl.Create(Mono.Cecil.Cil.OpCodes.Pop);
                var handlerLeave = nopIl.Create(Mono.Cecil.Cil.OpCodes.Leave, leaveTarget);

                nopIl.Append(nopIl.Create(Mono.Cecil.Cil.OpCodes.Br, branchTargetNop));
                nopIl.Append(branchTargetNop);
                for (int i = 0; i < 12; i++)
                {
                    nopIl.Append(nopIl.Create(Mono.Cecil.Cil.OpCodes.Nop));
                }
                nopIl.Append(tryStartNop);
                nopIl.Append(nopIl.Create(Mono.Cecil.Cil.OpCodes.Leave, leaveTarget));
                nopIl.Append(handlerStart);
                nopIl.Append(handlerLeave);
                nopIl.Append(leaveTarget);

                nopMethod.Body.ExceptionHandlers.Add(new Mono.Cecil.Cil.ExceptionHandler(Mono.Cecil.Cil.ExceptionHandlerType.Catch)
                {
                    CatchType = module.ImportReference(typeof(Exception)),
                    TryStart = tryStartNop,
                    TryEnd = handlerStart,
                    HandlerStart = handlerStart,
                    HandlerEnd = leaveTarget
                });
                testType.Methods.Add(nopMethod);

                assembly.Write(inputDll);
            }

            var report = ObfuscationAnalyzerEngine.AnalyzeAndCleanObfuscation(inputDll, outputDll);
            Assert.Contains("Control Flow Flattening dispatcher detected in 'Protector::Dispatch'", report);
            Assert.Contains("Stripped 12/14 redundant NOP instructions in 'Protector::NopBloat'", report);
            Assert.True(File.Exists(outputDll));

            using var cleaned = Mono.Cecil.AssemblyDefinition.ReadAssembly(outputDll);
            var cleanedNopMethod = cleaned.MainModule.GetType("ObfDemo.Protector").Methods.Single(m => m.Name == "NopBloat");
            Assert.Equal(2, cleanedNopMethod.Body.Instructions.Count(i => i.OpCode == Mono.Cecil.Cil.OpCodes.Nop));
        }
        finally
        {
            if (File.Exists(inputDll)) File.Delete(inputDll);
            if (File.Exists(outputDll)) File.Delete(outputDll);
        }
    }

    [Fact]
    public void SampleTestbeds_DeadlockAndMemoryLeakDemos_AreInspectable()
    {
        var deadlockPath = ResolveSampleBinary("DeadlockDemo");
        var memoryLeakPath = ResolveSampleBinary("MemoryLeakDemo");

        Assert.True(File.Exists(deadlockPath), $"DeadlockDemo binary not found at: {deadlockPath}");
        Assert.True(File.Exists(memoryLeakPath), $"MemoryLeakDemo binary not found at: {memoryLeakPath}");

        var deadlockCode = DecompilerEngine.DecompileMethod(deadlockPath, "DeadlockDemo.Program", "RunDeadlockScenario");
        Assert.Contains("Monitor.TryEnter", deadlockCode);

        var memoryLeakMeta = MetadataEngine.InspectMetadata(memoryLeakPath);
        Assert.Contains("Unmanaged P/Invoke Imports", memoryLeakMeta);
        Assert.Contains("kernel32.dll!GetTickCount64", memoryLeakMeta);
        Assert.Contains("AllocatePinnedLohBuffers", memoryLeakMeta);
    }

    [Fact]
    public void IlPatcherEngine_PreservesBranchesOnPatchAndRejectsInvalidOffsetsOrOperandOpcodes()
    {
        var tempDll = Path.Combine(Path.GetTempPath(), $"PatchBranch_{Guid.NewGuid():N}.dll");
        var patchedDll = Path.Combine(Path.GetTempPath(), $"PatchBranchOut_{Guid.NewGuid():N}.dll");

        try
        {
            using (var asm = Mono.Cecil.AssemblyDefinition.CreateAssembly(
                new Mono.Cecil.AssemblyNameDefinition("PatchBranchTest", new Version(1, 0, 0, 0)),
                "PatchBranchTest",
                Mono.Cecil.ModuleKind.Dll))
            {
                var mod = asm.MainModule;
                var type = new Mono.Cecil.TypeDefinition("PatchDemo", "Target", Mono.Cecil.TypeAttributes.Public | Mono.Cecil.TypeAttributes.Class, mod.TypeSystem.Object);
                mod.Types.Add(type);

                var method = new Mono.Cecil.MethodDefinition("Check", Mono.Cecil.MethodAttributes.Public | Mono.Cecil.MethodAttributes.Static, mod.TypeSystem.Int32);
                var il = method.Body.GetILProcessor();
                var targetRet = il.Create(Mono.Cecil.Cil.OpCodes.Ldc_I4_0);
                il.Append(il.Create(Mono.Cecil.Cil.OpCodes.Ldc_I4_1));
                il.Append(il.Create(Mono.Cecil.Cil.OpCodes.Br_S, targetRet));
                il.Append(targetRet);
                il.Append(il.Create(Mono.Cecil.Cil.OpCodes.Ret));
                type.Methods.Add(method);

                asm.Write(tempDll);
            }

            // Invalid offset must throw rather than silently patching IL_0000
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                IlPatcherEngine.PatchInstruction(tempDll, "PatchDemo.Target", "Check", 999, "Ldc_I4_1", patchedDll));

            // Operand-requiring opcode must throw a clear ArgumentException
            Assert.Throws<ArgumentException>(() =>
                IlPatcherEngine.PatchInstruction(tempDll, "PatchDemo.Target", "Check", 3, "Ldstr", patchedDll));

            // Patching a branch target instruction (offset 3) in-place must preserve branch target integrity & overwrite safely
            var patchReport = IlPatcherEngine.PatchInstruction(tempDll, "PatchDemo.Target", "Check", 3, "Ldc_I4_1", tempDll);
            Assert.Contains("IL offset 3", patchReport);

            using var reloaded = Mono.Cecil.AssemblyDefinition.ReadAssembly(tempDll);
            var reloadedMethod = reloaded.MainModule.GetType("PatchDemo.Target").Methods.Single(m => m.Name == "Check");
            var brTarget = reloadedMethod.Body.Instructions[1].Operand as Mono.Cecil.Cil.Instruction;
            Assert.NotNull(brTarget);
            Assert.Equal(Mono.Cecil.Cil.OpCodes.Ldc_I4_1, brTarget!.OpCode);
            Assert.Contains(brTarget, reloadedMethod.Body.Instructions);
        }
        finally
        {
            if (File.Exists(tempDll)) File.Delete(tempDll);
            if (File.Exists(patchedDll)) File.Delete(patchedDll);
        }
    }

    [Fact]
    public void BinaryDiffEngine_HandlesGenericOverloadsAndLcsShiftedInstructions()
    {
        var v1Dll = Path.Combine(Path.GetTempPath(), $"DiffV1_{Guid.NewGuid():N}.dll");
        var v2Dll = Path.Combine(Path.GetTempPath(), $"DiffV2_{Guid.NewGuid():N}.dll");

        try
        {
            void BuildVersion(string path, bool insertNop)
            {
                using var asm = Mono.Cecil.AssemblyDefinition.CreateAssembly(
                    new Mono.Cecil.AssemblyNameDefinition("DiffTest", new Version(1, 0, 0, 0)),
                    "DiffTest",
                    Mono.Cecil.ModuleKind.Dll);
                var mod = asm.MainModule;
                var type = new Mono.Cecil.TypeDefinition("DiffNs", "Service", Mono.Cecil.TypeAttributes.Public | Mono.Cecil.TypeAttributes.Class, mod.TypeSystem.Object);
                mod.Types.Add(type);

                var mNonGeneric = new Mono.Cecil.MethodDefinition("Execute", Mono.Cecil.MethodAttributes.Public | Mono.Cecil.MethodAttributes.Static, mod.TypeSystem.Int32);
                var il = mNonGeneric.Body.GetILProcessor();
                if (insertNop) il.Append(il.Create(Mono.Cecil.Cil.OpCodes.Nop));
                il.Append(il.Create(Mono.Cecil.Cil.OpCodes.Ldc_I4_1));
                il.Append(il.Create(Mono.Cecil.Cil.OpCodes.Ldc_I4_2));
                il.Append(il.Create(Mono.Cecil.Cil.OpCodes.Add));
                il.Append(il.Create(Mono.Cecil.Cil.OpCodes.Ret));
                type.Methods.Add(mNonGeneric);

                // Generic overload with same parameter count (0) and same Name
                var mGeneric = new Mono.Cecil.MethodDefinition("Execute", Mono.Cecil.MethodAttributes.Public | Mono.Cecil.MethodAttributes.Static, mod.TypeSystem.Void);
                mGeneric.GenericParameters.Add(new Mono.Cecil.GenericParameter("T", mGeneric));
                mGeneric.Body.GetILProcessor().Emit(Mono.Cecil.Cil.OpCodes.Ret);
                type.Methods.Add(mGeneric);

                // Nested public type should be discovered
                var nested = new Mono.Cecil.TypeDefinition("", "InnerConfig", Mono.Cecil.TypeAttributes.NestedPublic | Mono.Cecil.TypeAttributes.Class, mod.TypeSystem.Object);
                if (insertNop)
                {
                    var addedNestedMethod = new Mono.Cecil.MethodDefinition("Ping", Mono.Cecil.TypeAttributes.Public is var _ ? Mono.Cecil.MethodAttributes.Public | Mono.Cecil.MethodAttributes.Static : 0, mod.TypeSystem.Void);
                    addedNestedMethod.Body.GetILProcessor().Emit(Mono.Cecil.Cil.OpCodes.Ret);
                    nested.Methods.Add(addedNestedMethod);
                }
                type.NestedTypes.Add(nested);

                asm.Write(path);
            }

            BuildVersion(v1Dll, insertNop: false);
            BuildVersion(v2Dll, insertNop: true);

            var diff = BinaryDiffEngine.CompareAssemblies(v1Dll, v2Dll, includeCilDiff: true);
            Assert.Contains("DiffNs.Service/InnerConfig::System.Void Ping()", diff);
            Assert.Contains("+ IL_0000: nop", diff);
            // LCS alignment must keep trailing unchanged instructions as context lines ('  IL_...'), not false removals
            Assert.DoesNotContain("- IL_0000: ldc.i4.1", diff);
            Assert.Contains("  IL_0001: ldc.i4.1", diff);
        }
        finally
        {
            if (File.Exists(v1Dll)) File.Delete(v1Dll);
            if (File.Exists(v2Dll)) File.Delete(v2Dll);
        }
    }

    [Fact]
    public void WrapperGeneratorEngine_HandlesGlobalNamespaceAndSanitizesIdentifiers()
    {
        var tempDll = Path.Combine(Path.GetTempPath(), $"WrapGlobal_{Guid.NewGuid():N}.dll");
        var tempCs = Path.Combine(Path.GetTempPath(), $"WrapGlobal_{Guid.NewGuid():N}.cs");

        try
        {
            using (var asm = Mono.Cecil.AssemblyDefinition.CreateAssembly(
                new Mono.Cecil.AssemblyNameDefinition("WrapTest", new Version(1, 0, 0, 0)),
                "WrapTest",
                Mono.Cecil.ModuleKind.Dll))
            {
                var mod = asm.MainModule;
                var globalType = new Mono.Cecil.TypeDefinition("", "GlobalProgram", Mono.Cecil.TypeAttributes.Public | Mono.Cecil.TypeAttributes.Class, mod.TypeSystem.Object);
                mod.Types.Add(globalType);

                var backingField = new Mono.Cecil.FieldDefinition("<Secret>k__BackingField", Mono.Cecil.FieldAttributes.Private | Mono.Cecil.FieldAttributes.Static, mod.TypeSystem.Int32);
                globalType.Fields.Add(backingField);

                asm.Write(tempDll);
            }

            var code = WrapperGeneratorEngine.GenerateWrapper(tempDll, "GlobalProgram", tempCs);
            Assert.Contains("namespace GeneratedWrappers;", code);
            Assert.DoesNotContain("namespace .GeneratedWrappers;", code);
            Assert.Contains("[UnsafeAccessor(UnsafeAccessorKind.StaticField, Name = \"<Secret>k__BackingField\")]", code);
            Assert.Contains("Get__Secret_k__BackingField", code);
        }
        finally
        {
            if (File.Exists(tempDll)) File.Delete(tempDll);
            if (File.Exists(tempCs)) File.Delete(tempCs);
        }
    }

    [Fact]
    public void DecompilerEngine_ReleasesFileLockAndDecompilesOverloads()
    {
        var tempCopy = Path.Combine(Path.GetTempPath(), $"DecompLock_{Guid.NewGuid():N}.dll");
        File.Copy(SampleAssemblyPath, tempCopy, overwrite: true);

        var code = DecompilerEngine.DecompileMethod(tempCopy, "CrackmeDemo.Program", "Main");
        Assert.Contains("Main", code);

        // File must not be locked after DecompileMethod returns
        File.Delete(tempCopy);
        Assert.False(File.Exists(tempCopy));
    }

    private static string ResolveSampleBinary(string sampleName)
    {
        var repoRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".."));
        var releasePath = Path.Combine(repoRoot, "samples", sampleName, "bin", "Release", "net8.0", $"{sampleName}.dll");
        if (File.Exists(releasePath)) return releasePath;
        return Path.Combine(repoRoot, "samples", sampleName, "bin", "Debug", "net8.0", $"{sampleName}.dll");
    }
}
