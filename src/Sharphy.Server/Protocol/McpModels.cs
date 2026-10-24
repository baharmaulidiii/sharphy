using System.Text.Json.Serialization;

namespace Sharphy.Server.Protocol;

public sealed class McpToolDefinition
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("inputSchema")]
    public object InputSchema { get; set; } = new();
}

public sealed class McpToolListResult
{
    [JsonPropertyName("tools")]
    public List<McpToolDefinition> Tools { get; set; } = new();
}

public sealed class McpToolContent
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = "text";

    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;

    public static McpToolContent FromText(string text) => new() { Type = "text", Text = text };
}

public sealed class McpToolCallResult
{
    [JsonPropertyName("content")]
    public List<McpToolContent> Content { get; set; } = new();

    [JsonPropertyName("isError")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool IsError { get; set; }

    public static McpToolCallResult CreateSuccess(string text) => new()
    {
        Content = new List<McpToolContent> { McpToolContent.FromText(text) },
        IsError = false
    };

    public static McpToolCallResult CreateError(string errorMessage) => new()
    {
        Content = new List<McpToolContent> { McpToolContent.FromText(errorMessage) },
        IsError = true
    };
}

public sealed class McpInitializeResult
{
    [JsonPropertyName("protocolVersion")]
    public string ProtocolVersion { get; set; } = "2024-11-05";

    [JsonPropertyName("capabilities")]
    public McpServerCapabilities Capabilities { get; set; } = new();

    [JsonPropertyName("serverInfo")]
    public McpImplementationInfo ServerInfo { get; set; } = new();
}

public sealed class McpServerCapabilities
{
    [JsonPropertyName("tools")]
    public object Tools { get; set; } = new();
}

public sealed class McpImplementationInfo
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "sharphy";

    [JsonPropertyName("version")]
    public string Version { get; set; } = "0.1.0-preview";
}

