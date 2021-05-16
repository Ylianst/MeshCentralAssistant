using System;
using System.IO;
using System.Net;
using System.Text;
using System.Linq;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Web.Script.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.Net.NetworkInformation;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;

namespace MeshAssistant
{
    public class MeshCentralAgent
    {
        private MainForm parent;
        public bool debug = false;
        public int state = 0;
        public string HelpRequest = null;
        private byte[] MeshId = null;
        private string ServerId = null;
        private byte[] ServerTlsHash = null;
        private string ServerTlsHashStr = null;
        private Uri ServerUrl = null;
        private byte[] Nonce = null;
        private byte[] ServerNonce = null;
        private X509Certificate2 agentCert = null;
        private JavaScriptSerializer JSON = new JavaScriptSerializer();
        public webSocketClient WebSocket = null;
        private int ConnectionState = 0;
        private List<MeshCentralTunnel> tunnels = new List<MeshCentralTunnel>(); // List of active tunnels
        public Dictionary<string, Image> userimages = new Dictionary<string, Image>(); // UserID --> Image
        public Dictionary<string, string> userrealname = new Dictionary<string, string>(); // UserID --> Realname

        // Sessions
        public Dictionary<string, object> DesktopSessions = null;
        public Dictionary<string, object> TerminalSessions = null;
        public Dictionary<string, object> FilesSessions = null;
        public Dictionary<string, object> TcpSessions = null;
        public Dictionary<string, object> UdpSessions = null;
        public Dictionary<string, object> MessagesSessions = null;

        public delegate void onUserInfoChangeHandler(string userid, int change); // Change: 1 = Image, 2 = Realname
        public event onUserInfoChangeHandler onUserInfoChange;

        public delegate void onSessionChangedHandler();
        public event onSessionChangedHandler onSessionChanged;
        public void fireSessionChanged(int protocol) {
            if (onSessionChanged != null) { onSessionChanged(); }
            if (WebSocket == null) return;

            // Update the server with latest session
            if (protocol == 2)
            {
                string r = "{\"action\":\"sessions\",\"type\":\"kvm\",\"value\":{";
                if (DesktopSessions != null)
                {
                    bool first = true;
                    foreach (string userid in DesktopSessions.Keys)
                    {
                        if (first) { first = false; } else { r += ","; }
                        r += "\"" + userid + "\":" + (int)DesktopSessions[userid];
                    }
                }
                r += "}}";
                if (WebSocket != null) { WebSocket.SendBinary(UTF8Encoding.UTF8.GetBytes(r)); }
            }
            else if (protocol == 5)
            {
                string r = "{\"action\":\"sessions\",\"type\":\"files\",\"value\":{";
                if (FilesSessions != null) {
                    bool first = true;
                    foreach (string userid in FilesSessions.Keys)
                    {
                        if (first) { first = false; } else { r += ","; }
                        r += "\"" + userid + "\":" + (int)FilesSessions[userid];
                    }
                }
                r += "}}";
                if (WebSocket != null) { WebSocket.SendBinary(UTF8Encoding.UTF8.GetBytes(r)); }
            }
        }

        public MeshCentralAgent(MainForm parent)
        {
            this.parent = parent;

            // Load the agent certificate and private key
            agentCert = LoadAgentCertificate();

            // Load the MSH file
            Dictionary<string, string> msh = LoadMshFile();

            // Get the MeshId, ServerId, and ServerUrl
            if (msh.ContainsKey("MeshID")) { string m = msh["MeshID"]; if (m.StartsWith("0x")) { m = m.Substring(2); } MeshId = StringToByteArray(m); }
            if (msh.ContainsKey("ServerID")) { ServerId = msh["ServerID"]; }
            if (msh.ContainsKey("MeshServer")) { try { ServerUrl = new Uri(msh["MeshServer"]); } catch (Exception) { } }
        }

        public static bool checkMshFile()
        {
            // Load the MSH file
            Dictionary<string, string> msh = LoadMshFile();
            if (msh == null) return false;
            if (!msh.ContainsKey("MeshID")) return false;
            if (!msh.ContainsKey("ServerID")) return false;
            if (!msh.ContainsKey("MeshServer")) return false;
            return true;
        }

        private static X509Certificate2 LoadAgentCertificate()
        {
            X509Certificate2 cert = null;
            if (File.Exists("agentcert.pfx")) { try { cert = new X509Certificate2("agentcert.pfx", "dummy"); } catch (Exception) { } }
            if (cert != null) return cert;
            RSA rsa = RSA.Create(3072); // Generate asymmetric RSA key pair
            CertificateRequest req = new CertificateRequest("cn=MeshAgent", rsa, HashAlgorithmName.SHA384, RSASignaturePadding.Pkcs1);
            cert = req.CreateSelfSigned(DateTimeOffset.Now.AddDays(-10), DateTimeOffset.Now.AddYears(20));
            File.WriteAllBytes("agentcert.pfx", cert.Export(X509ContentType.Pkcs12, "dummy"));
            return cert;
        }

        private static Dictionary<string, string> LoadMshFile()
        {
            if (!File.Exists("meshagent.msh")) return null;
            string[] lines = null;
            try { lines = File.ReadAllLines("meshagent.msh"); } catch (Exception) { }
            if (lines == null) return null;
            Dictionary<string, string> vals = new Dictionary<string, string>();
            foreach (string line in lines) { int i = line.IndexOf('='); if (i > 0) { vals.Add(line.Substring(0, i), line.Substring(i + 1)); } }
            return vals;
        }

        public delegate void onNotifyHandler(string userid, string title, string msg);
        public event onNotifyHandler onNotify;
        public void notify(string userid, string title, string msg) { if (onNotify != null) { onNotify(userid, title, msg); } }

        public delegate void onDebugHandler(string msg);
        public event onDebugHandler onDebug;
        public void debugMsg(string msg) { if (onDebug != null) { onDebug(msg); } }

        public delegate void onStateChangedHandler(int state);
        public event onStateChangedHandler onStateChanged;
        public void changeState(int newState) { if (state != newState) { state = newState; if (onStateChanged != null) { onStateChanged(state); } } }

        public byte[] GenerateCryptographicRandom(int len)
        {
            RNGCryptoServiceProvider rngCryptoServiceProvider = new RNGCryptoServiceProvider();
            byte[] randomBytes = new byte[len];
            rngCryptoServiceProvider.GetBytes(randomBytes);
            return randomBytes;
        }

        public static byte[] StringToByteArray(string hex) { return Enumerable.Range(0, hex.Length).Where(x => x % 2 == 0).Select(x => Convert.ToByte(hex.Substring(x, 2), 16)).ToArray(); }

        public bool connect()
        {
            if ((MeshId == null) || (MeshId.Length != 48)) return false;
            if (WebSocket != null) return false;
            if (debug) { try { File.AppendAllText("debug.log", "Connect to " + ServerUrl + "\r\n"); } catch (Exception) { } }
            userimages = new Dictionary<string, Image>(); // UserID --> Image
            userrealname = new Dictionary<string, string>(); // UserID --> Realname
            WebSocket = new webSocketClient();
            WebSocket.pongTimeSeconds = 120; // Send a websocket pong every 2 minutes.
            WebSocket.xdebug = debug;
            WebSocket.onStateChanged += WebSocket_onStateChanged;
            WebSocket.onBinaryData += WebSocket_onBinaryData;
            ConnectionState = 1;
            return WebSocket.Start(ServerUrl, null);
        }

        private void WebSocket_onStateChanged(webSocketClient sender, webSocketClient.ConnectionStates state)
        {
            if (state == webSocketClient.ConnectionStates.Disconnected)
            {
                disconnect();
            }
            else if (state == webSocketClient.ConnectionStates.Connecting)
            {
                changeState(1);
            }
            else if (state == webSocketClient.ConnectionStates.Connected)
            {
                changeState(2);
                ConnectionState |= 2;

                // Compute the remote certificate SHA384 hash
                using (SHA384 sha384Hash = SHA384.Create())
                {
                    byte[] bytes = ServerTlsHash = sha384Hash.ComputeHash(WebSocket.RemoteCertificate.GetRawCertData());
                    StringBuilder builder = new StringBuilder();
                    for (int i = 0; i < bytes.Length; i++) { builder.Append(bytes[i].ToString("x2")); }
                    ServerTlsHashStr = builder.ToString().ToUpper();
                }
                //Debug("Websocket TLS hash: " + ServerTlsHash);

                // Send command 1, hash + nonce
                Nonce = GenerateCryptographicRandom(48);
                BinaryWriter bw = new BinaryWriter(new MemoryStream());
                bw.Write(Convert.ToInt16(IPAddress.HostToNetworkOrder((short)1)));
                bw.Write(ServerTlsHash);
                bw.Write(Nonce);
                WebSocket.SendBinary(((MemoryStream)bw.BaseStream).ToArray());
            }
        }

        public void disconnect()
        {
            if (WebSocket == null) return;
            if (debug) { try { File.AppendAllText("debug.log", "Disconnect\r\n"); } catch (Exception) { } }
            WebSocket.Dispose();
            WebSocket = null;
            ConnectionState = 0;
            changeState(0);
            foreach (MeshCentralTunnel tunnel in tunnels) { tunnel.disconnect(); }
        }

        private void serverConnected()
        {
            debugMsg("Server Connected");

            // Update network information
            sendNetworkInfo();

            // Send help session
            if (HelpRequest != null)
            {
                string userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                sendSessionUpdate("help", "{\"" + escapeJsonString(userName) + "\":\"" + escapeJsonString(HelpRequest) + "\"}");
                sendHelpEventLog(userName, HelpRequest);
            }
        }

        private void sendSessionUpdate(string type, string value)
        {
            WebSocket.SendBinary(UTF8Encoding.UTF8.GetBytes("{\"action\":\"sessions\",\"type\":\"" + type + "\",\"value\":" + value + "}"));
        }

        private void sendConsoleEventLog(string cmd)
        {
            WebSocket.SendBinary(UTF8Encoding.UTF8.GetBytes("{\"action\":\"log\",\"msgid\":17,\"msgArgs\":[\"" + escapeJsonString(cmd) + "\"],\"msg\":\"Processing console command: " + escapeJsonString(cmd) + "\"}"));
        }

        private void sendOpenUrlEventLog(string url)
        {
            WebSocket.SendBinary(UTF8Encoding.UTF8.GetBytes("{\"action\":\"log\",\"msgid\":20,\"msgArgs\":[\"" + escapeJsonString(url) + "\"],\"msg\":\"Opening: " + escapeJsonString(url) + "\"}"));
        }

        private void sendHelpEventLog(string username, string helpstring)
        {
            WebSocket.SendBinary(UTF8Encoding.UTF8.GetBytes("{\"action\":\"log\",\"msgid\":98,\"msgArgs\":[\"" + escapeJsonString(username) + "\",\"" + escapeJsonString(helpstring) + "\"],\"msg\":\"Help Requested, user: " + escapeJsonString(username) + ", details: " + escapeJsonString(helpstring) + "\"}"));
        }

        private string[] parseArgString(string cmd) {
            bool q = false;
            string acc = "";
            List<string> r = new List<string>();
            foreach (var c in cmd) { if ((c == ' ') && (q == false)) { if (acc.Length > 0) { r.Add(acc); acc = ""; } } else { if (c == '"') { q = !q; } else { acc += c; } } }
            if (acc.Length > 0) { r.Add(acc); }
            return r.ToArray();
        }

        private string fixMacAddress(string mac)
        {
            if (mac == "") return "00:00:00:00:00:00";
            if (mac.Length == 12) return mac.Substring(0, 2) + ":" + mac.Substring(2, 2) + ":" + mac.Substring(4, 2) + ":" + mac.Substring(6, 2) + ":" + mac.Substring(8, 2) + ":" + mac.Substring(10, 2);
            return mac;
        }

        private string escapeJsonString(string str) {
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

        private void sendNetworkInfo()
        {
            WebSocket.SendBinary(UTF8Encoding.UTF8.GetBytes("{\"action\":\"netinfo\",\"netif2\":" + getNetworkInfo() + "}"));
        }

        private void getUserImage(string userid)
        {
            WebSocket.SendBinary(UTF8Encoding.UTF8.GetBytes("{\"action\":\"getUserImage\",\"userid\":\"" + escapeJsonString(userid) + "\"}"));
        }

        private string getNetworkInfo()
        {
            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
            string response = "{\r\n";
            bool firstItem1 = true;
            foreach (NetworkInterface adapter in adapters)
            {
                if (firstItem1 == false) { response += ",\r\n"; }
                response += "  \"" + escapeJsonString(adapter.Name) + "\": [\r\n";
                IPInterfaceProperties properties = adapter.GetIPProperties();
                int i = 0;
                bool firstItem2 = true;
                foreach (UnicastIPAddressInformation addr in properties.UnicastAddresses)
                {
                    if (firstItem2 == false) { response += ",\r\n"; }
                    response += "    {\r\n";
                    response += "      \"address\": \"" + addr.Address.ToString() + "\",\r\n";
                    response += "      \"fqdn\": \"" + properties.DnsSuffix + "\",\r\n";
                    if (addr.Address.AddressFamily == AddressFamily.InterNetwork) { response += "      \"family\": \"IPv4\",\r\n"; }
                    if (addr.Address.AddressFamily == AddressFamily.InterNetworkV6) { response += "      \"family\": \"IPv6\",\r\n"; }
                    response += "      \"mac\": \"" + fixMacAddress(adapter.GetPhysicalAddress().ToString()) + "\",\r\n";
                    response += "      \"index\": \"" + adapter.GetIPProperties().GetIPv6Properties().Index + "\",\r\n";
                    response += "      \"type\": \"" + adapter.NetworkInterfaceType.ToString().ToLower() + "\",\r\n";
                    response += "      \"status\": \"" + adapter.OperationalStatus.ToString().ToLower() + "\"\r\n";
                    response += "    }";
                    firstItem2 = false;
                    i++;
                }
                if (!firstItem2) { response += "\r\n"; }
                response += "  ]";
                firstItem1 = false;
            }
            if (!firstItem1) { response += "\r\n"; }
            response += "}";
            return response;
        }

        public void processConsoleCommand(string rawcmd, string sessionid, string userid)
        {
            string response = null;
            string[] cmd = parseArgString(rawcmd);
            if (cmd.Length == 0) return;
            sendConsoleEventLog(rawcmd);

            switch (cmd[0])
            {
                case "help": {
                        response = "Available commands: \r\n" + "args, help, netinfo, notify, openurl" + ".";
                        break;
                    }
                case "args":
                    {
                        response = "Command arguments: ";
                        foreach (var arg in cmd) { response += ("[" + arg + "]"); }
                        response += "\r\n";
                        break;
                    }
                case "netinfo":
                    {
                        response = getNetworkInfo();
                        break;
                    }
                case "openurl":
                    {
                        if (cmd.Length != 2) { response = "Usage: openurl [url]"; } else { Process.Start(cmd[1]); sendOpenUrlEventLog(cmd[1]); response = "Ok"; }
                        break;
                    }
                case "notify":
                    {
                        if (cmd.Length != 3) { response = "Usage: notify [title] [message]"; } else { notify(userid, cmd[1], cmd[2]); response = "Ok"; }
                        break;
                    }
                default:
                    {
                        response = "Unknown command \"" + cmd[0] + "\", type \"help\" for list of avaialble commands.";
                        break;
                    }
            }

            if (response != null) {
                WebSocket.SendBinary(UTF8Encoding.UTF8.GetBytes("{\"action\":\"msg\",\"type\":\"console\",\"value\":\"" + escapeJsonString(response) + "\",\"sessionid\":\"" + sessionid + "\"}"));
            }
        }

        public void processServerJsonData(string data)
        {
            debugMsg("JSON: " + data);

            // Parse the received JSON
            Dictionary<string, object> jsonAction = new Dictionary<string, object>();
            try { jsonAction = JSON.Deserialize<Dictionary<string, object>>(data); } catch (Exception) { return; }
            if (jsonAction == null || jsonAction["action"].GetType() != typeof(string)) return;
            string userid = null;
            if (jsonAction.ContainsKey("userid") && (jsonAction["userid"].GetType() == typeof(string))) { userid = (string)jsonAction["userid"]; }
            if ((userid != null) && (jsonAction.ContainsKey("realname")) && (jsonAction["realname"].GetType() == typeof(string))) { userrealname[userid] = (string)jsonAction["realname"]; }

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
                case "getUserImage":
                    {
                        if (userid == null) return;

                        // Get the image
                        string imagestr = null;
                        if (jsonAction.ContainsKey("image") && (jsonAction["image"].GetType() == typeof(string))) { imagestr = (string)jsonAction["image"]; }
                        Image userImage = null;
                        if ((imagestr != null) && (imagestr.StartsWith("data:image/jpeg;base64,")))
                        {
                            // Decode the image
                            try { userImage = Image.FromStream(new MemoryStream(Convert.FromBase64String(imagestr.Substring(23)))); } catch (Exception) { }
                        }
                        if (userImage != null) { userimages[userid] = userImage; if (onUserInfoChange != null) { onUserInfoChange(userid, 1); } } // Send and event the user image
                        else { userimages[userid] = null; } // Indicate that no user image
                        break;
                    }
                case "openUrl":
                    {
                        if (!jsonAction.ContainsKey("url") || (jsonAction["url"].GetType() != typeof(string))) return;
                        sendOpenUrlEventLog(jsonAction["url"].ToString());
                        Process.Start(jsonAction["url"].ToString());
                        break;
                    }
                case "msg": {
                        if (!jsonAction.ContainsKey("type") || (jsonAction["type"].GetType() != typeof(string))) return;
                        string eventType = jsonAction["type"].ToString();
                        switch (eventType)
                        {
                            case "console": {
                                    if ((!jsonAction.ContainsKey("value")) && (jsonAction["value"].GetType() != typeof(string))) break;
                                    if ((!jsonAction.ContainsKey("sessionid")) && (jsonAction["sessionid"].GetType() != typeof(string))) break;
                                    string cmd = jsonAction["value"].ToString();
                                    string sessionid = jsonAction["sessionid"].ToString();
                                    processConsoleCommand(cmd, sessionid, userid);
                                    break;
                                }
                            case "localapp":
                                {
                                    if ((jsonAction.ContainsKey("value")) && (jsonAction["value"].GetType() == typeof(Dictionary<string, object>))) {
                                        Dictionary<string, object> localappvalue = (Dictionary<string, object>)jsonAction["value"];
                                        if (localappvalue["cmd"].GetType() == typeof(string))
                                        {
                                            string cmdvalue = localappvalue["cmd"].ToString();
                                            if (cmdvalue == "cancelhelp") { disconnect(); }
                                        }
                                    }
                                    break;
                                }
                            case "messagebox":
                                {
                                    // Check if we have a user image, if not, request it and update user real name if available
                                    if ((userid != null) && (!userimages.ContainsKey(userid))) { getUserImage(userid); }
                                    if ((jsonAction.ContainsKey("title")) && (jsonAction["title"].GetType() == typeof(string)) && (jsonAction.ContainsKey("msg")) && (jsonAction["msg"].GetType() == typeof(string)))
                                    {
                                        string title = jsonAction["title"].ToString();
                                        string message = jsonAction["msg"].ToString();
                                        notify(userid, title, message);
                                    }
                                    break;
                                }
                            case "tunnel":
                                {
                                    // Check if we have a user image, if not, request it and update user real name if available
                                    if ((userid != null) && (!userimages.ContainsKey(userid))) { getUserImage(userid); }

                                    // Start the new tunnel
                                    if ((jsonAction.ContainsKey("value")) && (jsonAction["value"].GetType() == typeof(string)))
                                    {
                                        try
                                        {
                                            string url = jsonAction["value"].ToString();
                                            string hash = null;
                                            if ((jsonAction.ContainsKey("servertlshash")) && (jsonAction["servertlshash"].GetType() == typeof(string))) { hash = jsonAction["servertlshash"].ToString(); }
                                            if (url.StartsWith("*/")) { string su = ServerUrl.ToString(); url = su.Substring(0, su.Length - 11) + url.Substring(1); }
                                            MeshCentralTunnel tunnel = new MeshCentralTunnel(this, new Uri(url), hash, jsonAction);
                                            tunnels.Add(tunnel);
                                        }
                                        catch (Exception) { }
                                    }
                                    break;
                                }
                            case "getclip": { GetClipboard(jsonAction); break; }
                            case "setclip": { SetClipboard(jsonAction); break; }
                            default:
                                {
                                    debugMsg("Unprocessed event type: " + eventType);
                                    break;
                                }
                        }
                        break;
                    }
                default:
                    {
                        debugMsg("Unprocessed command: " + action);
                        break;
                    }
            }
        }

        private delegate void ClipboardHandler(Dictionary<string, object> jsonAction);

        private void GetClipboard(Dictionary<string, object> jsonAction)
        {
            // Clipboard can only be fetched from the main thread
            if (parent.InvokeRequired) { parent.Invoke(new ClipboardHandler(GetClipboard), jsonAction); return; }
            if ((jsonAction.ContainsKey("sessionid")) && (jsonAction["sessionid"].GetType() == typeof(string)) && (jsonAction.ContainsKey("tag")) && (jsonAction["tag"].GetType() == typeof(System.Int32)))
            {
                string extraLogStr = "";
                if (jsonAction.ContainsKey("userid") && (jsonAction["userid"].GetType() == typeof(string))) { extraLogStr += ",\"userid\":\"" + escapeJsonString((string)jsonAction["userid"]) + "\""; }
                if (jsonAction.ContainsKey("username") && (jsonAction["username"].GetType() == typeof(string))) { extraLogStr += ",\"username\":\"" + escapeJsonString((string)jsonAction["username"]) + "\""; }
                if (jsonAction.ContainsKey("remoteaddr") && (jsonAction["remoteaddr"].GetType() == typeof(string))) { extraLogStr += ",\"remoteaddr\":\"" + escapeJsonString((string)jsonAction["remoteaddr"]) + "\""; }
                if (jsonAction.ContainsKey("sessionid") && (jsonAction["sessionid"].GetType() == typeof(string))) { extraLogStr += ",\"sessionid\":\"" + escapeJsonString((string)jsonAction["sessionid"]) + "\""; }

                string sessionid = (string)jsonAction["sessionid"];
                int tag = (int)jsonAction["tag"];
                if (Clipboard.ContainsText(TextDataFormat.Text))
                {
                    string clipboardValue = Clipboard.GetText();
                    WebSocket.SendBinary(UTF8Encoding.UTF8.GetBytes("{\"action\":\"log\",\"msgid\":21,\"msgArgs\":[" + clipboardValue.Length + "],\"msg\":\"Getting clipboard content, " + clipboardValue.Length + " byte(s)\"" + extraLogStr + "}"));
                    WebSocket.SendBinary(UTF8Encoding.UTF8.GetBytes("{\"action\":\"msg\",\"type\":\"getclip\",\"sessionid\":\"" + escapeJsonString(sessionid) + "\",\"data\":\"" + escapeJsonString(clipboardValue) + "\",\"tag\":" + tag + "}"));
                }
            }
        }

        private void SetClipboard(Dictionary<string, object> jsonAction)
        {
            // Clipboard can only be set from the main thread
            if (parent.InvokeRequired) { parent.Invoke(new ClipboardHandler(SetClipboard), jsonAction); return; }
            if ((jsonAction.ContainsKey("sessionid")) && (jsonAction["sessionid"].GetType() == typeof(string)) && (jsonAction.ContainsKey("data")) && (jsonAction["data"].GetType() == typeof(string)))
            {
                string extraLogStr = "";
                if (jsonAction.ContainsKey("userid") && (jsonAction["userid"].GetType() == typeof(string))) { extraLogStr += ",\"userid\":\"" + escapeJsonString((string)jsonAction["userid"]) + "\""; }
                if (jsonAction.ContainsKey("username") && (jsonAction["username"].GetType() == typeof(string))) { extraLogStr += ",\"username\":\"" + escapeJsonString((string)jsonAction["username"]) + "\""; }
                if (jsonAction.ContainsKey("remoteaddr") && (jsonAction["remoteaddr"].GetType() == typeof(string))) { extraLogStr += ",\"remoteaddr\":\"" + escapeJsonString((string)jsonAction["remoteaddr"]) + "\""; }
                if (jsonAction.ContainsKey("sessionid") && (jsonAction["sessionid"].GetType() == typeof(string))) { extraLogStr += ",\"sessionid\":\"" + escapeJsonString((string)jsonAction["sessionid"]) + "\""; }

                string clipboardData = (string)jsonAction["data"];
                Clipboard.SetText(clipboardData);
                string sessionid = (string)jsonAction["sessionid"];
                WebSocket.SendBinary(UTF8Encoding.UTF8.GetBytes("{\"action\":\"msg\",\"type\":\"setclip\",\"sessionid\":\"" + escapeJsonString(sessionid) + "\",\"success\":true}"));
            }
        }

        static bool ByteArrayCompare(byte[] a, byte[] b)
        {
            if (a.Length != b.Length) return false;
            for (var i = 0; i < a.Length; i++) { if (a[i] != b[i]) return false; }
            return true;
        }

        private void WebSocket_onBinaryData(webSocketClient sender, byte[] data, int off, int len, int orglen)
        {
            if (len < 2) return;

            if ((ConnectionState == 15) && (data[off] == 123))
            {
                // This is JSON data
                processServerJsonData(UTF8Encoding.UTF8.GetString(data, off, len));
                return;
            }

            int cmd = ((data[off] << 8) + data[off + 1]);
            switch (cmd) {
                case 1:
                    {
                        // 0x0001 + TlsHash(48) + ServerNonce(48)
                        if (len != 98) return;
                        byte[] tlsHash = new byte[48];
                        Array.Copy(data, off + 2, tlsHash, 0, 48);
                        ServerNonce = new byte[48];
                        Array.Copy(data, off + 50, ServerNonce, 0, 48);
                        if (ByteArrayCompare(tlsHash, ServerTlsHash) == false) { disconnect(); return; }

                        // Use our agent root private key to sign the ServerHash + ServerNonce + AgentNonce
                        byte[] dataToSign = new byte[48 + 48 + 48];
                        Array.Copy(tlsHash, 0, dataToSign, 0, 48);
                        Array.Copy(ServerNonce, 0, dataToSign, 48, 48);
                        Array.Copy(Nonce, 0, dataToSign, 96, 48);
                        byte[] signature = agentCert.GetRSAPrivateKey().SignData(dataToSign, HashAlgorithmName.SHA384, RSASignaturePadding.Pkcs1);

                        // Send back our certificate + signature
                        byte[] certData = agentCert.GetRawCertData();
                        BinaryWriter bw = new BinaryWriter(new MemoryStream());
                        bw.Write(Convert.ToInt16(IPAddress.HostToNetworkOrder((short)2)));
                        bw.Write(Convert.ToInt16(IPAddress.HostToNetworkOrder((short)certData.Length)));
                        bw.Write(certData);
                        bw.Write(signature);
                        WebSocket.SendBinary(((MemoryStream)bw.BaseStream).ToArray());
                        break;
                    }
                case 2:
                    {
                        // 0x0002 + CertLength(2) + AgentCert(CertLength) + signature
                        if (len < 4) return;
                        int certLength = ((data[off + 2] << 8) + data[off + 3]);
                        if (len < certLength + 4) return;
                        byte[] serverCertData = new byte[certLength];
                        Array.Copy(data, off + 4, serverCertData, 0, certLength);
                        byte[] serverSign = new byte[len - 4 - certLength];
                        Array.Copy(data, off + 4 + certLength, serverSign, 0, len - 4 - certLength);

                        // Server signature, verify it
                        X509Certificate2 serverCert = new X509Certificate2(serverCertData);
                        byte[] dataToVerify = new byte[48 + 48 + 48];
                        Array.Copy(ServerTlsHash, 0, dataToVerify, 0, 48);
                        Array.Copy(Nonce, 0, dataToVerify, 48, 48);
                        Array.Copy(ServerNonce, 0, dataToVerify, 96, 48);
                        if (serverCert.GetRSAPublicKey().VerifyData(dataToVerify, serverSign, HashAlgorithmName.SHA384, RSASignaturePadding.Pkcs1) == false) { disconnect(); return; }

                        // Connection is a success from our side, clean up
                        ServerTlsHash = null;
                        Nonce = null;
                        ServerNonce = null;
                        ConnectionState |= 4;

                        // Send information about our agent and computer
                        // Command 3: infover, agentid, agentversion, platformtype, meshid, capabilities, computername
                        string hostName = Dns.GetHostName();
                        byte[] hostNameBytes = UTF8Encoding.UTF8.GetBytes(hostName);
                        BinaryWriter bw = new BinaryWriter(new MemoryStream());
                        bw.Write(Convert.ToInt16(IPAddress.HostToNetworkOrder((short)3))); // Command 3
                        bw.Write(Convert.ToInt32(IPAddress.HostToNetworkOrder((int)1))); // Version, always 1
                        bw.Write(Convert.ToInt32(IPAddress.HostToNetworkOrder((int)34))); // Agent ID
                        bw.Write(Convert.ToInt32(IPAddress.HostToNetworkOrder((int)0))); // Agent Version
                        bw.Write(Convert.ToInt32(IPAddress.HostToNetworkOrder((int)1))); // Platform Type, this is the icon: 1 = Desktop, 2 = Laptop, 3 = Mobile, 4 = Server, 5 = Disk, 6 = Router
                        bw.Write(MeshId); // Mesh ID. This is the identifier of the initial device group
                        bw.Write(Convert.ToInt32(IPAddress.HostToNetworkOrder((int)(1 + 4 + 8)))); // Capabilities of the agent (bitmask): 1 = Desktop, 2 = Terminal, 4 = Files, 8 = Console, 16 = JavaScript, 32 = Temporary, 64 = Recovery
                        bw.Write(Convert.ToInt16(IPAddress.HostToNetworkOrder((short)hostNameBytes.Length))); // Computer Name Length
                        bw.Write(hostNameBytes); // Computer name
                        WebSocket.SendBinary(((MemoryStream)bw.BaseStream).ToArray());

                        // If server already confirmed authenticaiton, signal authenticated connection
                        if (ConnectionState == 15) { changeState(3); serverConnected(); }
                        break;
                    }
                case 4:
                    {
                        // Server confirmed authentication, we are allowed to send commands to the server
                        ConnectionState |= 8;
                        if (ConnectionState == 15) { changeState(3); serverConnected(); }
                        break;
                    }
                default:
                    {
                        debugMsg("Unprocessed command: #" + cmd);
                        break;
                    }
            }
        }

    }
}
