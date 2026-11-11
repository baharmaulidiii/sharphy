using System.Text.Json;
using Sharphy.Server.Protocol;
using Sharphy.Server.Tools;

namespace Sharphy.Server;

/// <summary>
/// Model Context Protocol (MCP) server host for Sharphy operating over pure Stdio transport.
/// </summary>
public sealed class SharphyMcpServer
{
    public const int MaxMessageSizeChars = 4 * 1024 * 1024;

    private readonly ToolRegistry _toolRegistry;
    private readonly TextReader _input;
    private readonly TextWriter _output;
    private readonly TextWriter _error;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly char[] _readBuffer = new char[4096];
    private int _bufPos;
    private int _bufLen;
    private bool _skipNextLf;
    private volatile bool _isRunning;

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
            bool exceededLimit;
            try
            {
                (line, exceededLimit) = await ReadLineBoundedAsync(MaxMessageSizeChars, cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                break;
            }

            if (line == null && !exceededLimit)
            {
                // Stdio stream closed by parent host / client
                break;
            }

            if (exceededLimit)
            {
                LogDiagnostic($"Rejected oversized JSON-RPC frame exceeding {MaxMessageSizeChars} characters.");
                var sizeError = JsonRpcResponse.CreateError(null, JsonRpcErrorCodes.InvalidRequest, $"Request exceeds maximum frame size of {MaxMessageSizeChars} characters.");
                await _output.WriteLineAsync(JsonSerializer.Serialize(sizeError, _jsonOptions)).ConfigureAwait(false);
                await _output.FlushAsync().ConfigureAwait(false);
                continue;
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
        if (messageJson.Length > MaxMessageSizeChars)
        {
            return JsonRpcResponse.CreateError(null, JsonRpcErrorCodes.InvalidRequest, $"Request exceeds maximum frame size of {MaxMessageSizeChars} characters.");
        }

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

        if (!string.Equals(request.JsonRpc, "2.0", StringComparison.Ordinal))
        {
            return JsonRpcResponse.CreateError(request.Id, JsonRpcErrorCodes.InvalidRequest, $"Unsupported JSON-RPC version '{request.JsonRpc}'. Expected '2.0'.");
        }

        // JSON-RPC 2.0 notifications (requests without an Id member) must not emit a response.
        if (request.Id == null)
        {
            if (request.Method == "notifications/initialized")
            {
                LogDiagnostic("Client connection successfully initialized.");
            }
            else if (request.Method.StartsWith("notifications/", StringComparison.Ordinal))
            {
                LogDiagnostic($"Received client notification '{request.Method}'.");
            }

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
        if (paramsElement.TryGetProperty("arguments", out var argsProp))
        {
            if (argsProp.ValueKind == JsonValueKind.Object)
            {
                arguments = argsProp;
            }
            else if (argsProp.ValueKind != JsonValueKind.Null && argsProp.ValueKind != JsonValueKind.Undefined)
            {
                return JsonRpcResponse.CreateError(request.Id, JsonRpcErrorCodes.InvalidParams, "Parameter 'arguments' must be an object.");
            }
        }

        var callResult = await _toolRegistry.DispatchAsync(toolName, arguments).ConfigureAwait(false);
        return JsonRpcResponse.Success(request.Id, callResult);
    }

    private async Task<(string? Line, bool ExceededLimit)> ReadLineBoundedAsync(int maxChars, CancellationToken cancellationToken)
    {
        var sb = new System.Text.StringBuilder();
        bool exceeded = false;

        while (true)
        {
            if (_bufPos >= _bufLen)
            {
                _bufLen = await _input.ReadAsync(_readBuffer.AsMemory(), cancellationToken).ConfigureAwait(false);
                _bufPos = 0;

                if (_bufLen == 0)
                {
                    if (exceeded) return (null, true);
                    return sb.Length > 0 ? (sb.ToString(), false) : (null, false);
                }
            }

            if (_skipNextLf)
            {
                _skipNextLf = false;
                if (_readBuffer[_bufPos] == '\n')
                {
                    _bufPos++;
                    continue;
                }
            }

            char ch = _readBuffer[_bufPos++];
            if (ch == '\r')
            {
                _skipNextLf = true;
                return exceeded ? (null, true) : (sb.ToString(), false);
            }

            if (ch == '\n')
            {
                return exceeded ? (null, true) : (sb.ToString(), false);
            }

            if (!exceeded)
            {
                if (sb.Length >= maxChars)
                {
                    exceeded = true;
                    sb.Clear();
                }
                else
                {
                    sb.Append(ch);
                }
            }
        }
    }

    private void LogDiagnostic(string message)
    {
        // MCP specification standard: write internal diagnostics to stderr to prevent polluting stdout
        _error.WriteLine($"[Sharphy:MCP] {message}");
    }
}
