using System.Text.Json.Serialization;

namespace folderchat.Services.Mcp
{
    public static class McpMethods
    {
        public const string Initialize = "initialize";
        public const string ListTools = "tools/list";
        public const string CallTool = "tools/call";
        public const string ListResources = "resources/list";
        public const string ReadResource = "resources/read";
        public const string ListPrompts = "prompts/list";
        public const string GetPrompt = "prompts/get";
        public const string SetLoggingLevel = "logging/setLevel";
    }

    public class InitializeRequest
    {
        [JsonPropertyName("protocolVersion")]
        public string ProtocolVersion { get; set; } = "2024-11-05";

        [JsonPropertyName("capabilities")]
        public ClientCapabilities Capabilities { get; set; } = new();

        [JsonPropertyName("clientInfo")]
        public ClientInfo ClientInfo { get; set; } = new();
    }

    public class ClientCapabilities
    {
        [JsonPropertyName("roots")]
        public RootsCapability? Roots { get; set; }

        [JsonPropertyName("sampling")]
        public object? Sampling { get; set; }

        [JsonPropertyName("experimental")]
        public object? Experimental { get; set; }
    }

    public class RootsCapability
    {
        [JsonPropertyName("listChanged")]
        public bool ListChanged { get; set; }
    }

    public class ClientInfo
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = "folderchat";

        [JsonPropertyName("version")]
        public string Version { get; set; } = "1.0.0";
    }

    public class InitializeResponse
    {
        [JsonPropertyName("protocolVersion")]
        public string ProtocolVersion { get; set; } = string.Empty;

        [JsonPropertyName("capabilities")]
        public ServerCapabilities Capabilities { get; set; } = new();

        [JsonPropertyName("serverInfo")]
        public ServerInfo ServerInfo { get; set; } = new();
    }

    public class ServerCapabilities
    {
        [JsonPropertyName("tools")]
        public object? Tools { get; set; }

        [JsonPropertyName("resources")]
        public object? Resources { get; set; }

        [JsonPropertyName("prompts")]
        public object? Prompts { get; set; }

        [JsonPropertyName("logging")]
        public object? Logging { get; set; }
    }

    public class ServerInfo
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("version")]
        public string Version { get; set; } = string.Empty;
    }

    public class ListToolsResponse
    {
        [JsonPropertyName("tools")]
        public List<Tool> Tools { get; set; } = new();
    }

    public class Tool
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("inputSchema")]
        public object? InputSchema { get; set; }
    }

    public class CallToolRequest
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("arguments")]
        public object? Arguments { get; set; }
    }

    public class CallToolResponse
    {
        [JsonPropertyName("content")]
        public List<ToolContent> Content { get; set; } = new();

        [JsonPropertyName("isError")]
        public bool IsError { get; set; }
    }

    public class ToolContent
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = "text";

        [JsonPropertyName("text")]
        public string? Text { get; set; }

        [JsonPropertyName("data")]
        public string? Data { get; set; }

        [JsonPropertyName("mimeType")]
        public string? MimeType { get; set; }
    }
}