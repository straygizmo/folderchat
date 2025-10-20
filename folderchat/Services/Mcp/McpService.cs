using folderchat.Models;
using System.Collections.Concurrent;
using System.Linq;
using System.Text.Json;

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

        private class ArgNormalizationResult
        {
            public Dictionary<string, object?> Args { get; set; } = new();
            public List<string> MissingRequiredNonNullable { get; set; } = new();
            public List<string> RequiredKeys { get; set; } = new();
            public Dictionary<string, string> RequiredTypes { get; set; } = new(); // key -> type display (e.g., "integer|null")
            public Dictionary<string, string> ExampleValues { get; set; } = new(); // key -> example (e.g., "10")
        }

        private ArgNormalizationResult BuildNormalizedArgs(string serverId, string toolName, object? arguments)
        {
            var result = new ArgNormalizationResult();
            var addedNulls = new List<string>();

            // 1) Convert arguments to a dictionary (accepting object? because it could be just a string)
            try
            {
                if (arguments != null)
                {
                    try
                    {
                        var json = JsonSerializer.Serialize(arguments);
                        var parsed = JsonSerializer.Deserialize<Dictionary<string, object?>>(json);
                        if (parsed != null)
                        {
                            result.Args = new Dictionary<string, object?>(parsed);
                        }
                    }
                    catch
                    {
                        // Silently skip if dictionary conversion fails (continue with the original empty dictionary)
                    }
                }
            }
            catch
            {
                // noop
            }

            // 2) Get the schema of the target tool
            Dictionary<string, object?>? schemaDict = null;
            Dictionary<string, object?>? props = null;
            var required = new List<string>();

            try
            {
                if (_serverTools.TryGetValue(serverId, out var tools))
                {
                    var tool = tools.FirstOrDefault(t => string.Equals(t.Name, toolName, StringComparison.OrdinalIgnoreCase));
                    if (tool?.InputSchema != null)
                    {
                        var schemaJson = JsonSerializer.Serialize(tool.InputSchema);
                        schemaDict = JsonSerializer.Deserialize<Dictionary<string, object?>>(schemaJson);

                        if (schemaDict != null)
                        {
                            if (schemaDict.TryGetValue("required", out var requiredObj) && requiredObj != null)
                            {
                                var requiredJson = JsonSerializer.Serialize(requiredObj);
                                required = JsonSerializer.Deserialize<List<string>>(requiredJson) ?? new List<string>();
                            }

                            if (schemaDict.TryGetValue("properties", out var propsObj) && propsObj != null)
                            {
                                var propsJson = JsonSerializer.Serialize(propsObj);
                                props = JsonSerializer.Deserialize<Dictionary<string, object?>>(propsJson);
                            }
                        }
                    }
                }
            }
            catch
            {
                // noop
            }

            result.RequiredKeys = required;

            // 3) Helper for extracting type information
            string BuildTypeDisplay(object? typeObj)
            {
                try
                {
                    if (typeObj == null) return "any";
                    var typeJson = JsonSerializer.Serialize(typeObj);
                    try
                    {
                        var list = JsonSerializer.Deserialize<List<string>>(typeJson);
                        if (list != null && list.Count > 0) return string.Join("|", list);
                    }
                    catch
                    {
                        var s = JsonSerializer.Deserialize<string>(typeJson);
                        if (!string.IsNullOrWhiteSpace(s)) return s;
                    }
                }
                catch { }
                return "any";
            }

            string PickPrimaryType(object? typeObj)
            {
                try
                {
                    if (typeObj == null) return "string";
                    var typeJson = JsonSerializer.Serialize(typeObj);
                    try
                    {
                        var list = JsonSerializer.Deserialize<List<string>>(typeJson);
                        if (list != null && list.Count > 0)
                        {
                            return (list.FirstOrDefault(t => !string.Equals(t, "null", StringComparison.OrdinalIgnoreCase)) ?? list[0]).ToLowerInvariant();
                        }
                    }
                    catch
                    {
                        var s = JsonSerializer.Deserialize<string>(typeJson);
                        if (!string.IsNullOrWhiteSpace(s)) return s.ToLowerInvariant();
                    }
                }
                catch { }
                return "string";
            }

            string BuildExampleForType(string primaryType, Dictionary<string, object?>? propSchema)
            {
                try
                {
                    if (primaryType == "integer") return "10";
                    if (primaryType == "number") return "1.0";
                    if (primaryType == "boolean") return "true";
                    if (primaryType == "array")
                    {
                        // Check the type of items
                        string itemsType = "string";
                        if (propSchema != null && propSchema.TryGetValue("items", out var itemsObj) && itemsObj != null)
                        {
                            var itemsJson = JsonSerializer.Serialize(itemsObj);
                            var itemsDict = JsonSerializer.Deserialize<Dictionary<string, object?>>(itemsJson);
                            if (itemsDict != null && itemsDict.TryGetValue("type", out var itTypeObj) && itTypeObj != null)
                            {
                                try
                                {
                                    var itTypeJson = JsonSerializer.Serialize(itTypeObj);
                                    try
                                    {
                                        var itList = JsonSerializer.Deserialize<List<string>>(itTypeJson);
                                        if (itList != null && itList.Count > 0) itemsType = itList[0];
                                    }
                                    catch
                                    {
                                        itemsType = JsonSerializer.Deserialize<string>(itTypeJson) ?? "string";
                                    }
                                }
                                catch { }
                            }
                        }
                        if (string.Equals(itemsType, "integer", StringComparison.OrdinalIgnoreCase)) return "[ 1 ]";
                        if (string.Equals(itemsType, "number", StringComparison.OrdinalIgnoreCase)) return "[ 1.0 ]";
                        if (string.Equals(itemsType, "boolean", StringComparison.OrdinalIgnoreCase)) return "[ true ]";
                        return "[ \"value\" ]";
                    }
                    if (primaryType == "object") return "{}";
                    if (primaryType == "null") return "null";
                }
                catch { }
                return "\"value\"";
            }

            bool TypeAllowsNull(object? typeObj)
            {
                try
                {
                    if (typeObj == null) return false;
                    var typeJson = JsonSerializer.Serialize(typeObj);
                    try
                    {
                        var list = JsonSerializer.Deserialize<List<string>>(typeJson);
                        if (list != null) return list.Any(t => string.Equals(t, "null", StringComparison.OrdinalIgnoreCase));
                    }
                    catch
                    {
                        var s = JsonSerializer.Deserialize<string>(typeJson);
                        return string.Equals(s, "null", StringComparison.OrdinalIgnoreCase);
                    }
                }
                catch { }
                return false;
            }

            object? CoerceValue(object? raw, string primaryType, Dictionary<string, object?>? propSchema)
            {
                try
                {
                    if (raw == null) return null;

                    // Expand JsonElement to its raw type (workaround for Dictionary iteration deserialization)
                    if (raw is JsonElement je)
                    {
                        switch (je.ValueKind)
                        {
                            case System.Text.Json.JsonValueKind.Number:
                                if (primaryType == "integer")
                                {
                                    if (je.TryGetInt64(out var il)) return il;
                                    if (je.TryGetDouble(out var dl)) return (long)dl;
                                }
                                if (je.TryGetDouble(out var d)) return d;
                                break;
                            case System.Text.Json.JsonValueKind.True:
                            case System.Text.Json.JsonValueKind.False:
                                return je.GetBoolean();
                            case System.Text.Json.JsonValueKind.Array:
                                // Extract as an array as is
                                var list = new List<object?>();
                                foreach (var e in je.EnumerateArray())
                                {
                                    list.Add(e.ValueKind == System.Text.Json.JsonValueKind.String ? e.GetString() : (object?)e.ToString());
                                }
                                return list;
                            case System.Text.Json.JsonValueKind.String:
                                raw = je.GetString();
                                break;
                            case System.Text.Json.JsonValueKind.Object:
                                // Treat object as a dictionary
                                var dict = JsonSerializer.Deserialize<Dictionary<string, object?>>(je.GetRawText());
                                return dict;
                            case System.Text.Json.JsonValueKind.Null:
                            case System.Text.Json.JsonValueKind.Undefined:
                                return null;
                        }
                    }

                    if (raw is string s)
                    {
                        if (primaryType == "integer")
                        {
                            if (long.TryParse(s, out var il)) return il;
                            if (double.TryParse(s, out var dl)) return (long)dl;
                        }
                        else if (primaryType == "number")
                        {
                            if (double.TryParse(s, out var d)) return d;
                        }
                        else if (primaryType == "boolean")
                        {
                            if (bool.TryParse(s, out var b)) return b;
                        }
                        else if (primaryType == "array")
                        {
                            // Preferentially interpret JSON array string
                            try
                            {
                                var arr = JsonSerializer.Deserialize<List<object?>>(s);
                                if (arr != null) return arr;
                            }
                            catch
                            {
                                // Lightly support comma-separated values (treat elements as strings)
                                var parts = s.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                                if (parts.Length > 1) return parts.Select(p => (object?)p).ToList();
                                // Wrap as a single element
                                return new List<object?> { s };
                            }
                        }
                        else if (primaryType == "object")
                        {
                            try
                            {
                                var obj = JsonSerializer.Deserialize<Dictionary<string, object?>>(s);
                                if (obj != null) return obj;
                            }
                            catch { }
                        }
                    }

                    // If already a number/boolean, etc., return as is
                    return raw;
                }
                catch
                {
                    return raw;
                }
            }

            // 4) Completion of unspecified required parameters and type conversion of existing arguments
            if (props != null)
            {
                foreach (var kv in props)
                {
                    var key = kv.Key;
                    var propSchemaJson = JsonSerializer.Serialize(kv.Value);
                    var propSchema = JsonSerializer.Deserialize<Dictionary<string, object?>>(propSchemaJson) ?? new Dictionary<string, object?>();
                    propSchema.TryGetValue("type", out var typeObj);

                    var typeDisplay = BuildTypeDisplay(typeObj);
                    var primaryType = PickPrimaryType(typeObj);
                    result.RequiredTypes[key] = typeDisplay;

                    // Convert existing values to match the type as much as possible
                    if (result.Args.ContainsKey(key))
                    {
                        var coerced = CoerceValue(result.Args[key], primaryType, propSchema);
                        result.Args[key] = coerced;
                    }
                }
            }

            // 4b) Nesting completion for object properties (e.g., req wrapper)
            if (props != null)
            {
                foreach (var kv in props)
                {
                    var key = kv.Key;
                    var propSchemaJson = JsonSerializer.Serialize(kv.Value);
                    var propSchema = JsonSerializer.Deserialize<Dictionary<string, object?>>(propSchemaJson) ?? new Dictionary<string, object?>();
                    object? typeObj = null;
                    if (propSchema.TryGetValue("type", out var tObj)) typeObj = tObj;

                    // Determine if it is an object type
                    bool isObject = false;
                    try
                    {
                        var typeStr = JsonSerializer.Deserialize<string>(JsonSerializer.Serialize(typeObj));
                        if (!string.IsNullOrEmpty(typeStr))
                        {
                            isObject = string.Equals(typeStr, "object", StringComparison.OrdinalIgnoreCase);
                        }
                        else
                        {
                            var typeList = JsonSerializer.Deserialize<List<string>>(JsonSerializer.Serialize(typeObj));
                            if (typeList != null)
                            {
                                isObject = typeList.Any(t => string.Equals(t, "object", StringComparison.OrdinalIgnoreCase));
                            }
                        }
                    }
                    catch { }

                    if (!isObject) continue;

                    // Get child properties
                    Dictionary<string, Dictionary<string, object>>? childProps = null;
                    try
                    {
                        if (propSchema.TryGetValue("properties", out var childPropsObj) && childPropsObj != null)
                        {
                            var childPropsJson = JsonSerializer.Serialize(childPropsObj);
                            childProps = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, object>>>(childPropsJson);
                        }
                    }
                    catch { }

                    // Get/create existing parent object value (key)
                    Dictionary<string, object?> parentObj;
                    if (result.Args.TryGetValue(key, out var parentVal) && parentVal is Dictionary<string, object?> pv)
                    {
                        parentObj = pv;
                    }
                    else
                    {
                        parentObj = new Dictionary<string, object?>();
                    }

                    // Move child keys existing at the top level to the parent
                    if (childProps != null)
                    {
                        foreach (var ck in childProps.Keys)
                        {
                            if (result.Args.TryGetValue(ck, out var childVal))
                            {
                                parentObj[ck] = childVal;
                                result.Args.Remove(ck);
                            }
                        }
                    }

                    // Set parent object (if moved or already existed)
                    if (parentObj.Count > 0 || (result.Args.ContainsKey(key) && result.Args[key] is Dictionary<string, object?>))
                    {
                        result.Args[key] = parentObj;
                    }
                }
            }

            // 5) Process required (complement null for nullable / add to missing list for non-nullable)
            if (props != null)
            {
                foreach (var key in required)
                {
                    var allowNull = false;
                    Dictionary<string, object?>? propSchema = null;
                    if (props.TryGetValue(key, out var propSchemaObj) && propSchemaObj != null)
                    {
                        var propSchemaJson = JsonSerializer.Serialize(propSchemaObj);
                        propSchema = JsonSerializer.Deserialize<Dictionary<string, object?>>(propSchemaJson);
                        if (propSchema != null && propSchema.TryGetValue("type", out var typeObj) && typeObj != null)
                        {
                            allowNull = TypeAllowsNull(typeObj);
                            var primary = PickPrimaryType(typeObj);
                            result.ExampleValues[key] = BuildExampleForType(primary, propSchema);
                        }
                        else
                        {
                            result.ExampleValues[key] = "\"value\"";
                        }
                    }
                    else
                    {
                        result.ExampleValues[key] = "\"value\"";
                    }

                    if (!result.Args.ContainsKey(key))
                    {
                        if (allowNull)
                        {
                            result.Args[key] = null;
                            addedNulls.Add(key);
                        }
                        else
                        {
                            result.MissingRequiredNonNullable.Add(key);
                        }
                    }
                }
            }

            if (addedNulls.Count > 0)
            {
                LogMessage?.Invoke(this, $"Normalized arguments for {toolName}: added null for [{string.Join(", ", addedNulls)}]");
            }

            return result;
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
                var norm = BuildNormalizedArgs(serverId, toolName, arguments);
                if (norm.MissingRequiredNonNullable.Count > 0)
                {
                    // Missing required arguments (non-nullable) - return a hint to make it easier for the user/LLM to correct
                    var missingList = string.Join(", ", norm.MissingRequiredNonNullable.Select(k => $"{k} ({(norm.RequiredTypes.TryGetValue(k, out var t) ? t : "unknown")})"));

                    // Minimal YAML example (required arguments only): supports nesting (e.g., under req)
                    var yamlSb = new System.Text.StringBuilder();
                    yamlSb.AppendLine("```yaml");
                    yamlSb.AppendLine("tool:");
                    yamlSb.AppendLine($"  name: {toolName}");

                    // First, output the top-level required keys. For object types (e.g., req), expand the children's required keys as well.
                    // Get the tool's schema and parse nested required fields
                    Dictionary<string, object?>? schemaDictForHint = null;
                    Dictionary<string, object?>? propsForHint = null;
                    List<string> requiredForHint = new List<string>();
                    try
                    {
                        if (_serverTools.TryGetValue(serverId, out var toolsForHint))
                        {
                            var toolForHint = toolsForHint.FirstOrDefault(t => string.Equals(t.Name, toolName, StringComparison.OrdinalIgnoreCase));
                            if (toolForHint?.InputSchema != null)
                            {
                                var schemaJson = JsonSerializer.Serialize(toolForHint.InputSchema);
                                schemaDictForHint = JsonSerializer.Deserialize<Dictionary<string, object?>>(schemaJson);

                                if (schemaDictForHint != null)
                                {
                                    if (schemaDictForHint.TryGetValue("required", out var requiredObj) && requiredObj != null)
                                    {
                                        var requiredJson = JsonSerializer.Serialize(requiredObj);
                                        requiredForHint = JsonSerializer.Deserialize<List<string>>(requiredJson) ?? new List<string>();
                                    }

                                    if (schemaDictForHint.TryGetValue("properties", out var propsObj) && propsObj != null)
                                    {
                                        var propsJson = JsonSerializer.Serialize(propsObj);
                                        propsForHint = JsonSerializer.Deserialize<Dictionary<string, object?>>(propsJson);
                                    }
                                }
                            }
                        }
                    }
                    catch { /* Fallback on schema acquisition failure */ }

                    // Infer example value from type (simplified version)
                    string ExampleForType(object? typeObj, Dictionary<string, object?>? propSchema)
                    {
                        string? primary = null;
                        try
                        {
                            var typeJson = JsonSerializer.Serialize(typeObj);
                            try
                            {
                                var types = JsonSerializer.Deserialize<List<string>>(typeJson);
                                primary = types?.FirstOrDefault(t => !string.Equals(t, "null", StringComparison.OrdinalIgnoreCase)) ?? types?.FirstOrDefault();
                            }
                            catch
                            {
                                primary = JsonSerializer.Deserialize<string>(typeJson);
                            }
                        }
                        catch { }
                        primary = primary?.ToLowerInvariant();

                        if (primary == "integer") return "10";
                        if (primary == "number") return "1.0";
                        if (primary == "boolean") return "true";
                        if (primary == "array")
                        {
                            // Check items type
                            string itemsType = "string";
                            try
                            {
                                if (propSchema != null && propSchema.TryGetValue("items", out var itemsObj) && itemsObj != null)
                                {
                                    var itemsJson = JsonSerializer.Serialize(itemsObj);
                                    var itemsDict = JsonSerializer.Deserialize<Dictionary<string, object?>>(itemsJson);
                                    if (itemsDict != null && itemsDict.TryGetValue("type", out var itTypeObj) && itTypeObj != null)
                                    {
                                        try
                                        {
                                            itemsType = JsonSerializer.Deserialize<string>(JsonSerializer.Serialize(itTypeObj)) ?? "string";
                                        }
                                        catch
                                        {
                                            var itList = JsonSerializer.Deserialize<List<string>>(JsonSerializer.Serialize(itTypeObj));
                                            if (itList != null && itList.Count > 0) itemsType = itList[0];
                                        }
                                    }
                                }
                            }
                            catch { }
                            if (string.Equals(itemsType, "integer", StringComparison.OrdinalIgnoreCase)) return "[ 1 ]";
                            if (string.Equals(itemsType, "number", StringComparison.OrdinalIgnoreCase)) return "[ 1.0 ]";
                            if (string.Equals(itemsType, "boolean", StringComparison.OrdinalIgnoreCase)) return "[ true ]";
                            return "[ \"value\" ]";
                        }
                        if (primary == "object") return "{}";
                        if (primary == "null") return "null";
                        return "\"value\"";
                    }

                    // For top-level required keys, expand nested required for object types, otherwise use example values
                    if (propsForHint != null)
                    {
                        foreach (var key in requiredForHint)
                        {
                            // Key's schema
                            Dictionary<string, object?>? propSchema = null;
                            if (propsForHint.TryGetValue(key, out var propSchemaObj) && propSchemaObj != null)
                            {
                                var propSchemaJson = JsonSerializer.Serialize(propSchemaObj);
                                propSchema = JsonSerializer.Deserialize<Dictionary<string, object?>>(propSchemaJson);
                            }

                            // type determination
                            object? typeObj = null;
                            if (propSchema != null && propSchema.TryGetValue("type", out var tObj)) typeObj = tObj;

                            // If object type, expand child required
                            bool isObject = false;
                            try
                            {
                                var typeStr = JsonSerializer.Deserialize<string>(JsonSerializer.Serialize(typeObj));
                                if (!string.IsNullOrEmpty(typeStr))
                                {
                                    isObject = string.Equals(typeStr, "object", StringComparison.OrdinalIgnoreCase);
                                }
                                else
                                {
                                    var typeList = JsonSerializer.Deserialize<List<string>>(JsonSerializer.Serialize(typeObj));
                                    if (typeList != null)
                                    {
                                        isObject = typeList.Any(t => string.Equals(t, "object", StringComparison.OrdinalIgnoreCase));
                                    }
                                }
                            }
                            catch { }

                            if (isObject && propSchema != null)
                            {
                                // child properties/required
                                Dictionary<string, Dictionary<string, object>>? childProps = null;
                                var childReq = new List<string>();
                                try
                                {
                                    if (propSchema.TryGetValue("properties", out var childPropsObj) && childPropsObj != null)
                                    {
                                        var childPropsJson = JsonSerializer.Serialize(childPropsObj);
                                        childProps = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, object>>>(childPropsJson);
                                    }
                                    if (propSchema.TryGetValue("required", out var childReqObj) && childReqObj != null)
                                    {
                                        var childReqJson = JsonSerializer.Serialize(childReqObj);
                                        childReq = JsonSerializer.Deserialize<List<string>>(childReqJson) ?? new List<string>();
                                    }
                                }
                                catch { }

                                yamlSb.AppendLine($"  {key}:");
                                if (childProps != null && childReq.Count > 0)
                                {
                                    foreach (var ck in childReq)
                                    {
                                        if (childProps.TryGetValue(ck, out var cprop))
                                        {
                                            object? ctypeObj = null;
                                            if (cprop.TryGetValue("type", out var ct)) ctypeObj = ct;
                                            var ex = ExampleForType(ctypeObj, cprop);
                                            yamlSb.AppendLine($"    {ck}: {ex}");
                                        }
                                        else
                                        {
                                            // Default if not defined in schema
                                            yamlSb.AppendLine($"    {ck}: \"value\"");
                                        }
                                    }
                                }
                                else
                                {
                                    // If there is no child information, exemplify as an empty object
                                    yamlSb.AppendLine("    # add required fields here");
                                }
                            }
                            else
                            {
                                // Normal key for non-object
                                var ex = norm.ExampleValues.TryGetValue(key, out var exv) ? exv : ExampleForType(typeObj, propSchema);
                                yamlSb.AppendLine($"  {key}: {ex}");
                            }
                        }
                    }
                    else
                    {
                        // Fallback: conventional simple output
                        foreach (var key in norm.RequiredKeys)
                        {
                            var example = norm.ExampleValues.TryGetValue(key, out var ex) ? ex : "\"value\"";
                            yamlSb.AppendLine($"  {key}: {example}");
                        }
                    }

                    yamlSb.AppendLine("```");

                    var hint = $"Missing required parameter(s) for tool '{toolName}': {missingList}\n" +
                               "Please resend the tool call including ALL required parameters.\n" +
                               "Minimal example:\n{yamlSb}";

                    LogMessage?.Invoke(this, $"Validation failed for tool {toolName}: {missingList}");

                    return new CallToolResponse
                    {
                        IsError = true,
                        Content = new List<ToolContent>
                        {
                            new ToolContent
                            {
                                Type = "text",
                                Text = hint
                            }
                        }
                    };
                }

                return await client.CallToolAsync(toolName, norm.Args);
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