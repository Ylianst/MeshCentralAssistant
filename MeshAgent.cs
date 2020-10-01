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
using System.Linq;
using System.Text;
using System.IO.Pipes;
using System.ServiceProcess;
using System.Security.Principal;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using Microsoft.Win32;

namespace MeshAssistant
{
    public class MeshAgent
    {
        private static ServiceController agentService = new ServiceController("Mesh Agent");
        private byte[] pipeBuffer = new byte[65535];
        private bool pipeWritePending = false;
        private NamedPipeClientStream pipeClient = null;
        public int State = 0;
        public int ServerState = 0;
        public Uri ServerUri = null;

        public delegate void onQueryResultHandler(string value, string result);
        public event onQueryResultHandler onQueryResult;

        public delegate void onStateChangedHandler(int state, int serverState);
        public event onStateChangedHandler onStateChanged;

        public static ServiceControllerStatus GetServiceStatus() { agentService.Refresh(); return agentService.Status; }
        public static void StartService() { try { agentService.Start(); } catch (Exception) { } }
        public static void StopService() { try { agentService.Stop(); } catch (Exception) { } }

        public static string GetNodeId32()
        {
            try
            {
                RegistryKey localKey32 = RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, RegistryView.Registry32);
                localKey32 = localKey32.OpenSubKey(@"SOFTWARE\Open Source\MeshAgent2");
                if (localKey32 != null)
                {
                    string nodeidb64 = localKey32.GetValue("NodeId").ToString().Replace('@', '+').Replace('$', '/');
                    return string.Concat(Convert.FromBase64String(nodeidb64).Select(b => b.ToString("X2")));
                }
            }
            catch (Exception) { }
            return null;
        }


        public static string GetNodeId64()
        {
            try {
                RegistryKey localKey64 = RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, RegistryView.Registry64);
                localKey64 = localKey64.OpenSubKey(@"SOFTWARE\Open Source\MeshAgent2");
                if (localKey64 != null) {
                    string nodeidb64 = localKey64.GetValue("NodeId").ToString().Replace('@', '+').Replace('$', '/');
                    return string.Concat(Convert.FromBase64String(nodeidb64).Select(b => b.ToString("X2")));
                }
            }
            catch (Exception) { }
            return null;
        }

        private void ChangeState(int state, int serverState)
        {
            if ((State == state) && (ServerState == serverState)) return;
            State = state;
            if (State == 0) { ServerState = 0; } else { ServerState = serverState; }
            if (onStateChanged != null) { onStateChanged(State, ServerState); }
        }

        public bool SendCommand(string cmd, string value)
        {
            if ((pipeClient == null) || (pipeClient.IsConnected == false) || (pipeWritePending == true)) return false;
            pipeWritePending = true;
            string data = "{\"cmd\":\"" + cmd + "\",\"value\":\"" + value + "\"}";
            byte[] buf2 = UTF8Encoding.UTF8.GetBytes(data);
            byte[] buf1 = BitConverter.GetBytes(buf2.Length + 4);
            byte[] buf = new byte[4 + buf2.Length];
            Array.Copy(buf1, 0, buf, 0, 4);
            Array.Copy(buf2, 0, buf, 4, buf2.Length);
            pipeClient.BeginWrite(buf, 0, buf.Length, new AsyncCallback(WritePipe), null);
            return true;
        }

        public bool ConnectPipe()
        {
            if (pipeClient != null) return true;
            string nodeid = GetNodeId64();
            if (nodeid != null)
            {
                pipeClient = new NamedPipeClientStream(".", nodeid + "-DAIPC", PipeDirection.InOut, PipeOptions.Asynchronous, TokenImpersonationLevel.Impersonation);
                try { pipeClient.Connect(10); } catch (Exception) { pipeClient = null; }
                if ((pipeClient != null) && pipeClient.IsConnected)
                {
                    ChangeState(1, 0);
                    SendCommand("register", "name");
                    pipeClient.BeginRead(pipeBuffer, 0, pipeBuffer.Length, new AsyncCallback(ReadPipe), null);
                    return true;
                }
                pipeClient = null;
            }

            nodeid = GetNodeId32();
            if (nodeid != null)
            {
                pipeClient = new NamedPipeClientStream(".", nodeid + "-DAIPC", PipeDirection.InOut, PipeOptions.Asynchronous, TokenImpersonationLevel.Impersonation);
                try { pipeClient.Connect(10); } catch (Exception) { pipeClient = null; }
                if ((pipeClient != null) && pipeClient.IsConnected)
                {
                    ChangeState(1, 0);
                    SendCommand("register", "name");
                    pipeClient.BeginRead(pipeBuffer, 0, pipeBuffer.Length, new AsyncCallback(ReadPipe), null);
                    return true;
                }
                pipeClient = null;
            }
            return false;
        }

        public void DisconnectPipe()
        {
            if (pipeClient != null) { pipeClient.Close(); }
            ServerState = 0;
            ServerUri = null;
    }

    public bool QueryDescriptors()
        {
            return SendCommand("query", "descriptors");
        }
        
        private void WritePipe(IAsyncResult r)
        {
            pipeClient.EndWrite(r);
            pipeWritePending = false;
        }

        private void ReadPipe(IAsyncResult r)
        {
            int len = pipeClient.EndRead(r);
            if (len == 0) { pipeClient.Dispose(); pipeClient = null; ChangeState(0, 0); return; }
            if (len < 4) { return; }

            int chunklen = BitConverter.ToInt32(pipeBuffer, 0);
            string data = UTF8Encoding.UTF8.GetString(pipeBuffer, 4, chunklen - 4);

            // Parse the received JSON
            Dictionary<string, object> jsonAction = new Dictionary<string, object>();
            jsonAction = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(data);
            if (jsonAction != null && jsonAction["cmd"].GetType() == typeof(string))
            {
                string action = jsonAction["cmd"].ToString();
                switch (action)
                {
                    case "serverstate":
                        {
                            ServerUri = new Uri(jsonAction["url"].ToString().Replace("wss://", "https://").Replace("ws://", "http://").Replace("/agent.ashx", ""));
                            ChangeState(1, int.Parse(jsonAction["value"].ToString()));
                            break;
                        }
                    case "query":
                        {
                            string value = jsonAction["value"].ToString();
                            string result = jsonAction["result"].ToString().Replace("\n", "\r\n");
                            if (onQueryResult != null) { onQueryResult(value, result); }
                            break;
                        }
                }
            }

            pipeClient.BeginRead(pipeBuffer, 0, pipeBuffer.Length, new AsyncCallback(ReadPipe), null);
        }

    }
}
