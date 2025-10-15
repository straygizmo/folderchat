using folderchat.Services.Mcp;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

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
            System.Diagnostics.Debug.WriteLine($"[MCP] Enhanced system message created");

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
                    iteration == 1 ? systemMessageToSend : null);
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
                currentUserInput = $"Based on the tool execution results below, please provide your final answer to the original question: \"{userInput}\"\n{toolResults}";
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
            sb.AppendLine("User: \"List files in C:\\\\Users\\\\Documents\"");
            sb.AppendLine("Assistant: \"I'll list the files in that directory for you.\"");
            sb.AppendLine("```yaml");
            sb.AppendLine("tool:");
            sb.AppendLine("  name: list_directory");
            sb.AppendLine("  path: \"C:\\\\Users\\\\Documents\"");
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

                            List<string> required = new List<string>();
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
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"[MCP] Error parsing tool schema for {tool.Name}: {ex.Message}");
                        // Fallback to simpler output if schema parsing fails
                        sb.AppendLine("  parameters: (schema parsing failed)");
                    }
                }
            }
            sb.AppendLine("```");
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
            bool inUnfencedBlock = false;

            Action<List<string>> parseAndClear = (block) => {
                if (!block.Any()) return;

                try
                {
                    string? toolName = null;
                    var argsDict = new Dictionary<string, object>();
                    bool inToolSection = false;

                    foreach (var blockLine in block)
                    {
                        if (string.IsNullOrWhiteSpace(blockLine)) continue;

                        if (blockLine.Trim() == "tool:")
                        {
                            inToolSection = true;
                            continue;
                        }

                        if (inToolSection)
                        {
                            if (!blockLine.StartsWith("  ") && !blockLine.StartsWith("\t")) continue;

                            var parts = blockLine.Trim().Split(new[] { ':' }, 2);
                            if (parts.Length != 2) continue;

                            var key = parts[0].Trim();
                            var value = parts[1].Trim();

                            if (key.Equals("name", StringComparison.OrdinalIgnoreCase))
                            {
                                toolName = value;
                            }
                            else
                            {
                                if (value.Length >= 2 && value.StartsWith("\"") && value.EndsWith("\""))
                                {
                                    argsDict[key] = value.Substring(1, value.Length - 2);
                                }
                                else
                                {
                                    argsDict[key] = value;
                                }
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(toolName))
                    {
                        toolCalls.Add(new ToolCall { ToolName = toolName, Arguments = argsDict });
                    }
                }
                catch { /* Ignore parsing errors */ }

                block.Clear();
            };

            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();

                if (inFencedBlock)
                {
                    if (trimmedLine == "```")
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
                        // Fall through to check if the current line starts a new block
                    }
                }

                // This check happens if not in a block, or after an unfenced block ends
                if (!inFencedBlock && !inUnfencedBlock)
                {
                    if (trimmedLine.StartsWith("```yaml"))
                    {
                        inFencedBlock = true;
                    }
                    else if (trimmedLine == "tool:")
                    {
                        inUnfencedBlock = true;
                        blockLines.Add(line);
                    }
                }
            }

            // If the response ends while in a block
            if (blockLines.Any())
            {
                parseAndClear(blockLines);
            }

            return toolCalls;
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