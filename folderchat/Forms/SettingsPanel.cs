using folderchat.Models;
using folderchat.Services;
using folderchat.Services.Mcp;
using System.Data;
using System.Text.Json;

namespace folderchat.Forms
{
    public partial class SettingsPanel : UserControl
    {
        private IMcpService? _mcpService;
        private McpServerCollection _mcpServerCollection = new();
        private MainForm? _mainForm;

        public SettingsPanel()
        {
            InitializeComponent();
            InitializeMcpComponents();
        }

        public void Initialize(MainForm mainForm)
        {
_mainForm = mainForm;
LoadSettings();

// Wire up the API server checkbox change event
chkEnableAPIServer.CheckedChanged += ChkEnableAPIServer_CheckedChanged;
        }

        private async void ChkEnableAPIServer_CheckedChanged(object? sender, EventArgs e)
        {
            if (_mainForm == null) return;

            var isEnabled = chkEnableAPIServer.Checked;
            Properties.Settings.Default.EnableAPIServer = isEnabled;
            Properties.Settings.Default.Save();

            await _mainForm.ToggleApiServer(isEnabled);
        }

        private void LoadSettings()
        {
            // Load Language selection
            InitializeLanguageComboBox();
            UpdateUILabels(); // Apply current language labels

            // Load API Provider selection
            var apiProvider = Properties.Settings.Default.API_Provider;
            cmbAPIProvider.SelectedItem = apiProvider;

            // Load provider-specific settings
            openAISettingsControl.LoadSettings();
            claudeCodeSettingsControl.LoadSettings();
            azureAIFoundrySettingsControl.LoadSettings();
            openRouterSettingsControl.LoadSettings();
            ollamaSettingsControl.LoadSettings();
            openAIAPISettingsControl.LoadSettings();
            geminiSettingsControl.LoadSettings();
            claudeSettingsControl.LoadSettings();

            // Load API Server settings
            chkEnableAPIServer.Checked = Properties.Settings.Default.EnableAPIServer;
            nudServerPort.Value = Properties.Settings.Default.APIServerPort == 0 ? 11550 : Properties.Settings.Default.APIServerPort;

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
            if (embeddingMethod == "GGUF")
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

            // Load chat method
            var chatMethod = Properties.Settings.Default.ChatMethod;
            if (chatMethod == "GGUF")
            {
                rbChatGGUF.Checked = true;
            }
            else
            {
                rbChatAPI.Checked = true;
            }

            // Load Chat GGUF models
            LoadChatGGUFModels();

            // Load selected Chat GGUF model
            var chatGGUFModel = Properties.Settings.Default.ChatGGUFModel;
            if (!string.IsNullOrEmpty(chatGGUFModel) && cmbChatGGUFModel.Items.Contains(chatGGUFModel))
            {
                cmbChatGGUFModel.SelectedItem = chatGGUFModel;
            }

            // Update chat method UI
            UpdateChatMethodUI();

            var selectedIndex = Properties.Settings.Default.UI_Theme;
            if (selectedIndex >= 0)
            {
                kryptonThemeListBox1.SelectedIndex = selectedIndex;
            }

            // Update UI visibility based on selected provider
            UpdateProviderUI(apiProvider);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (_mainForm == null) return;

            _mainForm.LogSystemMessage("Saving settings...");

            // Capture current chat settings before saving
            var oldApiProvider = Properties.Settings.Default.API_Provider;
            var oldChatMethod = Properties.Settings.Default.ChatMethod;
            var oldChatGGUFModel = Properties.Settings.Default.ChatGGUFModel;

            // Save API Provider selection
            Properties.Settings.Default.API_Provider = cmbAPIProvider.SelectedItem?.ToString() ?? "OpenAI Compatible";

            // Save provider-specific settings (always save, even if not currently selected)
            openAISettingsControl.SaveSettings();
            claudeCodeSettingsControl.SaveSettings();
            azureAIFoundrySettingsControl.SaveSettings();
            openRouterSettingsControl.SaveSettings();
            ollamaSettingsControl.SaveSettings();
            openAIAPISettingsControl.SaveSettings();
            geminiSettingsControl.SaveSettings();
            claudeSettingsControl.SaveSettings();

            // Save embedding settings
            Properties.Settings.Default.Embedding_Url = txtEmbeddingUrl.Text;
            Properties.Settings.Default.Embedding_Model = txtEmbeddingModel.Text;

            Properties.Settings.Default.RAG_ContextLength = (int)nudModelContextLength.Value;
            Properties.Settings.Default.RAG_ChunkSize = (int)nudChunkSize.Value;
            Properties.Settings.Default.RAG_ChunkOverlap = (int)nudChunkOverlap.Value;
            Properties.Settings.Default.RAG_TopKChunks = (int)nudTopKChunks.Value;
            Properties.Settings.Default.RAG_MaxContextLength = (int)nudMaxContextLength.Value;

            // Save embedding method
            Properties.Settings.Default.EmbeddingMethod = rbEmbeddingGGUF.Checked ? "GGUF" : "API";
            Properties.Settings.Default.EmbeddingGGUFModel = cmbGGUFModel.SelectedItem?.ToString() ?? "";

            // For backward compatibility, set UseNativeEmbedding based on embedding method
            Properties.Settings.Default.UseNativeEmbedding = rbEmbeddingGGUF.Checked;

            // Save chat method
            var newChatMethod = rbChatGGUF.Checked ? "GGUF" : "API";
            var newChatGGUFModel = cmbChatGGUFModel.SelectedItem?.ToString() ?? "";
            Properties.Settings.Default.ChatMethod = newChatMethod;
            Properties.Settings.Default.ChatGGUFModel = newChatGGUFModel;

            // Save API Server settings
            Properties.Settings.Default.APIServerPort = (int)nudServerPort.Value;

            // Save all settings to ensure they're persisted
            Properties.Settings.Default.Save();

            // Check if chat settings changed
            var newApiProvider = Properties.Settings.Default.API_Provider;
            bool chatSettingsChanged = oldApiProvider != newApiProvider ||
                                      oldChatMethod != newChatMethod ||
                                      (newChatMethod == "GGUF" && oldChatGGUFModel != newChatGGUFModel);

            // Check if we need to initialize or reinitialize Blazor WebView
            bool needsInitialization = _mainForm.NeedsBlazorInitialization();
            bool needsReinitialization = chatSettingsChanged && !needsInitialization;

            if (needsInitialization || needsReinitialization)
            {
                // Check if configuration is valid before initializing
                if (!_mainForm.IsConfigured())
                {
                    _mainForm.LogError("Configuration incomplete");
                    MessageBox.Show("Settings saved, but configuration is incomplete. Please complete all required settings.",
                        "Configuration Incomplete", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                try
                {
                    // Initialize/Reinitialize with new settings
                    _mainForm.ReinitializeBlazorWebView();

                    _mainForm.LogSystemMessage("Settings saved successfully");
                    MessageBox.Show("Settings saved successfully! Chat service has been loaded with the new configuration.",
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    _mainForm.LogError($"Failed to load chat service: {ex.Message}");
                    MessageBox.Show($"Settings saved, but failed to load chat service: {ex.Message}\n\nPlease check your configuration and try again.",
                        "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                _mainForm.LogSystemMessage("Settings saved successfully");
                MessageBox.Show("Settings saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                    btn.Values.Text = "Testing...";
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
                             $"Sample values: [{string.Join(", ", embedding.Take(5).Select(f => f.ToString("F4")))}...]";

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
                                         $"Sample values: [{string.Join(", ", sampleValues.Take(5).Select(f => f.ToString("F4")))}...]";

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
                    btn.Values.Text = "Test Embedding";
                }
            }
        }

        private void InitializeLanguageComboBox()
        {
            // Clear existing items
            cmbLanguage.Items.Clear();

            // Add language options
            cmbLanguage.Items.Add("English (US)");
            cmbLanguage.Items.Add("日本語 (Japanese)");

            // Set the current language
            var currentLanguage = Properties.Settings.Default.PreferredLanguage;
            if (currentLanguage == "ja-JP")
            {
                cmbLanguage.SelectedIndex = 1;
            }
            else
            {
                cmbLanguage.SelectedIndex = 0;
            }
        }

        private void cmbLanguage_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (Program.LocalizationService == null) return;

            var selectedIndex = cmbLanguage.SelectedIndex;
            var cultureName = selectedIndex == 1 ? "ja-JP" : "en-US";

            // Change the culture
            Program.LocalizationService.ChangeCulture(cultureName);

            // Update UI labels
            UpdateUILabels();
        }

        private void UpdateUILabels()
        {
            var loc = Program.LocalizationService;
            if (loc == null) return;

            // Update tab names
            tabPageChat.Text = loc.GetString("TabChat");
            tabPageRAG.Text = loc.GetString("TabRAG");
            tabPageGeneral.Text = loc.GetString("TabGeneral");
            tabPageMCP.Text = loc.GetString("TabMCP");

            // Update labels in Settings
            lblAPIProvider.Values.Text = loc.GetString("APIProvider") + ":";
            lblLanguage.Values.Text = loc.GetString("Language") + ":";

            // Update RAG Settings labels
            lblEmbeddingUrl.Values.Text = loc.GetString("EmbeddingURL") + ":";
            lblEmbeddingModel.Values.Text = loc.GetString("EmbeddingModel") + ":";
            lblModelContextLength.Values.Text = loc.GetString("ModelContextLength") + ":";
            lblChunkSize.Values.Text = loc.GetString("ChunkSize") + ":";
            lblChunkOverlap.Values.Text = loc.GetString("ChunkOverlap") + ":";
            lblTotalMaxContextLength.Values.Text = loc.GetString("TotalMaxContextLength") + ":";
            lblTopKChunks.Values.Text = loc.GetString("TopKChunks") + ":";

            // Update button texts
            btnSaveChatSettings.Values.Text = loc.GetString("Save");
            btnSaveRAGSettings.Values.Text = loc.GetString("Save");

            // Update MCP buttons
            btnAddMcpServer.Values.Text = loc.GetString("Add");
            btnEditMcpServer.Values.Text = loc.GetString("Edit");
            btnRemoveMcpServer.Values.Text = loc.GetString("Remove");
        }

        private void cmbAPIProvider_SelectedIndexChanged(object? sender, EventArgs e)
        {
            var selectedProvider = cmbAPIProvider.SelectedItem?.ToString();
            UpdateProviderUI(selectedProvider);
        }

        private void UpdateProviderUI(string? provider)
        {
            // Clear existing controls
            kPanelAPIProviderSettings.Controls.Clear();

            if (provider == "Claude Code")
            {
                // Add Claude Code settings control
                claudeCodeSettingsControl.Dock = DockStyle.Fill;
                kPanelAPIProviderSettings.Controls.Add(claudeCodeSettingsControl);
            }
            else if (provider == "Azure AI Foundry")
            {
                // Add Azure AI Foundry settings control
                azureAIFoundrySettingsControl.Dock = DockStyle.Fill;
                kPanelAPIProviderSettings.Controls.Add(azureAIFoundrySettingsControl);
            }
            else if (provider == "OpenRouter")
            {
                // Add OpenRouter settings control
                openRouterSettingsControl.Dock = DockStyle.Fill;
                kPanelAPIProviderSettings.Controls.Add(openRouterSettingsControl);
            }
            else if (provider == "Ollama")
            {
                // Add Ollama settings control
                ollamaSettingsControl.Dock = DockStyle.Fill;
                kPanelAPIProviderSettings.Controls.Add(ollamaSettingsControl);
            }
            else if (provider == "OpenAI")
            {
                // Add OpenAI API settings control
                openAIAPISettingsControl.Dock = DockStyle.Fill;
                kPanelAPIProviderSettings.Controls.Add(openAIAPISettingsControl);
            }
            else if (provider == "Gemini")
            {
                // Add Gemini settings control
                geminiSettingsControl.Dock = DockStyle.Fill;
                kPanelAPIProviderSettings.Controls.Add(geminiSettingsControl);
            }
            else if (provider == "Claude")
            {
                // Add Claude settings control
                claudeSettingsControl.Dock = DockStyle.Fill;
                kPanelAPIProviderSettings.Controls.Add(claudeSettingsControl);
            }
            else // OpenAI Compatible
            {
                // Add OpenAI Compatible settings control
                openAISettingsControl.Dock = DockStyle.Fill;
                kPanelAPIProviderSettings.Controls.Add(openAISettingsControl);
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

        private void LoadChatGGUFModels()
        {
            cmbChatGGUFModel.Items.Clear();

            try
            {
                var modelsPath = Path.Combine(PythonPathHelper.PythonToolsDirectory, "models");

                if (!Directory.Exists(modelsPath))
                {
                    return;
                }

                // Check for chat models in models/chat/[provider_name]/[model_name]/[model].gguf
                var chatPath = Path.Combine(modelsPath, "chat");
                if (Directory.Exists(chatPath))
                {
                    foreach (var providerDir in Directory.GetDirectories(chatPath))
                    {
                        var providerName = Path.GetFileName(providerDir);
                        var ggufFiles = Directory.GetFiles(providerDir, "*.gguf", SearchOption.AllDirectories);
                        foreach (var ggufFile in ggufFiles)
                        {
                            var fileName = Path.GetFileName(ggufFile);
                            var displayName = $"{providerName}/{fileName}";
                            cmbChatGGUFModel.Items.Add(displayName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading Chat GGUF models: {ex.Message}");
            }
        }

        private void UpdateEmbeddingMethodUI()
        {
            if (rbEmbeddingAPI.Checked)
            {
                // Show API controls
                txtEmbeddingUrl.Visible = true;
                lblEmbeddingUrl.Visible = true;
                txtEmbeddingModel.Visible = true;
                lblEmbeddingModel.Location = new Point(3, 85);

                // Hide GGUF controls
                cmbGGUFModel.Visible = false;
            }
            else if (rbEmbeddingGGUF.Checked)
            {
                // Hide API controls
                txtEmbeddingUrl.Visible = false;
                lblEmbeddingUrl.Visible = false;
                txtEmbeddingModel.Visible = false;
                lblEmbeddingModel.Location = new Point(3, 34);

                // Show GGUF controls
                cmbGGUFModel.Visible = true;
            }
        }

        private void UpdateChatMethodUI()
        {
            if (rbChatAPI.Checked)
            {
                // Show API provider controls
                cmbAPIProvider.Visible = true;
                lblAPIProvider.Text = "API Provider:";
                kPanelAPIProviderSettings.Dock = DockStyle.Fill;
                kPanelAPIProviderSettings.Visible = true;
                kPanelGGUFProviderSettings.Visible = false;

                // Hide GGUF controls
                cmbChatGGUFModel.Visible = false;
            }
            else if (rbChatGGUF.Checked)
            {
                // Hide API provider controls
                lblAPIProvider.Text = "Chat GGUF Model:";
                cmbAPIProvider.Visible = false;
                kPanelGGUFProviderSettings.Dock = DockStyle.Fill;
                kPanelGGUFProviderSettings.Visible = true;
                kPanelAPIProviderSettings.Visible = false;

                // Show GGUF controls
                cmbChatGGUFModel.Visible = true;
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

        private void rbChatAPI_CheckedChanged(object? sender, EventArgs e)
        {
            if (rbChatAPI.Checked)
            {
                UpdateChatMethodUI();
            }
        }

        private void rbChatGGUF_CheckedChanged(object? sender, EventArgs e)
        {
            if (rbChatGGUF.Checked)
            {
                UpdateChatMethodUI();
            }
        }

        private void KryptonThemeListBox1_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (_mainForm == null) return;

            Properties.Settings.Default.UI_Theme = kryptonThemeListBox1.SelectedIndex;
            Properties.Settings.Default.Save();

            // Notify MainForm about theme change
            _mainForm.OnThemeChanged();
        }

        #region MCP Server Management

        private void InitializeMcpComponents()
        {
            // Initialize MCP service
            _mcpService = new McpService();
            _mcpService.LogMessage += (sender, message) =>
            {
                AppendMcpLog(message);
            };
            _mcpService.ServerStatusChanged += (sender, args) =>
            {
                UpdateServerStatus(args);
            };

            // Setup DataGridView
            dgvMcpServers.AllowUserToAddRows = false;
            dgvMcpServers.AllowUserToDeleteRows = false;
            dgvMcpServers.ReadOnly = false;
            dgvMcpServers.Columns.Clear();
            dgvMcpServers.Columns.Add("Name", "Name");
            dgvMcpServers.Columns.Add("Status", "Status");
            dgvMcpServers.Columns.Add("Type", "Type");

            // Add checkbox column for Enabled
            var enabledColumn = new DataGridViewCheckBoxColumn
            {
                Name = "Enabled",
                HeaderText = "Enabled",
                Width = 50,
                ReadOnly = false
            };
            dgvMcpServers.Columns.Add(enabledColumn);

            dgvMcpServers.RowHeadersWidth = 25;
            dgvMcpServers.Columns[0].Width = 115;   // Name column
            dgvMcpServers.Columns[0].ReadOnly = true;
            dgvMcpServers.Columns[1].Width = 85;    // Status column
            dgvMcpServers.Columns[1].ReadOnly = true;
            dgvMcpServers.Columns[2].Width = 40;    // Type column
            dgvMcpServers.Columns[2].ReadOnly = true;

            // Handle checkbox cell click
            dgvMcpServers.CellContentClick += DgvMcpServers_CellContentClick;

            // Note: Event handlers are already wired up in Designer.cs
            // Do not add them here again to avoid duplicate event registration

            // Load MCP servers from settings
            LoadMcpServers();
        }

        private void LoadMcpServers()
        {
            try
            {
                var mcpServersJson = Properties.Settings.Default.MCP_Servers;
                if (!string.IsNullOrEmpty(mcpServersJson))
                {
                    _mcpServerCollection = JsonSerializer.Deserialize<McpServerCollection>(mcpServersJson) ?? new McpServerCollection();
                }
                else
                {
                    _mcpServerCollection = new McpServerCollection();
                }

                RefreshMcpServerList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading MCP servers: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveMcpServers()
        {
            try
            {
                var json = JsonSerializer.Serialize(_mcpServerCollection, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
                Properties.Settings.Default.MCP_Servers = json;
                Properties.Settings.Default.Save();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving MCP servers: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RefreshMcpServerList()
        {
            dgvMcpServers.Rows.Clear();
            foreach (var server in _mcpServerCollection.Servers)
            {
                var status = _mcpService?.IsServerLoaded(server.Id) ?? false ? "Connected" : "Disconnected";
                dgvMcpServers.Rows.Add(server.Name, status, server.TransportType.ToString(), server.IsEnabled);
                var row = dgvMcpServers.Rows[dgvMcpServers.Rows.Count - 1];
                row.Tag = server;
            }
        }

        private void BtnAddMcpServer_Click(object? sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("SettingsPanel: BtnAddMcpServer_Click - Creating dialog");
            using var dialog = new McpServerDialog();
            System.Diagnostics.Debug.WriteLine("SettingsPanel: BtnAddMcpServer_Click - Calling ShowDialog");
            var result = dialog.ShowDialog();
            System.Diagnostics.Debug.WriteLine($"SettingsPanel: BtnAddMcpServer_Click - ShowDialog returned {result}");
            if (result == DialogResult.OK)
            {
                _mcpServerCollection.Servers.Add(dialog.ServerConfig);
                SaveMcpServers();
                RefreshMcpServerList();
                AppendMcpLog($"Added server: {dialog.ServerConfig.Name}");
            }
            System.Diagnostics.Debug.WriteLine("SettingsPanel: BtnAddMcpServer_Click - Completed");
        }

        private void BtnEditMcpServer_Click(object? sender, EventArgs e)
        {
            if (dgvMcpServers.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a server to edit.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var server = dgvMcpServers.SelectedRows[0].Tag as McpServerConfig;
            if (server == null) return;

            System.Diagnostics.Debug.WriteLine("SettingsPanel: BtnEditMcpServer_Click - Creating dialog");
            using var dialog = new McpServerDialog(server);
            System.Diagnostics.Debug.WriteLine("SettingsPanel: BtnEditMcpServer_Click - Calling ShowDialog");
            var result = dialog.ShowDialog();
            System.Diagnostics.Debug.WriteLine($"SettingsPanel: BtnEditMcpServer_Click - ShowDialog returned {result}");
            if (result == DialogResult.OK)
            {
                SaveMcpServers();
                RefreshMcpServerList();
                AppendMcpLog($"Updated server: {server.Name}");
            }
            System.Diagnostics.Debug.WriteLine("SettingsPanel: BtnEditMcpServer_Click - Completed");
        }

        private void DgvMcpServers_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
        {
            // ヘッダー行のダブルクリックは無視
            if (e.RowIndex < 0)
            {
                return;
            }

            // ダブルクリックされた行を選択
            dgvMcpServers.ClearSelection();
            dgvMcpServers.Rows[e.RowIndex].Selected = true;

            // Editボタンと同等の処理を実行
            BtnEditMcpServer_Click(sender, e);
        }

        private void DgvMcpServers_KeyDown(object? sender, KeyEventArgs e)
        {
            // Deleteキーが押された場合
            if (e.KeyCode == Keys.Delete)
            {
                // Removeボタンと同等の処理を実行
                BtnRemoveMcpServer_Click(sender, e);
                e.Handled = true;
            }
        }

        private async void DgvMcpServers_CellContentClick(object? sender, DataGridViewCellEventArgs e)
        {
            // Check if the clicked cell is in the Enabled column
            if (e.RowIndex < 0 || e.ColumnIndex != 3) // Column 3 is the Enabled checkbox column
                return;

            // Get the server config from the row
            var row = dgvMcpServers.Rows[e.RowIndex];
            var server = row.Tag as McpServerConfig;
            if (server == null) return;

            // Commit the edit to get the new value
            dgvMcpServers.CommitEdit(DataGridViewDataErrorContexts.Commit);

            // Get the new checkbox value
            var isEnabled = (bool)(row.Cells[3].Value ?? false);

            // Update the server's enabled state
            server.IsEnabled = isEnabled;

            // Save the configuration
            SaveMcpServers();

            // Load or unload the server
            if (_mcpService != null)
            {
                try
                {
                    if (isEnabled)
                    {
                        AppendMcpLog($"Activating server: {server.Name}");
                        var success = await _mcpService.LoadServerAsync(server);
                        if (success)
                        {
                            AppendMcpLog($"Successfully activated: {server.Name}");
                        }
                        else
                        {
                            AppendMcpLog($"Failed to activate: {server.Name}");
                            // Revert checkbox if loading failed
                            server.IsEnabled = false;
                            row.Cells[3].Value = false;
                        }
                    }
                    else
                    {
                        AppendMcpLog($"Deactivating server: {server.Name}");
                        await _mcpService.UnloadServerAsync(server.Id);
                        AppendMcpLog($"Successfully deactivated: {server.Name}");
                    }

                    RefreshMcpServerList();

                    // Notify MainForm and WebView about the change
                    _mainForm?.NotifyMcpServersChanged();
                }
                catch (Exception ex)
                {
                    AppendMcpLog($"Error toggling server {server.Name}: {ex.Message}");
                    MessageBox.Show($"Error toggling server: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);

                    // Revert checkbox on error
                    server.IsEnabled = !isEnabled;
                    row.Cells[3].Value = !isEnabled;
                }
            }
        }

        private void BtnRemoveMcpServer_Click(object? sender, EventArgs e)
        {
            if (dgvMcpServers.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a server to remove.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var server = dgvMcpServers.SelectedRows[0].Tag as McpServerConfig;
            if (server == null) return;

            var result = MessageBox.Show($"Are you sure you want to remove '{server.Name}'?",
                "Confirm Remove", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                // Unload server if connected
                if (_mcpService?.IsServerLoaded(server.Id) ?? false)
                {
                    _mcpService.UnloadServerAsync(server.Id).GetAwaiter().GetResult();
                }

                _mcpServerCollection.Servers.Remove(server);
                SaveMcpServers();
                RefreshMcpServerList();
                AppendMcpLog($"Removed server: {server.Name}");
            }
        }


        private void AppendMcpLog(string message)
        {
            if (rtbMcpLog.InvokeRequired)
            {
                rtbMcpLog.Invoke(() => AppendMcpLog(message));
                return;
            }

            var timestamp = DateTime.Now.ToString("HH:mm:ss");
            rtbMcpLog.AppendText($"[{timestamp}] {message}\n");
            rtbMcpLog.ScrollToCaret();

            // Limit log size
            if (rtbMcpLog.Lines.Length > 1000)
            {
                var lines = rtbMcpLog.Lines.Skip(500).ToArray();
                rtbMcpLog.Lines = lines;
            }
        }

        private void UpdateServerStatus(McpServerEventArgs args)
        {
            if (dgvMcpServers.InvokeRequired)
            {
                dgvMcpServers.Invoke(() => UpdateServerStatus(args));
                return;
            }

            foreach (DataGridViewRow row in dgvMcpServers.Rows)
            {
                var server = row.Tag as McpServerConfig;
                if (server != null && server.Id == args.ServerId)
                {
                    row.Cells[1].Value = args.Status.ToString();
                    break;
                }
            }
        }

        public IMcpService? GetMcpService()
        {
            return _mcpService;
        }

        public List<McpServerConfig> GetMcpServerConfigs()
        {
            return _mcpServerCollection?.Servers ?? new List<McpServerConfig>();
        }

        public void SaveMcpServerConfigs()
        {
            SaveMcpServers();
        }

        public void RefreshMcpServerUI()
        {
            if (dgvMcpServers.InvokeRequired)
            {
                dgvMcpServers.Invoke(() => RefreshMcpServerUI());
                return;
            }
            RefreshMcpServerList();
        }

        #endregion

    }
}
