using System.Drawing;
using System.Windows.Forms;

namespace MeshAssistant
{
    public partial class NotifyForm : Form
    {
        private MainForm parent;
        private string orgtitle;

        public NotifyForm(MainForm parent)
        {
            this.parent = parent;
            InitializeComponent();
            this.orgtitle = this.Text;
        }

        public string userid;

        public string Message { set { maintTextBox.Text = value; } }
        public string UserName { set { nameLabel.Text = value; } }
        public string Title { set { this.Text = orgtitle + " - " + value; } }
        public Image UserImage { set { if (value == null) { mainPictureBox.Image = mainPictureBox.InitialImage; } else { mainPictureBox.Image = value; } } }

        private void NotifyForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            parent.notifyForm = null;
        }

        private void closeButton_Click(object sender, System.EventArgs e)
        {
            Close();
        }
    }
}
