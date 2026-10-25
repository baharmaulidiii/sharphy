using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.TypeSystem;

namespace Sharphy.Core.Engines;

/// <summary>
/// Provides CIL method decompilation powered by ICSharpCode.Decompiler (ILSpy engine).
/// </summary>
public static class DecompilerEngine
{
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

        var decompiler = new CSharpDecompiler(assemblyPath, settings);

        // Resolve target type definition within MainModule
        var targetType = decompiler.TypeSystem.MainModule.TypeDefinitions
            .FirstOrDefault(t => t.FullName.Equals(typeName, StringComparison.OrdinalIgnoreCase) ||
                                 t.Name.Equals(typeName, StringComparison.OrdinalIgnoreCase));

        if (targetType == null)
        {
            return $"// [Sharphy Decompiler] Type '{typeName}' was not found in assembly '{Path.GetFileName(assemblyPath)}'.";
        }

        var method = targetType.Methods
            .FirstOrDefault(m => m.Name.Equals(methodName, StringComparison.OrdinalIgnoreCase));

        if (method == null)
        {
            return $"// [Sharphy Decompiler] Method '{methodName}' was not found in type '{targetType.FullName}'.";
        }

        return decompiler.DecompileAsString(method.MetadataToken);
    }
}
