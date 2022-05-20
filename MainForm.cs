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
        public bool debug = false;
        public string[] args;
        public int timerSlowDown = 0;
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
        public PrivacyBarForm privacyBar = null;
        public UpdateForm updateForm = null;
        public EventsForm eventsForm = null;
        public BrowserForm browserForm = null;
        public GuestSharingForm guestSharingForm = null;
        public bool isAdministrator = false;
        public bool forceExit = false;
        public bool noUpdate = false;
        public ArrayList pastConsoleCommands = new ArrayList();
        public Dictionary<string, string> agents = null;
        public string selectedAgentName = null;
        public string currentAgentName = null;
        public NotifyForm notifyForm = null;
        public ConsentForm consentForm = null;
        public int embeddedMshLength = 0;
        public string selfExecutableHashHex = null;
        public string updateHash;
        public string updateUrl;
        public string updateServerHash;
        public List<PrivacyBarForm> privacyBars = null;
        private bool startVisible = false;
        public List<LogEventStruct> userEvents = new List<LogEventStruct>();
        public bool SystemTrayApp = true;
        public Image CustomizationLogo = null;
        public string CustomizationTitle = null;
        public string autoHelpRequest = null;
        public bool allowUseOfProxy = true;

        public struct LogEventStruct
        {
            public LogEventStruct(DateTime time, string userid, string msg) { this.time = time; this.userid = userid; this.msg = msg; }
            public DateTime time;
            public string userid;
            public string msg;
        }

        private void LoadEventsFromFile()
        {
            string[] events = null;
            try { events = File.ReadAllLines("events.log"); } catch (Exception) { }
            if (events == null) return;
            foreach (string e in events)
            {
                int i = e.IndexOf(", ");
                if (i == -1) continue;
                int j = e.IndexOf(", ", i + 2);
                if (j == -1) continue;
                string time = e.Substring(0, i);
                string userid = e.Substring(i + 2, j - i - 2);
                string msg = e.Substring(j + 2);
                userEvents.Add(new LogEventStruct(DateTime.Parse(time), userid, msg));
                if (userEvents.Count > 1000) { userEvents.RemoveAt(0); }
            }
        }

        public void Log(string msg) {
            if (debug) { try { File.AppendAllText("debug.log", DateTime.Now.ToString("HH:mm:tt.ffff: ") + msg + "\r\n"); } catch (Exception) { } }
        }

        public void Event(string userid, string msg)
        {
            DateTime now = DateTime.Now;
            LogEventStruct e = new LogEventStruct(now, userid, msg);
            userEvents.Add(e);
            AddEventToForm(e);
            try { File.AppendAllText("events.log", now.ToString("yyyy-MM-ddTHH:mm:sszzz") + ", " + userid + ", " + msg + "\r\n"); } catch (Exception) { }
            Log(string.Format("Event ({0}): {1}", userid, msg));
        }

        delegate void AddEventToFormHandler(LogEventStruct e);

        public void AddEventToForm(LogEventStruct e)
        {
            if (eventsForm == null) return;
            if (this.InvokeRequired) { this.Invoke(new AddEventToFormHandler(AddEventToForm), e); return; }
            eventsForm.addEvent(e);
        }

        private bool RemoteCertificateValidationCallbackGlobal(object sender, X509Certificate certificate, X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            if (mcagent.state < 2)
            {
                // If we are not connected at all, accept the TLS cert since we are going to be doing secondary auth.
                mcagent.WebSocket.tlsCert = new X509Certificate2(certificate);
                return true;
            }
            else
            {
                // We are connected, all further connections need to have the same TLS cert as the main control connection.
                if ((mcagent.ServerTlsHashStr != null) && ((mcagent.ServerTlsHashStr == certificate.GetCertHashString()) || (mcagent.ServerTlsHashStr == webSocketClient.GetMeshKeyHash(certificate)) || (mcagent.ServerTlsHashStr == webSocketClient.GetMeshCertHash(certificate)))) { return true; }
            }
            return false;
        }

        public MainForm(string[] args)
        {
            // Set TLS 1.2
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(RemoteCertificateValidationCallbackGlobal);

            // Perform self update operations if any.
            this.args = args;
            string update = null;
            string delete = null;
            foreach (string arg in this.args) 
            {
                if (arg.ToLower() == "-debug") { debug = true; }
                if ((arg.Length == 8) && (arg.ToLower() == "-visible")) { startVisible = true; }
                if ((arg.Length == 9) && (arg.ToLower() == "-noupdate")) { noUpdate = true; }
                if (arg.Length > 8 && arg.Substring(0, 8).ToLower() == "-update:") { update = arg.Substring(8); }
                if (arg.Length > 8 && arg.Substring(0, 8).ToLower() == "-delete:") { delete = arg.Substring(8); }
                if (arg.Length > 6 && arg.Substring(0, 6).ToLower() == "-help:") { autoHelpRequest = arg.Substring(6); }
                if (arg.Length > 11 && arg.Substring(0, 11).ToLower() == "-agentname:") { selectedAgentName = arg.Substring(11); }
                if ((arg.Length == 8) && (arg.ToLower() == "-connect")) { autoConnect = true; }
                if ((arg.Length == 8) && (arg.ToLower() == "-noproxy")) { allowUseOfProxy = false; }
            }
            if (debug) { try { File.AppendAllText("debug.log", "\r\n\r\n"); } catch (Exception) { } }
            Log("***** Starting MeshCentral Assistant *****");
            Log("Version " + Assembly.GetExecutingAssembly().GetName().Version.ToString());

            if (update != null)
            {
                Log("Performing update");

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

            if (delete != null) {
                Log("Performing update delete");
                try { System.Threading.Thread.Sleep(1000); File.Delete(delete); } catch (Exception) { }
            }

            // Set TLS 1.2
            Log("Set TLS 1.2");
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            Log("InitializeComponent()");
            InitializeComponent();
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            Translate.TranslateControl(this);
            Translate.TranslateContextMenu(this.mainContextMenuStrip);
            Translate.TranslateContextMenu(this.dialogContextMenuStrip);
            this.Opacity = 0;


            // If there is an embedded .msh file, write it out to "meshagent.msh"
            Log("Checking for embedded MSH file");
            string msh = ExeHandler.GetMshFromExecutable(Process.GetCurrentProcess().MainModule.FileName, out embeddedMshLength);
            if (msh == null) { msh = MeshCentralAgent.LoadMshFileStr(); }
            //if (msh != null) { try { File.WriteAllText(MeshCentralAgent.getSelfFilename(".msh"), msh); } catch (Exception ex) { MessageBox.Show(ex.ToString()); Application.Exit(); return; } }
            selfExecutableHashHex = ExeHandler.HashExecutable(Assembly.GetEntryAssembly().Location);

            // Check if the built-in agent will be activated
            Log("Check for built-in agent");
            currentAgentName = null;
            List<ToolStripItem> subMenus = new List<ToolStripItem>();
            string currentAgentSelection = Settings.GetRegValue("SelectedAgent", null);

            int mshCheckErr = MeshCentralAgent.checkMshStr(msh);
            if (mshCheckErr == 2)
            {
                forceExit = true;
                MessageBox.Show(string.Format(Properties.Resources.SignedExecutableServerLockError, Program.LockToHostname), Properties.Resources.MeshCentralAssistant, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
                return;
            }
            else if (mshCheckErr == 0)
            {
                Log("Starting built-in agent");
                mcagent = new MeshCentralAgent(this, msh, "MeshCentralAssistant", selfExecutableHashHex, debug);
                mcagent.autoConnect = autoConnect;
                if (allowUseOfProxy == false) { mcagent.allowUseOfProxy = allowUseOfProxy; }
                mcagent.onStateChanged += Mcagent_onStateChanged;
                mcagent.onNotify += Mcagent_onNotify;
                mcagent.onSelfUpdate += Agent_onSelfUpdate;
                mcagent.onSessionChanged += Mcagent_onSessionChanged;
                mcagent.onUserInfoChange += Mcagent_onUserInfoChange;
                mcagent.onRequestConsent += Mcagent_onRequestConsent;
                mcagent.onSelfSharingStatus += Mcagent_onSelfSharingStatus;
                mcagent.onLogEvent += Mcagent_onLogEvent;
                currentAgentSelection = "~"; // If a built-in agent is present, always default to that on start.
                currentAgentName = "~";
                ToolStripMenuItem m = new ToolStripMenuItem();
                m.Name = "AgentSelector-~";
                m.Text = Translate.T(Properties.Resources.DirectConnect);
                m.Checked = ((currentAgentName != null) && (currentAgentName.Equals("~")));
                m.Click += agentSelection_Click;
                subMenus.Add(m);
            } else {
                MeshCentralAgent.getMshCustomization(msh, out CustomizationTitle, out CustomizationLogo);
            }
            UpdateTitle();

            // Configure system tray
            if (SystemTrayApp == false)
            {
                mainNotifyIcon.Visible = false;
                this.ShowInTaskbar = true;
                this.MinimizeBox = true;
                startVisible = true;
                this.Height += 60;
                this.Width = (this.Width * 160) / 100;
                this.FormBorderStyle = FormBorderStyle.FixedDialog;
            }
            else
            {
                // Get the list of agents on the system
                Log("Get list of background agents");
                bool directConnectSeperator = false;
                agents = MeshAgent.GetAgentInfo(selectedAgentName);
                string[] agentNames = agents.Keys.ToArray();
                if (agents.Count > 0)
                {
                    Log(string.Format("Found {0} background agent(s)", agents.Count));
                    if ((currentAgentName == null) || (currentAgentName != "~"))
                    {
                        currentAgentName = agentNames[0]; // Default
                        for (var i = 0; i < agentNames.Length; i++) { if (agentNames[i] == currentAgentSelection) { currentAgentName = agentNames[i]; } }
                    }
                    if ((agentNames.Length > 1) || ((agentNames.Length > 0) && (mcagent != null)))
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
                mainNotifyIcon.Visible = true;
            }

            // Load events
            LoadEventsFromFile();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Log("MainForm_Load()");
            this.WindowState = FormWindowState.Normal;
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(Screen.PrimaryScreen.WorkingArea.Width - this.Width, Screen.PrimaryScreen.WorkingArea.Height - this.Height);
            this.Visible = startVisible;
            this.Opacity = 1;
            connectToAgent();
        }

        private void Mcagent_onLogEvent(DateTime time, string userid, string msg)
        {
            if (forceExit) return;
            LogEventStruct e = new LogEventStruct(time, userid, msg);
            userEvents.Add(e);
            AddEventToForm(e);
        }

        private void Mcagent_onRequestConsent(MeshCentralTunnel tunnel, string msg, int protocol, string userid)
        {
            if (forceExit) return;
            if (this.InvokeRequired) { this.Invoke(new MeshCentralAgent.onRequestConsentHandler(Mcagent_onRequestConsent), tunnel, msg, protocol, userid); return; }
            if ((msg == null) && (consentForm != null) && (consentForm.tunnel == tunnel))
            {
                Log("Closing consent form");
                consentForm.Close();
                consentForm = null;
            }
            else if ((consentForm == null) && (msg != null))
            {
                if ((userid != null) && (ConsentForm.autoConsent.ContainsKey(userid)))
                {
                    DateTime autoAcceptTime = ConsentForm.autoConsent[userid];
                    if (autoAcceptTime > DateTime.Now) { Log("Consent auto-accepted"); tunnel.ConsentAccepted(); return; } // Auto accept user consent
                }

                Log("Opening consent form");
                string realname = "Guest";
                Image userImage = null;
                if (userid != null)
                {
                    realname = userid.Split('/')[2];
                    if ((mcagent.usernames != null) && mcagent.usernames.ContainsKey(userid) && (mcagent.usernames[userid] != null)) { realname = mcagent.usernames[userid]; }
                    if ((mcagent.userrealname != null) && mcagent.userrealname.ContainsKey(userid) && (mcagent.userrealname[userid] != null)) { realname = mcagent.userrealname[userid]; }
                    if ((mcagent.userimages != null) && mcagent.userimages.ContainsKey(userid) && (mcagent.userimages[userid] != null)) { userImage = mcagent.userimages[userid]; }
                }
                consentForm = new ConsentForm(this);
                consentForm.userid = userid;
                consentForm.tunnel = tunnel;
                consentForm.Message = msg;
                consentForm.UserName = realname;
                consentForm.UserImage = userImage;
                consentForm.Show(this);
                consentForm.Focus();
            }
        }

        private void Mcagent_onSelfSharingStatus(bool allowed, string url)
        {
            if (forceExit) return;
            if (this.InvokeRequired) { this.Invoke(new MeshCentralAgent.onSelfSharingStatusHandler(Mcagent_onSelfSharingStatus), allowed, url); return; }
            if (guestSharingForm != null) { guestSharingForm.UpdateInfo(); }
        }

        public delegate void ShowNotificationHandler(string userid, string title, string message);

        public void ShowNotification(string userid, string title, string message)
        {
            if (forceExit) return;
            if (this.InvokeRequired) { this.Invoke(new ShowNotificationHandler(ShowNotification), userid, title, message); return; }
            Log("Show notification");
            string realname = userid.Split('/')[2];
            if ((mcagent.usernames != null) && mcagent.usernames.ContainsKey(userid) && (mcagent.usernames[userid] != null)) { realname = mcagent.usernames[userid]; }
            if ((mcagent.userrealname != null) && mcagent.userrealname.ContainsKey(userid) && (mcagent.userrealname[userid] != null)) { realname = mcagent.userrealname[userid]; }
            Image userImage = null;
            if ((mcagent.userimages != null) && mcagent.userimages.ContainsKey(userid) && (mcagent.userimages[userid] != null)) { userImage = mcagent.userimages[userid]; }
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
            if (forceExit) return;
            if (InvokeRequired) { Invoke(new MeshCentralAgent.onSessionChangedHandler(Mcagent_onSessionChanged)); return; }
            Log("onSessionChanged");
            updateBuiltinAgentStatus();
        }

        private void Mcagent_onUserInfoChange(string userid, int change)
        {
            if (forceExit) return;
            if (InvokeRequired) { Invoke(new MeshCentralAgent.onUserInfoChangeHandler(Mcagent_onUserInfoChange), userid, change); return; }
            Log(string.Format("onUserInfoChange {0}, {1}", userid, change));
            updateBuiltinAgentStatus();

            // If the notification or consent dialog is showing, check if we can update the real name and/or image
            if ((notifyForm != null) && (notifyForm.userid == userid))
            {
                if ((mcagent.userimages != null) && mcagent.userimages.ContainsKey(userid) && (mcagent.userimages[userid] != null)) { notifyForm.UserImage = mcagent.userimages[userid]; }
                if ((mcagent.userrealname != null) && mcagent.userrealname.ContainsKey(userid) && (mcagent.userrealname[userid] != null)) { notifyForm.UserName = mcagent.userrealname[userid]; }
                else if ((mcagent.usernames != null) && mcagent.usernames.ContainsKey(userid) && (mcagent.usernames[userid] != null)) { notifyForm.UserName = mcagent.usernames[userid]; }
            }
            if ((consentForm != null) && (consentForm.userid == userid))
            {
                if ((mcagent.userimages != null) && mcagent.userimages.ContainsKey(userid) && (mcagent.userimages[userid] != null)) { consentForm.UserImage = mcagent.userimages[userid]; }
                if ((mcagent.userrealname != null) && mcagent.userrealname.ContainsKey(userid) && (mcagent.userrealname[userid] != null)) { consentForm.UserName = mcagent.userrealname[userid]; }
                else if ((mcagent.usernames != null) && mcagent.usernames.ContainsKey(userid) && (mcagent.usernames[userid] != null)) { notifyForm.UserName = mcagent.usernames[userid]; }
            }
        }

        private void Mcagent_onNotify(string userid, string title, string msg)
        {
            if (forceExit) return;
            if (InvokeRequired) { Invoke(new MeshCentralAgent.onNotifyHandler(Mcagent_onNotify), userid, title, msg); return; }
            ShowNotification(userid, title, msg);
            //MessageBox.Show(msg, title);
            //mainNotifyIcon.BalloonTipText = title + " - " + msg;
            //mainNotifyIcon.ShowBalloonTip(2000);
        }

        private void Mcagent_onStateChanged(int state)
        {
            if (forceExit) return;
            if (InvokeRequired) { Invoke(new MeshCentralAgent.onStateChangedHandler(Mcagent_onStateChanged), state); return; }
            Log(string.Format("Mcagent_onStateChanged {0}", state));
            if (state == 0) { PrivacyBarClose(); }
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
            UpdateTitle();
            Log(string.Format("agentSelection_Click {0}", currentAgentName));
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
            if (mcagent == null) { updateSoftwareToolStripMenuItem1.Visible = updateSoftwareToolStripMenuItem.Visible = false; return; }
            helpRequested = (mcagent.HelpRequest != null);
            if (mcagent.state != 3)
            {
                updateSoftwareToolStripMenuItem1.Visible = updateSoftwareToolStripMenuItem.Visible = false; // If not connected, don't offer auto-update option.
                if (guestSharingForm != null) { guestSharingForm.Close(); }
            }

            if (mcagent.autoConnect)
            {
                // In auto connect mode, we can only request help when connected.
                if (mcagent.state == 0) { stateLabel.Text = Translate.T(Properties.Resources.Connecting); requestHelpButton.Text = Translate.T(Properties.Resources.RequestHelp); helpRequested = false; }
                if (mcagent.state == 1) { stateLabel.Text = Translate.T(Properties.Resources.Connecting); requestHelpButton.Text = Translate.T(Properties.Resources.RequestHelp); }
                if (mcagent.state == 2) { stateLabel.Text = Translate.T(Properties.Resources.Authenticating); requestHelpButton.Text = Translate.T(Properties.Resources.RequestHelp); }
                if (mcagent.state == 3) {
                    if (helpRequested) {
                        stateLabel.Text = Translate.T(Properties.Resources.HelpRequested);
                        requestHelpButton.Text = Translate.T(Properties.Resources.CancelHelpRequest);
                    } else {
                        stateLabel.Text = Translate.T(Properties.Resources.Connected);
                        requestHelpButton.Text = Translate.T(Properties.Resources.RequestHelp);
                    }
                }
                Agent_onSessionChanged();
                requestHelpButton.Enabled = (mcagent.state == 3);

                // Update context menu
                requestHelpToolStripMenuItem.Enabled = true;
                requestHelpToolStripMenuItem.Visible = (mcagent.state == 3) && !helpRequested;
                cancelHelpRequestToolStripMenuItem.Visible = ((mcagent.state == 3) && helpRequested);

                // Update image
                if (mcagent.state == 3) {
                    if (helpRequested) { setUiImage(uiImage.Question); } else { setUiImage(uiImage.Green); }
                } else {
                    setUiImage(uiImage.Red);
                }
            }
            else
            {
                // When not in auto-connect mode, we only connect when requesting help.
                if (mcagent.state == 0) { stateLabel.Text = Translate.T(Properties.Resources.Disconnected); requestHelpButton.Text = Translate.T(Properties.Resources.RequestHelp); }
                if (mcagent.state == 1) { stateLabel.Text = Translate.T(Properties.Resources.Connecting); requestHelpButton.Text = Translate.T(Properties.Resources.CancelHelpRequest); }
                if (mcagent.state == 2) { stateLabel.Text = Translate.T(Properties.Resources.Authenticating); requestHelpButton.Text = Translate.T(Properties.Resources.CancelHelpRequest); }
                if (mcagent.state == 3) { stateLabel.Text = Translate.T(Properties.Resources.HelpRequested); requestHelpButton.Text = Translate.T(Properties.Resources.CancelHelpRequest); }
                Agent_onSessionChanged();
                requestHelpButton.Enabled = true;
                if (mcagent.state == 0) { helpRequested = false; }

                // Update context menu
                requestHelpToolStripMenuItem.Enabled = true;
                requestHelpToolStripMenuItem.Visible = (mcagent.state == 0);
                cancelHelpRequestToolStripMenuItem.Visible = (mcagent.state != 0);

                // Update image
                if (mcagent.state == 3) {
                    setUiImage(uiImage.Question);
                } else {
                    if (mcagent.state == 0) {
                        setUiImage(uiImage.Red);
                    } else {
                        setUiImage(uiImage.Yellow);
                    }
                }
            }

            string[] userids = getSessionUserIdList();
            if (userids.Length == 1)
            {
                string userid = userids[0];
                string[] useridsplit = userid.Split('/');
                string realname = useridsplit[2];
                userid = useridsplit[0] + '/' + useridsplit[1] + '/' + useridsplit[2];
                if ((mcagent.userrealname != null) && mcagent.userrealname.ContainsKey(userid) && (mcagent.userrealname[userid] != null)) { realname = mcagent.userrealname[userid]; }
                else if ((mcagent.usernames != null) && mcagent.usernames.ContainsKey(userid) && (mcagent.usernames[userid] != null)) { realname = mcagent.usernames[userid]; }
                string guestname = "";
                if ((useridsplit.Length == 4) && (useridsplit[3].StartsWith("guest:"))) { guestname = " - " + UTF8Encoding.UTF8.GetString(Convert.FromBase64String(useridsplit[3].Substring(6))); }
                stateLabel.Text = realname + guestname;
                if (realname.StartsWith("~")) { stateLabel.Text = guestname.Substring(3); }
                Image userImage = null;
                if (mcagent.userimages.ContainsKey(userid) && (mcagent.userimages[userid] != null)) { userImage = mcagent.userimages[userid]; }
                if (userImage == null) { setUiImage(uiImage.User); } else { setUiImage(userImage); }
            }
            if (userids.Length > 1)
            {
                stateLabel.Text = Translate.T(Properties.Resources.MultipleUsers);
                setUiImage(uiImage.Users);
            }
        }

        private void connectToAgent()
        {
            if (forceExit) return;
            Log(string.Format("connectToAgent {0}", currentAgentName));

            if (agent != null) { agent.DisconnectPipe(); agent = null; connectionTimer.Enabled = false; }
            if ((mcagent != null) && (mcagent.state != 0)) { mcagent.disconnect(); }
            if ((currentAgentName != null) && (currentAgentName.Equals("~")))
            {
                Settings.SetRegValue("SelectedAgent", currentAgentName);
                updateBuiltinAgentStatus();
                startAgentToolStripMenuItem.Visible = false;
                stopAgentToolStripMenuItem.Visible = false;
                toolStripMenuItem2.Visible = false;

                // If auto-connect is specified, connect now
                if (autoConnect)
                {
                    mcagent.HelpRequest = autoHelpRequest;
                    autoHelpRequest = null;
                    mcagent.connect();
                }

                updateBuiltinAgentStatus();
            }
            else
            {
                if (currentAgentName == null) {
                    agent = new MeshAgent("MeshCentralAssistant", "Mesh Agent", null, selfExecutableHashHex, debug);
                } else {
                    agent = new MeshAgent("MeshCentralAssistant", currentAgentName, agents[currentAgentName], selfExecutableHashHex, debug);
                    Settings.SetRegValue("SelectedAgent", currentAgentName);
                }
                connectionTimer.Enabled = true;
                agent.onStateChanged += Agent_onStateChanged;
                agent.onQueryResult += Agent_onQueryResult;
                agent.onSessionChanged += Agent_onSessionChanged;
                agent.onUserInfoChange += Agent_onUserInfoChange;
                agent.onAmtState += Agent_onAmtState;
                agent.onSelfUpdate += Agent_onSelfUpdate;
                agent.onCancelHelp += Agent_onCancelHelp;
                agent.onConsoleMessage += Agent_onConsoleMessage;
                agent.ConnectPipe();
                UpdateServiceStatus();

                setUiImage(uiImage.Yellow);

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
            }
            UpdateTitle();
            Agent_onSessionChanged();
        }

        private void UpdateTitle()
        {
            if (currentAgentName == "~")
            {
                if (mcagent.CustomizationTitle != null)
                {
                    this.Text = mcagent.CustomizationTitle;
                }
                else
                {
                    this.Text = Translate.T(Properties.Resources.MeshCentralAssistant);
                }
            }
            else
            {
                if (currentAgentName != "Mesh Agent")
                {
                    if (CustomizationTitle != null)
                    {
                        this.Text = CustomizationTitle + " - " + currentAgentName;
                    }
                    else
                    {
                        this.Text = string.Format(Translate.T(Properties.Resources.XAssistant), currentAgentName);
                    }
                }
                else
                {
                    if (CustomizationTitle != null)
                    {
                        this.Text = mainNotifyIcon.Text = CustomizationTitle;
                    }
                    else
                    {
                        this.Text = mainNotifyIcon.Text = Translate.T(Properties.Resources.MeshCentralAssistant);
                    }
                }
            }
            mainNotifyIcon.Text = this.Text;
        }

        private void Agent_onUserInfoChange(string userid, int change)
        {
            if (forceExit) return;
            if (this.InvokeRequired) { this.Invoke(new MeshAgent.onUserInfoChangeHandler(Agent_onUserInfoChange), userid, change); return; }
            Agent_onSessionChanged();
        }

        private void Agent_onConsoleMessage(string str)
        {
            if (forceExit) return;
            if (consoleForm == null) return;
            if (this.InvokeRequired) { this.Invoke(new MeshAgent.onConsoleHandler(Agent_onConsoleMessage), str); return; }
            consoleForm.appendText(str);
        }

        private void Agent_onCancelHelp()
        {
            if (forceExit) return;
            if (this.InvokeRequired) { this.Invoke(new MeshAgent.onCancelHelpHandler(Agent_onCancelHelp)); return; }
            if (helpRequested == true) { requestHelpButton_Click(null, null); }
        }

        private void Agent_onSelfUpdate(string name, string hash, string url, string serverhash)
        {
            if (forceExit) return;
            if (this.InvokeRequired) { this.Invoke(new MeshAgent.onSelfUpdateHandler(Agent_onSelfUpdate), name, hash, url, serverhash); return; }
            updateHash = hash;
            updateUrl = url;
            updateServerHash = serverhash;
            Log(string.Format("DownloadUpdate \"{0}\", \"{1}\", \"{2}\"", hash, url, serverhash));
            if (noUpdate == false) DownloadUpdate();
        }

        private void Agent_onAmtState(System.Collections.Generic.Dictionary<string, object> state)
        {
            if (forceExit) return;
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
        
        private string[] UserIdToAccountNames(string[] userids)
        {
            string[] r = new string[userids.Length];
            for (var i = 0; i < userids.Length; i++) {
                string u = userids[i];
                string[] uu = u.Split('/');
                r[i] = ((uu.Length == 3) ? uu[2] : u);
                if ((uu.Length == 4) && (uu[3].StartsWith("guest:"))) { r[i] = UTF8Encoding.UTF8.GetString(Convert.FromBase64String(uu[3].Substring(6))); }
            }
            return r;
        }

        private string[] UserIdToRealnames(string[] userids)
        {
            string[] r = new string[userids.Length];
            for (var i = 0; i < userids.Length; i++) {
                string u = userids[i];
                if ((mcagent.userrealname != null) && (mcagent.userrealname.ContainsKey(u)) && (mcagent.userrealname[u] != null)) {
                    r[i] = mcagent.userrealname[u];
                } else if ((mcagent.usernames != null) && (mcagent.usernames.ContainsKey(u)) && (mcagent.usernames[u] != null)) {
                    r[i] = mcagent.usernames[u];
                } else {
                    string[] uu = u.Split('/');
                    r[i] = ((uu.Length == 3) ? uu[2] : u);
                    if ((uu.Length == 4) && (uu[3].StartsWith("guest:"))) { r[i] = UTF8Encoding.UTF8.GetString(Convert.FromBase64String(uu[3].Substring(6))); }
                }
            }
            return r;
        }

        private void Agent_onSessionChanged()
        {
            if (forceExit) return;
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
                if (count > 1) { mainNotifyIcon.BalloonTipText = string.Format(Translate.T(Properties.Resources.XRemoteSessionsAreActive), count); remoteSessionsLabel.Text = string.Format(Translate.T(Properties.Resources.XRemoteSessions), count); this.Visible = true; }
                if (count == 1) { mainNotifyIcon.BalloonTipText = Translate.T(Properties.Resources.OneRemoteSessionIsActive); remoteSessionsLabel.Text = Translate.T(Properties.Resources.OneRemoteSession); this.Visible = true; }
                if (count == 0) { mainNotifyIcon.BalloonTipText = Translate.T(Properties.Resources.NoActiveRemoteSessions); remoteSessionsLabel.Text = Translate.T(Properties.Resources.NoRemoteSessions); }
                if (count > 0) {
                    // {0} = Realname, {1} = Username
                    string privacyBarText = (mcagent.privacyBarText != null) ? mcagent.privacyBarText : Translate.T(Properties.Resources.RemoteSessionsX);
                    string[] userids = getSessionUserIdList();
                    string[] accountNames = UserIdToAccountNames(userids);
                    string[] realnames = UserIdToRealnames(userids);
                    PrivacyBarShow(privacyBarText.Replace("{1}", string.Join(", ", accountNames)).Replace("{0}", string.Join(", ", realnames)));
                } else {
                    PrivacyBarClose();
                }
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
                if (count > 1) { mainNotifyIcon.BalloonTipText = string.Format(Translate.T(Properties.Resources.XRemoteSessionsAreActive), count); remoteSessionsLabel.Text = string.Format(Translate.T(Properties.Resources.XRemoteSessions), count); this.Visible = true; }
                if (count == 1) { mainNotifyIcon.BalloonTipText = Translate.T(Properties.Resources.OneRemoteSessionIsActive); remoteSessionsLabel.Text = Translate.T(Properties.Resources.OneRemoteSession); this.Visible = true; }
                if (count == 0) { mainNotifyIcon.BalloonTipText = Translate.T(Properties.Resources.NoActiveRemoteSessions); remoteSessionsLabel.Text = Translate.T(Properties.Resources.NoRemoteSessions); }
                //mainNotifyIcon.ShowBalloonTip(2000);
                if (sessionsForm != null) { sessionsForm.UpdateInfo(); }

                stateLabel.Text = Translate.T(Properties.Resources.ConnectedToServer);
                setUiImage(uiImage.Green);

                string[] userids = getSessionUserIdList2();
                if (userids.Length == 1)
                {
                    string userid = userids[0];
                    string[] useridsplit = userid.Split('/');
                    if (useridsplit.Length > 3) { userid = useridsplit[0] + '/' + useridsplit[1] + '/' + useridsplit[2]; }
                    string realname = useridsplit[2];
                    string guestname = "";
                    if ((useridsplit.Length == 4) && (useridsplit[3].StartsWith("guest:"))) { guestname = " - " + UTF8Encoding.UTF8.GetString(Convert.FromBase64String(useridsplit[3].Substring(6))); }
                    if ((agent.userrealname != null) && (agent.userrealname.ContainsKey(userid)) && (agent.userrealname[userid] != null)) { realname = agent.userrealname[userid]; }
                    else if ((agent.usernames != null) && (agent.usernames.ContainsKey(userid)) && (agent.usernames[userid] != null)) { realname = agent.usernames[userid]; }
                    stateLabel.Text = realname + guestname;
                    Image userImage = null;
                    if ((agent.userimages != null) && agent.userimages.ContainsKey(userid) && (agent.userimages[userid] != null)) { userImage = agent.userimages[userid]; }
                    if (userImage == null) { setUiImage(uiImage.User); } else { setUiImage(userImage); }
                }
                if (userids.Length > 1)
                {
                    stateLabel.Text = Translate.T(Properties.Resources.MultipleUsers);
                    setUiImage(uiImage.Users);
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
                setUiImage(uiImage.Red);
                switch (status)
                {
                    case ServiceControllerStatus.ContinuePending: { stateLabel.Text = Translate.T(Properties.Resources.AgentIsContinuePending); break; }
                    case ServiceControllerStatus.Paused: { stateLabel.Text = Translate.T(Properties.Resources.AgentIsPaused); break; }
                    case ServiceControllerStatus.PausePending: { stateLabel.Text = Translate.T(Properties.Resources.AgentIsPausePending); break; }
                    case ServiceControllerStatus.Running: { stateLabel.Text = Translate.T(Properties.Resources.AgentIsRunning); break; }
                    case ServiceControllerStatus.StartPending: { stateLabel.Text = Translate.T(Properties.Resources.AgentIsStartPending); break; }
                    case ServiceControllerStatus.Stopped: { stateLabel.Text = Translate.T(Properties.Resources.AgentIsStopped); break; }
                    case ServiceControllerStatus.StopPending: { stateLabel.Text = Translate.T(Properties.Resources.AgentIsStoppedPending); break; }
                }
            }
            catch (Exception)
            {
                startAgentToolStripMenuItem.Enabled = false;
                stopAgentToolStripMenuItem.Enabled = false;
                stateLabel.Text = Translate.T(Properties.Resources.AgentNotInstalled);
                setUiImage(uiImage.Red);
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


        private void Agent_onStateChanged(int state, int serverState)
        {
            if (forceExit) return;
            if (this.InvokeRequired) { this.Invoke(new MeshAgent.onStateChangedHandler(Agent_onStateChanged), state, serverState); return; }

            Log(string.Format("Agent_onStateChanged {0}, {1}, {2}", state, serverState, currentAgentName));

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
                        setUiImage(uiImage.Red);
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
                            setUiImage(uiImage.Green);
                            stateLabel.Text = Translate.T(Properties.Resources.ConnectedToServer);
                            requestHelpToolStripMenuItem.Enabled = true;
                            requestHelpToolStripMenuItem.Visible = true;
                            cancelHelpRequestToolStripMenuItem.Visible = false;
                            if (autoHelpRequest != null) { RequestHelp(autoHelpRequest); autoHelpRequest = null; } // Automatically request help
                        }
                        else
                        {
                            setUiImage(uiImage.Yellow);
                            stateLabel.Text = Translate.T(Properties.Resources.AgentIsDisconnected);
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
            requestHelpButton.Text = Translate.T(Properties.Resources.RequestHelp);
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
                if (updateForm != null) { updateForm.Close(); }
            }
        }

        private void Agent_onQueryResult(string value, string result)
        {
            if (forceExit) return;
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
            if (forceExit) return;
            if (agent != null)
            {
                if (timerSlowDown > 0) { timerSlowDown--; if (timerSlowDown == 0) { connectionTimer.Interval = 10000; } }
                if (agent.State == 0) { agent.ConnectPipe(); }
                UpdateServiceStatus();
            }
            else
            {
                connectionTimer.Enabled = false;
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (forceExit || (e.CloseReason != CloseReason.UserClosing)) return;
            if (SystemTrayApp)
            {
                if (doclose == false) {
                    e.Cancel = true;
                    this.Visible = false;
                    return;
                }
            }
            else
            {
                if ((mcagent != null) && (currentAgentName != null) && (currentAgentName.Equals("~")) && (mcagent.state == 3))
                {
                    if (MessageBox.Show(this, Properties.Resources.DisconnectFromServerAndClose, this.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1) != DialogResult.OK)
                    {
                        e.Cancel = true;
                        return;
                    }
                }
            }

            forceExit = true;
            if (mcagent != null) { mcagent.disconnect(); }
        }

        private void openSiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(agent.ServerUri.ToString());
        }

        private void mainNotifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if ((e == null) || (e.Button == System.Windows.Forms.MouseButtons.Left)) {
                this.StartPosition = FormStartPosition.Manual;
                this.Location = new Point(Screen.PrimaryScreen.WorkingArea.Width - this.Width, Screen.PrimaryScreen.WorkingArea.Height - this.Height);
                this.WindowState = FormWindowState.Normal;
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
            Log(string.Format("requestHelpButton_Click {0}", currentAgentName));
            if (currentAgentName.Equals("~"))
            {
                if (autoConnect)
                {
                    // When in auto-connect mode, we can only request help when connected
                    if (mcagent.state != 3) return;
                    if (helpRequested)
                    {
                        // Cancel help request
                        mcagent.RequestHelp(null);
                        helpRequested = false;
                        updateBuiltinAgentStatus();
                    }
                    else
                    {
                        // No help currently requested, request it now.
                        if (requestHelpForm != null)
                        {
                            requestHelpForm.Focus();
                        }
                        else
                        {
                            requestHelpForm = new RequestHelpForm(this);
                            requestHelpForm.NoHelpRequestOk = false;
                            requestHelpForm.Show(this);
                        }
                    }
                }
                else
                {
                    // When not in auto-connect mode, we connect when requesting help and disconnect when canceling help request.
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
            }
            else
            {
                if (helpRequested == true)
                {
                    if ((agent == null) || (agent.CancelHelpRequest() == true))
                    {
                        helpRequested = false;
                        requestHelpButton.Text = Translate.T(Properties.Resources.RequestHelp);
                        stateLabel.Text = Translate.T(Properties.Resources.ConnectedToServer);
                        requestHelpToolStripMenuItem.Visible = true;
                        cancelHelpRequestToolStripMenuItem.Visible = false;
                        setUiImage(uiImage.Green);
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
            if (forceExit) return;
            Log(string.Format("RequestHelp {0}, \"{1}\"", currentAgentName, details));
            if (currentAgentName.Equals("~"))
            {
                if (autoConnect)
                {
                    if (mcagent.state == 3) {
                        helpRequested = true;
                        mcagent.RequestHelp(details);
                        updateBuiltinAgentStatus();
                    }
                }
                else
                {
                    if (details.Length > 0) { mcagent.HelpRequest = details; } else { mcagent.HelpRequest = null; }
                    mcagent.connect();
                }
            }
            else
            {
                if (agent.RequestHelp(details) == true)
                {
                    helpRequested = true;
                    requestHelpButton.Text = Translate.T(Properties.Resources.CancelHelpRequest);
                    stateLabel.Text = Translate.T(Properties.Resources.HelpRequested);
                    requestHelpToolStripMenuItem.Visible = false;
                    cancelHelpRequestToolStripMenuItem.Visible = true;
                    setUiImage(uiImage.Question);
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
            requestHelpToolStripMenuItem.Enabled = ((mcagent != null) && (mcagent.state != 0)) || ((agent != null) && (agent.State != 0));
            guestSharingToolStripMenuItem.Visible = ((mcagent != null) && (mcagent.selfSharingAllowed == true));
            guestSharingToolStripMenuItem.Enabled = (mcagent.state == 3);
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
        // Private bar section
        //

        public void PrivacyBarShow(string msg)
        {
            if (privacyBars != null)
            {
                // Update all privacy bar text
                Log(string.Format("PrivacyBarShow Update:\"{0}\"", msg));
                foreach (PrivacyBarForm f in privacyBars) { f.privacyText = msg; }
            }
            else
            {
                // Show all privacy bars
                Log(string.Format("PrivacyBarShow Show:\"{0}\"", msg));
                privacyBars = new List<PrivacyBarForm>();
                foreach (Screen s in Screen.AllScreens)
                {
                    PrivacyBarForm f = new PrivacyBarForm(this, s);
                    f.privacyText = msg;
                    f.Show();
                    privacyBars.Add(f);
                }
            }
        }

        public void PrivacyBarClose()
        {
            // Close all privacy bars
            if (privacyBars == null) return;
            Log("PrivacyBarClose");
            foreach (PrivacyBarForm f in privacyBars) { f.Close(); }
            privacyBars.Clear();
            privacyBars = null;

            // Drop all mcagent sessions
            if (mcagent != null) { mcagent.disconnectAllTunnels(); }
        }


        //
        // Self update section
        //

        string seflUpdateDownloadHash = null;
        string serverTlsCertHash = null;

        private void DownloadUpdate()
        {
            updateForm = new UpdateForm(this);
            updateForm.Show(this);
        }

        public void DownloadUpdateEx(bool doUpdateNow)
        {
            updateForm.Dispose();
            updateForm = null;
            if (doUpdateNow)
            {
                Event("Local", "Approved software update: " + updateUrl);
                updateSoftwareToolStripMenuItem1.Visible = updateSoftwareToolStripMenuItem.Visible = false;
                seflUpdateDownloadHash = updateHash;
                serverTlsCertHash = updateServerHash;
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(updateUrl);
                webRequest.Method = "GET";
                webRequest.Timeout = 10000;
                webRequest.BeginGetResponse(new AsyncCallback(DownloadUpdateRespone), webRequest);
                webRequest.ServerCertificateValidationCallback += RemoteCertificateValidationCallback;
            }
            else
            {
                Event("Local", "Delayed software update");
                updateSoftwareToolStripMenuItem1.Visible = updateSoftwareToolStripMenuItem.Visible = true;
            }
        }

        public bool RemoteCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            // Check MeshCentral server's TLS certificate. This is our first security layer.
            if ((serverTlsCertHash != null) && (serverTlsCertHash != certificate.GetCertHashString().ToLower()) && (serverTlsCertHash != GetMeshKeyHash(certificate).ToLower()) && (serverTlsCertHash != GetMeshCertHash(certificate).ToLower()))
            {
                Log("RemoteCertificateValidationCallback - Invalid");
                return false;
            }
            Log("RemoteCertificateValidationCallback - OK");
            return true;
        }

        private HttpWebResponse updateWebResponse = null;
        private Stream updateHttpInputStream = null;
        private FileStream updateFileOutputStream = null;

        private void DownloadUpdateRespone(IAsyncResult asyncResult)
        {
            try
            {
                HttpWebRequest webRequest = (HttpWebRequest)asyncResult.AsyncState;
                byte[] buffer = new byte[4096];
                updateWebResponse = (HttpWebResponse)webRequest.EndGetResponse(asyncResult);
                updateFileOutputStream = File.Create(Assembly.GetEntryAssembly().Location + ".update.exe");
                updateHttpInputStream = updateWebResponse.GetResponseStream();
                updateHttpInputStream.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(DownloadUpdateTransfer), buffer);
            }
            catch (Exception)
            {
                try { updateHttpInputStream.Close(); } catch (Exception) { }
                try { updateFileOutputStream.Close(); } catch (Exception) { }
                try { updateWebResponse.Close(); } catch (Exception) { }
                updateFileOutputStream = null;
                updateHttpInputStream = null;
                updateWebResponse = null;
            }
        }

        private void DownloadUpdateTransfer(IAsyncResult asyncResult)
        {
            try
            {

                byte[] buffer = (byte[])asyncResult.AsyncState;
                int len = updateHttpInputStream.EndRead(asyncResult);
                if (len > 0)
                {
                    updateFileOutputStream.Write(buffer, 0, len);
                    updateHttpInputStream.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(DownloadUpdateTransfer), buffer);
                }
                else
                {
                    // Close the streams
                    try { updateHttpInputStream.Close(); } catch (Exception) { }
                    try { updateFileOutputStream.Close(); } catch (Exception) { }
                    try { updateWebResponse.Close(); } catch (Exception) { }
                    updateFileOutputStream = null;
                    updateHttpInputStream = null;
                    updateWebResponse = null;

                    // Hash the resulting file, check that it's correct. This is our second security layer.
                    byte[] downloadHash;
                    using (var sha384 = SHA384Managed.Create()) { using (var stream = File.OpenRead(Assembly.GetEntryAssembly().Location + ".update.exe")) { downloadHash = sha384.ComputeHash(stream); } }
                    string downloadHashHex = BitConverter.ToString(downloadHash).Replace("-", string.Empty).ToLower();
                    bool hashMatch = (downloadHashHex == seflUpdateDownloadHash);
                    if (hashMatch == false) { hashMatch = (ExeHandler.HashExecutable(Assembly.GetEntryAssembly().Location + ".update.exe") == seflUpdateDownloadHash); }
                    if (hashMatch == false)
                    {
                        Log("DownloadUpdateRespone - Invalid hash");
                        System.Threading.Thread.Sleep(500);
                        File.Delete(Assembly.GetEntryAssembly().Location + ".update.exe");
                    }
                    else
                    {
                        Log("DownloadUpdateRespone - OK");
                        doclose = true;
                        forceExit = true;
                        System.Threading.Thread.Sleep(500);
                        string arguments = "-update:" + Assembly.GetEntryAssembly().Location + " " + string.Join(" ", args);
                        if (this.Visible == true) { arguments += " -visible"; }
                        Process.Start(Assembly.GetEntryAssembly().Location + ".update.exe", arguments);
                        Application.Exit();
                    }
                }
            }
            catch (Exception)
            {
                // Clean up
                try { updateHttpInputStream.Close(); } catch (Exception) { }
                try { updateFileOutputStream.Close(); } catch (Exception) { }
                try { updateWebResponse.Close(); } catch (Exception) { }
                updateFileOutputStream = null;
                updateHttpInputStream = null;
                updateWebResponse = null;
            }
        }

        /*
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
                    FileStream fileStream = File.OpenWrite(Assembly.GetEntryAssembly().Location + ".update.exe");
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
                    using (var sha384 = SHA384Managed.Create()) { using (var stream = File.OpenRead(Assembly.GetEntryAssembly().Location + ".update.exe")) { downloadHash = sha384.ComputeHash(stream); } }
                    string downloadHashHex = BitConverter.ToString(downloadHash).Replace("-", string.Empty).ToLower();
                    bool hashMatch = (downloadHashHex == seflUpdateDownloadHash);
                    if (hashMatch == false) { hashMatch = (ExeHandler.HashExecutable(Assembly.GetEntryAssembly().Location + ".update.exe") == seflUpdateDownloadHash); }
                    if (hashMatch == false)
                    {
                        Log("DownloadUpdateRespone - Invalid hash");
                        System.Threading.Thread.Sleep(500);
                        File.Delete(Assembly.GetEntryAssembly().Location + ".update.exe");
                    }
                    else
                    {
                        Log("DownloadUpdateRespone - OK");
                        doclose = true;
                        forceExit = true;
                        System.Threading.Thread.Sleep(500);
                        string arguments = "-update:" + Assembly.GetEntryAssembly().Location + " " + string.Join(" ", args);
                        if (this.Visible == true) { arguments += " -visible"; }
                        Process.Start(Assembly.GetEntryAssembly().Location + ".update.exe", arguments);
                        Application.Exit();
                    }
                }
            }
            catch (Exception) { }
        }
        */

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

        private void updateSoftwareToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if ((updateHash == null) || (updateUrl == null) || (updateServerHash == null)) return;
            DownloadUpdate();
        }

        private void showEventsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (eventsForm == null)
            {
                eventsForm = new EventsForm(this, userEvents);
                eventsForm.Show(this);
            }
            else
            {
                eventsForm.Focus();
            }
        }

        public void closingEventForm()
        {
            eventsForm.Dispose();
            eventsForm = null;
        }

        public delegate void OpenBrowserHandler(string url);

        public void OpenBrowser(string url)
        {
            if (this.InvokeRequired) { this.Invoke(new OpenBrowserHandler(OpenBrowser), url); return; }
            if (browserForm == null)
            {
                browserForm = new BrowserForm(this);
                browserForm.setUrl(url);
                browserForm.Show(this);
            }
            else
            {
                browserForm.Focus();
            }
        }

        public void CloseBrowser()
        {
            browserForm.Dispose();
            browserForm = null;
        }

        private enum uiImage {
            Green = 1,
            Yellow = 2,
            Red = 3,
            Question = 4,
            User = 5,
            Users = 6,
            Custom = 7
        }

        private void setUiImage(uiImage x)
        {
            mainPictureBox.Visible = (x != uiImage.Custom);
            pictureBoxCustom.Visible = (x == uiImage.Custom);
            switch (x)
            {
                case uiImage.Green:
                    if ((currentAgentName == "~") && (mcagent.CustomizationLogo != null)) { mainPictureBox.Image = mcagent.CustomizationLogo; } else { if (CustomizationLogo == null) { mainPictureBox.Image = Properties.Resources.MeshCentral; } else { mainPictureBox.Image = CustomizationLogo; } }
                    mainPictureBox.BackgroundImage = Properties.Resources.Green;
                    break;
                case uiImage.Yellow:
                    if ((currentAgentName == "~") && (mcagent.CustomizationLogo != null)) { mainPictureBox.Image = mcagent.CustomizationLogo; } else { if (CustomizationLogo == null) { mainPictureBox.Image = Properties.Resources.MeshCentral; } else { mainPictureBox.Image = CustomizationLogo; } }
                    mainPictureBox.BackgroundImage = Properties.Resources.Yellow;
                    break;
                case uiImage.Red:
                    if ((currentAgentName == "~") && (mcagent.CustomizationLogo != null)) { mainPictureBox.Image = mcagent.CustomizationLogo; } else { if (CustomizationLogo == null) { mainPictureBox.Image = Properties.Resources.MeshCentral; } else { mainPictureBox.Image = CustomizationLogo; } }
                    mainPictureBox.BackgroundImage = Properties.Resources.Red;
                    break;
                case uiImage.Question:
                    mainPictureBox.Image = Properties.Resources.Question;
                    mainPictureBox.BackgroundImage = Properties.Resources.Green;
                    break;
                case uiImage.User:
                    mainPictureBox.Image = Properties.Resources.User;
                    mainPictureBox.BackgroundImage = Properties.Resources.Green;
                    break;
                case uiImage.Users:
                    mainPictureBox.Image = Properties.Resources.Users;
                    mainPictureBox.BackgroundImage = Properties.Resources.Green;
                    break;
            }
        }

        private void setUiImage(Image x)
        {
            pictureBoxCustom.Image = RoundCorners(x, 30, this.BackColor);
            setUiImage(uiImage.Custom);
        }

        private void guestSharingToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
        }

        private void guestSharingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if ((mcagent == null) || (mcagent.state != 3) || (mcagent.selfSharingAllowed == false)) return;
            if (guestSharingForm != null)
            {
                guestSharingForm.Focus();
            }
            else
            {
                guestSharingForm = new GuestSharingForm(this);
                guestSharingForm.Show();
            }
        }
    }
}
