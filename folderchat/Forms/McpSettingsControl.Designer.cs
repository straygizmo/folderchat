namespace folderchat.Forms
{
    partial class McpSettingsControl
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
            kryptonPanel1 = new Krypton.Toolkit.KryptonPanel();
            lblMaxToolIterations = new Krypton.Toolkit.KryptonLabel();
            numMaxToolIterations = new Krypton.Toolkit.KryptonNumericUpDown();
            kryptonPanelMCPAll = new Krypton.Toolkit.KryptonPanel();
            kryptonSplitContainer1 = new Krypton.Toolkit.KryptonSplitContainer();
            grpMCPServers = new Krypton.Toolkit.KryptonGroupBox();
            dgvMcpServers = new Krypton.Toolkit.KryptonDataGridView();
            kryptonPanel2 = new Krypton.Toolkit.KryptonPanel();
            btnRemoveMcpServer = new Krypton.Toolkit.KryptonButton();
            btnEditMcpServer = new Krypton.Toolkit.KryptonButton();
            btnAddMcpServer = new Krypton.Toolkit.KryptonButton();
            kryptonGroupBox2 = new Krypton.Toolkit.KryptonGroupBox();
            rtbMcpLog = new Krypton.Toolkit.KryptonRichTextBox();
            ((System.ComponentModel.ISupportInitialize)kryptonPanel1).BeginInit();
            kryptonPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)kryptonPanelMCPAll).BeginInit();
            kryptonPanelMCPAll.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)kryptonSplitContainer1).BeginInit();
            (kryptonSplitContainer1.Panel1).BeginInit();
            kryptonSplitContainer1.Panel1.SuspendLayout();
            (kryptonSplitContainer1.Panel2).BeginInit();
            kryptonSplitContainer1.Panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)grpMCPServers).BeginInit();
            ((System.ComponentModel.ISupportInitialize)grpMCPServers.Panel).BeginInit();
            grpMCPServers.Panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvMcpServers).BeginInit();
            ((System.ComponentModel.ISupportInitialize)kryptonPanel2).BeginInit();
            kryptonPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)kryptonGroupBox2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)kryptonGroupBox2.Panel).BeginInit();
            kryptonGroupBox2.Panel.SuspendLayout();
            SuspendLayout();
            // 
            // kryptonPanel1
            // 
            kryptonPanel1.Controls.Add(lblMaxToolIterations);
            kryptonPanel1.Controls.Add(numMaxToolIterations);
            kryptonPanel1.Dock = DockStyle.Top;
            kryptonPanel1.Location = new Point(0, 0);
            kryptonPanel1.Name = "kryptonPanel1";
            kryptonPanel1.Size = new Size(338, 33);
            kryptonPanel1.TabIndex = 2;
            // 
            // lblMaxToolIterations
            // 
            lblMaxToolIterations.Location = new Point(4, 6);
            lblMaxToolIterations.Name = "lblMaxToolIterations";
            lblMaxToolIterations.Size = new Size(115, 20);
            lblMaxToolIterations.TabIndex = 0;
            lblMaxToolIterations.Values.Text = "Max Tool Iterations";
            // 
            // numMaxToolIterations
            // 
            numMaxToolIterations.Increment = new decimal(new int[] { 1, 0, 0, 0 });
            numMaxToolIterations.Location = new Point(122, 4);
            numMaxToolIterations.Maximum = new decimal(new int[] { 20, 0, 0, 0 });
            numMaxToolIterations.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numMaxToolIterations.Name = "numMaxToolIterations";
            numMaxToolIterations.Size = new Size(53, 22);
            numMaxToolIterations.TabIndex = 1;
            numMaxToolIterations.Value = new decimal(new int[] { 5, 0, 0, 0 });
            // 
            // kryptonPanelMCPAll
            // 
            kryptonPanelMCPAll.Controls.Add(kryptonSplitContainer1);
            kryptonPanelMCPAll.Dock = DockStyle.Fill;
            kryptonPanelMCPAll.Location = new Point(0, 0);
            kryptonPanelMCPAll.Margin = new Padding(4, 3, 4, 3);
            kryptonPanelMCPAll.Name = "kryptonPanelMCPAll";
            kryptonPanelMCPAll.Padding = new Padding(6);
            kryptonPanelMCPAll.Size = new Size(354, 577);
            kryptonPanelMCPAll.TabIndex = 0;
            // 
            // kryptonSplitContainer1
            // 
            kryptonSplitContainer1.Dock = DockStyle.Fill;
            kryptonSplitContainer1.Location = new Point(6, 6);
            kryptonSplitContainer1.Margin = new Padding(4, 3, 4, 3);
            kryptonSplitContainer1.Orientation = Orientation.Horizontal;
            // 
            // 
            // 
            kryptonSplitContainer1.Panel1.Controls.Add(grpMCPServers);
            // 
            // 
            // 
            kryptonSplitContainer1.Panel2.Controls.Add(kryptonGroupBox2);
            kryptonSplitContainer1.Size = new Size(342, 565);
            kryptonSplitContainer1.SplitterDistance = 400;
            kryptonSplitContainer1.TabIndex = 0;
            // 
            // grpMCPServers
            // 
            grpMCPServers.Dock = DockStyle.Fill;
            grpMCPServers.Location = new Point(0, 0);
            grpMCPServers.Margin = new Padding(4, 3, 4, 3);
            // 
            // 
            // 
            grpMCPServers.Panel.Controls.Add(dgvMcpServers);
            grpMCPServers.Panel.Controls.Add(kryptonPanel2);
            grpMCPServers.Panel.Controls.Add(kryptonPanel1);
            grpMCPServers.Size = new Size(342, 400);
            grpMCPServers.TabIndex = 0;
            grpMCPServers.Values.Heading = "MCP Servers";
            // 
            // dgvMcpServers
            // 
            dgvMcpServers.AllowUserToAddRows = false;
            dgvMcpServers.AllowUserToDeleteRows = false;
            dgvMcpServers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvMcpServers.BorderStyle = BorderStyle.None;
            dgvMcpServers.Dock = DockStyle.Fill;
            dgvMcpServers.Location = new Point(0, 33);
            dgvMcpServers.Margin = new Padding(4, 3, 4, 3);
            dgvMcpServers.Name = "dgvMcpServers";
            dgvMcpServers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvMcpServers.Size = new Size(338, 306);
            dgvMcpServers.TabIndex = 1;
            dgvMcpServers.CellDoubleClick += DgvMcpServers_CellDoubleClick;
            dgvMcpServers.KeyDown += DgvMcpServers_KeyDown;
            // 
            // kryptonPanel2
            // 
            kryptonPanel2.Controls.Add(btnRemoveMcpServer);
            kryptonPanel2.Controls.Add(btnEditMcpServer);
            kryptonPanel2.Controls.Add(btnAddMcpServer);
            kryptonPanel2.Dock = DockStyle.Bottom;
            kryptonPanel2.Location = new Point(0, 339);
            kryptonPanel2.Margin = new Padding(4, 3, 4, 3);
            kryptonPanel2.Name = "kryptonPanel2";
            kryptonPanel2.Size = new Size(338, 37);
            kryptonPanel2.TabIndex = 0;
            // 
            // btnRemoveMcpServer
            // 
            btnRemoveMcpServer.Location = new Point(204, 3);
            btnRemoveMcpServer.Margin = new Padding(4, 3, 4, 3);
            btnRemoveMcpServer.Name = "btnRemoveMcpServer";
            btnRemoveMcpServer.Size = new Size(93, 29);
            btnRemoveMcpServer.StateCommon.Border.Rounding = 4F;
            btnRemoveMcpServer.TabIndex = 2;
            btnRemoveMcpServer.Values.DropDownArrowColor = Color.Empty;
            btnRemoveMcpServer.Values.Text = "Remove";
            btnRemoveMcpServer.Click += BtnRemoveMcpServer_Click;
            // 
            // btnEditMcpServer
            // 
            btnEditMcpServer.Location = new Point(104, 3);
            btnEditMcpServer.Margin = new Padding(4, 3, 4, 3);
            btnEditMcpServer.Name = "btnEditMcpServer";
            btnEditMcpServer.Size = new Size(93, 29);
            btnEditMcpServer.StateCommon.Border.Rounding = 4F;
            btnEditMcpServer.TabIndex = 1;
            btnEditMcpServer.Values.DropDownArrowColor = Color.Empty;
            btnEditMcpServer.Values.Text = "Edit";
            btnEditMcpServer.Click += BtnEditMcpServer_Click;
            // 
            // btnAddMcpServer
            // 
            btnAddMcpServer.Location = new Point(4, 3);
            btnAddMcpServer.Margin = new Padding(4, 3, 4, 3);
            btnAddMcpServer.Name = "btnAddMcpServer";
            btnAddMcpServer.Size = new Size(93, 29);
            btnAddMcpServer.StateCommon.Border.Rounding = 4F;
            btnAddMcpServer.TabIndex = 0;
            btnAddMcpServer.Values.DropDownArrowColor = Color.Empty;
            btnAddMcpServer.Values.Text = "Add";
            btnAddMcpServer.Click += BtnAddMcpServer_Click;
            // 
            // kryptonGroupBox2
            // 
            kryptonGroupBox2.Dock = DockStyle.Fill;
            kryptonGroupBox2.Location = new Point(0, 0);
            kryptonGroupBox2.Margin = new Padding(4, 3, 4, 3);
            // 
            // 
            // 
            kryptonGroupBox2.Panel.Controls.Add(rtbMcpLog);
            kryptonGroupBox2.Size = new Size(342, 160);
            kryptonGroupBox2.TabIndex = 0;
            kryptonGroupBox2.Values.Heading = "Log";
            // 
            // rtbMcpLog
            // 
            rtbMcpLog.Dock = DockStyle.Fill;
            rtbMcpLog.Location = new Point(0, 0);
            rtbMcpLog.Margin = new Padding(4, 3, 4, 3);
            rtbMcpLog.Name = "rtbMcpLog";
            rtbMcpLog.ReadOnly = true;
            rtbMcpLog.Size = new Size(338, 136);
            rtbMcpLog.TabIndex = 0;
            rtbMcpLog.Text = "";
            // 
            // McpSettingsControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(kryptonPanelMCPAll);
            Margin = new Padding(4, 3, 4, 3);
            Name = "McpSettingsControl";
            Size = new Size(354, 577);
            ((System.ComponentModel.ISupportInitialize)kryptonPanel1).EndInit();
            kryptonPanel1.ResumeLayout(false);
            kryptonPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)kryptonPanelMCPAll).EndInit();
            kryptonPanelMCPAll.ResumeLayout(false);
            (kryptonSplitContainer1.Panel1).EndInit();
            kryptonSplitContainer1.Panel1.ResumeLayout(false);
            (kryptonSplitContainer1.Panel2).EndInit();
            kryptonSplitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)kryptonSplitContainer1).EndInit();
            ((System.ComponentModel.ISupportInitialize)grpMCPServers.Panel).EndInit();
            grpMCPServers.Panel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)grpMCPServers).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvMcpServers).EndInit();
            ((System.ComponentModel.ISupportInitialize)kryptonPanel2).EndInit();
            kryptonPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)kryptonGroupBox2.Panel).EndInit();
            kryptonGroupBox2.Panel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)kryptonGroupBox2).EndInit();
            ResumeLayout(false);

        }

        #endregion

        private Krypton.Toolkit.KryptonPanel kryptonPanelMCPAll;
        private Krypton.Toolkit.KryptonSplitContainer kryptonSplitContainer1;
        private Krypton.Toolkit.KryptonGroupBox grpMCPServers;
        private Krypton.Toolkit.KryptonDataGridView dgvMcpServers;
        private Krypton.Toolkit.KryptonPanel kryptonPanel2;
        private Krypton.Toolkit.KryptonButton btnRemoveMcpServer;
        private Krypton.Toolkit.KryptonButton btnEditMcpServer;
        private Krypton.Toolkit.KryptonButton btnAddMcpServer;
        private Krypton.Toolkit.KryptonGroupBox kryptonGroupBox2;
        private Krypton.Toolkit.KryptonRichTextBox rtbMcpLog;
        private Krypton.Toolkit.KryptonPanel kryptonPanel1;
        private Krypton.Toolkit.KryptonLabel lblMaxToolIterations;
        private Krypton.Toolkit.KryptonNumericUpDown numMaxToolIterations;
    }
}
