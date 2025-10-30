namespace folderchat.Forms.ProviderSettings
{
    partial class OllamaProviderSettingsControl
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
            lblApiUrl.TabIndex = 0;
            lblApiUrl.Values.Text = "API URL:";
            // 
            // txtApiUrl
            // 
            txtApiUrl.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtApiUrl.Location = new Point(3, 31);
            txtApiUrl.Margin = new Padding(3, 2, 3, 2);
            txtApiUrl.Name = "txtApiUrl";
            txtApiUrl.Size = new Size(544, 25);
            txtApiUrl.StateCommon.Border.Rounding = 4F;
            txtApiUrl.TabIndex = 1;
            txtApiUrl.Text = "http://localhost:11434";
            // 
            // lblModelList
            // 
            lblModelList.Location = new Point(3, 59);
            lblModelList.Margin = new Padding(3, 2, 3, 2);
            lblModelList.Name = "lblModelList";
            lblModelList.Size = new Size(87, 20);
            lblModelList.TabIndex = 2;
            lblModelList.Values.Text = "Model List (0):";
            // 
            // cmbModelList
            // 
            cmbModelList.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cmbModelList.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbModelList.DropDownWidth = 300;
            cmbModelList.IntegralHeight = false;
            cmbModelList.Location = new Point(3, 109);
            cmbModelList.Margin = new Padding(3, 2, 3, 2);
            cmbModelList.Name = "cmbModelList";
            cmbModelList.Size = new Size(544, 22);
            cmbModelList.StateCommon.ComboBox.Border.Rounding = 4F;
            cmbModelList.StateCommon.ComboBox.Content.TextH = Krypton.Toolkit.PaletteRelativeAlign.Near;
            cmbModelList.TabIndex = 3;
            // 
            // btnAdd
            // 
            btnAdd.Location = new Point(5, 80);
            btnAdd.Margin = new Padding(3, 2, 3, 2);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(75, 25);
            btnAdd.StateCommon.Border.Rounding = 4F;
            btnAdd.TabIndex = 4;
            btnAdd.Values.DropDownArrowColor = Color.Empty;
            btnAdd.Values.Text = "⊕ Add";
            btnAdd.Click += BtnAdd_Click;
            // 
            // btnFetch
            // 
            btnFetch.Location = new Point(86, 80);
            btnFetch.Margin = new Padding(3, 2, 3, 2);
            btnFetch.Name = "btnFetch";
            btnFetch.Size = new Size(75, 25);
            btnFetch.StateCommon.Border.Rounding = 4F;
            btnFetch.TabIndex = 5;
            btnFetch.Values.DropDownArrowColor = Color.Empty;
            btnFetch.Values.Text = "⚙ Fetch";
            btnFetch.Click += BtnFetch_Click;
            // 
            // btnReset
            // 
            btnReset.Location = new Point(167, 80);
            btnReset.Margin = new Padding(3, 2, 3, 2);
            btnReset.Name = "btnReset";
            btnReset.Size = new Size(75, 25);
            btnReset.StateCommon.Border.Rounding = 4F;
            btnReset.TabIndex = 6;
            btnReset.Values.DropDownArrowColor = Color.Empty;
            btnReset.Values.Text = "⊖ Remove";
            btnReset.Click += BtnReset_Click;
            // 
            // chkSupportSystemMessage
            // 
            chkSupportSystemMessage.Checked = true;
            chkSupportSystemMessage.CheckState = CheckState.Checked;
            chkSupportSystemMessage.Location = new Point(3, 145);
            chkSupportSystemMessage.Name = "chkSupportSystemMessage";
            chkSupportSystemMessage.Size = new Size(143, 20);
            chkSupportSystemMessage.TabIndex = 14;
            chkSupportSystemMessage.Values.Text = "Supports System Role";
            // 
            // OllamaProviderSettingsControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(chkSupportSystemMessage);
            Controls.Add(btnReset);
            Controls.Add(btnFetch);
            Controls.Add(btnAdd);
            Controls.Add(cmbModelList);
            Controls.Add(lblModelList);
            Controls.Add(txtApiUrl);
            Controls.Add(lblApiUrl);
            Name = "OllamaProviderSettingsControl";
            Size = new Size(550, 205);
            ((System.ComponentModel.ISupportInitialize)cmbModelList).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Krypton.Toolkit.KryptonLabel lblApiUrl;
        private Krypton.Toolkit.KryptonTextBox txtApiUrl;
        private Krypton.Toolkit.KryptonLabel lblModelList;
        private Krypton.Toolkit.KryptonComboBox cmbModelList;
        private Krypton.Toolkit.KryptonButton btnAdd;
        private Krypton.Toolkit.KryptonButton btnFetch;
        private Krypton.Toolkit.KryptonButton btnReset;
        private Krypton.Toolkit.KryptonCheckBox chkSupportSystemMessage;
    }
}
