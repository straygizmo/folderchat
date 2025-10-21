namespace folderchat.Forms
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            blazorWebView1 = new Microsoft.AspNetCore.Components.WebView.WindowsForms.BlazorWebView();
            tabMain = new TabControl();
            tabPageDir = new TabPage();
            ktvFolderTree = new Krypton.Toolkit.KryptonTreeView();
            contextMenuTreeView = new ContextMenuStrip(components);
            menuItemIndexing = new ToolStripMenuItem();
            menuItemSummarize = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            menuItemRefresh = new ToolStripMenuItem();
            lblRagStatus = new Krypton.Toolkit.KryptonLabel();
            tabPageSettings = new TabPage();
            kryptonManager1 = new Krypton.Toolkit.KryptonManager(components);
            toolTip1 = new ToolTip(components);
            kryptonStatusStrip1 = new Krypton.Toolkit.KryptonStatusStrip();
            toolStripStatusLabel1 = new ToolStripStatusLabel();
            toolStripStatusLabelApiStatus = new ToolStripStatusLabel();
            splitContainerMain = new SplitContainer();
            kListViewBottomLog = new Krypton.Toolkit.KryptonListView();
            columnHeader1 = new ColumnHeader();
            columnHeader2 = new ColumnHeader();
            columnHeader3 = new ColumnHeader();
            kSeparatorForLogView = new Krypton.Toolkit.KryptonSeparator();
            tabMain.SuspendLayout();
            tabPageDir.SuspendLayout();
            contextMenuTreeView.SuspendLayout();
            kryptonStatusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainerMain).BeginInit();
            splitContainerMain.Panel1.SuspendLayout();
            splitContainerMain.Panel2.SuspendLayout();
            splitContainerMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)kSeparatorForLogView).BeginInit();
            SuspendLayout();
            // 
            // blazorWebView1
            // 
            blazorWebView1.Dock = DockStyle.Fill;
            blazorWebView1.Location = new Point(0, 0);
            blazorWebView1.Name = "blazorWebView1";
            blazorWebView1.Size = new Size(829, 595);
            blazorWebView1.TabIndex = 0;
            blazorWebView1.Text = "blazorWebView1";
            // 
            // tabMain
            // 
            tabMain.Controls.Add(tabPageDir);
            tabMain.Controls.Add(tabPageSettings);
            tabMain.Dock = DockStyle.Fill;
            tabMain.Location = new Point(0, 0);
            tabMain.Margin = new Padding(3, 2, 3, 2);
            tabMain.Name = "tabMain";
            tabMain.SelectedIndex = 0;
            tabMain.Size = new Size(353, 595);
            tabMain.TabIndex = 2;
            // 
            // tabPageDir
            // 
            tabPageDir.Controls.Add(ktvFolderTree);
            tabPageDir.Controls.Add(lblRagStatus);
            tabPageDir.Location = new Point(4, 24);
            tabPageDir.Margin = new Padding(3, 2, 3, 2);
            tabPageDir.Name = "tabPageDir";
            tabPageDir.Padding = new Padding(3, 2, 3, 2);
            tabPageDir.Size = new Size(345, 567);
            tabPageDir.TabIndex = 0;
            tabPageDir.Text = "📁Dir to RAG";
            tabPageDir.UseVisualStyleBackColor = true;
            // 
            // ktvFolderTree
            // 
            ktvFolderTree.CheckBoxes = true;
            ktvFolderTree.ContextMenuStrip = contextMenuTreeView;
            ktvFolderTree.Dock = DockStyle.Fill;
            ktvFolderTree.Location = new Point(3, 2);
            ktvFolderTree.Margin = new Padding(3, 2, 3, 2);
            ktvFolderTree.MultiSelect = true;
            ktvFolderTree.Name = "ktvFolderTree";
            ktvFolderTree.Size = new Size(339, 561);
            ktvFolderTree.TabIndex = 0;
            ktvFolderTree.MouseDown += ktvFolderTree_MouseDown;
            // 
            // contextMenuTreeView
            // 
            contextMenuTreeView.Font = new Font("Segoe UI", 9F);
            contextMenuTreeView.Items.AddRange(new ToolStripItem[] { menuItemIndexing, menuItemSummarize, toolStripSeparator1, menuItemRefresh });
            contextMenuTreeView.Name = "contextMenuTreeView";
            contextMenuTreeView.Size = new Size(134, 76);
            // 
            // menuItemIndexing
            // 
            menuItemIndexing.Image = Properties.Resources.Indexing;
            menuItemIndexing.Name = "menuItemIndexing";
            menuItemIndexing.Size = new Size(133, 22);
            menuItemIndexing.Text = "Indexing";
            menuItemIndexing.Click += menuItemIndexing_Click;
            // 
            // menuItemSummarize
            // 
            menuItemSummarize.Name = "menuItemSummarize";
            menuItemSummarize.Size = new Size(133, 22);
            menuItemSummarize.Text = "Summarize";
            menuItemSummarize.Click += menuItemSummarize_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(130, 6);
            // 
            // menuItemRefresh
            // 
            menuItemRefresh.Image = Properties.Resources.Refresh;
            menuItemRefresh.Name = "menuItemRefresh";
            menuItemRefresh.Size = new Size(133, 22);
            menuItemRefresh.Text = "Refresh";
            menuItemRefresh.Click += menuItemRefresh_Click;
            // 
            // lblRagStatus
            // 
            lblRagStatus.Dock = DockStyle.Bottom;
            lblRagStatus.Location = new Point(3, 563);
            lblRagStatus.Margin = new Padding(3, 2, 3, 2);
            lblRagStatus.Name = "lblRagStatus";
            lblRagStatus.Size = new Size(339, 2);
            lblRagStatus.TabIndex = 2;
            lblRagStatus.Values.Text = "";
            // 
            // tabPageSettings
            // 
            tabPageSettings.AutoScroll = true;
            tabPageSettings.Location = new Point(4, 24);
            tabPageSettings.Margin = new Padding(3, 2, 3, 2);
            tabPageSettings.Name = "tabPageSettings";
            tabPageSettings.Padding = new Padding(9, 8, 9, 8);
            tabPageSettings.Size = new Size(345, 567);
            tabPageSettings.TabIndex = 2;
            tabPageSettings.Text = "⚙️Settings";
            tabPageSettings.UseVisualStyleBackColor = true;
            // 
            // kryptonStatusStrip1
            // 
            kryptonStatusStrip1.Font = new Font("Segoe UI", 9F);
            kryptonStatusStrip1.ImageScalingSize = new Size(20, 20);
            kryptonStatusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel1, toolStripStatusLabelApiStatus });
            kryptonStatusStrip1.Location = new Point(0, 733);
            kryptonStatusStrip1.Name = "kryptonStatusStrip1";
            kryptonStatusStrip1.ProgressBars = null;
            kryptonStatusStrip1.RenderMode = ToolStripRenderMode.ManagerRenderMode;
            kryptonStatusStrip1.Size = new Size(1186, 22);
            kryptonStatusStrip1.TabIndex = 4;
            kryptonStatusStrip1.Text = "kryptonStatusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new Size(41, 17);
            toolStripStatusLabel1.Text = "status:";
            toolStripStatusLabel1.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // toolStripStatusLabelApiStatus
            // 
            toolStripStatusLabelApiStatus.Name = "toolStripStatusLabelApiStatus";
            toolStripStatusLabelApiStatus.Size = new Size(110, 17);
            toolStripStatusLabelApiStatus.Text = "API Server: Stopped";
            toolStripStatusLabelApiStatus.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // splitContainerMain
            // 
            splitContainerMain.Dock = DockStyle.Fill;
            splitContainerMain.Location = new Point(0, 0);
            splitContainerMain.Name = "splitContainerMain";
            // 
            // splitContainerMain.Panel1
            // 
            splitContainerMain.Panel1.Controls.Add(tabMain);
            // 
            // splitContainerMain.Panel2
            // 
            splitContainerMain.Panel2.Controls.Add(blazorWebView1);
            splitContainerMain.Size = new Size(1186, 595);
            splitContainerMain.SplitterDistance = 353;
            splitContainerMain.TabIndex = 5;
            // 
            // kListViewBottomLog
            // 
            kListViewBottomLog.Columns.AddRange(new ColumnHeader[] { columnHeader1, columnHeader2, columnHeader3 });
            kListViewBottomLog.Dock = DockStyle.Bottom;
            kListViewBottomLog.FullRowSelect = true;
            kListViewBottomLog.GridLines = true;
            kListViewBottomLog.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            kListViewBottomLog.HideSelection = false;
            kListViewBottomLog.Location = new Point(0, 602);
            kListViewBottomLog.Name = "kListViewBottomLog";
            kListViewBottomLog.Size = new Size(1186, 131);
            kListViewBottomLog.TabIndex = 6;
            kListViewBottomLog.View = View.Details;
            kListViewBottomLog.DoubleClick += kListViewBottomLog_DoubleClick;
            // 
            // columnHeader1
            // 
            columnHeader1.Text = "Date";
            columnHeader1.Width = 150;
            // 
            // columnHeader2
            // 
            columnHeader2.Text = "Type";
            columnHeader2.Width = 100;
            // 
            // columnHeader3
            // 
            columnHeader3.Text = "Message";
            columnHeader3.Width = 1000;
            // 
            // kSeparatorForLogView
            // 
            kSeparatorForLogView.Cursor = Cursors.SizeNS;
            kSeparatorForLogView.Dock = DockStyle.Bottom;
            kSeparatorForLogView.Location = new Point(0, 595);
            kSeparatorForLogView.Name = "kSeparatorForLogView";
            kSeparatorForLogView.Orientation = Orientation.Horizontal;
            kSeparatorForLogView.Size = new Size(1186, 7);
            kSeparatorForLogView.TabIndex = 7;
            kSeparatorForLogView.MouseDoubleClick += kSeparatorForLogView_MouseDoubleClick;
            kSeparatorForLogView.MouseDown += KSeparatorForLogView_MouseDown;
            kSeparatorForLogView.MouseMove += KSeparatorForLogView_MouseMove;
            kSeparatorForLogView.MouseUp += KSeparatorForLogView_MouseUp;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1186, 755);
            Controls.Add(splitContainerMain);
            Controls.Add(kSeparatorForLogView);
            Controls.Add(kListViewBottomLog);
            Controls.Add(kryptonStatusStrip1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Location = new Point(0, 0);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "folderchat";
            FormClosing += MainForm_FormClosing;
            Load += MainForm_Load;
            tabMain.ResumeLayout(false);
            tabPageDir.ResumeLayout(false);
            tabPageDir.PerformLayout();
            contextMenuTreeView.ResumeLayout(false);
            kryptonStatusStrip1.ResumeLayout(false);
            kryptonStatusStrip1.PerformLayout();
            splitContainerMain.Panel1.ResumeLayout(false);
            splitContainerMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerMain).EndInit();
            splitContainerMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)kSeparatorForLogView).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Microsoft.AspNetCore.Components.WebView.WindowsForms.BlazorWebView blazorWebView1;
        private TabControl tabMain;
        private TabPage tabPageDir;
        private Krypton.Toolkit.KryptonTreeView ktvFolderTree;
        private ContextMenuStrip contextMenuTreeView;
        private ToolStripMenuItem menuItemIndexing;
        private ToolStripMenuItem menuItemSummarize;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem menuItemRefresh;
        private Krypton.Toolkit.KryptonLabel lblRagStatus;
        private TabPage tabPageSettings;
        private Krypton.Toolkit.KryptonManager kryptonManager1;
        private ToolTip toolTip1;
        private Krypton.Toolkit.KryptonStatusStrip kryptonStatusStrip1;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private ToolStripStatusLabel toolStripStatusLabelApiStatus;
        private SplitContainer splitContainerMain;
        private Krypton.Toolkit.KryptonListView kListViewBottomLog;
        private ColumnHeader columnHeader1;
        private ColumnHeader columnHeader2;
        private ColumnHeader columnHeader3;
        private Krypton.Toolkit.KryptonSeparator kSeparatorForLogView;
    }
}
