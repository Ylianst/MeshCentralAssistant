/*
Copyright 2009-2022 Intel Corporation

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

using System;
using System.Text;
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
            Translate.TranslateControl(this);
            Translate.TranslateListView(mainListView);
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

        private string getUserName(string userid)
        {
            string[] useridsplit = userid.Split('/');
            userid = useridsplit[0] + '/' + useridsplit[1] + '/' + useridsplit[2];
            string guestname = "";
            if ((useridsplit.Length == 4) && (useridsplit[3].StartsWith("guest:"))) { guestname = " - " + UTF8Encoding.UTF8.GetString(Convert.FromBase64String(useridsplit[3].Substring(6))); }
            if ((parent.mcagent != null) && (parent.mcagent.userrealname != null) && (parent.mcagent.userrealname.ContainsKey(userid))) { return (string)parent.mcagent.userrealname[userid] + guestname; }
            if ((parent.mcagent != null) && (parent.mcagent.usernames != null) && (parent.mcagent.usernames.ContainsKey(userid))) { return (string)parent.mcagent.usernames[userid] + guestname; }
            return useridsplit[2] + guestname;
        }

        private string getUserName2(string userid)
        {
            string[] useridsplit = userid.Split('/');
            userid = useridsplit[0] + '/' + useridsplit[1] + '/' + useridsplit[2];
            string guestname = "";
            if ((useridsplit.Length == 4) && (useridsplit[3].StartsWith("guest:"))) { guestname = " - " + UTF8Encoding.UTF8.GetString(Convert.FromBase64String(useridsplit[3].Substring(6))); }
            if ((parent.agent != null) && (parent.agent.userrealname != null) && (parent.agent.userrealname.ContainsKey(userid))) { return (string)parent.agent.userrealname[userid] + guestname; }
            if ((parent.agent != null) && (parent.agent.usernames != null) && (parent.agent.usernames.ContainsKey(userid))) { return (string)parent.agent.usernames[userid] + guestname; }
            return useridsplit[2] + guestname;
        }

        public void UpdateInfo()
        {
            mainListView.Items.Clear();

            if ((parent.mcagent != null) && (parent.currentAgentName != null) && (parent.currentAgentName.Equals("~")))
            {
                if ((parent.mcagent.DesktopSessions != null) && (parent.mcagent.DesktopSessions.Count > 0))
                {
                    foreach (string user in parent.mcagent.DesktopSessions.Keys)
                    {
                        mainListView.Items.Add(new ListViewItem(new string[3] { getUserName(user), Translate.T(Properties.Resources.Desktop), parent.mcagent.DesktopSessions[user].ToString() }));
                    }
                }
                if ((parent.mcagent.TerminalSessions != null) && (parent.mcagent.TerminalSessions.Count > 0))
                {

                    foreach (string user in parent.mcagent.TerminalSessions.Keys)
                    {
                        mainListView.Items.Add(new ListViewItem(new string[3] { getUserName(user), Translate.T(Properties.Resources.Terminal), parent.mcagent.TerminalSessions[user].ToString() }));
                    }
                }
                if ((parent.mcagent.FilesSessions != null) && (parent.mcagent.FilesSessions.Count > 0))
                {
                    foreach (string user in parent.mcagent.FilesSessions.Keys)
                    {
                        mainListView.Items.Add(new ListViewItem(new string[3] { getUserName(user), Translate.T(Properties.Resources.Files), parent.mcagent.FilesSessions[user].ToString() }));
                    }
                }
                if ((parent.mcagent.TcpSessions != null) && (parent.mcagent.TcpSessions.Count > 0))
                {
                    foreach (string user in parent.mcagent.TcpSessions.Keys)
                    {
                        mainListView.Items.Add(new ListViewItem(new string[3] { getUserName(user), Translate.T(Properties.Resources.TCPrelay), parent.mcagent.TcpSessions[user].ToString() }));
                    }
                }
                if ((parent.mcagent.UdpSessions != null) && (parent.mcagent.UdpSessions.Count > 0))
                {
                    foreach (string user in parent.mcagent.UdpSessions.Keys)
                    {
                        mainListView.Items.Add(new ListViewItem(new string[3] { getUserName(user), Translate.T(Properties.Resources.UDPrelay), parent.mcagent.UdpSessions[user].ToString() }));
                    }
                }
            }
            else if (parent.agent != null)
            {
                if ((parent.agent.DesktopSessions != null) && (parent.agent.DesktopSessions.Count > 0))
                {
                    foreach (string user in parent.agent.DesktopSessions.Keys)
                    {
                        mainListView.Items.Add(new ListViewItem(new string[3] { getUserName2(user), Translate.T(Properties.Resources.Desktop), parent.agent.DesktopSessions[user].ToString() }));
                    }
                }
                if ((parent.agent.TerminalSessions != null) && (parent.agent.TerminalSessions.Count > 0))
                {

                    foreach (string user in parent.agent.TerminalSessions.Keys)
                    {
                        mainListView.Items.Add(new ListViewItem(new string[3] { getUserName2(user), Translate.T(Properties.Resources.Terminal), parent.agent.TerminalSessions[user].ToString() }));
                    }
                }
                if ((parent.agent.FilesSessions != null) && (parent.agent.FilesSessions.Count > 0))
                {
                    foreach (string user in parent.agent.FilesSessions.Keys)
                    {
                        mainListView.Items.Add(new ListViewItem(new string[3] { getUserName2(user), Translate.T(Properties.Resources.Files), parent.agent.FilesSessions[user].ToString() }));
                    }
                }
                if ((parent.agent.TcpSessions != null) && (parent.agent.TcpSessions.Count > 0))
                {
                    foreach (string user in parent.agent.TcpSessions.Keys)
                    {
                        mainListView.Items.Add(new ListViewItem(new string[3] { getUserName2(user), Translate.T(Properties.Resources.TCPrelay), parent.agent.TcpSessions[user].ToString() }));
                    }
                }
                if ((parent.agent.UdpSessions != null) && (parent.agent.UdpSessions.Count > 0))
                {
                    foreach (string user in parent.agent.UdpSessions.Keys)
                    {
                        mainListView.Items.Add(new ListViewItem(new string[3] { getUserName2(user), Translate.T(Properties.Resources.UDPrelay), parent.agent.UdpSessions[user].ToString() }));
                    }
                }
            }

            if (mainListView.Items.Count == 0)
            {
                mainListView.Items.Add(new ListViewItem(new string[3] { Translate.T(Properties.Resources.PNoneP), "", "" }));
            }
        }

        private void closeButton_Click(object sender, System.EventArgs e)
        {
            Close();
        }
    }
}
