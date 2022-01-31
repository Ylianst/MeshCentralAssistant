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
using System.IO;
using System.IO.Compression;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Web.Script.Serialization;

namespace MeshAssistant
{
    public class MeshCentralTunnel
    {
        private Dictionary<string, object> creationArgs = null;
        private int state = 0; // 0 = Waitting for "c" or "cr", 1 = Waitting for protocol number, 2 = Running
        public int protocol = 0; // 1 = Terminal, 2 = Desktop, 5 = Files, 10 = File Transfer
        //private bool serverRecording = false;
        private MeshCentralAgent parent = null;
        public webSocketClient WebSocket = null;
        private JavaScriptSerializer JSON = new JavaScriptSerializer();
        private MeshCentralDesktop Desktop = null;
        private MeshCentralTerminal Terminal = null;
        private Dictionary<string, object> jsonOptions = null;
        private FileStream fileSendTransfer = null;
        private string fileSendTransferId = null;
        private FileStream fileRecvTransfer = null;
        private string fileRecvTransferPath = null;
        private int fileRecvTransferSize = 0;
        private int fileRecvId = 0;
        private string extraLogStr = "";
        public string sessionUserName = null;
        public string sessionLoggingUserName = null;
        public bool consentRequested = false;
        private ArrayList commandsOnHold = null;
        public long userRights = 0;
        public string userid = null;
        public string guestname = null;
        public bool disconnected = false;

        public void Log(string msg)
        {
            try { File.AppendAllText("debug.log", DateTime.Now.ToString("HH:mm:tt.ffff") + ": MCAgent: " + msg + "\r\n"); } catch (Exception) { }
        }

        public enum MeshRights : long
        {
            EDITMESH            = 0x00000001,
            MANAGEUSERS         = 0x00000002,
            MANAGECOMPUTERS     = 0x00000004,
            REMOTECONTROL       = 0x00000008,
            AGENTCONSOLE        = 0x00000010,
            SERVERFILES         = 0x00000020,
            WAKEDEVICE          = 0x00000040,
            SETNOTES            = 0x00000080,
            REMOTEVIEWONLY      = 0x00000100,
            NOTERMINAL          = 0x00000200,
            NOFILES             = 0x00000400,
            NOAMT               = 0x00000800,
            DESKLIMITEDINPUT    = 0x00001000,
            LIMITEVENTS         = 0x00002000,
            CHATNOTIFY          = 0x00004000,
            UNINSTALL           = 0x00008000,
            NODESKTOP           = 0x00010000,
            REMOTECOMMAND       = 0x00020000,
            RESETOFF            = 0x00040000,
            GUESTSHARING        = 0x00080000,
            ADMIN               = 0xFFFFFFFF
        }

        public MeshCentralTunnel(MeshCentralAgent parent, Uri uri, string serverHash, Dictionary<string, object> creationArgs)
        {
            this.parent = parent;
            this.creationArgs = creationArgs;
            WebSocket = new webSocketClient();
            WebSocket.allowUseOfProxy = parent.allowUseOfProxy;
            WebSocket.pongTimeSeconds = 120; // Send a websocket pong every 2 minutes.
            WebSocket.onStateChanged += WebSocket_onStateChanged;
            WebSocket.onBinaryData += WebSocket_onBinaryData;
            WebSocket.onStringData += WebSocket_onStringData;
            WebSocket.TLSCertCheck = webSocketClient.TLSCertificateCheck.Fingerprint;
            WebSocket.Start(uri, serverHash, null);

            // Setup extra log values
            if (creationArgs != null)
            {
                if (creationArgs.ContainsKey("userid") && (creationArgs["userid"].GetType() == typeof(string))) { extraLogStr += ",\"userid\":\"" + escapeJsonString((string)creationArgs["userid"]) + "\""; userid = (string)creationArgs["userid"]; }
                if (creationArgs.ContainsKey("username") && (creationArgs["username"].GetType() == typeof(string))) { extraLogStr += ",\"username\":\"" + escapeJsonString((string)creationArgs["username"]) + "\""; }
                if (creationArgs.ContainsKey("guestname") && (creationArgs["guestname"].GetType() == typeof(string))) { extraLogStr += ",\"guestname\":\"" + escapeJsonString((string)creationArgs["guestname"]) + "\""; guestname = (string)creationArgs["guestname"]; }
                if (creationArgs.ContainsKey("remoteaddr") && (creationArgs["remoteaddr"].GetType() == typeof(string))) { extraLogStr += ",\"remoteaddr\":\"" + escapeJsonString((string)creationArgs["remoteaddr"]) + "\""; }
                if (creationArgs.ContainsKey("sessionid") && (creationArgs["sessionid"].GetType() == typeof(string))) { extraLogStr += ",\"sessionid\":\"" + escapeJsonString((string)creationArgs["sessionid"]) + "\""; }
                if (creationArgs.ContainsKey("rights")) {
                    if (creationArgs["rights"].GetType() == typeof(System.Int32)) { userRights = (int)creationArgs["rights"]; }
                    if (creationArgs["rights"].GetType() == typeof(System.Int64)) { userRights = (long)creationArgs["rights"]; }
                }
            }
        }

        private void WebSocket_onStateChanged(webSocketClient sender, webSocketClient.ConnectionStates state)
        {
            if (state == webSocketClient.ConnectionStates.Disconnected)
            {
                disconnect();
            }
            else if (state == webSocketClient.ConnectionStates.Connecting)
            {

            }
            else if (state == webSocketClient.ConnectionStates.Connected)
            {

            }
        }

        public void disconnect()
        {
            if (disconnected == true) return;
            disconnected = true;
            if (fileSendTransfer != null) { fileSendTransfer.Close(); fileSendTransfer = null; }
            if (fileRecvTransfer != null) { fileRecvTransfer.Close(); fileRecvTransfer = null; }
            if (WebSocket != null) { WebSocket.Dispose(); WebSocket = null; }
            if (Terminal != null) { Terminal.Dispose(); Terminal = null; }
            if (Desktop != null) { MeshCentralDesktop.RemoveDesktopTunnel(this); Desktop = null; }

            // Cancel the consent request if active
            if (consentRequested == true) {
                parent.askForConsent(this, null, protocol, sessionUserName);
                consentRequested = false;
            }

            // Update session
            if (sessionUserName != null)
            {
                if ((protocol == 1) || (protocol == 8) || (protocol == 9)) // Terminal
                {
                    if (parent.TerminalSessions != null)
                    {
                        if (parent.TerminalSessions.ContainsKey(sessionUserName))
                        {
                            parent.TerminalSessions[sessionUserName] = (int)parent.TerminalSessions[sessionUserName] - 1;
                            if ((int)parent.TerminalSessions[sessionUserName] == 0) { parent.TerminalSessions.Remove(sessionUserName); }
                        }
                        parent.fireSessionChanged(1);
                    }
                }
                else if (protocol == 2) // Desktop
                {
                    if (parent.DesktopSessions != null)
                    {
                        if (parent.DesktopSessions.ContainsKey(sessionUserName))
                        {
                            parent.DesktopSessions[sessionUserName] = (int)parent.DesktopSessions[sessionUserName] - 1;
                            if ((int)parent.DesktopSessions[sessionUserName] == 0) { parent.DesktopSessions.Remove(sessionUserName); }
                        }
                        parent.fireSessionChanged(2);
                    }
                }
                else if (protocol == 5) // Files
                {
                    if (parent.FilesSessions != null)
                    {
                        if (parent.FilesSessions.ContainsKey(sessionUserName))
                        {
                            parent.FilesSessions[sessionUserName] = (int)parent.FilesSessions[sessionUserName] - 1;
                            if ((int)parent.FilesSessions[sessionUserName] == 0) { parent.FilesSessions.Remove(sessionUserName); }
                        }
                        parent.fireSessionChanged(5);
                    }
                }
                sessionUserName = null;
                sessionLoggingUserName = null;
            }

            // Update event log
            if ((protocol == 1) || (protocol == 8) || (protocol == 9)) { parent.Event(userid, "Closed terminal session"); }
            else if (protocol == 2) { parent.Event(userid, "Closed desktop session"); }
            else if (protocol == 5) { parent.Event(userid, "Closed files session"); }
        }
        
        private void connectedToServer()
        {

        }

        public void processServerJsonData(string data)
        {
            // Parse the received JSON
            Dictionary<string, object> jsonAction = new Dictionary<string, object>();
            try { jsonAction = JSON.Deserialize<Dictionary<string, object>>(data); } catch (Exception) { return; }
            if (jsonAction == null) return;
            if (jsonAction.ContainsKey("ctrlChannel") && (jsonAction["ctrlChannel"].GetType() == typeof(string)) && (((string)jsonAction["ctrlChannel"]).Equals("102938")))
            {
                // This is a control command
                if (!jsonAction.ContainsKey("type") || jsonAction["type"].GetType() != typeof(string)) return;
                string type = (string)jsonAction["type"];
                if (type.Equals("close")) { disconnect(); }
                if ((type.Equals("termsize")) && (Terminal != null)) {
                    int cols = 0;
                    int rows = 0;
                    if (jsonAction.ContainsKey("cols") && (jsonAction["cols"].GetType() == typeof(int))) { cols = (int)jsonAction["cols"]; }
                    if (jsonAction.ContainsKey("rows") && (jsonAction["rows"].GetType() == typeof(int))) { rows = (int)jsonAction["rows"]; }
                    if ((cols > 0) && (rows > 0)) { Terminal.Resize(cols, rows); }
                }
                return;
            }

            if (!jsonAction.ContainsKey("action") || jsonAction["action"].GetType() != typeof(string)) return;

            // Handle the JSON command
            string action = jsonAction["action"].ToString();
            switch (action)
            {
                case "ping": { WebSocket.SendBinary(UTF8Encoding.UTF8.GetBytes("{\"action\":\"pong\"}")); break; }
                case "pong": { break; }
                case "errorlog": { break; }
                case "sysinfo": { break; }
                case "coredump": { break; }
                case "getcoredump": { break; }
                default: { break; }
            }
        }

        private void WebSocket_onStringData(webSocketClient sender, string data, int orglen)
        {
            try
            {
                if (state == 0) // Waitting for browser connection
                {
                    if (data.Equals("c")) { state = 1; connectedToServer(); }
                    else if (data.Equals("cr")) { state = 1; /* serverRecording = true; */ connectedToServer(); }
                    return;
                }
                else if (state == 1) // Waitting for protocol
                {
                    // Parse the received JSON options
                    if (data.StartsWith("{")) {
                        try { jsonOptions = JSON.Deserialize<Dictionary<string, object>>(data); } catch (Exception) { }
                        if ((!jsonOptions.ContainsKey("type")) || (jsonOptions["type"].GetType() != typeof(string)) || (!((string)jsonOptions["type"]).Equals("options"))) { jsonOptions = null; }
                        return;
                    }
                    if (int.TryParse(data, out protocol)) { state = 2; }

                    // Add this session
                    if (creationArgs != null)
                    {
                        //if (creationArgs.ContainsKey("username") && (creationArgs["username"].GetType() == typeof(string))) { sessionUserName = (string)creationArgs["username"]; }
                        //if (creationArgs.ContainsKey("realname") && (creationArgs["realname"].GetType() == typeof(string))) { sessionUserName = (string)creationArgs["realname"]; }
                        if (creationArgs.ContainsKey("userid") && (creationArgs["userid"].GetType() == typeof(string))) {
                            sessionLoggingUserName = sessionUserName = (string)creationArgs["userid"];
                            if (creationArgs.ContainsKey("guestname") && (creationArgs["guestname"].GetType() == typeof(string))) {
                                sessionUserName += "/guest:" + Convert.ToBase64String(UTF8Encoding.UTF8.GetBytes((string)creationArgs["guestname"]));
                                sessionLoggingUserName += " - " + (string)creationArgs["guestname"];
                            }
                        } else {
                            sessionUserName = "user//~guest/guest:" + Convert.ToBase64String(UTF8Encoding.UTF8.GetBytes("Guest"));
                        }

                        if (sessionUserName != null)
                        {
                            if ((protocol == 1) || (protocol == 8) || (protocol == 9)) // Terminal: 1 = Admin Shell, 8 = User Shell, 9 = User PowerShell
                            {
                                if (parent.terminalSupport == false) { disconnect(); return; } // Terminal not supported
                                if (parent.TerminalSessions == null) { parent.TerminalSessions = new Dictionary<string, object>(); }
                                if (parent.TerminalSessions.ContainsKey(sessionUserName)) { parent.TerminalSessions[sessionUserName] = (int)parent.TerminalSessions[sessionUserName] + 1; } else { parent.TerminalSessions[sessionUserName] = 1; }
                                parent.fireSessionChanged(2);
                                parent.Event(userid, "Started terminal session");
                            }
                            if (protocol == 2) // Desktop
                            {
                                if (parent.DesktopSessions == null) { parent.DesktopSessions = new Dictionary<string, object>(); }
                                if (parent.DesktopSessions.ContainsKey(sessionUserName)) { parent.DesktopSessions[sessionUserName] = (int)parent.DesktopSessions[sessionUserName] + 1; } else { parent.DesktopSessions[sessionUserName] = 1; }
                                parent.fireSessionChanged(2);
                                parent.Event(userid, "Started desktop session");
                            }
                            if (protocol == 5) // Files
                            {
                                if (parent.FilesSessions == null) { parent.FilesSessions = new Dictionary<string, object>(); }
                                if (parent.FilesSessions.ContainsKey(sessionUserName)) { parent.FilesSessions[sessionUserName] = (int)parent.FilesSessions[sessionUserName] + 1; } else { parent.FilesSessions[sessionUserName] = 1; }
                                parent.fireSessionChanged(5);
                                parent.Event(userid, "Started files session");
                            }
                        }
                    }

                    int consent = 0;
                    if (creationArgs.ContainsKey("consent") && (creationArgs["consent"].GetType() == typeof(int))) { consent = (int)creationArgs["consent"]; }
                    if (creationArgs.ContainsKey("privacybartext") && (creationArgs["privacybartext"].GetType() == typeof(string))) {
                        if (parent.privacyBarText != (string)creationArgs["privacybartext"]) { parent.privacyBarText = (string)creationArgs["privacybartext"]; parent.PrivacyTextChanged(); }
                    }

                    if ((protocol == 1) || (protocol == 8) || (protocol == 9)) // Terminal
                    {
                        if (((consent & 0x10) != 0) || parent.autoConnect) // Remote Terminal Consent Prompt
                        {
                            consentRequested = true;
                            setConsoleText("Waiting for user to grant access...", 1, null, 0);
                            parent.askForConsent(this, "User \"{0}\" is requesting remote terminal control of this computer. Click allow to grant access.", protocol, sessionLoggingUserName);
                            parent.Event(userid, "Requesting user consent for terminal session");
                        }
                        else
                        {
                            int cols = 80;
                            int rows = 25;
                            if (jsonOptions != null)
                            {
                                if (jsonOptions.ContainsKey("cols") && (jsonOptions["cols"].GetType() == typeof(int))) { cols = (int)jsonOptions["cols"]; }
                                if (jsonOptions.ContainsKey("rows") && (jsonOptions["rows"].GetType() == typeof(int))) { rows = (int)jsonOptions["rows"]; }
                            }
                            Terminal = new MeshCentralTerminal(this, protocol, cols, rows);
                        }
                    }
                    else if (protocol == 2) // Desktop
                    {
                        if (((consent & 8) != 0) || parent.autoConnect) // Remote Desktop Consent Prompt
                        {
                            consentRequested = true;
                            setConsoleText("Waiting for user to grant access...", 1, null, 0);
                            parent.askForConsent(this, "User \"{0}\" is requesting remote desktop control of this computer. Click allow to grant access.", protocol, sessionLoggingUserName);
                            parent.Event(userid, "Requesting user consent for desktop session");
                        }
                        else
                        {
                            Desktop = MeshCentralDesktop.AddDesktopTunnel(this);
                        }
                    }
                    else if (protocol == 5) // Files
                    {
                        if (((consent & 32) != 0) || parent.autoConnect) // Remote Files Consent Prompt
                        {
                            consentRequested = true;
                            setConsoleText("Waiting for user to grant access...", 1, null, 0);
                            parent.askForConsent(this, "User \"{0}\" is requesting access to all files on this computer. Click allow to grant access.", protocol, sessionLoggingUserName);
                            parent.Event(userid, "Requesting user consent for files session");
                        }
                    }
                    else if (protocol == 10)
                    {
                        // File transfers are only allowed if the user has an active file session. If not, must disconnect since user consent may be required.
                        if ((sessionUserName == null) || (parent.doesUserHaveSession(sessionUserName, 5) == false)) { disconnect(); return; }

                        // Basic file transfer
                        if ((jsonOptions.ContainsKey("file")) && (jsonOptions["file"].GetType() == typeof(string)))
                        {
                            FileInfo f = new FileInfo((string)jsonOptions["file"]);
                            if (f.Exists)
                            {
                                try
                                {
                                    // Send the file
                                    WebSocket.onSendOk += WebSocket_onSendOk;
                                    fileSendTransfer = File.OpenRead(f.FullName);
                                    byte[] buf = new byte[65535];
                                    int len = fileSendTransfer.Read(buf, 0, buf.Length);
                                    if (len > 0) { WebSocket.SendBinary(buf, 0, len); } else { disconnect(); }
                                    WebSocket.SendString("{\"op\":\"ok\",\"size\":" + f.Length + "}");
                                    parent.WebSocket.SendBinary(UTF8Encoding.UTF8.GetBytes("{\"action\":\"log\",\"msgid\":106,\"msgArgs\":[\"" + escapeJsonString(f.FullName) + "\"," + f.Length + "],\"msg\":\"Download: " + escapeJsonString(f.FullName) + ", Size: " + f.Length + "\"" + extraLogStr + "}"));
                                    parent.Event(userid, string.Format("Downloaded file {0}", f.FullName));
                                }
                                catch (Exception)
                                {
                                    WebSocket.SendString("{\"op\":\"cancel\"}");
                                }
                            }
                            else
                            {
                                WebSocket.SendString("{\"op\":\"cancel\"}");
                            }
                        }
                    }

                    return;
                }
                else if (state == 2) // Running
                {
                    if (data.StartsWith("{")) { processServerJsonData(data); return; }
                }
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
            }
        }

        public void ConsentAccepted()
        {
            if (consentRequested == false) return;
            consentRequested = false;
            setConsoleText(null, 0, null, 0);
            if ((protocol == 1) || (protocol == 8) || (protocol == 9)) // Terminal
            {
                // Start remote terminal
                parent.Event(userid, "Terminal consent approved");
                int cols = 80;
                int rows = 25;
                if (jsonOptions != null)
                {
                    if (jsonOptions.ContainsKey("cols") && (jsonOptions["cols"].GetType() == typeof(int))) { cols = (int)jsonOptions["cols"]; }
                    if (jsonOptions.ContainsKey("rows") && (jsonOptions["rows"].GetType() == typeof(int))) { rows = (int)jsonOptions["rows"]; }
                }
                Terminal = new MeshCentralTerminal(this, protocol, cols, rows);
            }
            else if (protocol == 2) // Desktop
            {
                // Start remote desktop
                parent.Event(userid, "Desktop consent approved");
                Desktop = MeshCentralDesktop.AddDesktopTunnel(this);
            }
            else if (protocol == 5) // Files
            {
                // Start files
                parent.Event(userid, "Files consent approved");
                if (commandsOnHold != null) { foreach (string cmd in commandsOnHold) { ParseFilesCommand(cmd); } }
                commandsOnHold = null;
            }
        }

        public void ConsentRejected()
        {
            if (consentRequested == false) return;
            consentRequested = false;
            setConsoleText("Denied", 2, null, 0);
            disconnect();
            if ((protocol == 1) || (protocol == 8) || (protocol == 9)) { parent.Event(userid, "Terminal consent denied"); }
            else if (protocol == 2) { parent.Event(userid, "Desktop consent denied"); }
            else if (protocol == 5) { parent.Event(userid, "Files consent denied"); }
        }


        private void WebSocket_onSendOk(webSocketClient sender)
        {
            if (fileSendTransfer != null)
            {
                byte[] buf = new byte[65535];
                int len = fileSendTransfer.Read(buf, 0, buf.Length);
                if (len > 0) { WebSocket.SendBinary(buf, 0, len); } else { disconnect(); }
            }
        }

        public void setConsoleText(string msg, int msgid, string msgargs, int timeout)
        {
            string x = "{\"ctrlChannel\":\"102938\",\"type\":\"console\",\"msgid\":" + msgid;
            if (msg != null) { x += ",\"msg\":\"" + escapeJsonString(msg) + "\""; } else { x += ",\"msg\":null"; }
            if (msgargs != null) { x += ",\"msgargs\":" + msgargs; }
            if (timeout != 0) { x += ",\"timeout\":" + timeout; }
            x += "}";
            WebSocket.SendString(x);
        }

        public void clearConsoleText()
        {
            WebSocket.SendString("{\"ctrlChannel\":\"102938\",\"type\":\"console\",\"msgid\":0}");
        }

        private void WebSocket_onBinaryData(webSocketClient sender, byte[] data, int off, int len, int orglen)
        {
            try
            {
                if (state == 1)
                {
                    // Process connection options
                    string jsonStr = UTF8Encoding.UTF8.GetString(data, off, len);
                    WebSocket_onStringData(sender, jsonStr, jsonStr.Length);
                    return;
                }
                if (state != 2) return;
                if ((protocol == 1) || (protocol == 8) || (protocol == 9)) // Terminal
                {
                    if (Terminal != null) { Terminal.onBinaryData(data, off, len); }
                }
                else if (protocol == 2) // Desktop
                {
                    if (Desktop != null) { Desktop.onBinaryData(this, data, off, len); }
                }
                else if (protocol == 5) // Files
                {
                    if (data[off] == 123) { ParseFilesCommand(UTF8Encoding.UTF8.GetString(data, off, len)); }
                    else if (fileRecvTransfer != null)
                    {
                        bool err = false;
                        if (data[off] == 0)
                        {
                            try { fileRecvTransfer.Write(data, off + 1, len - 1); } catch (Exception) { err = true; }
                        }
                        else
                        {
                            try { fileRecvTransfer.Write(data, off, len); } catch (Exception) { err = true; }
                        }
                        if (err)
                        {
                            try { fileRecvTransfer.Close(); fileRecvTransfer = null; } catch (Exception) { }
                            fileRecvTransfer = null;
                            WebSocket.SendBinary(UTF8Encoding.UTF8.GetBytes("{\"action\":\"uploaderror\"}"));
                        }
                        else
                        {
                            WebSocket.SendBinary(UTF8Encoding.UTF8.GetBytes("{\"action\":\"uploadack\",\"reqid\":" + fileRecvId + "}"));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
            }
        }

        private string escapeJsonString(string str)
        {
            var r = "";
            foreach (char c in str)
            {
                if (c == '\r') { r += "\\r"; }
                else if (c == '\n') { r += "\\n"; }
                else if (c == '\\') { r += "\\\\"; }
                else if (c == '\"') { r += "\\\""; }
                else { r += c; }
            }
            return r;
        }

        private void ParseFilesCommand(string data)
        {
            // If we are asking for consent, hold file commands.
            if (consentRequested) {
                if (commandsOnHold == null) { commandsOnHold = new ArrayList(); }
                commandsOnHold.Add(data);
                return;
            }

            // Parse the received JSON
            Dictionary<string, object> jsonCommand = new Dictionary<string, object>();
            try { jsonCommand = JSON.Deserialize<Dictionary<string, object>>(data); } catch (Exception) { return; }
            if (jsonCommand == null || !jsonCommand.ContainsKey("action") || jsonCommand["action"].GetType() != typeof(string)) return;

            int reqid = 0;
            if ((jsonCommand.ContainsKey("reqid")) && (jsonCommand["reqid"].GetType() == typeof(System.Int32))) { reqid = (int)jsonCommand["reqid"]; }

            // Handle the JSON command
            string action = (string)jsonCommand["action"];
            string response = null;
            switch (action)
            {
                case "ls": // List folder
                    {
                        string path = "";
                        if (jsonCommand["path"].GetType() == typeof(string)) { path = (string)jsonCommand["path"]; }
                        if (path == "")
                        {
                            try
                            {
                                // Enumerate drives
                                response = "{\"path\":\"" + path + "\"";
                                if (reqid != 0) { response += ",\"reqid\":" + reqid; }
                                response += ",\"dir\":[";
                                bool firstDrive = true;
                                foreach (var drive in DriveInfo.GetDrives())
                                {
                                    if (drive.IsReady == false) continue;
                                    if (firstDrive) { firstDrive = false; } else { response += ","; }
                                    response += "{\"n\":\"" + escapeJsonString(drive.Name) + "\",\"t\":1}";
                                }
                                response += "]}";
                            }
                            catch (Exception) { }
                        } else {
                            try
                            {
                                // Enumerate folders
                                response = "{\"path\":\"" + path + "\"";
                                if (reqid != 0) { response += ",\"reqid\":" + reqid; }
                                response += ",\"dir\":[";
                                bool firstFile = true;
                                if (path.EndsWith("\\") == false) { path += "\\";  }
                                foreach (string folder in Directory.EnumerateDirectories(path))
                                {
                                    DirectoryInfo d = new DirectoryInfo(folder);
                                    string t = d.CreationTime.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");
                                    if (firstFile) { firstFile = false; } else { response += ","; }
                                    response += "{\"n\":\"" + escapeJsonString(d.Name) + "\",\"t\":2,\"d\":\"" + t + "\"}";
                                }
                                foreach (string folder in Directory.EnumerateFiles(path))
                                {
                                    FileInfo f = new FileInfo(folder);
                                    //if (f.Attributes.HasFlag(FileAttributes.Hidden)) continue;
                                    string t = f.CreationTime.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");
                                    if (firstFile) { firstFile = false; } else { response += ","; }
                                    response += "{\"n\":\"" + escapeJsonString(f.Name) + "\",\"t\":3,\"s\":" + f.Length + ",\"d\":\"" + t + "\"}";
                                }
                                response += "]}";
                            }
                            catch (Exception) { }
                        }
                        break;
                    }
                case "mkdir": // Create folder
                    {
                        string path = "";
                        if ((jsonCommand.ContainsKey("path")) && (jsonCommand["path"].GetType() == typeof(string))) { path = (string)jsonCommand["path"]; }
                        if (path != "") {
                            try
                            {
                                path = path.Replace("/", "\\");
                                Directory.CreateDirectory(path);
                                parent.WebSocket.SendBinary(UTF8Encoding.UTF8.GetBytes("{\"action\":\"log\",\"msgid\":44,\"msgArgs\":[\"" + escapeJsonString(path) + "\"],\"msg\":\"Create folder: " + escapeJsonString(path) + "\"" + extraLogStr + "}"));
                                parent.Event(userid, string.Format("Created folder: {0}", path));
                            }
                            catch (Exception) { }
                        }
                        break;
                    }
                case "rm": // Remove folders or files
                    {
                        bool rec = false;
                        string path = "";
                        ArrayList delfiles = null;
                        if (jsonCommand.ContainsKey("path") && (jsonCommand["path"].GetType() == typeof(string))) { path = (string)jsonCommand["path"]; }
                        if (jsonCommand.ContainsKey("rec") && (jsonCommand["rec"].GetType() == typeof(bool))) { rec = (bool)jsonCommand["rec"]; }
                        if (jsonCommand.ContainsKey("delfiles") && (jsonCommand["delfiles"].GetType() == typeof(ArrayList))) { delfiles = (ArrayList)jsonCommand["delfiles"]; }
                        if ((path != "") && (delfiles != null))
                        {
                            path = path.Replace("/","\\");
                            foreach (object o in delfiles)
                            {
                                if (o.GetType() != typeof(string)) continue;
                                string delfile = (string)o;
                                delfile = Path.Combine(path, delfile);
                                parent.Event(userid, string.Format("Deleted file: {0}", delfile));
                                bool ok = false;
                                try { File.Delete(delfile); ok = true; } catch (Exception) { }
                                if (ok == false) { try { Directory.Delete(delfile, rec); ok = true; } catch (Exception) { } }
                                if (ok) {
                                    if (rec) { parent.WebSocket.SendBinary(UTF8Encoding.UTF8.GetBytes("{\"action\":\"log\",\"msgid\":46,\"msgArgs\":[\"" + escapeJsonString(delfile) + "\",\"1\"],\"msg\":\"Delete recursive: \\\"" + escapeJsonString(delfile) + "\\\", 1 element(s) removed\"" + extraLogStr + "}")); }
                                    else { parent.WebSocket.SendBinary(UTF8Encoding.UTF8.GetBytes("{\"action\":\"log\",\"msgid\":47,\"msgArgs\":[\"" + escapeJsonString(delfile) + "\",\"1\"],\"msg\":\"Delete: \\\"" + escapeJsonString(delfile) + "\\\", 1 element(s) removed\"" + extraLogStr + "}")); }
                                }
                            }
                        }
                        break;
                    }
                case "rename": // File or folder rename
                    {
                        string path = null;
                        string oldname = null;
                        string newname = null;
                        if (jsonCommand.ContainsKey("path") && (jsonCommand["path"].GetType() == typeof(string))) { path = (string)jsonCommand["path"]; }
                        if (jsonCommand.ContainsKey("oldname") && (jsonCommand["oldname"].GetType() == typeof(string))) { oldname = (string)jsonCommand["oldname"]; }
                        if (jsonCommand.ContainsKey("newname") && (jsonCommand["newname"].GetType() == typeof(string))) { newname = (string)jsonCommand["newname"]; }
                        if ((path == null) || (oldname == null) || (newname == null)) break;
                        path = path.Replace("/", "\\");
                        bool ok = false;
                        string xoldname = Path.Combine(path, oldname);
                        string xnewname = Path.Combine(path, newname);
                        parent.Event(userid, string.Format("Renamed file: {0} to {1}", xoldname, newname));
                        try { File.Move(xoldname, xnewname); ok = true; } catch (Exception) { }
                        if (ok == false) { try { DirectoryInfo d = new DirectoryInfo(xoldname); d.MoveTo(xnewname); ok = true; } catch (Exception) { } }
                        if (ok) { parent.WebSocket.SendBinary(UTF8Encoding.UTF8.GetBytes("{\"action\":\"log\",\"msgid\":48,\"msgArgs\":[\"" + escapeJsonString(xoldname) + "\",\"" + escapeJsonString(newname) + "\"],\"msg\":\"Rename: " + escapeJsonString(xoldname) + " to " + escapeJsonString(newname) + "\"" + extraLogStr + "}")); }
                        break;
                    }
                case "copy": // Files copy
                    {
                        ArrayList names = null;
                        string scpath = null;
                        string dspath = null;
                        if (jsonCommand.ContainsKey("names") && (jsonCommand["names"].GetType() == typeof(ArrayList))) { names = (ArrayList)jsonCommand["names"]; }
                        if (jsonCommand.ContainsKey("scpath") && (jsonCommand["scpath"].GetType() == typeof(string))) { scpath = (string)jsonCommand["scpath"]; }
                        if (jsonCommand.ContainsKey("dspath") && (jsonCommand["dspath"].GetType() == typeof(string))) { dspath = (string)jsonCommand["dspath"]; }
                        if ((names == null) || (scpath == null) || (dspath == null)) break;
                        scpath = scpath.Replace("/", "\\");
                        dspath = dspath.Replace("/", "\\");
                        foreach (string name in names)
                        {
                            try {
                                string sc = Path.Combine(scpath, name);
                                string dc = Path.Combine(dspath, name);
                                parent.Event(userid, string.Format("Copied file: {0} to {1}", sc, dc));
                                File.Copy(sc, dc);
                                parent.WebSocket.SendBinary(UTF8Encoding.UTF8.GetBytes("{\"action\":\"log\",\"msgid\":51,\"msgArgs\":[\"" + escapeJsonString(sc) + "\",\"" + escapeJsonString(dc) + "\"],\"msg\":\"Copy: " + escapeJsonString(sc) + " to " + escapeJsonString(dc) + "\"" + extraLogStr + "}"));
                            } catch (Exception) { }
                        }
                        break;
                    }
                case "move": // Files move
                    {
                        ArrayList names = null;
                        string scpath = null;
                        string dspath = null;
                        if (jsonCommand.ContainsKey("names") && (jsonCommand["names"].GetType() == typeof(ArrayList))) { names = (ArrayList)jsonCommand["names"]; }
                        if (jsonCommand.ContainsKey("scpath") && (jsonCommand["scpath"].GetType() == typeof(string))) { scpath = (string)jsonCommand["scpath"]; }
                        if (jsonCommand.ContainsKey("dspath") && (jsonCommand["dspath"].GetType() == typeof(string))) { dspath = (string)jsonCommand["dspath"]; }
                        if ((names == null) || (scpath == null) || (dspath == null)) break;
                        scpath = scpath.Replace("/", "\\");
                        dspath = dspath.Replace("/", "\\");
                        foreach (string name in names)
                        {
                            try {
                                string sc = Path.Combine(scpath, name);
                                string dc = Path.Combine(dspath, name);
                                parent.Event(userid, string.Format("Moved file: {0} to {1}", sc, dc));
                                File.Move(sc, dc);
                                parent.WebSocket.SendBinary(UTF8Encoding.UTF8.GetBytes("{\"action\":\"log\",\"msgid\":52,\"msgArgs\":[\"" + escapeJsonString(sc) + "\",\"" + escapeJsonString(dc) + "\"],\"msg\":\"Move: " + escapeJsonString(sc) + " to " + escapeJsonString(dc) + "\"" + extraLogStr + "}"));
                            } catch (Exception) { }
                        }
                        break;
                    }
                case "zip": // Files zip
                    {
                        ArrayList files = null;
                        string path = null;
                        string output = null;
                        byte[] buf = new byte[65535];
                        if (jsonCommand.ContainsKey("files") && (jsonCommand["files"].GetType() == typeof(ArrayList))) { files = (ArrayList)jsonCommand["files"]; }
                        if (jsonCommand.ContainsKey("path") && (jsonCommand["path"].GetType() == typeof(string))) { path = (string)jsonCommand["path"]; }
                        if (jsonCommand.ContainsKey("output") && (jsonCommand["output"].GetType() == typeof(string))) { output = (string)jsonCommand["output"]; }
                        if ((files == null) || (path == null) || (output == null)) break;
                        path = path.Replace("/", "\\");
                        parent.Event(userid, string.Format("Zippped files from {0} to {1}", path, output));

                        using (FileStream zipToOpen = new FileStream(Path.Combine(path, output), FileMode.Create))
                        {
                            using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Create))
                            {
                                foreach (string file in files)
                                {
                                    ZipArchiveEntry archiveEntry = archive.CreateEntry(file);
                                    using (Stream writer = archiveEntry.Open())
                                    {
                                        try
                                        {
                                            using (var fileStream = new FileStream(Path.Combine(path, file), FileMode.Open))
                                            {
                                                int len = 0;
                                                do { len = fileStream.Read(buf, 0, buf.Length); if (len > 0) { writer.Write(buf, 0, len); } } while (len > 0);
                                            }
                                        }
                                        catch (Exception) { }
                                    }
                                }
                            }
                        }
                        // We are done, tell the browser to refresh the folder
                        WebSocket.SendBinary(UTF8Encoding.UTF8.GetBytes("{\"action\":\"refresh\"}"));
                        break;
                    }
                case "upload": // Receive a file from the browser
                    {
                        string path = null;
                        string name = null;
                        int size = 0;
                        bool append = false;
                        if (jsonCommand.ContainsKey("path") && (jsonCommand["path"].GetType() == typeof(string))) { path = (string)jsonCommand["path"]; }
                        if (jsonCommand.ContainsKey("name") && (jsonCommand["name"].GetType() == typeof(string))) { name = (string)jsonCommand["name"]; }
                        if (jsonCommand.ContainsKey("size") && (jsonCommand["size"].GetType() == typeof(System.Int32))) { size = (int)jsonCommand["size"]; }
                        if (jsonCommand.ContainsKey("append") && (jsonCommand["append"].GetType() == typeof(bool))) { append = (bool)jsonCommand["append"]; }
                        if ((path == null) || (name == null) || (size <= 0) || !Directory.Exists(path)) { disconnect(); return; }

                        string fullname = Path.Combine(path, name).Replace("/", "\\");
                        parent.Event(userid, string.Format("Uploading file {0} of size {1}", fullname, size));
                        try { fileRecvTransfer = File.Open(fullname, append ? FileMode.Append : FileMode.Create); } catch (Exception) { WebSocket.SendBinary(UTF8Encoding.UTF8.GetBytes("{\"action\":\"uploaderror\",\"reqid\":" + reqid + "}")); break; }
                        fileRecvId = reqid;
                        fileRecvTransferPath = fullname;
                        fileRecvTransferSize = size;
                        WebSocket.SendBinary(UTF8Encoding.UTF8.GetBytes("{\"action\":\"uploadstart\",\"reqid\":" + fileRecvId + "}"));
                        break;
                    }
                case "uploaddone": // Received a file from the browser is done
                    {
                        if ((fileRecvId == reqid) && (fileRecvTransfer != null))
                        {
                            WebSocket.SendBinary(UTF8Encoding.UTF8.GetBytes("{\"action\":\"uploaddone\",\"reqid\":" + fileRecvId + "}"));
                            try { fileRecvTransfer.Close(); } catch (Exception) { }
                            fileRecvTransfer = null;
                            fileRecvId = 0;
                            parent.WebSocket.SendBinary(UTF8Encoding.UTF8.GetBytes("{\"action\":\"log\",\"msgid\":105,\"msgArgs\":[\"" + escapeJsonString(fileRecvTransferPath) + "\"," + fileRecvTransferSize + "],\"msg\":\"Download: " + escapeJsonString(fileRecvTransferPath) + ", Size: " + fileRecvTransferSize + "\"" + extraLogStr + "}"));
                        }
                        break;
                    }
                case "uploadhash": // Hash a file
                    {
                        string path = null;
                        string name = null;
                        string tag = "null";
                        if (jsonCommand.ContainsKey("path") && (jsonCommand["path"].GetType() == typeof(string))) { path = (string)jsonCommand["path"]; }
                        if (jsonCommand.ContainsKey("name") && (jsonCommand["name"].GetType() == typeof(string))) { name = (string)jsonCommand["name"]; }
                        if ((path == null) || (name == null) || !Directory.Exists(path)) { disconnect(); return; }
                        if (jsonCommand.ContainsKey("tag"))
                        {
                            if (jsonCommand["tag"].GetType() == typeof(string)) { tag = "\"" + (string)jsonCommand["tag"] + "\""; }
                            if (jsonCommand["tag"].GetType() == typeof(Dictionary<string, object>)) { tag = new JavaScriptSerializer().Serialize(jsonCommand["tag"]); }
                        }
                        string hash = "null";
                        FileInfo fInfo = new FileInfo(Path.Combine(path, name));
                        if (fInfo.Exists == true)
                        {
                            using (SHA384 hasher = SHA384.Create())
                            {
                                try
                                {
                                    FileStream fileStream = fInfo.Open(FileMode.Open);
                                    fileStream.Position = 0;
                                    byte[] hashValue = hasher.ComputeHash(fileStream);
                                    hash = "\"" + BitConverter.ToString(hashValue).Replace("-", string.Empty) + "\"";
                                    fileStream.Close();
                                }
                                catch (Exception) { }
                            }
                        }
                        WebSocket.SendBinary(UTF8Encoding.UTF8.GetBytes("{\"action\":\"uploadhash\",\"reqid\":" + reqid + ",\"path\":\"" + escapeJsonString(path) + "\",\"name\":\"" + escapeJsonString(name) + "\",\"tag\":" + tag + ",\"hash\":" + hash + "}"));
                        break;
                    }
                case "download": // Sent a file to the browser
                    {
                        string sub = null;
                        string id = null;
                        string path = null;
                        if (jsonCommand.ContainsKey("sub") && (jsonCommand["sub"].GetType() == typeof(string))) { sub = (string)jsonCommand["sub"]; }
                        if (jsonCommand.ContainsKey("id")) { id = jsonCommand["id"].ToString(); }
                        if (jsonCommand.ContainsKey("path") && (jsonCommand["path"].GetType() == typeof(string))) { path = (string)jsonCommand["path"]; }
                        if ((sub == null) || (id == null)) return;

                        int sendNextBlock = 0;
                        if (sub == "start")
                        {
                            // Setup the download
                            //MeshServerLogEx((cmd.ask == 'coredump') ? 104 : 49, [cmd.path], 'Download: \"' + cmd.path + '\"', this.httprequest);
                            if ((path == null) || (fileSendTransfer != null)) {
                                WebSocket.SendBinary(UTF8Encoding.UTF8.GetBytes("{\"action\":\"download\",\"sub\":\"cancel\",\"id\":" + fileSendTransferId + "}"));
                                if (fileSendTransfer != null) { fileSendTransfer.Close(); fileSendTransfer = null; fileSendTransferId = null; }
                            }
                            try { fileSendTransfer = File.OpenRead(path); } catch (Exception) { }
                            if (fileSendTransfer == null) {
                                WebSocket.SendBinary(UTF8Encoding.UTF8.GetBytes("{\"action\":\"download\",\"sub\":\"cancel\",\"id\":" + id + "}"));
                            } else {
                                fileSendTransferId = id;
                                WebSocket.SendBinary(UTF8Encoding.UTF8.GetBytes("{\"action\":\"download\",\"sub\":\"start\",\"id\":" + fileSendTransferId + "}"));
                            }
                        }
                        else if ((fileSendTransfer != null) && (id == fileSendTransferId))
                        {
                            // Download commands
                            if (sub == "startack") {
                                sendNextBlock = 8;
                                if (jsonCommand.ContainsKey("ack") && (jsonCommand["ack"].GetType() == typeof(System.Int32))) { sendNextBlock = (int)jsonCommand["ack"]; }
                            } else if (sub == "stop") {
                                try { fileSendTransfer.Close(); } catch (Exception) { }
                            fileSendTransfer = null;
                                fileSendTransferId = null;
                            } else if (sub == "ack") { sendNextBlock = 1; }
                        }

                        // Send the next download block(s)
                        while (sendNextBlock > 0)
                        {
                            sendNextBlock--;
                            byte[] buf = new byte[16384];
                            buf[0] = 1;
                            int len = fileSendTransfer.Read(buf, 4, buf.Length - 4);
                            if (len < buf.Length - 4)
                            {
                                buf[3] = 1;
                                try { fileSendTransfer.Close(); } catch (Exception) { }
                                fileSendTransfer = null;
                                fileSendTransferId = null;
                                sendNextBlock = 0;
                            }
                            WebSocket.SendBinary(buf, 0, len + 4);
                        }

                        break;
                    }
                default: { break; }
            }
            if (response != null) { WebSocket.SendBinary(UTF8Encoding.UTF8.GetBytes(response)); }
        }

    }
}
