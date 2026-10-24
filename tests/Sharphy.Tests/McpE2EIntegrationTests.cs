using System.Text.Json;
using Sharphy.Server;
using Sharphy.Server.Protocol;
using Sharphy.Server.Tools;
using Xunit;

namespace Sharphy.Tests;

public class McpE2EIntegrationTests
{
    [Fact]
    public async Task E2E_FullMcpSessionLifecycleOverStdioStreams()
    {
        // 1. Arrange In-Memory Stdio Streams simulating real CLI process
        var clientToServerReader = new StringReader(
            """
            {"jsonrpc":"2.0","id":1,"method":"initialize","params":{}}
            {"jsonrpc":"2.0","method":"notifications/initialized"}
            {"jsonrpc":"2.0","id":2,"method":"tools/list","params":{}}
            {"jsonrpc":"2.0","id":3,"method":"tools/call","params":{"name":"sharphy_inspect_metadata","arguments":{"assemblyPath":"samples/CrackmeDemo.dll"}}}
            {"jsonrpc":"2.0","id":4,"method":"tools/call","params":{"name":"sharphy_extract_tokens","arguments":{"assemblyPath":"samples/CrackmeDemo.dll"}}}
            {"jsonrpc":"2.0","id":5,"method":"tools/call","params":{"name":"sharphy_generate_wrapper","arguments":{"assemblyPath":"samples/CrackmeDemo.dll","typeName":"Demo.Secret","outputPath":"Wrapper.cs"}}}
            {"jsonrpc":"2.0","id":6,"method":"tools/call","params":{"name":"unknown_tool","arguments":{}}}
            {"jsonrpc":"2.0","id":7,"method":"ping","params":{}}
            """ + "\n");

        var serverToClientWriter = new StringWriter();
        var serverDiagnosticWriter = new StringWriter();

        var server = new SharphyMcpServer(
            toolRegistry: new ToolRegistry(),
            input: clientToServerReader,
            output: serverToClientWriter,
            error: serverDiagnosticWriter);

        // 2. Act: Run server until stream EOF
        await server.RunAsync(CancellationToken.None);

        var outputLines = serverToClientWriter.ToString()
            .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        var diagnosticLogs = serverDiagnosticWriter.ToString();

        // 3. Assert: Verify Transport Purity
        // Stderr must have diagnostic logs
        Assert.Contains("[Sharphy:MCP] Sharphy MCP Server Stdio Host started", diagnosticLogs);
        Assert.Contains("[Sharphy:MCP] Client connection successfully initialized", diagnosticLogs);

        // Stdout must contain ONLY valid JSON-RPC frames (one JSON per line)
        Assert.Equal(7, outputLines.Length); // 7 requests with responses (notification returns null)

        foreach (var line in outputLines)
        {
            using var doc = JsonDocument.Parse(line);
            var root = doc.RootElement;
            Assert.True(root.TryGetProperty("jsonrpc", out var rpcVer));
            Assert.Equal("2.0", rpcVer.GetString());
            Assert.True(root.TryGetProperty("id", out _));
        }

        // Frame 1: Initialize response
        using (var doc1 = JsonDocument.Parse(outputLines[0]))
        {
            Assert.Equal(1, doc1.RootElement.GetProperty("id").GetInt64());
            Assert.True(doc1.RootElement.TryGetProperty("result", out var result));
            Assert.Equal("2024-11-05", result.GetProperty("protocolVersion").GetString());
            Assert.Equal("sharphy", result.GetProperty("serverInfo").GetProperty("name").GetString());
        }

        // Frame 2: Tools/list response
        using (var doc2 = JsonDocument.Parse(outputLines[1]))
        {
            Assert.Equal(2, doc2.RootElement.GetProperty("id").GetInt64());
            var tools = doc2.RootElement.GetProperty("result").GetProperty("tools");
            Assert.Equal(10, tools.GetArrayLength());
        }

        // Frame 3: Call inspect metadata
        using (var doc3 = JsonDocument.Parse(outputLines[2]))
        {
            Assert.Equal(3, doc3.RootElement.GetProperty("id").GetInt64());
            var content = doc3.RootElement.GetProperty("result").GetProperty("content");
            Assert.Contains("ECMA-335 Inspector", content[0].GetProperty("text").GetString());
        }

        // Frame 4: Call extract tokens (tool 8)
        using (var doc4 = JsonDocument.Parse(outputLines[3]))
        {
            Assert.Equal(4, doc4.RootElement.GetProperty("id").GetInt64());
            var content = doc4.RootElement.GetProperty("result").GetProperty("content");
            Assert.Contains("ALC Sandbox ready", content[0].GetProperty("text").GetString());
        }

        // Frame 5: Call generate wrapper (tool 10)
        using (var doc5 = JsonDocument.Parse(outputLines[4]))
        {
            Assert.Equal(5, doc5.RootElement.GetProperty("id").GetInt64());
            var content = doc5.RootElement.GetProperty("result").GetProperty("content");
            Assert.Contains("UnsafeAccessor Generator ready", content[0].GetProperty("text").GetString());
        }

        // Frame 6: Unknown tool error handling
        using (var doc6 = JsonDocument.Parse(outputLines[5]))
        {
            Assert.Equal(6, doc6.RootElement.GetProperty("id").GetInt64());
            var result = doc6.RootElement.GetProperty("result");
            Assert.True(result.GetProperty("isError").GetBoolean());
            Assert.Contains("Tool 'unknown_tool' was not found", result.GetProperty("content")[0].GetProperty("text").GetString());
        }

        // Frame 7: Ping response
        using (var doc7 = JsonDocument.Parse(outputLines[6]))
        {
            Assert.Equal(7, doc7.RootElement.GetProperty("id").GetInt64());
            Assert.True(doc7.RootElement.TryGetProperty("result", out _));
        }
    }
}
