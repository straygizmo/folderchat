using folderchat.Models;

namespace folderchat.Services.Mcp
{
    public interface IMcpService : IDisposable
    {
        Task<bool> LoadServerAsync(McpServerConfig config);
        Task UnloadServerAsync(string serverId);
        Task UnloadAllServersAsync();
        Task<List<Tool>> GetAllToolsAsync();
        Task<List<Tool>> GetServerToolsAsync(string serverId);
        Task<CallToolResponse> CallToolAsync(string toolName, object? arguments);
        Dictionary<string, IMcpClient> GetActiveServers();
        bool IsServerLoaded(string serverId);
        event EventHandler<McpServerEventArgs>? ServerStatusChanged;
        event EventHandler<string>? LogMessage;
    }

    public class McpServerEventArgs : EventArgs
    {
        public string ServerId { get; set; } = string.Empty;
        public string ServerName { get; set; } = string.Empty;
        public McpServerStatus Status { get; set; }
        public string? Message { get; set; }
    }

    public enum McpServerStatus
    {
        Disconnected,
        Connecting,
        Connected,
        Error
    }
}