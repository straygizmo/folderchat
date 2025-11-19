namespace folderchat.Forms
{
    partial class RagSettingsControl
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
            cmbGGUFModel = new Krypton.Toolkit.KryptonComboBox();
            nudMaxContextLength = new Krypton.Toolkit.KryptonNumericUpDown();
            lblTotalMaxContextLength = new Krypton.Toolkit.KryptonLabel();
            nudTopKChunks = new Krypton.Toolkit.KryptonNumericUpDown();
            lblTopKChunks = new Krypton.Toolkit.KryptonLabel();
            nudChunkOverlap = new Krypton.Toolkit.KryptonNumericUpDown();
            nudChunkSize = new Krypton.Toolkit.KryptonNumericUpDown();
            nudModelContextLength = new Krypton.Toolkit.KryptonNumericUpDown();
            txtEmbeddingModel = new Krypton.Toolkit.KryptonTextBox();
            txtEmbeddingUrl = new Krypton.Toolkit.KryptonTextBox();
            lblModelContextLength = new Krypton.Toolkit.KryptonLabel();
            lblChunkSize = new Krypton.Toolkit.KryptonLabel();
            lblChunkOverlap = new Krypton.Toolkit.KryptonLabel();
            rbEmbeddingAPI = new Krypton.Toolkit.KryptonRadioButton();
            rbEmbeddingGGUF = new Krypton.Toolkit.KryptonRadioButton();
            lblEmbeddingMethod = new Krypton.Toolkit.KryptonLabel();
            btnTestEmbedding = new Krypton.Toolkit.KryptonButton();
            lblEmbeddingUrl = new Krypton.Toolkit.KryptonLabel();
            lblEmbeddingModel = new Krypton.Toolkit.KryptonLabel();
            kryptonPanel1 = new Krypton.Toolkit.KryptonPanel();
            ((System.ComponentModel.ISupportInitialize)cmbGGUFModel).BeginInit();
            ((System.ComponentModel.ISupportInitialize)kryptonPanel1).BeginInit();
            kryptonPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // cmbGGUFModel
            // 
            cmbGGUFModel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cmbGGUFModel.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbGGUFModel.DropDownWidth = 295;
            cmbGGUFModel.Location = new Point(6, 55);
            cmbGGUFModel.Margin = new Padding(3, 2, 3, 2);
            cmbGGUFModel.Name = "cmbGGUFModel";
            cmbGGUFModel.Size = new Size(325, 24);
            cmbGGUFModel.StateCommon.ComboBox.Border.Rounding = 4F;
            cmbGGUFModel.TabIndex = 9;
            cmbGGUFModel.Visible = false;
            // 
            // nudMaxContextLength
            // 
            nudMaxContextLength.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            nudMaxContextLength.Increment = new decimal(new int[] { 500, 0, 0, 0 });
            nudMaxContextLength.Location = new Point(223, 273);
            nudMaxContextLength.Margin = new Padding(3, 2, 3, 2);
            nudMaxContextLength.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            nudMaxContextLength.Minimum = new decimal(new int[] { 1000, 0, 0, 0 });
            nudMaxContextLength.Name = "nudMaxContextLength";
            nudMaxContextLength.Size = new Size(108, 24);
            nudMaxContextLength.StateCommon.Border.Rounding = 4F;
            nudMaxContextLength.TabIndex = 21;
            nudMaxContextLength.Value = new decimal(new int[] { 8000, 0, 0, 0 });
            // 
            // lblTotalMaxContextLength
            // 
            lblTotalMaxContextLength.Location = new Point(16, 273);
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
            nudTopKChunks.Location = new Point(223, 245);
            nudTopKChunks.Margin = new Padding(3, 2, 3, 2);
            nudTopKChunks.Maximum = new decimal(new int[] { 50, 0, 0, 0 });
            nudTopKChunks.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            nudTopKChunks.Name = "nudTopKChunks";
            nudTopKChunks.Size = new Size(108, 24);
            nudTopKChunks.StateCommon.Border.Rounding = 4F;
            nudTopKChunks.TabIndex = 19;
            nudTopKChunks.Value = new decimal(new int[] { 5, 0, 0, 0 });
            // 
            // lblTopKChunks
            // 
            lblTopKChunks.Location = new Point(17, 243);
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
            nudChunkOverlap.Location = new Point(223, 199);
            nudChunkOverlap.Margin = new Padding(3, 2, 3, 2);
            nudChunkOverlap.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            nudChunkOverlap.Minimum = new decimal(new int[] { 0, 0, 0, 0 });
            nudChunkOverlap.Name = "nudChunkOverlap";
            nudChunkOverlap.Size = new Size(108, 24);
            nudChunkOverlap.StateCommon.Border.Rounding = 4F;
            nudChunkOverlap.TabIndex = 19;
            nudChunkOverlap.Value = new decimal(new int[] { 100, 0, 0, 0 });
            // 
            // nudChunkSize
            // 
            nudChunkSize.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            nudChunkSize.Increment = new decimal(new int[] { 50, 0, 0, 0 });
            nudChunkSize.Location = new Point(223, 173);
            nudChunkSize.Margin = new Padding(3, 2, 3, 2);
            nudChunkSize.Maximum = new decimal(new int[] { 4096, 0, 0, 0 });
            nudChunkSize.Minimum = new decimal(new int[] { 100, 0, 0, 0 });
            nudChunkSize.Name = "nudChunkSize";
            nudChunkSize.Size = new Size(108, 24);
            nudChunkSize.StateCommon.Border.Rounding = 4F;
            nudChunkSize.TabIndex = 17;
            nudChunkSize.Value = new decimal(new int[] { 500, 0, 0, 0 });
            // 
            // nudModelContextLength
            // 
            nudModelContextLength.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            nudModelContextLength.Increment = new decimal(new int[] { 256, 0, 0, 0 });
            nudModelContextLength.Location = new Point(223, 147);
            nudModelContextLength.Margin = new Padding(3, 2, 3, 2);
            nudModelContextLength.Maximum = new decimal(new int[] { 32768, 0, 0, 0 });
            nudModelContextLength.Minimum = new decimal(new int[] { 512, 0, 0, 0 });
            nudModelContextLength.Name = "nudModelContextLength";
            nudModelContextLength.Size = new Size(108, 24);
            nudModelContextLength.StateCommon.Border.Rounding = 4F;
            nudModelContextLength.TabIndex = 15;
            nudModelContextLength.Value = new decimal(new int[] { 2048, 0, 0, 0 });
            // 
            // txtEmbeddingModel
            // 
            txtEmbeddingModel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtEmbeddingModel.Location = new Point(6, 106);
            txtEmbeddingModel.Margin = new Padding(3, 2, 3, 2);
            txtEmbeddingModel.Name = "txtEmbeddingModel";
            txtEmbeddingModel.Size = new Size(325, 25);
            txtEmbeddingModel.StateCommon.Border.Rounding = 4F;
            txtEmbeddingModel.TabIndex = 13;
            txtEmbeddingModel.Text = "text-embedding-embeddinggemma-300m";
            // 
            // txtEmbeddingUrl
            // 
            txtEmbeddingUrl.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtEmbeddingUrl.Location = new Point(6, 55);
            txtEmbeddingUrl.Margin = new Padding(3, 2, 3, 2);
            txtEmbeddingUrl.Name = "txtEmbeddingUrl";
            txtEmbeddingUrl.Size = new Size(325, 25);
            txtEmbeddingUrl.StateCommon.Border.Rounding = 4F;
            txtEmbeddingUrl.TabIndex = 11;
            txtEmbeddingUrl.Text = "http://localhost:/1234";
            // 
            // lblModelContextLength
            // 
            lblModelContextLength.Location = new Point(17, 147);
            lblModelContextLength.Margin = new Padding(3, 2, 3, 2);
            lblModelContextLength.Name = "lblModelContextLength";
            lblModelContextLength.Size = new Size(135, 20);
            lblModelContextLength.TabIndex = 14;
            lblModelContextLength.Values.Text = "Model Context Length:";
            // 
            // lblChunkSize
            // 
            lblChunkSize.Location = new Point(17, 171);
            lblChunkSize.Margin = new Padding(3, 2, 3, 2);
            lblChunkSize.Name = "lblChunkSize";
            lblChunkSize.Size = new Size(73, 20);
            lblChunkSize.TabIndex = 16;
            lblChunkSize.Values.Text = "Chunk Size:";
            // 
            // lblChunkOverlap
            // 
            lblChunkOverlap.Location = new Point(17, 197);
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
            btnTestEmbedding.Location = new Point(40, 331);
            btnTestEmbedding.Margin = new Padding(3, 2, 3, 2);
            btnTestEmbedding.Name = "btnTestEmbedding";
            btnTestEmbedding.Size = new Size(255, 30);
            btnTestEmbedding.StateCommon.Border.Rounding = 4F;
            btnTestEmbedding.StateNormal.Border.Rounding = 3F;
            btnTestEmbedding.TabIndex = 22;
            btnTestEmbedding.Values.DropDownArrowColor = Color.Empty;
            btnTestEmbedding.Values.Text = "Test Embedding";
            btnTestEmbedding.Click += btnTestEmbedding_Click;
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
            // kryptonPanel1
            // 
            kryptonPanel1.Controls.Add(cmbGGUFModel);
            kryptonPanel1.Controls.Add(txtEmbeddingModel);
            kryptonPanel1.Controls.Add(lblEmbeddingMethod);
            kryptonPanel1.Controls.Add(lblEmbeddingModel);
            kryptonPanel1.Controls.Add(nudMaxContextLength);
            kryptonPanel1.Controls.Add(lblEmbeddingUrl);
            kryptonPanel1.Controls.Add(lblTotalMaxContextLength);
            kryptonPanel1.Controls.Add(btnTestEmbedding);
            kryptonPanel1.Controls.Add(nudTopKChunks);
            kryptonPanel1.Controls.Add(rbEmbeddingGGUF);
            kryptonPanel1.Controls.Add(lblTopKChunks);
            kryptonPanel1.Controls.Add(rbEmbeddingAPI);
            kryptonPanel1.Controls.Add(nudChunkOverlap);
            kryptonPanel1.Controls.Add(lblChunkOverlap);
            kryptonPanel1.Controls.Add(nudChunkSize);
            kryptonPanel1.Controls.Add(lblChunkSize);
            kryptonPanel1.Controls.Add(nudModelContextLength);
            kryptonPanel1.Controls.Add(lblModelContextLength);
            kryptonPanel1.Controls.Add(txtEmbeddingUrl);
            kryptonPanel1.Dock = DockStyle.Fill;
            kryptonPanel1.Location = new Point(0, 0);
            kryptonPanel1.Name = "kryptonPanel1";
            kryptonPanel1.Size = new Size(334, 492);
            kryptonPanel1.TabIndex = 23;
            // 
            // RagSettingsControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(kryptonPanel1);
            Name = "RagSettingsControl";
            Size = new Size(334, 492);
            ((System.ComponentModel.ISupportInitialize)cmbGGUFModel).EndInit();
            ((System.ComponentModel.ISupportInitialize)kryptonPanel1).EndInit();
            kryptonPanel1.ResumeLayout(false);
            kryptonPanel1.PerformLayout();
            ResumeLayout(false);

        }

        #endregion

        private Krypton.Toolkit.KryptonComboBox cmbGGUFModel;
        private Krypton.Toolkit.KryptonNumericUpDown nudMaxContextLength;
        private Krypton.Toolkit.KryptonLabel lblTotalMaxContextLength;
        private Krypton.Toolkit.KryptonNumericUpDown nudTopKChunks;
        private Krypton.Toolkit.KryptonLabel lblTopKChunks;
        private Krypton.Toolkit.KryptonNumericUpDown nudChunkOverlap;
        private Krypton.Toolkit.KryptonNumericUpDown nudChunkSize;
        private Krypton.Toolkit.KryptonNumericUpDown nudModelContextLength;
        private Krypton.Toolkit.KryptonTextBox txtEmbeddingModel;
        private Krypton.Toolkit.KryptonTextBox txtEmbeddingUrl;
        private Krypton.Toolkit.KryptonLabel lblModelContextLength;
        private Krypton.Toolkit.KryptonLabel lblChunkSize;
        private Krypton.Toolkit.KryptonLabel lblChunkOverlap;
        private Krypton.Toolkit.KryptonRadioButton rbEmbeddingAPI;
        private Krypton.Toolkit.KryptonRadioButton rbEmbeddingGGUF;
        private Krypton.Toolkit.KryptonLabel lblEmbeddingMethod;
        private Krypton.Toolkit.KryptonButton btnTestEmbedding;
        private Krypton.Toolkit.KryptonLabel lblEmbeddingUrl;
        private Krypton.Toolkit.KryptonLabel lblEmbeddingModel;
        private Krypton.Toolkit.KryptonPanel kryptonPanel1;
    }
}
