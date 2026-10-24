using System.Text.Json;
using Sharphy.Server.Protocol;
using Sharphy.Server.Tools;

namespace Sharphy.Server;

/// <summary>
/// Model Context Protocol (MCP) server host for Sharphy operating over pure Stdio transport.
/// </summary>
public sealed class SharphyMcpServer
{
    private readonly ToolRegistry _toolRegistry;
    private readonly TextReader _input;
    private readonly TextWriter _output;
    private readonly TextWriter _error;
    private readonly JsonSerializerOptions _jsonOptions;
    private bool _isRunning;

    public SharphyMcpServer(
        ToolRegistry? toolRegistry = null,
        TextReader? input = null,
        TextWriter? output = null,
        TextWriter? error = null)
    {
        _toolRegistry = toolRegistry ?? new ToolRegistry();
        _input = input ?? Console.In;
        _output = output ?? Console.Out;
        _error = error ?? Console.Error;

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };
    }

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        _isRunning = true;
        LogDiagnostic("Sharphy MCP Server Stdio Host started and ready for requests.");

        while (_isRunning && !cancellationToken.IsCancellationRequested)
        {
            string? line;
            try
            {
                line = await _input.ReadLineAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                break;
            }

            if (line == null)
            {
                // Stdio stream closed by parent host / client
                break;
            }

            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            try
            {
                var response = await ProcessMessageAsync(line).ConfigureAwait(false);
                if (response != null)
                {
                    var jsonResponse = JsonSerializer.Serialize(response, _jsonOptions);
                    await _output.WriteLineAsync(jsonResponse).ConfigureAwait(false);
                    await _output.FlushAsync().ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                LogDiagnostic($"Error processing JSON-RPC message: {ex.Message}");
                var errorResponse = JsonRpcResponse.CreateError(null, JsonRpcErrorCodes.InternalError, ex.Message);
                var jsonResponse = JsonSerializer.Serialize(errorResponse, _jsonOptions);
                await _output.WriteLineAsync(jsonResponse).ConfigureAwait(false);
                await _output.FlushAsync().ConfigureAwait(false);
            }
        }

        LogDiagnostic("Sharphy MCP Server Stdio Host stopped.");
    }

    public async Task<JsonRpcResponse?> ProcessMessageAsync(string messageJson)
    {
        JsonRpcRequest? request;
        try
        {
            request = JsonSerializer.Deserialize<JsonRpcRequest>(messageJson, _jsonOptions);
        }
        catch (JsonException ex)
        {
            return JsonRpcResponse.CreateError(null, JsonRpcErrorCodes.ParseError, $"JSON Parse error: {ex.Message}");
        }

        if (request == null || string.IsNullOrWhiteSpace(request.Method))
        {
            return JsonRpcResponse.CreateError(request?.Id, JsonRpcErrorCodes.InvalidRequest, "Invalid request payload.");
        }

        // Handle initialized notification (request without Id)
        if (request.Method == "notifications/initialized")
        {
            LogDiagnostic("Client connection successfully initialized.");
            return null;
        }

        return request.Method switch
        {
            "initialize" => HandleInitialize(request),
            "tools/list" => HandleToolsList(request),
            "tools/call" => await HandleToolsCallAsync(request).ConfigureAwait(false),
            "ping" => JsonRpcResponse.Success(request.Id, new { }),
            _ => JsonRpcResponse.CreateError(request.Id, JsonRpcErrorCodes.MethodNotFound, $"Method '{request.Method}' is not supported.")
        };
    }

    private JsonRpcResponse HandleInitialize(JsonRpcRequest request)
    {
        var result = new McpInitializeResult();
        return JsonRpcResponse.Success(request.Id, result);
    }

    private JsonRpcResponse HandleToolsList(JsonRpcRequest request)
    {
        var result = new McpToolListResult
        {
            Tools = _toolRegistry.GetToolDefinitions().ToList()
        };
        return JsonRpcResponse.Success(request.Id, result);
    }

    private async Task<JsonRpcResponse> HandleToolsCallAsync(JsonRpcRequest request)
    {
        if (request.Params is not JsonElement paramsElement)
        {
            return JsonRpcResponse.CreateError(request.Id, JsonRpcErrorCodes.InvalidParams, "Parameter 'params' for tools/call must be an object.");
        }

        if (!paramsElement.TryGetProperty("name", out var nameProp) || nameProp.GetString() is not string toolName)
        {
            return JsonRpcResponse.CreateError(request.Id, JsonRpcErrorCodes.InvalidParams, "Parameter 'name' is required for tools/call.");
        }

        JsonElement? arguments = null;
        if (paramsElement.TryGetProperty("arguments", out var argsProp) && argsProp.ValueKind == JsonValueKind.Object)
        {
            arguments = argsProp;
        }

        var callResult = await _toolRegistry.DispatchAsync(toolName, arguments).ConfigureAwait(false);
        return JsonRpcResponse.Success(request.Id, callResult);
    }

    private void LogDiagnostic(string message)
    {
        // MCP specification standard: write internal diagnostics to stderr to prevent polluting stdout
        _error.WriteLine($"[Sharphy:MCP] {message}");
    }
}
