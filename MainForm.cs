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

using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Drawing;
using System.Reflection;
using System.Collections;
using System.Diagnostics;
using System.Net.Security;
using System.Windows.Forms;
using System.ServiceProcess;
using System.Security.Principal;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Drawing.Drawing2D;

namespace MeshAssistant
{
    public partial class MainForm : Form
    {
        public string[] args;
        public int timerSlowDown = 0;
        public bool allowShowDisplay = false;
        public bool doclose = false;
        public bool helpRequested = false;
        public bool autoConnect = false;
        public MeshAgent agent = null; // This is a monitored agent
        public MeshCentralAgent mcagent = null; // This is the built-in agent
        public int queryNumber = 0;
        public SnapShotForm snapShotForm = null;
        public RequestHelpForm requestHelpForm = null;
        public SessionsForm sessionsForm = null;
        public MeInfoForm meInfoForm = null;
        public ConsoleForm consoleForm = null;
        public bool isAdministrator = false;
        public bool forceExit = false;
        public bool noUpdate = false;
        public ArrayList pastConsoleCommands = new ArrayList();
        public Dictionary<string, string> agents = null;
        public string selectedAgentName = null;
        public string currentAgentName = null;
        public NotifyForm notifyForm = null;
        public int embeddedMshLength = 0;
        public string selfExecutableHashHex = null;

        public MainForm(string[] args)
        {
            // Perform self update operations if any.
            this.args = args;
            bool startVisible = false;
            string update = null;
            string delete = null;
            foreach (string arg in this.args)
            {
                if ((arg.Length == 8) && (arg.ToLower() == "-visible")) { startVisible = true; }
                if ((arg.Length == 9) && (arg.ToLower() == "-noupdate")) { noUpdate = true; }
                if (arg.Length > 8 && arg.Substring(0, 8).ToLower() == "-update:") { update = arg.Substring(8); }
                if (arg.Length > 8 && arg.Substring(0, 8).ToLower() == "-delete:") { delete = arg.Substring(8); }
                if (arg.Length > 11 && arg.Substring(0, 11).ToLower() == "-agentname:") { selectedAgentName = arg.Substring(11); }
                if ((arg.Length == 12) && (arg.ToLower() == "-autoconnect")) { autoConnect = true; }
            }

            if (update != null)
            {
                // New args
                ArrayList args2 = new ArrayList();
                foreach (string a in args) { if (a.StartsWith("-update:") == false) { args2.Add(a); } }

                // Remove ".update.exe" and copy
                System.Threading.Thread.Sleep(1000);
                File.Copy(Assembly.GetEntryAssembly().Location, update, true);
                System.Threading.Thread.Sleep(1000);
                Process.Start(update, string.Join(" ", (string[])args2.ToArray(typeof(string))) + " -delete:" + Assembly.GetEntryAssembly().Location);
                this.forceExit = true;
                Application.Exit();
                return;
            }

            if (delete != null) { try { System.Threading.Thread.Sleep(1000); File.Delete(delete); } catch (Exception) { } }

            // If there is an embedded .msh file, write it out to "meshagent.msh"
            string msh = ExeHandler.GetMshFromExecutable(Process.GetCurrentProcess().MainModule.FileName, out embeddedMshLength);
            if (msh != null) { try { File.WriteAllText(MeshCentralAgent.getSelfFilename(".msh"), msh); } catch (Exception ex) { MessageBox.Show(ex.ToString()); Application.Exit(); return; } }
            computeSelfhash();

            // Set TLS 1.2
            ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

            InitializeComponent();

            // Check if the built-in agent will be activated
            currentAgentName = null;
            List<ToolStripItem> subMenus = new List<ToolStripItem>();
            string currentAgentSelection = Settings.GetRegValue("SelectedAgent", null);

            if (MeshCentralAgent.checkMshFile()) {
                mcagent = new MeshCentralAgent(this, "MeshCentralAssistant", selfExecutableHashHex);
                mcagent.onStateChanged += Mcagent_onStateChanged;
                mcagent.onNotify += Mcagent_onNotify;
                mcagent.onSelfUpdate += Agent_onSelfUpdate;
                mcagent.onSessionChanged += Mcagent_onSessionChanged; ;
                mcagent.onUserInfoChange += Mcagent_onUserInfoChange; ;
                if (currentAgentSelection.Equals("~")) { currentAgentName = "~"; }
                if (autoConnect == true) { currentAgentName = "~"; }
                ToolStripMenuItem m = new ToolStripMenuItem();
                m.Name = "AgentSelector-~";
                m.Text = "Direct Connect";
                m.Checked = ((currentAgentName != null) && (currentAgentName.Equals("~")));
                m.Click += agentSelection_Click;
                subMenus.Add(m);
            }

            // Get the list of agents on the system
            bool directConnectSeperator = false;
            agents = MeshAgent.GetAgentInfo(selectedAgentName);
            string[] agentNames = agents.Keys.ToArray();
            if (agents.Count > 0) {
                if ((currentAgentName == null) || (currentAgentName != "~"))
                {
                    currentAgentName = agentNames[0]; // Default
                    for (var i = 0; i < agentNames.Length; i++) { if (agentNames[i] == currentAgentSelection) { currentAgentName = agentNames[i]; } }
                }
                if (agentNames.Length > 1)
                {
                    for (var i = 0; i < agentNames.Length; i++)
                    {
                        if ((mcagent != null) && (!directConnectSeperator)) { subMenus.Add(new ToolStripSeparator()); directConnectSeperator = true; }
                        ToolStripMenuItem m = new ToolStripMenuItem();
                        m.Name = "AgentSelector-" + agentNames[i];
                        m.Text = agentNames[i];
                        m.Checked = (agentNames[i] == currentAgentName);
                        m.Click += agentSelection_Click;
                        subMenus.Add(m);
                    }
                }
            }
            agentSelectToolStripMenuItem.DropDownItems.AddRange(subMenus.ToArray());
            agentSelectToolStripMenuItem.Visible = (subMenus.Count > 1);

            connectToAgent();

            if (startVisible) { mainNotifyIcon_MouseClick(this, null); }
        }

        public void computeSelfhash()
        {
            // Hash our own executable
            if (embeddedMshLength != 0)
            { // Hash the entire file.
                byte[] selfHash;
                using (var sha384 = SHA384Managed.Create())
                {
                    using (var stream = File.OpenRead(System.Reflection.Assembly.GetEntryAssembly().Location))
                    {
                        selfHash = sha384.ComputeHash(stream);
                    }
                }
                selfExecutableHashHex = BitConverter.ToString(selfHash).Replace("-", string.Empty).ToLower();
            }
            else
            { // Hash the file, but skip the last portion of it where the .msh file would be.
                byte[] selfHash;
                using (var sha384 = SHA384Managed.Create())
                {
                    sha384.Initialize();
                    using (var stream = File.OpenRead(System.Reflection.Assembly.GetEntryAssembly().Location))
                    {
                        var fileLengthToHash = stream.Length - embeddedMshLength;
                        byte[] buf = new byte[65535];
                        while (fileLengthToHash > 0)
                        {
                            int l = stream.Read(buf, 0, (int)Math.Min(fileLengthToHash, buf.Length));
                            fileLengthToHash -= l;
                            sha384.TransformBlock(buf, 0, l, null, 0);
                        }
                        sha384.TransformFinalBlock(new byte[0], 0, 0);
                        selfHash = sha384.Hash;
                    }
                }
                selfExecutableHashHex = BitConverter.ToString(selfHash).Replace("-", string.Empty).ToLower();
            }
        }

        public delegate void ShowNotificationHandler(string userid, string title, string message);

        public void ShowNotification(string userid, string title, string message)
        {
            if (this.InvokeRequired) { this.Invoke(new ShowNotificationHandler(ShowNotification), userid, title, message); }
            string realname = userid.Split('/')[2];
            if (mcagent.userrealname.ContainsKey(userid)) { realname = mcagent.userrealname[userid]; }
            Image userImage = null;
            if (mcagent.userimages.ContainsKey(userid) && (mcagent.userimages[userid] != null)) { userImage = mcagent.userimages[userid]; }
            if (notifyForm == null) { notifyForm = new NotifyForm(this); notifyForm.Show(this); }
            notifyForm.userid = userid;
            notifyForm.Message = message;
            notifyForm.UserName = realname;
            notifyForm.UserImage = userImage;
            if ((title != null) && (title.Length > 0)) { notifyForm.Title = title; }
            notifyForm.Focus();
        }

        private void Mcagent_onSessionChanged()
        {
            if (InvokeRequired) { Invoke(new MeshCentralAgent.onSessionChangedHandler(Mcagent_onSessionChanged)); return; }
            updateBuiltinAgentStatus();
        }

        private void Mcagent_onUserInfoChange(string userid, int change)
        {
            if (InvokeRequired) { Invoke(new MeshCentralAgent.onUserInfoChangeHandler(Mcagent_onUserInfoChange), userid, change); return; }
            updateBuiltinAgentStatus();

            // If the notification dialog is showing, check if we can update the real name and/or image
            if ((notifyForm != null) && (notifyForm.userid == userid))
            {
                if (mcagent.userimages.ContainsKey(userid) && (mcagent.userimages[userid] != null)) { notifyForm.UserImage = mcagent.userimages[userid]; }
                if (mcagent.userrealname.ContainsKey(userid) && (mcagent.userrealname[userid] != null)) { notifyForm.UserName = mcagent.userrealname[userid]; }
            }
        }

        private void Mcagent_onNotify(string userid, string title, string msg)
        {
            if (InvokeRequired) { Invoke(new MeshCentralAgent.onNotifyHandler(Mcagent_onNotify), userid, title, msg); return; }
            ShowNotification(userid, title, msg);
            //MessageBox.Show(msg, title);
            //mainNotifyIcon.BalloonTipText = title + " - " + msg;
            //mainNotifyIcon.ShowBalloonTip(2000);
        }

        private void Mcagent_onStateChanged(int state)
        {
            if (InvokeRequired) { Invoke(new MeshCentralAgent.onStateChangedHandler(Mcagent_onStateChanged), state); return; }
            updateBuiltinAgentStatus();
        }

        private void agentSelection_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menu = (ToolStripMenuItem)sender;
            if (currentAgentName == menu.Name.Substring(14)) return;
            currentAgentName = menu.Name.Substring(14);
            foreach (Object obj in agentSelectToolStripMenuItem.DropDownItems) {
                if (obj.GetType() == typeof(ToolStripMenuItem))
                {
                    ToolStripMenuItem submenu = (ToolStripMenuItem)obj;
                    submenu.Checked = (submenu.Name.Substring(14) == currentAgentName);
                }
            }
            connectToAgent();
        }

        private string[] getSessionUserIdList()
        {
            ArrayList r = new ArrayList();
            if (mcagent.DesktopSessions != null) { foreach (string u in mcagent.DesktopSessions.Keys) { if (!r.Contains(u)) { r.Add(u); } } }
            if (mcagent.TerminalSessions != null) { foreach (string u in mcagent.TerminalSessions.Keys) { if (!r.Contains(u)) { r.Add(u); } } }
            if (mcagent.FilesSessions != null) { foreach (string u in mcagent.FilesSessions.Keys) { if (!r.Contains(u)) { r.Add(u); } } }
            if (mcagent.TcpSessions != null) { foreach (string u in mcagent.TcpSessions.Keys) { if (!r.Contains(u)) { r.Add(u); } } }
            if (mcagent.UdpSessions != null) { foreach (string u in mcagent.UdpSessions.Keys) { if (!r.Contains(u)) { r.Add(u); } } }
            return (string[])r.ToArray(typeof(string));
        }

        private string[] getSessionUserIdList2()
        {
            ArrayList r = new ArrayList();
            if (agent.DesktopSessions != null) { foreach (string u in agent.DesktopSessions.Keys) { if (!r.Contains(u)) { r.Add(u); } } }
            if (agent.TerminalSessions != null) { foreach (string u in agent.TerminalSessions.Keys) { if (!r.Contains(u)) { r.Add(u); } } }
            if (agent.FilesSessions != null) { foreach (string u in agent.FilesSessions.Keys) { if (!r.Contains(u)) { r.Add(u); } } }
            if (agent.TcpSessions != null) { foreach (string u in agent.TcpSessions.Keys) { if (!r.Contains(u)) { r.Add(u); } } }
            if (agent.UdpSessions != null) { foreach (string u in agent.UdpSessions.Keys) { if (!r.Contains(u)) { r.Add(u); } } }
            return (string[])r.ToArray(typeof(string));
        }


        public Image RoundCorners(Image StartImage, int CornerRadius, Color BackgroundColor)
        {
            CornerRadius *= 2;
            Bitmap RoundedImage = new Bitmap(StartImage.Width, StartImage.Height);
            using (Graphics g = Graphics.FromImage(RoundedImage))
            {
                g.Clear(BackgroundColor);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                Brush brush = new TextureBrush(StartImage);
                GraphicsPath gp = new GraphicsPath();
                gp.AddArc(0, 0, CornerRadius, CornerRadius, 180, 90);
                gp.AddArc(0 + RoundedImage.Width - CornerRadius, 0, CornerRadius, CornerRadius, 270, 90);
                gp.AddArc(0 + RoundedImage.Width - CornerRadius, 0 + RoundedImage.Height - CornerRadius, CornerRadius, CornerRadius, 0, 90);
                gp.AddArc(0, 0 + RoundedImage.Height - CornerRadius, CornerRadius, CornerRadius, 90, 90);
                g.FillPath(brush, gp);
                return RoundedImage;
            }
        }

        private void updateBuiltinAgentStatus()
        {
            if (mcagent == null) return;

            pictureBoxGreen.Visible = false; // Green
            pictureBoxRed.Visible = (mcagent.state == 0);  // Red
            pictureBoxYellow.Visible = (mcagent.state == 1) || (mcagent.state == 2); // Gray
            pictureBoxQuestion.Visible = (mcagent.state == 3); // Question
            pictureBoxUser.Visible = false;
            pictureBoxUsers.Visible = false;
            pictureBoxCustom.Visible = false;
            pictureBoxUsers.Visible = false;
            if (mcagent.state == 0) { stateLabel.Text = "Disconnected"; requestHelpButton.Text = "Request Help"; }
            if (mcagent.state == 1) { stateLabel.Text = "Connecting"; requestHelpButton.Text = "Cancel Help Request"; }
            if (mcagent.state == 2) { stateLabel.Text = "Authenticating"; requestHelpButton.Text = "Cancel Help Request"; }
            if (mcagent.state == 3) { stateLabel.Text = "Help Requested"; requestHelpButton.Text = "Cancel Help Request"; }
            Agent_onSessionChanged();
            requestHelpButton.Enabled = true;
            if (mcagent.state == 0) { helpRequested = false; }

            string[] userids = getSessionUserIdList();
            if (userids.Length == 1)
            {
                string userid = userids[0];
                string realname = userid.Split('/')[2];
                if (mcagent.userrealname.ContainsKey(userid)) { realname = mcagent.userrealname[userid]; }
                stateLabel.Text = realname;
                pictureBoxGreen.Visible = false; // Green
                pictureBoxRed.Visible = false;  // Red
                pictureBoxYellow.Visible = false; // Gray
                pictureBoxQuestion.Visible = false; // Question
                Image userImage = null;
                if (mcagent.userimages.ContainsKey(userid) && (mcagent.userimages[userid] != null)) { userImage = mcagent.userimages[userid]; }
                if (userImage == null)
                {
                    pictureBoxUser.Visible = true;
                    pictureBoxCustom.Visible = false;
                }
                else
                {
                    pictureBoxUser.Visible = false;
                    pictureBoxCustom.Image = RoundCorners(userImage, 30, this.BackColor);
                    pictureBoxCustom.Visible = true;
                }
            }
            if (userids.Length > 1)
            {
                stateLabel.Text = "Multiple Users";
                pictureBoxGreen.Visible = false; // Green
                pictureBoxRed.Visible = false;  // Red
                pictureBoxYellow.Visible = false; // Gray
                pictureBoxQuestion.Visible = false; // Question
                pictureBoxUsers.Visible = true;
            }
        }

        private void connectToAgent()
        {
            if (agent != null) { agent.DisconnectPipe(); agent = null; }
            if ((mcagent != null) && (mcagent.state != 0)) { mcagent.disconnect(); }
            if ((currentAgentName != null) && (currentAgentName.Equals("~")))
            {
                this.Text = "MeshCentral Assistant";
                Settings.SetRegValue("SelectedAgent", currentAgentName);
                updateBuiltinAgentStatus();
                startAgentToolStripMenuItem.Visible = false;
                stopAgentToolStripMenuItem.Visible = false;
                toolStripMenuItem2.Visible = false;
                this.StartPosition = FormStartPosition.Manual;
                this.Location = new Point(Screen.PrimaryScreen.WorkingArea.Width - this.Width, Screen.PrimaryScreen.WorkingArea.Height - this.Height);

                // If auto-connect is specified, connect now
                if (autoConnect)
                {
                    autoConnect = false;
                    mcagent.HelpRequest = null;
                    mcagent.connect();
                }
            }
            else
            {
                if (currentAgentName == null) {
                    agent = new MeshAgent("MeshCentralAssistant", "Mesh Agent", null, selfExecutableHashHex);
                } else {
                    agent = new MeshAgent("MeshCentralAssistant", currentAgentName, agents[currentAgentName], selfExecutableHashHex);
                    Settings.SetRegValue("SelectedAgent", currentAgentName);
                }
                agent.onStateChanged += Agent_onStateChanged;
                agent.onQueryResult += Agent_onQueryResult;
                agent.onSessionChanged += Agent_onSessionChanged;
                agent.onUserInfoChange += Agent_onUserInfoChange;
                agent.onAmtState += Agent_onAmtState;
                agent.onSelfUpdate += Agent_onSelfUpdate;
                agent.onCancelHelp += Agent_onCancelHelp;
                agent.onConsoleMessage += Agent_onConsoleMessage;
                this.StartPosition = FormStartPosition.Manual;
                this.Location = new Point(Screen.PrimaryScreen.WorkingArea.Width - this.Width, Screen.PrimaryScreen.WorkingArea.Height - this.Height);
                agent.ConnectPipe();
                UpdateServiceStatus();

                pictureBoxGreen.Visible = false;
                pictureBoxRed.Visible = false;
                pictureBoxYellow.Visible = true;

                isAdministrator = IsAdministrator();
                if (isAdministrator)
                {
                    startAgentToolStripMenuItem.Visible = true;
                    stopAgentToolStripMenuItem.Visible = true;
                    toolStripMenuItem2.Visible = true;
                }
                else
                {
                    startAgentToolStripMenuItem.Visible = false;
                    stopAgentToolStripMenuItem.Visible = false;
                    toolStripMenuItem2.Visible = false;
                }

                if (currentAgentName != "Mesh Agent")
                {
                    this.Text = string.Format("{0} Assistant", currentAgentName);
                }
                else
                {
                    this.Text = "MeshCentral Assistant";
                }
            }
            Agent_onSessionChanged();
        }

        private void Agent_onUserInfoChange(string userid, int change)
        {
            if (this.InvokeRequired) { this.Invoke(new MeshAgent.onUserInfoChangeHandler(Agent_onUserInfoChange), userid, change); return; }
            Agent_onSessionChanged();
        }

        private void Agent_onConsoleMessage(string str)
        {
            if (consoleForm == null) return;
            if (this.InvokeRequired) { this.Invoke(new MeshAgent.onConsoleHandler(Agent_onConsoleMessage), str); return; }
            consoleForm.appendText(str);
        }

        private void Agent_onCancelHelp()
        {
            if (this.InvokeRequired) { this.Invoke(new MeshAgent.onCancelHelpHandler(Agent_onCancelHelp)); return; }
            if (helpRequested == true) { requestHelpButton_Click(null, null); }
        }

        private void Agent_onSelfUpdate(string name, string hash, string url, string serverhash)
        {
            if (this.InvokeRequired) { this.Invoke(new MeshAgent.onSelfUpdateHandler(Agent_onSelfUpdate), name, hash, url, serverhash); return; }
            if (noUpdate == false) DownloadUpdate(hash, url, serverhash);
        }

        private void Agent_onAmtState(System.Collections.Generic.Dictionary<string, object> state)
        {
            if (this.InvokeRequired) { this.Invoke(new MeshAgent.onAmtStateHandler(Agent_onAmtState), state); return; }
            if (meInfoForm != null) { meInfoForm.updateInfo(state); }
        }

        private int CountSessions(Dictionary<string, object> sessions)
        {
            if (sessions == null) return 0;
            int count = 0;
            lock (sessions) { foreach (string key in sessions.Keys) { count += (int)sessions[key]; } }
            return count;
        }

        private void Agent_onSessionChanged()
        {
            if (this.InvokeRequired) { this.Invoke(new MeshAgent.onSessionChangedHandler(Agent_onSessionChanged)); return; }

            if ((mcagent != null) && (currentAgentName != null) && (currentAgentName.Equals("~")))
            {
                // Called when sessions on the agent have changed.
                int count = 0;
                if (mcagent.DesktopSessions != null) { count += CountSessions(mcagent.DesktopSessions); }
                if (mcagent.TerminalSessions != null) { count += CountSessions(mcagent.TerminalSessions); }
                if (mcagent.FilesSessions != null) { count += CountSessions(mcagent.FilesSessions); }
                if (mcagent.TcpSessions != null) { count += CountSessions(mcagent.TcpSessions); }
                if (mcagent.UdpSessions != null) { count += CountSessions(mcagent.UdpSessions); }
                if (count > 1) { mainNotifyIcon.BalloonTipText = count + " remote sessions are active."; remoteSessionsLabel.Text = (count + " remote sessions"); }
                if (count == 1) { mainNotifyIcon.BalloonTipText = "1 remote session is active."; remoteSessionsLabel.Text = "1 remote session"; }
                if (count == 0) { mainNotifyIcon.BalloonTipText = "No active remote sessions."; remoteSessionsLabel.Text = "No remote sessions"; }
                //mainNotifyIcon.ShowBalloonTip(2000);
                if (sessionsForm != null) { sessionsForm.UpdateInfo(); }
            }
            else if (agent != null)
            {
                // Called when sessions on the agent have changed.
                int count = 0;
                if (agent.DesktopSessions != null) { count += CountSessions(agent.DesktopSessions); }
                if (agent.TerminalSessions != null) { count += CountSessions(agent.TerminalSessions); }
                if (agent.FilesSessions != null) { count += CountSessions(agent.FilesSessions); }
                if (agent.TcpSessions != null) { count += CountSessions(agent.TcpSessions); }
                if (agent.UdpSessions != null) { count += CountSessions(agent.UdpSessions); }
                if (count > 1) { mainNotifyIcon.BalloonTipText = count + " remote sessions are active."; remoteSessionsLabel.Text = (count + " remote sessions"); }
                if (count == 1) { mainNotifyIcon.BalloonTipText = "1 remote session is active."; remoteSessionsLabel.Text = "1 remote session"; }
                if (count == 0) { mainNotifyIcon.BalloonTipText = "No active remote sessions."; remoteSessionsLabel.Text = "No remote sessions"; }
                //mainNotifyIcon.ShowBalloonTip(2000);
                if (sessionsForm != null) { sessionsForm.UpdateInfo(); }

                stateLabel.Text = "Connected to server";
                pictureBoxGreen.Visible = true; // Green
                pictureBoxRed.Visible = false;  // Red
                pictureBoxYellow.Visible = false; // Yellow
                pictureBoxQuestion.Visible = false; // Help
                pictureBoxUser.Visible = false;
                pictureBoxUsers.Visible = false;
                pictureBoxCustom.Visible = false;

                string[] userids = getSessionUserIdList2();
                if (userids.Length == 1)
                {
                    string userid = userids[0];
                    string realname = userid.Split('/')[2];
                    if (agent.userrealname.ContainsKey(userid)) { realname = agent.userrealname[userid]; }
                    stateLabel.Text = realname;
                    pictureBoxGreen.Visible = false; // Green
                    pictureBoxRed.Visible = false;  // Red
                    pictureBoxYellow.Visible = false; // Gray
                    pictureBoxQuestion.Visible = false; // Question
                    Image userImage = null;
                    if (agent.userimages.ContainsKey(userid) && (agent.userimages[userid] != null)) { userImage = agent.userimages[userid]; }
                    if (userImage == null)
                    {
                        pictureBoxUser.Visible = true;
                        pictureBoxCustom.Visible = false;
                    }
                    else
                    {
                        pictureBoxUser.Visible = false;
                        pictureBoxCustom.Image = RoundCorners(userImage, 30, this.BackColor);
                        pictureBoxCustom.Visible = true;
                    }
                }
                if (userids.Length > 1)
                {
                    stateLabel.Text = "Multiple Users";
                    pictureBoxGreen.Visible = false; // Green
                    pictureBoxRed.Visible = false;  // Red
                    pictureBoxYellow.Visible = false; // Gray
                    pictureBoxQuestion.Visible = false; // Question
                    pictureBoxUsers.Visible = true;
                }
            }
        }

        public void UpdateServiceStatus()
        {
            if ((currentAgentName != null) && (currentAgentName.Equals("~"))) return;

            try
            {
                ServiceControllerStatus status = MeshAgent.GetServiceStatus();
                startAgentToolStripMenuItem.Enabled = (status == ServiceControllerStatus.Stopped);
                stopAgentToolStripMenuItem.Enabled = (status != ServiceControllerStatus.Stopped);
                if (agent.State != 0) return;
                pictureBoxGreen.Visible = false; // Green
                pictureBoxRed.Visible = true;  // Red
                pictureBoxYellow.Visible = false; // Yellow
                pictureBoxQuestion.Visible = false; // Help
                pictureBoxUser.Visible = false;
                pictureBoxUsers.Visible = false;
                pictureBoxCustom.Visible = false;
                switch (status)
                {
                    case ServiceControllerStatus.ContinuePending: { stateLabel.Text = "Agent is continue pending"; break; }
                    case ServiceControllerStatus.Paused: { stateLabel.Text = "Agent is paused"; break; }
                    case ServiceControllerStatus.PausePending: { stateLabel.Text = "Agent is pause pending"; break; }
                    case ServiceControllerStatus.Running: { stateLabel.Text = "Agent is running"; break; }
                    case ServiceControllerStatus.StartPending: { stateLabel.Text = "Agent is start pending"; break; }
                    case ServiceControllerStatus.Stopped: { stateLabel.Text = "Agent is stopped"; break; }
                    case ServiceControllerStatus.StopPending: { stateLabel.Text = "Agent is stopped pending"; break; }
                }
            }
            catch (Exception)
            {
                startAgentToolStripMenuItem.Enabled = false;
                stopAgentToolStripMenuItem.Enabled = false;
                stateLabel.Text = "Agent not installed";
            }
        }

        public static bool IsAdministrator()
        {
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

        protected override void SetVisibleCore(bool value)
        {
            base.SetVisibleCore(allowShowDisplay ? value : allowShowDisplay);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.Visible = false;
            this.WindowState = FormWindowState.Minimized;
        }

        private void Agent_onStateChanged(int state, int serverState)
        {
            if (this.InvokeRequired) {
                this.Invoke(new MeshAgent.onStateChangedHandler(Agent_onStateChanged), state, serverState);
                return;
            }

            if (currentAgentName.Equals("~")) {
                intelMEStateToolStripMenuItem.Visible = false;
                intelAMTStateToolStripMenuItem.Visible = false;
                return;
            }

            //bool openUrlVisible = ((state == 1) && (agent.ServerUri != null));
            //try { openSiteToolStripMenuItem.Visible = openUrlVisible; } catch (Exception) { return; }
            switch (state)
            {
                case 0:
                    {
                        pictureBoxGreen.Visible = false; // Green
                        pictureBoxRed.Visible = true;  // Red
                        pictureBoxYellow.Visible = false; // Yellow
                        pictureBoxQuestion.Visible = false; // Help
                        pictureBoxUser.Visible = false; // User
                        pictureBoxUsers.Visible = false;
                        pictureBoxCustom.Visible = false;
                        UpdateServiceStatus();
                        requestHelpToolStripMenuItem.Enabled = false;
                        requestHelpToolStripMenuItem.Visible = true;
                        cancelHelpRequestToolStripMenuItem.Visible = false;
                        intelMEStateToolStripMenuItem.Visible = false;
                        intelAMTStateToolStripMenuItem.Visible = false;
                        break;
                    }
                case 1:
                    {
                        if (serverState == 1) {
                            pictureBoxGreen.Visible = true;
                            pictureBoxRed.Visible = false;
                            pictureBoxYellow.Visible = false;
                            pictureBoxQuestion.Visible = false;
                            pictureBoxUser.Visible = false;
                            pictureBoxUsers.Visible = false;
                            pictureBoxCustom.Visible = false;
                            stateLabel.Text = "Connected to server";
                            requestHelpToolStripMenuItem.Enabled = true;
                            requestHelpToolStripMenuItem.Visible = true;
                            cancelHelpRequestToolStripMenuItem.Visible = false;
                        } else {
                            pictureBoxGreen.Visible = false;
                            pictureBoxRed.Visible = false;
                            pictureBoxYellow.Visible = true;
                            pictureBoxQuestion.Visible = false;
                            pictureBoxUser.Visible = false;
                            pictureBoxUsers.Visible = false;
                            pictureBoxCustom.Visible = false;
                            stateLabel.Text = "Agent is disconnected";
                            requestHelpToolStripMenuItem.Enabled = false;
                            requestHelpToolStripMenuItem.Visible = true;
                            cancelHelpRequestToolStripMenuItem.Visible = false;
                        }
                        intelMEStateToolStripMenuItem.Visible = agent.IntelAmtSupport;
                        intelAMTStateToolStripMenuItem.Visible = agent.IntelAmtSupport;
                        break;
                    }
            }
            helpRequested = false;
            requestHelpButton.Text = "Request Help";
            requestHelpButton.Enabled = ((state == 1) && (serverState == 1));

            if (isAdministrator && agent.ServiceAgent)
            {
                startAgentToolStripMenuItem.Visible = true;
                stopAgentToolStripMenuItem.Visible = true;
                toolStripMenuItem2.Visible = true;
            }
            else
            {
                startAgentToolStripMenuItem.Visible = false;
                stopAgentToolStripMenuItem.Visible = false;
                toolStripMenuItem2.Visible = false;
            }

            if ((state != 1) || (serverState != 1))
            {
                if (snapShotForm != null) { snapShotForm.Close(); }
                if (requestHelpForm != null) { requestHelpForm.Close(); }
                if (sessionsForm != null) { sessionsForm.Close(); }
                if (meInfoForm != null) { meInfoForm.Close(); }
                if (consoleForm != null) { consoleForm.Close(); }
            }
        }

        private void Agent_onQueryResult(string value, string result)
        {
            if (this.InvokeRequired) { this.Invoke(new MeshAgent.onQueryResultHandler(Agent_onQueryResult), value, result); return; }
            if (snapShotForm != null) { snapShotForm.displaySnapShot(result); }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (agent != null) { agent.DisconnectPipe(); }
            if (mcagent != null) { mcagent.disconnect(); }
            doclose = true;
            Application.Exit();
        }

        private void connectionTimer_Tick(object sender, EventArgs e)
        {
            if (agent != null)
            {
                if (timerSlowDown > 0) { timerSlowDown--; if (timerSlowDown == 0) { connectionTimer.Interval = 10000; } }
                if (agent.State == 0) { agent.ConnectPipe(); }
                UpdateServiceStatus();
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (doclose == false) { e.Cancel = true; this.Visible = false; }
        }

        private void openSiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(agent.ServerUri.ToString());
        }

        private void mainNotifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if ((e == null) || (e.Button == System.Windows.Forms.MouseButtons.Left)) {
                this.WindowState = FormWindowState.Normal;
                this.allowShowDisplay = true;
                openToolStripMenuItem.Visible = this.Visible;
                closeToolStripMenuItem.Visible = !this.Visible;
                this.Visible = !this.Visible;
            }
        }

        private void startAgentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MeshAgent.StartService();
            connectionTimer.Enabled = false;
            connectionTimer.Interval = 500;
            timerSlowDown = 20;
            connectionTimer.Enabled = true;
        }

        private void stopAgentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MeshAgent.StopService();
            connectionTimer.Enabled = false;
            connectionTimer.Interval = 500;
            timerSlowDown = 20;
            connectionTimer.Enabled = true;
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
            this.allowShowDisplay = true;
            openToolStripMenuItem.Visible = this.Visible;
            closeToolStripMenuItem.Visible = !this.Visible;
            this.Visible = !this.Visible;
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F10)
            {
                if (snapShotForm == null)
                {
                    snapShotForm = new SnapShotForm(this);
                    snapShotForm.Show(this);
                }
                else
                {
                    snapShotForm.Focus();
                }
            }
            if (e.KeyCode == Keys.F9)
            {
                if (consoleForm == null)
                {
                    consoleForm = new ConsoleForm(this);
                    consoleForm.Show(this);
                }
                else
                {
                    consoleForm.Focus();
                }
            }
        }

        private void requestHelpButton_Click(object sender, EventArgs e)
        {
            if (currentAgentName.Equals("~"))
            {
                if (mcagent.state == 0)
                {
                    if (requestHelpForm != null)
                    {
                        requestHelpForm.Focus();
                    }
                    else
                    {
                        requestHelpForm = new RequestHelpForm(this);
                        requestHelpForm.NoHelpRequestOk = true;
                        requestHelpForm.Show(this);
                    }
                }
                else
                {
                    mcagent.disconnect();
                }
            }
            else
            {
                if (helpRequested == true)
                {
                    if ((agent == null) || (agent.CancelHelpRequest() == true))
                    {
                        helpRequested = false;
                        requestHelpButton.Text = "Request Help";
                        stateLabel.Text = "Connected to server";
                        requestHelpToolStripMenuItem.Visible = true;
                        cancelHelpRequestToolStripMenuItem.Visible = false;
                        pictureBoxGreen.Visible = true;
                        pictureBoxRed.Visible = false;
                        pictureBoxYellow.Visible = false;
                        pictureBoxQuestion.Visible = false;
                        pictureBoxUser.Visible = false;
                        pictureBoxUsers.Visible = false;
                        pictureBoxCustom.Visible = false;
                    }
                }
                else
                {
                    if (requestHelpForm != null)
                    {
                        requestHelpForm.Focus();
                    }
                    else
                    {
                        requestHelpForm = new RequestHelpForm(this);
                        requestHelpForm.Show(this);
                    }
                }
            }
        }

        public void RequestHelp(string details)
        {
            if (currentAgentName.Equals("~"))
            {
                if (details.Length > 0) { mcagent.HelpRequest = details; } else { mcagent.HelpRequest = null; }
                mcagent.connect();
            }
            else
            {
                if (agent.RequestHelp(details) == true)
                {
                    helpRequested = true;
                    requestHelpButton.Text = "Cancel Help Request";
                    stateLabel.Text = "Help requested";
                    requestHelpToolStripMenuItem.Visible = false;
                    cancelHelpRequestToolStripMenuItem.Visible = true;
                    pictureBoxGreen.Visible = false;
                    pictureBoxRed.Visible = false;
                    pictureBoxYellow.Visible = false;
                    pictureBoxQuestion.Visible = true;
                    pictureBoxUser.Visible = false;
                    pictureBoxUsers.Visible = false;
                    pictureBoxCustom.Visible = false;
                }
            }
        }

        private void remoteSessionsLabel_Click(object sender, EventArgs e)
        {
            if (sessionsForm != null)
            {
                sessionsForm.Focus();
            }
            else
            {
                sessionsForm = new SessionsForm(this);
                sessionsForm.Show(this);
            }
        }

        private void dialogContextMenuStrip_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            requestHelpToolStripMenuItem.Enabled = (agent.State != 0);
        }

        private void intelAMTStateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (agent == null) return;
            if (meInfoForm != null)
            {
                meInfoForm.Focus();
            }
            else
            {
                meInfoForm = new MeInfoForm(this);
                meInfoForm.Show(this);
            }
        }


        //
        // Self update section
        //

        string seflUpdateDownloadHash = null;
        string serverTlsCertHash = null;

        private void DownloadUpdate(string hash, string url, string serverHash)
        {
            seflUpdateDownloadHash = hash;
            serverTlsCertHash = serverHash;
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            Uri x = webRequest.RequestUri;
            webRequest.Method = "GET";
            webRequest.Timeout = 10000;
            webRequest.BeginGetResponse(new AsyncCallback(DownloadUpdateRespone), webRequest);
            webRequest.ServerCertificateValidationCallback += RemoteCertificateValidationCallback;
        }

        public bool RemoteCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            // Check MeshCentral server's TLS certificate. This is our first security layer.
            if ((serverTlsCertHash != null) && (serverTlsCertHash != certificate.GetCertHashString().ToLower()) && (serverTlsCertHash != GetMeshKeyHash(certificate).ToLower()) && (serverTlsCertHash != GetMeshCertHash(certificate).ToLower())) return false;
            return true;
        }

        private void DownloadUpdateRespone(IAsyncResult asyncResult)
        {
            long received = 0;
            HttpWebRequest webRequest = (HttpWebRequest)asyncResult.AsyncState;
            try
            {
                // Hash our own executable
                using (HttpWebResponse webResponse = (HttpWebResponse)webRequest.EndGetResponse(asyncResult))
                {
                    byte[] buffer = new byte[4096];
                    FileStream fileStream = File.OpenWrite(System.Reflection.Assembly.GetEntryAssembly().Location + ".update.exe");
                    using (Stream input = webResponse.GetResponseStream())
                    {
                        int size = input.Read(buffer, 0, buffer.Length);
                        while (size > 0)
                        {
                            fileStream.Write(buffer, 0, size);
                            received += size;
                            size = input.Read(buffer, 0, buffer.Length);
                        }
                    }
                    fileStream.Flush();
                    fileStream.Close();

                    // Hash the resulting file, check that it's correct. This is our second security layer.
                    byte[] downloadHash;
                    using (var sha384 = SHA384Managed.Create()) { using (var stream = File.OpenRead(System.Reflection.Assembly.GetEntryAssembly().Location + ".update.exe")) { downloadHash = sha384.ComputeHash(stream); } }
                    string downloadHashHex = BitConverter.ToString(downloadHash).Replace("-", string.Empty).ToLower();
                    if (downloadHashHex != seflUpdateDownloadHash)
                    {
                        System.Threading.Thread.Sleep(500);
                        File.Delete(System.Reflection.Assembly.GetEntryAssembly().Location + ".update.exe");
                    }
                    else
                    {
                        doclose = true;
                        forceExit = true;
                        System.Threading.Thread.Sleep(500);
                        string arguments = "-update:" + System.Reflection.Assembly.GetEntryAssembly().Location + " " + string.Join(" ", args);
                        if (this.Visible == true) { arguments += " -visible"; }
                        Process.Start(System.Reflection.Assembly.GetEntryAssembly().Location + ".update.exe", arguments);
                        Application.Exit();
                    }
                }
            }
            catch (Exception) { }
        }

        // Return a modified base64 SHA384 hash string of the certificate public key
        public static string GetMeshKeyHash(X509Certificate cert)
        {
            return ByteArrayToHexString(new SHA384Managed().ComputeHash(cert.GetPublicKey()));
        }

        // Return a modified base64 SHA384 hash string of the certificate
        public static string GetMeshCertHash(X509Certificate cert)
        {
            return ByteArrayToHexString(new SHA384Managed().ComputeHash(cert.GetRawCertData()));
        }

        public static string ByteArrayToHexString(byte[] Bytes)
        {
            StringBuilder Result = new StringBuilder(Bytes.Length * 2);
            string HexAlphabet = "0123456789ABCDEF";
            foreach (byte B in Bytes) { Result.Append(HexAlphabet[(int)(B >> 4)]); Result.Append(HexAlphabet[(int)(B & 0xF)]); }
            return Result.ToString();
        }

    }
}
