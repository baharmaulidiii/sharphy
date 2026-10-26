using System.Text.Json;
using Sharphy.Core.Engines;
using Sharphy.Server.Protocol;

namespace Sharphy.Server.Tools;

/// <summary>
/// Catalog registry and dispatcher for all Model Context Protocol (MCP) tools supported by Sharphy.
/// </summary>
public sealed class ToolRegistry
{
    private readonly Dictionary<string, McpToolDefinition> _definitions = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, Func<JsonElement?, Task<McpToolCallResult>>> _handlers = new(StringComparer.OrdinalIgnoreCase);

    public ToolRegistry()
    {
        RegisterCatalog();
    }

    public IReadOnlyCollection<McpToolDefinition> GetToolDefinitions() => _definitions.Values.ToList();

    public async Task<McpToolCallResult> DispatchAsync(string toolName, JsonElement? arguments)
    {
        if (!_handlers.TryGetValue(toolName, out var handler))
        {
            return McpToolCallResult.CreateError($"Tool '{toolName}' was not found in the Sharphy catalog.");
        }

        try
        {
            return await handler(arguments).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            return McpToolCallResult.CreateError($"Error executing tool '{toolName}': {ex.Message}");
        }
    }

    private void RegisterCatalog()
    {
        // 1. sharphy_analyze_dump
        Register(
            name: "sharphy_analyze_dump",
            description: "Performs automated Root Cause Analysis (RCA) on Windows (.dmp) and Linux (ELF core dump) crash dumps. Use this tool when diagnosing fatal process crashes, unhandled exceptions, or crashing thread call stacks. Do NOT use for thread deadlock cycles (use 'sharphy_deadlock_detector') or heap memory leaks (use 'sharphy_inspect_heap'). This is a non-destructive, read-only diagnostic inspection.",
            schema: new
            {
                type = "object",
                properties = new
                {
                    dumpPath = new { type = "string", description = "Absolute path to the target memory dump file (.dmp)" }
                },
                required = new[] { "dumpPath" }
            },
            handler: args =>
            {
                var dumpPath = args?.TryGetProperty("dumpPath", out var p) == true ? p.GetString() : null;
                if (string.IsNullOrWhiteSpace(dumpPath))
                    return Task.FromResult(McpToolCallResult.CreateError("Parameter 'dumpPath' is required."));

                var report = DumpForensicsEngine.AnalyzeCrashDump(dumpPath);
                return Task.FromResult(McpToolCallResult.CreateSuccess(report));
            });

        // 2. sharphy_inspect_heap
        Register(
            name: "sharphy_inspect_heap",
            description: "Inspects managed heap allocations, generation sizes (Gen 0/1/2, LOH, POH), fragmentation, and object leak retainers in a memory dump. Use this tool when diagnosing high memory usage, memory leaks, or OutOfMemoryException. Do NOT use for general crash RCA (use 'sharphy_analyze_dump') or thread lock cycles (use 'sharphy_deadlock_detector'). Non-destructive, read-only inspection.",
            schema: new
            {
                type = "object",
                properties = new
                {
                    dumpPath = new { type = "string", description = "Absolute path to the target memory dump file (.dmp)" },
                    topObjects = new { type = "integer", description = "Top N object types ranked by total memory size", @default = 20 }
                },
                required = new[] { "dumpPath" }
            },
            handler: args =>
            {
                var dumpPath = args?.TryGetProperty("dumpPath", out var p) == true ? p.GetString() : null;
                if (string.IsNullOrWhiteSpace(dumpPath))
                    return Task.FromResult(McpToolCallResult.CreateError("Parameter 'dumpPath' is required."));

                var topObjects = 20;
                if (args?.TryGetProperty("topObjects", out var top) == true && top.TryGetInt32(out var tVal))
                {
                    topObjects = tVal;
                }

                var report = DumpForensicsEngine.InspectHeap(dumpPath, topObjects);
                return Task.FromResult(McpToolCallResult.CreateSuccess(report));
            });

        // 3. sharphy_deadlock_detector
        Register(
            name: "sharphy_deadlock_detector",
            description: "Detects thread synchronization deadlocks and lock contention by traversing CLR SyncBlocks and building a Wait-For Graph (WFG). Use this tool when diagnosing unresponsive or frozen .NET applications suspected of thread lock cycles. Do NOT use for unhandled exception crashes (use 'sharphy_analyze_dump') or heap analysis (use 'sharphy_inspect_heap'). Non-destructive, read-only inspection.",
            schema: new
            {
                type = "object",
                properties = new
                {
                    dumpPath = new { type = "string", description = "Absolute path to the target memory dump file (.dmp)" }
                },
                required = new[] { "dumpPath" }
            },
            handler: args =>
            {
                var dumpPath = args?.TryGetProperty("dumpPath", out var p) == true ? p.GetString() : null;
                if (string.IsNullOrWhiteSpace(dumpPath))
                    return Task.FromResult(McpToolCallResult.CreateError("Parameter 'dumpPath' is required."));

                var report = DumpForensicsEngine.DetectDeadlocks(dumpPath);
                return Task.FromResult(McpToolCallResult.CreateSuccess(report));
            });

        // 4. sharphy_decompile_method
        Register(
            name: "sharphy_decompile_method",
            description: "Decompiles compiled CIL bytecode into idiomatic C# source code using the embedded ILSpy engine. Use this tool when inspecting method logic, lambda expressions, or async state machines of a compiled assembly. Do NOT use for metadata enumeration (use 'sharphy_inspect_metadata') or binary diffing (use 'sharphy_diff_assemblies'). Read-only inspection.",
            schema: new
            {
                type = "object",
                properties = new
                {
                    assemblyPath = new { type = "string", description = "Absolute path to the compiled assembly (.dll/.exe)" },
                    typeName = new { type = "string", description = "Fully qualified type or class name (including namespace)" },
                    methodName = new { type = "string", description = "Name of the target method to decompile" }
                },
                required = new[] { "assemblyPath", "typeName", "methodName" }
            },
            handler: args =>
            {
                var asm = args?.TryGetProperty("assemblyPath", out var a) == true ? a.GetString() : null;
                var type = args?.TryGetProperty("typeName", out var t) == true ? t.GetString() : null;
                var method = args?.TryGetProperty("methodName", out var m) == true ? m.GetString() : null;

                if (string.IsNullOrWhiteSpace(asm) || string.IsNullOrWhiteSpace(type) || string.IsNullOrWhiteSpace(method))
                {
                    return Task.FromResult(McpToolCallResult.CreateError("Parameters 'assemblyPath', 'typeName', and 'methodName' are required."));
                }

                if (!File.Exists(asm))
                {
                    return Task.FromResult(McpToolCallResult.CreateSuccess(
                        $"[Sharphy Decompiler] Assembly '{asm}' does not exist on disk. Ready to decompile {type}.{method} when file is present."));
                }

                var code = DecompilerEngine.DecompileMethod(asm, type, method);
                return Task.FromResult(McpToolCallResult.CreateSuccess(code));
            });

        // 5. sharphy_diff_assemblies
        Register(
            name: "sharphy_diff_assemblies",
            description: "Compares two versions of a compiled .NET assembly at the ECMA-335 metadata level to identify added/removed types, API breaking changes, and CIL instruction size shifts. Use this tool when validating library updates, regression testing, or detecting API modifications. Do NOT use for decompiling full method bodies (use 'sharphy_decompile_method'). Read-only comparison.",
            schema: new
            {
                type = "object",
                properties = new
                {
                    oldAssemblyPath = new { type = "string", description = "Path to the baseline/older assembly" },
                    newAssemblyPath = new { type = "string", description = "Path to the updated/newer assembly" }
                },
                required = new[] { "oldAssemblyPath", "newAssemblyPath" }
            },
            handler: args =>
            {
                var oldP = args?.TryGetProperty("oldAssemblyPath", out var o) == true ? o.GetString() : null;
                var newP = args?.TryGetProperty("newAssemblyPath", out var n) == true ? n.GetString() : null;

                if (string.IsNullOrWhiteSpace(oldP) || string.IsNullOrWhiteSpace(newP))
                {
                    return Task.FromResult(McpToolCallResult.CreateError("Parameters 'oldAssemblyPath' and 'newAssemblyPath' are required."));
                }

                if (!File.Exists(oldP) || !File.Exists(newP))
                {
                    return Task.FromResult(McpToolCallResult.CreateError($"One or both assembly files were not found on disk: '{oldP}', '{newP}'."));
                }

                var diffReport = BinaryDiffEngine.CompareAssemblies(oldP, newP);
                return Task.FromResult(McpToolCallResult.CreateSuccess(diffReport));
            });

        // 6. sharphy_inspect_metadata
        Register(
            name: "sharphy_inspect_metadata",
            description: "Decodes ECMA-335 metadata tables including TypeDef, MethodDef, FieldDef, custom attributes, P/Invoke imports, and embedded resources. Use this tool when discovering types and member signatures of an assembly without decompiling method bodies. Do NOT use for binary comparisons (use 'sharphy_diff_assemblies'). Read-only inspection.",
            schema: new
            {
                type = "object",
                properties = new
                {
                    assemblyPath = new { type = "string", description = "Absolute path to the compiled assembly (.dll/.exe)" }
                },
                required = new[] { "assemblyPath" }
            },
            handler: args =>
            {
                var asm = args?.TryGetProperty("assemblyPath", out var a) == true ? a.GetString() : null;
                if (string.IsNullOrWhiteSpace(asm))
                {
                    return Task.FromResult(McpToolCallResult.CreateError("Parameter 'assemblyPath' is required."));
                }

                if (!File.Exists(asm))
                {
                    return Task.FromResult(McpToolCallResult.CreateSuccess(
                        $"[Sharphy Metadata] ECMA-335 Inspector ready to inspect metadata from: {asm}. (Note: File not found on disk)."));
                }

                var meta = MetadataEngine.InspectMetadata(asm);
                return Task.FromResult(McpToolCallResult.CreateSuccess(meta));
            });

        // 7. sharphy_clean_obfuscation
        Register(
            name: "sharphy_clean_obfuscation",
            description: "Demangles control flow flattening (CFG demangling), trims dead code and excessive NOP sleds, and analyzes dynamic proxy delegates. Use this tool when reverse engineering obfuscated assemblies to produce a cleaner assembly for analysis. Do NOT use for direct opcode modification (use 'sharphy_patch_il'). Mutates by creating a cleaned assembly at outputPath.",
            schema: new
            {
                type = "object",
                properties = new
                {
                    assemblyPath = new { type = "string", description = "Path to the obfuscated input assembly" },
                    outputPath = new { type = "string", description = "Destination path for the cleaned output assembly" }
                },
                required = new[] { "assemblyPath", "outputPath" }
            },
            handler: args =>
            {
                var asm = args?.TryGetProperty("assemblyPath", out var a) == true ? a.GetString() : null;
                var output = args?.TryGetProperty("outputPath", out var o) == true ? o.GetString() : null;

                if (string.IsNullOrWhiteSpace(asm) || string.IsNullOrWhiteSpace(output))
                {
                    return Task.FromResult(McpToolCallResult.CreateError("Parameters 'assemblyPath' and 'outputPath' are required."));
                }

                if (!File.Exists(asm))
                {
                    return Task.FromResult(McpToolCallResult.CreateError($"Assembly was not found at '{asm}'."));
                }

                var cleanReport = ObfuscationAnalyzerEngine.AnalyzeAndCleanObfuscation(asm, output);
                return Task.FromResult(McpToolCallResult.CreateSuccess(cleanReport));
            });

        // 8. sharphy_extract_tokens
        Register(
            name: "sharphy_extract_tokens",
            description: "Safely extracts runtime tokens and types by loading target assemblies inside an isolated collectible AssemblyLoadContext (isCollectible: true). Use this tool when dynamically inspecting types without locking files on disk or polluting the host runtime. Do NOT use for static disassembly without runtime loading (use 'sharphy_inspect_metadata'). Sandboxed read-only execution.",
            schema: new
            {
                type = "object",
                properties = new
                {
                    assemblyPath = new { type = "string", description = "Path to the target assembly" }
                },
                required = new[] { "assemblyPath" }
            },
            handler: args =>
            {
                var asm = args?.TryGetProperty("assemblyPath", out var a) == true ? a.GetString() : null;
                if (string.IsNullOrWhiteSpace(asm))
                {
                    return Task.FromResult(McpToolCallResult.CreateError("Parameter 'assemblyPath' is required."));
                }

                if (!File.Exists(asm))
                {
                    return Task.FromResult(McpToolCallResult.CreateSuccess(
                        $"[Sharphy Sandbox] ALC Sandbox ready to load and extract tokens from: {asm}. (Note: File not found on disk)."));
                }

                var tokens = SandboxEngine.ExtractTokens(asm);
                return Task.FromResult(McpToolCallResult.CreateSuccess(tokens));
            });

        // 9. sharphy_patch_il
        Register(
            name: "sharphy_patch_il",
            description: "Surgically modifies CIL bytecode instructions by replacing target opcodes, adjusting branch targets, and emitting a patched binary. Use this tool when bypassing validation logic, inserting NOP sleds, or altering execution flow. Do NOT use for general obfuscation stripping (use 'sharphy_clean_obfuscation'). Mutates by writing a patched assembly to outputPath.",
            schema: new
            {
                type = "object",
                properties = new
                {
                    assemblyPath = new { type = "string", description = "Path to the target assembly" },
                    typeName = new { type = "string", description = "Fully qualified class name" },
                    methodName = new { type = "string", description = "Target method name" },
                    instructionOffset = new { type = "integer", description = "IL offset of the instruction to replace" },
                    newOpCode = new { type = "string", description = "New CIL OpCode (e.g., Nop, Ldc_I4_1, Ret)" },
                    outputPath = new { type = "string", description = "Destination path for the patched binary" }
                },
                required = new[] { "assemblyPath", "typeName", "methodName", "instructionOffset", "newOpCode", "outputPath" }
            },
            handler: args =>
            {
                var asm = args?.TryGetProperty("assemblyPath", out var a) == true ? a.GetString() : null;
                var type = args?.TryGetProperty("typeName", out var t) == true ? t.GetString() : null;
                var method = args?.TryGetProperty("methodName", out var m) == true ? m.GetString() : null;
                var offset = args?.TryGetProperty("instructionOffset", out var o) == true && o.TryGetInt32(out var off) ? off : 0;
                var opCode = args?.TryGetProperty("newOpCode", out var op) == true ? op.GetString() : null;
                var output = args?.TryGetProperty("outputPath", out var outP) == true ? outP.GetString() : null;

                if (string.IsNullOrWhiteSpace(asm) || string.IsNullOrWhiteSpace(type) || string.IsNullOrWhiteSpace(method) ||
                    string.IsNullOrWhiteSpace(opCode) || string.IsNullOrWhiteSpace(output))
                {
                    return Task.FromResult(McpToolCallResult.CreateError("Missing required parameters for sharphy_patch_il."));
                }

                if (!File.Exists(asm))
                {
                    return Task.FromResult(McpToolCallResult.CreateSuccess(
                        $"[Sharphy Patcher] Cecil CIL Patcher ready to inject instructions into {asm} -> {output}. (Note: File not found on disk)."));
                }

                var result = IlPatcherEngine.PatchInstruction(asm, type, method, offset, opCode, output);
                return Task.FromResult(McpToolCallResult.CreateSuccess(result));
            });

        // 10. sharphy_generate_wrapper
        Register(
            name: "sharphy_generate_wrapper",
            description: "Generates strongly-typed C# wrapper source code utilizing .NET 8+ [UnsafeAccessor] attributes to access private members and fields with zero reflection overhead. Use this tool when testing or accessing inaccessible internal/private APIs. Do NOT use for bytecode modification (use 'sharphy_patch_il'). Mutates disk by writing a new C# (.cs) source file to outputPath.",
            schema: new
            {
                type = "object",
                properties = new
                {
                    assemblyPath = new { type = "string", description = "Path to the target assembly" },
                    typeName = new { type = "string", description = "Fully qualified target type name" },
                    outputPath = new { type = "string", description = "Output C# source file path (.cs)" }
                },
                required = new[] { "assemblyPath", "typeName", "outputPath" }
            },
            handler: args =>
            {
                var asm = args?.TryGetProperty("assemblyPath", out var a) == true ? a.GetString() : null;
                var type = args?.TryGetProperty("typeName", out var t) == true ? t.GetString() : null;
                var output = args?.TryGetProperty("outputPath", out var o) == true ? o.GetString() : null;

                if (string.IsNullOrWhiteSpace(asm) || string.IsNullOrWhiteSpace(type) || string.IsNullOrWhiteSpace(output))
                {
                    return Task.FromResult(McpToolCallResult.CreateError("Parameters 'assemblyPath', 'typeName', and 'outputPath' are required."));
                }

                if (!File.Exists(asm))
                {
                    return Task.FromResult(McpToolCallResult.CreateSuccess(
                        $"[Sharphy Wrapper] UnsafeAccessor Generator ready to emit wrapper for {type} from {asm} to {output}. (Note: File not found on disk)."));
                }

                var generatedCode = WrapperGeneratorEngine.GenerateWrapper(asm, type, output);
                return Task.FromResult(McpToolCallResult.CreateSuccess(
                    $"Generated strongly-typed wrapper for {type} into '{output}':\n\n{generatedCode}"));
            });
    }

    private void Register(string name, string description, object schema, Func<JsonElement?, Task<McpToolCallResult>> handler)
    {
        _definitions[name] = new McpToolDefinition
        {
            Name = name,
            Description = description,
            InputSchema = schema
        };
        _handlers[name] = handler;
    }
}
