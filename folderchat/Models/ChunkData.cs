namespace folderchat.Models
{
    public class ChunkData
    {
        public string FilePath { get; set; } = string.Empty;
        public string ChunkText { get; set; } = string.Empty;
        public float[] Embedding { get; set; } = Array.Empty<float>();
        public int ChunkIndex { get; set; }
        public string MarkdownPath { get; set; } = string.Empty;
    }
}