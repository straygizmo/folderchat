using Krypton.Toolkit;

namespace folderchat.Forms.ProviderSettings
{
    public partial class OpenAIProviderSettingsControl : UserControl, IProviderSettingsControl
    {
        public OpenAIProviderSettingsControl()
        {
            InitializeComponent();
        }

        public void LoadSettings()
        {
            txtBaseUrl.Text = Properties.Settings.Default.OpenAI_BaseUrl;
            txtApiKey.Text = Properties.Settings.Default.OpenAI_ApiKey;

            // Load model list
            LoadModelList();
            chkSupportSystemMessage.Checked = Properties.Settings.Default.OpenAI_SupportsSystemRole;
        }

        public void SaveSettings()
        {
            Properties.Settings.Default.OpenAI_BaseUrl = txtBaseUrl.Text;
            Properties.Settings.Default.OpenAI_ApiKey = txtApiKey.Text;

            // Save selected model if any
            if (cmbModelList.SelectedItem != null)
            {
                Properties.Settings.Default.OpenAI_SelectedModel = cmbModelList.SelectedItem.ToString();
            }

            // Save model list
            SaveModelList();
            Properties.Settings.Default.OpenAI_SupportsSystemRole = chkSupportSystemMessage.Checked;
        }

        public bool ValidateSettings()
        {
            return !string.IsNullOrEmpty(txtApiKey.Text);
        }

        public string GetValidationError()
        {
            if (string.IsNullOrEmpty(txtApiKey.Text))
                return "API Key is required\n\n";
            return string.Empty;
        }

        private void LoadModelList()
        {
            cmbModelList.Items.Clear();

            if (!string.IsNullOrEmpty(Properties.Settings.Default.OpenAI_ModelList))
            {
                var models = Properties.Settings.Default.OpenAI_ModelList.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var model in models)
                {
                    cmbModelList.Items.Add(model.Trim());
                }
            }

            UpdateModelListLabel();

            // Select the saved model if exists
            if (!string.IsNullOrEmpty(Properties.Settings.Default.OpenAI_SelectedModel) &&
                cmbModelList.Items.Contains(Properties.Settings.Default.OpenAI_SelectedModel))
            {
                cmbModelList.SelectedItem = Properties.Settings.Default.OpenAI_SelectedModel;
            }
            else if (cmbModelList.Items.Count > 0)
            {
                cmbModelList.SelectedIndex = 0;
            }
        }

        private void SaveModelList()
        {
            var models = new List<string>();
            foreach (var item in cmbModelList.Items)
            {
                models.Add(item.ToString() ?? string.Empty);
            }
            Properties.Settings.Default.OpenAI_ModelList = string.Join(",", models);
        }

        private void UpdateModelListLabel()
        {
            lblModelList.Text = $"Model List ({cmbModelList.Items.Count}):";
        }

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            // Show input dialog to add a new model
            using (var form = new KryptonForm())
            {
                form.Text = "Add Model";
                form.Size = new Size(400, 150);
                form.StartPosition = FormStartPosition.CenterParent;
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.MaximizeBox = false;
                form.MinimizeBox = false;

                var label = new KryptonLabel
                {
                    Text = "Model Name:",
                    Location = new Point(10, 20),
                    AutoSize = true
                };

                var textBox = new KryptonTextBox
                {
                    Location = new Point(10, 45),
                    Size = new Size(360, 23)
                };

                var btnOk = new KryptonButton
                {
                    Text = "OK",
                    DialogResult = DialogResult.OK,
                    Location = new Point(200, 80),
                    Size = new Size(80, 30)
                };

                var btnCancel = new KryptonButton
                {
                    Text = "Cancel",
                    DialogResult = DialogResult.Cancel,
                    Location = new Point(290, 80),
                    Size = new Size(80, 30)
                };

                form.Controls.Add(label);
                form.Controls.Add(textBox);
                form.Controls.Add(btnOk);
                form.Controls.Add(btnCancel);
                form.AcceptButton = btnOk;
                form.CancelButton = btnCancel;

                if (form.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(textBox.Text))
                {
                    var modelName = textBox.Text.Trim();
                    if (!cmbModelList.Items.Contains(modelName))
                    {
                        cmbModelList.Items.Add(modelName);
                        cmbModelList.SelectedItem = modelName;
                        UpdateModelListLabel();
                    }
                }
            }
        }

        private async void BtnFetch_Click(object? sender, EventArgs e)
        {
            if (!ValidateSettings())
            {
                KryptonMessageBox.Show(GetValidationError(), "Validation Error", (KryptonMessageBoxButtons)MessageBoxButtons.OK, (KryptonMessageBoxIcon)MessageBoxIcon.Warning);
                return;
            }

            try
            {
                btnFetch.Enabled = false;
                btnFetch.Values.Text = "Fetching...";

                // TODO: Implement API call to fetch models from OpenAI Compatible API
                // This would require making an HTTP request to the API
                await Task.Delay(1000); // Placeholder

                KryptonMessageBox.Show("Model fetching is not yet implemented.\n\n", "Information", (KryptonMessageBoxButtons)MessageBoxButtons.OK, (KryptonMessageBoxIcon)MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                KryptonMessageBox.Show($"Error fetching models: {ex.Message}\n\n", "Error", (KryptonMessageBoxButtons)MessageBoxButtons.OK, (KryptonMessageBoxIcon)MessageBoxIcon.Error);
            }
            finally
            {
                btnFetch.Enabled = true;
                btnFetch.Values.Text = "⚙ Fetch";
            }
        }

        private void BtnReset_Click(object? sender, EventArgs e)
        {
            var result = KryptonMessageBox.Show(
                "Are you sure you want to clear all models?\n\n",
                "Confirm Reset",
                (KryptonMessageBoxButtons)MessageBoxButtons.YesNo,
                (KryptonMessageBoxIcon)MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                cmbModelList.Items.Clear();
                UpdateModelListLabel();
            }
        }

        private void cmbModelList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
