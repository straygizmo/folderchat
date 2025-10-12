using folderchat.Models;
using folderchat.Services.Mcp;
using Krypton.Toolkit;

namespace folderchat.Forms
{
    public partial class McpServerDialog : KryptonForm
    {
        public McpServerConfig ServerConfig { get; private set; }
        private IMcpService? _mcpService;

        public McpServerDialog(McpServerConfig? config = null)
        {
            InitializeComponent();

            // Initialize MCP service for testing
            _mcpService = new McpService();

            if (config != null)
            {
                ServerConfig = config;
                LoadConfig();
            }
            else
            {
                ServerConfig = new McpServerConfig();
            }

            // Debug: Track form lifecycle events
            this.Load += (s, e) => System.Diagnostics.Debug.WriteLine("McpServerDialog: Load event");
            this.Shown += (s, e) => System.Diagnostics.Debug.WriteLine("McpServerDialog: Shown event");
            this.FormClosing += (s, e) => System.Diagnostics.Debug.WriteLine($"McpServerDialog: FormClosing event - DialogResult={this.DialogResult}");
            this.FormClosed += (s, e) => System.Diagnostics.Debug.WriteLine("McpServerDialog: FormClosed event");
        }

        private void LoadConfig()
        {
            txtName.Text = ServerConfig.Name;
            txtCommand.Text = ServerConfig.Command;
            txtArguments.Text = ServerConfig.Arguments ?? string.Empty;
            txtWorkingDirectory.Text = ServerConfig.WorkingDirectory ?? string.Empty;
            txtDescription.Text = ServerConfig.Description ?? string.Empty;
            cmbTransportType.SelectedIndex = (int)ServerConfig.TransportType;
            chkEnabled.Checked = ServerConfig.IsEnabled;

            if (ServerConfig.EnvironmentVariables != null)
            {
                foreach (var kvp in ServerConfig.EnvironmentVariables)
                {
                    dgvEnvironmentVariables.Rows.Add(kvp.Key, kvp.Value);
                }
            }
        }

        private void SaveConfig()
        {
            ServerConfig.Name = txtName.Text.Trim();
            ServerConfig.Command = txtCommand.Text.Trim();
            ServerConfig.Arguments = string.IsNullOrWhiteSpace(txtArguments.Text) ? null : txtArguments.Text.Trim();
            ServerConfig.WorkingDirectory = string.IsNullOrWhiteSpace(txtWorkingDirectory.Text) ? null : txtWorkingDirectory.Text.Trim();
            ServerConfig.Description = string.IsNullOrWhiteSpace(txtDescription.Text) ? null : txtDescription.Text.Trim();
            ServerConfig.TransportType = (McpTransportType)cmbTransportType.SelectedIndex;
            ServerConfig.IsEnabled = chkEnabled.Checked;

            ServerConfig.EnvironmentVariables = new Dictionary<string, string>();
            foreach (DataGridViewRow row in dgvEnvironmentVariables.Rows)
            {
                if (!row.IsNewRow)
                {
                    var key = row.Cells[0].Value?.ToString()?.Trim();
                    var value = row.Cells[1].Value?.ToString() ?? string.Empty;
                    if (!string.IsNullOrEmpty(key))
                    {
                        ServerConfig.EnvironmentVariables[key] = value;
                    }
                }
            }
        }

        private void btnBrowseCommand_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Filter = "Executable files (*.exe;*.bat;*.cmd;*.ps1;*.py;*.js)|*.exe;*.bat;*.cmd;*.ps1;*.py;*.js|All files (*.*)|*.*";
                dialog.RestoreDirectory = true;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    txtCommand.Text = dialog.FileName;
                }
            }
        }

        private void btnBrowseWorkingDirectory_Click(object sender, EventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.ShowNewFolderButton = false;

                if (!string.IsNullOrWhiteSpace(txtWorkingDirectory.Text) && Directory.Exists(txtWorkingDirectory.Text))
                {
                    dialog.SelectedPath = txtWorkingDirectory.Text;
                }

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    txtWorkingDirectory.Text = dialog.SelectedPath;
                }
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("McpServerDialog: btnOK_Click called");

            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Please enter a server name.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Focus();
                DialogResult = DialogResult.None;
                return;
            }

            if (string.IsNullOrWhiteSpace(txtCommand.Text))
            {
                MessageBox.Show("Please enter a command.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCommand.Focus();
                DialogResult = DialogResult.None;
                return;
            }

            SaveConfig();
            System.Diagnostics.Debug.WriteLine("McpServerDialog: SaveConfig completed");
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("McpServerDialog: btnCancel_Click called");
            // DialogResult is already set by the button's DialogResult property
        }

        private async void btnTest_Click(object sender, EventArgs e)
        {
            if (_mcpService == null) return;

            // Validate required fields
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Please enter a server name.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtCommand.Text))
            {
                MessageBox.Show("Please enter a command.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCommand.Focus();
                return;
            }

            // Create temporary config from current form values
            var tempConfig = new McpServerConfig
            {
                Name = txtName.Text.Trim(),
                Command = txtCommand.Text.Trim(),
                Arguments = string.IsNullOrWhiteSpace(txtArguments.Text) ? null : txtArguments.Text.Trim(),
                WorkingDirectory = string.IsNullOrWhiteSpace(txtWorkingDirectory.Text) ? null : txtWorkingDirectory.Text.Trim(),
                Description = string.IsNullOrWhiteSpace(txtDescription.Text) ? null : txtDescription.Text.Trim(),
                TransportType = (McpTransportType)cmbTransportType.SelectedIndex,
                IsEnabled = chkEnabled.Checked,
                EnvironmentVariables = new Dictionary<string, string>()
            };

            foreach (DataGridViewRow row in dgvEnvironmentVariables.Rows)
            {
                if (!row.IsNewRow)
                {
                    var key = row.Cells[0].Value?.ToString()?.Trim();
                    var value = row.Cells[1].Value?.ToString() ?? string.Empty;
                    if (!string.IsNullOrEmpty(key))
                    {
                        tempConfig.EnvironmentVariables[key] = value;
                    }
                }
            }

            // Run test in background
            btnTest.Enabled = false;
            btnTest.Values.Text = "Testing...";

            await Task.Run(async () =>
            {
                try
                {
                    var success = await _mcpService.LoadServerAsync(tempConfig);

                    if (success)
                    {
                        var tools = await _mcpService.GetServerToolsAsync(tempConfig.Id);

                        Invoke(() =>
                        {
                            var message = $"✓ Connection Successful!\n\n" +
                                         $"Server: {tempConfig.Name}\n" +
                                         $"Tools available: {tools.Count}\n\n";

                            if (tools.Count > 0)
                            {
                                message += "Available tools:\n";
                                foreach (var tool in tools.Take(5))
                                {
                                    message += $"• {tool.Name}: {tool.Description}\n";
                                }
                                if (tools.Count > 5)
                                {
                                    message += $"... and {tools.Count - 5} more tools";
                                }
                            }

                            MessageBox.Show(message, "Test Successful",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                        });

                        // Unload after test
                        await _mcpService.UnloadServerAsync(tempConfig.Id);
                    }
                    else
                    {
                        Invoke(() =>
                        {
                            MessageBox.Show($"✗ Connection Failed!\n\nFailed to connect to server: {tempConfig.Name}\n\n" +
                                          "Please check your configuration and try again.",
                                "Test Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        });
                    }
                }
                catch (Exception ex)
                {
                    Invoke(() =>
                    {
                        var errorMessage = $"✗ Test Error!\n\nError: {ex.Message}";
                        if (ex.InnerException != null)
                        {
                            errorMessage += $"\n\nDetails: {ex.InnerException.Message}";
                        }

                        MessageBox.Show(errorMessage, "Test Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    });
                }
                finally
                {
                    Invoke(() =>
                    {
                        btnTest.Enabled = true;
                        btnTest.Values.Text = "Test";
                    });
                }
            });
        }
    }
}