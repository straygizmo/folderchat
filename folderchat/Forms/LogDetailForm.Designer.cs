namespace folderchat.Forms
{
    partial class LogDetailForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LogDetailForm));
            kryptonPanel1 = new Krypton.Toolkit.KryptonPanel();
            btnClose = new Krypton.Toolkit.KryptonButton();
            txtMessage = new Krypton.Toolkit.KryptonTextBox();
            lblMessageHeader = new Krypton.Toolkit.KryptonLabel();
            lblType = new Krypton.Toolkit.KryptonLabel();
            lblDate = new Krypton.Toolkit.KryptonLabel();
            ((System.ComponentModel.ISupportInitialize)kryptonPanel1).BeginInit();
            kryptonPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // kryptonPanel1
            // 
            kryptonPanel1.Controls.Add(btnClose);
            kryptonPanel1.Controls.Add(txtMessage);
            kryptonPanel1.Controls.Add(lblMessageHeader);
            kryptonPanel1.Controls.Add(lblType);
            kryptonPanel1.Controls.Add(lblDate);
            kryptonPanel1.Dock = DockStyle.Fill;
            kryptonPanel1.Location = new Point(0, 0);
            kryptonPanel1.Name = "kryptonPanel1";
            kryptonPanel1.Size = new Size(634, 279);
            kryptonPanel1.TabIndex = 0;
            // 
            // btnClose
            // 
            btnClose.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnClose.Location = new Point(499, 252);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(90, 25);
            btnClose.StateCommon.Border.Rounding = 3F;
            btnClose.TabIndex = 4;
            btnClose.Values.DropDownArrowColor = Color.Empty;
            btnClose.Values.Text = "Close";
            btnClose.Click += btnClose_Click;
            // 
            // txtMessage
            // 
            txtMessage.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtMessage.Location = new Point(14, 84);
            txtMessage.Multiline = true;
            txtMessage.Name = "txtMessage";
            txtMessage.ScrollBars = ScrollBars.Vertical;
            txtMessage.Size = new Size(608, 150);
            txtMessage.StateCommon.Border.Rounding = 3F;
            txtMessage.TabIndex = 3;
            // 
            // lblMessageHeader
            // 
            lblMessageHeader.Location = new Point(10, 64);
            lblMessageHeader.Name = "lblMessageHeader";
            lblMessageHeader.Size = new Size(61, 20);
            lblMessageHeader.TabIndex = 2;
            lblMessageHeader.Values.Text = "Message:";
            // 
            // lblType
            // 
            lblType.Location = new Point(10, 38);
            lblType.Name = "lblType";
            lblType.Size = new Size(39, 20);
            lblType.TabIndex = 1;
            lblType.Values.Text = "Type:";
            // 
            // lblDate
            // 
            lblDate.Location = new Point(10, 12);
            lblDate.Name = "lblDate";
            lblDate.Size = new Size(39, 20);
            lblDate.TabIndex = 0;
            lblDate.Values.Text = "Date:";
            // 
            // LogDetailForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(634, 279);
            Controls.Add(kryptonPanel1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Location = new Point(0, 0);
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new Size(300, 200);
            Name = "LogDetailForm";
            StartPosition = FormStartPosition.CenterScreen;
            StateCommon.Border.Rounding = 8F;
            Text = "Log Details";
            ((System.ComponentModel.ISupportInitialize)kryptonPanel1).EndInit();
            kryptonPanel1.ResumeLayout(false);
            kryptonPanel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Krypton.Toolkit.KryptonPanel kryptonPanel1;
        private Krypton.Toolkit.KryptonLabel lblDate;
        private Krypton.Toolkit.KryptonLabel lblMessageHeader;
        private Krypton.Toolkit.KryptonLabel lblType;
        private Krypton.Toolkit.KryptonTextBox txtMessage;
        private Krypton.Toolkit.KryptonButton btnClose;
    }
}
