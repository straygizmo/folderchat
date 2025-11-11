using System.Threading.Tasks;
using folderchat.Models;

namespace folderchat.Services
{
    public class DummyChatService : IChatService
    {
        private readonly string _errorMessage;

        public DummyChatService(string errorMessage)
        {
            _errorMessage = errorMessage;
        }

        public Task<ChatResult> SendMessageAsync(string message, string? systemMessage = null)
        {
            var result = new ChatResult
            {
                ActualUserMessage = message,
                AssistantResponse = _errorMessage,
                FunctionCalls = null
            };
            return Task.FromResult(result);
        }
    }
}
