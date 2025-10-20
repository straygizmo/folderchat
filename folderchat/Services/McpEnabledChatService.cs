using folderchat.Services.Mcp;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using System.Collections;

namespace folderchat.Services
{
    internal class McpEnabledChatService : IChatService
    {
        private readonly IChatService _innerChatService;
        private readonly IMcpService? _mcpService;
        private const int MaxToolIterations = 5; // Prevent infinite loops
        private static Forms.MainForm? _mainForm;
        private readonly bool _supportsSystemMessage;

        public McpEnabledChatService(IChatService innerChatService, IMcpService? mcpService, bool supportsSystemMessage)
        {
            _innerChatService = innerChatService;
            _mcpService = mcpService;
            _supportsSystemMessage = supportsSystemMessage;
        }

        public static void SetMainForm(Forms.MainForm mainForm)
        {
            _mainForm = mainForm;
        }

        public async Task<SendMessageAsyncResult> SendMessageAsync(string userInput, string? systemMessage = null)
        {
            // Check if MCP tools should be included
            if (_mcpService == null)
            {
                System.Diagnostics.Debug.WriteLine("[MCP] MCP service is null");
                return await _innerChatService.SendMessageAsync(userInput, systemMessage);
            }

            var tools = await _mcpService.GetAllToolsAsync();
            System.Diagnostics.Debug.WriteLine($"[MCP] Found {tools.Count} tools from MCP service");

            if (!tools.Any())
            {
                System.Diagnostics.Debug.WriteLine("[MCP] No MCP tools available");
                return await _innerChatService.SendMessageAsync(userInput, systemMessage);
            }

            // Build enhanced system message with tool information
            var enhancedSystemMessage = BuildSystemMessageWithTools(tools, systemMessage);
            System.Diagnostics.Debug.WriteLine("[MCP] Enhanced system message created");

            // Initial LLM call
            string currentUserInput = userInput;
            string actualUserMessageToSend = userInput;
            string? systemMessageToSend = enhancedSystemMessage;

            // If inner service doesn't support system messages (like Claude Code),
            // combine system message with user input
            if (!_supportsSystemMessage && !string.IsNullOrEmpty(enhancedSystemMessage))
            {
                actualUserMessageToSend = $"{enhancedSystemMessage}\n\n---\n\n[User Message]\n{userInput}";
                systemMessageToSend = null;

                // Log the combined message as user message only (no separate system message log)
                _mainForm?.LogChatMessage("user", actualUserMessageToSend);
            }
            else
            {
                // Log system message first, then user input
                if (!string.IsNullOrEmpty(enhancedSystemMessage))
                {
                    _mainForm?.LogChatMessage("system", enhancedSystemMessage);
                }
                _mainForm?.LogChatMessage("user", actualUserMessageToSend);
            }

            string lastResponse = "";
            int iteration = 0;
            var debugInfo = new StringBuilder();
            bool addDebugInfo = false; // Set to true to see debug info in chat

            while (iteration < MaxToolIterations)
            {
                iteration++;
                System.Diagnostics.Debug.WriteLine($"[MCP] Iteration {iteration}/{MaxToolIterations}");
                if (addDebugInfo) debugInfo.AppendLine($"[Debug] Iteration {iteration}");

                // Call LLM with appropriate message format
                var result = await _innerChatService.SendMessageAsync(
                    iteration == 1 ? actualUserMessageToSend : currentUserInput,
                    systemMessageToSend);
                lastResponse = result.AssistantResponse;
                System.Diagnostics.Debug.WriteLine($"[MCP] LLM Response: {lastResponse.Substring(0, Math.Min(200, lastResponse.Length))}...");

                // Log assistant response
                _mainForm?.LogChatMessage("assistant", lastResponse);

                // Check if LLM wants to use tools
                var toolCalls = ExtractToolCallsFromResponse(lastResponse);

                if (!toolCalls.Any())
                {
                    System.Diagnostics.Debug.WriteLine("[MCP] No tool calls found in response");
                    if (addDebugInfo) debugInfo.AppendLine("[Debug] No tool calls detected");
                    break; // No tool calls, return the response
                }

                System.Diagnostics.Debug.WriteLine($"[MCP] Found {toolCalls.Count} tool calls in response");
                if (addDebugInfo) debugInfo.AppendLine($"[Debug] Detected {toolCalls.Count} tool call(s)");

                // Execute tool calls
                var toolResults = new StringBuilder();
                toolResults.AppendLine("\n[Tool Execution Results]");

                foreach (var toolCall in toolCalls)
                {
                    System.Diagnostics.Debug.WriteLine($"[MCP] Executing tool: {toolCall.ToolName}");
                    try
                    {
                        var response = await _mcpService.CallToolAsync(toolCall.ToolName, toolCall.Arguments);
                        toolResults.AppendLine($"\n**Tool: {toolCall.ToolName}**");

                        // Build MCP server response for logging
                        var mcpResponseLog = new StringBuilder();
                        mcpResponseLog.AppendLine($"Tool: {toolCall.ToolName}");

                        foreach (var content in response.Content)
                        {
                            if (content.Type == "text" && !string.IsNullOrEmpty(content.Text))
                            {
                                toolResults.AppendLine(content.Text);
                                mcpResponseLog.AppendLine(content.Text);
                            }
                            else if (content.Type == "image" && !string.IsNullOrEmpty(content.Data))
                            {
                                toolResults.AppendLine($"[Image: {content.MimeType}]");
                                mcpResponseLog.AppendLine($"[Image: {content.MimeType}]");
                            }
                        }

                        if (response.IsError)
                        {
                            toolResults.AppendLine("[Error occurred during tool execution]");
                            mcpResponseLog.AppendLine("[Error occurred during tool execution]");
                        }

                        // Log MCP server response
                        _mainForm?.LogMessage("MCP Server", mcpResponseLog.ToString().Trim());
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"[MCP] Tool execution error: {ex.Message}");
                        toolResults.AppendLine($"\n**Tool: {toolCall.ToolName}** - Error: {ex.Message}");

                        // Log error
                        _mainForm?.LogMessage("MCP Server", $"Tool: {toolCall.ToolName}\nError: {ex.Message}");
                    }
                }

                // Prepare next iteration with tool results
                currentUserInput = $"Based on the tool execution results below, if any tool returned an error or indicates missing or invalid parameters, you MUST retry by responding with a YAML tool call including ALL required parameters as specified in the AVAILABLE TOOLS above. Otherwise, provide your final answer to the original question: \"{userInput}\"\n{toolResults}";
            }

            if (iteration >= MaxToolIterations)
            {
                System.Diagnostics.Debug.WriteLine("[MCP] Max iterations reached");
                lastResponse += "\n\n[Note: Maximum tool execution iterations reached]";
            }

            return new SendMessageAsyncResult
            {
                ActualUserMessage = actualUserMessageToSend,
                AssistantResponse = lastResponse
            };
        }

        private string BuildSystemMessageWithTools(List<Tool> tools, string? originalSystemMessage)
        {
            var sb = new StringBuilder();

            if (!string.IsNullOrEmpty(originalSystemMessage))
            {
                sb.AppendLine(originalSystemMessage);
                sb.AppendLine();
            }

            sb.AppendLine("# 🔧 TOOL USE CAPABILITY ENABLED");
            sb.AppendLine();
            sb.AppendLine("You have access to tools to perform a variety of tasks.");
            sb.AppendLine("To use a tool, respond with a `yaml` code block as shown in the examples.");
            sb.AppendLine();
            sb.AppendLine("## IMPORTANT RULES:");
            sb.AppendLine("1. If the user asks about files or directories, you MUST use the appropriate tool");
            sb.AppendLine("2. Do NOT make up file contents - always use read_file tool");
            sb.AppendLine("3. Do NOT guess directory contents - always use list_directory tool");
            sb.AppendLine("4. First provide your reasoning, THEN use the tool");
            sb.AppendLine("5. You MUST include all required parameters for the selected tool. If a parameter allows null, you may use null, but prefer real values.");
            sb.AppendLine();
            sb.AppendLine("## TOOL CALL FORMAT (YAML):");
            sb.AppendLine();
            sb.AppendLine("```yaml");
            sb.AppendLine("tool:");
            sb.AppendLine("  name: tool_name_here");
            sb.AppendLine("  parameter_name: \"parameter_value\"");
            sb.AppendLine("```");
            sb.AppendLine();
            sb.AppendLine("## EXAMPLE:");
            sb.AppendLine();
            sb.AppendLine("User: \"List files in C:\\Users\\Documents\"");
            sb.AppendLine("Assistant: \"I'll list the files in that directory for you.\"");
            sb.AppendLine("```yaml");
            sb.AppendLine("tool:");
            sb.AppendLine("  name: list_directory");
            sb.AppendLine("  path: \"C:\\Users\\Documents\"");
            sb.AppendLine("```");
            sb.AppendLine();
            sb.AppendLine("## AVAILABLE TOOLS:");
            sb.AppendLine();
            sb.AppendLine("```yaml");

            foreach (var tool in tools)
            {
                sb.AppendLine($"- name: {tool.Name}");
                if (!string.IsNullOrEmpty(tool.Description))
                {
                    sb.AppendLine($"  description: \"{tool.Description.Replace("\"", "\\\"")}\"");
                }

                if (tool.InputSchema != null)
                {
                    try
                    {
                        var schemaJson = JsonSerializer.Serialize(tool.InputSchema, new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull });
                        var schemaDict = JsonSerializer.Deserialize<Dictionary<string, object>>(schemaJson);

                        if (schemaDict != null && schemaDict.TryGetValue("properties", out var propertiesObj))
                        {
                            var propsJson = JsonSerializer.Serialize(propertiesObj);
                            var props = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, object>>>(propsJson);

                            var required = new List<string>();
                            if (schemaDict.TryGetValue("required", out var requiredObj))
                            {
                                var requiredJson = JsonSerializer.Serialize(requiredObj);
                                required = JsonSerializer.Deserialize<List<string>>(requiredJson) ?? new List<string>();
                            }

                            if (props != null && props.Count > 0)
                            {
                                sb.AppendLine("  parameters:");
                                foreach (var prop in props)
                                {
                                    sb.AppendLine($"    - name: {prop.Key}");
                                    if (prop.Value.TryGetValue("description", out var description) && description != null)
                                    {
                                        sb.AppendLine($"      description: \"{description.ToString().Replace("\"", "\\\"")}\"");
                                    }
                                    if (prop.Value.TryGetValue("type", out var type) && type != null)
                                    {
                                        sb.AppendLine($"      type: {type}");
                                    }
                                    sb.AppendLine($"      required: {(required.Contains(prop.Key) ? "true" : "false")}");

                                    // Nested example (for object type)
                                    try
                                    {
                                        if (prop.Value.TryGetValue("type", out var typeObj) && typeObj != null)
                                        {
                                            var typeStr = JsonSerializer.Deserialize<string>(JsonSerializer.Serialize(typeObj));
                                            var isObject = false;
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

                                            if (isObject && prop.Value.TryGetValue("properties", out var childPropsObj))
                                            {
                                                var childPropsJson = JsonSerializer.Serialize(childPropsObj);
                                                var childProps = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, object>>>(childPropsJson);

                                                var childReq = new List<string>();
                                                if (prop.Value.TryGetValue("required", out var childReqObj) && childReqObj != null)
                                                {
                                                    var childReqJson = JsonSerializer.Serialize(childReqObj);
                                                    childReq = JsonSerializer.Deserialize<List<string>>(childReqJson) ?? new List<string>();
                                                }

                                                if (childProps != null && childProps.Count > 0)
                                                {
                                                    sb.AppendLine($"      children:");
                                                    foreach (var child in childProps)
                                                    {
                                                        sb.AppendLine($"        - name: {child.Key}");
                                                        if (child.Value.TryGetValue("description", out var cdesc) && cdesc != null)
                                                        {
                                                            sb.AppendLine($"          description: \"{cdesc.ToString().Replace("\"", "\\\"")}\"");
                                                        }
                                                        if (child.Value.TryGetValue("type", out var ctype) && ctype != null)
                                                        {
                                                            sb.AppendLine($"          type: {ctype}");
                                                        }
                                                        sb.AppendLine($"          required: {(childReq.Contains(child.Key) ? "true" : "false")}");
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    catch { /* ignore nested parse errors */ }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"[MCP] Error parsing tool schema for {tool.Name}: {ex.Message}");
                        sb.AppendLine("  parameters: (schema parsing failed)");
                    }
                }
            }
            sb.AppendLine("```");
            sb.AppendLine();
            sb.AppendLine("## MINIMAL TOOL EXAMPLES (include all required parameters):");
            foreach (var tool in tools)
            {
                try
                {
                    if (tool.InputSchema == null) continue;

                    var schemaJson = JsonSerializer.Serialize(tool.InputSchema, new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull });
                    var schemaDict = JsonSerializer.Deserialize<Dictionary<string, object>>(schemaJson);
                    if (schemaDict == null) continue;

                    var required = new List<string>();
                    if (schemaDict.TryGetValue("required", out var requiredObj) && requiredObj != null)
                    {
                        var requiredJson = JsonSerializer.Serialize(requiredObj);
                        required = JsonSerializer.Deserialize<List<string>>(requiredJson) ?? new List<string>();
                    }
                    if (required.Count == 0) continue;

                    Dictionary<string, Dictionary<string, object>>? props = null;
                    if (schemaDict.TryGetValue("properties", out var propertiesObj) && propertiesObj != null)
                    {
                        var propsJson = JsonSerializer.Serialize(propertiesObj);
                        props = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, object>>>(propsJson);
                    }
                    if (props == null || props.Count == 0) continue;

                    sb.AppendLine("```yaml");
                    sb.AppendLine("tool:");
                    sb.AppendLine($"  name: {tool.Name}");

                    foreach (var key in required)
                    {
                        if (props.TryGetValue(key, out var propSchema) && propSchema != null)
                        {
                            // Support for nested required in object type (e.g., req)
                            try
                            {
                                string? primary = null;
                                if (propSchema.TryGetValue("type", out var typeObj) && typeObj != null)
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
                                primary = primary?.ToLowerInvariant();

                                if (primary == "object" && propSchema.TryGetValue("properties", out var childPropsObj))
                                {
                                    var childPropsJson = JsonSerializer.Serialize(childPropsObj);
                                    var childProps = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, object>>>(childPropsJson);

                                    var childReq = new List<string>();
                                    if (propSchema.TryGetValue("required", out var childReqObj) && childReqObj != null)
                                    {
                                        var childReqJson = JsonSerializer.Serialize(childReqObj);
                                        childReq = JsonSerializer.Deserialize<List<string>>(childReqJson) ?? new List<string>();
                                    }

                                    sb.AppendLine($"  {key}:");
                                    if (childProps != null)
                                    {
                                        foreach (var ck in childReq)
                                        {
                                            if (childProps.TryGetValue(ck, out var cprop))
                                            {
                                                // Estimation of example value
                                                string ex = "\"value\"";
                                                try
                                                {
                                                    if (cprop.TryGetValue("type", out var ctypeObj) && ctypeObj != null)
                                                    {
                                                        var tj = JsonSerializer.Serialize(ctypeObj);
                                                        string? cprim = null;
                                                        try
                                                        {
                                                            var tl = JsonSerializer.Deserialize<List<string>>(tj);
                                                            cprim = tl?.FirstOrDefault(t => !string.Equals(t, "null", StringComparison.OrdinalIgnoreCase)) ?? tl?.FirstOrDefault();
                                                        }
                                                        catch
                                                        {
                                                            cprim = JsonSerializer.Deserialize<string>(tj);
                                                        }
                                                        cprim = cprim?.ToLowerInvariant();

                                                        if (cprim == "integer") ex = "10";
                                                        else if (cprim == "number") ex = "1.0";
                                                        else if (cprim == "boolean") ex = "true";
                                                        else if (cprim == "array")
                                                        {
                                                            string itemsType = "string";
                                                            if (cprop.TryGetValue("items", out var itemsObj) && itemsObj != null)
                                                            {
                                                                var ij = JsonSerializer.Serialize(itemsObj);
                                                                var id = JsonSerializer.Deserialize<Dictionary<string, object>>(ij);
                                                                if (id != null && id.TryGetValue("type", out var itTypeObj) && itTypeObj != null)
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
                                                            if (string.Equals(itemsType, "integer", StringComparison.OrdinalIgnoreCase)) ex = "[ 1 ]";
                                                            else if (string.Equals(itemsType, "number", StringComparison.OrdinalIgnoreCase)) ex = "[ 1.0 ]";
                                                            else if (string.Equals(itemsType, "boolean", StringComparison.OrdinalIgnoreCase)) ex = "[ true ]";
                                                            else ex = "[ \"value\" ]";
                                                        }
                                                        else if (cprim == "object") ex = "{}";
                                                        else if (cprim == "null") ex = "null";
                                                        else ex = "\"value\"";
                                                    }
                                                }
                                                catch { }
                                                sb.AppendLine($"    {ck}: {ex}");
                                            }
                                        }
                                    }
                                    continue;
                                }
                            }
                            catch { }

                            // Normal example for non-object
                            string exampleValue = "\"value\"";
                            try
                            {
                                if (propSchema.TryGetValue("type", out var typeObj) && typeObj != null)
                                {
                                    var typeJson = JsonSerializer.Serialize(typeObj);
                                    List<string>? types = null;
                                    string? primary = null;
                                    try
                                    {
                                        types = JsonSerializer.Deserialize<List<string>>(typeJson);
                                        primary = types?.FirstOrDefault(t => !string.Equals(t, "null", StringComparison.OrdinalIgnoreCase)) ?? types?.FirstOrDefault();
                                    }
                                    catch
                                    {
                                        primary = JsonSerializer.Deserialize<string>(typeJson);
                                    }
                                    primary = primary?.ToLowerInvariant();

                                    if (primary == "integer") exampleValue = "10";
                                    else if (primary == "number") exampleValue = "1.0";
                                    else if (primary == "boolean") exampleValue = "true";
                                    else if (primary == "array")
                                    {
                                        string itemsType = "string";
                                        if (propSchema.TryGetValue("items", out var itemsObj) && itemsObj != null)
                                        {
                                            var itemsJson = JsonSerializer.Serialize(itemsObj);
                                            var itemsDict = JsonSerializer.Deserialize<Dictionary<string, object>>(itemsJson);
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
                                        if (string.Equals(itemsType, "integer", StringComparison.OrdinalIgnoreCase)) exampleValue = "[ 1 ]";
                                        else if (string.Equals(itemsType, "number", StringComparison.OrdinalIgnoreCase)) exampleValue = "[ 1.0 ]";
                                        else if (string.Equals(itemsType, "boolean", StringComparison.OrdinalIgnoreCase)) exampleValue = "[ true ]";
                                        else exampleValue = "[ \"value\" ]";
                                    }
                                    else if (primary == "object") exampleValue = "{}";
                                    else if (primary == "null") exampleValue = "null";
                                    else exampleValue = "\"value\"";
                                }
                            }
                            catch { /* ignore example generation errors */ }
                            sb.AppendLine($"  {key}: {exampleValue}");
                        }
                        else
                        {
                            sb.AppendLine($"  {key}: \"value\"");
                        }
                    }

                    sb.AppendLine("```");
                }
                catch { /* ignore per-tool example errors */ }
            }

            sb.AppendLine();
            sb.AppendLine("Remember: When you need to use a tool, use the ```yaml code block with YAML format shown above!");

            return sb.ToString();
        }

        private List<ToolCall> ExtractToolCallsFromResponse(string response)
        {
            var toolCalls = new List<ToolCall>();
            var lines = response.Split('\n');

            var blockLines = new List<string>();
            bool inFencedBlock = false;
            string? fenceLang = null;
            bool inUnfencedBlock = false;

            void parseAndClear(List<string> block)
            {
                if (!block.Any()) return;

                var text = string.Join("\n", block);
                Dictionary<string, object?>? root = null;
                Dictionary<string, object?>? toolMap = null;

                try
                {
                    if (string.Equals(fenceLang, "json", StringComparison.OrdinalIgnoreCase))
                    {
                        root = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object?>>(text);
                    }
                    else
                    {
                        var deserializer = new DeserializerBuilder()
                            .WithNamingConvention(CamelCaseNamingConvention.Instance)
                            .Build();
                        var obj = deserializer.Deserialize<object>(text);
                        root = ToPlain(obj) as Dictionary<string, object?>;
                    }
                }
                catch
                {
                    // ignore parse errors
                }

                if (root != null)
                {
                    if (root.TryGetValue("tool", out var toolSection) && toolSection is object)
                    {
                        toolMap = ToPlain(toolSection) as Dictionary<string, object?>;
                    }
                    else if (root.ContainsKey("name") || root.ContainsKey("req"))
                    {
                        // treat whole root as tool map if it looks like a tool block without "tool:" wrapper
                        toolMap = root;
                    }
                }

                if (toolMap != null)
                {
                    try
                    {
                        string? toolName = null;
                        if (toolMap.TryGetValue("name", out var nameVal) && nameVal != null)
                        {
                            toolName = nameVal.ToString();
                        }

                        if (!string.IsNullOrEmpty(toolName))
                        {
                            var argsDict = new Dictionary<string, object?>();
                            foreach (var kv in toolMap)
                            {
                                if (string.Equals(kv.Key, "name", StringComparison.OrdinalIgnoreCase)) continue;
                                argsDict[kv.Key] = kv.Value;
                            }

                            toolCalls.Add(new ToolCall { ToolName = toolName, Arguments = argsDict });
                        }
                    }
                    catch { /* ignore */ }
                }

                block.Clear();
                fenceLang = null;
            }

            foreach (var line in lines)
            {
                var trimmed = line.Trim();

                if (inFencedBlock)
                {
                    if (trimmed == "```")
                    {
                        parseAndClear(blockLines);
                        inFencedBlock = false;
                    }
                    else
                    {
                        blockLines.Add(line);
                    }
                    continue;
                }

                if (inUnfencedBlock)
                {
                    if (line.StartsWith("  ") || line.StartsWith("\t") || string.IsNullOrWhiteSpace(line))
                    {
                        blockLines.Add(line);
                    }
                    else
                    {
                        parseAndClear(blockLines);
                        inUnfencedBlock = false;
                    }
                }

                if (!inFencedBlock && !inUnfencedBlock)
                {
                    if (trimmed.StartsWith("```yaml", StringComparison.OrdinalIgnoreCase))
                    {
                        inFencedBlock = true;
                        fenceLang = "yaml";
                    }
                    else if (trimmed.StartsWith("```json", StringComparison.OrdinalIgnoreCase))
                    {
                        inFencedBlock = true;
                        fenceLang = "json";
                    }
                    else if (trimmed == "tool:")
                    {
                        inUnfencedBlock = true;
                        blockLines.Add(line);
                    }
                }
            }

            if (blockLines.Any())
            {
                parseAndClear(blockLines);
            }

            return toolCalls;

            // Local helper to convert YamlDotNet nested dictionaries/lists to plain .NET types
            object? ToPlain(object? obj)
            {
                if (obj == null) return null;

                if (obj is Dictionary<object, object> d)
                {
                    var res = new Dictionary<string, object?>();
                    foreach (var kv in d)
                    {
                        var key = kv.Key?.ToString() ?? "";
                        res[key] = ToPlain(kv.Value);
                    }
                    return res;
                }
                if (obj is IDictionary id)
                {
                    var res = new Dictionary<string, object?>();
                    foreach (DictionaryEntry de in id)
                    {
                        var key = de.Key?.ToString() ?? "";
                        res[key] = ToPlain(de.Value);
                    }
                    return res;
                }
                if (obj is IEnumerable en && obj is not string)
                {
                    var list = new List<object?>();
                    foreach (var item in en)
                    {
                        list.Add(ToPlain(item));
                    }
                    return list;
                }
                return obj;
            }
        }

        public void ClearHistory()
        {
            _innerChatService.ClearHistory();
        }

        private class ToolCall
        {
            public string ToolName { get; set; } = string.Empty;
            public object? Arguments { get; set; }
        }
    }
}