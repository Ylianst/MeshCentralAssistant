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
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace MeshAssistant
{
    class MeshCentralDesktop
    {
        private List<MeshCentralTunnel> tunnels = new List<MeshCentralTunnel>();
        private Thread mainThread = null;
        private int currentDisplay = -1;
        private Size ScreenSize = Size.Empty;
        private ImageCodecInfo jgpEncoder;
        private EncoderParameters myEncoderParameters;
        private byte[] skipHeader = new byte[16];
        private Bitmap captureBitmap = null;
        private int encoderType = 1;
        private int encoderCompression = 30;
        private int encoderScaling = 1024;
        private int newEncoderScaling = 1024;
        private bool encoderScalingChanged = false;
        private int encoderFrameRate = 100;
        private int mousePointer = 0;
        private ulong[] oldcrcs = null;
        private ulong[] newcrcs = null;
        private int tilesWide = 0;
        private int tilesHigh = 0;
        private int tilesFullWide = 0;
        private int tilesFullHigh = 0;
        private int tilesRemainingWidth = 0;
        private int tilesRemainingHeight = 0;
        private int tilesCount = 0;
        private Rectangle screenRect = Rectangle.Empty;
        private int screenScaleWidth = 0;
        private int screenScaleHeight = 0;
        private int exceptionLogState = 0;
        private bool UnableToCaptureShowing = false;
        private bool tunnelAdded = true;

        // Global desktop control
        public static MeshCentralDesktop globalDesktop = null;
        public static MeshCentralDesktop AddDesktopTunnel(MeshCentralTunnel tunnel)
        {
            if (globalDesktop == null) { globalDesktop = new MeshCentralDesktop(tunnel); } else { globalDesktop.AddTunnel(tunnel); }
            return globalDesktop;
        }
        public static void RemoveDesktopTunnel(MeshCentralTunnel tunnel)
        {
            if (globalDesktop == null) return;
            if (globalDesktop.RemoveTunnel(tunnel)) { globalDesktop.Dispose(); globalDesktop = null; }
        }

        public void Log(string msg)
        {
            try { File.AppendAllText("debug.log", DateTime.Now.ToString("HH:mm:tt.ffff") + ": MCAgent: " + msg + "\r\n"); } catch (Exception) { }
        }

        [DllImport("user32.dll")]
        public static extern void mouse_event(uint dwFlags, int dx, int dy, uint dwData, int dwExtraInfo);

        [DllImport("user32.dll")]
        public static extern int SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        private static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("User32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int uMsg, int wParam, string lParam);

        [DllImport("user32.dll")]
        private static extern int MapVirtualKey(int uCode, uint uMapType);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint SendInput(uint nInputs, Input[] pInputs, int cbSize);

        [DllImport("user32.dll")]
        private static extern IntPtr GetMessageExtraInfo();

        const uint MAPVK_VK_TO_VSC = 0x00;
        const uint MAPVK_VSC_TO_VK = 0x01;
        const uint MAPVK_VK_TO_CHAR = 0x02;
        const uint MAPVK_VSC_TO_VK_EX = 0x03;
        const uint MAPVK_VK_TO_VSC_EX = 0x04;

        public struct Input
        {
            public int type;
            public InputUnion u;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MouseInput
        {
            public int dx;
            public int dy;
            public uint mouseData;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct HardwareInput
        {
            public uint uMsg;
            public ushort wParamL;
            public ushort wParamH;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct KeyboardInput
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct InputUnion
        {
            [FieldOffset(0)]
            public MouseInput mi;
            [FieldOffset(0)]
            public KeyboardInput ki;
            [FieldOffset(0)]
            public HardwareInput hi;
        }

        [Flags]
        enum MouseEventFlags : uint
        {
            MOUSEEVENTF_MOVE = 0x0001,
            MOUSEEVENTF_LEFTDOWN = 0x0002,
            MOUSEEVENTF_LEFTUP = 0x0004,
            MOUSEEVENTF_RIGHTDOWN = 0x0008,
            MOUSEEVENTF_RIGHTUP = 0x0010,
            MOUSEEVENTF_MIDDLEDOWN = 0x0020,
            MOUSEEVENTF_MIDDLEUP = 0x0040,
            MOUSEEVENTF_XDOWN = 0x0080,
            MOUSEEVENTF_XUP = 0x0100,
            MOUSEEVENTF_WHEEL = 0x0800,
            MOUSEEVENTF_VIRTUALDESK = 0x4000,
            MOUSEEVENTF_ABSOLUTE = 0x8000
        }

        // Keystrokes, mouse motions, and button clicks.
        [DllImport("user32.dll", SetLastError = true)]
        static extern uint SendInput(uint nInputs, ref INPUT pInputs, int cbSize);

        // INPUT
        [StructLayout(LayoutKind.Sequential)]
        struct INPUT
        {
            public SendInputEventType type;
            public MouseKeybdhardwareInputUnion mkhi; // Mouse, Keyboard, Hardware, Input
        }

        enum SendInputEventType : int
        {
            MOUSE = 0,
            KEYBOARD = 1,
            HARDWARE = 2
        }

        [StructLayout(LayoutKind.Explicit)]
        struct MouseKeybdhardwareInputUnion
        {
            //[FieldOffset(0)]
            //public MouseInputData mi; // mouse input

            [FieldOffset(0)]
            public KEYBDINPUT ki; // keyboard input

            [FieldOffset(0)]
            public HARDWAREINPUT hi; // hardware input
        }

        /*
        struct MouseInputData
        {
            public int dx;
            public int dy;
            public uint mouseData;
            public MouseEventFlags dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }
        */

        [StructLayout(LayoutKind.Sequential)]
        struct KEYBDINPUT
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct HARDWAREINPUT
        {
            public int uMsg;
            public short wParamL;
            public short wParamH;
        }

        [Flags]
        public enum KeyEventF
        {
            KeyDown = 0x0000,
            ExtendedKey = 0x0001,
            KeyUp = 0x0002,
            Unicode = 0x0004,
            Scancode = 0x0008
        }

        [StructLayout(LayoutKind.Sequential)]
        struct POINT
        {
            public Int32 x;
            public Int32 y;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct CURSORINFO
        {
            public Int32 cbSize;        // Specifies the size, in bytes, of the structure. 
                                        // The caller must set this to Marshal.SizeOf(typeof(CURSORINFO)).
            public Int32 flags;         // Specifies the cursor state. This parameter can be one of the following values:
                                        //    0             The cursor is hidden.
                                        //    CURSOR_SHOWING    The cursor is showing.
            public IntPtr hCursor;          // Handle to the cursor. 
            public POINT ptScreenPos;       // A POINT structure that receives the screen coordinates of the cursor. 
        }

        [DllImport("user32.dll")]
        static extern bool GetCursorInfo(out CURSORINFO pci);

        public MeshCentralDesktop(MeshCentralTunnel tunnel)
        {
            lock (tunnels) { tunnels.Add(tunnel); tunnelAdded = true; }

            // Send displays
            SendDisplays(tunnel);

            // Send display locations and sizes
            SendDisplayInfo(tunnel);

            // Setup the JPEG encoder
            jgpEncoder = GetEncoder(ImageFormat.Jpeg);
            myEncoderParameters = new EncoderParameters(1);
            EncoderParameter myEncoderParameter = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, encoderCompression);
            myEncoderParameters.Param[0] = myEncoderParameter;

            // Start the capture thread
            mainThread = new Thread(new ThreadStart(MainDesktopLoop));
            mainThread.Start();
        }

        public void AddTunnel(MeshCentralTunnel tunnel)
        {
            lock (tunnels) { tunnels.Add(tunnel); tunnelAdded = true; }

            // Send displays
            SendDisplays(tunnel);

            // Send display locations and sizes
            SendDisplayInfo(tunnel);
        }

        public bool RemoveTunnel(MeshCentralTunnel tunnel)
        {
            lock (tunnels) { tunnels.Remove(tunnel); }
            return (tunnels.Count == 0);
        }

        public void Dispose()
        {
            mainThread = null;
            tunnels.Clear();
        }

        public void onBinaryData(MeshCentralTunnel tunnel, byte[] data, int off, int len)
        {
            try
            {
                if (len < 4) return;
                int cmd = ((data[off + 0] << 8) + data[off + 1]);
                int cmdlen = ((data[off + 2] << 8) + data[off + 3]);
                if (cmdlen != len) return;

                switch (cmd)
                {
                    case 1: // Key
                        {
                            if (cmdlen < 6) break;

                            // Check user rights. If view only, ignore this command
                            if ((tunnel.userRights != (long)MeshCentralTunnel.MeshRights.ADMIN) && ((tunnel.userRights & (long)MeshCentralTunnel.MeshRights.REMOTEVIEWONLY) != 0)) break;
                            bool limitedinput = false;
                            if ((tunnel.userRights != (long)MeshCentralTunnel.MeshRights.ADMIN) && ((tunnel.userRights & (long)MeshCentralTunnel.MeshRights.DESKLIMITEDINPUT) != 0)) { limitedinput = true; }

                            // Decode the command
                            int action = data[off + 4];
                            int key = data[off + 5];

                            // Check limited input
                            if (limitedinput && (action > 1)) break; // With limited input, extended keys are not allowed
                                                                     // TODO

                            // Setup the flags
                            uint flags = 0;
                            if (action == 1) { flags += (uint)(KeyEventF.KeyUp); }
                            if (action == 3) { flags += (uint)(KeyEventF.ExtendedKey | KeyEventF.KeyUp); }
                            if (action == 4) { flags += (uint)(KeyEventF.ExtendedKey); }

                            // Send the input
                            Input[] inputs = new Input[]
                            {
                            new Input
                            {
                                type = (int)SendInputEventType.KEYBOARD,
                                u = new InputUnion
                                {
                                    ki = new KeyboardInput
                                    {
                                        wVk = (ushort)key,
                                        wScan = (ushort)MapVirtualKey((int)key, MAPVK_VK_TO_VSC),
                                        dwFlags = flags,
                                        dwExtraInfo = GetMessageExtraInfo()
                                    }
                                }
                            }
                            };
                            SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(Input)));
                            break;
                        }
                    case 2: // Mouse
                        {
                            if (cmdlen < 10) break;

                            // Check user rights. If view only, ignore this command
                            if ((tunnel.userRights != (long)MeshCentralTunnel.MeshRights.ADMIN) && ((tunnel.userRights & (long)MeshCentralTunnel.MeshRights.REMOTEVIEWONLY) != 0)) break;

                            uint mouseFlags = (uint)((data[off + 4] << 8) + data[off + 5]);
                            if (mouseFlags == 0x88) break; // Ignore double click message
                            int x = (1024 * ((data[off + 6] << 8) + data[off + 7])) / encoderScaling;
                            int y = (1024 * ((data[off + 8] << 8) + data[off + 9])) / encoderScaling;
                            uint mouseWheel = 0;
                            if (cmdlen >= 12) { mouseWheel = (uint)((data[off + 10] << 8) + data[off + 11]); if (mouseWheel > 32768) { mouseWheel -= 65535; } }
                            try
                            {
                                if (currentDisplay != -1)
                                {
                                    Point tscreenlocation = Screen.AllScreens[currentDisplay].Bounds.Location;
                                    x += tscreenlocation.X;
                                    y += tscreenlocation.Y;
                                }
                                else
                                {
                                    Screen[] screens = Screen.AllScreens;
                                    Rectangle allScreens = Rectangle.Empty;
                                    foreach (Screen s in screens) { if (allScreens == Rectangle.Empty) { allScreens = s.Bounds; } else { allScreens = Rectangle.Union(allScreens, s.Bounds); } }
                                    x += allScreens.Left;
                                    y += allScreens.Top;
                                }
                            }
                            catch (Exception) { }

                            Cursor.Position = new Point(x, y);
                            if (mouseWheel != 0) { mouseFlags |= (uint)MouseEventFlags.MOUSEEVENTF_WHEEL; }
                            mouse_event(mouseFlags, x, y, (uint)mouseWheel, 0);
                            break;
                        }
                    case 5: // Settings
                        {
                            if (cmdlen < 6) break;
                            encoderType = data[off + 4];
                            int xencoderCompression = data[off + 5];
                            if (xencoderCompression < 0) { xencoderCompression = 0; }
                            if (xencoderCompression > 100) { xencoderCompression = 100; }
                            if (xencoderCompression != encoderCompression)
                            {
                                encoderCompression = xencoderCompression;
                                EncoderParameter myEncoderParameter = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, encoderCompression);
                                myEncoderParameters.Param[0] = myEncoderParameter;
                            }
                            if (cmdlen >= 8)
                            {
                                int xencoderScaling = ((data[off + 6] << 8) + data[off + 7]);
                                if (xencoderScaling > 1024) { xencoderScaling = 1024; }
                                if (xencoderScaling < 128) { xencoderScaling = 128; }
                                if (xencoderScaling != encoderScaling) { newEncoderScaling = xencoderScaling; encoderScalingChanged = true; }
                            }
                            if (cmdlen >= 10)
                            {
                                int xencoderFrameRate = ((data[off + 8] << 8) + data[off + 9]);
                                if (xencoderFrameRate < 50) { xencoderFrameRate = 50; }
                                if (xencoderFrameRate > 30000) { xencoderFrameRate = 30000; }
                                if (xencoderFrameRate != encoderFrameRate) { encoderFrameRate = xencoderFrameRate; }
                            }
                            break;
                        }
                    case 8:
                        {
                            break;
                        }
                    case 10: // Ctrl-Alt-Del
                        {
                            // Check user rights. If view only, ignore this command
                            if ((tunnel.userRights != (long)MeshCentralTunnel.MeshRights.ADMIN) && ((tunnel.userRights & (long)MeshCentralTunnel.MeshRights.REMOTEVIEWONLY) != 0)) break;
                            if ((tunnel.userRights != (long)MeshCentralTunnel.MeshRights.ADMIN) && ((tunnel.userRights & (long)MeshCentralTunnel.MeshRights.DESKLIMITEDINPUT) != 0)) break;
                            // This is not supported since we are not running as a admin service
                            break;
                        }
                    case 11: // Query displays
                        {
                            SendDisplays(tunnel);
                            break;
                        }
                    case 12: // Set display
                        {
                            if (cmdlen < 6) break;
                            int selectedDisplay = ((data[off + 4] << 8) + data[off + 5]);
                            if (selectedDisplay == 65535)
                            {
                                currentDisplay = -1;
                            }
                            else
                            {
                                if (selectedDisplay < 1) break;
                                if (selectedDisplay > Screen.AllScreens.Length) break;
                                currentDisplay = (selectedDisplay - 1);
                            }
                            break;
                        }
                    case 15: // Touch
                        {
                            // Check user rights. If view only, ignore this command
                            if ((tunnel.userRights != (long)MeshCentralTunnel.MeshRights.ADMIN) && ((tunnel.userRights & (long)MeshCentralTunnel.MeshRights.REMOTEVIEWONLY) != 0)) break;

                            break;
                        }
                    case 82: // Display location and size
                        {
                            SendDisplayInfo(tunnel);
                            break;
                        }
                    case 85: // Unicode Key
                        {
                            if (cmdlen < 7) break;

                            // Check user rights. If view only, ignore this command
                            if ((tunnel.userRights != (long)MeshCentralTunnel.MeshRights.ADMIN) && ((tunnel.userRights & (long)MeshCentralTunnel.MeshRights.REMOTEVIEWONLY) != 0)) break;
                            //bool limitedinput = false;
                            //if ((tunnel.userRights != (long)MeshCentralTunnel.MeshRights.ADMIN) && ((tunnel.userRights & (long)MeshCentralTunnel.MeshRights.DESKLIMITEDINPUT) != 0)) { limitedinput = true; }

                            // Decode the command
                            int action = data[off + 4];
                            int key = (data[off + 5] << 8) + data[off + 6];

                            // Check limited input
                            // TODO

                            // Setup the flags
                            uint flags = (uint)(KeyEventF.Unicode);
                            if (action == 1) { flags += (uint)(KeyEventF.KeyUp); }

                            // Send the input
                            Input[] inputs = new Input[]
                            {
                            new Input
                            {
                                type = (int)SendInputEventType.KEYBOARD,
                                u = new InputUnion
                                {
                                    ki = new KeyboardInput
                                    {
                                        wVk = 0,
                                        wScan = (ushort)key,
                                        dwFlags = flags,
                                        dwExtraInfo = GetMessageExtraInfo()
                                    }
                                }
                            }
                            };
                            SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(Input)));
                            break;
                        }
                    case 87:
                        {
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                // If requested, store this exception in the log
                if (exceptionLogState == 0) { Log(ex.ToString()); exceptionLogState++; }
            }
        }

        private void SendDisplayInfo(MeshCentralTunnel tunnel)
        {
            Screen[] screens = Screen.AllScreens;
            byte[] buf = new byte[4 + (screens.Length * 10)];
            buf[1] = 82; // Display location and size
            buf[2] = (byte)(buf.Length >> 8); // Command Length
            buf[3] = (byte)(buf.Length & 0xFF); // Command Length
            int ptr = 4;
            int displayid = 1;
            foreach (Screen screen in screens)
            {
                buf[ptr + 0] = (byte)(displayid >> 8); // Display ID
                buf[ptr + 1] = (byte)(displayid & 0xFF);
                buf[ptr + 2] = (byte)(screen.Bounds.Left >> 8); // Location X
                buf[ptr + 3] = (byte)(screen.Bounds.Left & 0xFF);
                buf[ptr + 4] = (byte)(screen.Bounds.Top >> 8); // Location Y
                buf[ptr + 5] = (byte)(screen.Bounds.Top & 0xFF);
                buf[ptr + 6] = (byte)(screen.Bounds.Width >> 8); // Width
                buf[ptr + 7] = (byte)(screen.Bounds.Width & 0xFF);
                buf[ptr + 8] = (byte)(screen.Bounds.Height >> 8); // Height
                buf[ptr + 9] = (byte)(screen.Bounds.Height & 0xFF);
                displayid++;
                ptr += 10;
            }
            //string hex = BitConverter.ToString(buf).Replace("-", string.Empty);
            tunnel.WebSocket.SendBinary(buf, 0, buf.Length);
        }

        private void SendDisplays(MeshCentralTunnel tunnel)
        {
            Screen[] screens = Screen.AllScreens;
            int screenCount = screens.Length;
            if (screenCount > 1) { screenCount++; }
            byte[] buf = new byte[8 + (screenCount * 2)];
            buf[1] = 11; // Get Displays
            buf[3] = (byte)buf.Length; // Command Length
            buf[5] = (byte)screenCount; // Display count
            if (currentDisplay == -1) { buf[6] = 255; buf[7] = 255; } else { buf[7] = (byte)currentDisplay; } // Selected Display
            int ptr = 8;
            for (var i = 0; i < screens.Length; i++) { buf[ptr] = 0; buf[ptr + 1] = (byte)(i + 1); ptr += 2; }
            if (screens.Length > 1) { buf[ptr] = 255; buf[ptr + 1] = 255; }

            // Send normal command
            tunnel.WebSocket.SendBinary(buf, 0, buf.Length);
        }

        // Send binary data to all tunnels
        private void SendBinaryAllTunnels(byte[] buf)
        {
            lock (tunnels) {
                foreach (MeshCentralTunnel tunnel in tunnels) {
                    try { tunnel.WebSocket.SendBinary(buf); } catch (Exception) { }
                }
            }
        }

        // Send binary data to all tunnels
        private void SendBinaryAllTunnels(byte[] buf, int off, int len)
        {
            lock (tunnels) {
                foreach (MeshCentralTunnel tunnel in tunnels) {
                    try { tunnel.WebSocket.SendBinary(buf, off, len); } catch (Exception) { }
                }
            }
        }

        // Set console text for all tunnels
        private void SetConsoleTextAllTunnels(string msg, int msgid, string msgargs, int timeout)
        {
            lock (tunnels) {
                foreach (MeshCentralTunnel tunnel in tunnels) {
                    try { tunnel.setConsoleText(msg, msgid, msgargs, timeout); } catch (Exception) { }
                }
            }
        }

        // Clear console text for all tunnels
        private void ClearConsoleTextAllTunnels()
        {
            lock (tunnels) {
                foreach (MeshCentralTunnel tunnel in tunnels) {
                    try { tunnel.clearConsoleText(); } catch (Exception) { }
                }
            }
        }

        private void MainDesktopLoop()
        {
            while (true)
            {
                if (mainThread == null) return;
                Thread.Sleep(encoderFrameRate);
                if (mainThread == null) return;

                try
                {
                    // Look to see that is the tunnel with the maximum number of pending bytes in the outbound buffer
                    long maxPendingOutboundBytes = 0;
                    lock (tunnels) { foreach (MeshCentralTunnel tunnel in tunnels) { if (tunnel.WebSocket.PendingSendLength > maxPendingOutboundBytes) { maxPendingOutboundBytes = tunnel.WebSocket.PendingSendLength; } } }
                    if (maxPendingOutboundBytes > 1024) { continue; } // If there is data pending in the outbound buffer, skip this round.

                    int pointerType = 0;
                    try
                    {
                        // Take a look at the mouse cursor
                        // var mouseCursors = ['default', 'progress', 'crosshair', 'pointer', 'help', 'text', 'no-drop', 'move', 'nesw-resize', 'ns-resize', 'nwse-resize', 'w-resize', 'alias', 'wait', 'none', 'not-allowed', 'col-resize', 'row-resize', 'copy', 'zoom-in', 'zoom-out'];
                        CURSORINFO pci;
                        pci.cbSize = Marshal.SizeOf(typeof(CURSORINFO));
                        GetCursorInfo(out pci);
                        if (pci.hCursor == Cursors.Default.Handle) { pointerType = 0; } // Default
                        else if (pci.hCursor == Cursors.Cross.Handle) { pointerType = 2; } // Crosshair
                        else if (pci.hCursor == Cursors.Hand.Handle) { pointerType = 3; } // Pointer
                        else if (pci.hCursor == Cursors.Help.Handle) { pointerType = 4; } // Help
                        else if (pci.hCursor == Cursors.IBeam.Handle) { pointerType = 5; } // Text
                        else if (pci.hCursor == Cursors.SizeNESW.Handle) { pointerType = 8; } // nesw-resize
                        else if (pci.hCursor == Cursors.SizeNS.Handle) { pointerType = 9; } // ns-resize
                        else if (pci.hCursor == Cursors.SizeNWSE.Handle) { pointerType = 10; } // nwse-resize
                        else if (pci.hCursor == Cursors.SizeWE.Handle) { pointerType = 11; } // w-resize
                        else if (pci.hCursor == Cursors.WaitCursor.Handle) { pointerType = 13; } // Wait
                        else if (pci.hCursor == Cursors.No.Handle) { pointerType = 15; } // not-allowed
                        else if (pci.hCursor == Cursors.VSplit.Handle) { pointerType = 16; } // col-resize
                        else if (pci.hCursor == Cursors.HSplit.Handle) { pointerType = 17; } // col-resize
                        else if (pci.hCursor == Cursors.AppStarting.Handle) { pointerType = 13; } // Wait
                    }
                    catch (Exception) { } // If we can't get the mouse pointer, use default.
                    if (mousePointer != pointerType) // Update the mouse pointer
                    {
                        byte[] mousePointerCmd = new byte[5];
                        mousePointerCmd[1] = 88;
                        mousePointerCmd[3] = 5;
                        mousePointerCmd[4] = (byte)pointerType;
                        SendBinaryAllTunnels(mousePointerCmd, 0, 5);
                        mousePointer = pointerType;
                    }

                    // Get the size and location of the currently selected screen
                    Size tscreensize = Size.Empty;
                    Point tscreenlocation = Point.Empty;
                    Screen[] screens = Screen.AllScreens;
                    if (currentDisplay == -1) // We are looking at all screens
                    {
                        Rectangle allScreens = Rectangle.Empty;
                        foreach (Screen s in screens)
                        {
                            if (allScreens == Rectangle.Empty) {
                                allScreens = s.Bounds;
                            } else {
                                allScreens = Rectangle.Union(allScreens, s.Bounds);
                            }
                        }
                        if (allScreens != Rectangle.Empty)
                        {
                            tscreensize = new Size(allScreens.Width, allScreens.Height);
                            tscreenlocation = new Point(allScreens.Left, allScreens.Top);
                        }
                    } else if ((currentDisplay >= 0) && (currentDisplay < screens.Length)) { // We are looking at a specific screen
                        tscreensize = Screen.AllScreens[currentDisplay].Bounds.Size;
                        tscreenlocation = Screen.AllScreens[currentDisplay].Bounds.Location;
                    }

                    // If the size of the screen does not match the current client set size, update the client
                    if ((ScreenSize.Width != tscreensize.Width) || (ScreenSize.Height != tscreensize.Height) || (captureBitmap == null) || (encoderScalingChanged == true) || (tunnelAdded == true))
                    {
                        tunnelAdded = false;
                        encoderScaling = newEncoderScaling;
                        encoderScalingChanged = false;
                        ScreenSize = tscreensize;
                        screenScaleWidth = ((encoderScaling * ScreenSize.Width) / 1024);
                        screenScaleHeight = ((encoderScaling * ScreenSize.Height) / 1024);

                        byte[] screenSizeCmd = new byte[8];
                        screenSizeCmd[1] = 7; // Command 7, screen size
                        screenSizeCmd[3] = 8; // Command size, 8 bytes
                        screenSizeCmd[4] = (byte)(screenScaleWidth >> 8);
                        screenSizeCmd[5] = (byte)(screenScaleWidth & 0xFF);
                        screenSizeCmd[6] = (byte)(screenScaleHeight >> 8);
                        screenSizeCmd[7] = (byte)(screenScaleHeight & 0xFF);
                        SendBinaryAllTunnels(screenSizeCmd);

                        // Update the main bitmap and setup the CRC's.
                        screenRect = new Rectangle(0, 0, screenScaleWidth, screenScaleHeight);
                        captureBitmap = new Bitmap(ScreenSize.Width, ScreenSize.Height, PixelFormat.Format24bppRgb);
                        tilesWide = tilesFullWide = (screenScaleWidth >> 6);
                        tilesHigh = tilesFullHigh = (screenScaleHeight >> 6);
                        tilesRemainingWidth = (screenScaleWidth % 64);
                        tilesRemainingHeight = (screenScaleHeight % 64);
                        if (tilesRemainingWidth != 0) { tilesWide++; }
                        if (tilesRemainingHeight != 0) { tilesHigh++; }
                        tilesCount = (tilesWide * tilesHigh);
                        oldcrcs = new ulong[tilesCount]; // 64 x 64 tiles
                        newcrcs = new ulong[tilesCount]; // 64 x 64 tiles
                    }

                    // Capture the screen & scale it if needed
                    Graphics captureGraphics = Graphics.FromImage(captureBitmap);
                    try
                    {
                        captureGraphics.CopyFromScreen(tscreenlocation.X, tscreenlocation.Y, 0, 0, ScreenSize);
                    }
                    catch (Exception)
                    {
                        // Attempt to switch desktop display
                        if (UnableToCaptureShowing == false) { SetConsoleTextAllTunnels("Unable to capture display", 6, null, 0); UnableToCaptureShowing = true; }
                        SwitchToActiveDesktop(); // Attempt to switch desktops
                        continue;
                    }

                    Bitmap scaledCaptureBitmap = captureBitmap;
                    if (encoderScaling != 1024) { scaledCaptureBitmap = new Bitmap(captureBitmap, screenScaleWidth, screenScaleHeight); }

                    // Compute all tile CRC's
                    computeAllCRCs(scaledCaptureBitmap);

                    // Compute how many tiles have changed, do nothing if no changes
                    int changedTiles = 0;
                    for (var i = 0; i < tilesCount; i++) { if (oldcrcs[i] != newcrcs[i]) { changedTiles++; } }
                    if (changedTiles > 0)
                    {
                        // If 85% of the all tiles have changed, send the entire screen
                        if ((changedTiles * 100) >= (tilesCount * 85))
                        {
                            SendBitmap(0, 0, scaledCaptureBitmap);
                            for (var i = 0; i < tilesCount; i++) { oldcrcs[i] = newcrcs[i]; }
                        }
                        else
                        {
                            // Send all changed tiles
                            // This version has horizontal & vertial optimization, JPEG as wide as possible then as high as possible
                            int sendx = -1;
                            int sendy = 0;
                            int sendw = 0;
                            for (int i = 0; i < tilesHigh; i++)
                            {
                                for (int j = 0; j < tilesWide; j++)
                                {
                                    int tileNumber = (i * tilesWide) + j;
                                    if (oldcrcs[tileNumber] != newcrcs[tileNumber])
                                    {
                                        oldcrcs[tileNumber] = newcrcs[tileNumber];
                                        if (sendx == -1) { sendx = j; sendy = i; sendw = 1; } else { sendw += 1; }
                                    }
                                    else
                                    {
                                        if (sendx != -1) { SendSubBitmapRow(scaledCaptureBitmap, sendx, sendy, sendw); sendx = -1; }
                                    }
                                }
                                if (sendx != -1) { SendSubBitmapRow(scaledCaptureBitmap, sendx, sendy, sendw); sendx = -1; }
                            }
                            if (sendx != -1) { SendSubBitmapRow(scaledCaptureBitmap, sendx, sendy, sendw); sendx = -1; }
                        }
                    }

                    // Everything went ok
                    exceptionLogState = 0;
                    if (UnableToCaptureShowing == true) { ClearConsoleTextAllTunnels(); UnableToCaptureShowing = false; }
                }
                catch (Exception ex) { Log(ex.ToString()); }
            }
        }

        private void SwitchToActiveDesktop()
        {
            try
            {
                IntPtr inputDesktop = Win32Api.OpenInputDesktop(0, true, Win32Api.DESKTOP_Createmenu | Win32Api.DESKTOP_Createwindow | Win32Api.DESKTOP_Enumerate | Win32Api.DESKTOP_Hookcontrol | Win32Api.DESKTOP_Writeobjects | Win32Api.DESKTOP_Readobjects | Win32Api.DESKTOP_Switchdesktop | Win32Api.GENERIC_WRITE);
                if (inputDesktop == IntPtr.Zero) return;
                if (Win32Api.SetThreadDesktop(inputDesktop))
                {
                    byte[] desktopNameBuffer = new byte[1024];
                    uint desktopNameBufferNeeded = 0;
                    if (Win32Api.GetUserObjectInformation(inputDesktop, Win32Api.UOI_NAME, desktopNameBuffer, (uint)desktopNameBuffer.Length, out desktopNameBufferNeeded))
                    {
                        string n = System.Text.UTF8Encoding.UTF8.GetString(desktopNameBuffer, 0, (int)desktopNameBufferNeeded);
                        if (n.EndsWith("\0")) { n = n.Substring(0, n.Length - 1); }
                        //Log("NewDesktopName: " + n);
                    }
                }
                else
                {
                    //Log("SetThreadDesktop Fail: " + inputDesktop.ToString());
                }
                Win32Api.CloseDesktop(inputDesktop);
            }
            catch (Exception ex) {
                Log("SwitchToActiveDesktop Exception: " + ex.ToString());
            }
        }

        // See if we can expand a row downwards
        private void SendSubBitmapRow(Bitmap image, int x, int y, int w) {
            int h;
            bool exit = false;
            for (h = (y + 1); h < tilesHigh; h++)
            {
                // Check if the row is all different
                for (int xx = x; xx < (x + w); xx++) { int tileNumber = (h * tilesWide) + xx; if (oldcrcs[tileNumber] == newcrcs[tileNumber]) { exit = true; break; } }
                // If all different set the CRC's to the same, otherwise exit.
                if (!exit) { for (int xx = x; xx < (x + w); xx++) { int tileNumber = (h * tilesWide) + xx; oldcrcs[tileNumber] = newcrcs[tileNumber]; } } else break;
            }
            h -= y;
            //Console.WriteLine("SendSubBitmapRow: " + x + ", " + y + ", " + w + ", " + h);

            SendSubBitmap(image, x * 64, y * 64, w * 64, h * 64);
        }

        private void computeAllCRCs(Bitmap image)
        {
            // Lock the bitmap's bits.  
            Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);
            BitmapData bmpData = image.LockBits(rect, ImageLockMode.ReadOnly, image.PixelFormat);

            // Clear all CRC's
            for (int i = 0; i < tilesCount; i++) { newcrcs[i] = 0; }

            // Get the address of the first line.
            IntPtr bitmapPtr = bmpData.Scan0;
            int ptr = 0;
            int tileWidthReads = 24;
            int tileWidthRemainReads64 = (tilesRemainingWidth * 3) / 8;
            int tileWidthRemainReads8 = (tilesRemainingWidth * 3) % 8;
            if (image.PixelFormat == PixelFormat.Format32bppArgb) {
                tileWidthReads = 32;
                tileWidthRemainReads64 = (tilesRemainingWidth * 4) / 8;
                tileWidthRemainReads8 = (tilesRemainingWidth * 4) % 8;
            }

            // Handle all of the full tiles for the height
            for (int i = 0; i < tilesFullHigh; i++) {
                for (int j = 0; j < 64; j++) {
                    ptr = (((i * 64) + j) * bmpData.Stride);
                    for (int k = 0; k < tilesFullWide; k++) {
                        for (int l = 0; l < tileWidthReads; l++) {
                            newcrcs[(i * tilesWide) + k] = CRC(Marshal.ReadInt64(bitmapPtr, ptr), newcrcs[(i * tilesWide) + k]);
                            ptr += 8;
                        }
                    }
                    if (tilesRemainingWidth > 0)
                    {
                        // Handle the reminder of the width
                        for (int l = 0; l < tileWidthRemainReads64; l++)
                        {
                            newcrcs[(i * tilesWide) + tilesFullWide] = CRC(Marshal.ReadInt64(bitmapPtr, ptr), newcrcs[(i * tilesWide) + tilesFullWide]);
                            ptr += 8;
                        }
                        for (int l = 0; l < tileWidthRemainReads8; l++)
                        {
                            newcrcs[(i * tilesWide) + tilesFullWide] = CRC(Marshal.ReadByte(bitmapPtr, ptr), newcrcs[(i * tilesWide) + tilesFullWide]);
                            ptr += 1;
                        }
                    }
                }
            }

            // Handle the reminder of the height
            if (tilesRemainingHeight > 0)
            {
                for (int j = 0; j < tilesRemainingHeight; j++)
                {
                    for (int k = 0; k < tilesFullWide; k++)
                    {
                        for (int l = 0; l < tileWidthReads; l++)
                        {
                            newcrcs[(tilesFullHigh * tilesWide) + k] = CRC(Marshal.ReadInt64(bitmapPtr, ptr), newcrcs[(tilesFullHigh * tilesWide) + k]);
                            ptr += 8;
                        }
                    }
                    if (tilesRemainingWidth > 0)
                    {
                        // Handle the reminder of the width
                        for (int l = 0; l < tileWidthRemainReads64; l++)
                        {
                            newcrcs[(tilesFullHigh * tilesWide) + tilesFullWide] = CRC(Marshal.ReadInt64(bitmapPtr, ptr), newcrcs[(tilesFullHigh * tilesWide) + tilesFullWide]);
                            ptr += 8;
                        }
                        for (int l = 0; l < tileWidthRemainReads8; l++)
                        {
                            newcrcs[(tilesFullHigh * tilesWide) + tilesFullWide] = CRC(Marshal.ReadByte(bitmapPtr, ptr), newcrcs[(tilesFullHigh * tilesWide) + tilesFullWide]);
                            ptr += 1;
                        }
                    }
                }
            }

            // Unlock the bits.
            image.UnlockBits(bmpData);
        }

        private void SendSubBitmap(Bitmap image, int x, int y, int w, int h)
        {
            SendBitmap(x, y, image.Clone(Rectangle.Intersect(screenRect, new Rectangle(x, y, w, h)), image.PixelFormat));
        }

        private void SendBitmap(int x, int y, Bitmap image)
        {
            MemoryStream memoryBuffer = new MemoryStream();
            memoryBuffer.Write(skipHeader, 0, 16); // Skip the first 16 bytes
            image.Save(memoryBuffer, jgpEncoder, myEncoderParameters); // Write the JPEG image at 100% scale
            byte[] imageCmd = memoryBuffer.GetBuffer();
            int cmdlen = (int)(memoryBuffer.Length - 8);

            // Jumbo command
            if (memoryBuffer.Length > 65000)
            {
                imageCmd[0] = 0;
                imageCmd[1] = 27; // Command 27, JUMBO
                imageCmd[2] = 0;
                imageCmd[3] = 8; // Command size, 8 bytes
                imageCmd[4] = (byte)((cmdlen >> 24) & 0xFF);
                imageCmd[5] = (byte)((cmdlen >> 16) & 0xFF);
                imageCmd[6] = (byte)((cmdlen >> 8) & 0xFF);
                imageCmd[7] = (byte)((cmdlen) & 0xFF);

                // Tile command
                imageCmd[8] = 0;
                imageCmd[9] = 3; // Command 3, tile
                imageCmd[10] = 0;
                imageCmd[11] = 0;
                imageCmd[12] = (byte)(x >> 8);   // X
                imageCmd[13] = (byte)(x & 0xFF); // X
                imageCmd[14] = (byte)(y >> 8);   // Y
                imageCmd[15] = (byte)(y & 0xFF); // Y

                // Send with JUMBO command
                SendBinaryAllTunnels(imageCmd, 0, cmdlen + 8);
            }
            else
            {
                // Tile command
                imageCmd[8] = 0;
                imageCmd[9] = 3; // Command 3, tile
                imageCmd[10] = (byte)(cmdlen >> 8);   // Command size, 8 bytes + image
                imageCmd[11] = (byte)(cmdlen & 0xFF); // Command size, 8 bytes + image
                imageCmd[12] = (byte)(x >> 8);   // X
                imageCmd[13] = (byte)(x & 0xFF); // X
                imageCmd[14] = (byte)(y >> 8);   // Y
                imageCmd[15] = (byte)(y & 0xFF); // Y

                // Send normal command
                SendBinaryAllTunnels(imageCmd, 8, cmdlen);
            }
        }

        private ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs) { if (codec.FormatID == format.Guid) { return codec; } }
            return null;
        }

        private ulong CRC(long a, ulong b) { return Crc64.Compute(BitConverter.GetBytes(a), b); }
    }

    public class Crc64
    {
        private static readonly ulong[] Table = {
            0x0000000000000000, 0x7ad870c830358979,
            0xf5b0e190606b12f2, 0x8f689158505e9b8b,
            0xc038e5739841b68f, 0xbae095bba8743ff6,
            0x358804e3f82aa47d, 0x4f50742bc81f2d04,
            0xab28ecb46814fe75, 0xd1f09c7c5821770c,
            0x5e980d24087fec87, 0x24407dec384a65fe,
            0x6b1009c7f05548fa, 0x11c8790fc060c183,
            0x9ea0e857903e5a08, 0xe478989fa00bd371,
            0x7d08ff3b88be6f81, 0x07d08ff3b88be6f8,
            0x88b81eabe8d57d73, 0xf2606e63d8e0f40a,
            0xbd301a4810ffd90e, 0xc7e86a8020ca5077,
            0x4880fbd87094cbfc, 0x32588b1040a14285,
            0xd620138fe0aa91f4, 0xacf86347d09f188d,
            0x2390f21f80c18306, 0x594882d7b0f40a7f,
            0x1618f6fc78eb277b, 0x6cc0863448deae02,
            0xe3a8176c18803589, 0x997067a428b5bcf0,
            0xfa11fe77117cdf02, 0x80c98ebf2149567b,
            0x0fa11fe77117cdf0, 0x75796f2f41224489,
            0x3a291b04893d698d, 0x40f16bccb908e0f4,
            0xcf99fa94e9567b7f, 0xb5418a5cd963f206,
            0x513912c379682177, 0x2be1620b495da80e,
            0xa489f35319033385, 0xde51839b2936bafc,
            0x9101f7b0e12997f8, 0xebd98778d11c1e81,
            0x64b116208142850a, 0x1e6966e8b1770c73,
            0x8719014c99c2b083, 0xfdc17184a9f739fa,
            0x72a9e0dcf9a9a271, 0x08719014c99c2b08,
            0x4721e43f0183060c, 0x3df994f731b68f75,
            0xb29105af61e814fe, 0xc849756751dd9d87,
            0x2c31edf8f1d64ef6, 0x56e99d30c1e3c78f,
            0xd9810c6891bd5c04, 0xa3597ca0a188d57d,
            0xec09088b6997f879, 0x96d1784359a27100,
            0x19b9e91b09fcea8b, 0x636199d339c963f2,
            0xdf7adabd7a6e2d6f, 0xa5a2aa754a5ba416,
            0x2aca3b2d1a053f9d, 0x50124be52a30b6e4,
            0x1f423fcee22f9be0, 0x659a4f06d21a1299,
            0xeaf2de5e82448912, 0x902aae96b271006b,
            0x74523609127ad31a, 0x0e8a46c1224f5a63,
            0x81e2d7997211c1e8, 0xfb3aa75142244891,
            0xb46ad37a8a3b6595, 0xceb2a3b2ba0eecec,
            0x41da32eaea507767, 0x3b024222da65fe1e,
            0xa2722586f2d042ee, 0xd8aa554ec2e5cb97,
            0x57c2c41692bb501c, 0x2d1ab4dea28ed965,
            0x624ac0f56a91f461, 0x1892b03d5aa47d18,
            0x97fa21650afae693, 0xed2251ad3acf6fea,
            0x095ac9329ac4bc9b, 0x7382b9faaaf135e2,
            0xfcea28a2faafae69, 0x8632586aca9a2710,
            0xc9622c4102850a14, 0xb3ba5c8932b0836d,
            0x3cd2cdd162ee18e6, 0x460abd1952db919f,
            0x256b24ca6b12f26d, 0x5fb354025b277b14,
            0xd0dbc55a0b79e09f, 0xaa03b5923b4c69e6,
            0xe553c1b9f35344e2, 0x9f8bb171c366cd9b,
            0x10e3202993385610, 0x6a3b50e1a30ddf69,
            0x8e43c87e03060c18, 0xf49bb8b633338561,
            0x7bf329ee636d1eea, 0x012b592653589793,
            0x4e7b2d0d9b47ba97, 0x34a35dc5ab7233ee,
            0xbbcbcc9dfb2ca865, 0xc113bc55cb19211c,
            0x5863dbf1e3ac9dec, 0x22bbab39d3991495,
            0xadd33a6183c78f1e, 0xd70b4aa9b3f20667,
            0x985b3e827bed2b63, 0xe2834e4a4bd8a21a,
            0x6debdf121b863991, 0x1733afda2bb3b0e8,
            0xf34b37458bb86399, 0x8993478dbb8deae0,
            0x06fbd6d5ebd3716b, 0x7c23a61ddbe6f812,
            0x3373d23613f9d516, 0x49aba2fe23cc5c6f,
            0xc6c333a67392c7e4, 0xbc1b436e43a74e9d,
            0x95ac9329ac4bc9b5, 0xef74e3e19c7e40cc,
            0x601c72b9cc20db47, 0x1ac40271fc15523e,
            0x5594765a340a7f3a, 0x2f4c0692043ff643,
            0xa02497ca54616dc8, 0xdafce7026454e4b1,
            0x3e847f9dc45f37c0, 0x445c0f55f46abeb9,
            0xcb349e0da4342532, 0xb1eceec59401ac4b,
            0xfebc9aee5c1e814f, 0x8464ea266c2b0836,
            0x0b0c7b7e3c7593bd, 0x71d40bb60c401ac4,
            0xe8a46c1224f5a634, 0x927c1cda14c02f4d,
            0x1d148d82449eb4c6, 0x67ccfd4a74ab3dbf,
            0x289c8961bcb410bb, 0x5244f9a98c8199c2,
            0xdd2c68f1dcdf0249, 0xa7f41839ecea8b30,
            0x438c80a64ce15841, 0x3954f06e7cd4d138,
            0xb63c61362c8a4ab3, 0xcce411fe1cbfc3ca,
            0x83b465d5d4a0eece, 0xf96c151de49567b7,
            0x76048445b4cbfc3c, 0x0cdcf48d84fe7545,
            0x6fbd6d5ebd3716b7, 0x15651d968d029fce,
            0x9a0d8ccedd5c0445, 0xe0d5fc06ed698d3c,
            0xaf85882d2576a038, 0xd55df8e515432941,
            0x5a3569bd451db2ca, 0x20ed197575283bb3,
            0xc49581ead523e8c2, 0xbe4df122e51661bb,
            0x3125607ab548fa30, 0x4bfd10b2857d7349,
            0x04ad64994d625e4d, 0x7e7514517d57d734,
            0xf11d85092d094cbf, 0x8bc5f5c11d3cc5c6,
            0x12b5926535897936, 0x686de2ad05bcf04f,
            0xe70573f555e26bc4, 0x9ddd033d65d7e2bd,
            0xd28d7716adc8cfb9, 0xa85507de9dfd46c0,
            0x273d9686cda3dd4b, 0x5de5e64efd965432,
            0xb99d7ed15d9d8743, 0xc3450e196da80e3a,
            0x4c2d9f413df695b1, 0x36f5ef890dc31cc8,
            0x79a59ba2c5dc31cc, 0x037deb6af5e9b8b5,
            0x8c157a32a5b7233e, 0xf6cd0afa9582aa47,
            0x4ad64994d625e4da, 0x300e395ce6106da3,
            0xbf66a804b64ef628, 0xc5bed8cc867b7f51,
            0x8aeeace74e645255, 0xf036dc2f7e51db2c,
            0x7f5e4d772e0f40a7, 0x05863dbf1e3ac9de,
            0xe1fea520be311aaf, 0x9b26d5e88e0493d6,
            0x144e44b0de5a085d, 0x6e963478ee6f8124,
            0x21c640532670ac20, 0x5b1e309b16452559,
            0xd476a1c3461bbed2, 0xaeaed10b762e37ab,
            0x37deb6af5e9b8b5b, 0x4d06c6676eae0222,
            0xc26e573f3ef099a9, 0xb8b627f70ec510d0,
            0xf7e653dcc6da3dd4, 0x8d3e2314f6efb4ad,
            0x0256b24ca6b12f26, 0x788ec2849684a65f,
            0x9cf65a1b368f752e, 0xe62e2ad306bafc57,
            0x6946bb8b56e467dc, 0x139ecb4366d1eea5,
            0x5ccebf68aecec3a1, 0x2616cfa09efb4ad8,
            0xa97e5ef8cea5d153, 0xd3a62e30fe90582a,
            0xb0c7b7e3c7593bd8, 0xca1fc72bf76cb2a1,
            0x45775673a732292a, 0x3faf26bb9707a053,
            0x70ff52905f188d57, 0x0a2722586f2d042e,
            0x854fb3003f739fa5, 0xff97c3c80f4616dc,
            0x1bef5b57af4dc5ad, 0x61372b9f9f784cd4,
            0xee5fbac7cf26d75f, 0x9487ca0fff135e26,
            0xdbd7be24370c7322, 0xa10fceec0739fa5b,
            0x2e675fb4576761d0, 0x54bf2f7c6752e8a9,
            0xcdcf48d84fe75459, 0xb71738107fd2dd20,
            0x387fa9482f8c46ab, 0x42a7d9801fb9cfd2,
            0x0df7adabd7a6e2d6, 0x772fdd63e7936baf,
            0xf8474c3bb7cdf024, 0x829f3cf387f8795d,
            0x66e7a46c27f3aa2c, 0x1c3fd4a417c62355,
            0x935745fc4798b8de, 0xe98f353477ad31a7,
            0xa6df411fbfb21ca3, 0xdc0731d78f8795da,
            0x536fa08fdfd90e51, 0x29b7d047efec8728,
        };

        public static ulong Compute(byte[] s, ulong crc = 0)
        {
            for (int j = 0; j < s.Length; j++) { crc = Crc64.Table[(byte)(crc ^ s[j])] ^ (crc >> 8); }
            return crc;
        }

    }
}

