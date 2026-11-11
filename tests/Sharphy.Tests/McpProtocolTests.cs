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

    [Fact]
    public async Task ProcessMessage_ArbitraryNotificationWithoutId_ReturnsNull()
    {
        var server = new SharphyMcpServer();
        var request = "{\"jsonrpc\":\"2.0\",\"method\":\"notifications/cancelled\",\"params\":{\"requestId\":42}}";

        var response = await server.ProcessMessageAsync(request);

        Assert.Null(response);
    }

    [Fact]
    public async Task ProcessMessage_ToolsCall_ValidatesHeapTopObjectsAndPatchOffset()
    {
        var server = new SharphyMcpServer();

        var invalidTopReq = """
        {
            "jsonrpc": "2.0",
            "id": 10,
            "method": "tools/call",
            "params": {
                "name": "sharphy_inspect_heap",
                "arguments": {
                    "dumpPath": "missing.dmp",
                    "topObjects": 0
                }
            }
        }
        """;

        var invalidTopResp = await server.ProcessMessageAsync(invalidTopReq);
        Assert.NotNull(invalidTopResp);
        var invalidTopResult = Assert.IsType<McpToolCallResult>(invalidTopResp.Result);
        Assert.True(invalidTopResult.IsError);
        Assert.Contains("positive integer", invalidTopResult.Content[0].Text);

        var invalidOffsetReq = """
        {
            "jsonrpc": "2.0",
            "id": 11,
            "method": "tools/call",
            "params": {
                "name": "sharphy_patch_il",
                "arguments": {
                    "assemblyPath": "missing.dll",
                    "typeName": "Demo.Program",
                    "methodName": "Main",
                    "instructionOffset": -5,
                    "newOpCode": "Nop",
                    "outputPath": "out.dll"
                }
            }
        }
        """;

        var invalidOffsetResp = await server.ProcessMessageAsync(invalidOffsetReq);
        Assert.NotNull(invalidOffsetResp);
        var invalidOffsetResult = Assert.IsType<McpToolCallResult>(invalidOffsetResp.Result);
        Assert.True(invalidOffsetResult.IsError);
        Assert.Contains("non-negative integer", invalidOffsetResult.Content[0].Text);
    }

    [Fact]
    public async Task ProcessMessage_RejectsNon20JsonRpcVersionAndNonObjectArguments()
    {
        var server = new SharphyMcpServer();

        var legacyReq = """
        {
            "jsonrpc": "1.0",
            "id": 12,
            "method": "tools/list"
        }
        """;

        var legacyResp = await server.ProcessMessageAsync(legacyReq);
        Assert.NotNull(legacyResp);
        Assert.NotNull(legacyResp.Error);
        Assert.Equal(JsonRpcErrorCodes.InvalidRequest, legacyResp.Error.Code);

        var missingVersionReq = """
        {
            "id": 14,
            "method": "tools/list"
        }
        """;

        var missingVersionResp = await server.ProcessMessageAsync(missingVersionReq);
        Assert.NotNull(missingVersionResp);
        Assert.NotNull(missingVersionResp.Error);
        Assert.Equal(JsonRpcErrorCodes.InvalidRequest, missingVersionResp.Error.Code);

        var badArgsReq = """
        {
            "jsonrpc": "2.0",
            "id": 13,
            "method": "tools/call",
            "params": {
                "name": "sharphy_inspect_metadata",
                "arguments": [1, 2, 3]
            }
        }
        """;

        var badArgsResp = await server.ProcessMessageAsync(badArgsReq);
        Assert.NotNull(badArgsResp);
        Assert.NotNull(badArgsResp.Error);
        Assert.Equal(JsonRpcErrorCodes.InvalidParams, badArgsResp.Error.Code);
    }

    [Fact]
    public async Task RunAsync_RejectsOversizedFramesAndRecoversForNextRequest()
    {
        var hugeFrame = new string('x', SharphyMcpServer.MaxMessageSizeChars + 32);
        var pingFrame = """{"jsonrpc":"2.0","id":77,"method":"ping","params":{}}""";
        using var input = new StringReader(hugeFrame + "\n" + pingFrame + "\n");
        using var output = new StringWriter();
        using var err = new StringWriter();

        var server = new SharphyMcpServer(input: input, output: output, error: err);
        await server.RunAsync(TestContext.Current.CancellationToken);

        var lines = output.ToString()
            .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        Assert.Equal(2, lines.Length);
        Assert.Contains("maximum frame size", lines[0]);
        Assert.Contains("\"id\":77", lines[1]);
        Assert.DoesNotContain("\"error\"", lines[1]);
    }
}

