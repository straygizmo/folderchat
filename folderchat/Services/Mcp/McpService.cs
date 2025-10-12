using folderchat.Models;
using System.Collections.Concurrent;

namespace folderchat.Services.Mcp
{
    public class McpService : IMcpService
    {
        private readonly ConcurrentDictionary<string, IMcpClient> _activeServers = new();
        private readonly ConcurrentDictionary<string, McpServerConfig> _serverConfigs = new();
        private readonly ConcurrentDictionary<string, List<Tool>> _serverTools = new();
        private readonly ConcurrentDictionary<string, string> _toolToServerMap = new();

        public event EventHandler<McpServerEventArgs>? ServerStatusChanged;
        public event EventHandler<string>? LogMessage;

        public async Task<bool> LoadServerAsync(McpServerConfig config)
        {
            if (string.IsNullOrEmpty(config.Id))
            {
                LogMessage?.Invoke(this, "Server configuration must have an ID");
                return false;
            }

            if (!config.IsEnabled)
            {
                LogMessage?.Invoke(this, $"Server {config.Name} is disabled");
                return false;
            }

            try
            {
                // Unload existing server if already loaded
                if (_activeServers.ContainsKey(config.Id))
                {
                    await UnloadServerAsync(config.Id);
                }

                ServerStatusChanged?.Invoke(this, new McpServerEventArgs
                {
                    ServerId = config.Id,
                    ServerName = config.Name,
                    Status = McpServerStatus.Connecting
                });

                IMcpClient client;
                if (config.TransportType == McpTransportType.Stdio)
                {
                    // Add working directory to environment variables if specified
                    var envVars = config.EnvironmentVariables != null
                        ? new Dictionary<string, string>(config.EnvironmentVariables)
                        : new Dictionary<string, string>();

                    if (!string.IsNullOrWhiteSpace(config.WorkingDirectory))
                    {
                        envVars["WORKING_DIR"] = config.WorkingDirectory;
                    }

                    client = new SdkMcpClientAdapter(
                        config.Command,
                        config.Arguments,
                        envVars.Count > 0 ? envVars : null
                    );
                }
                else
                {
                    LogMessage?.Invoke(this, "HTTP transport not yet implemented");
                    ServerStatusChanged?.Invoke(this, new McpServerEventArgs
                    {
                        ServerId = config.Id,
                        ServerName = config.Name,
                        Status = McpServerStatus.Error,
                        Message = "HTTP transport not yet implemented"
                    });
                    return false;
                }

                // Subscribe to client log messages
                client.LogMessage += (sender, message) =>
                {
                    LogMessage?.Invoke(this, $"[{config.Name}] {message}");
                };

                // Connect to the server
                var connected = await client.ConnectAsync();
                if (!connected)
                {
                    ServerStatusChanged?.Invoke(this, new McpServerEventArgs
                    {
                        ServerId = config.Id,
                        ServerName = config.Name,
                        Status = McpServerStatus.Error,
                        Message = "Failed to connect to server"
                    });
                    client.Dispose();
                    return false;
                }

                // Initialize the connection
                var initRequest = new InitializeRequest
                {
                    ProtocolVersion = "2024-11-05",
                    ClientInfo = new ClientInfo
                    {
                        Name = "folderchat",
                        Version = "1.0.0"
                    },
                    Capabilities = new ClientCapabilities()
                    {
                        Experimental = null,
                        Roots = null,
                        Sampling = null
                    }
                };

                try
                {
                    var initResponse = await client.InitializeAsync(initRequest);
                    LogMessage?.Invoke(this, $"Server {config.Name} initialized: {initResponse.ServerInfo.Name} v{initResponse.ServerInfo.Version}");
                }
                catch (Exception ex)
                {
                    LogMessage?.Invoke(this, $"Failed to initialize server {config.Name}: {ex.Message}");
                    ServerStatusChanged?.Invoke(this, new McpServerEventArgs
                    {
                        ServerId = config.Id,
                        ServerName = config.Name,
                        Status = McpServerStatus.Error,
                        Message = $"Failed to initialize: {ex.Message}"
                    });
                    await client.DisconnectAsync();
                    client.Dispose();
                    return false;
                }

                // Get available tools
                try
                {
                    var tools = await client.ListToolsAsync();
                    _serverTools[config.Id] = tools;

                    // Map tools to server
                    foreach (var tool in tools)
                    {
                        _toolToServerMap[$"{config.Id}:{tool.Name}"] = config.Id;
                    }

                    LogMessage?.Invoke(this, $"Server {config.Name} provides {tools.Count} tools");
                }
                catch (Exception ex)
                {
                    LogMessage?.Invoke(this, $"Failed to list tools from {config.Name}: {ex.Message}");
                }

                // Store the active client and config
                _activeServers[config.Id] = client;
                _serverConfigs[config.Id] = config;

                ServerStatusChanged?.Invoke(this, new McpServerEventArgs
                {
                    ServerId = config.Id,
                    ServerName = config.Name,
                    Status = McpServerStatus.Connected
                });

                return true;
            }
            catch (Exception ex)
            {
                LogMessage?.Invoke(this, $"Error loading server {config.Name}: {ex.Message}");
                ServerStatusChanged?.Invoke(this, new McpServerEventArgs
                {
                    ServerId = config.Id,
                    ServerName = config.Name,
                    Status = McpServerStatus.Error,
                    Message = ex.Message
                });
                return false;
            }
        }

        public async Task UnloadServerAsync(string serverId)
        {
            if (_activeServers.TryRemove(serverId, out var client))
            {
                _serverConfigs.TryRemove(serverId, out var config);
                _serverTools.TryRemove(serverId, out _);

                // Remove tool mappings
                var keysToRemove = _toolToServerMap.Keys
                    .Where(k => k.StartsWith($"{serverId}:"))
                    .ToList();
                foreach (var key in keysToRemove)
                {
                    _toolToServerMap.TryRemove(key, out _);
                }

                await client.DisconnectAsync();
                client.Dispose();

                ServerStatusChanged?.Invoke(this, new McpServerEventArgs
                {
                    ServerId = serverId,
                    ServerName = config?.Name ?? serverId,
                    Status = McpServerStatus.Disconnected
                });

                LogMessage?.Invoke(this, $"Server {config?.Name ?? serverId} unloaded");
            }
        }

        public async Task UnloadAllServersAsync()
        {
            var serverIds = _activeServers.Keys.ToList();
            foreach (var serverId in serverIds)
            {
                await UnloadServerAsync(serverId);
            }
        }

        public async Task<List<Tool>> GetAllToolsAsync()
        {
            var allTools = new List<Tool>();
            foreach (var tools in _serverTools.Values)
            {
                allTools.AddRange(tools);
            }
            return allTools;
        }

        public async Task<List<Tool>> GetServerToolsAsync(string serverId)
        {
            if (_serverTools.TryGetValue(serverId, out var tools))
            {
                return tools;
            }
            return new List<Tool>();
        }

        public async Task<CallToolResponse> CallToolAsync(string toolName, object? arguments)
        {
            // Find which server provides this tool
            string? serverId = null;
            foreach (var kvp in _toolToServerMap)
            {
                if (kvp.Key.EndsWith($":{toolName}"))
                {
                    serverId = kvp.Value;
                    break;
                }
            }

            if (serverId == null)
            {
                throw new Exception($"Tool {toolName} not found in any loaded server");
            }

            if (!_activeServers.TryGetValue(serverId, out var client))
            {
                throw new Exception($"Server {serverId} is not loaded");
            }

            try
            {
                return await client.CallToolAsync(toolName, arguments);
            }
            catch (Exception ex)
            {
                LogMessage?.Invoke(this, $"Error calling tool {toolName}: {ex.Message}");
                throw;
            }
        }

        public Dictionary<string, IMcpClient> GetActiveServers()
        {
            return new Dictionary<string, IMcpClient>(_activeServers);
        }

        public bool IsServerLoaded(string serverId)
        {
            return _activeServers.ContainsKey(serverId);
        }

        public void Dispose()
        {
            UnloadAllServersAsync().GetAwaiter().GetResult();
        }
    }
}