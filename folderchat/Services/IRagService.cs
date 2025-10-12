using folderchat.Models;

namespace folderchat.Services
{
    public interface IRagService
    {
        Task ProcessFoldersAsync(List<string> folderPaths, IProgress<string> progress);
        Task<List<SearchResult>> SearchRelevantChunksAsync(string query, List<string> folderPaths, int topK, int maxContextLength);
        Task<float[]> GenerateEmbeddingAsync(string text);
    }
}