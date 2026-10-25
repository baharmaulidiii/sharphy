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
}
