using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace folderchat.Services
{
    internal class OpenRouterChatService : IChatService
    {
        private readonly HttpClient _httpClient;
        private readonly string _model;
        private readonly string _apiEndpoint;
        private readonly List<ChatMessage> _conversationHistory;

        public OpenRouterChatService(string apiUrl, string apiKey, string model)
        {
            // Validate required parameters
            if (string.IsNullOrEmpty(apiKey))
                throw new ArgumentException("API Key cannot be empty", nameof(apiKey));
            if (string.IsNullOrEmpty(model))
                throw new ArgumentException("Model cannot be empty", nameof(model));

            if (string.IsNullOrEmpty(apiUrl))
                apiUrl = "https://openrouter.ai/api/v1";

            _model = model;

            // Ensure the apiUrl ends with /v1
            if (!apiUrl.EndsWith("/v1"))
            {
                apiUrl = apiUrl.TrimEnd('/') + "/v1";
            }

            // Build the full endpoint URL
            _apiEndpoint = $"{apiUrl}/chat/completions";

            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
            _httpClient.DefaultRequestHeaders.Add("HTTP-Referer", "https://github.com/folderchat-app/folderchat");
            _httpClient.DefaultRequestHeaders.Add("X-Title", "folderchat");
            _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
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
                messages.Add(new ChatMessage { Role = "system", Content = systemMessage });

                // Add existing conversation history (excluding any previous system messages)
                foreach (var msg in _conversationHistory)
                {
                    if (msg.Role != "system")
                    {
                        messages.Add(msg);
                    }
                }

                // Add the new user message
                messages.Add(new ChatMessage { Role = "user", Content = userInput });
            }
            else
            {
                // Add user message to history
                _conversationHistory.Add(new ChatMessage { Role = "user", Content = userInput });
                messages = _conversationHistory;
            }

            // Create request
            var request = new ChatRequest
            {
                Model = _model,
                Messages = messages
            };

            // Debug: Log request
            var requestJson = JsonSerializer.Serialize(request, new JsonSerializerOptions { WriteIndented = true });
            System.Diagnostics.Debug.WriteLine($"OpenRouter Request URL: {_apiEndpoint}");
            System.Diagnostics.Debug.WriteLine($"OpenRouter Request Body: {requestJson}");

            // Send request using absolute URL
            var response = await _httpClient.PostAsJsonAsync(_apiEndpoint, request);

            // Read response content for debugging
            var responseContent = await response.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine($"OpenRouter Response Status: {response.StatusCode}");
            System.Diagnostics.Debug.WriteLine($"OpenRouter Response Content: {responseContent}");

            // Save response to temp file for debugging
            if (responseContent.Length > 1000)
            {
                var tempFile = Path.Combine(Path.GetTempPath(), $"openrouter_response_{DateTime.Now:yyyyMMdd_HHmmss}.txt");
                await File.WriteAllTextAsync(tempFile, responseContent);
                System.Diagnostics.Debug.WriteLine($"Large response saved to: {tempFile}");
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"OpenRouter API error (Status: {response.StatusCode}): {responseContent}");
            }

            ChatResponse? result;
            try
            {
                if (string.IsNullOrEmpty(responseContent))
                {
                    throw new Exception($"OpenRouter returned empty response. Status: {response.StatusCode}, Headers: {string.Join(", ", response.Headers.Select(h => $"{h.Key}={string.Join(",", h.Value)}"))}");
                }
                result = JsonSerializer.Deserialize<ChatResponse>(responseContent);
            }
            catch (JsonException ex)
            {
                var preview = responseContent.Substring(0, Math.Min(500, responseContent.Length));
                // Check if it's HTML
                if (responseContent.TrimStart().StartsWith("<"))
                {
                    throw new Exception($"OpenRouter returned HTML instead of JSON (Status: {response.StatusCode}, Length: {responseContent.Length}). This usually means the endpoint is wrong. Preview: {preview}", ex);
                }
                throw new Exception($"Failed to parse OpenRouter response. Status: {response.StatusCode}, Content length: {responseContent.Length}, Preview: {preview}", ex);
            }

            if (result == null || result.Choices == null || result.Choices.Length == 0)
                throw new Exception($"Invalid response from OpenRouter API: {responseContent.Substring(0, Math.Min(500, responseContent.Length))}");

            var assistantMessage = result.Choices[0].Message.Content ?? "";

            // Update the conversation history
            if (!string.IsNullOrEmpty(systemMessage))
            {
                // Store system message in history if it was provided
                _conversationHistory.Clear();
                _conversationHistory.Add(new ChatMessage { Role = "system", Content = systemMessage });

                // Re-add all messages except the initial system message
                foreach (var msg in messages.Skip(1))
                {
                    _conversationHistory.Add(msg);
                }
            }

            // Add assistant response to history
            _conversationHistory.Add(new ChatMessage { Role = "assistant", Content = assistantMessage });

            // Return response
            return new SendMessageAsyncResult
            {
                ActualUserMessage = userInput,
                AssistantResponse = assistantMessage
            };
        }

        public void ClearHistory()
        {
            _conversationHistory.Clear();
        }

        private class ChatMessage
        {
            [JsonPropertyName("role")]
            public string Role { get; set; } = "";

            [JsonPropertyName("content")]
            public string Content { get; set; } = "";
        }

        private class ChatRequest
        {
            [JsonPropertyName("model")]
            public string Model { get; set; } = "";

            [JsonPropertyName("messages")]
            public List<ChatMessage> Messages { get; set; } = new();
        }

        private class ChatResponse
        {
            [JsonPropertyName("choices")]
            public Choice[]? Choices { get; set; }
        }

        private class Choice
        {
            [JsonPropertyName("message")]
            public ChatMessage Message { get; set; } = new();
        }
    }
}
