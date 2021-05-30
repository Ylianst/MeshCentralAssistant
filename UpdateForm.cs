using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MeshAssistant
{
    public partial class UpdateForm : Form
    {
        private MainForm parent;

        public UpdateForm(MainForm parent)
        {
            this.parent = parent;
            InitializeComponent();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            this.parent.DownloadUpdateEx(true);
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.parent.DownloadUpdateEx(false);
        }
    }
}
