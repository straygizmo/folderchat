using folderchat.Services;
using Krypton.Toolkit;
using System.Data;

namespace folderchat.Forms
{
    public partial class RagSettingsControl : UserControl
    {
        private MainForm? _mainForm;
        private bool _locSubscribed = false;

        public RagSettingsControl()
        {
            InitializeComponent();
        }

        public void Initialize(MainForm mainForm)
        {
            _mainForm = mainForm;

            // Apply localization to UI elements
            ApplyLocalization();

            // Subscribe to culture change to update labels dynamically
            if (Program.LocalizationService != null && !_locSubscribed)
            {
                Program.LocalizationService.CultureChanged += (_, __) => ApplyLocalization();
                _locSubscribed = true;
            }
        }

        private void ApplyLocalization()
        {
            var loc = Program.LocalizationService;
            if (loc == null) return;

            lblEmbeddingMethod.Values.Text = $"{loc.GetString("EmbeddingMethod")}:";
            lblEmbeddingUrl.Values.Text = $"{loc.GetString("EmbeddingURL")}:";
            lblEmbeddingModel.Values.Text = $"{loc.GetString("EmbeddingModel")}:";
            lblModelContextLength.Values.Text = $"{loc.GetString("ModelContextLength")}:";
            lblChunkSize.Values.Text = $"{loc.GetString("ChunkSize")}:";
            lblChunkOverlap.Values.Text = $"{loc.GetString("ChunkOverlap")}:";
            lblTopKChunks.Values.Text = $"{loc.GetString("TopKChunks")}:";
            lblTotalMaxContextLength.Values.Text = $"{loc.GetString("TotalMaxContextLength")}:";
            rbEmbeddingAPI.Values.Text = loc.GetString("API");
            rbEmbeddingGGUF.Values.Text = loc.GetString("Local");

            // Prefer dedicated label if available, otherwise fallback to generic "Test"
            var testText = loc.GetString("TestEmbedding");
            if (testText == "TestEmbedding")
            {
                testText = loc.GetString("Test");
            }
            btnTestEmbedding.Values.Text = testText;
        }

        public void LoadSettings()
        {
            // Load embedding settings
            txtEmbeddingUrl.Text = Properties.Settings.Default.Embedding_Url;
            txtEmbeddingModel.Text = Properties.Settings.Default.Embedding_Model;
            nudModelContextLength.Value = Properties.Settings.Default.RAG_ContextLength;
            nudChunkSize.Value = Properties.Settings.Default.RAG_ChunkSize;
            nudChunkOverlap.Value = Properties.Settings.Default.RAG_ChunkOverlap;
            nudTopKChunks.Value = Properties.Settings.Default.RAG_TopKChunks;
            nudMaxContextLength.Value = Properties.Settings.Default.RAG_MaxContextLength;

            // Load embedding method
            var embeddingMethod = Properties.Settings.Default.EmbeddingMethod;
            if (embeddingMethod == "Local")
            {
                rbEmbeddingGGUF.Checked = true;
            }
            else
            {
                rbEmbeddingAPI.Checked = true;
            }

            // Load GGUF models
            LoadGGUFModels();

            // Load selected GGUF model
            var ggufModel = Properties.Settings.Default.EmbeddingGGUFModel;
            if (!string.IsNullOrEmpty(ggufModel) && cmbGGUFModel.Items.Contains(ggufModel))
            {
                cmbGGUFModel.SelectedItem = ggufModel;
            }

            // Update UI visibility based on selected method
            UpdateEmbeddingMethodUI();
        }

        public void SaveSettings()
        {
            // Save embedding settings
            Properties.Settings.Default.Embedding_Url = txtEmbeddingUrl.Text;
            Properties.Settings.Default.Embedding_Model = txtEmbeddingModel.Text;

            Properties.Settings.Default.RAG_ContextLength = (int)nudModelContextLength.Value;
            Properties.Settings.Default.RAG_ChunkSize = (int)nudChunkSize.Value;
            Properties.Settings.Default.RAG_ChunkOverlap = (int)nudChunkOverlap.Value;
            Properties.Settings.Default.RAG_TopKChunks = (int)nudTopKChunks.Value;
            Properties.Settings.Default.RAG_MaxContextLength = (int)nudMaxContextLength.Value;

            // Save embedding method
            Properties.Settings.Default.EmbeddingMethod = rbEmbeddingGGUF.Checked ? "Local" : "API";
            Properties.Settings.Default.EmbeddingGGUFModel = cmbGGUFModel.SelectedItem?.ToString() ?? "";

            // For backward compatibility, set UseNativeEmbedding based on embedding method
            Properties.Settings.Default.UseNativeEmbedding = rbEmbeddingGGUF.Checked;
        }

        private void UpdateEmbeddingMethodUI()
        {
            if (rbEmbeddingAPI.Checked)
            {
                // Show API controls
                txtEmbeddingUrl.Visible = true;
                lblEmbeddingUrl.Visible = true;
                txtEmbeddingModel.Visible = true;
                lblEmbeddingModel.Location = new Point(3, txtEmbeddingModel.Top - 21);

                // Hide GGUF controls
                cmbGGUFModel.Visible = false;
            }
            else if (rbEmbeddingGGUF.Checked)
            {
                // Hide API controls
                txtEmbeddingUrl.Visible = false;
                lblEmbeddingUrl.Visible = false;
                txtEmbeddingModel.Visible = false;
                lblEmbeddingModel.Location = new Point(3, cmbGGUFModel.Top - 21);

                // Show GGUF controls
                cmbGGUFModel.Visible = true;
            }
        }

        private void rbEmbeddingAPI_CheckedChanged(object? sender, EventArgs e)
        {
            if (rbEmbeddingAPI.Checked)
            {
                UpdateEmbeddingMethodUI();
            }
        }

        private void rbEmbeddingGGUF_CheckedChanged(object? sender, EventArgs e)
        {
            if (rbEmbeddingGGUF.Checked)
            {
                UpdateEmbeddingMethodUI();
            }
        }

        private void LoadGGUFModels()
        {
            cmbGGUFModel.Items.Clear();

            try
            {
                var modelsPath = Path.Combine(PythonPathHelper.PythonToolsDirectory, "models");

                if (!Directory.Exists(modelsPath))
                {
                    return;
                }

                // Check for embedding models in models/embedding/[provider_name]/[model].gguf
                var embeddingPath = Path.Combine(modelsPath, "embedding");
                if (Directory.Exists(embeddingPath))
                {
                    foreach (var providerDir in Directory.GetDirectories(embeddingPath))
                    {
                        var providerName = Path.GetFileName(providerDir);
                        var ggufFiles = Directory.GetFiles(providerDir, "*.gguf", SearchOption.AllDirectories);
                        foreach (var ggufFile in ggufFiles)
                        {
                            var fileName = Path.GetFileName(ggufFile);
                            var displayName = $"{providerName}/{fileName}";
                            cmbGGUFModel.Items.Add(displayName);
                        }
                    }
                }

                // Legacy support: Check for .gguf files in models/[provider_name]/
                foreach (var dir in Directory.GetDirectories(modelsPath))
                {
                    var dirName = Path.GetFileName(dir);
                    // Skip the embedding folder (already processed above)
                    if (dirName == "embedding" || dirName == "chat")
                    {
                        continue;
                    }

                    var ggufFiles = Directory.GetFiles(dir, "*.gguf");
                    foreach (var ggufFile in ggufFiles)
                    {
                        var fileName = Path.GetFileName(ggufFile);
                        var displayName = $"{dirName}/{fileName}";
                        cmbGGUFModel.Items.Add(displayName);
                    }
                }

                // Legacy support: Check for .gguf files directly in models folder
                foreach (var ggufFile in Directory.GetFiles(modelsPath, "*.gguf"))
                {
                    var fileName = Path.GetFileName(ggufFile);
                    cmbGGUFModel.Items.Add(fileName);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading GGUF models: {ex.Message}");
            }
        }

        private async void btnTestEmbedding_Click(object? sender, EventArgs e)
        {
            if (_mainForm == null) return;

            try
            {
                _mainForm.LogSystemMessage("Testing embedding configuration...");

                // Get current settings from UI
                var embeddingUrl = txtEmbeddingUrl.Text;
                var embeddingModel = txtEmbeddingModel.Text;
                var apiKey = Properties.Settings.Default.OpenAI_ApiKey;
                var useNativeEmbedding = rbEmbeddingGGUF.Checked;
                var ggufModel = cmbGGUFModel.SelectedItem?.ToString() ?? "";

                // Validate settings
                if (useNativeEmbedding)
                {
                    if (string.IsNullOrWhiteSpace(ggufModel))
                    {
                        _mainForm.LogError("GGUF Model is required for embedding test");
                        MessageBox.Show("GGUF Model is required.", "Validation Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(embeddingUrl))
                    {
                        _mainForm.LogError("Embedding URL is required for embedding test");
                        MessageBox.Show("Embedding URL is required.", "Validation Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (string.IsNullOrWhiteSpace(embeddingModel))
                    {
                        _mainForm.LogError("Embedding Model is required for embedding test");
                        MessageBox.Show("Embedding Model is required.", "Validation Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                // Disable button during test
                if (sender is Krypton.Toolkit.KryptonButton btn)
                {
                    btn.Enabled = false;
                    btn.Values.Text = Program.LocalizationService?.GetString("Testing") ?? "Testing...";
                }

                string message;

                if (useNativeEmbedding)
                {
                    // Use native gguf_loader (Python GGUF implementation)
                    var ragService = new RagService(
                        embeddingUrl,
                        apiKey,
                        embeddingModel,
                        useNativeEmbedding,
                        contextLength: 2048,
                        chunkSize: 500,
                        chunkOverlap: 100,
                        ggufModel: ggufModel
                    );

                    const string testText = "This is a test to verify the embedding configuration.";
                    var embedding = await ragService.GenerateEmbeddingAsync(testText);

                    message = $"✓ Connection Successful!\n\n" +
                             $"Method: Native GGUF (gguf_loader)\n" +
                             $"Embedding Model: Local GGUF\n" +
                             $"Embedding Dimension: {embedding.Length}\n" +
                             $"Sample values: [{string.Join(", ", embedding.Take(5).Select(f => f.ToString("F4")))}...]" ;

                    _mainForm.LogSystemMessage($"Embedding test successful (GGUF): dimension={embedding.Length}");
                }
                else
                {
                    // Use Python implementation
                    var pythonExecutable = PythonPathHelper.PythonExecutable;
                    var testScript = PythonPathHelper.GetScriptPath("test_embedding.py");

                    // Create temporary config file
                    var tempConfigFile = Path.Combine(Path.GetTempPath(), $"test_embedding_config_{Guid.NewGuid()}.json");

                    try
                    {
                        var config = new
                        {
                            embedding_url = embeddingUrl,
                            embedding_model = embeddingModel,
                            api_key = apiKey
                        };

                        var configJson = System.Text.Json.JsonSerializer.Serialize(config, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
                        await File.WriteAllTextAsync(tempConfigFile, configJson);

                        // Run Python test script
                        var processStartInfo = new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = pythonExecutable,
                            Arguments = $"\"{testScript}\" \"{tempConfigFile}\"",
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            UseShellExecute = false,
                            CreateNoWindow = true,
                            WorkingDirectory = PythonPathHelper.PythonToolsDirectory
                        };

                        using var process = new System.Diagnostics.Process { StartInfo = processStartInfo };
                        process.Start();

                        var output = await process.StandardOutput.ReadToEndAsync();
                        var error = await process.StandardError.ReadToEndAsync();

                        await process.WaitForExitAsync();

                        // Parse JSON result from Python script
                        var lastLine = output.Trim().Split('\n').LastOrDefault() ?? "";
                        var result = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(lastLine);

                        if (result != null && result.ContainsKey("success"))
                        {
                            var success = result["success"].ToString() == "True";

                            if (success)
                            {
                                var dimension = result.ContainsKey("dimension") ? result["dimension"].ToString() : "unknown";
                                var sampleValues = result.ContainsKey("sample_values")
                                    ? System.Text.Json.JsonSerializer.Deserialize<float[]>(result["sample_values"].ToString() ?? "[]")
                                    : Array.Empty<float>();

                                message = $"✓ Connection Successful!\n\n" +
                                         $"Method: Python\n" +
                                         $"Embedding Model: {embeddingModel}\n" +
                                         $"Embedding URL: {embeddingUrl}\n" +
                                         $"Embedding Dimension: {dimension}\n" +
                                         $"Sample values: [{string.Join(", ", sampleValues.Take(5).Select(f => f.ToString("F4")))}...]" ;

                                _mainForm.LogSystemMessage($"Embedding test successful (Python): dimension={dimension}");
                            }
                            else
                            {
                                var errorMsg = result.ContainsKey("error") ? result["error"].ToString() : "Unknown error";
                                _mainForm.LogError($"Embedding test failed: {errorMsg}");
                                throw new Exception(errorMsg ?? "Unknown error");
                            }
                        }
                        else
                        {
                            throw new Exception($"Invalid Python script output:\n{output}\n\nError:\n{error}");
                        }
                    }
                    finally
                    {
                        // Clean up temp file
                        if (File.Exists(tempConfigFile))
                        {
                            try { File.Delete(tempConfigFile); } catch { }
                        }
                    }
                }

                MessageBox.Show(message, "Embedding Test Result",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                // Show error message
                _mainForm.LogError($"Embedding test error: {ex.Message}");
                var errorMessage = $"✗ Connection Failed!\n\n" +
                                  $"Error: {ex.Message}";

                if (ex.InnerException != null)
                {
                    errorMessage += $"\n\nDetails: {ex.InnerException.Message}";
                }

                MessageBox.Show(errorMessage, "Embedding Test Result",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Re-enable button
                if (sender is Krypton.Toolkit.KryptonButton btn)
                {
                    btn.Enabled = true;
                    {
                        var loc = Program.LocalizationService;
                        var text = loc?.GetString("TestEmbedding") ?? "Test Embedding";
                        if (text == "TestEmbedding")
                        {
                            text = loc?.GetString("Test") ?? "Test";
                        }
                        btn.Values.Text = text;
                    }
                }
            }
        }
    }
}
