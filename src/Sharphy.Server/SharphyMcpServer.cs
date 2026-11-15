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

        JsonDocument doc;
        try
        {
            doc = JsonDocument.Parse(messageJson);
        }
        catch (JsonException ex)
        {
            return JsonRpcResponse.CreateError(null, JsonRpcErrorCodes.ParseError, $"JSON Parse error: {ex.Message}");
        }

        using (doc)
        {
            var root = doc.RootElement;
            if (root.ValueKind != JsonValueKind.Object)
            {
                return JsonRpcResponse.CreateError(null, JsonRpcErrorCodes.InvalidRequest, "Invalid JSON-RPC request payload.");
            }

            object? requestId = null;
            if (root.TryGetProperty("id", out var idProp) && idProp.ValueKind != JsonValueKind.Null && idProp.ValueKind != JsonValueKind.Undefined)
            {
                if (idProp.ValueKind is not (JsonValueKind.String or JsonValueKind.Number))
                {
                    return JsonRpcResponse.CreateError(null, JsonRpcErrorCodes.InvalidRequest, "JSON-RPC 'id' must be a string, number, or null.");
                }

                requestId = idProp.Clone();
            }

            if (!root.TryGetProperty("jsonrpc", out var versionProp) ||
                versionProp.ValueKind != JsonValueKind.String ||
                !string.Equals(versionProp.GetString(), "2.0", StringComparison.Ordinal))
            {
                var provided = versionProp.ValueKind == JsonValueKind.Undefined ? null : versionProp.ToString();
                return JsonRpcResponse.CreateError(requestId, JsonRpcErrorCodes.InvalidRequest, $"Unsupported JSON-RPC version '{provided}'. Expected '2.0'.");
            }

            if (!root.TryGetProperty("method", out var methodProp) ||
                methodProp.ValueKind != JsonValueKind.String)
            {
                return JsonRpcResponse.CreateError(requestId, JsonRpcErrorCodes.InvalidRequest, "Invalid request payload: 'method' must be a string.");
            }

            var method = methodProp.GetString();
            if (string.IsNullOrWhiteSpace(method))
            {
                return JsonRpcResponse.CreateError(requestId, JsonRpcErrorCodes.InvalidRequest, "Invalid request payload.");
            }

            // JSON-RPC 2.0 notifications (requests without an Id member) must not emit a response.
            if (requestId == null)
            {
                if (method == "notifications/initialized")
                {
                    LogDiagnostic("Client connection successfully initialized.");
                }
                else if (method.StartsWith("notifications/", StringComparison.Ordinal))
                {
                    LogDiagnostic($"Received client notification '{method}'.");
                }

                return null;
            }

            JsonElement? paramsElement = root.TryGetProperty("params", out var pElem) ? pElem.Clone() : null;

            return method switch
            {
                "initialize" => HandleInitialize(requestId),
                "tools/list" => HandleToolsList(requestId),
                "tools/call" => await HandleToolsCallAsync(requestId, paramsElement).ConfigureAwait(false),
                "ping" => JsonRpcResponse.Success(requestId, new { }),
                _ => JsonRpcResponse.CreateError(requestId, JsonRpcErrorCodes.MethodNotFound, $"Method '{method}' is not supported.")
            };
        }
    }

    private JsonRpcResponse HandleInitialize(object requestId)
    {
        var result = new McpInitializeResult();
        return JsonRpcResponse.Success(requestId, result);
    }

    private JsonRpcResponse HandleToolsList(object requestId)
    {
        var result = new McpToolListResult
        {
            Tools = _toolRegistry.GetToolDefinitions().ToList()
        };
        return JsonRpcResponse.Success(requestId, result);
    }

    private async Task<JsonRpcResponse> HandleToolsCallAsync(object requestId, JsonElement? rawParams)
    {
        if (rawParams is not JsonElement paramsElement || paramsElement.ValueKind != JsonValueKind.Object)
        {
            return JsonRpcResponse.CreateError(requestId, JsonRpcErrorCodes.InvalidParams, "Parameter 'params' for tools/call must be an object.");
        }

        if (!paramsElement.TryGetProperty("name", out var nameProp) || nameProp.ValueKind != JsonValueKind.String || nameProp.GetString() is not string toolName)
        {
            return JsonRpcResponse.CreateError(requestId, JsonRpcErrorCodes.InvalidParams, "Parameter 'name' must be a string for tools/call.");
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
                return JsonRpcResponse.CreateError(requestId, JsonRpcErrorCodes.InvalidParams, "Parameter 'arguments' must be an object.");
            }
        }

        var callResult = await _toolRegistry.DispatchAsync(toolName, arguments).ConfigureAwait(false);
        return JsonRpcResponse.Success(requestId, callResult);
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
