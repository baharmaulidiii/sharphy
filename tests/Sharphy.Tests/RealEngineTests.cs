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
}
