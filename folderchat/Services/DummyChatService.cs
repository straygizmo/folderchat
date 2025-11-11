using System.Threading.Tasks;
using folderchat.Models; // For MessageType
using folderchat.Services; // For SendMessageAsyncResult

namespace folderchat.Services
{
    public class DummyChatService : IChatService
    {
        private readonly string _errorMessage;

        public DummyChatService(string errorMessage)
        {
            _errorMessage = errorMessage;
        }

        public Task<SendMessageAsyncResult> SendMessageAsync(string message, string? systemMessage = null)
        {
            var result = new SendMessageAsyncResult
            {
                ActualUserMessage = message,
                AssistantResponse = _errorMessage
            };
            return Task.FromResult(result);
        }

        public void ClearHistory()
        {
            // Dummy service does not maintain history, so nothing to clear.
        }
    }
}
