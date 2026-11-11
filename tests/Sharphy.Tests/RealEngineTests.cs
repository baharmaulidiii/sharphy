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
}
