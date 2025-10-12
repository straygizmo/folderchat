namespace folderchat.Models
{
    public class SearchResult
    {
        public string Text { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public int ChunkIndex { get; set; }
        public float Similarity { get; set; }
        public string FolderPath { get; set; } = string.Empty;
    }
}