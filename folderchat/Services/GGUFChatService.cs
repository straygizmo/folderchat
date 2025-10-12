using LLama;
using LLama.Common;
using LLama.Native;

namespace folderchat.Services
{
    internal class GGUFChatService : IChatService
    {
        private readonly LLamaWeights _model;
        private readonly LLamaContext _context;
        private readonly InteractiveExecutor _executor;
        private readonly List<string> _conversationHistory;
        private readonly InferenceParams _inferenceParams;

        public GGUFChatService(string modelPath)
        {
            if (string.IsNullOrEmpty(modelPath) || !File.Exists(modelPath))
            {
                throw new FileNotFoundException($"GGUF model not found at: {modelPath}");
            }

            _conversationHistory = new List<string>();

            try
            {
                // Model parameters
                var parameters = new ModelParams(modelPath)
                {
                    ContextSize = 2048,
                    GpuLayerCount = 0 // CPU only, change to higher value for GPU offloading
                };

                // Load model
                _model = LLamaWeights.LoadFromFile(parameters);
                _context = _model.CreateContext(parameters);
                _executor = new InteractiveExecutor(_context);

                // Inference parameters
                _inferenceParams = new InferenceParams()
                {
                    MaxTokens = 2048,
                    AntiPrompts = new List<string> { "User:" }
                };
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Failed to load GGUF model from: {modelPath}\n" +
                    $"File size: {new FileInfo(modelPath).Length / 1024 / 1024} MB\n" +
                    $"Error: {ex.Message}\n\n" +
                    $"Possible solutions:\n" +
                    $"1. Make sure the model file is a valid GGUF format\n" +
                    $"2. Ensure LLamaSharp.Backend.Cpu is properly installed\n" +
                    $"3. Try a different GGUF model\n" +
                    $"4. Check if you have enough RAM to load the model",
                    ex);
            }
        }

        public async Task<SendMessageAsyncResult> SendMessageAsync(string userInput, string? systemMessage = null)
        {
            if (string.IsNullOrEmpty(userInput))
                throw new ArgumentNullException(nameof(userInput), "Message is empty.");

            // Build prompt with conversation history
            var promptBuilder = new System.Text.StringBuilder();

            // Add system message if provided
            if (!string.IsNullOrEmpty(systemMessage))
            {
                promptBuilder.AppendLine($"System: {systemMessage}");
                promptBuilder.AppendLine();
            }

            // Add conversation history
            foreach (var msg in _conversationHistory)
            {
                promptBuilder.AppendLine(msg);
            }

            // Add current user message
            promptBuilder.AppendLine($"User: {userInput}");
            promptBuilder.Append("Assistant:");

            var prompt = promptBuilder.ToString();

            // Generate response
            var responseBuilder = new System.Text.StringBuilder();
            await foreach (var text in _executor.InferAsync(prompt, _inferenceParams))
            {
                responseBuilder.Append(text);
            }

            var response = responseBuilder.ToString().Trim();

            // Update conversation history
            _conversationHistory.Add($"User: {userInput}");
            _conversationHistory.Add($"Assistant: {response}");

            // Keep history manageable (last 10 exchanges)
            if (_conversationHistory.Count > 20)
            {
                _conversationHistory.RemoveRange(0, _conversationHistory.Count - 20);
            }

            return new SendMessageAsyncResult
            {
                ActualUserMessage = userInput,
                AssistantResponse = response
            };
        }

        public void ClearHistory()
        {
            _conversationHistory.Clear();
        }

        public void Dispose()
        {
            _context?.Dispose();
            _model?.Dispose();
        }
    }
}
