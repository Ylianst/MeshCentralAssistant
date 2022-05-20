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
    public partial class GuestSharingForm : Form
    {
        MainForm parent;

        public GuestSharingForm(MainForm parent)
        {
            this.parent = parent;
            InitializeComponent();
            UpdateInfo();
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void copyLinkButton_Click(object sender, EventArgs e)
        {
            if (linkTextBox.Text != "") { Clipboard.SetText(linkTextBox.Text); }
        }

        public void UpdateInfo()
        {
            // Update state from the mesh agent
            if ((parent.mcagent != null) && (parent.mcagent.selfSharingUrl != null))
            {
                linkTextBox.Text = parent.mcagent.selfSharingUrl;
                desktopCheckBox.Checked = ((parent.mcagent.selfSharingFlags & 2) != 0);
                viewOnlyCheckBox.Checked = parent.mcagent.selfSharingViewOnly;
                terminalCheckBox.Checked = ((parent.mcagent.selfSharingFlags & 1) != 0);
                filesCheckBox.Checked = ((parent.mcagent.selfSharingFlags & 4) != 0);
            }
            else
            {
                linkTextBox.Text = "";
            }

            // Update the UI state
            copyLinkButton.Enabled = (linkTextBox.Text != "");
            cancelSharingButton.Enabled = (linkTextBox.Text != "");
            if (linkTextBox.Text == "")
            {
                desktopCheckBox.Enabled = true;
                viewOnlyCheckBox.Enabled = desktopCheckBox.Checked;
                terminalCheckBox.Enabled = true;
                filesCheckBox.Enabled = true;
                createLinkButton.Enabled = (desktopCheckBox.Checked || terminalCheckBox.Checked || filesCheckBox.Checked);
            }
            else
            {
                desktopCheckBox.Enabled = false;
                viewOnlyCheckBox.Enabled = false;
                terminalCheckBox.Enabled = false;
                filesCheckBox.Enabled = false;
                createLinkButton.Enabled = false;
            }
        }

        private void desktopCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            UpdateInfo();
        }

        private void GuestSharingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            parent.guestSharingForm = null;
        }

        private void createLinkButton_Click(object sender, EventArgs e)
        {
            int flags = 0;
            if (desktopCheckBox.Checked) { flags += 2; }
            if (terminalCheckBox.Checked) { flags += 1; }
            if (filesCheckBox.Checked) { flags += 4; }
            if (parent.mcagent != null) { parent.mcagent.sendRequestGuestSharing(flags, viewOnlyCheckBox.Checked); }
        }

        private void cancelSharingButton_Click(object sender, EventArgs e)
        {
            if (parent.mcagent != null) { parent.mcagent.sendRequestGuestSharing(0, false); }
        }
    }
}
