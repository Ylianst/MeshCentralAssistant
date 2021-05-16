using System;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Collections;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace MeshAssistant
{
    public class MeshCentralTunnel
    {
        private Dictionary<string, object> creationArgs = null;
        private int state = 0; // 0 = Waitting for "c" or "cr", 1 = Waitting for protocol number, 2 = Running
        private int protocol = 0; // 1 = Terminal, 2 = Desktop, 5 = Files, 10 = File Transfer
        //private bool serverRecording = false;
        private MeshCentralAgent parent = null;
        public webSocketClient WebSocket = null;
        private JavaScriptSerializer JSON = new JavaScriptSerializer();
        private MeshCentralDesktop Desktop = null;
        private Dictionary<string, object> jsonOptions = null;
        private FileStream fileSendTransfer = null;
        private FileStream fileRecvTransfer = null;
        private string fileRecvTransferPath = null;
        private int fileRecvTransferSize = 0;
        private int fileRecvId = 0;
        private string extraLogStr = "";
        private string sessionUserName = null;
        public long userRights = 0;

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
            WebSocket.pongTimeSeconds = 120; // Send a websocket pong every 2 minutes.
            WebSocket.onStateChanged += WebSocket_onStateChanged;
            WebSocket.onBinaryData += WebSocket_onBinaryData;
            WebSocket.onStringData += WebSocket_onStringData;
            WebSocket.Start(uri, serverHash);

            // Setup extra log values
            if (creationArgs != null)
            {
                if (creationArgs.ContainsKey("userid") && (creationArgs["userid"].GetType() == typeof(string))) { extraLogStr += ",\"userid\":\"" + escapeJsonString((string)creationArgs["userid"]) + "\""; }
                if (creationArgs.ContainsKey("username") && (creationArgs["username"].GetType() == typeof(string))) { extraLogStr += ",\"username\":\"" + escapeJsonString((string)creationArgs["username"]) + "\""; }
                if (creationArgs.ContainsKey("remoteaddr") && (creationArgs["remoteaddr"].GetType() == typeof(string))) { extraLogStr += ",\"remoteaddr\":\"" + escapeJsonString((string)creationArgs["remoteaddr"]) + "\""; }
                if (creationArgs.ContainsKey("sessionid") && (creationArgs["sessionid"].GetType() == typeof(string))) { extraLogStr += ",\"sessionid\":\"" + escapeJsonString((string)creationArgs["sessionid"]) + "\""; }
                if (creationArgs.ContainsKey("rights") && ((creationArgs["rights"].GetType() == typeof(System.Int32)) || (creationArgs["rights"].GetType() == typeof(System.Int64)))) { userRights = (long)creationArgs["rights"]; }
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
            if (fileSendTransfer != null) { fileSendTransfer.Close(); fileSendTransfer = null; }
            if (fileRecvTransfer != null) { fileRecvTransfer.Close(); fileRecvTransfer = null; }
            if (WebSocket != null) { WebSocket.Dispose(); WebSocket = null; }
            if (Desktop != null) { Desktop.Dispose(); Desktop = null; }

            // Update session
            if (sessionUserName != null)
            {
                if (protocol == 2)
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
                if (protocol == 5)
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
            }
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
                    if (creationArgs.ContainsKey("userid") && (creationArgs["userid"].GetType() == typeof(string))) { sessionUserName = (string)creationArgs["userid"]; }
                    if (sessionUserName != null)
                    {
                        if (protocol == 2) // Desktop
                        {
                            if (parent.DesktopSessions == null) { parent.DesktopSessions = new Dictionary<string, object>(); }
                            if (parent.DesktopSessions.ContainsKey(sessionUserName)) { parent.DesktopSessions[sessionUserName] = (int)parent.DesktopSessions[sessionUserName] + 1; } else { parent.DesktopSessions[sessionUserName] = 1; }
                            parent.fireSessionChanged(2);
                        }
                        if (protocol == 5) // Files
                        {
                            if (parent.FilesSessions == null) { parent.FilesSessions = new Dictionary<string, object>(); }
                            if (parent.FilesSessions.ContainsKey(sessionUserName)) { parent.FilesSessions[sessionUserName] = (int)parent.FilesSessions[sessionUserName] + 1; } else { parent.FilesSessions[sessionUserName] = 1; }
                            parent.fireSessionChanged(5);
                        }
                    }
                }

                if (protocol == 2)
                {
                    Desktop = new MeshCentralDesktop(this);
                }
                else if (protocol == 10)
                {
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

        private void WebSocket_onSendOk(webSocketClient sender)
        {
            if (fileSendTransfer != null)
            {
                byte[] buf = new byte[65535];
                int len = fileSendTransfer.Read(buf, 0, buf.Length);
                if (len > 0) { WebSocket.SendBinary(buf, 0, len); } else { disconnect(); }
            }
        }

        private void WebSocket_onBinaryData(webSocketClient sender, byte[] data, int off, int len, int orglen)
        {
            if (state != 2) return;
            if (protocol == 2) // Desktop
            {
                if (Desktop != null) { Desktop.onBinaryData(data, off, len); }
            }
            else if (protocol == 5) // Files
            {
                if (data[off] == 123) { ParseFilesCommand(UTF8Encoding.UTF8.GetString(data, off, len)); }
                else if (fileRecvTransfer != null) {
                    bool err = false;
                    if (data[off] == 0) {
                        try { fileRecvTransfer.Write(data, off + 1, len - 1); } catch (Exception) { err = true; }
                    } else {
                        try { fileRecvTransfer.Write(data, off, len); } catch (Exception) { err = true; }
                    }
                    if (err) {
                        try { fileRecvTransfer.Close(); fileRecvTransfer = null; } catch (Exception) { }
                        fileRecvTransfer = null;
                        WebSocket.SendBinary(UTF8Encoding.UTF8.GetBytes("{\"action\":\"uploaderror\"}"));
                    } else {
                        WebSocket.SendBinary(UTF8Encoding.UTF8.GetBytes("{\"action\":\"uploadack\",\"reqid\":" + fileRecvId + "}"));
                    }
                }
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
                        if ((jsonCommand.ContainsKey("path")) && (jsonCommand["path"].GetType() == typeof(string))) { path = (string)jsonCommand["path"]; }
                        if (path != "") {
                            path = path.Replace("/", "\\");
                            Directory.CreateDirectory(path);
                            parent.WebSocket.SendBinary(UTF8Encoding.UTF8.GetBytes("{\"action\":\"log\",\"msgid\":44,\"msgArgs\":[\"" + escapeJsonString(path) + "\"],\"msg\":\"Create folder: " + escapeJsonString(path) + "\"" + extraLogStr + "}"));
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
                        if (jsonCommand.ContainsKey("path") && (jsonCommand["path"].GetType() == typeof(string))) { path = (string)jsonCommand["path"]; }
                        if (jsonCommand.ContainsKey("name") && (jsonCommand["name"].GetType() == typeof(string))) { name = (string)jsonCommand["name"]; }
                        if (jsonCommand.ContainsKey("size") && (jsonCommand["size"].GetType() == typeof(System.Int32))) { size = (int)jsonCommand["size"]; }
                        if ((path == null) || (name == null) || (size <= 0) || !Directory.Exists(path)) { disconnect(); return; }

                        string fullname = Path.Combine(path, name).Replace("/", "\\");
                        try { fileRecvTransfer = File.OpenWrite(fullname); } catch (Exception) { WebSocket.SendBinary(UTF8Encoding.UTF8.GetBytes("{\"action\":\"uploaderror\",\"reqid\":" + reqid + "}")); break; }
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
                default: { break; }
            }
            if (response != null) { WebSocket.SendBinary(UTF8Encoding.UTF8.GetBytes(response)); }
        }

    }
}
