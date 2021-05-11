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

namespace MeshAssistant
{
    public partial class MainForm : Form
    {
        public string[] args;
        public int timerSlowDown = 0;
        public bool allowShowDisplay = false;
        public bool doclose = false;
        public bool helpRequested = false;
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

            // Set TLS 1.2
            ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

            InitializeComponent();

            // Check if the built-in agent will be activated
            currentAgentName = null;
            List<ToolStripItem> subMenus = new List<ToolStripItem>();
            string currentAgentSelection = Settings.GetRegValue("SelectedAgent", null);

            if (MeshCentralAgent.checkMshFile()) {
                mcagent = new MeshCentralAgent();
                mcagent.onStateChanged += Mcagent_onStateChanged;
                mcagent.onNotify += Mcagent_onNotify;
                if (currentAgentSelection.Equals("~")) { currentAgentName = "~"; }
                ToolStripMenuItem m = new ToolStripMenuItem();
                m.Name = "AgentSelector-~";
                m.Text = "Built-in agent";
                m.Checked = ((currentAgentName != null) && (currentAgentName.Equals("~")));
                m.Click += agentSelection_Click;
                subMenus.Add(m);
            }

            // Get the list of agents on the system
            agents = MeshAgent.GetAgentInfo(selectedAgentName);
            string[] agentNames = agents.Keys.ToArray();
            if (agents.Count > 0) {
                if ((currentAgentName == null) || (currentAgentName != "~")) { currentAgentName = agentNames[0]; } // Default
                for (var i = 0; i < agentNames.Length; i++) { if (agentNames[i] == currentAgentSelection) { currentAgentName = agentNames[i]; } }
                if (agentNames.Length > 1)
                {
                    for (var i = 0; i < agentNames.Length; i++)
                    {
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

        private void Mcagent_onNotify(string title, string msg)
        {
            if (InvokeRequired) { Invoke(new MeshCentralAgent.onNotifyHandler(Mcagent_onNotify), title, msg); return; }
            //MessageBox.Show(msg, title);
            mainNotifyIcon.BalloonTipText = title + " - " + msg;
            mainNotifyIcon.ShowBalloonTip(2000);
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
            foreach (ToolStripMenuItem submenu in agentSelectToolStripMenuItem.DropDownItems) {
                submenu.Checked = (submenu.Name.Substring(14) == currentAgentName);
            }
            connectToAgent();
        }

        private void updateBuiltinAgentStatus()
        {
            if (mcagent == null) return;
            pictureBox1.Visible = false; // Green
            pictureBox2.Visible = (mcagent.state == 0);  // Red
            pictureBox3.Visible = (mcagent.state == 1) || (mcagent.state == 2); // Gray
            pictureBox4.Visible = (mcagent.state == 3); // Question
            if (mcagent.state == 0) { stateLabel.Text = "Disconnected"; requestHelpButton.Text = "Request Help"; }
            if (mcagent.state == 1) { stateLabel.Text = "Connecting"; requestHelpButton.Text = "Cancel Help Request"; }
            if (mcagent.state == 2) { stateLabel.Text = "Authenticating"; requestHelpButton.Text = "Cancel Help Request"; }
            if (mcagent.state == 3) { stateLabel.Text = "Help Requested"; requestHelpButton.Text = "Cancel Help Request"; }
            Agent_onSessionChanged();
            requestHelpButton.Enabled = true;
            if (mcagent.state == 0) { helpRequested = false; }
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
            }
            else
            {
                if (currentAgentName == null) {
                    agent = new MeshAgent("MeshCentralAssistant", "Mesh Agent", null);
                } else {
                    agent = new MeshAgent("MeshCentralAssistant", currentAgentName, agents[currentAgentName]);
                    Settings.SetRegValue("SelectedAgent", currentAgentName);
                }
                agent.onStateChanged += Agent_onStateChanged;
                agent.onQueryResult += Agent_onQueryResult;
                agent.onSessionChanged += Agent_onSessionChanged;
                agent.onAmtState += Agent_onAmtState;
                agent.onSelfUpdate += Agent_onSelfUpdate;
                agent.onCancelHelp += Agent_onCancelHelp;
                agent.onConsoleMessage += Agent_onConsoleMessage;
                this.StartPosition = FormStartPosition.Manual;
                this.Location = new Point(Screen.PrimaryScreen.WorkingArea.Width - this.Width, Screen.PrimaryScreen.WorkingArea.Height - this.Height);
                agent.ConnectPipe();
                UpdateServiceStatus();

                pictureBox1.Visible = false;
                pictureBox2.Visible = false;
                pictureBox3.Visible = true;

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

        private void Agent_onSessionChanged()
        {
            if (agent == null) return;
            if (this.InvokeRequired) { this.Invoke(new MeshAgent.onSessionChangedHandler(Agent_onSessionChanged)); return; }

            if ((currentAgentName != null) && (currentAgentName.Equals("~")))
            {
                remoteSessionsLabel.Text = "Built-in Agent";
            }
            else
            {
                // Called when sessions on the agent have changed.
                int count = 0;
                if (agent.DesktopSessions != null) { count += agent.DesktopSessions.Count; }
                if (agent.TerminalSessions != null) { count += agent.TerminalSessions.Count; }
                if (agent.FilesSessions != null) { count += agent.FilesSessions.Count; }
                if (agent.TcpSessions != null) { count += agent.TcpSessions.Count; }
                if (agent.UdpSessions != null) { count += agent.UdpSessions.Count; }
                if (count > 1) { mainNotifyIcon.BalloonTipText = count + " remote sessions are active."; remoteSessionsLabel.Text = (count + " remote sessions"); }
                if (count == 1) { mainNotifyIcon.BalloonTipText = "1 remote session is active."; remoteSessionsLabel.Text = "1 remote session"; }
                if (count == 0) { mainNotifyIcon.BalloonTipText = "No active remote sessions."; remoteSessionsLabel.Text = "No remote sessions"; }
                //mainNotifyIcon.ShowBalloonTip(2000);
                if (sessionsForm != null) { sessionsForm.UpdateInfo(); }
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
                pictureBox1.Visible = false; // Green
                pictureBox2.Visible = true;  // Red
                pictureBox3.Visible = false; // Yellow
                pictureBox4.Visible = false; // Help
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
                        pictureBox1.Visible = false; // Green
                        pictureBox2.Visible = true;  // Red
                        pictureBox3.Visible = false; // Yellow
                        pictureBox4.Visible = false; // Help
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
                            pictureBox1.Visible = true;
                            pictureBox2.Visible = false;
                            pictureBox3.Visible = false;
                            pictureBox4.Visible = false;
                            stateLabel.Text = "Connected to server";
                            requestHelpToolStripMenuItem.Enabled = true;
                            requestHelpToolStripMenuItem.Visible = true;
                            cancelHelpRequestToolStripMenuItem.Visible = false;
                        } else {
                            pictureBox1.Visible = false;
                            pictureBox2.Visible = false;
                            pictureBox3.Visible = true;
                            pictureBox4.Visible = false;
                            stateLabel.Text = "Agent is active";
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
                        pictureBox1.Visible = true;
                        pictureBox2.Visible = false;
                        pictureBox3.Visible = false;
                        pictureBox4.Visible = false;
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
                mcagent.HelpRequest = details;
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
                    pictureBox1.Visible = false;
                    pictureBox2.Visible = false;
                    pictureBox3.Visible = false;
                    pictureBox4.Visible = true;
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
