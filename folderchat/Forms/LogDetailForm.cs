using Krypton.Toolkit;

namespace folderchat.Forms
{
    /// <summary>
    /// Log detail display form
    /// </summary>
    public partial class LogDetailForm : KryptonForm
    {
        public LogDetailForm(string date, string type, string message)
        {
            InitializeComponent();
            lblDate.Text = $"Date: {date}";
            lblType.Text = $"Type: {type}";
            txtMessage.Text = message;
            txtMessage.SelectionStart = 0;
            txtMessage.SelectionLength = 0;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
