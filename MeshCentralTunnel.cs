using System;
using System.Text;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace MeshAssistant
{
    public class MeshCentralTunnel
    {
        private int state = 0; // 0 = Waitting for "c" or "cr", 1 = Waitting for protocol number, 2 = Running
        private int protocol = 0; // 5 = Files
        //private bool serverRecording = false;
        private MeshCentralAgent parent = null;
        private webSocketClient WebSocket = null;
        private JavaScriptSerializer JSON = new JavaScriptSerializer();

        public MeshCentralTunnel(MeshCentralAgent parent, Uri uri, string serverHash)
        {
            this.parent = parent;
            WebSocket = new webSocketClient();
            WebSocket.pongTimeSeconds = 120; // Send a websocket pong every 2 minutes.
            WebSocket.onStateChanged += WebSocket_onStateChanged;
            WebSocket.onBinaryData += WebSocket_onBinaryData;
            WebSocket.onStringData += WebSocket_onStringData;
            WebSocket.Start(uri, serverHash);
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
            if (WebSocket == null) return;
            WebSocket.Dispose();
            WebSocket = null;
        }
        
        private void connectedToServer()
        {

        }

        public void processServerJsonData(string data)
        {
            // Parse the received JSON
            Dictionary<string, object> jsonAction = new Dictionary<string, object>();
            try { jsonAction = JSON.Deserialize<Dictionary<string, object>>(data); } catch (Exception) { return; }
            if (jsonAction == null || jsonAction["action"].GetType() != typeof(string)) return;

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
            if (state == 0) // Waitting for browser connection
            {
                if (data.Equals("c")) { state = 1; connectedToServer(); }
                else if (data.Equals("cr")) { state = 1; /* serverRecording = true; */ connectedToServer(); }
                return;
            }
            else if (state == 1) // Waitting for protocol
            {
                if (int.TryParse(data, out protocol)) { state = 2; }
                return;
            }
            else if (state == 2) // Running
            {
                if (data.StartsWith("{")) { processServerJsonData(data); return; }
            }
        }

        private void WebSocket_onBinaryData(webSocketClient sender, byte[] data, int off, int len, int orglen)
        {
            if (state != 2) return;

            if (protocol == 5) // Files
            {
                ParseFilesCommand(UTF8Encoding.UTF8.GetString(data, off, len));
            }
        }

        private void ParseFilesCommand(string data)
        {
            // Parse the received JSON
            Dictionary<string, object> jsonCommand = new Dictionary<string, object>();
            try { jsonCommand = JSON.Deserialize<Dictionary<string, object>>(data); } catch (Exception) { return; }
            if (jsonCommand == null || jsonCommand["action"].GetType() != typeof(string)) return;

            int reqid = 0;
            if (jsonCommand["reqid"].GetType() == typeof(System.Int32)) { reqid = (int)jsonCommand["reqid"]; }

            // Handle the JSON command
            string action = (string)jsonCommand["action"];
            switch (action)
            {
                case "ls":
                    {
                        string path = "";
                        if (jsonCommand["path"].GetType() == typeof(string)) { path = (string)jsonCommand["path"]; }
                        if (path == "")
                        {
                            // Enumerate drives


                        } else {
                            // Enumerate folders


                        }
                        break;
                    }
                default: { break; }
            }

        }

    }
}
