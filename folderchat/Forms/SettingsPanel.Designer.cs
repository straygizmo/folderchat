namespace folderchat.Forms
{
    partial class SettingsPanel
    {
        /// <summary> 
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region コンポーネント デザイナーで生成されたコード

        /// <summary> 
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を 
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            tabSettings = new TabControl();
            tabPageChat = new TabPage();
            kPanelGGUFProviderSettings = new Krypton.Toolkit.KryptonPanel();
            kPanelAPIProviderSettings = new Krypton.Toolkit.KryptonPanel();
            kPanelChatSettingTop = new Krypton.Toolkit.KryptonPanel();
            cmbAPIProvider = new Krypton.Toolkit.KryptonComboBox();
            lblAPIProvider = new Krypton.Toolkit.KryptonLabel();
            rbChatAPI = new Krypton.Toolkit.KryptonRadioButton();
            rbChatGGUF = new Krypton.Toolkit.KryptonRadioButton();
            lblChatMethod = new Krypton.Toolkit.KryptonLabel();
            cmbChatGGUFModel = new Krypton.Toolkit.KryptonComboBox();
            btnSaveChatSettings = new Krypton.Toolkit.KryptonButton();
            tabPageRAG = new TabPage();
            kPanelRAGTop = new Krypton.Toolkit.KryptonPanel();
            nudMaxContextLength = new Krypton.Toolkit.KryptonNumericUpDown();
            lblTotalMaxContextLength = new Krypton.Toolkit.KryptonLabel();
            nudTopKChunks = new Krypton.Toolkit.KryptonNumericUpDown();
            lblTopKChunks = new Krypton.Toolkit.KryptonLabel();
            nudChunkOverlap = new Krypton.Toolkit.KryptonNumericUpDown();
            nudChunkSize = new Krypton.Toolkit.KryptonNumericUpDown();
            nudModelContextLength = new Krypton.Toolkit.KryptonNumericUpDown();
            txtEmbeddingModel = new Krypton.Toolkit.KryptonTextBox();
            txtEmbeddingUrl = new Krypton.Toolkit.KryptonTextBox();
            lblEmbeddingUrl = new Krypton.Toolkit.KryptonLabel();
            lblEmbeddingModel = new Krypton.Toolkit.KryptonLabel();
            lblModelContextLength = new Krypton.Toolkit.KryptonLabel();
            lblChunkSize = new Krypton.Toolkit.KryptonLabel();
            lblChunkOverlap = new Krypton.Toolkit.KryptonLabel();
            rbEmbeddingAPI = new Krypton.Toolkit.KryptonRadioButton();
            rbEmbeddingGGUF = new Krypton.Toolkit.KryptonRadioButton();
            cmbGGUFModel = new Krypton.Toolkit.KryptonComboBox();
            lblEmbeddingMethod = new Krypton.Toolkit.KryptonLabel();
            btnTestEmbedding = new Krypton.Toolkit.KryptonButton();
            btnSaveRAGSettings = new Krypton.Toolkit.KryptonButton();
            tabPageTheme = new TabPage();
            kryptonThemeListBox1 = new Krypton.Toolkit.KryptonThemeListBox();
            kryptonPanel4 = new Krypton.Toolkit.KryptonPanel();
            kryptonLabel1 = new Krypton.Toolkit.KryptonLabel();
            lblLanguage = new Krypton.Toolkit.KryptonLabel();
            cmbLanguage = new Krypton.Toolkit.KryptonComboBox();
            tabPageMCP = new TabPage();
            dgvMcpServers = new Krypton.Toolkit.KryptonDataGridView();
            kPanelMCPBottom = new Krypton.Toolkit.KryptonPanel();
            btnAddMcpServer = new Krypton.Toolkit.KryptonButton();
            btnEditMcpServer = new Krypton.Toolkit.KryptonButton();
            btnRemoveMcpServer = new Krypton.Toolkit.KryptonButton();
            rtbMcpLog = new Krypton.Toolkit.KryptonRichTextBox();
            claudeCodeSettingsControl = new folderchat.Forms.ProviderSettings.ClaudeCodeProviderSettingsControl();
            openAISettingsControl = new folderchat.Forms.ProviderSettings.OpenAIProviderSettingsControl();
            azureAIFoundrySettingsControl = new folderchat.Forms.ProviderSettings.AzureAIFoundryProviderSettingsControl();
            openRouterSettingsControl = new folderchat.Forms.ProviderSettings.OpenRouterProviderSettingsControl();
            ollamaSettingsControl = new folderchat.Forms.ProviderSettings.OllamaProviderSettingsControl();
            openAIAPISettingsControl = new folderchat.Forms.ProviderSettings.OpenAIAPIProviderSettingsControl();
            geminiSettingsControl = new folderchat.Forms.ProviderSettings.GeminiProviderSettingsControl();
            claudeSettingsControl = new folderchat.Forms.ProviderSettings.ClaudeProviderSettingsControl();
            tabSettings.SuspendLayout();
            tabPageChat.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)kPanelGGUFProviderSettings).BeginInit();
            ((System.ComponentModel.ISupportInitialize)kPanelAPIProviderSettings).BeginInit();
            ((System.ComponentModel.ISupportInitialize)kPanelChatSettingTop).BeginInit();
            kPanelChatSettingTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)cmbAPIProvider).BeginInit();
            ((System.ComponentModel.ISupportInitialize)cmbChatGGUFModel).BeginInit();
            tabPageRAG.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)kPanelRAGTop).BeginInit();
            kPanelRAGTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)cmbGGUFModel).BeginInit();
            tabPageTheme.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)kryptonPanel4).BeginInit();
            kryptonPanel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)cmbLanguage).BeginInit();
            tabPageMCP.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvMcpServers).BeginInit();
            ((System.ComponentModel.ISupportInitialize)kPanelMCPBottom).BeginInit();
            kPanelMCPBottom.SuspendLayout();
            SuspendLayout();
            // 
            // tabSettings
            // 
            tabSettings.Controls.Add(tabPageChat);
            tabSettings.Controls.Add(tabPageRAG);
            tabSettings.Controls.Add(tabPageTheme);
            tabSettings.Controls.Add(tabPageMCP);
            tabSettings.Dock = DockStyle.Fill;
            tabSettings.Location = new Point(0, 0);
            tabSettings.Margin = new Padding(3, 2, 3, 2);
            tabSettings.Name = "tabSettings";
            tabSettings.SelectedIndex = 0;
            tabSettings.Size = new Size(345, 553);
            tabSettings.TabIndex = 16;
            // 
            // tabPageChat
            // 
            tabPageChat.Controls.Add(kPanelGGUFProviderSettings);
            tabPageChat.Controls.Add(kPanelAPIProviderSettings);
            tabPageChat.Controls.Add(kPanelChatSettingTop);
            tabPageChat.Controls.Add(btnSaveChatSettings);
            tabPageChat.Location = new Point(4, 24);
            tabPageChat.Margin = new Padding(3, 2, 3, 2);
            tabPageChat.Name = "tabPageChat";
            tabPageChat.Padding = new Padding(3, 2, 3, 2);
            tabPageChat.Size = new Size(337, 525);
            tabPageChat.TabIndex = 0;
            tabPageChat.Text = "Chat Settings";
            tabPageChat.UseVisualStyleBackColor = true;
            // 
            // kPanelGGUFProviderSettings
            // 
            kPanelGGUFProviderSettings.Location = new Point(61, 324);
            kPanelGGUFProviderSettings.Name = "kPanelGGUFProviderSettings";
            kPanelGGUFProviderSettings.Size = new Size(176, 138);
            kPanelGGUFProviderSettings.TabIndex = 4;
            // 
            // kPanelAPIProviderSettings
            // 
            kPanelAPIProviderSettings.Location = new Point(61, 135);
            kPanelAPIProviderSettings.Name = "kPanelAPIProviderSettings";
            kPanelAPIProviderSettings.Size = new Size(176, 140);
            kPanelAPIProviderSettings.TabIndex = 3;
            // 
            // kPanelChatSettingTop
            // 
            kPanelChatSettingTop.Controls.Add(cmbAPIProvider);
            kPanelChatSettingTop.Controls.Add(lblAPIProvider);
            kPanelChatSettingTop.Controls.Add(rbChatAPI);
            kPanelChatSettingTop.Controls.Add(rbChatGGUF);
            kPanelChatSettingTop.Controls.Add(lblChatMethod);
            kPanelChatSettingTop.Controls.Add(cmbChatGGUFModel);
            kPanelChatSettingTop.Dock = DockStyle.Top;
            kPanelChatSettingTop.Location = new Point(3, 2);
            kPanelChatSettingTop.Margin = new Padding(3, 2, 3, 2);
            kPanelChatSettingTop.Name = "kPanelChatSettingTop";
            kPanelChatSettingTop.Size = new Size(331, 84);
            kPanelChatSettingTop.TabIndex = 0;
            // 
            // cmbAPIProvider
            // 
            cmbAPIProvider.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cmbAPIProvider.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbAPIProvider.DropDownWidth = 250;
            cmbAPIProvider.Items.AddRange(new object[] { "Azure AI Foundry", "Claude", "Claude Code", "Gemini", "Ollama", "OpenAI", "OpenAI Compatible", "OpenRouter" });
            cmbAPIProvider.Location = new Point(8, 53);
            cmbAPIProvider.Name = "cmbAPIProvider";
            cmbAPIProvider.Size = new Size(320, 22);
            cmbAPIProvider.StateCommon.ComboBox.Content.TextH = Krypton.Toolkit.PaletteRelativeAlign.Near;
            cmbAPIProvider.TabIndex = 1;
            cmbAPIProvider.SelectedIndexChanged += cmbAPIProvider_SelectedIndexChanged;
            // 
            // lblAPIProvider
            // 
            lblAPIProvider.Location = new Point(8, 34);
            lblAPIProvider.Margin = new Padding(3, 2, 3, 2);
            lblAPIProvider.Name = "lblAPIProvider";
            lblAPIProvider.Size = new Size(80, 20);
            lblAPIProvider.TabIndex = 14;
            lblAPIProvider.Values.Text = "API Provider:";
            // 
            // rbChatAPI
            // 
            rbChatAPI.Location = new Point(144, 10);
            rbChatAPI.Margin = new Padding(3, 2, 3, 2);
            rbChatAPI.Name = "rbChatAPI";
            rbChatAPI.Size = new Size(41, 20);
            rbChatAPI.TabIndex = 15;
            rbChatAPI.Values.Text = "API";
            rbChatAPI.CheckedChanged += rbChatAPI_CheckedChanged;
            // 
            // rbChatGGUF
            // 
            rbChatGGUF.Location = new Point(203, 10);
            rbChatGGUF.Margin = new Padding(3, 2, 3, 2);
            rbChatGGUF.Name = "rbChatGGUF";
            rbChatGGUF.Size = new Size(54, 20);
            rbChatGGUF.TabIndex = 16;
            rbChatGGUF.Values.Text = "GGUF";
            rbChatGGUF.CheckedChanged += rbChatGGUF_CheckedChanged;
            // 
            // lblChatMethod
            // 
            lblChatMethod.Location = new Point(3, 10);
            lblChatMethod.Margin = new Padding(3, 2, 3, 2);
            lblChatMethod.Name = "lblChatMethod";
            lblChatMethod.Size = new Size(85, 20);
            lblChatMethod.TabIndex = 17;
            lblChatMethod.Values.Text = "Chat Method:";
            // 
            // cmbChatGGUFModel
            // 
            cmbChatGGUFModel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cmbChatGGUFModel.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbChatGGUFModel.DropDownWidth = 304;
            cmbChatGGUFModel.Location = new Point(8, 53);
            cmbChatGGUFModel.Margin = new Padding(3, 2, 3, 2);
            cmbChatGGUFModel.Name = "cmbChatGGUFModel";
            cmbChatGGUFModel.Size = new Size(320, 22);
            cmbChatGGUFModel.StateCommon.ComboBox.Content.TextH = Krypton.Toolkit.PaletteRelativeAlign.Near;
            cmbChatGGUFModel.TabIndex = 18;
            // 
            // btnSaveChatSettings
            // 
            btnSaveChatSettings.Dock = DockStyle.Bottom;
            btnSaveChatSettings.Location = new Point(3, 493);
            btnSaveChatSettings.Margin = new Padding(3, 2, 3, 2);
            btnSaveChatSettings.Name = "btnSaveChatSettings";
            btnSaveChatSettings.Size = new Size(331, 30);
            btnSaveChatSettings.TabIndex = 1;
            btnSaveChatSettings.Values.DropDownArrowColor = Color.Empty;
            btnSaveChatSettings.Values.Text = "Save";
            btnSaveChatSettings.Click += btnSave_Click;
            // 
            // tabPageRAG
            // 
            tabPageRAG.Controls.Add(kPanelRAGTop);
            tabPageRAG.Controls.Add(btnSaveRAGSettings);
            tabPageRAG.Location = new Point(4, 24);
            tabPageRAG.Margin = new Padding(3, 2, 3, 2);
            tabPageRAG.Name = "tabPageRAG";
            tabPageRAG.Padding = new Padding(3, 2, 3, 2);
            tabPageRAG.Size = new Size(337, 525);
            tabPageRAG.TabIndex = 1;
            tabPageRAG.Text = "RAG Settings";
            tabPageRAG.UseVisualStyleBackColor = true;
            // 
            // kPanelRAGTop
            // 
            kPanelRAGTop.Controls.Add(nudMaxContextLength);
            kPanelRAGTop.Controls.Add(lblTotalMaxContextLength);
            kPanelRAGTop.Controls.Add(nudTopKChunks);
            kPanelRAGTop.Controls.Add(lblTopKChunks);
            kPanelRAGTop.Controls.Add(nudChunkOverlap);
            kPanelRAGTop.Controls.Add(nudChunkSize);
            kPanelRAGTop.Controls.Add(nudModelContextLength);
            kPanelRAGTop.Controls.Add(txtEmbeddingModel);
            kPanelRAGTop.Controls.Add(txtEmbeddingUrl);
            kPanelRAGTop.Controls.Add(lblEmbeddingUrl);
            kPanelRAGTop.Controls.Add(lblEmbeddingModel);
            kPanelRAGTop.Controls.Add(lblModelContextLength);
            kPanelRAGTop.Controls.Add(lblChunkSize);
            kPanelRAGTop.Controls.Add(lblChunkOverlap);
            kPanelRAGTop.Controls.Add(rbEmbeddingAPI);
            kPanelRAGTop.Controls.Add(rbEmbeddingGGUF);
            kPanelRAGTop.Controls.Add(cmbGGUFModel);
            kPanelRAGTop.Controls.Add(lblEmbeddingMethod);
            kPanelRAGTop.Controls.Add(btnTestEmbedding);
            kPanelRAGTop.Dock = DockStyle.Fill;
            kPanelRAGTop.Location = new Point(3, 2);
            kPanelRAGTop.Margin = new Padding(3, 2, 3, 2);
            kPanelRAGTop.Name = "kPanelRAGTop";
            kPanelRAGTop.Size = new Size(331, 491);
            kPanelRAGTop.TabIndex = 0;
            // 
            // nudMaxContextLength
            // 
            nudMaxContextLength.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            nudMaxContextLength.Increment = new decimal(new int[] { 500, 0, 0, 0 });
            nudMaxContextLength.Location = new Point(222, 270);
            nudMaxContextLength.Margin = new Padding(3, 2, 3, 2);
            nudMaxContextLength.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            nudMaxContextLength.Minimum = new decimal(new int[] { 1000, 0, 0, 0 });
            nudMaxContextLength.Name = "nudMaxContextLength";
            nudMaxContextLength.Size = new Size(87, 22);
            nudMaxContextLength.TabIndex = 21;
            nudMaxContextLength.Value = new decimal(new int[] { 8000, 0, 0, 0 });
            // 
            // lblTotalMaxContextLength
            // 
            lblTotalMaxContextLength.Location = new Point(16, 270);
            lblTotalMaxContextLength.Margin = new Padding(3, 2, 3, 2);
            lblTotalMaxContextLength.Name = "lblTotalMaxContextLength";
            lblTotalMaxContextLength.Size = new Size(193, 20);
            lblTotalMaxContextLength.TabIndex = 20;
            lblTotalMaxContextLength.Values.Text = "Total Max Context Length (chars):";
            // 
            // nudTopKChunks
            // 
            nudTopKChunks.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            nudTopKChunks.Increment = new decimal(new int[] { 1, 0, 0, 0 });
            nudTopKChunks.Location = new Point(223, 242);
            nudTopKChunks.Margin = new Padding(3, 2, 3, 2);
            nudTopKChunks.Maximum = new decimal(new int[] { 50, 0, 0, 0 });
            nudTopKChunks.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            nudTopKChunks.Name = "nudTopKChunks";
            nudTopKChunks.Size = new Size(87, 22);
            nudTopKChunks.TabIndex = 19;
            nudTopKChunks.Value = new decimal(new int[] { 5, 0, 0, 0 });
            // 
            // lblTopKChunks
            // 
            lblTopKChunks.Location = new Point(17, 240);
            lblTopKChunks.Margin = new Padding(3, 2, 3, 2);
            lblTopKChunks.Name = "lblTopKChunks";
            lblTopKChunks.Size = new Size(88, 20);
            lblTopKChunks.TabIndex = 18;
            lblTopKChunks.Values.Text = "Top K Chunks:";
            // 
            // nudChunkOverlap
            // 
            nudChunkOverlap.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            nudChunkOverlap.Increment = new decimal(new int[] { 10, 0, 0, 0 });
            nudChunkOverlap.Location = new Point(223, 196);
            nudChunkOverlap.Margin = new Padding(3, 2, 3, 2);
            nudChunkOverlap.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            nudChunkOverlap.Minimum = new decimal(new int[] { 0, 0, 0, 0 });
            nudChunkOverlap.Name = "nudChunkOverlap";
            nudChunkOverlap.Size = new Size(87, 22);
            nudChunkOverlap.TabIndex = 19;
            nudChunkOverlap.Value = new decimal(new int[] { 100, 0, 0, 0 });
            // 
            // nudChunkSize
            // 
            nudChunkSize.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            nudChunkSize.Increment = new decimal(new int[] { 50, 0, 0, 0 });
            nudChunkSize.Location = new Point(223, 170);
            nudChunkSize.Margin = new Padding(3, 2, 3, 2);
            nudChunkSize.Maximum = new decimal(new int[] { 4096, 0, 0, 0 });
            nudChunkSize.Minimum = new decimal(new int[] { 100, 0, 0, 0 });
            nudChunkSize.Name = "nudChunkSize";
            nudChunkSize.Size = new Size(87, 22);
            nudChunkSize.TabIndex = 17;
            nudChunkSize.Value = new decimal(new int[] { 500, 0, 0, 0 });
            // 
            // nudModelContextLength
            // 
            nudModelContextLength.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            nudModelContextLength.Increment = new decimal(new int[] { 256, 0, 0, 0 });
            nudModelContextLength.Location = new Point(223, 144);
            nudModelContextLength.Margin = new Padding(3, 2, 3, 2);
            nudModelContextLength.Maximum = new decimal(new int[] { 32768, 0, 0, 0 });
            nudModelContextLength.Minimum = new decimal(new int[] { 512, 0, 0, 0 });
            nudModelContextLength.Name = "nudModelContextLength";
            nudModelContextLength.Size = new Size(87, 22);
            nudModelContextLength.TabIndex = 15;
            nudModelContextLength.Value = new decimal(new int[] { 2048, 0, 0, 0 });
            // 
            // txtEmbeddingModel
            // 
            txtEmbeddingModel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtEmbeddingModel.Location = new Point(6, 106);
            txtEmbeddingModel.Margin = new Padding(3, 2, 3, 2);
            txtEmbeddingModel.Name = "txtEmbeddingModel";
            txtEmbeddingModel.Size = new Size(304, 23);
            txtEmbeddingModel.TabIndex = 13;
            txtEmbeddingModel.Text = "text-embedding-embeddinggemma-300m";
            // 
            // txtEmbeddingUrl
            // 
            txtEmbeddingUrl.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtEmbeddingUrl.Location = new Point(6, 55);
            txtEmbeddingUrl.Margin = new Padding(3, 2, 3, 2);
            txtEmbeddingUrl.Name = "txtEmbeddingUrl";
            txtEmbeddingUrl.Size = new Size(304, 23);
            txtEmbeddingUrl.TabIndex = 11;
            txtEmbeddingUrl.Text = "http://localhost:/1234";
            // 
            // lblEmbeddingUrl
            // 
            lblEmbeddingUrl.Location = new Point(3, 34);
            lblEmbeddingUrl.Margin = new Padding(3, 2, 3, 2);
            lblEmbeddingUrl.Name = "lblEmbeddingUrl";
            lblEmbeddingUrl.Size = new Size(101, 20);
            lblEmbeddingUrl.TabIndex = 10;
            lblEmbeddingUrl.Values.Text = "Embedding URL:";
            // 
            // lblEmbeddingModel
            // 
            lblEmbeddingModel.Location = new Point(3, 85);
            lblEmbeddingModel.Margin = new Padding(3, 2, 3, 2);
            lblEmbeddingModel.Name = "lblEmbeddingModel";
            lblEmbeddingModel.Size = new Size(114, 20);
            lblEmbeddingModel.TabIndex = 12;
            lblEmbeddingModel.Values.Text = "Embedding Model:";
            // 
            // lblModelContextLength
            // 
            lblModelContextLength.Location = new Point(17, 144);
            lblModelContextLength.Margin = new Padding(3, 2, 3, 2);
            lblModelContextLength.Name = "lblModelContextLength";
            lblModelContextLength.Size = new Size(135, 20);
            lblModelContextLength.TabIndex = 14;
            lblModelContextLength.Values.Text = "Model Context Length:";
            // 
            // lblChunkSize
            // 
            lblChunkSize.Location = new Point(17, 168);
            lblChunkSize.Margin = new Padding(3, 2, 3, 2);
            lblChunkSize.Name = "lblChunkSize";
            lblChunkSize.Size = new Size(73, 20);
            lblChunkSize.TabIndex = 16;
            lblChunkSize.Values.Text = "Chunk Size:";
            // 
            // lblChunkOverlap
            // 
            lblChunkOverlap.Location = new Point(17, 194);
            lblChunkOverlap.Margin = new Padding(3, 2, 3, 2);
            lblChunkOverlap.Name = "lblChunkOverlap";
            lblChunkOverlap.Size = new Size(94, 20);
            lblChunkOverlap.TabIndex = 18;
            lblChunkOverlap.Values.Text = "Chunk Overlap:";
            // 
            // rbEmbeddingAPI
            // 
            rbEmbeddingAPI.Location = new Point(144, 10);
            rbEmbeddingAPI.Margin = new Padding(3, 2, 3, 2);
            rbEmbeddingAPI.Name = "rbEmbeddingAPI";
            rbEmbeddingAPI.Size = new Size(41, 20);
            rbEmbeddingAPI.TabIndex = 7;
            rbEmbeddingAPI.Values.Text = "API";
            rbEmbeddingAPI.CheckedChanged += rbEmbeddingAPI_CheckedChanged;
            // 
            // rbEmbeddingGGUF
            // 
            rbEmbeddingGGUF.Location = new Point(203, 10);
            rbEmbeddingGGUF.Margin = new Padding(3, 2, 3, 2);
            rbEmbeddingGGUF.Name = "rbEmbeddingGGUF";
            rbEmbeddingGGUF.Size = new Size(54, 20);
            rbEmbeddingGGUF.TabIndex = 8;
            rbEmbeddingGGUF.Values.Text = "GGUF";
            rbEmbeddingGGUF.CheckedChanged += rbEmbeddingGGUF_CheckedChanged;
            // 
            // cmbGGUFModel
            // 
            cmbGGUFModel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cmbGGUFModel.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbGGUFModel.DropDownWidth = 295;
            cmbGGUFModel.Location = new Point(6, 55);
            cmbGGUFModel.Margin = new Padding(3, 2, 3, 2);
            cmbGGUFModel.Name = "cmbGGUFModel";
            cmbGGUFModel.Size = new Size(304, 22);
            cmbGGUFModel.TabIndex = 9;
            cmbGGUFModel.Visible = false;
            // 
            // lblEmbeddingMethod
            // 
            lblEmbeddingMethod.Location = new Point(3, 10);
            lblEmbeddingMethod.Margin = new Padding(3, 2, 3, 2);
            lblEmbeddingMethod.Name = "lblEmbeddingMethod";
            lblEmbeddingMethod.Size = new Size(122, 20);
            lblEmbeddingMethod.TabIndex = 6;
            lblEmbeddingMethod.Values.Text = "Embedding Method:";
            // 
            // btnTestEmbedding
            // 
            btnTestEmbedding.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            btnTestEmbedding.Location = new Point(40, 328);
            btnTestEmbedding.Margin = new Padding(3, 2, 3, 2);
            btnTestEmbedding.Name = "btnTestEmbedding";
            btnTestEmbedding.Size = new Size(247, 30);
            btnTestEmbedding.TabIndex = 22;
            btnTestEmbedding.Values.DropDownArrowColor = Color.Empty;
            btnTestEmbedding.Values.Text = "Test Embedding";
            btnTestEmbedding.Click += btnTestEmbedding_Click;
            // 
            // btnSaveRAGSettings
            // 
            btnSaveRAGSettings.Dock = DockStyle.Bottom;
            btnSaveRAGSettings.Location = new Point(3, 493);
            btnSaveRAGSettings.Margin = new Padding(3, 2, 3, 2);
            btnSaveRAGSettings.Name = "btnSaveRAGSettings";
            btnSaveRAGSettings.Size = new Size(331, 30);
            btnSaveRAGSettings.TabIndex = 2;
            btnSaveRAGSettings.Values.DropDownArrowColor = Color.Empty;
            btnSaveRAGSettings.Values.Text = "Save";
            btnSaveRAGSettings.Click += btnSave_Click;
            // 
            // tabPageTheme
            // 
            tabPageTheme.Controls.Add(kryptonThemeListBox1);
            tabPageTheme.Controls.Add(kryptonPanel4);
            tabPageTheme.Location = new Point(4, 24);
            tabPageTheme.Name = "tabPageTheme";
            tabPageTheme.Size = new Size(337, 525);
            tabPageTheme.TabIndex = 2;
            tabPageTheme.Text = "UI";
            tabPageTheme.UseVisualStyleBackColor = true;
            // 
            // kryptonThemeListBox1
            // 
            kryptonThemeListBox1.DefaultPalette = Krypton.Toolkit.PaletteMode.Microsoft365Blue;
            kryptonThemeListBox1.Dock = DockStyle.Fill;
            kryptonThemeListBox1.Location = new Point(0, 75);
            kryptonThemeListBox1.Name = "kryptonThemeListBox1";
            kryptonThemeListBox1.Size = new Size(337, 450);
            kryptonThemeListBox1.TabIndex = 0;
            kryptonThemeListBox1.SelectedIndexChanged += KryptonThemeListBox1_SelectedIndexChanged;
            // 
            // kryptonPanel4
            // 
            kryptonPanel4.Controls.Add(kryptonLabel1);
            kryptonPanel4.Controls.Add(lblLanguage);
            kryptonPanel4.Controls.Add(cmbLanguage);
            kryptonPanel4.Dock = DockStyle.Top;
            kryptonPanel4.Location = new Point(0, 0);
            kryptonPanel4.Name = "kryptonPanel4";
            kryptonPanel4.Size = new Size(337, 75);
            kryptonPanel4.TabIndex = 1;
            // 
            // kryptonLabel1
            // 
            kryptonLabel1.Location = new Point(2, 55);
            kryptonLabel1.Margin = new Padding(3, 2, 3, 2);
            kryptonLabel1.Name = "kryptonLabel1";
            kryptonLabel1.Size = new Size(50, 20);
            kryptonLabel1.TabIndex = 17;
            kryptonLabel1.Values.Text = "Theme:";
            // 
            // lblLanguage
            // 
            lblLanguage.Location = new Point(3, 8);
            lblLanguage.Margin = new Padding(3, 2, 3, 2);
            lblLanguage.Name = "lblLanguage";
            lblLanguage.Size = new Size(67, 20);
            lblLanguage.TabIndex = 15;
            lblLanguage.Values.Text = "Language:";
            // 
            // cmbLanguage
            // 
            cmbLanguage.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cmbLanguage.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbLanguage.DropDownWidth = 289;
            cmbLanguage.Location = new Point(4, 30);
            cmbLanguage.Margin = new Padding(3, 2, 3, 2);
            cmbLanguage.Name = "cmbLanguage";
            cmbLanguage.Size = new Size(330, 22);
            cmbLanguage.TabIndex = 16;
            cmbLanguage.SelectedIndexChanged += cmbLanguage_SelectedIndexChanged;
            // 
            // tabPageMCP
            // 
            tabPageMCP.Controls.Add(dgvMcpServers);
            tabPageMCP.Controls.Add(kPanelMCPBottom);
            tabPageMCP.Controls.Add(rtbMcpLog);
            tabPageMCP.Location = new Point(4, 24);
            tabPageMCP.Name = "tabPageMCP";
            tabPageMCP.Size = new Size(337, 525);
            tabPageMCP.TabIndex = 3;
            tabPageMCP.Text = "MCP";
            tabPageMCP.UseVisualStyleBackColor = true;
            // 
            // dgvMcpServers
            // 
            dgvMcpServers.BorderStyle = BorderStyle.None;
            dgvMcpServers.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvMcpServers.Dock = DockStyle.Fill;
            dgvMcpServers.Location = new Point(0, 41);
            dgvMcpServers.Name = "dgvMcpServers";
            dgvMcpServers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvMcpServers.Size = new Size(337, 296);
            dgvMcpServers.TabIndex = 0;
            dgvMcpServers.CellDoubleClick += DgvMcpServers_CellDoubleClick;
            dgvMcpServers.KeyDown += DgvMcpServers_KeyDown;
            // 
            // kPanelMCPBottom
            // 
            kPanelMCPBottom.Controls.Add(btnAddMcpServer);
            kPanelMCPBottom.Controls.Add(btnEditMcpServer);
            kPanelMCPBottom.Controls.Add(btnRemoveMcpServer);
            kPanelMCPBottom.Dock = DockStyle.Top;
            kPanelMCPBottom.Location = new Point(0, 0);
            kPanelMCPBottom.Name = "kPanelMCPBottom";
            kPanelMCPBottom.Size = new Size(337, 41);
            kPanelMCPBottom.TabIndex = 0;
            // 
            // btnAddMcpServer
            // 
            btnAddMcpServer.Location = new Point(13, 6);
            btnAddMcpServer.Name = "btnAddMcpServer";
            btnAddMcpServer.Size = new Size(50, 25);
            btnAddMcpServer.TabIndex = 1;
            btnAddMcpServer.Values.DropDownArrowColor = Color.Empty;
            btnAddMcpServer.Values.Text = "Add";
            btnAddMcpServer.Click += BtnAddMcpServer_Click;
            // 
            // btnEditMcpServer
            // 
            btnEditMcpServer.Location = new Point(69, 6);
            btnEditMcpServer.Name = "btnEditMcpServer";
            btnEditMcpServer.Size = new Size(50, 25);
            btnEditMcpServer.TabIndex = 2;
            btnEditMcpServer.Values.DropDownArrowColor = Color.Empty;
            btnEditMcpServer.Values.Text = "Edit";
            btnEditMcpServer.Click += BtnEditMcpServer_Click;
            // 
            // btnRemoveMcpServer
            // 
            btnRemoveMcpServer.Location = new Point(125, 6);
            btnRemoveMcpServer.Name = "btnRemoveMcpServer";
            btnRemoveMcpServer.Size = new Size(60, 25);
            btnRemoveMcpServer.TabIndex = 3;
            btnRemoveMcpServer.Values.DropDownArrowColor = Color.Empty;
            btnRemoveMcpServer.Values.Text = "Remove";
            btnRemoveMcpServer.Click += BtnRemoveMcpServer_Click;
            // 
            // rtbMcpLog
            // 
            rtbMcpLog.Dock = DockStyle.Bottom;
            rtbMcpLog.Location = new Point(0, 337);
            rtbMcpLog.Name = "rtbMcpLog";
            rtbMcpLog.ReadOnly = true;
            rtbMcpLog.Size = new Size(337, 188);
            rtbMcpLog.TabIndex = 6;
            rtbMcpLog.Text = "";
            // 
            // claudeCodeSettingsControl
            // 
            claudeCodeSettingsControl.Location = new Point(0, 0);
            claudeCodeSettingsControl.Name = "claudeCodeSettingsControl";
            claudeCodeSettingsControl.Size = new Size(316, 120);
            claudeCodeSettingsControl.TabIndex = 2;
            // 
            // openAISettingsControl
            // 
            openAISettingsControl.Location = new Point(0, 0);
            openAISettingsControl.Name = "openAISettingsControl";
            openAISettingsControl.Size = new Size(315, 243);
            openAISettingsControl.TabIndex = 2;
            // 
            // azureAIFoundrySettingsControl
            // 
            azureAIFoundrySettingsControl.Location = new Point(0, 0);
            azureAIFoundrySettingsControl.Name = "azureAIFoundrySettingsControl";
            azureAIFoundrySettingsControl.Size = new Size(550, 270);
            azureAIFoundrySettingsControl.TabIndex = 3;
            // 
            // openRouterSettingsControl
            // 
            openRouterSettingsControl.Location = new Point(0, 0);
            openRouterSettingsControl.Name = "openRouterSettingsControl";
            openRouterSettingsControl.Size = new Size(550, 170);
            openRouterSettingsControl.TabIndex = 4;
            // 
            // ollamaSettingsControl
            // 
            ollamaSettingsControl.Location = new Point(0, 0);
            ollamaSettingsControl.Name = "ollamaSettingsControl";
            ollamaSettingsControl.Size = new Size(550, 120);
            ollamaSettingsControl.TabIndex = 5;
            // 
            // openAIAPISettingsControl
            // 
            openAIAPISettingsControl.Location = new Point(0, 0);
            openAIAPISettingsControl.Name = "openAIAPISettingsControl";
            openAIAPISettingsControl.Size = new Size(550, 170);
            openAIAPISettingsControl.TabIndex = 6;
            // 
            // geminiSettingsControl
            // 
            geminiSettingsControl.Location = new Point(0, 0);
            geminiSettingsControl.Name = "geminiSettingsControl";
            geminiSettingsControl.Size = new Size(550, 170);
            geminiSettingsControl.TabIndex = 7;
            // 
            // claudeSettingsControl
            // 
            claudeSettingsControl.Location = new Point(0, 0);
            claudeSettingsControl.Name = "claudeSettingsControl";
            claudeSettingsControl.Size = new Size(550, 240);
            claudeSettingsControl.TabIndex = 8;
            // 
            // SettingsPanel
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tabSettings);
            Name = "SettingsPanel";
            Size = new Size(345, 553);
            tabSettings.ResumeLayout(false);
            tabPageChat.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)kPanelGGUFProviderSettings).EndInit();
            ((System.ComponentModel.ISupportInitialize)kPanelAPIProviderSettings).EndInit();
            ((System.ComponentModel.ISupportInitialize)kPanelChatSettingTop).EndInit();
            kPanelChatSettingTop.ResumeLayout(false);
            kPanelChatSettingTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)cmbAPIProvider).EndInit();
            ((System.ComponentModel.ISupportInitialize)cmbChatGGUFModel).EndInit();
            tabPageRAG.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)kPanelRAGTop).EndInit();
            kPanelRAGTop.ResumeLayout(false);
            kPanelRAGTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)cmbGGUFModel).EndInit();
            tabPageTheme.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)kryptonPanel4).EndInit();
            kryptonPanel4.ResumeLayout(false);
            kryptonPanel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)cmbLanguage).EndInit();
            tabPageMCP.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvMcpServers).EndInit();
            ((System.ComponentModel.ISupportInitialize)kPanelMCPBottom).EndInit();
            kPanelMCPBottom.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TabControl tabSettings;
        private TabPage tabPageChat;
        private Krypton.Toolkit.KryptonPanel kPanelGGUFProviderSettings;
        private Krypton.Toolkit.KryptonPanel kPanelAPIProviderSettings;
        private Krypton.Toolkit.KryptonPanel kPanelChatSettingTop;
        private Krypton.Toolkit.KryptonComboBox cmbAPIProvider;
        private Krypton.Toolkit.KryptonLabel lblAPIProvider;
        private Krypton.Toolkit.KryptonRadioButton rbChatAPI;
        private Krypton.Toolkit.KryptonRadioButton rbChatGGUF;
        private ProviderSettings.OpenAIProviderSettingsControl openAISettingsControl;
        private ProviderSettings.ClaudeCodeProviderSettingsControl claudeCodeSettingsControl;
        private ProviderSettings.AzureAIFoundryProviderSettingsControl azureAIFoundrySettingsControl;
        private ProviderSettings.OpenRouterProviderSettingsControl openRouterSettingsControl;
        private ProviderSettings.OllamaProviderSettingsControl ollamaSettingsControl;
        private ProviderSettings.OpenAIAPIProviderSettingsControl openAIAPISettingsControl;
        private ProviderSettings.GeminiProviderSettingsControl geminiSettingsControl;
        private ProviderSettings.ClaudeProviderSettingsControl claudeSettingsControl;
        private Krypton.Toolkit.KryptonLabel lblChatMethod;
        private Krypton.Toolkit.KryptonComboBox cmbChatGGUFModel;
        private Krypton.Toolkit.KryptonButton btnSaveChatSettings;
        private TabPage tabPageRAG;
        private Krypton.Toolkit.KryptonPanel kPanelRAGTop;
        private Krypton.Toolkit.KryptonNumericUpDown nudMaxContextLength;
        private Krypton.Toolkit.KryptonLabel lblTotalMaxContextLength;
        private Krypton.Toolkit.KryptonNumericUpDown nudTopKChunks;
        private Krypton.Toolkit.KryptonLabel lblTopKChunks;
        private Krypton.Toolkit.KryptonNumericUpDown nudChunkOverlap;
        private Krypton.Toolkit.KryptonNumericUpDown nudChunkSize;
        private Krypton.Toolkit.KryptonNumericUpDown nudModelContextLength;
        private Krypton.Toolkit.KryptonTextBox txtEmbeddingModel;
        private Krypton.Toolkit.KryptonTextBox txtEmbeddingUrl;
        private Krypton.Toolkit.KryptonLabel lblEmbeddingUrl;
        private Krypton.Toolkit.KryptonLabel lblEmbeddingModel;
        private Krypton.Toolkit.KryptonLabel lblModelContextLength;
        private Krypton.Toolkit.KryptonLabel lblChunkSize;
        private Krypton.Toolkit.KryptonLabel lblChunkOverlap;
        private Krypton.Toolkit.KryptonRadioButton rbEmbeddingAPI;
        private Krypton.Toolkit.KryptonRadioButton rbEmbeddingGGUF;
        private Krypton.Toolkit.KryptonComboBox cmbGGUFModel;
        private Krypton.Toolkit.KryptonLabel lblEmbeddingMethod;
        private Krypton.Toolkit.KryptonButton btnTestEmbedding;
        private Krypton.Toolkit.KryptonButton btnSaveRAGSettings;
        private TabPage tabPageTheme;
        public Krypton.Toolkit.KryptonThemeListBox kryptonThemeListBox1;
        private Krypton.Toolkit.KryptonPanel kryptonPanel4;
        private Krypton.Toolkit.KryptonLabel kryptonLabel1;
        private Krypton.Toolkit.KryptonLabel lblLanguage;
        private Krypton.Toolkit.KryptonComboBox cmbLanguage;
        private TabPage tabPageMCP;
        private Krypton.Toolkit.KryptonPanel kPanelMCPBottom;
        private Krypton.Toolkit.KryptonButton btnAddMcpServer;
        private Krypton.Toolkit.KryptonButton btnEditMcpServer;
        private Krypton.Toolkit.KryptonButton btnRemoveMcpServer;
        private Krypton.Toolkit.KryptonRichTextBox rtbMcpLog;
        private Krypton.Toolkit.KryptonDataGridView dgvMcpServers;
    }
}
