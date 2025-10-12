using System.Diagnostics;
using System.Text;

namespace folderchat.Services
{
    internal class ClaudeCodeChatService : IChatService
    {
        private readonly string _cliPath;
        private readonly string _model;
        private readonly List<string> _conversationHistory;

        public ClaudeCodeChatService(string cliPath, string model)
        {
            _cliPath = cliPath ?? "claude";
            _model = model ?? "sonnet";
            _conversationHistory = new List<string>();
        }

        public async Task<SendMessageAsyncResult> SendMessageAsync(string userInput, string? systemMessage = null)
        {
            if (string.IsNullOrEmpty(userInput))
                throw new ArgumentNullException(nameof(userInput), "Message is empty.");

            // Note: Claude Code doesn't support system messages, so we ignore the systemMessage parameter
            // The context will be included in the user message instead

            // Add user message to history
            _conversationHistory.Add($"User: {userInput}");

            try
            {
                // Build the command
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = _cliPath,
                    Arguments = $"-p \"{EscapeArgument(userInput)}\" --model {_model}",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    StandardOutputEncoding = Encoding.UTF8,
                    StandardErrorEncoding = Encoding.UTF8
                };

                using var process = Process.Start(processStartInfo);
                if (process == null)
                {
                    throw new InvalidOperationException("Failed to start Claude CLI process.");
                }

                // Read the output asynchronously
                var outputBuilder = new StringBuilder();
                var errorBuilder = new StringBuilder();

                var outputTask = Task.Run(async () =>
                {
                    using var reader = process.StandardOutput;
                    var buffer = new char[4096];
                    int read;
                    while ((read = await reader.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        outputBuilder.Append(buffer, 0, read);
                    }
                });

                var errorTask = Task.Run(async () =>
                {
                    using var reader = process.StandardError;
                    var buffer = new char[4096];
                    int read;
                    while ((read = await reader.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        errorBuilder.Append(buffer, 0, read);
                    }
                });

                await Task.WhenAll(outputTask, errorTask);
                await process.WaitForExitAsync();

                if (process.ExitCode != 0)
                {
                    var errorMessage = errorBuilder.ToString();
                    if (!string.IsNullOrEmpty(errorMessage))
                    {
                        throw new Exception($"Claude CLI error: {errorMessage}");
                    }
                    throw new Exception($"Claude CLI exited with code {process.ExitCode}");
                }

                var response = outputBuilder.ToString().Trim();

                // Add assistant response to history
                _conversationHistory.Add($"Assistant: {response}");

                return new SendMessageAsyncResult
                {
                    ActualUserMessage = userInput,
                    AssistantResponse = response
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error executing Claude CLI: {ex.Message}", ex);
            }
        }

        public void ClearHistory()
        {
            _conversationHistory.Clear();
        }

        private static string EscapeArgument(string arg)
        {
            // Escape double quotes and backslashes for command line arguments
            return arg.Replace("\\", "\\\\").Replace("\"", "\\\"");
        }
    }
}