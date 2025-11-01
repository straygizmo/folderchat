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
            cmbChatGGUFModel = new Krypton.Toolkit.KryptonComboBox();
            cmbAPIProvider = new Krypton.Toolkit.KryptonComboBox();
            lblAPIProvider = new Krypton.Toolkit.KryptonLabel();
            rbChatAPI = new Krypton.Toolkit.KryptonRadioButton();
            rbChatGGUF = new Krypton.Toolkit.KryptonRadioButton();
            lblChatMethod = new Krypton.Toolkit.KryptonLabel();
            btnSaveChatSettings = new Krypton.Toolkit.KryptonButton();
            tabPageRAG = new TabPage();
            btnSaveRAGSettings = new Krypton.Toolkit.KryptonButton();
            tabPageMCP = new TabPage();
            tabPageGeneral = new TabPage();
            kryptonThemeListBox1 = new Krypton.Toolkit.KryptonThemeListBox();
            kryptonPanel4 = new Krypton.Toolkit.KryptonPanel();
            kryptonLabel1 = new Krypton.Toolkit.KryptonLabel();
            lblLanguage = new Krypton.Toolkit.KryptonLabel();
            cmbLanguage = new Krypton.Toolkit.KryptonComboBox();
            tabPageServer = new TabPage();
            kPanelAPI = new Krypton.Toolkit.KryptonPanel();
            nudServerPort = new Krypton.Toolkit.KryptonNumericUpDown();
            lblServerPort = new Krypton.Toolkit.KryptonLabel();
            chkEnableAPIServer = new Krypton.Toolkit.KryptonCheckBox();
            btnSaveAPIServerSettings = new Krypton.Toolkit.KryptonButton();
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
            ((System.ComponentModel.ISupportInitialize)cmbChatGGUFModel).BeginInit();
            ((System.ComponentModel.ISupportInitialize)cmbAPIProvider).BeginInit();
            tabPageRAG.SuspendLayout();
            tabPageMCP.SuspendLayout();
            tabPageGeneral.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)kryptonPanel4).BeginInit();
            kryptonPanel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)cmbLanguage).BeginInit();
            tabPageServer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)kPanelAPI).BeginInit();
            kPanelAPI.SuspendLayout();
            SuspendLayout();
            // 
            // tabSettings
            // 
            tabSettings.Controls.Add(tabPageChat);
            tabSettings.Controls.Add(tabPageRAG);
            tabSettings.Controls.Add(tabPageMCP);
            tabSettings.Controls.Add(tabPageGeneral);
            tabSettings.Controls.Add(tabPageServer);
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
            tabPageChat.Text = "Chat";
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
            kPanelChatSettingTop.Controls.Add(cmbChatGGUFModel);
            kPanelChatSettingTop.Controls.Add(cmbAPIProvider);
            kPanelChatSettingTop.Controls.Add(lblAPIProvider);
            kPanelChatSettingTop.Controls.Add(rbChatAPI);
            kPanelChatSettingTop.Controls.Add(rbChatGGUF);
            kPanelChatSettingTop.Controls.Add(lblChatMethod);
            kPanelChatSettingTop.Dock = DockStyle.Top;
            kPanelChatSettingTop.Location = new Point(3, 2);
            kPanelChatSettingTop.Margin = new Padding(3, 2, 3, 2);
            kPanelChatSettingTop.Name = "kPanelChatSettingTop";
            kPanelChatSettingTop.Size = new Size(331, 84);
            kPanelChatSettingTop.TabIndex = 0;
            // 
            // cmbChatGGUFModel
            // 
            cmbChatGGUFModel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cmbChatGGUFModel.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbChatGGUFModel.DropDownWidth = 304;
            cmbChatGGUFModel.Location = new Point(7, 53);
            cmbChatGGUFModel.Margin = new Padding(3, 2, 3, 2);
            cmbChatGGUFModel.Name = "cmbChatGGUFModel";
            cmbChatGGUFModel.Size = new Size(320, 24);
            cmbChatGGUFModel.StateCommon.ComboBox.Border.Rounding = 4F;
            cmbChatGGUFModel.StateCommon.ComboBox.Content.TextH = Krypton.Toolkit.PaletteRelativeAlign.Near;
            cmbChatGGUFModel.TabIndex = 18;
            // 
            // cmbAPIProvider
            // 
            cmbAPIProvider.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cmbAPIProvider.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbAPIProvider.DropDownWidth = 250;
            cmbAPIProvider.Items.AddRange(new object[] { "Azure AI Foundry", "Claude", "Claude Code", "Gemini", "Ollama", "OpenAI", "OpenAI Compatible", "OpenRouter" });
            cmbAPIProvider.Location = new Point(8, 53);
            cmbAPIProvider.Name = "cmbAPIProvider";
            cmbAPIProvider.Size = new Size(320, 24);
            cmbAPIProvider.StateCommon.ComboBox.Border.Rounding = 3F;
            cmbAPIProvider.StateCommon.ComboBox.Content.TextH = Krypton.Toolkit.PaletteRelativeAlign.Near;
            cmbAPIProvider.TabIndex = 1;
            cmbAPIProvider.SelectedIndexChanged += cmbAPIProvider_SelectedIndexChanged;
            // 
            // lblAPIProvider
            // 
            lblAPIProvider.Location = new Point(7, 34);
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
            tabPageRAG.Controls.Add(btnSaveRAGSettings);
            tabPageRAG.Location = new Point(4, 24);
            tabPageRAG.Margin = new Padding(3, 2, 3, 2);
            tabPageRAG.Name = "tabPageRAG";
            tabPageRAG.Padding = new Padding(3, 2, 3, 2);
            tabPageRAG.Size = new Size(337, 525);
            tabPageRAG.TabIndex = 1;
            tabPageRAG.Text = "RAG";
            tabPageRAG.UseVisualStyleBackColor = true;
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
            // tabPageMCP
            // 
            tabPageMCP.Location = new Point(4, 24);
            tabPageMCP.Name = "tabPageMCP";
            tabPageMCP.Size = new Size(337, 525);
            tabPageMCP.TabIndex = 3;
            tabPageMCP.Text = "MCP";
            tabPageMCP.UseVisualStyleBackColor = true;
            // 
            // tabPageGeneral
            // 
            tabPageGeneral.Controls.Add(kryptonThemeListBox1);
            tabPageGeneral.Controls.Add(kryptonPanel4);
            tabPageGeneral.Location = new Point(4, 24);
            tabPageGeneral.Name = "tabPageGeneral";
            tabPageGeneral.Size = new Size(337, 525);
            tabPageGeneral.TabIndex = 2;
            tabPageGeneral.Text = "General";
            tabPageGeneral.UseVisualStyleBackColor = true;
            // 
            // kryptonThemeListBox1
            // 
            kryptonThemeListBox1.DefaultPalette = Krypton.Toolkit.PaletteMode.Microsoft365Blue;
            kryptonThemeListBox1.Dock = DockStyle.Fill;
            kryptonThemeListBox1.Location = new Point(0, 88);
            kryptonThemeListBox1.Name = "kryptonThemeListBox1";
            kryptonThemeListBox1.Size = new Size(337, 437);
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
            kryptonPanel4.Size = new Size(337, 88);
            kryptonPanel4.TabIndex = 1;
            // 
            // kryptonLabel1
            // 
            kryptonLabel1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            kryptonLabel1.Location = new Point(4, 67);
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
            cmbLanguage.Size = new Size(330, 24);
            cmbLanguage.StateCommon.ComboBox.Border.Rounding = 4F;
            cmbLanguage.TabIndex = 16;
            cmbLanguage.SelectedIndexChanged += cmbLanguage_SelectedIndexChanged;
            // 
            // tabPageServer
            // 
            tabPageServer.Controls.Add(kPanelAPI);
            tabPageServer.Controls.Add(btnSaveAPIServerSettings);
            tabPageServer.Location = new Point(4, 24);
            tabPageServer.Name = "tabPageServer";
            tabPageServer.Size = new Size(337, 525);
            tabPageServer.TabIndex = 4;
            tabPageServer.Text = "API Server";
            tabPageServer.UseVisualStyleBackColor = true;
            // 
            // kPanelAPI
            // 
            kPanelAPI.Controls.Add(nudServerPort);
            kPanelAPI.Controls.Add(lblServerPort);
            kPanelAPI.Controls.Add(chkEnableAPIServer);
            kPanelAPI.Dock = DockStyle.Fill;
            kPanelAPI.Location = new Point(0, 0);
            kPanelAPI.Name = "kPanelAPI";
            kPanelAPI.Size = new Size(337, 495);
            kPanelAPI.TabIndex = 0;
            // 
            // nudServerPort
            // 
            nudServerPort.Increment = new decimal(new int[] { 1, 0, 0, 0 });
            nudServerPort.Location = new Point(98, 61);
            nudServerPort.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
            nudServerPort.Minimum = new decimal(new int[] { 1000, 0, 0, 0 });
            nudServerPort.Name = "nudServerPort";
            nudServerPort.Size = new Size(88, 24);
            nudServerPort.StateCommon.Border.Rounding = 4F;
            nudServerPort.TabIndex = 2;
            nudServerPort.Value = new decimal(new int[] { 11550, 0, 0, 0 });
            // 
            // lblServerPort
            // 
            lblServerPort.Location = new Point(22, 64);
            lblServerPort.Name = "lblServerPort";
            lblServerPort.Size = new Size(73, 20);
            lblServerPort.TabIndex = 1;
            lblServerPort.Values.Text = "Server Port:";
            // 
            // chkEnableAPIServer
            // 
            chkEnableAPIServer.Location = new Point(22, 26);
            chkEnableAPIServer.Name = "chkEnableAPIServer";
            chkEnableAPIServer.Size = new Size(119, 20);
            chkEnableAPIServer.TabIndex = 0;
            chkEnableAPIServer.Values.Text = "Enable API Server";
            // 
            // btnSaveAPIServerSettings
            // 
            btnSaveAPIServerSettings.Dock = DockStyle.Bottom;
            btnSaveAPIServerSettings.Location = new Point(0, 495);
            btnSaveAPIServerSettings.Margin = new Padding(3, 2, 3, 2);
            btnSaveAPIServerSettings.Name = "btnSaveAPIServerSettings";
            btnSaveAPIServerSettings.Size = new Size(337, 30);
            btnSaveAPIServerSettings.TabIndex = 3;
            btnSaveAPIServerSettings.Values.DropDownArrowColor = Color.Empty;
            btnSaveAPIServerSettings.Values.Text = "Save";
            btnSaveAPIServerSettings.Click += btnSave_Click;
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
            ((System.ComponentModel.ISupportInitialize)cmbChatGGUFModel).EndInit();
            ((System.ComponentModel.ISupportInitialize)cmbAPIProvider).EndInit();
            tabPageRAG.ResumeLayout(false);
            tabPageMCP.ResumeLayout(false);
            tabPageGeneral.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)kryptonPanel4).EndInit();
            kryptonPanel4.ResumeLayout(false);
            kryptonPanel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)cmbLanguage).EndInit();
            tabPageServer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)kPanelAPI).EndInit();
            kPanelAPI.ResumeLayout(false);
            kPanelAPI.PerformLayout();
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
        private Krypton.Toolkit.KryptonButton btnSaveRAGSettings;
        private TabPage tabPageGeneral;
        public Krypton.Toolkit.KryptonThemeListBox kryptonThemeListBox1;
        private Krypton.Toolkit.KryptonPanel kryptonPanel4;
        private Krypton.Toolkit.KryptonLabel kryptonLabel1;
        private Krypton.Toolkit.KryptonLabel lblLanguage;
        private Krypton.Toolkit.KryptonComboBox cmbLanguage;
        private TabPage tabPageMCP;
        private TabPage tabPageServer;
        private Krypton.Toolkit.KryptonPanel kPanelAPI;
        private Krypton.Toolkit.KryptonCheckBox chkEnableAPIServer;
        private Krypton.Toolkit.KryptonNumericUpDown nudServerPort;
        private Krypton.Toolkit.KryptonLabel lblServerPort;
        private Krypton.Toolkit.KryptonButton btnSaveAPIServerSettings;

    }
}