using Microsoft.AspNetCore.Components.WebView.WindowsForms;
using folderchat.Pages;
using folderchat.Services;
using folderchat.Services.Mcp;
using folderchat.Models;
using Krypton.Toolkit;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace folderchat.Forms
{
    public partial class MainForm : KryptonForm
    {
        private SettingsPanel? _settingsPanel;
        private Pages.Chat? _chatComponent;
        private TreeNode? _rightClickedNode;
        private IndexingService? _indexingService;
        private ApiServerService? _apiServerService;
        private ToolStripMenuItem? menuItemShowFiles;
        private ToolStripMenuItem? menuItemOpenInExplorer;
        private bool _showFiles = false;

        // Event to notify Blazor about theme changes
        public event EventHandler<string>? ThemeChanged;

        // Event to notify Blazor about MCP server changes
        public event EventHandler? McpServersChanged;

        public bool IsDarkTheme
        {
            get
            {
                var mode = GetThemeMode();
                return mode == "dark" || mode == "dark-indigo";
            }
        }

        public string GetThemeMode()
        {
            try
            {
                // Get the selected theme name from KryptonThemeListBox
                if (_settingsPanel?.kryptonThemeListBox1.SelectedItem != null)
                {
                    var themeName = _settingsPanel.kryptonThemeListBox1.SelectedItem.ToString() ?? "";

                    // Check if theme name contains both "Blue" and "Dark" → Indigo dark mode
                    if (themeName.Contains("Blue", StringComparison.OrdinalIgnoreCase) &&
                        themeName.Contains("Dark", StringComparison.OrdinalIgnoreCase))
                    {
                        return "dark-indigo";
                    }

                    // Check if theme name contains "Black" or "Dark" → Regular dark mode
                    if (themeName.Contains("Black", StringComparison.OrdinalIgnoreCase) ||
                        themeName.Contains("Dark", StringComparison.OrdinalIgnoreCase))
                    {
                        return "dark";
                    }

                    // Otherwise → Light mode
                    return "light";
                }
                return "light";
            }
            catch
            {
                return "light";
            }
        }

        public MainForm()
        {
            InitializeComponent();
            InitializeSettingsPanel();
            InitializeContextMenu();
            InitializeTreeViewEvents();
            InitializeIndexingService();
            _apiServerService = new ApiServerService(this);
        }

        private void InitializeIndexingService()
        {
            _indexingService = new IndexingService(
                logMessage: LogRAGMessage,
                logError: LogError,
                updateStatus: (status) => toolStripStatusLabel1.Text = status,
                getEnabled: () => true,
                setEnabled: (enabled) => { }
            );
        }

        private void InitializeTreeViewEvents()
        {
            // Subscribe to mouse down event to capture right-clicked node
            ktvFolderTree.MouseDown += KtvFolderTree_MouseDown;

            // Also subscribe to context menu opening event as a fallback
            contextMenuTreeView.Opening += ContextMenuTreeView_Opening;
        }

        private void ContextMenuTreeView_Opening(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            // Get the current mouse position relative to the tree view
            var mousePosition = ktvFolderTree.PointToClient(Cursor.Position);
            var node = ktvFolderTree.GetNodeAt(mousePosition);

            if (node != null)
            {
                _rightClickedNode = node;
                ktvFolderTree.SelectedNode = node;
                //LogSystemMessage($"Context menu opening - node: {node.Text}, Tag: {node.Tag}");
            }
            else
            {
                //LogSystemMessage($"Context menu opening - no node at position ({mousePosition.X}, {mousePosition.Y})");
                // Cancel the context menu if no node is found
                e.Cancel = true;
            }
        }

        private void InitializeContextMenu()
        {
            // Add "Open in Explorer" menu item
            menuItemOpenInExplorer = new ToolStripMenuItem("Open in Explorer");
            menuItemOpenInExplorer.Click += new System.EventHandler(menuItemOpenInExplorer_Click);
            contextMenuTreeView.Items.Add(menuItemOpenInExplorer);

            // Add "Show Files" menu item
            menuItemShowFiles = new ToolStripMenuItem("Show Files");
            menuItemShowFiles.CheckOnClick = true;
            menuItemShowFiles.Checked = _showFiles;
            menuItemShowFiles.Click += new System.EventHandler(menuItemShowFiles_Click);
            contextMenuTreeView.Items.Add(new ToolStripSeparator());
            contextMenuTreeView.Items.Add(menuItemShowFiles);

            // Set up localization for context menu items
            if (Program.LocalizationService != null)
            {
                UpdateContextMenuLocalization();

                // Subscribe to culture changes
                Program.LocalizationService.CultureChanged += (sender, culture) =>
                {
                    UpdateContextMenuLocalization();
                    UpdateTabsLocalization();
                };
            }
            UpdateTabsLocalization(); // Initial call to set tab names
        }

        private void UpdateTabsLocalization()
        {
            if (Program.LocalizationService == null) return;

            tabPageDir.Text = Program.LocalizationService.GetString("TabDirToRag");
            tabPageSettings.Text = Program.LocalizationService.GetString("TabSettings");
        }

        private void menuItemShowFiles_Click(object? sender, EventArgs e)
        {
            if (menuItemShowFiles != null)
            {
                _showFiles = menuItemShowFiles.Checked;
                SaveTreeState();
                LoadFolderTree();
            }
        }

        private void UpdateContextMenuLocalization()
        {
            menuItemIndexing.Text = Program.LocalizationService.GetString("Indexing");
            menuItemSummarize.Text = Program.LocalizationService.GetString("Summarize");
            menuItemRefresh.Text = Program.LocalizationService.GetString("Refresh");
            menuItemShowFiles.Text = Program.LocalizationService.GetString("ShowFiles");
            menuItemOpenInExplorer.Text = Program.LocalizationService.GetString("OpenInExplorer");
        }

        private void InitializeSettingsPanel()
        {
            _settingsPanel = new SettingsPanel();
            _settingsPanel.Dock = DockStyle.Fill;
            tabPageSettings.Controls.Add(_settingsPanel);
            _settingsPanel.Initialize(this);
        }

        private bool _isDragging = false;
        private int _lastY = 0;
        private int _savedLogViewHeight = 131; // Default height

        private void KSeparatorForLogView_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _isDragging = true;
                _lastY = e.Y + kSeparatorForLogView.Top;
            }
        }

        private void KSeparatorForLogView_MouseMove(object? sender, MouseEventArgs e)
        {
            if (_isDragging)
            {
                int currentY = e.Y + kSeparatorForLogView.Top;
                int delta = currentY - _lastY;

                int newLogViewHeight = kListViewBottomLog.Height - delta;

                // Set minimum and maximum height
                int minHeight = 30;
                int maxHeight = this.ClientSize.Height - kryptonStatusStrip1.Height - 100;

                if (newLogViewHeight >= minHeight && newLogViewHeight <= maxHeight)
                {
                    kListViewBottomLog.Height = newLogViewHeight;
                    _lastY = currentY;
                    _savedLogViewHeight = newLogViewHeight;
                }
            }
        }

        private void KSeparatorForLogView_MouseUp(object? sender, MouseEventArgs e)
        {
            _isDragging = false;
        }

        private void kSeparatorForLogView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (kListViewBottomLog.Height > 0)
            {
                // Hide log view
                _savedLogViewHeight = kListViewBottomLog.Height;
                kListViewBottomLog.Height = 0;
            }
            else
            {
                // Show log view
                kListViewBottomLog.Height = _savedLogViewHeight;
            }
        }

        private void kListViewBottomLog_DoubleClick(object? sender, EventArgs e)
        {
                        if (kListViewBottomLog.SelectedItems.Count == 0)
                return;

            if (kListViewBottomLog.SelectedItems[0] is LogListViewItem item)
            {
                var date = item.SubItems[0].Text;
                var type = item.SubItems[1].Text;
                var message = item.OriginalMessage; // Get the original message

                // Create and show modeless dialog
                var detailForm = new LogDetailForm(date, type, message);
                detailForm.Show(this);
            }
        }

        public void LogRAGMessage(string message)
        {
            LogMessage("RAG", message);
        }

        /// <summary>
        /// Logs a system message to the bottom log view
        /// </summary>
        public void LogSystemMessage(string message)
        {
            LogMessage("System", message);
        }

        /// <summary>
        /// Logs a user message to the bottom log view
        /// </summary>
        public void LogUserMessage(string message)
        {
            LogMessage("User", message);
        }

        /// <summary>
        /// Logs an error message to the bottom log view
        /// </summary>
        public void LogError(string message)
        {
            LogMessage("Error", message);
        }

        /// <summary>
        /// Logs a chat message with role information to the bottom log view
        /// </summary>
        public void LogChatMessage(string role, string message)
        {
            LogMessage($"Role:{role}", message);
        }

        /// <summary>
        /// Logs a message to the bottom log view
        /// </summary>
        public void LogMessage(string type, string message)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(() => LogMessage(type, message));
                return;
            }

            try
            {
                // Limit log size to prevent memory issues (keep last 1000 items)
                // Check BEFORE adding to prevent exceeding the limit
                if (kListViewBottomLog.Items.Count >= 1000)
                {
                    kListViewBottomLog.Items.RemoveAt(0);
                }

                var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                // Replace newlines with spaces for single-line display in the list view
                var displayMessage = message.Replace('\n', ' ').Replace('\r', ' ');

                // Use the custom LogListViewItem to store the original message
                var item = new LogListViewItem(new[] { timestamp, type, displayMessage }, message);

                kListViewBottomLog.Items.Add(item);

                // Auto-scroll to the latest item
                kListViewBottomLog.EnsureVisible(kListViewBottomLog.Items.Count - 1);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in LogMessage: {ex.Message}");
            }
        }

        private async void MainForm_Load(object? sender, EventArgs e)
        {
            LogSystemMessage("Application started");

            RestoreWindowPlacement();

            //LoadSettings();
            LoadFolderTree();

            // Always initialize Blazor WebView so the chat UI renders even before configuration
            LogSystemMessage("Initializing Blazor WebView");
            InitializeBlazorWebView();

            if (!IsConfigured())
            {
                // Show settings tab to allow user to configure
                ShowSettingsDialog();

                // Show a message to guide the user
                LogSystemMessage("Configuration required - showing settings dialog");
                MessageBox.Show("Please configure your API settings in the Settings tab before using the chat.",
                    "Configuration Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                // Auto-load enabled MCP servers
                await LoadEnabledMcpServersAsync();
            }

            // Start API server if enabled
            if (Properties.Settings.Default.EnableAPIServer)
            {
                if (_apiServerService != null)
                {
                    _ = _apiServerService.StartApiServer(); // Fire-and-forget to not block the UI thread
                }
            }
            else
            {
                toolStripStatusLabelApiStatus.Text = "API Server: Disabled";
            }
        }

        private async Task LoadEnabledMcpServersAsync()
        {
            try
            {
                var mcpService = GetMcpService();
                if (mcpService == null)
                {
                    LogSystemMessage("MCP service not available");
                    return;
                }

                var mcpServers = GetMcpServerConfigs();
                var enabledServers = mcpServers.Where(s => s.IsEnabled).ToList();

                if (enabledServers.Count > 0)
                {
                    LogSystemMessage($"Auto-loading {enabledServers.Count} enabled MCP server(s)...");

                    foreach (var server in enabledServers)
                    {
                        LogSystemMessage($"Loading MCP server: {server.Name}");
                        var success = await mcpService.LoadServerAsync(server);
                        if (success)
                        {
                            LogSystemMessage($"Successfully loaded: {server.Name}");
                        }
                        else
                        {
                            LogError($"Failed to load: {server.Name}");
                        }
                    }

                    LogSystemMessage("MCP server auto-load completed");
                }
            }
            catch (Exception ex)
            {
                LogError($"Error auto-loading MCP servers: {ex.Message}");
            }
        }

        public bool NeedsBlazorInitialization()
        {
            return blazorWebView1.Services == null;
        }

                        public void ReinitializeBlazorWebView()
        {
            LogSystemMessage("Reinitializing Blazor WebView by recreating the control.");

            // 親コントロールから古いWebViewを削除
            if (blazorWebView1 != null)
            {
                splitContainerMain.Panel2.Controls.Remove(blazorWebView1);
                blazorWebView1.Dispose();
            }

            // 新しいBlazorWebViewインスタンスを作成
            blazorWebView1 = new Microsoft.AspNetCore.Components.WebView.WindowsForms.BlazorWebView();
            blazorWebView1.Dock = DockStyle.Fill;
            blazorWebView1.Location = new Point(0, 0);
            blazorWebView1.Name = "blazorWebView1";
            blazorWebView1.TabIndex = 0;

            // 親コントロールに新しいWebViewを追加
            splitContainerMain.Panel2.Controls.Add(blazorWebView1);

            // 新しいWebViewを初期化
            InitializeBlazorWebView();
        }

        private void InitializeBlazorWebView()
        {
            // Set HostPage first, before any other operations
            if (string.IsNullOrEmpty(blazorWebView1.HostPage))
            {
                blazorWebView1.HostPage = "wwwroot/index.html";
            }

            var services = new ServiceCollection();
            services.AddWindowsFormsBlazorWebView();

            // Add this form as a singleton so it can be injected
            services.AddSingleton(this);

            // Add localization service
            services.AddSingleton(Program.LocalizationService!);

            // Add RAG service
            services.AddScoped<IRagService>(serviceProvider =>
            {
                var embeddingUrl = Properties.Settings.Default.Embedding_Url;
                var apiKey = Properties.Settings.Default.OpenAI_ApiKey;
                var embeddingModel = Properties.Settings.Default.Embedding_Model;
                var useNativeEmbedding = Properties.Settings.Default.UseNativeEmbedding;
                var ggufModel = Properties.Settings.Default.EmbeddingGGUFModel;
                var contextLength = Properties.Settings.Default.RAG_ContextLength;
                var chunkSize = Properties.Settings.Default.RAG_ChunkSize;
                var chunkOverlap = Properties.Settings.Default.RAG_ChunkOverlap;

                return new RagService(embeddingUrl, apiKey, embeddingModel, useNativeEmbedding,
                    contextLength, chunkSize, chunkOverlap, ggufModel);
            });

            // Choose the chat service based on the selected chat method
            var chatMethod = Properties.Settings.Default.ChatMethod;
            var apiProvider = Properties.Settings.Default.API_Provider;

            services.AddScoped<IChatService>(serviceProvider =>
            {
                try
                {
                    IChatService innerService;

                    if (chatMethod == "GGUF")
                    {
                        // Use GGUF local model
                        var chatGGUFModel = Properties.Settings.Default.ChatGGUFModel;

                        if (string.IsNullOrEmpty(chatGGUFModel))
                        {
                            throw new InvalidOperationException("Chat GGUF model is not configured. Please select a GGUF model in Settings.");
                        }

                        // Build full path to model file
                        // chatGGUFModel is in format: "provider_name/model.gguf"
                        var modelPath = Path.Combine(
                            PythonPathHelper.PythonToolsDirectory,
                            "models",
                            "chat",
                            chatGGUFModel.Replace("/", Path.DirectorySeparatorChar.ToString())
                        );

                        if (!File.Exists(modelPath))
                        {
                            throw new FileNotFoundException($"GGUF model file not found at: {modelPath}");
                        }

                        innerService = new GGUFChatService(modelPath);
                    }
                    else if (apiProvider == "Claude Code")
                    {
                        var cliPath = Properties.Settings.Default.ClaudeCode_CLIPath;
                        var model = Properties.Settings.Default.ClaudeCode_Model;
                        if (string.IsNullOrEmpty(cliPath) || string.IsNullOrEmpty(model))
                            throw new InvalidOperationException("Claude Code CLI path or model is not configured.");
                        innerService = new ClaudeCodeChatService(cliPath, model);
                    }
                    else if (apiProvider == "Azure AI Foundry")
                    {
                        var azureApiUrl = Properties.Settings.Default.AzureAI_ApiUrl;
                        var azureApiKey = Properties.Settings.Default.AzureAI_ApiKey;
                        var azureModel = Properties.Settings.Default.AzureAI_SelectedModel;
                        var azureApiVersion = Properties.Settings.Default.AzureAI_ApiVersion;
                        if (string.IsNullOrEmpty(azureApiUrl) || string.IsNullOrEmpty(azureApiKey) || string.IsNullOrEmpty(azureModel) || azureApiUrl == "https://YOUR_RESOURCE_NAME.openai.azure.com")
                            throw new InvalidOperationException("Azure AI Foundry settings are not fully configured.");
                        innerService = new AzureOpenAIChatService(azureApiUrl, azureApiKey, azureModel, azureApiVersion);
                    }
                    else if (apiProvider == "OpenRouter")
                    {
                        var openRouterApiUrl = Properties.Settings.Default.OpenRouter_ApiUrl;
                        var openRouterApiKey = Properties.Settings.Default.OpenRouter_ApiKey;
                        var openRouterModel = Properties.Settings.Default.OpenRouter_SelectedModel;
                        if (string.IsNullOrEmpty(openRouterApiKey) || string.IsNullOrEmpty(openRouterModel))
                            throw new InvalidOperationException("OpenRouter settings are not fully configured.");
                        innerService = new OpenRouterChatService(openRouterApiUrl, openRouterApiKey, openRouterModel);
                    }
                    else if (apiProvider == "Ollama")
                    {
                        var ollamaApiUrl = Properties.Settings.Default.Ollama_ApiUrl;
                        var ollamaModel = Properties.Settings.Default.Ollama_SelectedModel;
                        if (string.IsNullOrEmpty(ollamaApiUrl) || string.IsNullOrEmpty(ollamaModel))
                            throw new InvalidOperationException("Ollama settings are not fully configured.");
                        innerService = new OpenAIChatService(ollamaApiUrl, "", ollamaModel);
                    }
                    else if (apiProvider == "OpenAI")
                    {
                        var openAIApiUrl = Properties.Settings.Default.OpenAIAPI_ApiUrl;
                        var openAIApiKey = Properties.Settings.Default.OpenAIAPI_ApiKey;
                        var openAIModel = Properties.Settings.Default.OpenAIAPI_SelectedModel;
                        if (string.IsNullOrEmpty(openAIApiKey) || string.IsNullOrEmpty(openAIModel))
                            throw new InvalidOperationException("OpenAI settings are not fully configured.");
                        innerService = new OpenAIChatService(openAIApiUrl, openAIApiKey, openAIModel);
                    }
                    else if (apiProvider == "Gemini")
                    {
                        var geminiApiUrl = Properties.Settings.Default.Gemini_ApiUrl;
                        var geminiApiKey = Properties.Settings.Default.Gemini_ApiKey;
                        var geminiModel = Properties.Settings.Default.Gemini_SelectedModel;
                        if (string.IsNullOrEmpty(geminiApiKey) || string.IsNullOrEmpty(geminiModel))
                            throw new InvalidOperationException("Gemini settings are not fully configured.");
                        innerService = new OpenAIChatService(geminiApiUrl, geminiApiKey, geminiModel);
                    }
                    else if (apiProvider == "Claude")
                    {
                        var claudeApiUrl = Properties.Settings.Default.Claude_ApiUrl;
                        var claudeApiKey = Properties.Settings.Default.Claude_ApiKey;
                        var claudeModel = Properties.Settings.Default.Claude_SelectedModel;
                        if (string.IsNullOrEmpty(claudeApiKey) || string.IsNullOrEmpty(claudeModel))
                            throw new InvalidOperationException("Claude settings are not fully configured.");
                        innerService = new OpenAIChatService(claudeApiUrl, claudeApiKey, claudeModel);
                    }
                    else // OpenAI Compatible
                    {
                        var baseUrl = Properties.Settings.Default.OpenAI_BaseUrl;
                        var apiKey = Properties.Settings.Default.OpenAI_ApiKey;
                        var model = Properties.Settings.Default.OpenAI_SelectedModel;
                        if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(model))
                            throw new InvalidOperationException("OpenAI Compatible settings are not fully configured.");
                        innerService = new OpenAIChatService(baseUrl, apiKey, model);
                    }

                    var mainForm = serviceProvider.GetRequiredService<MainForm>();
                    var useRag = Properties.Settings.Default.UseRAG;

                    // Get the SupportsSystemRole flag for the current provider
                    bool supportsSystemMessage = apiProvider switch
                    {
                        "Claude Code" => Properties.Settings.Default.ClaudeCode_SupportsSystemRole,
                        "Azure AI Foundry" => Properties.Settings.Default.AzureAI_SupportsSystemRole,
                        "OpenRouter" => Properties.Settings.Default.OpenRouter_SupportsSystemRole,
                        "Ollama" => Properties.Settings.Default.Ollama_SupportsSystemRole,
                        "OpenAI" => Properties.Settings.Default.OpenAIAPI_SupportsSystemRole,
                        "Gemini" => Properties.Settings.Default.Gemini_SupportsSystemRole,
                        "Claude" => Properties.Settings.Default.Claude_SupportsSystemRole,
                        _ => Properties.Settings.Default.OpenAI_SupportsSystemRole // OpenAI Compatible
                    };

                    IChatService finalService = innerService;

                    if (useRag)
                    {
                        // Wrap the inner service with RAG capabilities
                        var ragService = serviceProvider.GetRequiredService<IRagService>();

                        // Use supportsSystemMessage flag to determine whether to use context in system message
                        finalService = new RagEnabledChatService(innerService, ragService, mainForm, supportsSystemMessage);
                    }
                    else
                    {
                        // Wrap with MCP capabilities (only when RAG is OFF)
                        var mcpService = mainForm.GetMcpService();
                        var maxToolIterations = Properties.Settings.Default.MCP_MaxToolIterations;
                        finalService = new McpEnabledChatService(innerService, mcpService, supportsSystemMessage, maxToolIterations);

                        // Set MainForm for logging
                        McpEnabledChatService.SetMainForm(mainForm);
                    }

                    return finalService;
                }
                catch (Exception ex)
                {
                    var mainForm = serviceProvider.GetRequiredService<MainForm>();
                    mainForm.LogError($"Failed to initialize chat service: {ex.Message}");
                    
                    var localizationService = serviceProvider.GetRequiredService<ILocalizationService>();
                    var errorMessage = localizationService.GetString("ChatServiceInitializationError");
                    return new DummyChatService(errorMessage);
                }
            });

            // Build and set the service provider
            blazorWebView1.Services = services.BuildServiceProvider();

            // Add root component only if not already added
            if (blazorWebView1.RootComponents.Count == 0)
            {
                blazorWebView1.RootComponents.Add<Chat>("#app");
            }
        }

        public bool IsConfigured()
        {
            var apiProvider = Properties.Settings.Default.API_Provider;

            if (apiProvider == "Claude Code")
            {
                var claudePath = Properties.Settings.Default.ClaudeCode_CLIPath;
                var claudeModel = Properties.Settings.Default.ClaudeCode_Model;
                return !string.IsNullOrEmpty(claudePath) && !string.IsNullOrEmpty(claudeModel);
            }
            else if (apiProvider == "Azure AI Foundry")
            {
                var azureApiUrl = Properties.Settings.Default.AzureAI_ApiUrl;
                var azureApiKey = Properties.Settings.Default.AzureAI_ApiKey;
                var azureModel = Properties.Settings.Default.AzureAI_SelectedModel;
                return !string.IsNullOrEmpty(azureApiUrl) &&
                       !string.IsNullOrEmpty(azureApiKey) &&
                       !string.IsNullOrEmpty(azureModel) &&
                       azureApiUrl != "https://YOUR_RESOURCE_NAME.openai.azure.com";
            }
            else if (apiProvider == "OpenRouter")
            {
                var openRouterApiKey = Properties.Settings.Default.OpenRouter_ApiKey;
                var openRouterModel = Properties.Settings.Default.OpenRouter_SelectedModel;
                return !string.IsNullOrEmpty(openRouterApiKey) && !string.IsNullOrEmpty(openRouterModel);
            }
            else if (apiProvider == "Ollama")
            {
                var ollamaApiUrl = Properties.Settings.Default.Ollama_ApiUrl;
                var ollamaModel = Properties.Settings.Default.Ollama_SelectedModel;
                return !string.IsNullOrEmpty(ollamaApiUrl) && !string.IsNullOrEmpty(ollamaModel);
            }
            else if (apiProvider == "OpenAI")
            {
                var openAIApiKey = Properties.Settings.Default.OpenAIAPI_ApiKey;
                var openAIModel = Properties.Settings.Default.OpenAIAPI_SelectedModel;
                return !string.IsNullOrEmpty(openAIApiKey) && !string.IsNullOrEmpty(openAIModel);
            }
            else if (apiProvider == "Gemini")
            {
                var geminiApiKey = Properties.Settings.Default.Gemini_ApiKey;
                var geminiModel = Properties.Settings.Default.Gemini_SelectedModel;
                return !string.IsNullOrEmpty(geminiApiKey) && !string.IsNullOrEmpty(geminiModel);
            }
            else if (apiProvider == "Claude")
            {
                var claudeApiKey = Properties.Settings.Default.Claude_ApiKey;
                var claudeModel = Properties.Settings.Default.Claude_SelectedModel;
                return !string.IsNullOrEmpty(claudeApiKey) && !string.IsNullOrEmpty(claudeModel);
            }
            else // OpenAI Compatible
            {
                var apiKey = Properties.Settings.Default.OpenAI_ApiKey;
                var model = Properties.Settings.Default.OpenAI_SelectedModel;
                return !string.IsNullOrEmpty(apiKey) && !string.IsNullOrEmpty(model);
            }
        }

        private void ShowSettingsDialog()
        {
            tabMain.SelectedIndex = 1;
        }


        private void LoadFolderTree()
        {
            ktvFolderTree.BeginUpdate();
            ktvFolderTree.Nodes.Clear();

            foreach (var drive in DriveInfo.GetDrives())
            {
                if (drive.IsReady)
                {
                    var driveNode = new TreeNode(drive.Name)
                    {
                        Tag = drive.RootDirectory.FullName
                    };
                    driveNode.Nodes.Add(new TreeNode());
                    ktvFolderTree.Nodes.Add(driveNode);
                }
            }

            // Only register BeforeExpand event once
            ktvFolderTree.BeforeExpand -= KtvFolderTree_BeforeExpand;
            ktvFolderTree.BeforeExpand += KtvFolderTree_BeforeExpand;
            ktvFolderTree.EndUpdate();

            RestoreTreeState();
        }

        private void KtvFolderTree_MouseDown(object? sender, MouseEventArgs e)
        {
        }

        private void KtvFolderTree_BeforeExpand(object? sender, TreeViewCancelEventArgs e)
        {
            if (e.Node == null || e.Node.Nodes.Count != 1 || e.Node.Nodes[0].Tag != null)
                return;

            e.Node.Nodes.Clear();

            var directoryPath = e.Node.Tag?.ToString();
            if (string.IsNullOrEmpty(directoryPath))
                return;

            try
            {
                var directoryInfo = new DirectoryInfo(directoryPath);
                foreach (var directory in directoryInfo.GetDirectories())
                {
                    try
                    {
                        var dirNode = new TreeNode(directory.Name)
                        {
                            Tag = directory.FullName
                        };
                        dirNode.Nodes.Add(new TreeNode());
                        e.Node.Nodes.Add(dirNode);
                    }
                    catch
                    {
                    }
                }

                if (_showFiles)
                {
                    foreach (var file in directoryInfo.GetFiles())
                    {
                        try
                        {
                            var fileNode = new TreeNode(file.Name)
                            {
                                Tag = file.FullName
                            };
                            e.Node.Nodes.Add(fileNode);
                        }
                        catch
                        {
                        }
                    }
                }
            }
            catch
            {
            }
        }

        public List<string> GetCheckedFolders()
        {
            var checkedFolders = new List<string>();
            GetCheckedFoldersRecursive(ktvFolderTree.Nodes, checkedFolders);
            return checkedFolders;
        }

        private void GetCheckedFoldersRecursive(TreeNodeCollection nodes, List<string> checkedFolders)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.Checked && node.Tag != null)
                {
                    checkedFolders.Add(node.Tag.ToString()!);
                }
                if (node.Nodes.Count > 0)
                {
                    GetCheckedFoldersRecursive(node.Nodes, checkedFolders);
                }
            }
        }

        public async Task Indexing()
        {
            SaveTreeState();

            var checkedFolders = GetCheckedFolders();

            if (_indexingService != null)
            {
                await _indexingService.IndexAsync(checkedFolders);
            }
        }

        private void SaveTreeState()
        {
            try
            {
                var treeState = new Dictionary<string, TreeNodeState>();
                SerializeTreeState(ktvFolderTree.Nodes, treeState);
                var json = JsonSerializer.Serialize(treeState);
                Properties.Settings.Default.FolderTree_State = json;
                Properties.Settings.Default.Save();
            }
            catch
            {
            }
        }

        private void SerializeTreeState(TreeNodeCollection nodes, Dictionary<string, TreeNodeState> treeState)
        {
            foreach (TreeNode node in nodes)
            {
                var path = node.Tag?.ToString();
                if (!string.IsNullOrEmpty(path))
                {
                    treeState[path] = new TreeNodeState
                    {
                        IsChecked = node.Checked,
                        IsExpanded = node.IsExpanded
                    };

                    if (node.Nodes.Count > 0)
                    {
                        SerializeTreeState(node.Nodes, treeState);
                    }
                }
            }
        }

                private async void MainForm_FormClosing(object sender, FormClosingEventArgs e)
                {
                    SaveWindowPlacement();
                    SaveTreeState();
        
                    if (_apiServerService != null && _apiServerService.IsRunning)
                    {
                        await _apiServerService.StopApiServer();
                    }
                }
        private void RestoreTreeState()
        {
            try
            {
                var json = Properties.Settings.Default.FolderTree_State;
                if (string.IsNullOrEmpty(json))
                    return;

                var treeState = JsonSerializer.Deserialize<Dictionary<string, TreeNodeState>>(json);
                if (treeState == null)
                    return;

                DeserializeTreeState(ktvFolderTree.Nodes, treeState);
            }
            catch
            {
            }
        }

        private void DeserializeTreeState(TreeNodeCollection nodes, Dictionary<string, TreeNodeState> treeState)
        {
            foreach (TreeNode node in nodes)
            {
                var path = node.Tag?.ToString();
                if (!string.IsNullOrEmpty(path) && treeState.TryGetValue(path, out var state))
                {
                    node.Checked = state.IsChecked;

                    if (state.IsExpanded && node.Nodes.Count > 0)
                    {
                        node.Expand();

                        if (node.Nodes.Count > 0 && node.Nodes[0].Tag != null)
                        {
                            DeserializeTreeState(node.Nodes, treeState);
                        }
                    }
                }
            }
        }

        private class TreeNodeState
        {
            public bool IsChecked { get; set; }
            public bool IsExpanded { get; set; }
        }

        // --- Window placement persistence ---
        private void SaveWindowPlacement()
        {
            try
            {
                Properties.Settings.Default.MainWindow_State = this.WindowState == FormWindowState.Maximized ? "Maximized" : "Normal";

                var bounds = this.WindowState == FormWindowState.Normal ? this.Bounds : this.RestoreBounds;

                Properties.Settings.Default.MainWindow_Left = bounds.Left;
                Properties.Settings.Default.MainWindow_Top = bounds.Top;
                Properties.Settings.Default.MainWindow_Width = bounds.Width;
                Properties.Settings.Default.MainWindow_Height = bounds.Height;

                Properties.Settings.Default.Save();
            }
            catch
            {
            }
        }

        private void RestoreWindowPlacement()
        {
            try
            {
                var state = Properties.Settings.Default.MainWindow_State;
                int left = Properties.Settings.Default.MainWindow_Left;
                int top = Properties.Settings.Default.MainWindow_Top;
                int width = Properties.Settings.Default.MainWindow_Width;
                int height = Properties.Settings.Default.MainWindow_Height;

                if (width <= 0 || height <= 0)
                {
                    var screen = System.Windows.Forms.Screen.PrimaryScreen?.WorkingArea ?? new System.Drawing.Rectangle(0, 0, 1024, 768);
                    width = Math.Min(1200, Math.Max(800, screen.Width * 2 / 3));
                    height = Math.Min(800, Math.Max(600, screen.Height * 2 / 3));
                }

                var target = new System.Drawing.Rectangle(
                    left > 0 ? left : this.Left,
                    top > 0 ? top : this.Top,
                    width,
                    height
                );

                var visible = EnsureVisibleOnAnyScreen(target);

                this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
                this.Bounds = visible;

                if (string.Equals(state, "Maximized", StringComparison.OrdinalIgnoreCase))
                {
                    this.WindowState = FormWindowState.Maximized;
                }
                else
                {
                    this.WindowState = FormWindowState.Normal;
                }
            }
            catch
            {
            }
        }

        private System.Drawing.Rectangle EnsureVisibleOnAnyScreen(System.Drawing.Rectangle bounds)
        {
            foreach (var screen in System.Windows.Forms.Screen.AllScreens)
            {
                var wa = screen.WorkingArea;
                if (wa.IntersectsWith(bounds))
                {
                    var x = Math.Max(wa.Left, Math.Min(bounds.Left, wa.Right - Math.Max(100, bounds.Width)));
                    var y = Math.Max(wa.Top, Math.Min(bounds.Top, wa.Bottom - Math.Max(100, bounds.Height)));
                    var w = Math.Min(bounds.Width, wa.Width);
                    var h = Math.Min(bounds.Height, wa.Height);
                    return new System.Drawing.Rectangle(x, y, w, h);
                }
            }

            var primary = System.Windows.Forms.Screen.PrimaryScreen?.WorkingArea ?? new System.Drawing.Rectangle(0, 0, 1024, 768);
            var w2 = Math.Min(bounds.Width, primary.Width);
            var h2 = Math.Min(bounds.Height, primary.Height);
            var x2 = primary.Left + (primary.Width - w2) / 2;
            var y2 = primary.Top + (primary.Height - h2) / 2;
            return new System.Drawing.Rectangle(x2, y2, w2, h2);
        }

        public void OnThemeChanged()
        {
            // Notify Blazor about theme change with theme mode
            ThemeChanged?.Invoke(this, GetThemeMode());
        }

        public void RegisterChatComponent(Pages.Chat chatComponent)
        {
            _chatComponent = chatComponent;
        }

        public IMcpService? GetMcpService()
        {
            return _settingsPanel?.GetMcpService();
        }

        public List<McpServerConfig> GetMcpServerConfigs()
        {
            return _settingsPanel?.GetMcpServerConfigs() ?? new List<McpServerConfig>();
        }

        public void SaveMcpServerConfigs()
        {
            _settingsPanel?.SaveMcpServerConfigs();
        }

        public void NotifyMcpServersChanged()
        {
            McpServersChanged?.Invoke(this, EventArgs.Empty);
        }

        public void RefreshMcpServerUI()
        {
            _settingsPanel?.RefreshMcpServerUI();
        }

        // Context menu event handlers
        private async void menuItemIndexing_Click(object sender, EventArgs e)
        {
            await Indexing();

            try
            {
                var checkedFolders = GetCheckedFolders();
                if (checkedFolders.Count > 0 && !Properties.Settings.Default.UseRAG)
                {
                    Properties.Settings.Default.UseRAG = true;
                    Properties.Settings.Default.Save();
                    LogSystemMessage("UseRAG toggled ON automatically after indexing because folders are selected.");
                    ReinitializeBlazorWebView();
                }
            }
            catch (Exception ex)
            {
                LogError($"Failed to auto-enable RAG after indexing: {ex.Message}");
            }

            MessageBox.Show("RAG processing completed successfully!", "Success",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void menuItemRefresh_Click(object sender, EventArgs e)
        {
            LoadFolderTree();
        }

        private async void menuItemSummarize_Click(object sender, EventArgs e)
        {
            await SummarizeSelectedFolder();
        }

        private void menuItemOpenInExplorer_Click(object? sender, EventArgs e)
        {
            try
            {
                var selectedPath = _rightClickedNode?.Tag?.ToString();
                if (string.IsNullOrEmpty(selectedPath))
                {
                    MessageBox.Show("フォルダまたはファイルが選択されていません。", "エラー",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // 開く対象がファイルかフォルダかを判定
                if (File.Exists(selectedPath))
                {
                    // ファイルを選択状態でエクスプローラーを開く
                    var psi = new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = "explorer.exe",
                        Arguments = $"/select,\"{selectedPath}\"",
                        UseShellExecute = true
                    };
                    System.Diagnostics.Process.Start(psi);
                }
                else if (Directory.Exists(selectedPath))
                {
                    // フォルダを開く
                    var psi = new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = "explorer.exe",
                        Arguments = $"\"{selectedPath}\"",
                        UseShellExecute = true
                    };
                    System.Diagnostics.Process.Start(psi);
                }
                else
                {
                    MessageBox.Show($"指定されたパスが存在しません:\n{selectedPath}", "エラー",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                LogError($"Failed to open Explorer: {ex.Message}");
                MessageBox.Show("エクスプローラーを開けませんでした。", "エラー",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task SummarizeSelectedFolder()
        {
            LogSystemMessage($"SummarizeSelectedFolder called. _rightClickedNode is {(_rightClickedNode == null ? "null" : $"'{_rightClickedNode.Text}'")}");

            // Get the right-clicked node from the tree view
            if (_rightClickedNode == null)
            {
                LogError("_rightClickedNode is null - no folder was right-clicked");
                MessageBox.Show("Please right-click on a folder to summarize.", "No Folder Selected",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedPath = _rightClickedNode.Tag?.ToString();
            if (string.IsNullOrEmpty(selectedPath))
            {
                MessageBox.Show("Invalid folder selection.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            LogSystemMessage($"Right-clicked folder: {_rightClickedNode.Text} -> Path: {selectedPath}");

            // Check if chat component is available
            if (_chatComponent == null)
            {
                MessageBox.Show("Chat component is not initialized. Please wait for the application to fully load.",
                    "Not Ready", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            LogSystemMessage($"Starting summarization for: {selectedPath}");
            toolStripStatusLabel1.Text = "Summarizing...";

            // Switch to chat tab to show the conversation
            if (InvokeRequired)
            {
                Invoke(() => tabMain.Parent.Controls[0].Focus());
            }

            try
            {
                // Check if API is configured
                if (!IsConfigured())
                {
                    MessageBox.Show("Please configure your API settings in the Settings tab before using the summarization feature.",
                        "Configuration Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Verify the directory exists
                if (!Directory.Exists(selectedPath))
                {
                    LogError($"Directory does not exist: {selectedPath}");
                    MessageBox.Show($"The selected directory does not exist:\n{selectedPath}", "Directory Not Found",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Get all files from the selected folder
                var directoryInfo = new DirectoryInfo(selectedPath);
                LogSystemMessage($"Searching for files in: {directoryInfo.FullName}");

                FileInfo[] files;
                try
                {
                    files = directoryInfo.GetFiles("*.*", SearchOption.AllDirectories);
                    LogSystemMessage($"Found {files.Length} files in total");
                }
                catch (UnauthorizedAccessException ex)
                {
                    LogError($"Access denied to directory: {ex.Message}");
                    MessageBox.Show($"Access denied to one or more directories:\n{ex.Message}", "Access Denied",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (files.Length == 0)
                {
                    LogSystemMessage($"No files found in directory: {directoryInfo.FullName}");
                    MessageBox.Show($"No files found in the selected folder:\n{selectedPath}", "No Files",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                LogSystemMessage($"Found {files.Length} files to process");

                // Process files that need conversion to markdown
                var pythonInterop = new PythonInteropService();
                var supportedTextExtensions = new[] { ".txt", ".md", ".py", ".js", ".ts", ".jsx", ".tsx",
                    ".json", ".yaml", ".yml", ".toml", ".ini", ".cfg",
                    ".html", ".css", ".xml", ".csv", ".sql", ".sh",
                    ".bat", ".ps1", ".c", ".cpp", ".h", ".java", ".cs",
                    ".go", ".rs", ".rb", ".php", ".swift", ".kt", ".scala" };
                var needsConversionExtensions = new[] { ".pdf", ".docx", ".xlsx", ".pptx", ".doc", ".xls", ".ppt" };

                // Convert documents to markdown if necessary
                var allContent = new System.Text.StringBuilder();
                int processedFileCount = 0;
                int skippedFileCount = 0;

                foreach (var file in files)
                {
                    try
                    {
                        string content = "";
                        var extension = file.Extension.ToLower();

                        LogSystemMessage($"Processing file: {file.Name} (extension: {extension})");

                        if (needsConversionExtensions.Contains(extension))
                        {
                            // Convert to markdown
                            LogSystemMessage($"Converting {file.Name} to markdown...");
                            var mdFilePath = await pythonInterop.ConvertToMarkdownAsync(file.FullName);
                            if (File.Exists(mdFilePath))
                            {
                                content = await File.ReadAllTextAsync(mdFilePath);
                                LogSystemMessage($"Converted {file.Name}, content length: {content.Length}");
                            }
                            else
                            {
                                LogError($"Conversion failed for {file.Name}");
                            }
                        }
                        else if (supportedTextExtensions.Contains(extension))
                        {
                            // Read as text
                            content = await File.ReadAllTextAsync(file.FullName);
                            LogSystemMessage($"Read {file.Name}, content length: {content.Length}");
                        }
                        else
                        {
                            LogSystemMessage($"Skipping {file.Name} - unsupported extension: {extension}");
                            skippedFileCount++;
                        }

                        if (!string.IsNullOrEmpty(content))
                        {
                            allContent.AppendLine($"## File: {file.Name}");
                            allContent.AppendLine(content);
                            allContent.AppendLine();
                            processedFileCount++;
                        }
                    }
                    catch (Exception ex)
                    {
                        LogError($"Error processing file {file.Name}: {ex.Message}");
                        skippedFileCount++;
                    }
                }

                LogSystemMessage($"Processed {processedFileCount} files, skipped {skippedFileCount} files");

                if (allContent.Length == 0)
                {
                    MessageBox.Show("No content could be extracted from the files.", "No Content",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Build the summarization prompt
                var summaryPrompt = new System.Text.StringBuilder();
                summaryPrompt.AppendLine("Please provide a comprehensive summary of the following content:");

                // Add translation instruction if language is not English
                if (Program.LocalizationService != null)
                {
                    var currentCulture = Program.LocalizationService.CurrentCulture;
                    if (currentCulture.TwoLetterISOLanguageName != "en")
                    {
                        var languageName = currentCulture.DisplayName;
                        summaryPrompt.AppendLine($"\nIMPORTANT: Please provide your response in {languageName} ({currentCulture.Name}).");
                    }
                }

                summaryPrompt.AppendLine($"\n{allContent}");

                var promptText = summaryPrompt.ToString();

                // Log the user message (prompt) with its role
                LogChatMessage("user", $"Summarize folder: {selectedPath}");
                LogSystemMessage("Requesting LLM to generate summary...");

                // Get a chat service without RAG for summarization (RAG is incompatible with summarization)
                var chatServiceForSummarization = GetChatServiceForSummarization();

                // Send the message through chat component using a specific service (bypassing RAG)
                if (_chatComponent != null)
                {
                    await _chatComponent.SendMessageWithSpecificServiceAsync(promptText, chatServiceForSummarization);

                    // The response will automatically be logged and displayed in chat
                    LogSystemMessage("Summary generated successfully");
                    toolStripStatusLabel1.Text = "Summarization complete";
                }
                else
                {
                    LogError("Chat component is not initialized");
                    toolStripStatusLabel1.Text = "Error: Chat not initialized";
                }
            }
            catch (Exception ex)
            {
                LogError($"Summarization error: {ex.Message}");
                toolStripStatusLabel1.Text = "Error occurred";

                // Display error in chat as well
                if (_chatComponent != null)
                {
                    await _chatComponent.AddMessageToChatAsync(
                        $"Error during summarization: {ex.Message}",
                        false,
                        MessageType.Error
                    );
                }
            }
        }

        public IChatService GetChatServiceForSummarization()
        {
            var apiProvider = Properties.Settings.Default.API_Provider;
            var chatMethod = Properties.Settings.Default.ChatMethod;

            IChatService chatService;

            if (chatMethod == "GGUF")
            {
                var chatGGUFModel = Properties.Settings.Default.ChatGGUFModel;
                var modelPath = Path.Combine(
                    PythonPathHelper.PythonToolsDirectory,
                    "models",
                    "chat",
                    chatGGUFModel.Replace("/", Path.DirectorySeparatorChar.ToString())
                );
                chatService = new GGUFChatService(modelPath);
            }
            else if (apiProvider == "Claude Code")
            {
                var cliPath = Properties.Settings.Default.ClaudeCode_CLIPath;
                var model = Properties.Settings.Default.ClaudeCode_Model;
                chatService = new ClaudeCodeChatService(cliPath, model);
            }
            else if (apiProvider == "Azure AI Foundry")
            {
                var azureApiUrl = Properties.Settings.Default.AzureAI_ApiUrl;
                var azureApiKey = Properties.Settings.Default.AzureAI_ApiKey;
                var azureModel = Properties.Settings.Default.AzureAI_SelectedModel;
                var azureApiVersion = Properties.Settings.Default.AzureAI_ApiVersion;
                chatService = new AzureOpenAIChatService(azureApiUrl, azureApiKey, azureModel, azureApiVersion);
            }
            else if (apiProvider == "OpenRouter")
            {
                var openRouterApiUrl = Properties.Settings.Default.OpenRouter_ApiUrl;
                var openRouterApiKey = Properties.Settings.Default.OpenRouter_ApiKey;
                var openRouterModel = Properties.Settings.Default.OpenRouter_SelectedModel;
                chatService = new OpenRouterChatService(openRouterApiUrl, openRouterApiKey, openRouterModel);
            }
            else if (apiProvider == "Ollama")
            {
                var ollamaApiUrl = Properties.Settings.Default.Ollama_ApiUrl;
                var ollamaModel = Properties.Settings.Default.Ollama_SelectedModel;
                chatService = new OpenAIChatService(ollamaApiUrl, "", ollamaModel);
            }
            else if (apiProvider == "OpenAI")
            {
                var openAIApiUrl = Properties.Settings.Default.OpenAIAPI_ApiUrl;
                var openAIApiKey = Properties.Settings.Default.OpenAIAPI_ApiKey;
                var openAIModel = Properties.Settings.Default.OpenAIAPI_SelectedModel;
                chatService = new OpenAIChatService(openAIApiUrl, openAIApiKey, openAIModel);
            }
            else if (apiProvider == "Gemini")
            {
                var geminiApiUrl = Properties.Settings.Default.Gemini_ApiUrl;
                var geminiApiKey = Properties.Settings.Default.Gemini_ApiKey;
                var geminiModel = Properties.Settings.Default.Gemini_SelectedModel;
                chatService = new OpenAIChatService(geminiApiUrl, geminiApiKey, geminiModel);
            }
            else if (apiProvider == "Claude")
            {
                var claudeApiUrl = Properties.Settings.Default.Claude_ApiUrl;
                var claudeApiKey = Properties.Settings.Default.Claude_ApiKey;
                var claudeModel = Properties.Settings.Default.Claude_SelectedModel;
                chatService = new OpenAIChatService(claudeApiUrl, claudeApiKey, claudeModel);
            }
            else // OpenAI Compatible
            {
                var baseUrl = Properties.Settings.Default.OpenAI_BaseUrl;
                var apiKey = Properties.Settings.Default.OpenAI_ApiKey;
                var model = Properties.Settings.Default.OpenAI_SelectedModel;
                chatService = new OpenAIChatService(baseUrl, apiKey, model);
            }

            return chatService;
        }

                private void ktvFolderTree_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                // Get the node at the mouse position
                var node = ktvFolderTree.GetNodeAt(e.X, e.Y);
                if (node != null)
                {
                    _rightClickedNode = node;
                    // Also select the node for visual feedback
                    ktvFolderTree.SelectedNode = node;
                    LogSystemMessage($"Right-clicked on node: {node.Text}, Tag: {node.Tag}");
                }
                else
                {
                    LogSystemMessage($"Right-clicked but no node found at position ({e.X}, {e.Y})");
                }
            }
        }

        public class LogListViewItem : ListViewItem
        {
            public string OriginalMessage { get; set; }

            public LogListViewItem(string[] items, string originalMessage) : base(items)
            {
                OriginalMessage = originalMessage;
            }
        }

        public async Task ToggleApiServer(bool enable)
        {
            if (_apiServerService == null) return;

            if (enable)
            {
                // Start the API server
                _ = _apiServerService.StartApiServer();
            }
            else
            {
                // Stop the API server
                await _apiServerService.StopApiServer();
            }
        }

        public Pages.Chat? GetChatComponent()
        {
            return _chatComponent;
        }

        public void SetApiStatus(string status)
        {
            if (InvokeRequired)
            {
                Invoke(() => toolStripStatusLabelApiStatus.Text = status);
            }
            else
            {
                toolStripStatusLabelApiStatus.Text = status;
            }
        }
    }
}
