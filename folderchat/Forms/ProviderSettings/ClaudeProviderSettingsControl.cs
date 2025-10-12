using Krypton.Toolkit;

namespace folderchat.Forms.ProviderSettings
{
    public partial class ClaudeProviderSettingsControl : UserControl, IProviderSettingsControl
    {
        public ClaudeProviderSettingsControl()
        {
            InitializeComponent();
        }

        public void LoadSettings()
        {
            txtApiUrl.Text = string.IsNullOrEmpty(Properties.Settings.Default.Claude_ApiUrl)
                ? "https://api.anthropic.com/v1"
                : Properties.Settings.Default.Claude_ApiUrl;
            txtApiKey.Text = Properties.Settings.Default.Claude_ApiKey;

            // Load model list
            LoadModelList();
            chkSupportSystemMessage.Checked = Properties.Settings.Default.Claude_SupportsSystemRole;
        }

        public void SaveSettings()
        {
            Properties.Settings.Default.Claude_ApiUrl = txtApiUrl.Text;
            Properties.Settings.Default.Claude_ApiKey = txtApiKey.Text;

            // Save selected model if any
            if (cmbModelList.SelectedItem != null)
            {
                Properties.Settings.Default.Claude_SelectedModel = cmbModelList.SelectedItem.ToString();
            }

            // Save model list
            SaveModelList();
            Properties.Settings.Default.Claude_SupportsSystemRole = chkSupportSystemMessage.Checked;
        }

        public bool ValidateSettings()
        {
            return !string.IsNullOrEmpty(txtApiUrl.Text) &&
                   !string.IsNullOrEmpty(txtApiKey.Text);
        }

        public string GetValidationError()
        {
            if (string.IsNullOrEmpty(txtApiUrl.Text))
                return "API URL is required\n\n";
            if (string.IsNullOrEmpty(txtApiKey.Text))
                return "API Key is required\n\n";
            return string.Empty;
        }

        private void LoadModelList()
        {
            cmbModelList.Items.Clear();

            if (!string.IsNullOrEmpty(Properties.Settings.Default.Claude_ModelList))
            {
                var models = Properties.Settings.Default.Claude_ModelList.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var model in models)
                {
                    cmbModelList.Items.Add(model.Trim());
                }
            }

            UpdateModelListLabel();

            // Select the saved model if exists
            if (!string.IsNullOrEmpty(Properties.Settings.Default.Claude_SelectedModel) &&
                cmbModelList.Items.Contains(Properties.Settings.Default.Claude_SelectedModel))
            {
                cmbModelList.SelectedItem = Properties.Settings.Default.Claude_SelectedModel;
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
            Properties.Settings.Default.Claude_ModelList = string.Join(",", models);
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

                // Claude API doesn't provide a models endpoint, so we'll use a predefined list
                // of known Claude models
                await Task.Delay(500); // Simulate API call

                var knownModels = new List<string>
                {
                    "claude-opus-4-1-20250805",
                    "claude-sonnet-4-5-20250929",
                    "claude-sonnet-4-20250514",
                };

                // Clear existing items and add known models
                cmbModelList.Items.Clear();
                foreach (var model in knownModels)
                {
                    cmbModelList.Items.Add(model);
                }

                UpdateModelListLabel();

                if (cmbModelList.Items.Count > 0)
                {
                    cmbModelList.SelectedIndex = 0;
                }

                KryptonMessageBox.Show($"Successfully loaded {knownModels.Count} known Claude models.\n\nNote: Claude API doesn't provide a models endpoint, so this is a list of known models.\n\n", "Success", (KryptonMessageBoxButtons)MessageBoxButtons.OK, (KryptonMessageBoxIcon)MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                KryptonMessageBox.Show($"Error loading models: {ex.Message}", "Error", (KryptonMessageBoxButtons)MessageBoxButtons.OK, (KryptonMessageBoxIcon)MessageBoxIcon.Error);
            }
            finally
            {
                btnFetch.Enabled = true;
                btnFetch.Values.Text = "⚙ Fetch";
            }
        }

        private void BtnReset_Click(object? sender, EventArgs e)
        {
            if (cmbModelList.SelectedItem == null)
            {
                KryptonMessageBox.Show("Please select a model to remove.\n\n", "No Selection", (KryptonMessageBoxButtons)MessageBoxButtons.OK, (KryptonMessageBoxIcon)MessageBoxIcon.Warning);
                return;
            }

            var selectedModel = cmbModelList.SelectedItem.ToString();
            var result = KryptonMessageBox.Show(
                $"Are you sure you want to remove '{selectedModel}'?\n\n",
                "Confirm Remove",
                (KryptonMessageBoxButtons)MessageBoxButtons.YesNo,
                (KryptonMessageBoxIcon)MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                cmbModelList.Items.Remove(cmbModelList.SelectedItem);
                UpdateModelListLabel();

                // Select first item if available
                if (cmbModelList.Items.Count > 0)
                {
                    cmbModelList.SelectedIndex = 0;
                }
            }
        }
    }
}
