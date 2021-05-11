using System;
using System.IO;
using System.Net;
using System.Text;
using System.Linq;
using System.Net.Sockets;
using System.Net.Security;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Web.Script.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Net.NetworkInformation;
using System.Diagnostics;

namespace MeshAssistant
{
    public class MeshCentralAgent
    {
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
        private xwebclient WebSocket = null;
        private int ConnectionState = 0;

        public MeshCentralAgent()
        {
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

        public delegate void onNotifyHandler(string title, string msg);
        public event onNotifyHandler onNotify;
        public void notify(string title, string msg) { if (onNotify != null) { onNotify(title, msg); } }

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
            WebSocket = new xwebclient();
            WebSocket.xdebug = debug;
            ConnectionState = 1;
            return WebSocket.Start(this, ServerUrl);
        }

        public void disconnect()
        {
            if (WebSocket == null) return;
            if (debug) { try { File.AppendAllText("debug.log", "Disconnect\r\n"); } catch (Exception) { } }
            WebSocket.Dispose();
            WebSocket = null;
            ConnectionState = 0;
        }

        private void connected()
        {
            debugMsg("Connected");
            ConnectionState |= 2;

            // Send command 1, hash + nonce
            Nonce = GenerateCryptographicRandom(48);
            BinaryWriter bw = new BinaryWriter(new MemoryStream());
            bw.Write(Convert.ToInt16(IPAddress.HostToNetworkOrder((short)1)));
            bw.Write(ServerTlsHash);
            bw.Write(Nonce);
            WebSocket.WriteBinaryWebSocket(((MemoryStream)bw.BaseStream).ToArray());
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
            WebSocket.WriteStringWebSocket("{\"action\":\"sessions\",\"type\":\"" + type + "\",\"value\":" + value + "}");
        }

        private void sendConsoleEventLog(string cmd)
        {
            WebSocket.WriteStringWebSocket("{\"action\":\"log\",\"msgid\":17,\"msgArgs\":[\"" + escapeJsonString(cmd) + "\"],\"msg\":\"Processing console command: " + escapeJsonString(cmd) + "\"}");
        }

        private void sendOpenUrlEventLog(string url)
        {
            WebSocket.WriteStringWebSocket("{\"action\":\"log\",\"msgid\":20,\"msgArgs\":[\"" + escapeJsonString(url) + "\"],\"msg\":\"Opening: " + escapeJsonString(url) + "\"}");
        }

        private void sendHelpEventLog(string username, string helpstring)
        {
            WebSocket.WriteStringWebSocket("{\"action\":\"log\",\"msgid\":98,\"msgArgs\":[\"" + escapeJsonString(username) + "\",\"" + escapeJsonString(helpstring) + "\"],\"msg\":\"Help Requested, user: " + escapeJsonString(username) + ", details: " + escapeJsonString(helpstring) + "\"}");
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
            WebSocket.WriteStringWebSocket("{\"action\":\"netinfo\",\"netif2\":" + getNetworkInfo() + "}");
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

        public void processConsoleCommand(string rawcmd, string sessionid)
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
                        if (cmd.Length != 3) { response = "Usage: notify [title] [message]"; } else { notify(cmd[1], cmd[2]); response = "Ok"; }
                        break;
                    }
                default:
                    {
                        response = "Unknown command \"" + cmd[0] + "\", type \"help\" for list of avaialble commands.";
                        break;
                    }
            }

            if (response != null) {
                WebSocket.WriteStringWebSocket("{\"action\":\"msg\",\"type\":\"console\",\"value\":\"" + response.Replace("\r", "\\r").Replace("\n", "\\n").Replace("\"","\\\"") + "\",\"sessionid\":\"" + sessionid + "\"}");
            }
        }

        public void processServerJsonData(string data)
        {
            debugMsg("JSON: " + data);

            // Parse the received JSON
            Dictionary<string, object> jsonAction = new Dictionary<string, object>();
            try { jsonAction = JSON.Deserialize<Dictionary<string, object>>(data); } catch (Exception) { return; }
            if (jsonAction == null || jsonAction["action"].GetType() != typeof(string)) return;

            // Handle the JSON command
            string action = jsonAction["action"].ToString();
            switch (action)
            {
                case "ping": { WebSocket.WriteStringWebSocket("{\"action\":\"pong\"}"); break; }
                case "pong": { break; }
                case "errorlog": { break; }
                case "sysinfo": { break; }
                case "coredump": { break; }
                case "getcoredump": { break; }
                case "openUrl":
                    {
                        if (jsonAction["url"].GetType() != typeof(string)) return;
                        sendOpenUrlEventLog(jsonAction["url"].ToString());
                        Process.Start(jsonAction["url"].ToString());
                        break;
                    }
                case "msg": {
                        if (jsonAction["type"].GetType() != typeof(string)) return;
                        string eventType = jsonAction["type"].ToString();
                        switch (eventType)
                        {
                            case "console": {
                                    if (jsonAction["value"].GetType() != typeof(string)) break;
                                    if (jsonAction["sessionid"].GetType() != typeof(string)) break;
                                    string cmd = jsonAction["value"].ToString();
                                    string sessionid = jsonAction["sessionid"].ToString();
                                    processConsoleCommand(cmd, sessionid);
                                    break;
                                }
                            case "localapp":
                                {
                                    if (jsonAction["value"].GetType() == typeof(Dictionary<string, object>)) {
                                        Dictionary<string, object> localappvalue = (Dictionary<string, object>)jsonAction["value"];
                                        if (localappvalue["cmd"].GetType() == typeof(string))
                                        {
                                            string cmdvalue = localappvalue["cmd"].ToString();
                                            if (cmdvalue == "cancelhelp") { disconnect(); }
                                        }
                                    }
                                    if (jsonAction["value"].GetType() != typeof(string)) break;
                                    break;
                                }
                            case "messagebox":
                                {
                                    if ((jsonAction["title"].GetType() == typeof(string)) && (jsonAction["msg"].GetType() == typeof(string)))
                                    {
                                        string title = jsonAction["title"].ToString();
                                        string message = jsonAction["msg"].ToString();
                                        notify(title, message);
                                    }
                                    break;
                                }
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

        static bool ByteArrayCompare(byte[] a, byte[] b)
        {
            if (a.Length != b.Length) return false;
            for (var i = 0; i < a.Length; i++) { if (a[i] != b[i]) return false; }
            return true;
        }

        public void processServerBinaryData(byte[] data, int off, int len)
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
                        WebSocket.WriteBinaryWebSocket(((MemoryStream)bw.BaseStream).ToArray());

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
                        bw.Write(Convert.ToInt32(IPAddress.HostToNetworkOrder((int)8))); // Capabilities of the agent (bitmask): 1 = Desktop, 2 = Terminal, 4 = Files, 8 = Console, 16 = JavaScript, 32 = Temporary, 64 = Recovery
                        bw.Write(Convert.ToInt16(IPAddress.HostToNetworkOrder((short)hostNameBytes.Length))); // Computer Name Length
                        bw.Write(hostNameBytes); // Computer name
                        WebSocket.WriteBinaryWebSocket(((MemoryStream)bw.BaseStream).ToArray());

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


        public class xwebclient : IDisposable
        {
            private MeshCentralAgent parent = null;
            private TcpClient wsclient = null;
            private SslStream wsstream = null;
            private NetworkStream wsrawstream = null;
            private int state = 0;
            private Uri url = null;
            private byte[] readBuffer = new Byte[500];
            private int readBufferLen = 0;
            private int accopcodes = 0;
            private bool accmask = false;
            private int acclen = 0;
            private bool proxyInUse = false;
            public bool xdebug = false;
            public bool xtlsdump = false;
            public bool xignoreCert = false;

            public static string GetProxyForUrlUsingPac(string DestinationUrl, string PacUri)
            {
                IntPtr WinHttpSession = Win32Api.WinHttpOpen("User", Win32Api.WINHTTP_ACCESS_TYPE_DEFAULT_PROXY, IntPtr.Zero, IntPtr.Zero, 0);

                Win32Api.WINHTTP_AUTOPROXY_OPTIONS ProxyOptions = new Win32Api.WINHTTP_AUTOPROXY_OPTIONS();
                Win32Api.WINHTTP_PROXY_INFO ProxyInfo = new Win32Api.WINHTTP_PROXY_INFO();

                ProxyOptions.dwFlags = Win32Api.WINHTTP_AUTOPROXY_CONFIG_URL;
                ProxyOptions.dwAutoDetectFlags = (Win32Api.WINHTTP_AUTO_DETECT_TYPE_DHCP | Win32Api.WINHTTP_AUTO_DETECT_TYPE_DNS_A);
                ProxyOptions.lpszAutoConfigUrl = PacUri;

                // Get Proxy 
                bool IsSuccess = Win32Api.WinHttpGetProxyForUrl(WinHttpSession, DestinationUrl, ref ProxyOptions, ref ProxyInfo);
                Win32Api.WinHttpCloseHandle(WinHttpSession);

                if (IsSuccess)
                {
                    return ProxyInfo.lpszProxy;
                }
                else
                {
                    Console.WriteLine("Error: {0}", Win32Api.GetLastError());
                    return null;
                }
            }

            public void Dispose()
            {
                try { wsstream.Close(); } catch (Exception) { }
                try { wsstream.Dispose(); } catch (Exception) { }
                wsstream = null;
                wsclient = null;
                state = -1;
                parent.changeState(0);
                parent.WebSocket = null;
            }

            public void Debug(string msg) { parent.debugMsg(msg);  if(xdebug) { try { File.AppendAllText("debug.log", "Debug-" + msg + "\r\n"); } catch (Exception) { } } }
            public void TlsDump(string direction, byte[] data, int offset, int len) { if (xtlsdump) { try { File.AppendAllText("debug.log", direction + ": " + BitConverter.ToString(data, offset, len).Replace("-", string.Empty) + "\r\n"); } catch (Exception) { } } }

            public bool Start(MeshCentralAgent parent, Uri url)
            {
                if (state != 0) return false;
                parent.changeState(1);
                state = 1;
                this.parent = parent;
                this.url = url;
                Uri proxyUri = null;

                // Check if we need to use a HTTP proxy (Auto-proxy way)
                try
                {
                    RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", true);
                    Object x = registryKey.GetValue("AutoConfigURL", null);
                    if ((x != null) && (x.GetType() == typeof(string)))
                    {
                        string proxyStr = GetProxyForUrlUsingPac("http" + ((url.Port == 80) ? "" : "s") + "://" + url.Host + ":" + url.Port, x.ToString());
                        if (proxyStr != null) { proxyUri = new Uri("http://" + proxyStr); }
                    }
                }
                catch (Exception) { proxyUri = null; }

                // Check if we need to use a HTTP proxy (Normal way)
                if (proxyUri == null)
                {
                    var proxy = HttpWebRequest.GetSystemWebProxy();
                    proxyUri = proxy.GetProxy(url);
                    if ((url.Host.ToLower() == proxyUri.Host.ToLower()) && (url.Port == proxyUri.Port)) { proxyUri = null; }
                }

                if (proxyUri != null)
                {
                    // Proxy in use
                    proxyInUse = true;
                    wsclient = new TcpClient();
                    Debug("Connecting with proxy in use: " + proxyUri.ToString());
                    wsclient.BeginConnect(proxyUri.Host, proxyUri.Port, new AsyncCallback(OnConnectSink), this);
                }
                else
                {
                    // No proxy in use
                    proxyInUse = false;
                    wsclient = new TcpClient();
                    Debug("Connecting without proxy");
                    wsclient.BeginConnect(url.Host, url.Port, new AsyncCallback(OnConnectSink), this);
                }
                return true;
            }

            private void OnConnectSink(IAsyncResult ar)
            {
                if (wsclient == null) return;

                // Accept the connection
                try
                {
                    wsclient.EndConnect(ar);
                }
                catch (Exception ex)
                {
                    Debug("Websocket TCP failed to connect: " + ex.ToString());
                    Dispose();
                    return;
                }

                if (proxyInUse == true)
                {
                    // Send proxy connection request
                    wsrawstream = wsclient.GetStream();
                    byte[] proxyRequestBuf = UTF8Encoding.UTF8.GetBytes("CONNECT " + url.Host + ":" + url.Port + " HTTP/1.1\r\nHost: " + url.Host + ":" + url.Port + "\r\n\r\n");
                    TlsDump("OutRaw", proxyRequestBuf, 0, proxyRequestBuf.Length);
                    try { wsrawstream.Write(proxyRequestBuf, 0, proxyRequestBuf.Length); } catch (Exception ex) { Debug(ex.ToString()); }
                    wsrawstream.BeginRead(readBuffer, readBufferLen, readBuffer.Length - readBufferLen, new AsyncCallback(OnProxyResponseSink), this);
                }
                else
                {
                    // Start TLS connection
                    Debug("Websocket TCP connected, doing TLS...");
                    wsstream = new SslStream(wsclient.GetStream(), false, VerifyServerCertificate, null);
                    wsstream.BeginAuthenticateAsClient(url.Host, null, System.Security.Authentication.SslProtocols.Tls12, false, new AsyncCallback(OnTlsSetupSink), this);
                }
            }

            private void OnProxyResponseSink(IAsyncResult ar)
            {
                if (wsrawstream == null) return;

                int len = 0;
                try { len = wsrawstream.EndRead(ar); } catch (Exception) { }
                if (len == 0)
                {
                    // Disconnect
                    Debug("Websocket proxy disconnected, length = 0.");
                    Dispose();
                    return;
                }

                TlsDump("InRaw", readBuffer, 0, readBufferLen);

                readBufferLen += len;
                string proxyResponse = UTF8Encoding.UTF8.GetString(readBuffer, 0, readBufferLen);
                if (proxyResponse.IndexOf("\r\n\r\n") >= 0)
                {
                    // We get a full proxy response, we should get something like "HTTP/1.1 200 Connection established\r\n\r\n"
                    if (proxyResponse.StartsWith("HTTP/1.1 200 "))
                    {
                        // All good, start TLS setup.
                        readBufferLen = 0;
                        Debug("Websocket TCP connected, doing TLS...");
                        wsstream = new SslStream(wsrawstream, false, VerifyServerCertificate, null);
                        wsstream.BeginAuthenticateAsClient(url.Host, null, System.Security.Authentication.SslProtocols.Tls12, false, new AsyncCallback(OnTlsSetupSink), this);
                    }
                    else
                    {
                        // Invalid response
                        Debug("Proxy connection failed: " + proxyResponse);
                        Dispose();
                    }
                }
                else
                {
                    if (readBufferLen == readBuffer.Length)
                    {
                        // Buffer overflow
                        Debug("Proxy connection failed");
                        Dispose();
                    }
                    else
                    {
                        // Read more proxy data
                        wsrawstream.BeginRead(readBuffer, readBufferLen, readBuffer.Length - readBufferLen, new AsyncCallback(OnProxyResponseSink), this);
                    }
                }
            }

            public string Base64Encode(string plainText)
            {
                var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
                return System.Convert.ToBase64String(plainTextBytes);
            }

            public string Base64Decode(string base64EncodedData)
            {
                var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
                return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
            }

            private void OnTlsSetupSink(IAsyncResult ar)
            {
                if (wsstream == null) return;

                // Accept the connection
                try
                {
                    wsstream.EndAuthenticateAsClient(ar);
                }
                catch (Exception ex)
                {
                    // Disconnect
                    if (ex.InnerException != null)
                    {
                        MessageBox.Show(ex.Message + ", Inner: " + ex.InnerException.ToString(), "MeshCentral Agent");
                    }
                    else
                    {
                        MessageBox.Show(ex.Message, "MeshCentral Agent");
                    }
                    Debug("Websocket TLS failed: " + ex.ToString());
                    Dispose();
                    return;
                }

                // Compute the remote certificate SHA384 hash
                using (SHA384 sha384Hash = SHA384.Create())
                {
                    byte[] bytes = parent.ServerTlsHash = sha384Hash.ComputeHash(wsstream.RemoteCertificate.GetRawCertData());
                    StringBuilder builder = new StringBuilder();
                    for (int i = 0; i < bytes.Length; i++) { builder.Append(bytes[i].ToString("x2")); }
                    parent.ServerTlsHashStr = builder.ToString().ToUpper();
                }
                Debug("Websocket TLS hash: " + parent.ServerTlsHash);

                // Send the HTTP headers
                Debug("Websocket TLS setup, sending HTTP header...");
                string header = "GET " + url.PathAndQuery + " HTTP/1.1\r\nHost: " + url.Host + "\r\nUpgrade: websocket\r\nConnection: Upgrade\r\nSec-WebSocket-Key: dGhlIHNhbXBsZSBub25jZQ==\r\nSec-WebSocket-Version: 13\r\n\r\n";
                try { wsstream.Write(UTF8Encoding.UTF8.GetBytes(header)); } catch (Exception ex) { Debug(ex.ToString()); }

                // Start receiving data
                wsstream.BeginRead(readBuffer, readBufferLen, readBuffer.Length - readBufferLen, new AsyncCallback(OnTlsDataSink), this);
            }

            private void OnTlsDataSink(IAsyncResult ar)
            {
                if (wsstream == null) return;

                int len = 0;
                try { len = wsstream.EndRead(ar); } catch (Exception) { }
                if (len == 0)
                {
                    // Disconnect
                    Debug("Websocket disconnected, length = 0.");
                    Dispose();
                    return;
                }
                //parent.Debug("#" + counter + ": Websocket got new data: " + len);
                readBufferLen += len;
                TlsDump("In", readBuffer, 0, len);

                // Consume all of the data
                int consumed = 0;
                int ptr = 0;
                do
                {
                    consumed = ProcessBuffer(readBuffer, ptr, readBufferLen - ptr);
                    if (consumed < 0) { Dispose(); return; } // Error, close the connection
                    ptr += consumed;
                } while ((consumed > 0) && ((readBufferLen - consumed) > 0));

                // Move the data forward
                if ((ptr > 0) && (readBufferLen - ptr) > 0)
                {
                    //Console.Write("MOVE FORWARD\r\n");
                    Array.Copy(readBuffer, ptr, readBuffer, 0, (readBufferLen - ptr));
                }
                readBufferLen = (readBufferLen - ptr);

                // If the buffer is too small, double the size here.
                if (readBuffer.Length - readBufferLen == 0)
                {
                    //Debug("Increasing the read buffer size from " + readBuffer.Length + " to " + (readBuffer.Length * 2) + ".");
                    byte[] readBuffer2 = new byte[readBuffer.Length * 2];
                    Array.Copy(readBuffer, 0, readBuffer2, 0, readBuffer.Length);
                    readBuffer = readBuffer2;
                }

                // Receive more data
                try { wsstream.BeginRead(readBuffer, readBufferLen, readBuffer.Length - readBufferLen, new AsyncCallback(OnTlsDataSink), this); } catch (Exception) { }
            }

            private int ProcessBuffer(byte[] buffer, int offset, int len)
            {
                string ss = UTF8Encoding.UTF8.GetString(buffer, offset, len);

                if (state == 1)
                {
                    // Look for the end of the http header
                    string header = UTF8Encoding.UTF8.GetString(buffer, offset, len);
                    int i = header.IndexOf("\r\n\r\n");
                    if (i == -1) return 0;
                    Dictionary<string, string> parsedHeader = ParseHttpHeader(header.Substring(0, i));
                    if ((parsedHeader == null) || (parsedHeader["_Path"] != "101")) { Debug("Websocket bad header."); return -1; } // Bad header, close the connection
                    //Debug("Websocket got setup upgrade header.");
                    state = 2;
                    parent.changeState(2);
                    parent.connected();
                    return len; // TODO: Technically we need to return the header length before UTF8 convert.
                }
                else if (state == 2)
                {
                    // Parse a websocket fragment header
                    if (len < 2) return 0;
                    int headsize = 2;
                    accopcodes = buffer[offset];
                    accmask = ((buffer[offset + 1] & 0x80) != 0);
                    acclen = (buffer[offset + 1] & 0x7F);

                    if ((accopcodes & 0x0F) == 8)
                    {
                        // Close the websocket
                        Debug("Websocket got closed fragment.");
                        return -1;
                    }

                    if (acclen == 126)
                    {
                        if (len < 4) return 0;
                        headsize = 4;
                        acclen = (buffer[offset + 2] << 8) + (buffer[offset + 3]);
                    }
                    else if (acclen == 127)
                    {
                        if (len < 10) return 0;
                        headsize = 10;
                        acclen = (buffer[offset + 6] << 24) + (buffer[offset + 7] << 16) + (buffer[offset + 8] << 8) + (buffer[offset + 9]);
                        Debug("Websocket receive large fragment: " + acclen);
                    }
                    if (accmask == true)
                    {
                        // TODO: Do unmasking here.
                        headsize += 4;
                    }
                    //parent.Debug("#" + counter + ": Websocket frag header - FIN: " + ((accopcodes & 0x80) != 0) + ", OP: " + (accopcodes & 0x0F) + ", LEN: " + acclen + ", MASK: " + accmask);
                    state = 3;
                    return headsize;
                }
                else if (state == 3)
                {
                    // Parse a websocket fragment data
                    if (len < acclen) return 0;
                    //Console.Write("WSREAD: " + acclen + "\r\n");
                    ProcessWsBuffer(buffer, offset, acclen, accopcodes);
                    state = 2;
                    return acclen;
                }
                return 0;
            }

            private void ProcessWsBuffer(byte[] data, int offset, int len, int op)
            {
                if (op == 130) { parent.processServerBinaryData(data, offset, len); }
                //else if (op == 129) { parent.processServerTextData(UTF8Encoding.UTF8.GetString(data, offset, len)); }
            }

            private Dictionary<string, string> ParseHttpHeader(string header)
            {
                string[] lines = header.Replace("\r\n", "\r").Split('\r');
                if (lines.Length < 2) { return null; }
                string[] directive = lines[0].Split(' ');
                Dictionary<string, string> values = new Dictionary<string, string>();
                values["_Action"] = directive[0];
                values["_Path"] = directive[1];
                values["_Protocol"] = directive[2];
                for (int i = 1; i < lines.Length; i++)
                {
                    var j = lines[i].IndexOf(":");
                    values[lines[i].Substring(0, j).ToLower()] = lines[i].Substring(j + 1).Trim();
                }
                return values;
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

            private bool VerifyServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
            {
                return true;
            }

            public void WriteStringWebSocket(string data) {
                WriteBinaryWebSocket(UTF8Encoding.UTF8.GetBytes(data));
            }

            public void WriteBinaryWebSocket(byte[] data)
            {
                // Convert the string into a buffer with 4 byte of header space.
                int len = data.Length;
                byte[] buf = new byte[4 + data.Length];
                Array.Copy(data, 0, buf, 4, data.Length);
                len = buf.Length - 4;

                // Check that everything is ok
                if ((state < 2) || (len < 1) || (len > 65535)) { Dispose(); return; }

                //Console.Write("Length: " + len + "\r\n");
                //System.Threading.Thread.Sleep(0);

                if (len < 126)
                {
                    // Small fragment
                    buf[2] = 130; // Fragment op code (129 = text, 130 = binary)
                    buf[3] = (byte)(len & 0x7F);
                    //try { wsstream.BeginWrite(buf, 2, len + 2, new AsyncCallback(WriteWebSocketAsyncDone), args); } catch (Exception) { Dispose(); return; }
                    TlsDump("Out", buf, 2, len + 2);
                    try { wsstream.Write(buf, 2, len + 2); } catch (Exception ex) { Debug(ex.ToString()); }
                }
                else
                {
                    // Large fragment
                    buf[0] = 130; // Fragment op code (129 = text, 130 = binary)
                    buf[1] = 126;
                    buf[2] = (byte)((len >> 8) & 0xFF);
                    buf[3] = (byte)(len & 0xFF);
                    //try { wsstream.BeginWrite(buf, 0, len + 4, new AsyncCallback(WriteWebSocketAsyncDone), args); } catch (Exception) { Dispose(); return; }
                    TlsDump("Out", buf, 0, len + 4);
                    try { wsstream.Write(buf, 0, len + 4); } catch (Exception ex) { Debug(ex.ToString()); }
                }
            }


        }

    }
}
