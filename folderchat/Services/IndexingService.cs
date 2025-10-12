using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace folderchat.Services
{
    /// <summary>
    /// Service class responsible for indexing folders for RAG processing
    /// </summary>
    public class IndexingService
    {
        private readonly Action<string> _logMessage;
        private readonly Action<string> _logError;
        private readonly Action<string> _updateStatus;
        private readonly Func<bool> _getEnabled;
        private readonly Action<bool> _setEnabled;

        /// <summary>
        /// Initializes a new instance of the IndexingService class
        /// </summary>
        /// <param name="logMessage">Action to log RAG messages</param>
        /// <param name="logError">Action to log error messages</param>
        /// <param name="updateStatus">Action to update status text</param>
        /// <param name="getEnabled">Function to get the enabled state of the indexing button</param>
        /// <param name="setEnabled">Action to set the enabled state of the indexing button</param>
        public IndexingService(
            Action<string> logMessage,
            Action<string> logError,
            Action<string> updateStatus,
            Func<bool> getEnabled,
            Action<bool> setEnabled)
        {
            _logMessage = logMessage ?? throw new ArgumentNullException(nameof(logMessage));
            _logError = logError ?? throw new ArgumentNullException(nameof(logError));
            _updateStatus = updateStatus ?? throw new ArgumentNullException(nameof(updateStatus));
            _getEnabled = getEnabled ?? throw new ArgumentNullException(nameof(getEnabled));
            _setEnabled = setEnabled ?? throw new ArgumentNullException(nameof(setEnabled));
        }

        /// <summary>
        /// Performs indexing on the provided list of folder paths
        /// </summary>
        /// <param name="checkedFolders">List of folder paths to index</param>
        /// <returns>Task representing the asynchronous operation</returns>
        public async Task IndexAsync(List<string> checkedFolders)
        {
            if (checkedFolders.Count == 0)
            {
                _logError("No folders selected for indexing");
                MessageBox.Show("Please select at least one folder to process.", "No Folders Selected",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _logMessage($"Starting indexing for {checkedFolders.Count} folder(s)");
            _setEnabled(false);
            _updateStatus("Processing...");

            try
            {
                var embeddingUrl = Properties.Settings.Default.Embedding_Url;
                var apiKey = Properties.Settings.Default.OpenAI_ApiKey;
                var embeddingModel = Properties.Settings.Default.Embedding_Model;
                var contextLength = Properties.Settings.Default.RAG_ContextLength;
                var chunkSize = Properties.Settings.Default.RAG_ChunkSize;
                var chunkOverlap = Properties.Settings.Default.RAG_ChunkOverlap;
                var useNativeEmbedding = Properties.Settings.Default.UseNativeEmbedding;
                var ggufModel = Properties.Settings.Default.EmbeddingGGUFModel;

                // Validation based on embedding method
                if (useNativeEmbedding)
                {
                    if (string.IsNullOrEmpty(ggufModel))
                    {
                        MessageBox.Show("Please select a GGUF model in the Settings tab.",
                            "Configuration Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(embeddingModel))
                    {
                        MessageBox.Show("Please configure Embedding settings in the Settings tab.",
                            "Configuration Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                var ragService = new Services.RagService(embeddingUrl, apiKey, embeddingModel, useNativeEmbedding, contextLength, chunkSize, chunkOverlap, ggufModel);
                var progress = new Progress<string>(status =>
                {
                    _updateStatus(status);
                    _logMessage(status);
                    Application.DoEvents();
                });

                await ragService.ProcessFoldersAsync(checkedFolders, progress);

                _logMessage("Indexing completed successfully");
                _updateStatus("Processing complete!");
            }
            catch (Exception ex)
            {
                _logError($"RAG processing error: {ex.Message}");
                _updateStatus("Error occurred");
                var detailedError = $"Error during RAG processing:\n\n{ex.Message}\n\nStack Trace:\n{ex.StackTrace}";
                if (ex.InnerException != null)
                {
                    detailedError += $"\n\nInner Exception:\n{ex.InnerException.Message}";
                }
                MessageBox.Show(detailedError, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _setEnabled(true);
            }
        }
    }
}
