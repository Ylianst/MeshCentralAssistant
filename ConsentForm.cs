using System.Drawing;
using System.Windows.Forms;

namespace MeshAssistant
{
    public partial class ConsentForm : Form
    {
        private MainForm parent;
        private string orgtitle;
        private string message = "";

        public ConsentForm(MainForm parent)
        {
            this.parent = parent;
            InitializeComponent();
            this.orgtitle = this.Text;
        }

        public string userid;
        public MeshCentralTunnel tunnel;

        public string Message { set { message = value; updateInfo(); } }
        public string UserName { set { nameLabel.Text = value; updateInfo(); } }
        public string Title { set { this.Text = string.Format(Properties.Resources.TitleMerge, orgtitle, value); } }
        public Image UserImage { set { if (value == null) { mainPictureBox.Image = mainPictureBox.InitialImage; } else { mainPictureBox.Image = value; } } }

        private void updateInfo()
        {
            mainLabel.Text = string.Format(message, nameLabel.Text);
        }

        private void closeButton_Click(object sender, System.EventArgs e)
        {
            tunnel.ConsentRejected();
            Close();
        }

        private void ConsentForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            parent.consentForm = null;
        }

        private void okButton_Click(object sender, System.EventArgs e)
        {
            tunnel.ConsentAccepted();
            Close();
        }
    }
}
