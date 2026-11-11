using System.Reflection.PortableExecutable;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;

namespace Sharphy.Core.Engines;

/// <summary>
/// Provides CIL method decompilation powered by ICSharpCode.Decompiler (ILSpy engine).
/// </summary>
public static class DecompilerEngine
{
    private const int MaxOverloadsToDecompile = 10;

    public static string DecompileMethod(string assemblyPath, string typeName, string methodName)
    {
        if (!File.Exists(assemblyPath))
        {
            throw new FileNotFoundException($"Assembly file was not found at '{assemblyPath}'.", assemblyPath);
        }

        var settings = new DecompilerSettings(LanguageVersion.Latest)
        {
            ThrowOnAssemblyResolveErrors = false,
            RemoveDeadCode = false,
            AlwaysUseBraces = true
        };

        using var peStream = new FileStream(assemblyPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
        using var peFile = new PEFile(assemblyPath, peStream, PEStreamOptions.Default);
        var resolver = new UniversalAssemblyResolver(assemblyPath, throwOnError: false, peFile.DetectTargetFrameworkId());
        var decompiler = new CSharpDecompiler(peFile, resolver, settings);

        var normalizedTypeName = typeName.Replace('/', '.').Replace('+', '.');

        // Resolve target type definition within MainModule (supports top-level and nested types)
        var targetType = decompiler.TypeSystem.MainModule.TypeDefinitions
            .FirstOrDefault(t => t.FullName.Equals(normalizedTypeName, StringComparison.OrdinalIgnoreCase) ||
                                 t.ReflectionName.Equals(typeName, StringComparison.OrdinalIgnoreCase) ||
                                 t.Name.Equals(typeName, StringComparison.OrdinalIgnoreCase));

        if (targetType == null)
        {
            return $"// [Sharphy Decompiler] Type '{typeName}' was not found in assembly '{Path.GetFileName(assemblyPath)}'.";
        }

        var methods = targetType.Methods
            .Where(m => m.Name.Equals(methodName, StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (methods.Count == 0)
        {
            return $"// [Sharphy Decompiler] Method '{methodName}' was not found in type '{targetType.FullName}'.";
        }

        if (methods.Count == 1)
        {
            return decompiler.DecompileAsString(methods[0].MetadataToken);
        }

        var chunks = methods
            .Take(MaxOverloadsToDecompile)
            .Select(m => decompiler.DecompileAsString(m.MetadataToken));

        var result = string.Join(Environment.NewLine + Environment.NewLine, chunks);
        if (methods.Count > MaxOverloadsToDecompile)
        {
            result += $"{Environment.NewLine}// ... ({methods.Count - MaxOverloadsToDecompile} additional overloads omitted)";
        }

        return result;
    }
}
