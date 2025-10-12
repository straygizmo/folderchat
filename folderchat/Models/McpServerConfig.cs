using System.Text.Json.Serialization;

namespace folderchat.Models
{
    public class McpServerConfig
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("command")]
        public string Command { get; set; } = string.Empty;

        [JsonPropertyName("arguments")]
        public string? Arguments { get; set; }

        [JsonPropertyName("transportType")]
        public McpTransportType TransportType { get; set; } = McpTransportType.Stdio;

        [JsonPropertyName("httpUrl")]
        public string? HttpUrl { get; set; }

        [JsonPropertyName("isEnabled")]
        public bool IsEnabled { get; set; } = true;

        [JsonPropertyName("environmentVariables")]
        public Dictionary<string, string>? EnvironmentVariables { get; set; }

        [JsonPropertyName("workingDirectory")]
        public string? WorkingDirectory { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }
    }

    public enum McpTransportType
    {
        Stdio,
        Http
    }

    public class McpServerCollection
    {
        [JsonPropertyName("servers")]
        public List<McpServerConfig> Servers { get; set; } = new();
    }
}