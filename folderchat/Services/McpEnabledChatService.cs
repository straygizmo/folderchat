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

            sb.AppendLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            sb.AppendLine("🔧 TOOL USE CAPABILITY ENABLED");
            sb.AppendLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            sb.AppendLine();
            sb.AppendLine("You have access to tools that can interact with the file system and perform actions.");
            sb.AppendLine("When you need to use a tool, respond with a code block using 'tool' language identifier.");
            sb.AppendLine();
            sb.AppendLine("IMPORTANT RULES:");
            sb.AppendLine("1. If the user asks about files or directories, you MUST use the appropriate tool");
            sb.AppendLine("2. Do NOT make up file contents - always use read_file tool");
            sb.AppendLine("3. Do NOT guess directory contents - always use list_directory tool");
            sb.AppendLine("4. First provide your reasoning, THEN use the tool");
            sb.AppendLine();
            sb.AppendLine("TOOL CALL FORMAT:");
            sb.AppendLine("```tool");
            sb.AppendLine("tool_name_here");
            sb.AppendLine("{");
            sb.AppendLine("  \"parameter_name\": \"parameter_value\"");
            sb.AppendLine("}");
            sb.AppendLine("```");
            sb.AppendLine();
            sb.AppendLine("EXAMPLE:");
            sb.AppendLine("User: \"List files in C:\\\\Users\\\\Documents\"");
            sb.AppendLine("Assistant: \"I'll list the files in that directory for you.");
            sb.AppendLine("```tool");
            sb.AppendLine("list_directory");
            sb.AppendLine("{");
            sb.AppendLine("  \"path\": \"C:\\\\Users\\\\Documents\"");
            sb.AppendLine("}");
            sb.AppendLine("```\"");
            sb.AppendLine();
            sb.AppendLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            sb.AppendLine("AVAILABLE TOOLS:");
            sb.AppendLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            sb.AppendLine();

            foreach (var tool in tools)
            {
                sb.AppendLine($"📌 {tool.Name}");
                sb.AppendLine($"   {tool.Description}");

                // Try to extract schema information
                if (tool.InputSchema != null)
                {
                    try
                    {
                        var schemaJson = JsonSerializer.Serialize(tool.InputSchema);
                        var schemaDict = JsonSerializer.Deserialize<Dictionary<string, object>>(schemaJson);
                        if (schemaDict != null && schemaDict.ContainsKey("properties"))
                        {
                            var propsJson = JsonSerializer.Serialize(schemaDict["properties"]);
                            var props = JsonSerializer.Deserialize<Dictionary<string, object>>(propsJson);
                            if (props != null && props.Count > 0)
                            {
                                sb.AppendLine("   Parameters: " + string.Join(", ", props.Keys));
                            }
                        }

                        // Check for required fields
                        if (schemaDict != null && schemaDict.ContainsKey("required"))
                        {
                            var requiredJson = JsonSerializer.Serialize(schemaDict["required"]);
                            var required = JsonSerializer.Deserialize<List<string>>(requiredJson);
                            if (required != null && required.Count > 0)
                            {
                                sb.AppendLine("   Required: " + string.Join(", ", required));
                            }
                        }
                    }
                    catch
                    {
                        // Ignore schema parsing errors
                    }
                }

                sb.AppendLine();
            }

            sb.AppendLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            sb.AppendLine();
            sb.AppendLine("Remember: When you need to use a tool, use the ```tool code block format shown above!");

            return sb.ToString();
        }

        private List<ToolCall> ExtractToolCallsFromResponse(string response)
        {
            var toolCalls = new List<ToolCall>();

            // Look for tool call blocks in markdown code fence format
            var lines = response.Split('\n');
            bool inToolBlock = false;
            string? currentToolName = null;
            var currentArgsLines = new List<string>();

            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i].Trim();

                if (line == "```tool" || line.StartsWith("```tool"))
                {
                    inToolBlock = true;
                    currentToolName = null;
                    currentArgsLines.Clear();
                    continue;
                }

                if (line == "```" && inToolBlock)
                {
                    // End of tool block
                    if (currentToolName != null)
                    {
                        var argsJson = string.Join("\n", currentArgsLines);
                        object? arguments = null;

                        if (!string.IsNullOrWhiteSpace(argsJson))
                        {
                            try
                            {
                                arguments = JsonSerializer.Deserialize<Dictionary<string, object>>(argsJson);
                            }
                            catch
                            {
                                // If parsing fails, treat as empty arguments
                                arguments = new Dictionary<string, object>();
                            }
                        }

                        toolCalls.Add(new ToolCall { ToolName = currentToolName, Arguments = arguments });
                    }

                    inToolBlock = false;
                    currentToolName = null;
                    currentArgsLines.Clear();
                    continue;
                }

                if (inToolBlock)
                {
                    if (currentToolName == null && !string.IsNullOrWhiteSpace(line))
                    {
                        // First non-empty line is the tool name
                        currentToolName = line;
                    }
                    else if (currentToolName != null)
                    {
                        // Subsequent lines are arguments
                        currentArgsLines.Add(line);
                    }
                }
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