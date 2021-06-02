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
    public partial class BrowserForm : Form
    {
        private MainForm parent;

        public BrowserForm(MainForm parent)
        {
            this.parent = parent;
            InitializeComponent();
        }

        public void setUrl(string url)
        {
            mainBrowser.Url = new Uri(url);
        }

        private void BrowserForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            parent.CloseBrowser();
        }
    }
}
