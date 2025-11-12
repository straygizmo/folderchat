using OpenAI;
using OpenAI.Embeddings;
using System.ClientModel;
using System.Text;
using System.Text.Json;
using System.Diagnostics;
using folderchat.Models;
using folderchat.Exceptions;

namespace folderchat.Services
{
    public class RagService : IRagService
    {
        private readonly PythonInteropService _pythonService;
        private readonly EmbeddingClient? _embeddingClient;
        private readonly bool _useNativeEmbedding;
        private readonly string _embeddingUrl;
        private readonly string _apiKey;
        private readonly string _embeddingModel;
        private readonly string _ggufModel;
        private readonly int _contextLength;
        private readonly int _chunkSize;
        private readonly int _chunkOverlap;
        private readonly string _pythonExecutable;
        private readonly string _ragIndexerScript;
        private readonly string[] _supportedExtensions = { ".txt", ".md", ".py", ".js", ".ts", ".jsx", ".tsx",
                                                           ".json", ".yaml", ".yml", ".toml", ".ini", ".cfg",
                                                           ".html", ".css", ".xml", ".csv", ".sql", ".sh",
                                                           ".bat", ".ps1", ".c", ".cpp", ".h", ".java", ".cs",
                                                           ".go", ".rs", ".rb", ".php", ".swift", ".kt", ".scala",
                                                           ".pdf", ".docx", ".xlsx", ".pptx", ".doc", ".xls", ".ppt" };

        internal const string EmbeddingsFileName = ".embeddings.jsonl";

        public RagService(string embeddingUrl, string apiKey, string embeddingModel, bool useNativeEmbedding = false, int contextLength = 2048, int chunkSize = 500, int chunkOverlap = 100, string ggufModel = "")
        {
            _pythonService = new PythonInteropService();
            _embeddingUrl = embeddingUrl;
            _apiKey = apiKey;
            _embeddingModel = embeddingModel;
            _ggufModel = ggufModel;
            _useNativeEmbedding = useNativeEmbedding;
            _contextLength = contextLength;
            _chunkSize = Math.Min(chunkSize, contextLength);
            _chunkOverlap = Math.Min(chunkOverlap, _chunkSize);

            // Initialize OpenAI-compatible embedding client if native embedding is NOT enabled
            if (!_useNativeEmbedding)
            {
                OpenAIClient openAIClient;
                if (string.IsNullOrEmpty(embeddingUrl) || embeddingUrl == "https://api.openai.com/v1")
                {
                    openAIClient = new OpenAIClient(new ApiKeyCredential(apiKey));
                }
                else
                {
                    openAIClient = new OpenAIClient(new ApiKeyCredential(apiKey), new OpenAIClientOptions
                    {
                        Endpoint = new Uri(embeddingUrl)
                    });
                }
                _embeddingClient = openAIClient.GetEmbeddingClient(embeddingModel);
            }

            // Initialize Python tools path using PythonPathHelper
            _pythonExecutable = PythonPathHelper.PythonExecutable;
            _ragIndexerScript = PythonPathHelper.GetScriptPath("rag_indexer.py");

            // PythonPathHelper.GetScriptPath already validates file existence
        }

        public async Task ProcessFoldersAsync(List<string> folderPaths, IProgress<string> progress)
        {
            // All indexing logic is now handled by the Python script
            await ProcessFoldersPythonAsync(folderPaths, progress);
        }

        private async Task ProcessFoldersPythonAsync(List<string> folderPaths, IProgress<string> progress)
        {
            var tempConfigFile = Path.Combine(Path.GetTempPath(), $"rag_config_{Guid.NewGuid()}.json");
            
            try
            {
                var config = new
                {
                    folders = folderPaths,
                    embedding_url = _embeddingUrl,
                    embedding_model = _embeddingModel,
                    context_length = _contextLength,
                    chunk_size = _chunkSize,
                    chunk_overlap = _chunkOverlap,
                    api_key = _apiKey,
                    use_native_embedding = _useNativeEmbedding,
                    gguf_model = _ggufModel
                };
                
                var configJson = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(tempConfigFile, configJson);
                
                progress.Report($"DEBUG: Python executable: {_pythonExecutable}");
                progress.Report($"DEBUG: Script: {_ragIndexerScript}");
                progress.Report($"DEBUG: Config file: {tempConfigFile}");
                progress.Report($"DEBUG: Config: {configJson}");
                
                if (!File.Exists(_pythonExecutable))
                {
                    throw new Exception($"Python executable not found at: {_pythonExecutable}");
                }
                
                if (!File.Exists(_ragIndexerScript))
                {
                    throw new Exception($"RAG indexer script not found at: {_ragIndexerScript}");
                }

                if (!Directory.Exists(PythonPathHelper.PythonToolsDirectory))
                {
                    throw new Exception($"Working directory for Python script not found at: {PythonPathHelper.PythonToolsDirectory}");
                }
                
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = _pythonExecutable,
                    Arguments = $"\"{_ragIndexerScript}\" \"{tempConfigFile}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = PythonPathHelper.PythonToolsDirectory
                };

                using var process = new Process { StartInfo = processStartInfo };

                progress.Report($"DEBUG: Starting process...");
                process.Start();

                var outputLines = new List<string>();
                var errorLines = new List<string>();

                var outputTask = Task.Run(async () =>
                {
                    string line;
                    while ((line = await process.StandardOutput.ReadLineAsync()) != null)
                    {
                        if (!string.IsNullOrEmpty(line))
                        {
                            outputLines.Add(line);
                            progress.Report(line);
                        }
                    }
                });

                var errorTask = Task.Run(async () =>
                {
                    string line;
                    while ((line = await process.StandardError.ReadLineAsync()) != null)
                    {
                        if (!string.IsNullOrEmpty(line))
                        {
                            errorLines.Add(line);
                            progress.Report($"ERROR: {line}");
                        }
                    }
                });

                await Task.WhenAll(outputTask, errorTask);
                await process.WaitForExitAsync();

                progress.Report($"DEBUG: Process exit code: {process.ExitCode}");

                if (process.ExitCode != 0)
                {
                    var errorDetail = errorLines.Count > 0 
                        ? string.Join("\n", errorLines) 
                        : "No error output captured";
                    throw new Exception($"Python RAG indexing failed with exit code {process.ExitCode}\nError details:\n{errorDetail}");
                }
            }
            catch (Exception ex)
            {
                var errorMsg = $"Failed to run Python RAG indexer: {ex.Message}\n" +
                              $"Python: {_pythonExecutable}\n" +
                              $"Script: {_ragIndexerScript}\n" +
                              $"Config: {tempConfigFile}";
                throw new Exception(errorMsg, ex);
            }
            finally
            {
                if (File.Exists(tempConfigFile))
                {
                    try
                    {
                        File.Delete(tempConfigFile);
                    }
                    catch
                    {
                    }
                }
            }
        }

        public async Task<List<SearchResult>> SearchRelevantChunksAsync(string query, List<string> folderPaths, int topK, int maxContextLength)
        {
            var allResults = new List<SearchResult>();

            // Generate embedding for the query - this will throw VectorizationException if it fails
            var queryEmbedding = await GenerateEmbeddingAsync(query);

            foreach (var folderPath in folderPaths)
            {
                var embeddingsFile = Path.Combine(folderPath, EmbeddingsFileName);
                if (!File.Exists(embeddingsFile))
                    continue;

                var lines = await File.ReadAllLinesAsync(embeddingsFile);
                foreach (var line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    try
                    {
                        var chunk = JsonSerializer.Deserialize<Dictionary<string, object>>(line);
                        if (chunk == null)
                            continue;

                        // Parse the embedding
                        float[] chunkEmbedding;
                        if (chunk.ContainsKey("Embedding") && chunk["Embedding"] is JsonElement embeddingElement)
                        {
                            chunkEmbedding = JsonSerializer.Deserialize<float[]>(embeddingElement.GetRawText()) ?? Array.Empty<float>();
                        }
                        else if (chunk.ContainsKey("embedding") && chunk["embedding"] is JsonElement embeddingElement2)
                        {
                            chunkEmbedding = JsonSerializer.Deserialize<float[]>(embeddingElement2.GetRawText()) ?? Array.Empty<float>();
                        }
                        else
                        {
                            continue;
                        }

                        // Calculate similarity
                        var similarity = CosineSimilarity(queryEmbedding, chunkEmbedding);

                        // Parse text and file information
                        string text = "";
                        string file = "";
                        int chunkIndex = 0;

                        if (chunk.ContainsKey("ChunkText") && chunk["ChunkText"] is JsonElement textElement)
                            text = textElement.GetString() ?? "";
                        else if (chunk.ContainsKey("text") && chunk["text"] is JsonElement textElement2)
                            text = textElement2.GetString() ?? "";

                        if (chunk.ContainsKey("FilePath") && chunk["FilePath"] is JsonElement fileElement)
                            file = fileElement.GetString() ?? "";
                        else if (chunk.ContainsKey("file") && chunk["file"] is JsonElement fileElement2)
                            file = fileElement2.GetString() ?? "";

                        if (chunk.ContainsKey("ChunkIndex") && chunk["ChunkIndex"] is JsonElement indexElement)
                            chunkIndex = indexElement.GetInt32();
                        else if (chunk.ContainsKey("chunk_index") && chunk["chunk_index"] is JsonElement indexElement2)
                            chunkIndex = indexElement2.GetInt32();

                        allResults.Add(new SearchResult
                        {
                            Text = text,
                            FilePath = file,
                            ChunkIndex = chunkIndex,
                            Similarity = similarity,
                            FolderPath = folderPath
                        });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error parsing chunk: {ex.Message}");
                    }
                }
            }

            // Sort by similarity and take top K
            var topResults = allResults
                .OrderByDescending(r => r.Similarity)
                .Take(topK)
                .ToList();

            // Limit by context length
            var limitedResults = new List<SearchResult>();
            int currentLength = 0;
            foreach (var result in topResults)
            {
                if (currentLength + result.Text.Length > maxContextLength)
                {
                    // If this is the first result and it's too long, truncate it
                    if (limitedResults.Count == 0 && result.Text.Length > maxContextLength)
                    {
                        result.Text = result.Text.Substring(0, maxContextLength);
                        limitedResults.Add(result);
                    }
                    break;
                }
                limitedResults.Add(result);
                currentLength += result.Text.Length;
            }

            return limitedResults;
        }

        public async Task<float[]> GenerateEmbeddingAsync(string text)
        {
            if (_useNativeEmbedding)
            {
                // Use native gguf_loader (Python GGUF implementation)
                return await GenerateEmbeddingNativeAsync(text);
            }
            else
            {
                // Use OpenAI-compatible API
                if (_embeddingClient == null)
                {
                    throw new VectorizationException(
                        "Embedding client is not initialized",
                        "Initialization Error",
                        "• Check your API credentials and configuration\n" +
                        "• Ensure the embedding service is properly configured in Settings");
                }

                try
                {
                    var embedding = await _embeddingClient.GenerateEmbeddingAsync(text);
                    return embedding.Value.ToFloats().ToArray();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error generating embedding: {ex.Message}");
                    throw new VectorizationException(
                        $"Failed to generate embedding: {ex.Message}",
                        ex);
                }
            }
        }

        private async Task<float[]> GenerateEmbeddingNativeAsync(string text)
        {
            var tempConfigFile = Path.Combine(Path.GetTempPath(), $"native_embedding_{Guid.NewGuid()}.json");

            try
            {
                // Build model path from _ggufModel (e.g., "unsloth/embeddinggemma-300M-Q8_0.gguf")
                string? modelPath = null;
                if (!string.IsNullOrEmpty(_ggufModel))
                {
                    modelPath = Path.Combine(PythonPathHelper.PythonToolsDirectory, "models", "embedding", _ggufModel);
                }

                var config = new
                {
                    text = text,
                    model_path = modelPath
                };

                var configJson = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(tempConfigFile, configJson);

                var nativeEmbeddingScript = PythonPathHelper.GetScriptPath("native_embedding_server.py");

                var processStartInfo = new ProcessStartInfo
                {
                    FileName = _pythonExecutable,
                    Arguments = $"\"{nativeEmbeddingScript}\" \"{tempConfigFile}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = PythonPathHelper.PythonToolsDirectory
                };

                using var process = new Process { StartInfo = processStartInfo };
                process.Start();

                var output = await process.StandardOutput.ReadToEndAsync();
                var error = await process.StandardError.ReadToEndAsync();

                await process.WaitForExitAsync();

                if (process.ExitCode != 0)
                {
                    var detailedError = $"Exit code: {process.ExitCode}\n" +
                                      $"Standard Output:\n{output}\n" +
                                      $"Standard Error:\n{error}\n" +
                                      $"Script: {nativeEmbeddingScript}\n" +
                                      $"Config: {tempConfigFile}\n" +
                                      $"Python: {_pythonExecutable}";

                    throw new VectorizationException(
                        $"Native embedding failed with exit code {process.ExitCode}",
                        "Native Embedding Error",
                        detailedError);
                }

                // Parse JSON result
                var lastLine = output.Trim().Split('\n').LastOrDefault() ?? "";
                var result = JsonSerializer.Deserialize<Dictionary<string, object>>(lastLine);

                if (result != null && result.ContainsKey("success"))
                {
                    var success = result["success"].ToString() == "True";

                    if (success && result.ContainsKey("embedding"))
                    {
                        var embeddingElement = (JsonElement)result["embedding"];
                        return JsonSerializer.Deserialize<float[]>(embeddingElement.GetRawText()) ?? Array.Empty<float>();
                    }
                    else if (result.ContainsKey("error"))
                    {
                        var errorMsg = result["error"].ToString();
                        throw new VectorizationException(
                            "Native embedding generation failed",
                            "Native Embedding Error",
                            errorMsg ?? "Unknown error");
                    }
                }

                throw new VectorizationException(
                    "Invalid response from native embedding server",
                    "Native Embedding Error",
                    $"Output: {output}");
            }
            catch (VectorizationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new VectorizationException(
                    $"Failed to generate native embedding: {ex.Message}",
                    "Native Embedding Error",
                    ex.StackTrace ?? "");
            }
            finally
            {
                if (File.Exists(tempConfigFile))
                {
                    try
                    {
                        File.Delete(tempConfigFile);
                    }
                    catch
                    {
                    }
                }
            }
        }

        private float CosineSimilarity(float[] a, float[] b)
        {
            if (a.Length != b.Length || a.Length == 0)
                return 0;

            float dotProduct = 0;
            float normA = 0;
            float normB = 0;

            for (int i = 0; i < a.Length; i++)
            {
                dotProduct += a[i] * b[i];
                normA += a[i] * a[i];
                normB += b[i] * b[i];
            }

            normA = (float)Math.Sqrt(normA);
            normB = (float)Math.Sqrt(normB);

            if (normA == 0 || normB == 0)
                return 0;

            return dotProduct / (normA * normB);
        }
    }
}