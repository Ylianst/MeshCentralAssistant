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
using System.Text;
using System.Linq;
using System.Management;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Web.Script.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.Net.NetworkInformation;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;
using CERTENROLLLib;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Reflection;
using System.Net.Security;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;

namespace MeshAssistant
{
    public class MeshCentralAgent
    {
        private MainForm parent;
        public bool debug = false;
        public bool autoConnect = false; // Agent is in auto-connection mode. When enabled, user consent is mandatory.
        public int autoConnectTime = 5; // Number of seconds to next connection attempt.
        private System.Threading.Timer autoConnectTimer = null;
        public int state = 0;
        public string HelpRequest = null;
        private byte[] MeshId = null;
        private string MeshIdMB64 = null;
        private string ServerId = null;
        private byte[] ServerTlsHash = null;
        public string ServerTlsHashStr = null;
        private Uri ServerUrl = null;
        private byte[] Nonce = null;
        private byte[] ServerNonce = null;
        private X509Certificate2 agentCert = null;
        private JavaScriptSerializer JSON = new JavaScriptSerializer();
        public webSocketClient WebSocket = null;
        private int ConnectionState = 0;
        private Dictionary<string, string> msh = null; // MSH key --> value
        private List<MeshCentralTunnel> tunnels = new List<MeshCentralTunnel>(); // List of active tunnels
        private List<MeshCentralTcpTunnel> tcptunnels = new List<MeshCentralTcpTunnel>(); // List of active TCP tunnels
        public Dictionary<string, Image> userimages = new Dictionary<string, Image>(); // UserID --> Image
        public Dictionary<string, string> usernames = new Dictionary<string, string>(); // UserID --> User name
        public Dictionary<string, string> userrealname = new Dictionary<string, string>(); // UserID --> User real name
        private string softwareName = null;
        private string selfExecutableHashHex = null;
        public string privacyBarText = null;
        private Random rand = new Random();
        private string discoveryKey = null;
        private MeshDiscovery scanner = null;
        public Image CustomizationLogo = null;
        public string CustomizationTitle = null;
        private int autoConnectFlags = 0;
        PerformanceCounter cpuCounter;
        PerformanceCounter ramCounter;
        public bool terminalSupport = MeshCentralTerminal.CheckTerminalSupport();
        public List<HttpFileDownload> fileDownloads = new List<HttpFileDownload>();
        public List<string> pendingWakeOnLan = new List<string>();
        private System.Threading.Timer pendingWakeOnLanTimer = null;
        private string agentTag = null;
        public bool selfSharingAllowed = false;
        public string selfSharingUrl = null;
        public bool selfSharingViewOnly = false;
        public int selfSharingFlags = 0;
        public bool allowUseOfProxy = true;

        // Sessions
        public Dictionary<string, object> DesktopSessions = null;
        public Dictionary<string, object> TerminalSessions = null;
        public Dictionary<string, object> FilesSessions = null;
        public Dictionary<string, object> TcpSessions = null;
        public Dictionary<string, object> UdpSessions = null;
        public Dictionary<string, object> MessagesSessions = null;

        public delegate void onUserInfoChangeHandler(string userid, int change); // Change: 1 = Image, 2 = Realname, 3 = PrivacyText
        public event onUserInfoChangeHandler onUserInfoChange;
        public static string getSelfFilename(string ext) { string s = Process.GetCurrentProcess().MainModule.FileName; return s.Substring(0, s.Length - 4) + ext; }

        public delegate void onSelfUpdateHandler(string name, string hash, string url, string serverhash);
        public event onSelfUpdateHandler onSelfUpdate;

        public delegate void onRequestConsentHandler(MeshCentralTunnel tunnel, string msg, int protocol, string userid);
        public event onRequestConsentHandler onRequestConsent;

        public delegate void onSelfSharingStatusHandler(bool allowed, string url);
        public event onSelfSharingStatusHandler onSelfSharingStatus;

        public void askForConsent(MeshCentralTunnel tunnel, string msg, int protocol, string userid)
        {
            if (onRequestConsent != null) { onRequestConsent(tunnel, msg, protocol, userid); }
        }

        public bool doesUserHaveSession(string userid, int protocol)
        {
            lock (tunnels) { foreach (MeshCentralTunnel tunnel in tunnels) { if ((tunnel.protocol == protocol) && (tunnel.sessionUserName == userid) && (tunnel.consentRequested == false)) return true; } }
            return false;
        }

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
                if (WebSocket != null) WebSocket.SendString(r);
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
                if (WebSocket != null) WebSocket.SendString(r);
            }
        }

        public MeshCentralAgent(MainForm parent, string mshstr, string softwareName, string selfExecutableHashHex, bool debug)
        {
            this.debug = debug;
            this.parent = parent;
            this.softwareName = softwareName;
            this.selfExecutableHashHex = selfExecutableHashHex;

            // Load the agent certificate and private key
            agentCert = LoadAgentCertificate();

            // Load the MSH file
            string[] lines = mshstr.Replace("\r\n", "\r").Split('\r');
            msh = new Dictionary<string, string>();
            foreach (string line in lines) {
                int i = line.IndexOf('=');
                if (i > 0)
                {
                    string key = line.Substring(0, i);
                    string val = line.Substring(i + 1);
                    if (msh.ContainsKey(key) == false) { msh.Add(key, val); }
                }
            }

            // Get the MeshId, ServerId, and ServerUrl
            if (msh.ContainsKey("MeshID")) { string m = msh["MeshID"]; if (m.StartsWith("0x")) { m = m.Substring(2); } MeshId = StringToByteArray(m); }
            if (msh.ContainsKey("ServerID")) { ServerId = msh["ServerID"]; }
            if (msh.ContainsKey("MeshServer") && (msh["MeshServer"] != "local")) { try { ServerUrl = new Uri(msh["MeshServer"]); } catch (Exception) { } }
            if (msh.ContainsKey("Title")) { CustomizationTitle = msh["Title"]; }
            if (msh.ContainsKey("AutoConnect")) {
                if (int.TryParse(msh["AutoConnect"], out autoConnectFlags))
                {
                    parent.autoConnect = ((autoConnectFlags & 1) != 0); // Auto connect with built-in agent
                    parent.SystemTrayApp = ((autoConnectFlags & 2) == 0); // Be a system tray tool
                }
            }
            if (msh.ContainsKey("ignoreProxyFile") || msh.ContainsKey("noProxy")) { allowUseOfProxy = false; }
            if (msh.ContainsKey("DiscoveryKey")) { discoveryKey = msh["DiscoveryKey"]; }

            // Check if we are locked to a server
            if (Program.LockToServerId != null) { ServerId = Program.LockToServerId; }
            if ((Program.LockToHostname != null) && (ServerUrl.Host != Program.LockToHostname)) { ServerUrl = new Uri(ServerUrl.Scheme + "://" + Program.LockToHostname + ":" + ServerUrl.Port + ServerUrl.PathAndQuery); }

            Log("MSH MeshID: " + msh["MeshID"]);
            Log("MSH ServerID: " + ServerId);
            Log("MSH MeshServer: " + ServerUrl);

            // Create a modified Base64 meshid, this will be used to request updates.
            MeshIdMB64 = Convert.ToBase64String(MeshId).Replace("+", "@").Replace("/", "$");

            // Load customization image
            if (msh.ContainsKey("Image")) { try { CustomizationLogo = new Bitmap(new MemoryStream(Convert.FromBase64String(msh["Image"]))); } catch (Exception) { } }
            if ((CustomizationLogo == null) && (File.Exists("assistant-logo.png"))) { try { CustomizationLogo = new Bitmap(new MemoryStream(File.ReadAllBytes("assistant-logo.png"))); } catch (Exception) { } }

            // Load agent tag
            string strExeFilePath = Assembly.GetExecutingAssembly().Location;
            if (strExeFilePath.ToLower().EndsWith(".exe"))
            {
                // if the agent tag file exists, load it.
                strExeFilePath = strExeFilePath.Substring(0, strExeFilePath.Length - 4) + ".tag";
                if (File.Exists(strExeFilePath))
                {
                    string tagFileData = File.ReadAllText(strExeFilePath);
                    if (tagFileData.Length > 1000) { tagFileData = tagFileData.Substring(0, 1000); } // Only keep the first 1000 characters of the tag file.
                    agentTag = tagFileData;
                }
            }

            // Continue setup on a different thread
            Thread t = new Thread(new ThreadStart(MeshCentralAgentEx));
            t.Start();
        }

        private void MeshCentralAgentEx()
        {
            try
            {
                // Setup scanner that finds the MeshCentral server
                if (ServerUrl == null)
                {
                    scanner = new MeshDiscovery(discoveryKey);
                    scanner.OnNotify += Discovery_OnNotify;
                    scanner.MulticastPing();
                }

                // Setup perfromance counters (There two calls take time, this is why we do these on a different thread).
                cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
                ramCounter = new PerformanceCounter("Memory", "Available MBytes");
            }
            catch (Exception) { }
        }

        private void Discovery_OnNotify(MeshDiscovery sender, IPEndPoint source, IPEndPoint local, string agentCertHash, string url, string name, string info)
        {
            if (ServerId != agentCertHash) return;
            url += "agent.ashx";
            Uri u = null;
            try { u = new Uri(url); } catch (Exception) { return; }
            if (u == null) return;
            if ((ServerUrl == null) || (ServerUrl.Equals(u) == false))
            {
                ServerUrl = u;
                if (((state == 0) && (autoConnect)) || (state == 1))
                {
                    autoConnectTime = 5;
                    reconnectAttempt(null);
                }
            }
        }

        public static int checkMshFile()
        {
            // Load the MSH file
            Dictionary<string, string> msh = LoadMshFile();
            if (msh == null) return 1;
            if (!msh.ContainsKey("MeshID")) return 1;
            if (!msh.ContainsKey("ServerID")) return 1;
            if (!msh.ContainsKey("MeshServer")) return 1;

            // Try to parse the URL
            Uri u = null;
            try { u = new Uri(msh["MeshServer"]); } catch (Exception) { }
            if (u == null) return 1;

            // Check if we are locked to a server
            if ((Program.LockToServerId != null) && (msh["ServerID"] != Program.LockToServerId)) return 2;
            if ((Program.LockToHostname != null) && (u.Host.ToLower() != Program.LockToHostname.ToLower())) return 2;

            return 0;
        }

        public static int checkMshStr(string mshstr)
        {
            if (mshstr == null) return 1;
            string[] lines = mshstr.Replace("\r\n", "\r").Split('\r');
            Dictionary<string, string> msh = new Dictionary<string, string>();
            foreach (string line in lines)
            {
                int i = line.IndexOf('=');
                if (i > 0)
                {
                    string key = line.Substring(0, i);
                    string val = line.Substring(i + 1);
                    if (msh.ContainsKey(key) == false) { msh.Add(key, val); }
                }
            }
            if (!msh.ContainsKey("MeshID")) return 1;
            if (!msh.ContainsKey("ServerID")) return 1;
            if (!msh.ContainsKey("MeshServer")) return 1;

            // Try to parse the URL if not set to local
            Uri u = null;
            if (msh["MeshServer"].ToLower() != "local")
            {
                try { u = new Uri(msh["MeshServer"]); } catch (Exception) { }
                if (u == null) return 1;
            }

            // Check if we are locked to a server
            if ((Program.LockToServerId != null) && (msh["ServerID"] != Program.LockToServerId)) return 2;
            if ((Program.LockToHostname != null) && ((u == null) || (u.Host.ToLower() != Program.LockToHostname.ToLower()))) return 2;

            return 0;
        }

        public static bool getMshCustomization(string mshstr, out string title, out Image image)
        {
            title = null;
            image = null;
            if (mshstr == null) return false;
            string[] lines = mshstr.Replace("\r\n", "\r").Split('\r');
            Dictionary<string, string> msh = new Dictionary<string, string>();
            foreach (string line in lines) { int i = line.IndexOf('='); if (i > 0) { msh.Add(line.Substring(0, i), line.Substring(i + 1)); } }
            if (msh.ContainsKey("Title")) { title = msh["Title"]; }
            if (msh.ContainsKey("Image")) { try { image = new Bitmap(new MemoryStream(Convert.FromBase64String(msh["Image"]))); } catch (Exception) { } }
            return true;
        }

        private X509Certificate2 LoadAgentCertificate()
        {
            string p12filename = getSelfFilename(".p12");
            X509Certificate2 cert = null;
            if (File.Exists(p12filename)) { try { cert = new X509Certificate2(p12filename, "dummy"); } catch (Exception) { } }
            if (cert != null) { Log("LoadAgentCertificate() - Loaded existing certificate from file."); return cert; }
            if (cert == null)
            {
                string certstr = Settings.GetRegValue("cert", null);
                if (certstr != null) { try { cert = new X509Certificate2(Convert.FromBase64String(certstr), "dummy"); } catch (Exception) { } }
                if (cert != null) { Log("LoadAgentCertificate() - Loaded existing certificate from registry."); return cert; }
            }
            try
            {
                Log("LoadAgentCertificate() - Creating new certificate...");
                RSA rsa = RSA.Create(3072); // Generate asymmetric RSA key pair
                CertificateRequest req = new CertificateRequest("cn=MeshAgent", rsa, HashAlgorithmName.SHA384, RSASignaturePadding.Pkcs1);
                cert = req.CreateSelfSigned(DateTimeOffset.Now.AddDays(-10), DateTimeOffset.Now.AddYears(20));
                //File.WriteAllBytes(p12filename, cert.Export(X509ContentType.Pkcs12, "dummy"));
                Settings.SetRegValue("cert", Convert.ToBase64String(cert.Export(X509ContentType.Pkcs12, "dummy")));
                Log("LoadAgentCertificate() - Certificate created");
                return cert;
            } catch (Exception) { }

            Log("LoadAgentCertificate() - Creating new certificate using CertEnroll...");
            cert = CreateSelfSignedCertificate("MeshAgent");
            //File.WriteAllBytes(p12filename, cert.Export(X509ContentType.Pkcs12, "dummy"));
            Settings.SetRegValue("cert", Convert.ToBase64String(cert.Export(X509ContentType.Pkcs12, "dummy")));
            Log("LoadAgentCertificate() - Certificate created");
            return cert;
        }

        public static X509Certificate2 CreateSelfSignedCertificate(string subjectName)
        {
            // create DN for subject and issuer
            var dn = new CX500DistinguishedName();
            dn.Encode("CN=" + subjectName, X500NameFlags.XCN_CERT_NAME_STR_NONE);

            // create a new private key for the certificate
            CX509PrivateKey privateKey = new CX509PrivateKey();
            privateKey.ProviderName = "Microsoft Base Cryptographic Provider v1.0";
            privateKey.MachineContext = false;
            privateKey.Length = 2048;
            privateKey.KeySpec = X509KeySpec.XCN_AT_SIGNATURE; // use is not limited
            privateKey.ExportPolicy = X509PrivateKeyExportFlags.XCN_NCRYPT_ALLOW_PLAINTEXT_EXPORT_FLAG;
            privateKey.Create();

            // Use the stronger SHA384 hashing algorithm
            var hashobj = new CObjectId();
            hashobj.InitializeFromAlgorithmName(ObjectIdGroupId.XCN_CRYPT_HASH_ALG_OID_GROUP_ID,
                ObjectIdPublicKeyFlags.XCN_CRYPT_OID_INFO_PUBKEY_ANY,
                AlgorithmFlags.AlgorithmFlagsNone, "SHA384");

            // add extended key usage if you want - look at MSDN for a list of possible OIDs
            var oid = new CObjectId();
            oid.InitializeFromValue("1.3.6.1.5.5.7.3.1"); // SSL server
            var oidlist = new CObjectIds();
            oidlist.Add(oid);
            var eku = new CX509ExtensionEnhancedKeyUsage();
            eku.InitializeEncode(oidlist);

            // Create the self signing request
            var cert = new CX509CertificateRequestCertificate();
            cert.InitializeFromPrivateKey(X509CertificateEnrollmentContext.ContextUser, privateKey, "");
            cert.Subject = dn;
            cert.Issuer = dn; // the issuer and the subject are the same
            cert.NotBefore = DateTime.Now.AddDays(-10);
            cert.NotAfter = DateTime.Now.AddYears(20);
            cert.X509Extensions.Add((CX509Extension)eku); // add the EKU
            cert.HashAlgorithm = hashobj; // Specify the hashing algorithm
            cert.Encode(); // encode the certificate

            // Do the final enrollment process
            var enroll = new CX509Enrollment();
            enroll.InitializeFromRequest(cert); // load the certificate
            enroll.CertificateFriendlyName = subjectName; // Optional: add a friendly name
            string csr = enroll.CreateRequest(); // Output the request in base64
                                                 // and install it back as the response
            enroll.InstallResponse(InstallResponseRestrictionFlags.AllowUntrustedCertificate,
                csr, EncodingType.XCN_CRYPT_STRING_BASE64, ""); // no password
                                                                // output a base64 encoded PKCS#12 so we can import it back to the .Net security classes
            var base64encoded = enroll.CreatePFX("", // no password, this is for internal consumption
                PFXExportOptions.PFXExportChainWithRoot);

            // instantiate the target class with the PKCS#12 data (and the empty password)
            return new System.Security.Cryptography.X509Certificates.X509Certificate2(
                System.Convert.FromBase64String(base64encoded), "",
                // mark the private key as exportable (this is usually what you want to do)
                System.Security.Cryptography.X509Certificates.X509KeyStorageFlags.Exportable
            );
        }

        public static string LoadMshFileStr()
        {
            string mshfilename = getSelfFilename(".msh");
            if (!File.Exists(mshfilename)) return null;
            string str = null;
            try { str = File.ReadAllText(mshfilename); } catch (Exception) { }
            return str;
        }

        public static Dictionary<string, string> LoadMshFile()
        {
            string mshfilename = getSelfFilename(".msh");
            if (!File.Exists(mshfilename)) return null;
            string[] lines = null;
            try { lines = File.ReadAllLines(mshfilename); } catch (Exception) { }
            if (lines == null) return null;
            Dictionary<string, string> vals = new Dictionary<string, string>();
            foreach (string line in lines) { int i = line.IndexOf('='); if (i > 0) { vals.Add(line.Substring(0, i), line.Substring(i + 1)); } }
            return vals;
        }

        public delegate void onNotifyHandler(string userid, string title, string msg);
        public event onNotifyHandler onNotify;
        public void notify(string userid, string title, string msg) { if (onNotify != null) { onNotify(userid, title, msg); } }

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

        public void Log(string msg)
        {
            if (debug) {
                //Console.WriteLine(msg);
                try { File.AppendAllText("debug.log", DateTime.Now.ToString("HH:mm:tt.ffff") + ": MCAgent: " + msg + "\r\n"); } catch (Exception) { }
            }
        }

        public delegate void onLogEventHandler(DateTime time, string userid, string msg);
        public event onLogEventHandler onLogEvent;

        public void Event(string userid, string msg)
        {
            DateTime now = DateTime.Now;
            if (onLogEvent != null) { onLogEvent(now, userid, msg); }
            try { File.AppendAllText("events.log", now.ToString("yyyy-MM-ddTHH:mm:sszzz") + ", " + userid + ", " + msg + "\r\n"); } catch (Exception) { }
        }

        public static byte[] StringToByteArray(string hex) { return Enumerable.Range(0, hex.Length).Where(x => x % 2 == 0).Select(x => Convert.ToByte(hex.Substring(x, 2), 16)).ToArray(); }

        public bool connect()
        {
            Log("connect()");
            autoConnectTime = 5;
            if (autoConnectTimer != null) { autoConnectTimer.Dispose(); autoConnectTimer = null; }
            if (ServerUrl == null) return false;
            return connectex();
        }

        private bool connectex()
        {
            if ((MeshId == null) || (MeshId.Length != 48)) return false;
            if (WebSocket != null) return false;
            Log(string.Format("Attempting connection to {0}", ServerUrl));
            userimages = new Dictionary<string, Image>(); // UserID --> Image
            usernames = new Dictionary<string, string>(); // UserID --> User names
            userrealname = new Dictionary<string, string>(); // UserID --> User real name
            WebSocket = new webSocketClient();
            WebSocket.pongTimeSeconds = 120; // Send a websocket pong every 2 minutes.
            WebSocket.allowUseOfProxy = allowUseOfProxy;
            WebSocket.debug = debug;
            WebSocket.onStateChanged += WebSocket_onStateChanged;
            WebSocket.onBinaryData += WebSocket_onBinaryData;
            WebSocket.onStringData += WebSocket_onStringData;
            ConnectionState = 1;
            WebSocket.TLSCertCheck = webSocketClient.TLSCertificateCheck.Ignore;
            if (ServerUrl.Scheme == "http") { ServerUrl = new Uri(ServerUrl.OriginalString.Replace("http://", "ws://")); }
            if (ServerUrl.Scheme == "https") { ServerUrl = new Uri(ServerUrl.OriginalString.Replace("https://", "wss://")); }
            return WebSocket.Start(ServerUrl, null, null);
        }

        private void WebSocket_onStateChanged(webSocketClient sender, webSocketClient.ConnectionStates state)
        {
            Log(string.Format("WebSocket_onStateChanged: {0}", state));
            if (state == webSocketClient.ConnectionStates.Disconnected)
            {
                disconnectex();
            }
            else if (state == webSocketClient.ConnectionStates.Connecting)
            {
                changeState(1);
            }
            else if (state == webSocketClient.ConnectionStates.Connected)
            {
                if ((ConnectionState & 2) != 0) return;
                changeState(2);
                ConnectionState |= 2;

                // Get the TLS certificate
                X509Certificate cert = WebSocket.RemoteCertificate;
                if (cert == null) return;

                // Compute the remote certificate SHA384 hash
                using (SHA384 sha384Hash = SHA384.Create())
                {
                    byte[] bytes = ServerTlsHash = sha384Hash.ComputeHash(cert.GetRawCertData());
                    StringBuilder builder = new StringBuilder();
                    for (int i = 0; i < bytes.Length; i++) { builder.Append(bytes[i].ToString("x2")); }
                    ServerTlsHashStr = builder.ToString().ToUpper();
                }
                Log(string.Format("Websocket TLS hash: {0}", ServerTlsHashStr));

                // Send command 1, hash + nonce
                Nonce = GenerateCryptographicRandom(48);
                BinaryWriter bw = new BinaryWriter(new MemoryStream());
                bw.Write(Convert.ToInt16(IPAddress.HostToNetworkOrder((short)1)));
                bw.Write(ServerTlsHash);
                bw.Write(Nonce);
                if (WebSocket != null) WebSocket.SendBinary(((MemoryStream)bw.BaseStream).ToArray());
            }
        }

        public void disconnect()
        {
            Log("Disconnect");
            autoConnectTime = 5;
            if (autoConnectTimer != null) { autoConnectTimer.Dispose(); autoConnectTimer = null; }
            disconnectex();
        }

        private void disconnectex()
        {
            if (WebSocket == null) return;
            Log("DisconnectEx");
            WebSocket.Dispose();
            WebSocket = null;
            ConnectionState = 0;
            changeState(0);
            foreach (MeshCentralTunnel tunnel in tunnels) { tunnel.disconnect(); }
            foreach (MeshCentralTcpTunnel tcptunnel in tcptunnels) { tcptunnel.disconnect(); }

            // Setup auto-reconnect timer if needed
            if (autoConnect) {
                Log(string.Format("Setting connect retry timer to {0} seconds", autoConnectTime));
                if (autoConnectTimer != null) { autoConnectTimer.Dispose(); autoConnectTimer = null; }
                autoConnectTimer = new System.Threading.Timer(new System.Threading.TimerCallback(reconnectAttempt), null, autoConnectTime * 1000, autoConnectTime * 1000);
            }
        }

        private void reconnectAttempt(object state)
        {
            Log("ReconnectAttempt");
            if (autoConnectTimer != null) { autoConnectTimer.Dispose(); autoConnectTimer = null; }
            autoConnectTime = autoConnectTime + rand.Next(4, autoConnectTime);
            if (autoConnectTime > 1200) { autoConnectTime = 1200; }
            connectex();
        }

        public void disconnectAllTunnels()
        {
            foreach (MeshCentralTunnel tunnel in tunnels) { tunnel.disconnect(); }
        }

        private void serverConnected()
        {
            Log("Server Connected");
            autoConnectTime = 5;

            // Send agent tag
            sendAgentTag();

            // Send core information
            SendCoreInfo();

            // Update network information
            sendNetworkInfo();

            // Send help session
            if (HelpRequest != null)
            {
                string userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                sendSessionUpdate("help", "{\"" + escapeJsonString(userName) + "\":\"" + escapeJsonString(HelpRequest) + "\"}");
                sendHelpEventLog(userName, HelpRequest);
            }

            // Send self-update query
            if ((softwareName != null) && (selfExecutableHashHex != null))
            {
                sendSelfUpdateQuery(softwareName, selfExecutableHashHex);
            }

            // Send system information
            sendSysInfo(null, null);

            // Remove any guest self-sharing links
            sendRequestGuestSharing(0, false);
        }

        public void SendCoreInfo()
        {
            string core = GetCoreInfo();
            Log(string.Format("sendCoreInfo: {0}", core));
            if (WebSocket != null) WebSocket.SendString(core);
        }

        public string GetCoreInfo()
        {
            int caps = (1 + 4 + 8); // Capabilities of the agent (bitmask): 1 = Desktop, 2 = Terminal, 4 = Files, 8 = Console, 16 = JavaScript, 32 = Temporary, 64 = Recovery
            if (terminalSupport) { caps += 2; }
            string version = "v" + Assembly.GetExecutingAssembly().GetName().Version.ToString();
            string core = "{\"action\":\"coreinfo\",\"value\":\"" + escapeJsonString(version) + "\",\"caps\":" + caps;
            try { core += ",\"root\":" + (IsAdministrator() ? "true" : "false"); } catch (Exception) { }
            try { core += ",\"users\":[\"" + escapeJsonString(WindowsIdentity.GetCurrent().Name) + "\"]"; } catch (Exception) { }
            try
            {
                bool first = true;
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_OperatingSystem"))
                {
                    foreach (ManagementObject queryObj in searcher.Get())
                    {
                        if (first) { first = false; } else { continue; }
                        core += ",\"osdesc\":\"" + escapeJsonString(queryObj["Caption"].ToString() + " v" + queryObj["Version"].ToString()) + "\"";
                    }
                }
            }
            catch (Exception) { }
            core += "}";
            return core;
        }

        public void RequestHelp(string HelpRequest)
        {
            if (this.HelpRequest == HelpRequest) return;
            this.HelpRequest = HelpRequest;
            if (HelpRequest != null) {
                // Send help request
                string userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                sendSessionUpdate("help", "{\"" + escapeJsonString(userName) + "\":\"" + escapeJsonString(HelpRequest) + "\"}");
                sendHelpEventLog(userName, HelpRequest);
                Event("local", "Requested help for \"" + HelpRequest.Replace("\r\n", " ") + "\"");
            } else {
                // Clear help request
                sendSessionUpdate("help", "{}");
                Event("local", "Canceled help request");
            }
        }

        public void sendRequestGuestSharing(int flags, bool viewOnly)
        {
            Log(string.Format("sendRequestGuestSharing {0}, {1}", flags, viewOnly));
            if (WebSocket != null) WebSocket.SendString("{\"action\":\"guestShare\",\"flags\":" + flags + ",\"viewOnly\":" + ((viewOnly == true)?"true":"false") + "}");
        }

        private void sendSessionUpdate(string type, string value)
        {
            Log(string.Format("sendSessionUpdate {0}, {1}", type, value));
            if (WebSocket != null) WebSocket.SendString("{\"action\":\"sessions\",\"type\":\"" + type + "\",\"value\":" + value + "}");
        }

        private void sendConsoleEventLog(string cmd)
        {
            Log(string.Format("sendConsoleEventLog {0}", cmd));
            if (WebSocket != null) WebSocket.SendString("{\"action\":\"log\",\"msgid\":17,\"msgArgs\":[\"" + escapeJsonString(cmd) + "\"],\"msg\":\"Processing console command: " + escapeJsonString(cmd) + "\"}");
        }

        private void sendOpenUrlEventLog(string url)
        {
            Log(string.Format("sendOpenUrlEventLog {0}", url));
            if (WebSocket != null) WebSocket.SendString("{\"action\":\"log\",\"msgid\":20,\"msgArgs\":[\"" + escapeJsonString(url) + "\"],\"msg\":\"Opening: " + escapeJsonString(url) + "\"}");
        }

        private void sendHelpEventLog(string username, string helpstring)
        {
            Log(string.Format("sendHelpEventLog {0}, {1}", username, helpstring));
            if (WebSocket != null) WebSocket.SendString("{\"action\":\"log\",\"msgid\":98,\"msgArgs\":[\"" + escapeJsonString(username) + "\",\"" + escapeJsonString(helpstring) + "\"],\"msg\":\"Help Requested, user: " + escapeJsonString(username) + ", details: " + escapeJsonString(helpstring) + "\"}");
        }

        private void sendSelfUpdateQuery(string softwareName, string softwareHash)
        {
            Log(string.Format("sendSelfUpdateQuery {0}, {1}", softwareName, softwareHash));
            if (WebSocket != null) WebSocket.SendString("{\"action\":\"meshToolInfo\",\"name\":\"" + escapeJsonString(softwareName) + "\",\"hash\":\"" + escapeJsonString(softwareHash) + "\",\"cookie\":true,\"msh\":true}");
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
            Log("sendNetworkInfo");
            if (WebSocket != null) WebSocket.SendString("{\"action\":\"netinfo\",\"netif2\":" + getNetworkInfo() + "}");
        }

        private void getUserImage(string userid)
        {
            Log(string.Format("getUserImage {0}", userid));
            if (WebSocket != null) WebSocket.SendString("{\"action\":\"getUserImage\",\"userid\":\"" + escapeJsonString(userid) + "\"}");
        }

        private string getNetworkInfo()
        {
            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
            string response = "{";
            bool firstItem1 = true;
            foreach (NetworkInterface adapter in adapters)
            {
                if (firstItem1 == false) { response += ","; }
                response += "\"" + escapeJsonString(adapter.Name) + "\":[";
                IPInterfaceProperties properties = adapter.GetIPProperties();
                int i = 0;
                bool firstItem2 = true;
                foreach (UnicastIPAddressInformation addr in properties.UnicastAddresses)
                {
                    if (firstItem2 == false) { response += ","; }
                    response += "{";
                    response += "\"address\":\"" + addr.Address.ToString() + "\",";
                    response += "\"fqdn\":\"" + properties.DnsSuffix + "\",";
                    if (addr.Address.AddressFamily == AddressFamily.InterNetwork) {
                        response += "\"family\":\"IPv4\",";
                        response += "\"index\":\"" + adapter.GetIPProperties().GetIPv4Properties().Index + "\",";
                    }
                    if (addr.Address.AddressFamily == AddressFamily.InterNetworkV6) {
                        response += "\"family\":\"IPv6\",";
                        response += "\"index\":\"" + adapter.GetIPProperties().GetIPv6Properties().Index + "\",";
                    }
                    response += "\"mac\":\"" + fixMacAddress(adapter.GetPhysicalAddress().ToString()) + "\",";
                    response += "\"type\":\"" + adapter.NetworkInterfaceType.ToString().ToLower() + "\",";
                    response += "\"status\":\"" + adapter.OperationalStatus.ToString().ToLower() + "\"";
                    response += "}";
                    firstItem2 = false;
                    i++;
                }
                response += "]";
                firstItem1 = false;
            }
            response += "}";
            Log("getNetworkInfo: " + response);
            return response;
        }

        public void processConsoleCommand(string rawcmd, string sessionid, string userid)
        {
            Log(string.Format("processConsoleCommand, cmd=\"{0}\", sessionid=\"{1}\", userid=\"{2}\"", rawcmd, sessionid, userid));
            Event(userid, string.Format("Console command: {0}", rawcmd));

            string response = null;
            string[] cmd = parseArgString(rawcmd);
            if (cmd.Length == 0) return;
            sendConsoleEventLog(rawcmd);

            switch (cmd[0])
            {
                case "help": {
                        response = "Available commands: \r\n" + "args, coreinfo, help, netinfo, notify, openurl, sysinfo" + ".";
                        break;
                    }
                case "sysinfo":
                    {
                        response = getSysInfo();
                        break;
                    }
                case "coreinfo":
                    {
                        response = GetCoreInfo();
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
                        if (cmd.Length != 2) {
                            response = "Usage: openurl [url]";
                        } else if ((!cmd[1].ToLower().StartsWith("http://")) && (!cmd[1].ToLower().StartsWith("https://"))) {
                            response = "Url must start with http:// or https://";
                        } else {
                            Process.Start(cmd[1]);
                            sendOpenUrlEventLog(cmd[1]); response = "Ok";
                        }
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
                if (WebSocket != null) WebSocket.SendString("{\"action\":\"msg\",\"type\":\"console\",\"value\":\"" + escapeJsonString(response) + "\",\"sessionid\":\"" + sessionid + "\"}");
            }
        }

        public void PrivacyTextChanged()
        {
            Log("PrivacyTextChanged");
            if (onUserInfoChange != null) { onUserInfoChange(null, 3); } // Event the privacy text change
        }

        public void processServerJsonData(string data)
        {
            Log("processServerJsonData: " + data);

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
                case "ping": { if (WebSocket != null) WebSocket.SendString("{\"action\":\"pong\"}"); break; }
                case "pong": { break; }
                case "errorlog": { break; }
                case "sysinfo": { break; }
                case "coredump": { break; }
                case "getcoredump": { break; }
                case "getUserImage":
                    {
                        if (userid == null) return;

                        // Get username
                        if (jsonAction.ContainsKey("name") && (jsonAction["name"].GetType() == typeof(string))) { usernames[userid] = (string)jsonAction["name"]; }

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
                        string url = (string)jsonAction["url"];
                        if ((!url.ToLower().StartsWith("http://")) && (!url.ToLower().StartsWith("https://"))) return;
                        Event(userid, string.Format("Opening URL: {0}", url));
                        sendOpenUrlEventLog(url);
                        Process.Start(url);
                        //parent.OpenBrowser(url);
                        break;
                    }
                case "msg": {
                        if (!jsonAction.ContainsKey("type") || (jsonAction["type"].GetType() != typeof(string))) return;
                        string eventType = jsonAction["type"].ToString();
                        switch (eventType)
                        {
                            case "cpuinfo":
                                {
                                    // Continue sysinfo on a different thread
                                    Thread t = new Thread(new ParameterizedThreadStart(taskcpuinfo));
                                    t.Start(jsonAction);
                                    break;
                                }
                            case "sysinfo":
                                {
                                    // Continue sysinfo on a different thread
                                    Thread t = new Thread(new ParameterizedThreadStart(tasksysinfo));
                                    t.Start(jsonAction);
                                    break;
                                }
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
                                            if (cmdvalue == "cancelhelp") {
                                                if (parent.autoConnect == false) {
                                                    disconnect();
                                                } else {
                                                    HelpRequest = null;
                                                    sendSessionUpdate("help", "{}"); // Clear help request
                                                    if (onStateChanged != null) { onStateChanged(state); }
                                                }
                                            }
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
                                        string title = (string)jsonAction["title"];
                                        string message = (string)jsonAction["msg"];
                                        Event(userid, string.Format("Message box: {0}, {1}", title, message));
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
                                        if ((jsonAction.ContainsKey("tcpaddr")) && (jsonAction["tcpaddr"].GetType() == typeof(string)) && (jsonAction.ContainsKey("tcpport")) && ((jsonAction["tcpport"].GetType() == typeof(int)) || (jsonAction["tcpport"].GetType() == typeof(string))))
                                        {
                                            // TCP relay tunnel
                                            try
                                            {
                                                string tcpaddr = (string)jsonAction["tcpaddr"];
                                                int tcpport = 0;
                                                if (jsonAction["tcpport"].GetType() == typeof(int)) { tcpport = (int)jsonAction["tcpport"]; }
                                                if (jsonAction["tcpport"].GetType() == typeof(string)) { tcpport = int.Parse((string)jsonAction["tcpport"]); }
                                                string url = (string)jsonAction["value"];
                                                string hash = null;
                                                if ((jsonAction.ContainsKey("servertlshash")) && (jsonAction["servertlshash"].GetType() == typeof(string))) { hash = (string)jsonAction["servertlshash"]; }
                                                if (url.StartsWith("*/")) { url = "wss://" + ServerUrl.Authority + url.Substring(1); }
                                                Event(userid, string.Format("Started TCP tunnel to {0}:{1}", tcpaddr, tcpport));
                                                MeshCentralTcpTunnel tunnel = new MeshCentralTcpTunnel(this, new Uri(url), hash, jsonAction, tcpaddr, tcpport);
                                                tcptunnels.Add(tunnel);
                                            }
                                            catch (Exception) { }
                                        }
                                        else
                                        {
                                            // Application tunnel
                                            try
                                            {
                                                string url = jsonAction["value"].ToString();
                                                string hash = null;
                                                if ((jsonAction.ContainsKey("servertlshash")) && (jsonAction["servertlshash"].GetType() == typeof(string))) { hash = (string)jsonAction["servertlshash"]; }
                                                if (url.StartsWith("*/")) { url = "wss://" + ServerUrl.Authority + url.Substring(1); }
                                                MeshCentralTunnel tunnel = new MeshCentralTunnel(this, new Uri(url), hash, jsonAction);
                                                tunnels.Add(tunnel);
                                            }
                                            catch (Exception) { }
                                        }
                                    }
                                    break;
                                }
                            case "getclip": {
                                    // Require that the user have an active remote desktop session to perform this operation.
                                    if ((userid == null) || (doesUserHaveSession(userid, 2) == false)) return;
                                    Event(userid, "Requested clipboard content");
                                    GetClipboard(jsonAction);
                                    break;
                                }
                            case "setclip": {
                                    // Require that the user have an active remote desktop session to perform this operation.
                                    if ((userid == null) || (doesUserHaveSession(userid, 2) == false)) return;
                                    Event(userid, "Set clipboard content");
                                    SetClipboard(jsonAction);
                                    break;
                                }
                            case "ps": {
                                    // Continue ps on a different thread
                                    Thread t = new Thread(new ParameterizedThreadStart(taskps));
                                    t.Start(jsonAction);
                                    break;
                                }
                            case "psinfo":
                                {
                                    // Continue psinfo on a different thread
                                    Thread t = new Thread(new ParameterizedThreadStart(taskpsinfo));
                                    t.Start(jsonAction);
                                    break;
                                }
                            case "pskill":
                                {
                                    int pid = 0;
                                    if ((jsonAction.ContainsKey("value")) && (jsonAction["value"].GetType() == typeof(System.Int32))) { pid = (int)jsonAction["value"]; }
                                    if (pid > 0) { try { Process p = Process.GetProcessById(pid); if (p != null) { p.Kill(); } } catch (Exception) { } }
                                    Event(userid, string.Format("Killed process {0}", pid));
                                    break;
                                }
                            case "services":
                                {
                                    // TODO: List system services
                                    break;
                                }
                            case "guestShare":
                                {
                                    if (jsonAction.ContainsKey("url") && (jsonAction["url"] != null) && (jsonAction["url"].GetType() == typeof(string)))
                                    {
                                        selfSharingUrl = (string)jsonAction["url"];
                                        if (jsonAction.ContainsKey("flags") && (jsonAction["flags"] != null) && (jsonAction["flags"].GetType() == typeof(int))) { selfSharingFlags = (int)jsonAction["flags"]; }
                                        if (jsonAction.ContainsKey("viewOnly") && (jsonAction["viewOnly"] != null) && (jsonAction["viewOnly"].GetType() == typeof(bool))) { selfSharingViewOnly = (bool)jsonAction["viewOnly"]; }
                                    }
                                    else
                                    {
                                        selfSharingUrl = null;
                                        selfSharingFlags = 0;
                                    }
                                    if (onSelfSharingStatus != null) { onSelfSharingStatus(selfSharingAllowed, selfSharingUrl); }
                                    break;
                                }
                            default:
                                {
                                    Log("Unprocessed event type: " + eventType);
                                    break;
                                }
                        }
                        break;
                    }
                case "meshToolInfo":
                    {
                        string name = null;
                        string hash = null;
                        string url = null;
                        string serverhash = null;
                        if (jsonAction.ContainsKey("name")) { name = jsonAction["name"].ToString(); } // Download tool name
                        if (jsonAction.ContainsKey("hash")) { hash = jsonAction["hash"].ToString(); } // File Hash
                        if (jsonAction.ContainsKey("url")) { url = jsonAction["url"].ToString(); } // Server url
                        if (jsonAction.ContainsKey("serverhash")) { serverhash = jsonAction["serverhash"].ToString(); } // Server TLS certificate hash
                        if ((name != null) && (hash != null) && (url != null) && (onSelfUpdate != null) && (name == softwareName)) {
                            if (url.StartsWith("*/")) { url = "https://" + ServerUrl.Authority + url.Substring(1); }
                            url += ("&meshid=" + MeshIdMB64 + "&ac=" + autoConnectFlags);
                            if ((msh == null) || (!msh.ContainsKey("disableUpdate") && !msh.ContainsKey("DisableUpdate"))) { // Only self-update if .msh allows it
                                onSelfUpdate(name, hash, url, serverhash);
                            }
                        }
                        break;
                    }
                case "wget":
                    {
                        // Download a file from URL
                        string urlpath = null;
                        string path = null;
                        string folder = null;
                        string servertlshash = null;
                        bool overwrite = false;
                        bool createFolder = false;
                        if (jsonAction.ContainsKey("urlpath")) { urlpath = jsonAction["urlpath"].ToString(); }
                        if (jsonAction.ContainsKey("path")) { path = jsonAction["path"].ToString(); }
                        if (jsonAction.ContainsKey("folder")) { folder = jsonAction["folder"].ToString(); }
                        if (jsonAction.ContainsKey("servertlshash")) { servertlshash = jsonAction["servertlshash"].ToString(); }
                        if (jsonAction.ContainsKey("overwrite") && (jsonAction["overwrite"].GetType() == typeof(Boolean))) { overwrite = (Boolean)jsonAction["overwrite"]; }
                        if (jsonAction.ContainsKey("createFolder") && (jsonAction["createFolder"].GetType() == typeof(Boolean))) { createFolder = (Boolean)jsonAction["createFolder"]; }
                        if ((urlpath == null) || (path == null) || (folder == null) || (servertlshash == null) || (userid == null)) break;

                        // Check if user consent is needed
                        if (autoConnect == true) return; // TODO: Do user consent.

                        // Check if the folder exists, create it if asked to do so.
                        if (!Directory.Exists(folder))
                        {
                            if (createFolder == false) return;
                            try { Directory.CreateDirectory(folder); } catch (Exception) { }
                            if (!Directory.Exists(folder)) return;
                        }

                        // Check if the file exists
                        if (File.Exists(path))
                        {
                            if (overwrite == false) return;
                            try { File.Delete(path); } catch (Exception) { }
                            if (File.Exists(path)) return;
                        }

                        // Log the event
                        parent.Event(userid, string.Format("Uploaded file {0}", path));

                        // Setup and perform file download
                        string dlurl = ServerUrl.AbsoluteUri.Substring(0, ServerUrl.AbsoluteUri.Length - ServerUrl.PathAndQuery.Length) + urlpath;
                        HttpFileDownload httpget = new HttpFileDownload(this, dlurl, path, servertlshash);
                        fileDownloads.Add(httpget);
                        httpget.Start();

                        break;
                    }
                case "wakeonlan":
                    {
                        // Send wake-on-lan on all interfaces for all MAC addresses in data.macs array. The array is a list of HEX MAC addresses.
                        if (jsonAction.ContainsKey("macs") && (jsonAction["macs"].GetType() == typeof(ArrayList))) {
                            int added = 0;
                            ArrayList macs = (ArrayList)jsonAction["macs"];
                            foreach (object x in macs) { if (x.GetType() == typeof(string)) { pendingWakeOnLan.Add(((string)x).Replace(":", "")); added++; } }
                            if ((added > 0) && (pendingWakeOnLanTimer == null)) { pendingWakeOnLanTimer = new System.Threading.Timer(new System.Threading.TimerCallback(sendNextWakeOnLanPacket), null, 100, 100); }
                        }
                        break;
                    }
                case "serverInfo":
                    {
                        if (jsonAction.ContainsKey("agentSelfGuestSharing") && (jsonAction["agentSelfGuestSharing"].GetType() == typeof(Boolean))) {
                            if (selfSharingAllowed != (Boolean)jsonAction["agentSelfGuestSharing"])
                            {
                                selfSharingAllowed = (Boolean)jsonAction["agentSelfGuestSharing"];
                                if (onSelfSharingStatus != null) { onSelfSharingStatus(selfSharingAllowed, selfSharingUrl); }
                            }
                        }
                        break;
                    }
                case "guestShare":
                    {
                        if (jsonAction.ContainsKey("url") && (jsonAction["url"] != null) && (jsonAction["url"].GetType() == typeof(string)))
                        {
                            selfSharingUrl = (string)jsonAction["url"];
                            if (jsonAction.ContainsKey("flags") && (jsonAction["flags"] != null) && (jsonAction["flags"].GetType() == typeof(int))) { selfSharingFlags = (int)jsonAction["flags"]; }
                            if (jsonAction.ContainsKey("viewOnly") && (jsonAction["viewOnly"] != null) && (jsonAction["viewOnly"].GetType() == typeof(bool))) { selfSharingViewOnly = (bool)jsonAction["viewOnly"]; }
                        }
                        else {
                            selfSharingUrl = null;
                            selfSharingFlags = 0;
                        }
                        if (onSelfSharingStatus != null) { onSelfSharingStatus(selfSharingAllowed, selfSharingUrl); }
                        break;
                    }
                default:
                    {
                        Log("Unprocessed command: " + action);
                        break;
                    }
            }
        }

        private void taskps(object obj)
        {
            Dictionary<string, object> jsonAction = (Dictionary<string, object>)obj;
            string sessionid = null;
            if ((jsonAction.ContainsKey("sessionid")) && (jsonAction["sessionid"].GetType() == typeof(string))) { sessionid = (string)jsonAction["sessionid"]; }
            string ps = getAllProcesses();
            if (WebSocket != null) WebSocket.SendString("{\"action\":\"msg\",\"type\":\"ps\",\"sessionid\":\"" + escapeJsonString(sessionid) + "\",\"value\":\"" + escapeJsonString(ps) + "\"}");
        }

        private void taskcpuinfo(object obj)
        {
            Dictionary<string, object> jsonAction = (Dictionary<string, object>)obj;

            if ((!jsonAction.ContainsKey("sessionid")) && (jsonAction["sessionid"].GetType() != typeof(string))) return;
            string sessionid = (string)jsonAction["sessionid"];
            string tag = null;
            if ((jsonAction.ContainsKey("tag")) && (jsonAction["tag"].GetType() == typeof(string))) { tag = (string)jsonAction["tag"]; }

            try
            {
                string r = "{\"action\":\"msg\",\"type\":\"cpuinfo\",\"sessionid\":\"" + escapeJsonString(sessionid) + "\"";
                if (tag != null) { r += "\"tag\":\"" + escapeJsonString(tag) + "\""; }
                r += ",\"cpu\":{\"total\":" + cpuCounter.NextValue() + "}";
                r += ",\"memory\":{\"percentConsumed\":" + GetUsedMemory() + "}";
                try
                {
                    Dictionary<string, double> temps = new Dictionary<string, double>();
                    using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(@"root\WMI", "SELECT * FROM MSAcpi_ThermalZoneTemperature"))
                    {
                        foreach (ManagementObject queryObj in searcher.Get())
                        {
                            double temperature = Convert.ToDouble(queryObj["CurrentTemperature"].ToString());
                            // Convert the value to celsius degrees
                            temps.Add(queryObj["InstanceName"].ToString(), (temperature - 2732) / 10.0);
                        }
                    }
                    if (temps.Count > 0)
                    {
                        r += ",\"thermals\":[";
                        bool first = true;
                        foreach (string key in temps.Keys) { if (first) { first = false; } else { r += ","; } r += "\"" + temps[key] + "\""; }
                        r += "]";
                    }
                }
                catch (Exception) { }

                r += "}";
                if (WebSocket != null) WebSocket.SendString(r);
            }
            catch (Exception) { }
        }

        private void tasksysinfo(object obj)
        {
            Dictionary<string, object> jsonAction = (Dictionary<string, object>)obj;
            if ((!jsonAction.ContainsKey("sessionid")) && (jsonAction["sessionid"].GetType() != typeof(string))) return;
            string sessionid = (string)jsonAction["sessionid"];
            string tag = null;
            if ((jsonAction.ContainsKey("tag")) && (jsonAction["tag"].GetType() == typeof(string))) { tag = (string)jsonAction["tag"]; }
            sendSysInfoMsg(sessionid, tag);
        }

        private void taskpsinfo(object obj)
        {
            Dictionary<string, object> jsonAction = (Dictionary < string, object> )obj;

            string sessionid = null;
            if ((jsonAction.ContainsKey("sessionid")) && (jsonAction["sessionid"].GetType() == typeof(string))) { sessionid = (string)jsonAction["sessionid"]; }
            int pid = -1;
            if ((jsonAction.ContainsKey("pid")) && (jsonAction["pid"].GetType() == typeof(int))) { pid = (int)jsonAction["pid"]; }
            if (pid < 0) return;
            Process p = Process.GetProcessById(pid);
            if (p == null)
            {
                if (WebSocket != null) WebSocket.SendString("{\"action\":\"msg\",\"type\":\"psinfo\",\"sessionid\":\"" + escapeJsonString(sessionid) + "\",\"pid\":" + pid + ",\"value\":null}");
            }
            else
            {
                string processUser = null;
                string processDomain = null;
                string processCmd = null;
                try { GetProcessOwnerByProcessId(pid, out processUser, out processDomain, out processCmd); } catch (Exception) { }
                string x = "";
                try { x += "\"processName\":\"" + escapeJsonString(p.ProcessName) + "\""; } catch (Exception) { }
                try { if (processUser != null) { x += ",\"processUser\":\"" + escapeJsonString(processUser) + "\""; } } catch (Exception) { }
                try { if (processDomain != null) { x += ",\"processDomain\":\"" + escapeJsonString(processDomain) + "\""; } } catch (Exception) { }
                try { if (processCmd != null) { x += ",\"cmd\":\"" + escapeJsonString(processCmd) + "\""; } } catch (Exception) { }
                try { x += ",\"privateMemorySize\":" + p.PrivateMemorySize64.ToString(); } catch (Exception) { }
                try { x += ",\"virtualMemorySize\":" + p.VirtualMemorySize64.ToString(); } catch (Exception) { }
                try { x += ",\"workingSet\":" + p.WorkingSet64.ToString(); } catch (Exception) { }
                try { x += ",\"totalProcessorTime\":" + p.TotalProcessorTime.TotalSeconds.ToString(); } catch (Exception) { }
                try { x += ",\"userProcessorTime\":" + p.UserProcessorTime.TotalSeconds.ToString(); } catch (Exception) { }
                try { x += ",\"startTime\":\"" + escapeJsonString(p.StartTime.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")) + "\""; } catch (Exception) { }
                try { x += ",\"sessionId\":" + p.SessionId.ToString(); } catch (Exception) { }
                try { x += ",\"privilegedProcessorTime\":" + p.PrivilegedProcessorTime.TotalSeconds.ToString(); } catch (Exception) { }
                try { if (p.PriorityBoostEnabled) { x += ",\"PriorityBoostEnabled\":true"; } } catch (Exception) { }
                try { x += ",\"peakWorkingSet\":" + p.PeakWorkingSet64.ToString(); } catch (Exception) { }
                try { x += ",\"peakVirtualMemorySize\":" + p.PeakVirtualMemorySize64.ToString(); } catch (Exception) { }
                try { x += ",\"peakPagedMemorySize\":" + p.PeakPagedMemorySize64.ToString(); } catch (Exception) { }
                try { x += ",\"pagedSystemMemorySize\":" + p.PagedSystemMemorySize64.ToString(); } catch (Exception) { }
                try { x += ",\"pagedMemorySize\":" + p.PagedMemorySize64.ToString(); } catch (Exception) { }
                try { x += ",\"nonpagedSystemMemorySize\":" + p.NonpagedSystemMemorySize64.ToString(); } catch (Exception) { }
                try { x += ",\"mainWindowTitle\":\"" + escapeJsonString(p.MainWindowTitle) + "\""; } catch (Exception) { }
                try { if ((p.MachineName != null) && (p.MachineName != ".")) { x += ",\"machineName\":\"" + escapeJsonString(p.MachineName) + "\""; } } catch (Exception) { }
                try { x += ",\"handleCount\":" + p.HandleCount.ToString(); } catch (Exception) { }
                if (WebSocket != null) WebSocket.SendString("{\"action\":\"msg\",\"type\":\"psinfo\",\"sessionid\":\"" + escapeJsonString(sessionid) + "\",\"pid\":" + pid + ",\"value\":{" + x + "}}");
            }
        }

        private async void sendNextWakeOnLanPacket(object state)
        {
            string mac = null;
            lock (pendingWakeOnLan) { if (pendingWakeOnLan.Count > 0) { mac = pendingWakeOnLan[0]; pendingWakeOnLan.RemoveAt(0); } }
            if (mac != null) { await WakeOnLan(mac); }
            if ((pendingWakeOnLan.Count == 0) && (pendingWakeOnLanTimer != null)) { pendingWakeOnLanTimer.Dispose(); pendingWakeOnLanTimer = null; }
        }

        private void sendSysInfoMsg(string sessionid, string tag)
        {
            try
            {
                string sysinfo = getSysInfo();
                if (sysinfo == null) return;
                string r = "{\"action\":\"msg\",\"type\":\"sysinfo\"";
                if (sessionid != null) { r += ",\"sessionid\":\"" + escapeJsonString(sessionid) + "\""; }
                if (tag != null) { r += ",\"tag\":\"" + escapeJsonString(tag) + "\""; }
                r += ",\"data\":" + sysinfo + "}";
                if (WebSocket != null) WebSocket.SendString(r);
            }
            catch (Exception) { }
        }

        private void sendSysInfo(string sessionid, string tag)
        {
            try
            {
                string sysinfo = getSysInfo();
                if (sysinfo == null) return;
                if (WebSocket != null) WebSocket.SendString("{\"action\":\"sysinfo\",\"data\":" + sysinfo + "}");
            }
            catch (Exception) { }
        }

        private string addQueryValue(ManagementObject queryObj, string vname, string name, ref bool qfirst)
        {
            if (queryObj[name] == null) return "";
            string r = "";
            if (qfirst) { qfirst = false; } else { r = ","; }
            return r + "\"" + vname + "\":\"" + escapeJsonString(queryObj[name].ToString()) + "\"";
        }

        private string getSysInfo()
        {
            bool first;
            bool qfirst;
            try
            {
                string r = "{";
                r += "\"hardware\":{";
                r += "\"windows\":{";
                r += "\"memory\":[";
                first = true;
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_PhysicalMemory"))
                {
                    foreach (ManagementObject queryObj in searcher.Get())
                    {
                        if (first) { first = false; } else { r += ","; }
                        r += "{";
                        qfirst = true;
                        r += addQueryValue(queryObj, "BankLabel", "BankLabel", ref qfirst);
                        r += addQueryValue(queryObj, "Capacity", "Capacity", ref qfirst);
                        r += addQueryValue(queryObj, "DataWidth", "DataWidth", ref qfirst);
                        r += addQueryValue(queryObj, "Description", "Description", ref qfirst);
                        r += addQueryValue(queryObj, "DeviceLocator", "DeviceLocator", ref qfirst);
                        r += addQueryValue(queryObj, "FormFactor", "FormFactor", ref qfirst);
                        r += addQueryValue(queryObj, "Manufacturer", "Manufacturer", ref qfirst);
                        r += addQueryValue(queryObj, "MemoryType", "MemoryType", ref qfirst);
                        r += addQueryValue(queryObj, "Name", "Name", ref qfirst);
                        r += addQueryValue(queryObj, "PartNumber", "PartNumber", ref qfirst);
                        r += addQueryValue(queryObj, "SerialNumber", "SerialNumber", ref qfirst);
                        r += addQueryValue(queryObj, "Speed", "Speed", ref qfirst);
                        r += addQueryValue(queryObj, "Tag", "Tag", ref qfirst);
                        r += addQueryValue(queryObj, "TotalWidth", "TotalWidth", ref qfirst);
                        r += addQueryValue(queryObj, "TypeDetail", "TypeDetail", ref qfirst);
                        r += "}";
                    }
                }
                r += "]"; // memory
                r += ",\"osinfo\":{";
                first = true;
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_OperatingSystem"))
                {
                    foreach (ManagementObject queryObj in searcher.Get())
                    {
                        if (first) { first = false; } else { continue; }
                        qfirst = true;
                        r += addQueryValue(queryObj, "BootDevice", "BootDevice", ref qfirst);
                        r += addQueryValue(queryObj, "BuildNumber", "BuildNumber", ref qfirst);
                        r += addQueryValue(queryObj, "BuildType", "BuildType", ref qfirst);
                        r += addQueryValue(queryObj, "Caption", "Caption", ref qfirst);
                        r += addQueryValue(queryObj, "CodeSet", "CodeSet", ref qfirst);
                        r += addQueryValue(queryObj, "CountryCode", "CountryCode", ref qfirst);
                        r += addQueryValue(queryObj, "CreationClassName", "CreationClassName", ref qfirst);
                        r += addQueryValue(queryObj, "CSCreationClassName", "CSCreationClassName", ref qfirst);
                        r += addQueryValue(queryObj, "CSName", "CSName", ref qfirst);
                        r += addQueryValue(queryObj, "CurrentTimeZone", "CurrentTimeZone", ref qfirst);
                        r += addQueryValue(queryObj, "DataExecutionPrevention_32BitApplications", "DataExecutionPrevention_32BitApplications", ref qfirst);
                        r += addQueryValue(queryObj, "DataExecutionPrevention_Available", "DataExecutionPrevention_Available", ref qfirst);
                        r += addQueryValue(queryObj, "DataExecutionPrevention_Drivers", "DataExecutionPrevention_Drivers", ref qfirst);
                        r += addQueryValue(queryObj, "DataExecutionPrevention_SupportPolicy", "DataExecutionPrevention_SupportPolicy", ref qfirst);
                        r += addQueryValue(queryObj, "Debug", "Debug", ref qfirst);
                        r += addQueryValue(queryObj, "Distributed", "Distributed", ref qfirst);
                        r += addQueryValue(queryObj, "EncryptionLevel", "EncryptionLevel", ref qfirst);
                        r += addQueryValue(queryObj, "ForegroundApplicationBoost", "ForegroundApplicationBoost", ref qfirst);
                        r += addQueryValue(queryObj, "InstallDate", "InstallDate", ref qfirst);
                        r += addQueryValue(queryObj, "LastBootUpTime", "LastBootUpTime", ref qfirst);
                        r += addQueryValue(queryObj, "Locale", "Locale", ref qfirst);
                        r += addQueryValue(queryObj, "Manufacturer", "Manufacturer", ref qfirst);
                        r += addQueryValue(queryObj, "MaxNumberOfProcesses", "MaxNumberOfProcesses", ref qfirst);
                        r += addQueryValue(queryObj, "MUILanguages", "MUILanguages", ref qfirst);
                        r += addQueryValue(queryObj, "Name", "Name", ref qfirst);
                        r += addQueryValue(queryObj, "NumberOfLicensedUsers", "NumberOfLicensedUsers", ref qfirst);
                        r += addQueryValue(queryObj, "NumberOfProcesses", "NumberOfProcesses", ref qfirst);
                        r += addQueryValue(queryObj, "NumberOfUsers", "NumberOfUsers", ref qfirst);
                        r += addQueryValue(queryObj, "OperatingSystemSKU", "OperatingSystemSKU", ref qfirst);
                        r += addQueryValue(queryObj, "OSArchitecture", "OSArchitecture", ref qfirst);
                        r += addQueryValue(queryObj, "OSLanguage", "OSLanguage", ref qfirst);
                        r += addQueryValue(queryObj, "OSProductSuite", "OSProductSuite", ref qfirst);
                        r += addQueryValue(queryObj, "OSType", "OSType", ref qfirst);
                        r += addQueryValue(queryObj, "PortableOperatingSystem", "PortableOperatingSystem", ref qfirst);
                        r += addQueryValue(queryObj, "Primary", "Primary", ref qfirst);
                        r += addQueryValue(queryObj, "ProductType", "ProductType", ref qfirst);
                        r += addQueryValue(queryObj, "SerialNumber", "SerialNumber", ref qfirst);
                        r += addQueryValue(queryObj, "ServicePackMajorVersion", "ServicePackMajorVersion", ref qfirst);
                        r += addQueryValue(queryObj, "ServicePackMinorVersion", "ServicePackMinorVersion", ref qfirst);
                        r += addQueryValue(queryObj, "SizeStoredInPagingFiles", "SizeStoredInPagingFiles", ref qfirst);
                        r += addQueryValue(queryObj, "Status", "Status", ref qfirst);
                        r += addQueryValue(queryObj, "SuiteMask", "SuiteMask", ref qfirst);
                        r += addQueryValue(queryObj, "SystemDevice", "SystemDevice", ref qfirst);
                        r += addQueryValue(queryObj, "SystemDirectory", "SystemDirectory", ref qfirst);
                        r += addQueryValue(queryObj, "SystemDrive", "SystemDrive", ref qfirst);
                        r += addQueryValue(queryObj, "Version", "Version", ref qfirst);
                        r += addQueryValue(queryObj, "WindowsDirectory", "WindowsDirectory", ref qfirst);
                    }
                }
                r += "}"; // osinfo
                r += ",\"cpu\":[";
                first = true;
                string firstCpuName = null;
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_Processor"))
                {
                    foreach (ManagementObject queryObj in searcher.Get())
                    {
                        if (first) { first = false; } else { r += ","; }
                        r += "{";
                        qfirst = true;
                        if (queryObj["Name"] != null) { firstCpuName = queryObj["Name"].ToString(); }
                        r += addQueryValue(queryObj, "Caption", "Caption", ref qfirst);
                        r += addQueryValue(queryObj, "DeviceID", "DeviceID", ref qfirst);
                        r += addQueryValue(queryObj, "Manufacturer", "Manufacturer", ref qfirst);
                        r += addQueryValue(queryObj, "MaxClockSpeed", "MaxClockSpeed", ref qfirst);
                        r += addQueryValue(queryObj, "Name", "Name", ref qfirst);
                        r += addQueryValue(queryObj, "SocketDesignation", "SocketDesignation", ref qfirst);
                        r += "}";
                    }
                }
                r += "]"; // cpu
                r += "},"; // windows
                r += "\"identifiers\":{";

                Dictionary<string, string> identifiers = new Dictionary<string, string>();
                if (firstCpuName != null) { identifiers.Add("cpu_name", firstCpuName); }
                first = true;
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM CIM_BIOSElement"))
                {
                    foreach (ManagementObject queryObj in searcher.Get())
                    {
                        if (first) { first = false; } else { continue; }
                        if (queryObj["ReleaseDate"] != null) { identifiers.Add("bios_date", queryObj["ReleaseDate"].ToString()); }
                        if (queryObj["Manufacturer"] != null) { identifiers.Add("bios_vendor", queryObj["Manufacturer"].ToString()); }
                        if (queryObj["Version"] != null) { identifiers.Add("bios_version", queryObj["SMBIOSBIOSVersion"].ToString()); }
                    }
                }
                first = true;
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_BaseBoard"))
                {
                    foreach (ManagementObject queryObj in searcher.Get())
                    {
                        if (first) { first = false; } else { continue; }
                        if (queryObj["Product"] != null) { identifiers.Add("board_name", queryObj["Product"].ToString()); }
                        if (queryObj["SerialNumber"] != null) { identifiers.Add("board_serial", queryObj["SerialNumber"].ToString()); }
                        if (queryObj["Manufacturer"] != null) { identifiers.Add("board_vendor", queryObj["Manufacturer"].ToString()); }
                        if (queryObj["Version"] != null) { identifiers.Add("board_version", queryObj["Version"].ToString()); }
                    }
                }
                first = true;
                foreach (string key in identifiers.Keys)
                {
                    if (first) { first = false; } else { r += ","; }
                    r += "\"" + key + "\":\"" + escapeJsonString(identifiers[key]) + "\"";
                }

                List<string> gpuNames = new List<string>();
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_VideoController"))
                {
                    foreach (ManagementObject queryObj in searcher.Get())
                    {
                        if (queryObj["Name"] != null) { gpuNames.Add(queryObj["Name"].ToString()); }
                    }
                }
                if (gpuNames.Count > 0)
                {
                    r += ",\"gpu_name\":[";
                    first = true;
                    foreach (string n in gpuNames)
                    {
                        if (first) { first = false; } else { r += ","; }
                        r += "\"" + escapeJsonString(n) + "\"";
                    }
                    r += "]";
                }

                r += ",\"storage_devices\":[";
                first = true;
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_DiskDrive"))
                {
                    foreach (ManagementObject queryObj in searcher.Get())
                    {
                        if (queryObj["Size"] == null) continue;
                        if (first) { first = false; } else { r += ","; }
                        r += "{";
                        qfirst = true;
                        r += addQueryValue(queryObj, "Caption", "Model", ref qfirst);
                        r += addQueryValue(queryObj, "Size", "Size", ref qfirst);
                        r += addQueryValue(queryObj, "Serial", "SerialNumber", ref qfirst);
                        r += "}";
                    }
                }
                r += "]"; // storage_devices
                r += "}"; // identifiers
                r += "}"; // hardware
                string hash = null;
                using (SHA384 shaM = new SHA384Managed()) { hash = Convert.ToBase64String(shaM.ComputeHash(UTF8Encoding.UTF8.GetBytes(r))); }
                r += ",\"hash\":\"" + hash + "\"";
                r += "}"; // start
                return r;
            }
            catch (Exception ex) {
                try { File.AppendAllText("debug.log", DateTime.Now.ToString("HH:mm:tt.ffff") + ": MCAgent: " + ex.ToString() + "\r\n"); } catch (Exception) { }
            }
            return null;
        }

        private string getAllProcesses()
        {
            Log("getAllProcesses()");
            string r = "{";
            using (ManagementObjectSearcher Processes = new ManagementObjectSearcher("SELECT * FROM Win32_Process"))
            {
                bool first = true;
                foreach (ManagementObject p in Processes.Get())
                {
                    if (p["ProcessId"].ToString() == "0") continue;
                    string rr = null;
                    try
                    {
                        if (first) { first = false; } else { r += ","; }
                        string ppath = "";
                        if (p["CommandLine"] != null) { ppath = ",\"path\":\"" + escapeJsonString(p["CommandLine"].ToString()) + "\""; }
                        rr = "\"" + p["ProcessId"].ToString() + "\":{\"pid\":" + p["ProcessId"].ToString() + ",\"cmd\":\"" + escapeJsonString(p["Description"].ToString()) + "\"" + ppath + "}";
                    }
                    catch (Exception) { }
                    if (rr != null) { r += rr; }
                }
            }
            return r + "}";
        }

        private delegate void ClipboardHandler(Dictionary<string, object> jsonAction);

        private void GetClipboard(Dictionary<string, object> jsonAction)
        {
            Log("GetClipboard()");

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
                    if (WebSocket != null) WebSocket.SendString("{\"action\":\"log\",\"msgid\":21,\"msgArgs\":[" + clipboardValue.Length + "],\"msg\":\"Getting clipboard content, " + clipboardValue.Length + " byte(s)\"" + extraLogStr + "}");
                    if (WebSocket != null) WebSocket.SendString("{\"action\":\"msg\",\"type\":\"getclip\",\"sessionid\":\"" + escapeJsonString(sessionid) + "\",\"data\":\"" + escapeJsonString(clipboardValue) + "\",\"tag\":" + tag + "}");
                }
            }
        }

        private void SetClipboard(Dictionary<string, object> jsonAction)
        {
            Log("SetClipboard()");

            // Clipboard can only be set from the main thread
            if (parent.InvokeRequired) { parent.Invoke(new ClipboardHandler(SetClipboard), jsonAction); return; }
            if ((jsonAction.ContainsKey("sessionid")) && (jsonAction["sessionid"].GetType() == typeof(string)) && (jsonAction.ContainsKey("data")) && (jsonAction["data"].GetType() == typeof(string)))
            {
                string extraLogStr = "";
                if (jsonAction.ContainsKey("userid") && (jsonAction["userid"].GetType() == typeof(string))) { extraLogStr += ",\"userid\":\"" + escapeJsonString((string)jsonAction["userid"]) + "\""; }
                if (jsonAction.ContainsKey("username") && (jsonAction["username"].GetType() == typeof(string))) { extraLogStr += ",\"username\":\"" + escapeJsonString((string)jsonAction["username"]) + "\""; }
                if (jsonAction.ContainsKey("remoteaddr") && (jsonAction["remoteaddr"].GetType() == typeof(string))) { extraLogStr += ",\"remoteaddr\":\"" + escapeJsonString((string)jsonAction["remoteaddr"]) + "\""; }
                if (jsonAction.ContainsKey("sessionid") && (jsonAction["sessionid"].GetType() == typeof(string))) { extraLogStr += ",\"sessionid\":\"" + escapeJsonString((string)jsonAction["sessionid"]) + "\""; }

                bool ok = false;
                if (jsonAction.ContainsKey("data") && (jsonAction["data"].GetType() == typeof(string)))
                {
                    try
                    {
                        Clipboard.SetText((string)jsonAction["data"]);
                        ok = true;
                    }
                    catch (Exception) { }
                }

                if (WebSocket != null)
                {
                    string sessionid = (string)jsonAction["sessionid"];
                    if (ok)
                    {
                        WebSocket.SendString("{\"action\":\"msg\",\"type\":\"setclip\",\"sessionid\":\"" + escapeJsonString(sessionid) + "\",\"success\":true}");
                    }
                    else
                    {
                        WebSocket.SendString("{\"action\":\"msg\",\"type\":\"setclip\",\"sessionid\":\"" + escapeJsonString(sessionid) + "\",\"success\":false}");
                    }
                }
            }
        }

        static bool ByteArrayCompare(byte[] a, byte[] b)
        {
            if (a.Length != b.Length) return false;
            for (var i = 0; i < a.Length; i++) { if (a[i] != b[i]) return false; }
            return true;
        }

        private void WebSocket_onStringData(webSocketClient sender, string data, int orglen)
        {
            // Process JSON data
            if ((ConnectionState == 15) && (data.Length > 2) && (data[0] == '{')) { processServerJsonData(data); }
        }

        private void sendAgentTag()
        {
            BinaryWriter bw = new BinaryWriter(new MemoryStream());
            bw.Write(Convert.ToInt16(IPAddress.HostToNetworkOrder((short)15))); // MeshCommand_AgentTag
            if ((agentTag != null) && (agentTag.Length > 0)) bw.Write(UTF8Encoding.UTF8.GetBytes(agentTag)); // Agent tag in UTF8 format
            if (WebSocket != null) WebSocket.SendBinary(((MemoryStream)bw.BaseStream).ToArray());
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
            Log(string.Format("Binary command: cmd={0}, len={1}", cmd, len));
            switch (cmd) {
                case 1:
                    {
                        // 0x0001 + TlsHash(48) + ServerNonce(48)
                        if (len != 98) return;
                        byte[] tlsHash = new byte[48];
                        Array.Copy(data, off + 2, tlsHash, 0, 48);
                        ServerNonce = new byte[48];
                        Array.Copy(data, off + 50, ServerNonce, 0, 48);
                        if (ByteArrayCompare(tlsHash, ServerTlsHash) == false) { disconnectex(); return; }

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
                        if (WebSocket != null) WebSocket.SendBinary(((MemoryStream)bw.BaseStream).ToArray());
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
                        if (serverCert.GetRSAPublicKey().VerifyData(dataToVerify, serverSign, HashAlgorithmName.SHA384, RSASignaturePadding.Pkcs1) == false) { disconnectex(); return; }

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
                        if (WebSocket != null) WebSocket.SendBinary(((MemoryStream)bw.BaseStream).ToArray());

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
                        Log("Unprocessed command: #" + cmd);
                        break;
                    }
            }
        }


        [DllImport("psapi.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetPerformanceInfo([Out] out PerformanceInformation PerformanceInformation, [In] int Size);

        [StructLayout(LayoutKind.Sequential)]
        public struct PerformanceInformation
        {
            public int Size;
            public IntPtr CommitTotal;
            public IntPtr CommitLimit;
            public IntPtr CommitPeak;
            public IntPtr PhysicalTotal;
            public IntPtr PhysicalAvailable;
            public IntPtr SystemCache;
            public IntPtr KernelTotal;
            public IntPtr KernelPaged;
            public IntPtr KernelNonPaged;
            public IntPtr PageSize;
            public int HandlesCount;
            public int ProcessCount;
            public int ThreadCount;
        }

        public static float GetUsedMemory()
        {
            PerformanceInformation pi = new PerformanceInformation();
            if (GetPerformanceInfo(out pi, Marshal.SizeOf(pi)))
            {
                return (float)((pi.PhysicalAvailable.ToInt64() * 100) / pi.PhysicalTotal.ToInt64());
            }
            else { return 0; }
        }

        public static Int64 GetPhysicalAvailableMemoryInMiB()
        {
            PerformanceInformation pi = new PerformanceInformation();
            if (GetPerformanceInfo(out pi, Marshal.SizeOf(pi)))
            {
                return Convert.ToInt64((pi.PhysicalAvailable.ToInt64() * pi.PageSize.ToInt64() / 1048576));
            }
            else { return -1; }
        }

        public static Int64 GetTotalMemoryInMiB()
        {
            PerformanceInformation pi = new PerformanceInformation();
            if (GetPerformanceInfo(out pi, Marshal.SizeOf(pi)))
            {
                return Convert.ToInt64((pi.PhysicalTotal.ToInt64() * pi.PageSize.ToInt64() / 1048576));
            }
            else { return -1; }
        }

        public static bool IsAdministrator()
        {
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

        public static void GetProcessOwnerByProcessId(int processId, out string user, out string domain, out string cmd)
        {
            user = null;
            domain = null;
            cmd = null;
            using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Process WHERE ProcessID = '" + processId + "'"))
            {
                if (searcher.Get().Count != 1) return;
                var process = searcher.Get().Cast<ManagementObject>().First();
                if (process["CommandLine"] != null) { cmd = process["CommandLine"].ToString(); }
                var ownerInfo = new string[2];
                process.InvokeMethod("GetOwner", ownerInfo);
                user = ownerInfo[0];
                domain = ownerInfo[1];
            }
        }


        /// <summary>
        /// Send wake-on-lan packet
        /// </summary>
        /// <param name="macAddress"></param>
        /// <returns></returns>
        public static async Task WakeOnLan(string macAddress)
        {
            byte[] magicPacket = BuildWakePacket(macAddress);
            foreach (NetworkInterface networkInterface in NetworkInterface.GetAllNetworkInterfaces().Where((n) =>
                n.NetworkInterfaceType != NetworkInterfaceType.Loopback && n.OperationalStatus == OperationalStatus.Up))
            {
                IPInterfaceProperties iPInterfaceProperties = networkInterface.GetIPProperties();
                foreach (MulticastIPAddressInformation multicastIPAddressInformation in iPInterfaceProperties.MulticastAddresses)
                {
                    IPAddress multicastIpAddress = multicastIPAddressInformation.Address;
                    if (multicastIpAddress.ToString().StartsWith("ff02::1%", StringComparison.OrdinalIgnoreCase))
                    {
                        UnicastIPAddressInformation unicastIPAddressInformation = iPInterfaceProperties.UnicastAddresses.Where((u) =>
                            u.Address.AddressFamily == AddressFamily.InterNetworkV6 && !u.Address.IsIPv6LinkLocal).FirstOrDefault();
                        if (unicastIPAddressInformation != null)
                        {
                            await SendWakeOnLan(unicastIPAddressInformation.Address, multicastIpAddress, magicPacket);
                            break;
                        }
                    }
                    else if (multicastIpAddress.ToString().Equals("224.0.0.1"))
                    {
                        UnicastIPAddressInformation unicastIPAddressInformation = iPInterfaceProperties.UnicastAddresses.Where((u) =>
                            u.Address.AddressFamily == AddressFamily.InterNetwork && !iPInterfaceProperties.GetIPv4Properties().IsAutomaticPrivateAddressingActive).FirstOrDefault();
                        if (unicastIPAddressInformation != null)
                        {
                            await SendWakeOnLan(unicastIPAddressInformation.Address, multicastIpAddress, magicPacket);
                            break;
                        }
                    }
                }
            }
        }

        static byte[] BuildWakePacket(string macAddress)
        {
            byte[] macBytes = new byte[6];
            for (int i = 0; i < 6; i++) { macBytes[i] = Convert.ToByte(macAddress.Substring(i * 2, 2), 16); }

            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    for (int i = 0; i < 6; i++) { bw.Write((byte)0xff); }
                    for (int i = 0; i < 16; i++) { bw.Write(macBytes); }
                }
                return ms.ToArray();
            }
        }

        static async Task SendWakeOnLan(IPAddress localIpAddress, IPAddress multicastIpAddress, byte[] magicPacket)
        {
            using (UdpClient client = new UdpClient(new IPEndPoint(localIpAddress, 0)))
            {
                await client.SendAsync(magicPacket, magicPacket.Length, multicastIpAddress.ToString(), 9);
            }
        }

    }


    public class HttpFileDownload {
        private MeshCentralAgent parent;
        private string uri;
        private string filepath;
        private string certhash;

        public HttpFileDownload(MeshCentralAgent parent, string uri, string filepath, string certhash) {
            this.parent = parent;
            this.uri = uri;
            this.filepath = filepath;
            this.certhash = certhash;
        }

        public async void Start()
        {
            try
            {
                using (var fileStream = new FileStream(filepath, FileMode.Create))
                {
                    // Download the file
                    HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);
                    request.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(RemoteCertificateValidationCallback);
                    using (WebResponse response = await request.GetResponseAsync())
                    {
                        using (Stream dataStream = response.GetResponseStream())
                        {
                            int len = 0;
                            byte[] buffer = new byte[4096];
                            do
                            {
                                len = await dataStream.ReadAsync(buffer, 0, buffer.Length);
                                if (len > 0) { await fileStream.WriteAsync(buffer, 0, len); }
                            }
                            while (len > 0);
                        }
                    }
                }
            }
            catch (Exception) { }
            parent.fileDownloads.Remove(this);
        }

        private bool RemoteCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            using (SHA384 sha384Hash = SHA384.Create()) { if (BitConverter.ToString(sha384Hash.ComputeHash(certificate.GetRawCertData())).Replace("-", "").ToLower().Equals(certhash.ToLower())) return true; }
            return false;
        }

    }

}
