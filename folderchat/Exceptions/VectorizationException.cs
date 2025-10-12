using System;

namespace folderchat.Exceptions
{
    public class VectorizationException : Exception
    {
        public string ErrorType { get; set; }
        public string SuggestedActions { get; set; }

        public VectorizationException(string message) : base(message)
        {
            ErrorType = "General Vectorization Error";
            SuggestedActions = "Check your embedding service configuration.";
        }

        public VectorizationException(string message, Exception innerException) : base(message, innerException)
        {
            ErrorType = DetermineErrorType(innerException);
            SuggestedActions = GenerateSuggestedActions(ErrorType, innerException);
        }

        public VectorizationException(string message, string errorType, string suggestedActions) : base(message)
        {
            ErrorType = errorType;
            SuggestedActions = suggestedActions;
        }

        private static string DetermineErrorType(Exception ex)
        {
            if (ex.Message.Contains("401") || ex.Message.Contains("Unauthorized") || ex.Message.Contains("API key"))
            {
                return "Authentication Error";
            }
            else if (ex.Message.Contains("404") || ex.Message.Contains("Not Found"))
            {
                return "Endpoint Not Found";
            }
            else if (ex.Message.Contains("429") || ex.Message.Contains("rate limit"))
            {
                return "Rate Limit Exceeded";
            }
            else if (ex.Message.Contains("timeout", StringComparison.OrdinalIgnoreCase) || ex is TimeoutException)
            {
                return "Timeout Error";
            }
            else if (ex.Message.Contains("network", StringComparison.OrdinalIgnoreCase) || ex is System.Net.Http.HttpRequestException)
            {
                return "Network Error";
            }
            else if (ex.Message.Contains("model", StringComparison.OrdinalIgnoreCase))
            {
                return "Model Error";
            }
            else
            {
                return "Service Error";
            }
        }

        private static string GenerateSuggestedActions(string errorType, Exception ex)
        {
            switch (errorType)
            {
                case "Authentication Error":
                    return "• Check that your API key is correct in Settings > Embedding Settings\n" +
                           "• Ensure your API key has the necessary permissions\n" +
                           "• Verify the API key hasn't expired";

                case "Endpoint Not Found":
                    return "• Verify the embedding service URL is correct\n" +
                           "• Check if the model name is valid\n" +
                           "• Ensure the service is running and accessible";

                case "Rate Limit Exceeded":
                    return "• Wait a moment and try again\n" +
                           "• Consider reducing the frequency of requests\n" +
                           "• Check your API plan limits";

                case "Timeout Error":
                    return "• Check your internet connection\n" +
                           "• The embedding service may be slow or overloaded\n" +
                           "• Try again in a few moments";

                case "Network Error":
                    return "• Check your internet connection\n" +
                           "• Verify firewall settings allow the connection\n" +
                           "• Ensure the embedding service is accessible from your network";

                case "Model Error":
                    return "• Verify the embedding model name is correct\n" +
                           "• Check if the model is available in your region\n" +
                           "• Ensure the model supports the requested operation";

                default:
                    return "• Check the embedding service configuration in Settings\n" +
                           "• Verify the service is operational\n" +
                           $"• Error details: {ex.Message}";
            }
        }

        public string GetFormattedErrorMessage()
        {
            return $"[Vectorization Error: {ErrorType}]\n\n" +
                   $"Failed to generate embeddings for your message.\n\n" +
                   $"Error: {Message}\n\n" +
                   $"Suggested actions:\n{SuggestedActions}";
        }
    }
}