namespace folderchat.Forms.ProviderSettings
{
    partial class AzureAIFoundryProviderSettingsControl
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            lblApiUrl = new Krypton.Toolkit.KryptonLabel();
            txtApiUrl = new Krypton.Toolkit.KryptonTextBox();
            lblApiVersion = new Krypton.Toolkit.KryptonLabel();
            txtApiVersion = new Krypton.Toolkit.KryptonTextBox();
            lblApiKey = new Krypton.Toolkit.KryptonLabel();
            txtApiKey = new Krypton.Toolkit.KryptonTextBox();
            lblModelList = new Krypton.Toolkit.KryptonLabel();
            cmbModelList = new Krypton.Toolkit.KryptonComboBox();
            btnAdd = new Krypton.Toolkit.KryptonButton();
            btnFetch = new Krypton.Toolkit.KryptonButton();
            btnReset = new Krypton.Toolkit.KryptonButton();
            chkSupportSystemMessage = new Krypton.Toolkit.KryptonCheckBox();
            ((System.ComponentModel.ISupportInitialize)cmbModelList).BeginInit();
            SuspendLayout();
            // 
            // lblApiUrl
            // 
            lblApiUrl.Location = new Point(3, 10);
            lblApiUrl.Margin = new Padding(3, 2, 3, 2);
            lblApiUrl.Name = "lblApiUrl";
            lblApiUrl.Size = new Size(56, 20);
            lblApiUrl.TabIndex = 2;
            lblApiUrl.Values.Text = "API URL:";
            // 
            // txtApiUrl
            // 
            txtApiUrl.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtApiUrl.Location = new Point(3, 31);
            txtApiUrl.Margin = new Padding(3, 2, 3, 2);
            txtApiUrl.Name = "txtApiUrl";
            txtApiUrl.Size = new Size(544, 23);
            txtApiUrl.TabIndex = 3;
            txtApiUrl.Text = "https://YOUR_RESOURCE_NAME.openai.azure.com";
            // 
            // lblApiVersion
            // 
            lblApiVersion.Location = new Point(3, 58);
            lblApiVersion.Margin = new Padding(3, 2, 3, 2);
            lblApiVersion.Name = "lblApiVersion";
            lblApiVersion.Size = new Size(76, 20);
            lblApiVersion.TabIndex = 4;
            lblApiVersion.Values.Text = "API Version:";
            // 
            // txtApiVersion
            // 
            txtApiVersion.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtApiVersion.Location = new Point(3, 80);
            txtApiVersion.Margin = new Padding(3, 2, 3, 2);
            txtApiVersion.Name = "txtApiVersion";
            txtApiVersion.Size = new Size(544, 23);
            txtApiVersion.TabIndex = 5;
            txtApiVersion.Text = "2025-01-01-preview";
            // 
            // lblApiKey
            // 
            lblApiKey.Location = new Point(3, 107);
            lblApiKey.Margin = new Padding(3, 2, 3, 2);
            lblApiKey.Name = "lblApiKey";
            lblApiKey.Size = new Size(54, 20);
            lblApiKey.TabIndex = 6;
            lblApiKey.Values.Text = "API Key:";
            // 
            // txtApiKey
            // 
            txtApiKey.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtApiKey.Location = new Point(3, 128);
            txtApiKey.Margin = new Padding(3, 2, 3, 2);
            txtApiKey.Name = "txtApiKey";
            txtApiKey.PasswordChar = '●';
            txtApiKey.Size = new Size(544, 23);
            txtApiKey.TabIndex = 7;
            txtApiKey.UseSystemPasswordChar = true;
            // 
            // lblModelList
            // 
            lblModelList.Location = new Point(3, 157);
            lblModelList.Margin = new Padding(3, 2, 3, 2);
            lblModelList.Name = "lblModelList";
            lblModelList.Size = new Size(87, 20);
            lblModelList.TabIndex = 8;
            lblModelList.Values.Text = "Model List (0):";
            // 
            // cmbModelList
            // 
            cmbModelList.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cmbModelList.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbModelList.DropDownWidth = 300;
            cmbModelList.IntegralHeight = false;
            cmbModelList.Location = new Point(3, 205);
            cmbModelList.Margin = new Padding(3, 2, 3, 2);
            cmbModelList.Name = "cmbModelList";
            cmbModelList.Size = new Size(544, 22);
            cmbModelList.StateCommon.ComboBox.Content.TextH = Krypton.Toolkit.PaletteRelativeAlign.Near;
            cmbModelList.TabIndex = 9;
            // 
            // btnAdd
            // 
            btnAdd.Location = new Point(3, 176);
            btnAdd.Margin = new Padding(3, 2, 3, 2);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(75, 25);
            btnAdd.TabIndex = 10;
            btnAdd.Values.DropDownArrowColor = Color.Empty;
            btnAdd.Values.Text = "⊕ Add";
            btnAdd.Click += BtnAdd_Click;
            // 
            // btnFetch
            // 
            btnFetch.Location = new Point(85, 176);
            btnFetch.Margin = new Padding(3, 2, 3, 2);
            btnFetch.Name = "btnFetch";
            btnFetch.Size = new Size(75, 25);
            btnFetch.TabIndex = 11;
            btnFetch.Values.DropDownArrowColor = Color.Empty;
            btnFetch.Values.Text = "⚙ Fetch";
            btnFetch.Click += BtnFetch_Click;
            // 
            // btnReset
            // 
            btnReset.Location = new Point(166, 176);
            btnReset.Margin = new Padding(3, 2, 3, 2);
            btnReset.Name = "btnReset";
            btnReset.Size = new Size(75, 25);
            btnReset.TabIndex = 12;
            btnReset.Values.DropDownArrowColor = Color.Empty;
            btnReset.Values.Text = "⊖ Remove";
            btnReset.Click += BtnReset_Click;
            // 
            // chkSupportSystemMessage
            // 
            chkSupportSystemMessage.Checked = true;
            chkSupportSystemMessage.CheckState = CheckState.Checked;
            chkSupportSystemMessage.Location = new Point(3, 244);
            chkSupportSystemMessage.Name = "chkSupportSystemMessage";
            chkSupportSystemMessage.Size = new Size(143, 20);
            chkSupportSystemMessage.TabIndex = 13;
            chkSupportSystemMessage.Values.Text = "Supports System Role";
            // 
            // AzureAIFoundryProviderSettingsControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(chkSupportSystemMessage);
            Controls.Add(btnReset);
            Controls.Add(btnFetch);
            Controls.Add(btnAdd);
            Controls.Add(cmbModelList);
            Controls.Add(lblModelList);
            Controls.Add(txtApiKey);
            Controls.Add(lblApiKey);
            Controls.Add(txtApiVersion);
            Controls.Add(lblApiVersion);
            Controls.Add(txtApiUrl);
            Controls.Add(lblApiUrl);
            Name = "AzureAIFoundryProviderSettingsControl";
            Size = new Size(550, 283);
            ((System.ComponentModel.ISupportInitialize)cmbModelList).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Krypton.Toolkit.KryptonLabel lblApiUrl;
        private Krypton.Toolkit.KryptonTextBox txtApiUrl;
        private Krypton.Toolkit.KryptonLabel lblApiVersion;
        private Krypton.Toolkit.KryptonTextBox txtApiVersion;
        private Krypton.Toolkit.KryptonLabel lblApiKey;
        private Krypton.Toolkit.KryptonTextBox txtApiKey;
        private Krypton.Toolkit.KryptonLabel lblModelList;
        private Krypton.Toolkit.KryptonComboBox cmbModelList;
        private Krypton.Toolkit.KryptonButton btnAdd;
        private Krypton.Toolkit.KryptonButton btnFetch;
        private Krypton.Toolkit.KryptonButton btnReset;
        private Krypton.Toolkit.KryptonCheckBox chkSupportSystemMessage;
    }
}
