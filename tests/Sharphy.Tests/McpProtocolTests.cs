using System.Text.Json;
using Sharphy.Server;
using Sharphy.Server.Protocol;
using Sharphy.Server.Tools;
using Xunit;

namespace Sharphy.Tests;

public class McpProtocolTests
{
    [Fact]
    public async Task ProcessMessage_Initialize_ReturnsValidProtocolResult()
    {
        var server = new SharphyMcpServer();
        var request = "{\"jsonrpc\":\"2.0\",\"id\":1,\"method\":\"initialize\",\"params\":{}}";

        var response = await server.ProcessMessageAsync(request);

        Assert.NotNull(response);
        Assert.Equal("2.0", response.JsonRpc);
        Assert.Null(response.Error);
        Assert.NotNull(response.Result);

        var initResult = Assert.IsType<McpInitializeResult>(response.Result);
        Assert.Equal("2024-11-05", initResult.ProtocolVersion);
        Assert.Equal("sharphy", initResult.ServerInfo.Name);
    }

    [Fact]
    public async Task ProcessMessage_ToolsList_ContainsAllTenRegisteredTools()
    {
        var server = new SharphyMcpServer();
        var request = "{\"jsonrpc\":\"2.0\",\"id\":2,\"method\":\"tools/list\",\"params\":{}}";

        var response = await server.ProcessMessageAsync(request);

        Assert.NotNull(response);
        Assert.Null(response.Error);

        var toolListResult = Assert.IsType<McpToolListResult>(response.Result);
        Assert.Equal(10, toolListResult.Tools.Count);

        var expectedTools = new[]
        {
            "sharphy_analyze_dump",
            "sharphy_inspect_heap",
            "sharphy_deadlock_detector",
            "sharphy_decompile_method",
            "sharphy_diff_assemblies",
            "sharphy_inspect_metadata",
            "sharphy_clean_obfuscation",
            "sharphy_extract_tokens",
            "sharphy_patch_il",
            "sharphy_generate_wrapper"
        };

        foreach (var expected in expectedTools)
        {
            Assert.Contains(toolListResult.Tools, t => t.Name == expected);
        }
    }

    [Fact]
    public async Task ProcessMessage_ToolsCall_DispatchesSuccessfully()
    {
        var server = new SharphyMcpServer();
        var request = """
        {
            "jsonrpc": "2.0",
            "id": 3,
            "method": "tools/call",
            "params": {
                "name": "sharphy_inspect_metadata",
                "arguments": {
                    "assemblyPath": "C:\\fake\\test.dll"
                }
            }
        }
        """;

        var response = await server.ProcessMessageAsync(request);

        Assert.NotNull(response);
        Assert.Null(response.Error);

        var callResult = Assert.IsType<McpToolCallResult>(response.Result);
        Assert.False(callResult.IsError);
        Assert.NotEmpty(callResult.Content);
        Assert.Contains("ECMA-335 Inspector", callResult.Content[0].Text);
    }

    [Fact]
    public async Task ProcessMessage_InvalidMethod_ReturnsMethodNotFound()
    {
        var server = new SharphyMcpServer();
        var request = "{\"jsonrpc\":\"2.0\",\"id\":99,\"method\":\"non_existent_method\",\"params\":{}}";

        var response = await server.ProcessMessageAsync(request);

        Assert.NotNull(response);
        Assert.NotNull(response.Error);
        Assert.Equal(JsonRpcErrorCodes.MethodNotFound, response.Error.Code);
    }
}

