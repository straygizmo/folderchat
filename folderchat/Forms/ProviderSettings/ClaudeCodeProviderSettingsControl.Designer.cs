namespace folderchat.Forms.ProviderSettings
{
    partial class ClaudeCodeProviderSettingsControl
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
            txtClaudeCodePath = new Krypton.Toolkit.KryptonTextBox();
            cmbClaudeModel = new Krypton.Toolkit.KryptonComboBox();
            lblClaudeCodePath = new Krypton.Toolkit.KryptonLabel();
            lblClaudeModel = new Krypton.Toolkit.KryptonLabel();
            chkSupportSystemMessage = new Krypton.Toolkit.KryptonCheckBox();
            ((System.ComponentModel.ISupportInitialize)cmbClaudeModel).BeginInit();
            SuspendLayout();
            // 
            // txtClaudeCodePath
            // 
            txtClaudeCodePath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtClaudeCodePath.Location = new Point(3, 31);
            txtClaudeCodePath.Margin = new Padding(3, 2, 3, 2);
            txtClaudeCodePath.Name = "txtClaudeCodePath";
            txtClaudeCodePath.Size = new Size(290, 25);
            txtClaudeCodePath.StateCommon.Border.Rounding = 4F;
            txtClaudeCodePath.TabIndex = 1;
            txtClaudeCodePath.Text = "claude";
            // 
            // cmbClaudeModel
            // 
            cmbClaudeModel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cmbClaudeModel.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbClaudeModel.DropDownWidth = 250;
            cmbClaudeModel.Items.AddRange(new object[] { "sonnet", "opus", "haiku" });
            cmbClaudeModel.Location = new Point(3, 80);
            cmbClaudeModel.Name = "cmbClaudeModel";
            cmbClaudeModel.Size = new Size(290, 22);
            cmbClaudeModel.StateCommon.ComboBox.Border.Rounding = 4F;
            cmbClaudeModel.StateCommon.ComboBox.Content.TextH = Krypton.Toolkit.PaletteRelativeAlign.Near;
            cmbClaudeModel.TabIndex = 3;
            // 
            // lblClaudeCodePath
            // 
            lblClaudeCodePath.Location = new Point(3, 10);
            lblClaudeCodePath.Margin = new Padding(3, 2, 3, 2);
            lblClaudeCodePath.Name = "lblClaudeCodePath";
            lblClaudeCodePath.Size = new Size(131, 20);
            lblClaudeCodePath.TabIndex = 0;
            lblClaudeCodePath.Values.Text = "Claude Code CLI Path:";
            // 
            // lblClaudeModel
            // 
            lblClaudeModel.Location = new Point(3, 59);
            lblClaudeModel.Margin = new Padding(3, 2, 3, 2);
            lblClaudeModel.Name = "lblClaudeModel";
            lblClaudeModel.Size = new Size(48, 20);
            lblClaudeModel.TabIndex = 2;
            lblClaudeModel.Values.Text = "Model:";
            // 
            // chkSupportSystemMessage
            // 
            chkSupportSystemMessage.Enabled = false;
            chkSupportSystemMessage.Location = new Point(3, 117);
            chkSupportSystemMessage.Name = "chkSupportSystemMessage";
            chkSupportSystemMessage.Size = new Size(143, 20);
            chkSupportSystemMessage.TabIndex = 14;
            chkSupportSystemMessage.Values.Text = "Supports System Role";
            // 
            // ClaudeCodeProviderSettingsControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(chkSupportSystemMessage);
            Controls.Add(cmbClaudeModel);
            Controls.Add(lblClaudeModel);
            Controls.Add(txtClaudeCodePath);
            Controls.Add(lblClaudeCodePath);
            Name = "ClaudeCodeProviderSettingsControl";
            Size = new Size(296, 174);
            ((System.ComponentModel.ISupportInitialize)cmbClaudeModel).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Krypton.Toolkit.KryptonTextBox txtClaudeCodePath;
        private Krypton.Toolkit.KryptonComboBox cmbClaudeModel;
        private Krypton.Toolkit.KryptonLabel lblClaudeCodePath;
        private Krypton.Toolkit.KryptonLabel lblClaudeModel;
        private Krypton.Toolkit.KryptonCheckBox chkSupportSystemMessage;
    }
}
