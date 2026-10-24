using System.Text.Json;
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
            description: "Performs automated Root Cause Analysis (RCA) on Windows / Linux crash dump (.dmp) files.",
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

                return Task.FromResult(McpToolCallResult.CreateSuccess(
                    $"[Sharphy RCA] Dump Analysis Engine ready to process: {dumpPath}."));
            });

        // 2. sharphy_inspect_heap
        Register(
            name: "sharphy_inspect_heap",
            description: "Inspects managed heap objects, detects memory leaks, duplicate strings, and LOH fragmentation.",
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
                return Task.FromResult(McpToolCallResult.CreateSuccess(
                    $"[Sharphy Heap] Managed Heap Inspector ready to scan objects in dump: {dumpPath}."));
            });

        // 3. sharphy_deadlock_detector
        Register(
            name: "sharphy_deadlock_detector",
            description: "Detects inter-thread deadlocks and sync-block lock contention across threads in a crash dump.",
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
                return Task.FromResult(McpToolCallResult.CreateSuccess(
                    $"[Sharphy Deadlock] Deadlock Graph Traversal ready to analyze lock cycles in: {dumpPath}."));
            });

        // 4. sharphy_decompile_method
        Register(
            name: "sharphy_decompile_method",
            description: "Decompiles a specific CIL method from an assembly into clean, modern C# source code.",
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
                return Task.FromResult(McpToolCallResult.CreateSuccess(
                    $"[Sharphy Decompiler] ILSpy Engine ready to reconstruct {type}.{method} from {asm}."));
            });

        // 5. sharphy_diff_assemblies
        Register(
            name: "sharphy_diff_assemblies",
            description: "Compares two binary assembly versions to identify breaking API changes and internal logic modifications.",
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
                return Task.FromResult(McpToolCallResult.CreateSuccess(
                    $"[Sharphy Diff] Comparing baseline assembly: {oldP} with updated version: {newP}."));
            });

        // 6. sharphy_inspect_metadata
        Register(
            name: "sharphy_inspect_metadata",
            description: "Dumps ECMA-335 type hierarchies, generic arguments, P/Invoke signatures, and embedded resources.",
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
                return Task.FromResult(McpToolCallResult.CreateSuccess(
                    $"[Sharphy Metadata] ECMA-335 Inspector ready to inspect metadata from: {asm}."));
            });

        // 7. sharphy_clean_obfuscation
        Register(
            name: "sharphy_clean_obfuscation",
            description: "Analyzes and demangles common obfuscation patterns (CFG flattening, proxy delegates, string decrypters).",
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
                return Task.FromResult(McpToolCallResult.CreateSuccess(
                    $"[Sharphy Deobfuscator] Obfuscation Analyzer ready to inspect {asm} -> {output}."));
            });

        // 8. sharphy_extract_tokens
        Register(
            name: "sharphy_extract_tokens",
            description: "Extracts decryption tokens safely inside an isolated AssemblyLoadContext sandbox.",
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
                return Task.FromResult(McpToolCallResult.CreateSuccess(
                    $"[Sharphy Sandbox] ALC Sandbox ready to load and extract tokens from: {asm}."));
            });

        // 9. sharphy_patch_il
        Register(
            name: "sharphy_patch_il",
            description: "Replaces or injects CIL instructions (e.g. nop-ing validation routines) and emits a patched binary.",
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
                var output = args?.TryGetProperty("outputPath", out var o) == true ? o.GetString() : null;
                return Task.FromResult(McpToolCallResult.CreateSuccess(
                    $"[Sharphy Patcher] Cecil CIL Patcher ready to inject instructions into {asm} -> {output}."));
            });

        // 10. sharphy_generate_wrapper
        Register(
            name: "sharphy_generate_wrapper",
            description: "Generates strongly-typed C# wrapper using [UnsafeAccessor] or Roslyn targeting private members.",
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
                return Task.FromResult(McpToolCallResult.CreateSuccess(
                    $"[Sharphy Wrapper] UnsafeAccessor Generator ready to emit wrapper for {type} from {asm} to {output}."));
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
