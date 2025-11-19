using folderchat.Models;
using folderchat.Services;
using folderchat.Services.Mcp;
using folderchat.Forms.ProviderSettings;
using System.Data;
using System.Text.Json;

namespace folderchat.Forms
{
    public partial class SettingsPanel : UserControl
    {
        private MainForm? _mainForm;
        private RagSettingsControl ragSettingsControl;
        private McpSettingsControl mcpSettingsControl;

        public SettingsPanel()
        {
            InitializeComponent();
            ragSettingsControl = new RagSettingsControl();
            this.tabPageRAG.Controls.Add(this.ragSettingsControl);
            this.ragSettingsControl.Dock = System.Windows.Forms.DockStyle.Fill;

            mcpSettingsControl = new McpSettingsControl();
            this.tabPageMCP.Controls.Add(this.mcpSettingsControl);
            this.mcpSettingsControl.Dock = System.Windows.Forms.DockStyle.Fill;
        }

        public void Initialize(MainForm mainForm)
        {
            _mainForm = mainForm;
            ragSettingsControl.Initialize(mainForm);
            mcpSettingsControl.Initialize(mainForm);
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
            ragSettingsControl.LoadSettings();

            // Load API Server settings
            chkEnableAPIServer.Checked = Properties.Settings.Default.EnableAPIServer;
            nudServerPort.Value = Properties.Settings.Default.APIServerPort == 0 ? 11550 : Properties.Settings.Default.APIServerPort;

            // Load chat method
            var chatMethod = Properties.Settings.Default.ChatMethod;
            if (chatMethod == "GGUF")
            {
                rbChatLocal.Checked = true;
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

            // Load Voice Input selection
            var voiceInputMode = Properties.Settings.Default.VoiceInputMode;
            cmbVoiceInput.SelectedItem = voiceInputMode;

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

            // Capture current RAG settings before saving
            var oldEmbeddingMethod = Properties.Settings.Default.EmbeddingMethod;
            var oldEmbeddingUrl = Properties.Settings.Default.Embedding_Url;
            var oldEmbeddingModel = Properties.Settings.Default.Embedding_Model;
            var oldEmbeddingGGUFModel = Properties.Settings.Default.EmbeddingGGUFModel;
            var oldUseNativeEmbedding = Properties.Settings.Default.UseNativeEmbedding;
            var oldRagContextLength = Properties.Settings.Default.RAG_ContextLength;
            var oldRagChunkSize = Properties.Settings.Default.RAG_ChunkSize;
            var oldRagChunkOverlap = Properties.Settings.Default.RAG_ChunkOverlap;
            var oldTopKChunks = Properties.Settings.Default.RAG_TopKChunks;
            var oldMaxContextLength = Properties.Settings.Default.RAG_MaxContextLength;

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
            ragSettingsControl.SaveSettings();

            // Save chat method
            var newChatMethod = rbChatLocal.Checked ? "GGUF" : "API";
            var newChatGGUFModel = cmbChatGGUFModel.SelectedItem?.ToString() ?? "";
            Properties.Settings.Default.ChatMethod = newChatMethod;
            Properties.Settings.Default.ChatGGUFModel = newChatGGUFModel;

            // Save API Server settings
            Properties.Settings.Default.APIServerPort = (int)nudServerPort.Value;

            // Save Voice Input selection
            var newVoiceInputMode = cmbVoiceInput.SelectedItem?.ToString() ?? "Disabled";
            Properties.Settings.Default.VoiceInputMode = newVoiceInputMode;

            // Save all settings to ensure they're persisted
            Properties.Settings.Default.Save();

            // Update microphone state in Chat.razor
            if (_mainForm != null)
            {
                _mainForm.UpdateVoiceInputState(newVoiceInputMode);
            }

            // Check if chat settings changed
            var newApiProvider = Properties.Settings.Default.API_Provider;
            bool chatSettingsChanged = oldApiProvider != newApiProvider ||
                                      oldChatMethod != newChatMethod ||
                                      (newChatMethod == "GGUF" && oldChatGGUFModel != newChatGGUFModel);

            // Check if we need to initialize or reinitialize Blazor WebView
            bool ragSettingsChanged =
                oldEmbeddingMethod != Properties.Settings.Default.EmbeddingMethod ||
                oldEmbeddingUrl != Properties.Settings.Default.Embedding_Url ||
                oldEmbeddingModel != Properties.Settings.Default.Embedding_Model ||
                oldEmbeddingGGUFModel != Properties.Settings.Default.EmbeddingGGUFModel ||
                oldUseNativeEmbedding != Properties.Settings.Default.UseNativeEmbedding ||
                oldRagContextLength != Properties.Settings.Default.RAG_ContextLength ||
                oldRagChunkSize != Properties.Settings.Default.RAG_ChunkSize ||
                oldRagChunkOverlap != Properties.Settings.Default.RAG_ChunkOverlap ||
                oldTopKChunks != Properties.Settings.Default.RAG_TopKChunks ||
                oldMaxContextLength != Properties.Settings.Default.RAG_MaxContextLength;

            bool needsInitialization = _mainForm.NeedsBlazorInitialization();
            bool needsReinitialization = (chatSettingsChanged || ragSettingsChanged) && !needsInitialization;

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
            lblTheme.Values.Text = loc.GetString("Theme") + ":";
            lblChatMethod.Values.Text = loc.GetString("ChatMethod") + ":";
            lblVoiceInput.Values.Text = loc.GetString("VoiceInput") + ":";
            lblServerPort.Values.Text = loc.GetString("ServerPort") + ":";

            // Update checkboxes
            chkEnableAPIServer.Values.Text = loc.GetString("EnableAPIServer");

            // Update radio buttons
            rbChatAPI.Values.Text = loc.GetString("API");
            rbChatLocal.Values.Text = loc.GetString("Local");

            // Update button texts
            btnSaveChatSettings.Values.Text = loc.GetString("Save");
            btnSaveRAGSettings.Values.Text = loc.GetString("Save");

            // Update MCP buttons
            mcpSettingsControl.UpdateUILabels();
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
            else if (rbChatLocal.Checked)
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

        private void rbChatAPI_CheckedChanged(object? sender, EventArgs e)
        {
            if (rbChatAPI.Checked)
            {
                UpdateChatMethodUI();
            }
        }

        private void rbChatLocal_CheckedChanged(object? sender, EventArgs e)
        {
            if (rbChatLocal.Checked)
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

        public IMcpService? GetMcpService()
        {
            return mcpSettingsControl.GetMcpService();
        }

        public List<McpServerConfig> GetMcpServerConfigs()
        {
            return mcpSettingsControl.GetMcpServerConfigs();
        }

        public void SaveMcpServerConfigs()
        {
            mcpSettingsControl.SaveMcpServerConfigs();
        }

        public void RefreshMcpServerUI()
        {
            mcpSettingsControl.RefreshMcpServerUI();
        }

        private void cmbVoiceInput_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_mainForm == null) return;

            var newVoiceInputMode = cmbVoiceInput.SelectedItem?.ToString() ?? "Disabled";
            Properties.Settings.Default.VoiceInputMode = newVoiceInputMode;
            Properties.Settings.Default.Save();

            _mainForm.UpdateVoiceInputState(newVoiceInputMode);
        }
    }
}
