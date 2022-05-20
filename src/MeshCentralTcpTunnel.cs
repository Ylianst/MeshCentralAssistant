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
using System.Net.Sockets;
using System.Collections.Generic;

namespace MeshAssistant
{
    class MeshCentralTcpTunnel
    {
        private Dictionary<string, object> creationArgs = null;
        private int state = 0; // 0 = Waitting for "c" or "cr", 1 = Waitting for protocol number, 2 = Running
        private MeshCentralAgent parent = null;
        public webSocketClient WebSocket = null;
        private string extraLogStr = "";
        private string sessionUserName = null;
        public string sessionLoggingUserName = null;
        public long userRights = 0;
        private string tcpaddr = null;
        private int tcpport = 0;
        private TcpClient tcpclient = null;
        private NetworkStream tcpstream = null;
        private byte[] tcpReadBuffer = new byte[65535];
        private bool tcpstreamShouldRead = false;
        public string userid = null;
        public string guestname = null;

        public MeshCentralTcpTunnel(MeshCentralAgent parent, Uri uri, string serverHash, Dictionary<string, object> creationArgs, string tcpaddr, int tcpport)
        {
            //Console.WriteLine("MeshCentralTcpTunnel(" + tcpaddr + ":" + tcpport + ")");
            this.parent = parent;
            this.tcpaddr = tcpaddr;
            this.tcpport = tcpport;
            this.creationArgs = creationArgs;
            WebSocket = new webSocketClient();
            WebSocket.pongTimeSeconds = 120; // Send a websocket pong every 2 minutes.
            WebSocket.onStateChanged += WebSocket_onStateChanged;
            WebSocket.onBinaryData += WebSocket_onBinaryData;
            WebSocket.onStringData += WebSocket_onStringData;
            WebSocket.onSendOk += WebSocket_onSendOk;
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
                if (creationArgs.ContainsKey("rights") && ((creationArgs["rights"].GetType() == typeof(System.Int32)) || (creationArgs["rights"].GetType() == typeof(System.Int64)))) { userRights = (long)creationArgs["rights"]; }
            }
        }

        private void WebSocket_onStateChanged(webSocketClient sender, webSocketClient.ConnectionStates state)
        {
            //Console.WriteLine("WebSocket_onStateChanged(" + state + ")");
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
            //Console.WriteLine("disconnect()");
            if (WebSocket != null) { WebSocket.Dispose(); WebSocket = null; }

            // Update session
            if (sessionUserName != null)
            {
                if (parent.TcpSessions != null)
                {
                    if (parent.TcpSessions.ContainsKey(sessionUserName))
                    {
                        parent.TcpSessions[sessionUserName] = (int)parent.TcpSessions[sessionUserName] - 1;
                        if ((int)parent.TcpSessions[sessionUserName] == 0) { parent.TcpSessions.Remove(sessionUserName); }
                    }
                    parent.fireSessionChanged(2);
                }
                sessionUserName = null;
                sessionLoggingUserName = null;
            }

            parent.Event(userid, "Closed TCP tunnel");
        }

        private void connectedToServer()
        {
            //Console.WriteLine("connectedToServer()");
            if (WebSocket != null)
            {
                WebSocket.Pause();
                tcpclient = new TcpClient();
                tcpclient.BeginConnect(tcpaddr, tcpport, new AsyncCallback(OnTcpConnect), null);
            }
            else
            {
                disconnect();
            }
        }

        private void OnTcpConnect(IAsyncResult ar)
        {
            //Console.WriteLine("OnTcpConnect()");
            try { tcpclient.EndConnect(ar); } catch (Exception) { disconnect(); return; }
            tcpstream = tcpclient.GetStream();
            try { tcpstream.BeginRead(tcpReadBuffer, 0, tcpReadBuffer.Length, new AsyncCallback(OnTcpRead), this); } catch (Exception) { }
            if (WebSocket != null) { WebSocket.Resume(); }
        }

        private void OnTcpRead(IAsyncResult ar)
        {
            int len = 0;
            try { len = tcpstream.EndRead(ar); } catch (Exception) { disconnect(); return; }
            if (len == 0) { disconnect(); return; }
            //Console.WriteLine("OnTcpRead(" + len + ")");
            if (WebSocket != null) { WebSocket.SendBinary(tcpReadBuffer, 0, len); }
            tcpstreamShouldRead = true;
        }

        private void OnTcpWrite(IAsyncResult ar)
        {
            //Console.WriteLine("OnTcpWrite()");
            try { tcpstream.EndWrite(ar); } catch (Exception) { disconnect(); return; }
            if (WebSocket != null) { WebSocket.Resume(); }
        }

        private void WebSocket_onStringData(webSocketClient sender, string data, int orglen)
        {
            if (state == 0) // Waitting for browser connection
            {
                if (data.Equals("c") || data.Equals("cr")) { state = 1; connectedToServer(); }

                // Add this session
                if (creationArgs != null)
                {
                    //if (creationArgs.ContainsKey("username") && (creationArgs["username"].GetType() == typeof(string))) { sessionUserName = (string)creationArgs["username"]; }
                    //if (creationArgs.ContainsKey("realname") && (creationArgs["realname"].GetType() == typeof(string))) { sessionUserName = (string)creationArgs["realname"]; }

                    if (creationArgs.ContainsKey("userid") && (creationArgs["userid"].GetType() == typeof(string)))
                    {
                        sessionLoggingUserName = sessionUserName = (string)creationArgs["userid"];
                        if (creationArgs.ContainsKey("guestname") && (creationArgs["guestname"].GetType() == typeof(string)))
                        {
                            sessionUserName += "/guest:" + Convert.ToBase64String(UTF8Encoding.UTF8.GetBytes((string)creationArgs["guestname"]));
                            sessionLoggingUserName += " - " + (string)creationArgs["guestname"];
                        }
                    }

                    if (sessionUserName != null)
                    {
                        if (parent.TcpSessions == null) { parent.TcpSessions = new Dictionary<string, object>(); }
                        if (parent.TcpSessions.ContainsKey(sessionUserName)) { parent.TcpSessions[sessionUserName] = (int)parent.TcpSessions[sessionUserName] + 1; } else { parent.TcpSessions[sessionUserName] = 1; }
                        parent.fireSessionChanged(9);
                    }
                }
            }
        }

        private void WebSocket_onSendOk(webSocketClient sender)
        {
            //Console.WriteLine("WebSocket_onSendOk()");
            if (tcpstreamShouldRead == true) {
                try { tcpstream.BeginRead(tcpReadBuffer, 0, tcpReadBuffer.Length, new AsyncCallback(OnTcpRead), this); } catch (Exception) { disconnect(); }
            }
        }

        private void WebSocket_onBinaryData(webSocketClient sender, byte[] data, int off, int len, int orglen)
        {
            //Console.WriteLine("WebSocket_onBinaryData()");
            if (WebSocket != null) { WebSocket.Pause(); }
            try { tcpstream.BeginWrite(data, off, len, new AsyncCallback(OnTcpWrite), this); } catch (Exception) { disconnect(); }
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


    }
}
