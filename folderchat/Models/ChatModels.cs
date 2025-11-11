namespace folderchat.Models
{
    public enum MessageType
    {
        Normal,
        Error,
        Info,
        User,       // Added for API
        Assistant,   // Added for API
        Pending      // Pending bubble shown in chat for temporary display
    }

    public class ChatMessageModel
    {
        public string Text { get; set; } = string.Empty;
        public bool IsUser { get; set; }
        public MessageType Type { get; set; } = MessageType.Normal;
    }
}
