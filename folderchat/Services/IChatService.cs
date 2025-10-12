namespace folderchat.Services
{
    /// <summary>
    /// Result of sending a message to the chat service
    /// </summary>
    public class SendMessageAsyncResult
    {
        /// <summary>
        /// The actual user message sent to the LLM (may include RAG context)
        /// </summary>
        public string ActualUserMessage { get; set; } = string.Empty;

        /// <summary>
        /// The assistant's response
        /// </summary>
        public string AssistantResponse { get; set; } = string.Empty;
    }

    public interface IChatService
    {
        Task<SendMessageAsyncResult> SendMessageAsync(string userInput, string? systemMessage = null);
        void ClearHistory();
    }
}
