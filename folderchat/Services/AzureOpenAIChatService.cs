using OpenAI;
using OpenAI.Chat;
using System.ClientModel;

namespace folderchat.Services
{
    internal class AzureOpenAIChatService : IChatService
    {
        private readonly ChatClient _chatClient;
        private readonly List<ChatMessage> _conversationHistory;

        public AzureOpenAIChatService(string endpoint, string apiKey, string deploymentName, string apiVersion)
        {
            // Validate required parameters
            if (string.IsNullOrEmpty(endpoint))
                throw new ArgumentException("Endpoint cannot be empty", nameof(endpoint));
            if (string.IsNullOrEmpty(apiKey))
                throw new ArgumentException("API Key cannot be empty", nameof(apiKey));
            if (string.IsNullOrEmpty(deploymentName))
                throw new ArgumentException("Deployment Name cannot be empty", nameof(deploymentName));
            if (string.IsNullOrEmpty(apiVersion))
                throw new ArgumentException("API Version cannot be empty", nameof(apiVersion));

            // Azure OpenAI full endpoint format:
            // https://{resource-name}.openai.azure.com/openai/deployments/{deployment-name}/chat/completions?api-version={api-version}

            var baseEndpoint = endpoint.TrimEnd('/');

            // Build the complete Azure endpoint with deployment and api-version
            // The OpenAI SDK expects the base endpoint, and will append /chat/completions
            var azureEndpoint = $"{baseEndpoint}/openai/deployments/{deploymentName}?api-version={apiVersion}";

            var azureClient = new OpenAIClient(
                new ApiKeyCredential(apiKey),
                new OpenAIClientOptions
                {
                    Endpoint = new Uri(azureEndpoint)
                }
            );

            // For Azure, we still need to pass the deployment name to GetChatClient
            // even though it's in the endpoint
            _chatClient = azureClient.GetChatClient(deploymentName);
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
