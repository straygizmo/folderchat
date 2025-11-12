using System.Text;
using folderchat.Forms;
using folderchat.Exceptions;

namespace folderchat.Services
{
    internal class RagEnabledChatService : IChatService
    {
        private readonly IChatService _innerChatService;
        private readonly IRagService _ragService;
        private readonly MainForm _mainForm;
        private readonly List<string> _conversationHistory;
        private readonly bool _useContextInSystemMessage;
        private static string _lastContext = string.Empty;
        private static List<string> _lastCheckedFolders = new List<string>();

        public static string LastContext => _lastContext;
        public static List<string> LastCheckedFolders => _lastCheckedFolders;

        public RagEnabledChatService(IChatService innerChatService, IRagService ragService, MainForm mainForm, bool useContextInSystemMessage)
        {
            _innerChatService = innerChatService;
            _ragService = ragService;
            _mainForm = mainForm;
            _useContextInSystemMessage = useContextInSystemMessage;
            _conversationHistory = new List<string>();
        }

        public async Task<SendMessageAsyncResult> SendMessageAsync(string userInput, string? systemMessage = null)
        {
            if (string.IsNullOrEmpty(userInput))
                throw new ArgumentNullException(nameof(userInput), "Message is empty.");

            // Get checked folders from MainForm
            var checkedFolders = _mainForm.GetCheckedFolders();

            if (checkedFolders.Count == 0)
            {
                // No folders selected for RAG, just pass through to inner service
                _lastCheckedFolders = new List<string>();

                // Log system message if present
                if (!string.IsNullOrEmpty(systemMessage))
                {
                    _mainForm?.LogChatMessage("system", systemMessage);
                }

                // Log user input
                _mainForm?.LogChatMessage("user", userInput);

                var noRagResult = await _innerChatService.SendMessageAsync(userInput, systemMessage);

                // Log assistant response
                _mainForm?.LogChatMessage("assistant", noRagResult.AssistantResponse);

                return noRagResult;
            }

            // Check if folder selection has changed
            bool foldersChanged = !checkedFolders.SequenceEqual(_lastCheckedFolders);

            // Only check for missing embeddings if folders have changed
            var foldersNeedingIndex = new List<string>();
            if (foldersChanged)
            {
                foreach (var folder in checkedFolders)
                {
                    var embeddingsFile = Path.Combine(folder, RagService.EmbeddingsFileName);
                    if (!File.Exists(embeddingsFile))
                    {
                        foldersNeedingIndex.Add(folder);
                    }
                }

                // Index folders if needed
                if (foldersNeedingIndex.Count > 0)
                {
                    var progress = new Progress<string>(status =>
                    {
                        Console.WriteLine($"Indexing: {status}");
                        _mainForm?.LogRAGMessage(status);
                    });

                    await _mainForm.Indexing();
                }

                // Update last checked folders after indexing
                _lastCheckedFolders = new List<string>(checkedFolders);
            }

            // Get RAG settings
            var topK = Properties.Settings.Default.RAG_TopKChunks;
            var maxContextLength = Properties.Settings.Default.RAG_MaxContextLength;

            // Search for relevant chunks with error handling
            List<Models.SearchResult> relevantChunks = new List<Models.SearchResult>();
            try
            {
                relevantChunks = await _ragService.SearchRelevantChunksAsync(
                    userInput,
                    checkedFolders,
                    topK,
                    maxContextLength);
            }
            catch (VectorizationException vex)
            {
                // Return the formatted error message as an AI response
                var errorMessage = vex.GetFormattedErrorMessage();
                return new SendMessageAsyncResult
                {
                    ActualUserMessage = userInput,
                    AssistantResponse = errorMessage
                };
            }
            catch (Exception ex)
            {
                // Handle unexpected errors
                var errorMessage = $"[Error]\n\nAn unexpected error occurred while processing your message with RAG.\n\n" +
                       $"Error: {ex.Message}\n\n" +
                       "Suggested actions:\n" +
                       "• Check your embedding service configuration in Settings\n" +
                       "• Try disabling RAG by unchecking folders in the Dir to RAG tab\n" +
                       "• Contact support if the issue persists";
                return new SendMessageAsyncResult
                {
                    ActualUserMessage = userInput,
                    AssistantResponse = errorMessage
                };
            }

            // Build enhanced message with context
            string messageToSend;
            string? contextAsSystemMessage = null;
            string actualUserMessage; // This is what will be shown in UI

            if (_useContextInSystemMessage && relevantChunks.Count > 0)
            {
                // Build system message with context
                contextAsSystemMessage = BuildSystemMessageWithContext(relevantChunks);
                messageToSend = userInput;
                actualUserMessage = userInput; // For providers that use system messages, user message stays as is

                // Store the context for display
                _lastContext = contextAsSystemMessage;

                // Combine with existing system message if present
                if (!string.IsNullOrEmpty(systemMessage))
                {
                    contextAsSystemMessage = $"{systemMessage}\n\n{contextAsSystemMessage}";
                }

                // Log system message with context, then user input
                _mainForm?.LogChatMessage("system", contextAsSystemMessage);
                _mainForm?.LogChatMessage("user", userInput);
            }
            else
            {
                // Build enhanced message with context included in user message
                messageToSend = BuildEnhancedMessage(userInput, relevantChunks!);
                actualUserMessage = messageToSend; // For providers without system messages, this includes context

                // Store the context for display
                _lastContext = ExtractContextFromEnhancedMessage(messageToSend, userInput);

                // For providers that don't support system messages, only log the combined user message
                // (no separate system message log)
                _mainForm?.LogChatMessage("user", messageToSend);
            }

            // Send message to inner chat service with optional system message
            var serviceResult = await _innerChatService.SendMessageAsync(messageToSend, contextAsSystemMessage ?? systemMessage);

            // Log assistant response
            _mainForm?.LogChatMessage("assistant", serviceResult.AssistantResponse);

            // Add to conversation history
            _conversationHistory.Add($"User: {userInput}");
            _conversationHistory.Add($"Assistant: {serviceResult.AssistantResponse}");

            return new SendMessageAsyncResult
            {
                ActualUserMessage = actualUserMessage,
                AssistantResponse = serviceResult.AssistantResponse
            };
        }

        private string BuildEnhancedMessage(string userInput, List<Models.SearchResult> relevantChunks)
        {
            if (relevantChunks == null || relevantChunks.Count == 0)
            {
                return userInput;
            }

            var messageBuilder = new StringBuilder();

            // Add user message
            messageBuilder.AppendLine("[User Message]");
            messageBuilder.AppendLine(userInput);

            // Add context header
            messageBuilder.AppendLine();
            messageBuilder.AppendLine("---");
            messageBuilder.AppendLine("Please use the following context to answer the user's message:");

            // Add relevant chunks
            foreach (var chunk in relevantChunks)
            {
                var folderName = Path.GetFileName(chunk.FolderPath);
                messageBuilder.AppendLine($"[Context Source: {folderName}/{chunk.FilePath}]");
                messageBuilder.AppendLine(chunk.Text);
                messageBuilder.AppendLine();
                messageBuilder.AppendLine("---");
                messageBuilder.AppendLine();
            }

            return messageBuilder.ToString();
        }

        public void ClearHistory()
        {
            _conversationHistory.Clear();
            _innerChatService.ClearHistory();
        }

        private string BuildSystemMessageWithContext(List<Models.SearchResult> relevantChunks)
        {
            var messageBuilder = new StringBuilder();
            messageBuilder.AppendLine("Please use the following context to answer the user's message:");
            messageBuilder.AppendLine();

            // Add relevant chunks
            foreach (var chunk in relevantChunks)
            {
                var folderName = Path.GetFileName(chunk.FolderPath);
                messageBuilder.AppendLine($"[Context Source: {folderName}/{chunk.FilePath}]");
                messageBuilder.AppendLine(chunk.Text);
                messageBuilder.AppendLine();
                messageBuilder.AppendLine("---");
                messageBuilder.AppendLine();
            }

            return messageBuilder.ToString().Trim();
        }

        private string ExtractContextFromEnhancedMessage(string enhancedMessage, string userInput)
        {
            // If no context was added, return empty
            if (enhancedMessage == userInput)
            {
                return string.Empty;
            }

            // Extract the context portion (everything before "[User Message]")
            var userMessageIndex = enhancedMessage.IndexOf("[User Message]");
            if (userMessageIndex > 0)
            {
                return enhancedMessage.Substring(0, userMessageIndex).Trim();
            }

            return string.Empty;
        }
    }
}