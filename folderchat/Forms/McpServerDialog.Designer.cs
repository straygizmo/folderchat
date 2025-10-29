namespace folderchat.Forms
{
    partial class McpServerDialog
    {
        private System.ComponentModel.IContainer components = null;
        private Krypton.Toolkit.KryptonLabel lblName;
        private Krypton.Toolkit.KryptonTextBox txtName;
        private Krypton.Toolkit.KryptonLabel lblCommand;
        private Krypton.Toolkit.KryptonTextBox txtCommand;
        private Krypton.Toolkit.KryptonButton btnBrowseCommand;
        private Krypton.Toolkit.KryptonLabel lblArguments;
        private Krypton.Toolkit.KryptonTextBox txtArguments;
        private Krypton.Toolkit.KryptonLabel lblWorkingDirectory;
        private Krypton.Toolkit.KryptonTextBox txtWorkingDirectory;
        private Krypton.Toolkit.KryptonButton btnBrowseWorkingDirectory;
        private Krypton.Toolkit.KryptonLabel lblTransportType;
        private Krypton.Toolkit.KryptonComboBox cmbTransportType;
        private Krypton.Toolkit.KryptonLabel lblDescription;
        private Krypton.Toolkit.KryptonTextBox txtDescription;
        private Krypton.Toolkit.KryptonCheckBox chkEnabled;
        private Krypton.Toolkit.KryptonLabel lblEnvironmentVariables;
        private Krypton.Toolkit.KryptonDataGridView dgvEnvironmentVariables;
        private Krypton.Toolkit.KryptonButton btnOK;
        private Krypton.Toolkit.KryptonButton btnCancel;
        private Krypton.Toolkit.KryptonButton btnTest;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            lblName = new Krypton.Toolkit.KryptonLabel();
            txtName = new Krypton.Toolkit.KryptonTextBox();
            lblCommand = new Krypton.Toolkit.KryptonLabel();
            txtCommand = new Krypton.Toolkit.KryptonTextBox();
            btnBrowseCommand = new Krypton.Toolkit.KryptonButton();
            lblArguments = new Krypton.Toolkit.KryptonLabel();
            txtArguments = new Krypton.Toolkit.KryptonTextBox();
            lblWorkingDirectory = new Krypton.Toolkit.KryptonLabel();
            txtWorkingDirectory = new Krypton.Toolkit.KryptonTextBox();
            btnBrowseWorkingDirectory = new Krypton.Toolkit.KryptonButton();
            lblTransportType = new Krypton.Toolkit.KryptonLabel();
            cmbTransportType = new Krypton.Toolkit.KryptonComboBox();
            lblDescription = new Krypton.Toolkit.KryptonLabel();
            txtDescription = new Krypton.Toolkit.KryptonTextBox();
            chkEnabled = new Krypton.Toolkit.KryptonCheckBox();
            lblEnvironmentVariables = new Krypton.Toolkit.KryptonLabel();
            dgvEnvironmentVariables = new Krypton.Toolkit.KryptonDataGridView();
            dataGridViewTextBoxColumn1 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn2 = new DataGridViewTextBoxColumn();
            btnOK = new Krypton.Toolkit.KryptonButton();
            btnCancel = new Krypton.Toolkit.KryptonButton();
            btnTest = new Krypton.Toolkit.KryptonButton();
            ((System.ComponentModel.ISupportInitialize)cmbTransportType).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvEnvironmentVariables).BeginInit();
            SuspendLayout();
            // 
            // lblName
            // 
            lblName.Location = new Point(12, 15);
            lblName.Name = "lblName";
            lblName.Size = new Size(46, 20);
            lblName.TabIndex = 0;
            lblName.Values.Text = "Name:";
            // 
            // txtName
            // 
            txtName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtName.Location = new Point(120, 12);
            txtName.Name = "txtName";
            txtName.Size = new Size(479, 25);
            txtName.StateCommon.Border.Rounding = 3F;
            txtName.TabIndex = 1;
            // 
            // lblCommand
            // 
            lblCommand.Location = new Point(12, 44);
            lblCommand.Name = "lblCommand";
            lblCommand.Size = new Size(70, 20);
            lblCommand.TabIndex = 2;
            lblCommand.Values.Text = "Command:";
            // 
            // txtCommand
            // 
            txtCommand.Location = new Point(120, 41);
            txtCommand.Name = "txtCommand";
            txtCommand.Size = new Size(439, 25);
            txtCommand.StateCommon.Border.Rounding = 3F;
            txtCommand.TabIndex = 3;
            // 
            // btnBrowseCommand
            // 
            btnBrowseCommand.Location = new Point(565, 41);
            btnBrowseCommand.Name = "btnBrowseCommand";
            btnBrowseCommand.Size = new Size(30, 23);
            btnBrowseCommand.StateNormal.Border.Rounding = 3F;
            btnBrowseCommand.TabIndex = 4;
            btnBrowseCommand.Values.DropDownArrowColor = Color.Empty;
            btnBrowseCommand.Values.Text = "...";
            btnBrowseCommand.Click += btnBrowseCommand_Click;
            // 
            // lblArguments
            // 
            lblArguments.Location = new Point(12, 73);
            lblArguments.Name = "lblArguments";
            lblArguments.Size = new Size(73, 20);
            lblArguments.TabIndex = 5;
            lblArguments.Values.Text = "Arguments:";
            // 
            // txtArguments
            // 
            txtArguments.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtArguments.Location = new Point(120, 70);
            txtArguments.Multiline = true;
            txtArguments.Name = "txtArguments";
            txtArguments.ScrollBars = ScrollBars.Both;
            txtArguments.Size = new Size(479, 100);
            txtArguments.StateCommon.Border.Rounding = 3F;
            txtArguments.TabIndex = 6;
            // 
            // lblWorkingDirectory
            // 
            lblWorkingDirectory.Location = new Point(12, 179);
            lblWorkingDirectory.Name = "lblWorkingDirectory";
            lblWorkingDirectory.Size = new Size(113, 20);
            lblWorkingDirectory.TabIndex = 7;
            lblWorkingDirectory.Values.Text = "Working Directory:";
            // 
            // txtWorkingDirectory
            // 
            txtWorkingDirectory.Location = new Point(120, 176);
            txtWorkingDirectory.Name = "txtWorkingDirectory";
            txtWorkingDirectory.Size = new Size(439, 25);
            txtWorkingDirectory.StateCommon.Border.Rounding = 3F;
            txtWorkingDirectory.TabIndex = 8;
            // 
            // btnBrowseWorkingDirectory
            // 
            btnBrowseWorkingDirectory.Location = new Point(565, 176);
            btnBrowseWorkingDirectory.Name = "btnBrowseWorkingDirectory";
            btnBrowseWorkingDirectory.Size = new Size(30, 23);
            btnBrowseWorkingDirectory.StateNormal.Border.Rounding = 3F;
            btnBrowseWorkingDirectory.TabIndex = 9;
            btnBrowseWorkingDirectory.Values.DropDownArrowColor = Color.Empty;
            btnBrowseWorkingDirectory.Values.Text = "...";
            btnBrowseWorkingDirectory.Click += btnBrowseWorkingDirectory_Click;
            // 
            // lblTransportType
            // 
            lblTransportType.Location = new Point(12, 208);
            lblTransportType.Name = "lblTransportType";
            lblTransportType.Size = new Size(95, 20);
            lblTransportType.TabIndex = 10;
            lblTransportType.Values.Text = "Transport Type:";
            // 
            // cmbTransportType
            // 
            cmbTransportType.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbTransportType.DropDownWidth = 121;
            cmbTransportType.Items.AddRange(new object[] { "Stdio", "HTTP" });
            cmbTransportType.Location = new Point(120, 205);
            cmbTransportType.Name = "cmbTransportType";
            cmbTransportType.Size = new Size(121, 24);
            cmbTransportType.StateCommon.ComboBox.Border.Rounding = 3F;
            cmbTransportType.TabIndex = 11;
            cmbTransportType.Text = "Stdio";
            // 
            // lblDescription
            // 
            lblDescription.Location = new Point(12, 237);
            lblDescription.Name = "lblDescription";
            lblDescription.Size = new Size(75, 20);
            lblDescription.TabIndex = 12;
            lblDescription.Values.Text = "Description:";
            // 
            // txtDescription
            // 
            txtDescription.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtDescription.Location = new Point(120, 234);
            txtDescription.Multiline = true;
            txtDescription.Name = "txtDescription";
            txtDescription.Size = new Size(479, 50);
            txtDescription.StateCommon.Border.Rounding = 3F;
            txtDescription.TabIndex = 13;
            // 
            // chkEnabled
            // 
            chkEnabled.Checked = true;
            chkEnabled.CheckState = CheckState.Checked;
            chkEnabled.Location = new Point(120, 290);
            chkEnabled.Name = "chkEnabled";
            chkEnabled.Size = new Size(67, 20);
            chkEnabled.TabIndex = 14;
            chkEnabled.Values.Text = "Enabled";
            // 
            // lblEnvironmentVariables
            // 
            lblEnvironmentVariables.Location = new Point(12, 316);
            lblEnvironmentVariables.Name = "lblEnvironmentVariables";
            lblEnvironmentVariables.Size = new Size(135, 20);
            lblEnvironmentVariables.TabIndex = 15;
            lblEnvironmentVariables.Values.Text = "Environment Variables:";
            // 
            // dgvEnvironmentVariables
            // 
            dgvEnvironmentVariables.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            dgvEnvironmentVariables.BorderStyle = BorderStyle.None;
            dgvEnvironmentVariables.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvEnvironmentVariables.Columns.AddRange(new DataGridViewColumn[] { dataGridViewTextBoxColumn1, dataGridViewTextBoxColumn2 });
            dgvEnvironmentVariables.Location = new Point(120, 342);
            dgvEnvironmentVariables.Name = "dgvEnvironmentVariables";
            dgvEnvironmentVariables.Size = new Size(479, 120);
            dgvEnvironmentVariables.TabIndex = 16;
            // 
            // dataGridViewTextBoxColumn1
            // 
            dataGridViewTextBoxColumn1.HeaderText = "Key";
            dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            dataGridViewTextBoxColumn1.Width = 170;
            // 
            // dataGridViewTextBoxColumn2
            // 
            dataGridViewTextBoxColumn2.HeaderText = "Value";
            dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            dataGridViewTextBoxColumn2.Width = 240;
            // 
            // btnOK
            // 
            btnOK.DialogResult = DialogResult.OK;
            btnOK.Location = new Point(397, 477);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(77, 25);
            btnOK.StateNormal.Border.Rounding = 3F;
            btnOK.TabIndex = 18;
            btnOK.Values.DropDownArrowColor = Color.Empty;
            btnOK.Values.Text = "OK";
            btnOK.Click += btnOK_Click;
            // 
            // btnCancel
            // 
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Location = new Point(478, 477);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(77, 25);
            btnCancel.StateNormal.Border.Rounding = 3F;
            btnCancel.TabIndex = 19;
            btnCancel.Values.DropDownArrowColor = Color.Empty;
            btnCancel.Values.Text = "Cancel";
            btnCancel.Click += btnCancel_Click;
            // 
            // btnTest
            // 
            btnTest.Location = new Point(120, 477);
            btnTest.Name = "btnTest";
            btnTest.Size = new Size(75, 25);
            btnTest.StateNormal.Border.Rounding = 3F;
            btnTest.TabIndex = 17;
            btnTest.Values.DropDownArrowColor = Color.Empty;
            btnTest.Values.Text = "Test";
            btnTest.Click += btnTest_Click;
            // 
            // McpServerDialog
            // 
            AcceptButton = btnOK;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = btnCancel;
            ClientSize = new Size(617, 518);
            Controls.Add(btnCancel);
            Controls.Add(btnOK);
            Controls.Add(btnTest);
            Controls.Add(dgvEnvironmentVariables);
            Controls.Add(lblEnvironmentVariables);
            Controls.Add(chkEnabled);
            Controls.Add(txtDescription);
            Controls.Add(lblDescription);
            Controls.Add(cmbTransportType);
            Controls.Add(lblTransportType);
            Controls.Add(btnBrowseWorkingDirectory);
            Controls.Add(txtWorkingDirectory);
            Controls.Add(lblWorkingDirectory);
            Controls.Add(txtArguments);
            Controls.Add(lblArguments);
            Controls.Add(btnBrowseCommand);
            Controls.Add(txtCommand);
            Controls.Add(lblCommand);
            Controls.Add(txtName);
            Controls.Add(lblName);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Location = new Point(0, 0);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "McpServerDialog";
            StartPosition = FormStartPosition.CenterParent;
            StateCommon.Border.Rounding = 8F;
            Text = "MCP Server Configuration";
            ((System.ComponentModel.ISupportInitialize)cmbTransportType).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvEnvironmentVariables).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
    }
}