using folderchat.Models;
using folderchat.Services;
using folderchat.Services.Mcp;
using System.Data;
using System.Text.Json;

namespace folderchat.Forms
{
    public partial class McpSettingsControl : UserControl
    {
        private IMcpService? _mcpService;
        private McpServerCollection _mcpServerCollection = new();
        private MainForm? _mainForm;

        public McpSettingsControl()
        {
            InitializeComponent();
        }

        public void Initialize(MainForm mainForm)
        {
            _mainForm = mainForm;
            InitializeMcpComponents();
        }

        public void LoadSettings()
        {
            LoadMcpServers();
        }

        public void SaveSettings()
        {
            SaveMcpServers();
        }

        public void UpdateUILabels()
        {
            var loc = Program.LocalizationService;
            if (loc == null) return;

            // Update MCP buttons
            btnAddMcpServer.Values.Text = loc.GetString("Add");
            btnEditMcpServer.Values.Text = loc.GetString("Edit");
            btnRemoveMcpServer.Values.Text = loc.GetString("Remove");
        }

        #region MCP Server Management

        private void InitializeMcpComponents()
        {
            // Initialize MCP service
            _mcpService = new McpService();
            _mcpService.LogMessage += (sender, message) =>
            {
                AppendMcpLog(message);
            };
            _mcpService.ServerStatusChanged += (sender, args) =>
            {
                UpdateServerStatus(args);
            };

            // Setup DataGridView
            dgvMcpServers.AllowUserToAddRows = false;
            dgvMcpServers.AllowUserToDeleteRows = false;
            dgvMcpServers.ReadOnly = false;
            dgvMcpServers.Columns.Clear();
            dgvMcpServers.Columns.Add("Name", "Name");
            dgvMcpServers.Columns.Add("Status", "Status");
            dgvMcpServers.Columns.Add("Type", "Type");

            // Add checkbox column for Enabled
            var enabledColumn = new DataGridViewCheckBoxColumn
            {
                Name = "Enabled",
                HeaderText = "Enabled",
                Width = 50,
                ReadOnly = false
            };
            dgvMcpServers.Columns.Add(enabledColumn);

            dgvMcpServers.RowHeadersWidth = 25;
            dgvMcpServers.Columns[0].Width = 115;   // Name column
            dgvMcpServers.Columns[0].ReadOnly = true;
            dgvMcpServers.Columns[1].Width = 85;    // Status column
            dgvMcpServers.Columns[1].ReadOnly = true;
            dgvMcpServers.Columns[2].Width = 50;    // Type column
            dgvMcpServers.Columns[2].ReadOnly = true;

            // Handle checkbox cell click
            dgvMcpServers.CellContentClick += DgvMcpServers_CellContentClick;

            // Note: Event handlers are already wired up in Designer.cs
            // Do not add them here again to avoid duplicate event registration

            // Load MCP servers from settings
            LoadMcpServers();
        }

        private void LoadMcpServers()
        {
            try
            {
                var mcpServersJson = Properties.Settings.Default.MCP_Servers;
                if (!string.IsNullOrEmpty(mcpServersJson))
                {
                    _mcpServerCollection = JsonSerializer.Deserialize<McpServerCollection>(mcpServersJson) ?? new McpServerCollection();
                }
                else
                {
                    _mcpServerCollection = new McpServerCollection();
                }

                RefreshMcpServerList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading MCP servers: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveMcpServers()
        {
            try
            {
                var json = JsonSerializer.Serialize(_mcpServerCollection, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
                Properties.Settings.Default.MCP_Servers = json;
                Properties.Settings.Default.Save();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving MCP servers: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RefreshMcpServerList()
        {
            dgvMcpServers.Rows.Clear();
            foreach (var server in _mcpServerCollection.Servers)
            {
                var status = _mcpService?.IsServerLoaded(server.Id) ?? false ? "Connected" : "Disconnected";
                dgvMcpServers.Rows.Add(server.Name, status, server.TransportType.ToString(), server.IsEnabled);
                var row = dgvMcpServers.Rows[dgvMcpServers.Rows.Count - 1];
                row.Tag = server;
            }
        }

        private void BtnAddMcpServer_Click(object? sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("McpSettingsControl: BtnAddMcpServer_Click - Creating dialog");
            using var dialog = new McpServerDialog();
            System.Diagnostics.Debug.WriteLine("McpSettingsControl: BtnAddMcpServer_Click - Calling ShowDialog");
            var result = dialog.ShowDialog();
            System.Diagnostics.Debug.WriteLine($"McpSettingsControl: BtnAddMcpServer_Click - ShowDialog returned {result}");
            if (result == DialogResult.OK)
            {
                _mcpServerCollection.Servers.Add(dialog.ServerConfig);
                SaveMcpServers();
                RefreshMcpServerList();
                AppendMcpLog($"Added server: {dialog.ServerConfig.Name}");
            }
            System.Diagnostics.Debug.WriteLine("McpSettingsControl: BtnAddMcpServer_Click - Completed");
        }

        private void BtnEditMcpServer_Click(object? sender, EventArgs e)
        {
            if (dgvMcpServers.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a server to edit.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var server = dgvMcpServers.SelectedRows[0].Tag as McpServerConfig;
            if (server == null) return;

            System.Diagnostics.Debug.WriteLine("McpSettingsControl: BtnEditMcpServer_Click - Creating dialog");
            using var dialog = new McpServerDialog(server);
            System.Diagnostics.Debug.WriteLine("McpSettingsControl: BtnEditMcpServer_Click - Calling ShowDialog");
            var result = dialog.ShowDialog();
            System.Diagnostics.Debug.WriteLine($"McpSettingsControl: BtnEditMcpServer_Click - ShowDialog returned {result}");
            if (result == DialogResult.OK)
            {
                SaveMcpServers();
                RefreshMcpServerList();
                AppendMcpLog($"Updated server: {server.Name}");
            }
            System.Diagnostics.Debug.WriteLine("McpSettingsControl: BtnEditMcpServer_Click - Completed");
        }

        private void DgvMcpServers_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
        {
            // ヘッダー行のダブルクリックは無視
            if (e.RowIndex < 0)
            {
                return;
            }

            // ダブルクリックされた行を選択
            dgvMcpServers.ClearSelection();
            dgvMcpServers.Rows[e.RowIndex].Selected = true;

            // Editボタンと同等の処理を実行
            BtnEditMcpServer_Click(sender, e);
        }

        private void DgvMcpServers_KeyDown(object? sender, KeyEventArgs e)
        {
            // Deleteキーが押された場合
            if (e.KeyCode == Keys.Delete)
            {
                // Removeボタンと同等の処理を実行
                BtnRemoveMcpServer_Click(sender, e);
                e.Handled = true;
            }
        }

        private async void DgvMcpServers_CellContentClick(object? sender, DataGridViewCellEventArgs e)
        {
            // Check if the clicked cell is in the Enabled column
            if (e.RowIndex < 0 || e.ColumnIndex != 3) // Column 3 is the Enabled checkbox column
                return;

            // Get the server config from the row
            var row = dgvMcpServers.Rows[e.RowIndex];
            var server = row.Tag as McpServerConfig;
            if (server == null) return;

            // Commit the edit to get the new value
            dgvMcpServers.CommitEdit(DataGridViewDataErrorContexts.Commit);

            // Get the new checkbox value
            var isEnabled = (bool)(row.Cells[3].Value ?? false);

            // Update the server's enabled state
            server.IsEnabled = isEnabled;

            // Save the configuration
            SaveMcpServers();

            // Load or unload the server
            if (_mcpService != null)
            {
                try
                {
                    if (isEnabled)
                    {
                        AppendMcpLog($"Activating server: {server.Name}");
                        var success = await _mcpService.LoadServerAsync(server);
                        if (success)
                        {
                            AppendMcpLog($"Successfully activated: {server.Name}");
                        }
                        else
                        {
                            AppendMcpLog($"Failed to activate: {server.Name}");
                            // Revert checkbox if loading failed
                            server.IsEnabled = false;
                            row.Cells[3].Value = false;
                        }
                    }
                    else
                    {
                        AppendMcpLog($"Deactivating server: {server.Name}");
                        await _mcpService.UnloadServerAsync(server.Id);
                        AppendMcpLog($"Successfully deactivated: {server.Name}");
                    }

                    RefreshMcpServerList();

                    // Notify MainForm and WebView about the change
                    _mainForm?.NotifyMcpServersChanged();
                }
                catch (Exception ex)
                {
                    AppendMcpLog($"Error toggling server {server.Name}: {ex.Message}");
                    MessageBox.Show($"Error toggling server: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);

                    // Revert checkbox on error
                    server.IsEnabled = !isEnabled;
                    row.Cells[3].Value = !isEnabled;
                }
            }
        }

        private void BtnRemoveMcpServer_Click(object? sender, EventArgs e)
        {
            if (dgvMcpServers.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a server to remove.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var server = dgvMcpServers.SelectedRows[0].Tag as McpServerConfig;
            if (server == null) return;

            var result = MessageBox.Show($"Are you sure you want to remove '{server.Name}'", "Confirm Remove", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                // Unload server if connected
                if (_mcpService?.IsServerLoaded(server.Id) ?? false)
                {
                    _mcpService.UnloadServerAsync(server.Id).GetAwaiter().GetResult();
                }

                _mcpServerCollection.Servers.Remove(server);
                SaveMcpServers();
                RefreshMcpServerList();
                AppendMcpLog($"Removed server: {server.Name}");
            }
        }


        private void AppendMcpLog(string message)
        {
            if (rtbMcpLog.InvokeRequired)
            {
                rtbMcpLog.Invoke(() => AppendMcpLog(message));
                return;
            }

            var timestamp = DateTime.Now.ToString("HH:mm:ss");
            rtbMcpLog.AppendText($"[{timestamp}] {message}");
            rtbMcpLog.ScrollToCaret();

            // Limit log size
            if (rtbMcpLog.Lines.Length > 1000)
            {
                var lines = rtbMcpLog.Lines.Skip(500).ToArray();
                rtbMcpLog.Lines = lines;
            }
        }

        private void UpdateServerStatus(McpServerEventArgs args)
        {
            if (dgvMcpServers.InvokeRequired)
            {
                dgvMcpServers.Invoke(() => UpdateServerStatus(args));
                return;
            }

            foreach (DataGridViewRow row in dgvMcpServers.Rows)
            {
                var server = row.Tag as McpServerConfig;
                if (server != null && server.Id == args.ServerId)
                {
                    row.Cells[1].Value = args.Status.ToString();
                    break;
                }
            }
        }

        public IMcpService? GetMcpService()
        {
            return _mcpService;
        }

        public List<McpServerConfig> GetMcpServerConfigs()
        {
            return _mcpServerCollection?.Servers ?? new List<McpServerConfig>();
        }

        public void SaveMcpServerConfigs()
        {
            SaveMcpServers();
        }

        public void RefreshMcpServerUI()
        {
            if (dgvMcpServers.InvokeRequired)
            {
                dgvMcpServers.Invoke(() => RefreshMcpServerUI());
                return;
            }
            RefreshMcpServerList();
        }

        #endregion
    }
}
