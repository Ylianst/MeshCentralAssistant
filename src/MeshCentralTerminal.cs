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
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;

namespace MeshAssistant
{
    public class MeshCentralTerminal
    {
        private MeshCentralTunnel parent = null;
        private Thread mainThread = null;
        private int width;
        private int height;
        private ConPTY.PseudoConsolePipe inputPipe;
        private ConPTY.PseudoConsolePipe outputPipe;
        private ConPTY.PseudoConsole pseudoConsole;
        private ConPTY.XProcess process;
        private StreamWriter writer;
        private int protocol = 0;

        /// <summary>
        /// Check if Pseudo Console is supported
        /// </summary>
        /// <returns></returns>
        public static bool CheckTerminalSupport()
        {
            IntPtr hPC;
            int createResult = 0;
            try { createResult = Win32Api.CreatePseudoConsole(new Win32Api.COORD { X = (short)0, Y = (short)0 }, IntPtr.Zero, IntPtr.Zero, 0, out hPC); } catch (Exception) { }
            return (createResult == -2147024809);
        }


        public void Log(string msg)
        {
            try { File.AppendAllText("debug.log", DateTime.Now.ToString("HH:mm:tt.ffff") + ": MCAgent: " + msg + "\r\n"); } catch (Exception) { }
        }

        public MeshCentralTerminal(MeshCentralTunnel parent, int protocol, int width, int height)
        {
            this.parent = parent;
            this.protocol = protocol;
            this.width = width;
            this.height = height;

            // Start the capture thread
            mainThread = new Thread(new ThreadStart(MainTerminalLoop));
            mainThread.Start();
        }

        /// <summary>
        /// Get an AutoResetEvent that signals when the process exits
        /// </summary>
        private static AutoResetEvent WaitForExit(ConPTY.XProcess process) => new AutoResetEvent(false) { SafeWaitHandle = new SafeWaitHandle(process.ProcessInfo.hProcess, ownsHandle: false) };

        public void Dispose()
        {
            if (process != null) {
                try { Process p = Process.GetProcessById(process.ProcessInfo.dwProcessId); if (p.HasExited == false) { p.Kill(); } } catch (Exception) { }
                try { process.Dispose(); } catch (Exception) { }
                process = null;
            }
            inputPipe = null;
            outputPipe = null;
            pseudoConsole = null;
            writer = null;
            mainThread = null;
            parent = null;
        }

        public void Resize(int width, int height)
        {
            if (pseudoConsole != null) { pseudoConsole.Resize(width, height); }
        }

        public void onBinaryData(byte[] data, int off, int len)
        {
            string termData = UTF8Encoding.UTF8.GetString(data, off, len);
            if (writer != null) { writer.Write(termData); }
        }

        private void MainTerminalLoop()
        {
            string cmd = "cmd.exe";
            if (protocol == 9) { cmd = "powershell.exe"; }
            using (var inputPipe = new ConPTY.PseudoConsolePipe())
            using (var outputPipe = new ConPTY.PseudoConsolePipe())
            using (var pseudoConsole = ConPTY.PseudoConsole.Create(inputPipe.ReadSide, outputPipe.WriteSide, (short)width, (short)height))
            using (var process = ConPTY.ProcessFactory.Start(cmd, ConPTY.PseudoConsole.PseudoConsoleThreadAttribute, pseudoConsole.Handle))
            {
                this.inputPipe = inputPipe;
                this.outputPipe = outputPipe;
                this.pseudoConsole = pseudoConsole;
                this.process = process;
                Task.Run(() => CopyPipeToOutput(outputPipe.ReadSide, parent)); // Output
                writer = new StreamWriter(new FileStream(inputPipe.WriteSide, FileAccess.Write)); // Input
                writer.AutoFlush = true;
                OnClose(() => DisposeResources(process, pseudoConsole, outputPipe, inputPipe)); // Close
                WaitForExit(process).WaitOne(Timeout.Infinite);
            }
        }

        /// <summary>
        /// Reads PseudoConsole output and copies it to the terminal's standard out.
        /// </summary>
        /// <param name="outputReadSide">the "read" side of the pseudo console output pipe</param>
        private static void CopyPipeToOutput(SafeFileHandle outputReadSide, MeshCentralTunnel tunnel)
        {
            byte[] buffer = new byte[1024];
            using (var pseudoConsoleOutput = new FileStream(outputReadSide, FileAccess.Read))
            {
                while (true)
                {
                    int len = 0;
                    try { len = pseudoConsoleOutput.Read(buffer, 0, buffer.Length); } catch (Exception) { }
                    if (len == 0) { tunnel.disconnect(); return; }
                    if (tunnel.WebSocket != null) { try { tunnel.WebSocket.SendBinary(buffer, 0, len); } catch (Exception) { } }
                }
            }
        }

        /// <summary>
        /// Set a callback for when the terminal is closed (e.g. via the "X" window decoration button).
        /// Intended for resource cleanup logic.
        /// </summary>
        private static void OnClose(Action handler)
        {
            Win32Api.SetConsoleCtrlHandler(eventType =>
            {
                if (eventType == Win32Api.CtrlTypes.CTRL_CLOSE_EVENT) { handler(); }
                return false;
            }, true);
        }

        private void DisposeResources(params IDisposable[] disposables)
        {
            foreach (var disposable in disposables) { disposable.Dispose(); }
        }

    }
}
