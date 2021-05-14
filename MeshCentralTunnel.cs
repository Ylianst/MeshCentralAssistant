using System;
using System.Text;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.IO;
using System.Collections;

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
            if (jsonAction == null) return;
            if (jsonAction.ContainsKey("ctrlChannel") && (jsonAction["ctrlChannel"].GetType() == typeof(string)) && (((string)jsonAction["ctrlChannel"]).Equals("102938")))
            {
                // This is a control command
                if (!jsonAction.ContainsKey("type") || jsonAction["type"].GetType() != typeof(string)) return;
                string type = (string)jsonAction["type"];
                if (type.Equals("close")) { disconnect(); return; }
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
            // Parse the received JSON
            Dictionary<string, object> jsonCommand = new Dictionary<string, object>();
            try { jsonCommand = JSON.Deserialize<Dictionary<string, object>>(data); } catch (Exception) { return; }
            if (jsonCommand == null || jsonCommand["action"].GetType() != typeof(string)) return;

            int reqid = 0;
            if (jsonCommand["reqid"].GetType() == typeof(System.Int32)) { reqid = (int)jsonCommand["reqid"]; }

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
                        } else {
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
                        break;
                    }
                case "mkdir": // Create folder
                    {
                        string path = "";
                        if (jsonCommand["path"].GetType() == typeof(string)) { path = (string)jsonCommand["path"]; }
                        if (path != "") {
                            Directory.CreateDirectory(path);
                            parent.WebSocket.SendBinary(UTF8Encoding.UTF8.GetBytes("{\"action\":\"log\",\"msgid\":44,\"msgArgs\":[\"" + escapeJsonString(path) + "\"],\"msg\":\"Create folder: " + escapeJsonString(path) + "\"}"));
                        }
                        break;
                    }
                case "rm": // Remove folders or files
                    {
                        bool rec = false;
                        string path = "";
                        ArrayList delfiles = null;
                        if (jsonCommand["path"].GetType() == typeof(string)) { path = (string)jsonCommand["path"]; }
                        if (jsonCommand["rec"].GetType() == typeof(bool)) { rec = (bool)jsonCommand["rec"]; }
                        if (jsonCommand["delfiles"].GetType() == typeof(ArrayList)) { delfiles = (ArrayList)jsonCommand["delfiles"]; }
                        if ((path != "") && (delfiles != null))
                        {
                            path = path.Replace("/","\\");
                            foreach (object o in delfiles)
                            {
                                if (o.GetType() != typeof(string)) continue;
                                string delfile = (string)o;
                                delfile = Path.Combine(path, delfile);
                            }
                        }
                        break;
                    }
                default: { break; }
            }

            // "{\"action\":\"pong\"}"
            if (response != null) { WebSocket.SendBinary(UTF8Encoding.UTF8.GetBytes(response)); }

        }

    }
}
