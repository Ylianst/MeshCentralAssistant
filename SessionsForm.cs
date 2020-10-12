/*
Copyright 2009-2020 Intel Corporation

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System.Windows.Forms;

namespace MeshAssistant
{
    public partial class SessionsForm : Form
    {
        private MainForm parent;

        public SessionsForm(MainForm parent)
        {
            this.parent = parent;
            InitializeComponent();
            UpdateInfo();
        }

        private void SessionsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            parent.sessionsForm = null;
        }

        static char[] userDelimiters = new char[] { '/' };

        private string formatUserName(string user)
        {
            string[] parts = user.Split(userDelimiters);
            if (parts.Length == 3) return parts[2];
            return user;
        }

        public void UpdateInfo()
        {
            mainListView.Items.Clear();
            if ((parent.agent.DesktopSessions != null) && (parent.agent.DesktopSessions.Count > 0))
            {
                foreach (string user in parent.agent.DesktopSessions.Keys)
                {
                    mainListView.Items.Add(new ListViewItem(new string[3] { formatUserName(user), "Desktop", parent.agent.DesktopSessions[user].ToString() }));
                }
            }
            if ((parent.agent.TerminalSessions != null) && (parent.agent.TerminalSessions.Count > 0))
            {
                
                foreach (string user in parent.agent.TerminalSessions.Keys)
                {
                    mainListView.Items.Add(new ListViewItem(new string[3] { formatUserName(user), "Terminal", parent.agent.TerminalSessions[user].ToString() }));
                }
            }
            if ((parent.agent.FilesSessions != null) && (parent.agent.FilesSessions.Count > 0))
            {
                foreach (string user in parent.agent.FilesSessions.Keys)
                {
                    mainListView.Items.Add(new ListViewItem(new string[3] { formatUserName(user), "Files", parent.agent.FilesSessions[user].ToString() }));
                }
            }
            if ((parent.agent.TcpSessions != null) && (parent.agent.TcpSessions.Count > 0))
            {
                foreach (string user in parent.agent.TcpSessions.Keys)
                {
                    mainListView.Items.Add(new ListViewItem(new string[3] { formatUserName(user), "TCP relay", parent.agent.TcpSessions[user].ToString() }));
                }
            }
            if ((parent.agent.UdpSessions != null) && (parent.agent.UdpSessions.Count > 0))
            {
                foreach (string user in parent.agent.UdpSessions.Keys)
                {
                    mainListView.Items.Add(new ListViewItem(new string[3] { formatUserName(user), "UDP relay", parent.agent.UdpSessions[user].ToString() }));
                }
            }
            if (mainListView.Items.Count == 0)
            {
                mainListView.Items.Add(new ListViewItem(new string[3] { "(None)", "", "" }));
            }
        }

        private void closeButton_Click(object sender, System.EventArgs e)
        {
            Close();
        }
    }
}
