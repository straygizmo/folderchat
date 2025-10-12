using OpenAI;
using OpenAI.Chat;
using System.ClientModel;

namespace folderchat.Services
{
    internal class OpenAIChatService : IChatService
    {
        private readonly ChatClient _chatClient;
        private readonly List<ChatMessage> _conversationHistory;

        public OpenAIChatService(string baseUrl, string apiKey, string model)
        {
            // Validate required parameters
            // Note: apiKey can be empty for Ollama or other local APIs
            if (string.IsNullOrEmpty(model))
                throw new ArgumentException("Model cannot be empty", nameof(model));

            OpenAIClient openAIClient;

            if (string.IsNullOrEmpty(baseUrl) || baseUrl == "https://api.openai.com/v1")
            {
                // For official OpenAI API, API key is required
                if (string.IsNullOrEmpty(apiKey))
                    throw new ArgumentException("API Key cannot be empty for OpenAI API", nameof(apiKey));
                openAIClient = new OpenAIClient(new ApiKeyCredential(apiKey));
            }
            else
            {
                // For custom endpoints (like Ollama), API key may be optional
                var credential = string.IsNullOrEmpty(apiKey) ? new ApiKeyCredential("not-needed") : new ApiKeyCredential(apiKey);
                openAIClient = new OpenAIClient(credential, new OpenAIClientOptions
                {
                    Endpoint = new Uri(baseUrl)
                });
            }

            _chatClient = openAIClient.GetChatClient(model);
            _conversationHistory = new List<ChatMessage>();
        }

        public async Task<SendMessageAsyncResult> SendMessageAsync(string userInput, string? systemMessage = null)
        {
            if (string.IsNullOrEmpty(userInput))
                throw new ArgumentNullException(nameof(userInput), "Message is empty.");

            // Create temporary conversation for this request if system message is provided
            List<ChatMessage> messages;
            if (!string.IsNullOrEmpty(systemMessage))
            {
                // Use a temporary list with system message
                messages = new List<ChatMessage>();
                messages.Add(new SystemChatMessage(systemMessage));

                // Add existing conversation history (excluding any previous system messages)
                foreach (var msg in _conversationHistory)
                {
                    if (msg is not SystemChatMessage)
                    {
                        messages.Add(msg);
                    }
                }

                // Add the new user message
                messages.Add(new UserChatMessage(userInput));
            }
            else
            {
                // Add user message to history
                _conversationHistory.Add(new UserChatMessage(userInput));
                messages = _conversationHistory;
            }

            // Chat completion request
            ChatCompletion completion = await _chatClient.CompleteChatAsync(messages);

            // Update the conversation history
            if (!string.IsNullOrEmpty(systemMessage))
            {
                // Store system message in history if it was provided
                _conversationHistory.Clear();
                _conversationHistory.Add(new SystemChatMessage(systemMessage));

                // Re-add all messages except the initial system message
                foreach (var msg in messages.Skip(1))
                {
                    _conversationHistory.Add(msg);
                }
            }

            // Add assistant response to history
            _conversationHistory.Add(new AssistantChatMessage(completion));

            // Return response
            return new SendMessageAsyncResult
            {
                ActualUserMessage = userInput,
                AssistantResponse = completion.Content[0].Text
            };
        }

        public void ClearHistory()
        {
            _conversationHistory.Clear();
        }
    }
}
