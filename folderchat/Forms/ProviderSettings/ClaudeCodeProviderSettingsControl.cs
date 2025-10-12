using Krypton.Toolkit;

namespace folderchat.Forms.ProviderSettings
{
    public partial class ClaudeCodeProviderSettingsControl : UserControl, IProviderSettingsControl
    {
        public ClaudeCodeProviderSettingsControl()
        {
            InitializeComponent();
        }

        public void LoadSettings()
        {
            txtClaudeCodePath.Text = Properties.Settings.Default.ClaudeCode_CLIPath;
            var claudeModel = Properties.Settings.Default.ClaudeCode_Model;
            if (cmbClaudeModel.Items.Contains(claudeModel))
            {
                cmbClaudeModel.SelectedItem = claudeModel;
            }
            else
            {
                cmbClaudeModel.SelectedIndex = 0; // Default to first item
            }
            chkSupportSystemMessage.Checked = Properties.Settings.Default.ClaudeCode_SupportsSystemRole;
        }

        public void SaveSettings()
        {
            Properties.Settings.Default.ClaudeCode_CLIPath = txtClaudeCodePath.Text;
            Properties.Settings.Default.ClaudeCode_Model = cmbClaudeModel.SelectedItem?.ToString() ?? "sonnet";
            Properties.Settings.Default.ClaudeCode_SupportsSystemRole = chkSupportSystemMessage.Checked;
        }

        public bool ValidateSettings()
        {
            return !string.IsNullOrEmpty(txtClaudeCodePath.Text) && cmbClaudeModel.SelectedItem != null;
        }

        public string GetValidationError()
        {
            if (string.IsNullOrEmpty(txtClaudeCodePath.Text))
                return "Claude Code CLI Path is required";
            if (cmbClaudeModel.SelectedItem == null)
                return "Model selection is required";
            return string.Empty;
        }
    }
}
