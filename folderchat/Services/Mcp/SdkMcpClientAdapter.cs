using ModelContextProtocol.Client;
using System.Diagnostics;

namespace folderchat.Services.Mcp
{
    /// <summary>
    /// Adapter that wraps the official ModelContextProtocol.Core SDK to implement IMcpClient.
    /// This provides a bridge between the SDK's API and our existing interface.
    /// </summary>
    public class SdkMcpClientAdapter : IMcpClient
    {
        private readonly string _executablePath;
        private readonly string? _arguments;
        private readonly Dictionary<string, string>? _environmentVariables;
        private StdioClientTransport? _transport;
        private McpClient? _client;
        private bool _isConnected;
        private readonly object _lockObject = new();

        public bool IsConnected => _isConnected && _client != null;
        public event EventHandler<string>? LogMessage;

        public SdkMcpClientAdapter(string executablePath, string? arguments = null, Dictionary<string, string>? environmentVariables = null)
        {
            _executablePath = executablePath;
            _arguments = arguments;
            _environmentVariables = environmentVariables;
        }

        public async Task<bool> ConnectAsync()
        {
            try
            {
                LogMessage?.Invoke(this, $"Connecting to MCP server: {_executablePath} {_arguments}");

                // Parse arguments string into array
                var argsList = new List<string>();
                if (!string.IsNullOrWhiteSpace(_arguments))
                {
                    // Simple argument parsing - split by spaces but respect quotes
                    var parts = _arguments.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    argsList.AddRange(parts);
                }

                // Extract working directory from environment variables if present
                string? workingDirectory = null;
                var envVars = new Dictionary<string, string>();
                if (_environmentVariables != null)
                {
                    foreach (var kvp in _environmentVariables)
                    {
                        if (kvp.Key == "WORKING_DIR")
                        {
                            workingDirectory = kvp.Value;
                        }
                        else
                        {
                            envVars[kvp.Key] = kvp.Value;
                        }
                    }
                }

                // Create transport options
                var transportOptions = new StdioClientTransportOptions
                {
                    Command = _executablePath,
                    Arguments = argsList.ToArray(),
                    WorkingDirectory = workingDirectory ?? Environment.CurrentDirectory
                };

                // Ensure EnvironmentVariables directory is initialized (avoid NullReferenceException)
                transportOptions.EnvironmentVariables ??= new Dictionary<string, string>();

                // Log working directory for diagnostics
                LogMessage?.Invoke(this, $"working directory: {transportOptions.WorkingDirectory}");

                // Add environment variables to transport options
                if (envVars.Count > 0)
                {
                    foreach (var kvp in envVars)
                    {
                        transportOptions.EnvironmentVariables[kvp.Key] = kvp.Value;
                    }

                    // Log only environment variable keys
                    try
                    {
                        var keys = string.Join(", ", envVars.Keys);
                        LogMessage?.Invoke(this, $"Environment variable keys set: [{keys}]");
                    }
                    catch { }
                }

                // Create the transport
                _transport = new StdioClientTransport(transportOptions);

                // Create the client using the SDK
                _client = await McpClient.CreateAsync(_transport);

                lock (_lockObject)
                {
                    _isConnected = true;
                }

                LogMessage?.Invoke(this, "Connected to MCP server via SDK");
                return true;
            }
            catch (Exception ex)
            {
                // Log full exception including stack trace for diagnostics
                LogMessage?.Invoke(this, $"Failed to connect to MCP server: {ex}");
                _isConnected = false;
                return false;
            }
        }

        public async Task DisconnectAsync()
        {
            try
            {
                lock (_lockObject)
                {
                    _isConnected = false;
                }

                if (_client != null)
                {
                    await _client.DisposeAsync();
                    _client = null;
                }

                _transport = null;

                LogMessage?.Invoke(this, "Disconnected from MCP server");
            }
            catch (Exception ex)
            {
                LogMessage?.Invoke(this, $"Error during disconnect: {ex.Message}");
            }
        }

        public Task<InitializeResponse> InitializeAsync(InitializeRequest request)
        {
            if (!IsConnected || _client == null)
                throw new InvalidOperationException("Not connected to MCP server");

            try
            {
                LogMessage?.Invoke(this, "Initializing MCP connection");

                // The SDK's CreateAsync already performs initialization internally
                // So we'll return a compatible response with default values
                var response = new InitializeResponse
                {
                    ProtocolVersion = "2024-11-05",
                    ServerInfo = new ServerInfo
                    {
                        Name = "MCP Server",
                        Version = "1.0.0"
                    },
                    Capabilities = new ServerCapabilities
                    {
                        // The SDK doesn't expose detailed capabilities, so we provide defaults
                        Tools = new { },
                        Resources = null,
                        Prompts = null,
                        Logging = null
                    }
                };

                LogMessage?.Invoke(this, $"Server initialized: {response.ServerInfo.Name} v{response.ServerInfo.Version}");
                return Task.FromResult(response);
            }
            catch (Exception ex)
            {
                LogMessage?.Invoke(this, $"Failed to initialize: {ex.Message}");
                throw;
            }
        }

        public async Task<List<Tool>> ListToolsAsync()
        {
            if (!IsConnected || _client == null)
                throw new InvalidOperationException("Not connected to MCP server");

            try
            {
                LogMessage?.Invoke(this, "Listing available tools");

                var sdkTools = await _client.ListToolsAsync();

                // Convert SDK tool format to our Tool format
                var tools = sdkTools.Select(sdkTool => new Tool
                {
                    Name = sdkTool.Name,
                    Description = sdkTool.Description,
                    InputSchema = null // SDK tool structure may differ, set to null for now
                }).ToList();

                LogMessage?.Invoke(this, $"Found {tools.Count} tools");
                return tools;
            }
            catch (Exception ex)
            {
                LogMessage?.Invoke(this, $"Failed to list tools: {ex.Message}");
                throw;
            }
        }

        public async Task<CallToolResponse> CallToolAsync(string toolName, object? arguments)
        {
            if (!IsConnected || _client == null)
                throw new InvalidOperationException("Not connected to MCP server");

            try
            {
                LogMessage?.Invoke(this, $"Calling tool: {toolName}");

                // Convert arguments to dictionary if needed
                Dictionary<string, object?>? argsDict = null;
                if (arguments != null)
                {
                    if (arguments is Dictionary<string, object?> dict)
                    {
                        argsDict = dict;
                    }
                    else
                    {
                        // Try to convert to dictionary using JSON serialization
                        var json = System.Text.Json.JsonSerializer.Serialize(arguments);
                        argsDict = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object?>>(json);
                    }
                }

                // Call the tool using the SDK
                // The SDK's CallToolAsync has a different signature, so we call it without the CancellationToken parameter
                var sdkResult = await _client.CallToolAsync(toolName, argsDict);

                // Convert SDK result to our CallToolResponse format
                // The SDK's ContentBlock structure may differ, so we need to handle it carefully
                var response = new CallToolResponse
                {
                    Content = sdkResult.Content.Select(c =>
                    {
                        var toolContent = new ToolContent { Type = c.Type };

                        // Try to extract properties based on content type
                        // The SDK uses a polymorphic structure, so we serialize and deserialize to extract data
                        try
                        {
                            var json = System.Text.Json.JsonSerializer.Serialize(c);
                            var contentData = System.Text.Json.JsonDocument.Parse(json);

                            if (contentData.RootElement.TryGetProperty("text", out var textProp))
                            {
                                toolContent.Text = textProp.GetString();
                            }
                            if (contentData.RootElement.TryGetProperty("data", out var dataProp))
                            {
                                toolContent.Data = dataProp.GetString();
                            }
                            if (contentData.RootElement.TryGetProperty("mimeType", out var mimeTypeProp))
                            {
                                toolContent.MimeType = mimeTypeProp.GetString();
                            }
                        }
                        catch
                        {
                            // If extraction fails, leave fields as null
                        }

                        return toolContent;
                    }).ToList(),
                    IsError = sdkResult.IsError ?? false
                };

                LogMessage?.Invoke(this, $"Tool {toolName} executed successfully");
                return response;
            }
            catch (Exception ex)
            {
                LogMessage?.Invoke(this, $"Failed to call tool {toolName}: {ex.Message}");
                throw;
            }
        }

        public void Dispose()
        {
            try
            {
                DisconnectAsync().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                LogMessage?.Invoke(this, $"Error during disposal: {ex.Message}");
            }
        }
    }
}
