using System;
using System.IO;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace MeshAssistant
{
    class MeshCentralDesktop
    {
        private MeshCentralTunnel parent;
        private Thread mainThread = null;
        private int currentDisplay = -1;
        private Size ScreenSize = Size.Empty;
        private ImageCodecInfo jgpEncoder;
        private EncoderParameters myEncoderParameters;
        private MemoryStream memoryBuffer = new MemoryStream();
        private byte[] skipHeader = new byte[16];
        private Bitmap captureBitmap = null;
        private int encoderType = 1;
        private int encoderCompression = 30;
        private int encoderScaling = 1024;
        private int newEncoderScaling = 1024;
        private bool encoderScalingChanged = false;
        private int encoderFrameRate = 100;
        private int mousePointer = 0;
        private long[] oldcrcs = null;
        private long[] newcrcs = null;
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
            [FieldOffset(0)]
            public MouseInputData mi; // mouse input

            [FieldOffset(0)]
            public KEYBDINPUT ki; // keyboard input

            [FieldOffset(0)]
            public HARDWAREINPUT hi; // hardware input
        }

        struct MouseInputData
        {
            public int dx;
            public int dy;
            public uint mouseData;
            public MouseEventFlags dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

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

        public MeshCentralDesktop(MeshCentralTunnel parent)
        {
            this.parent = parent;

            // Setup the JPEG encoder
            jgpEncoder = GetEncoder(ImageFormat.Jpeg);
            myEncoderParameters = new EncoderParameters(1);
            EncoderParameter myEncoderParameter = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, encoderCompression);
            myEncoderParameters.Param[0] = myEncoderParameter;

            // Send displays
            SendDisplays();

            // Start the capture thread
            mainThread = new Thread(new ThreadStart(MainDesktopLoop));
            mainThread.Start();
        }

        public void Dispose()
        {
            mainThread = null;
            parent = null;
        }

        public void onBinaryData(byte[] data, int off, int len)
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
                        if ((parent.userRights != (long)MeshCentralTunnel.MeshRights.ADMIN) && ((parent.userRights & (long)MeshCentralTunnel.MeshRights.REMOTEVIEWONLY) != 0)) break;
                        bool limitedinput = false;
                        if ((parent.userRights != (long)MeshCentralTunnel.MeshRights.ADMIN) && ((parent.userRights & (long)MeshCentralTunnel.MeshRights.DESKLIMITEDINPUT) != 0)) { limitedinput = true; }

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
                        if ((parent.userRights != (long)MeshCentralTunnel.MeshRights.ADMIN) && ((parent.userRights & (long)MeshCentralTunnel.MeshRights.REMOTEVIEWONLY) != 0)) break;

                        int b = ((data[off + 4] << 8) + data[off + 5]);
                        int x = (1024 * ((data[off + 6] << 8) + data[off + 7])) / encoderScaling;
                        int y = (1024 * ((data[off + 8] << 8) + data[off + 9])) / encoderScaling;
                        if (currentDisplay != -1) { try { Point tscreenlocation = Screen.AllScreens[currentDisplay].Bounds.Location; x += tscreenlocation.X; y += tscreenlocation.Y; } catch (Exception) { } }
                        int w = 0;
                        if (cmdlen >= 12) { w = (int)((data[off + 10] << 8) + data[off + 11]); if (w > 32768) { w -= 65535; } }
                        Cursor.Position = new Point(x, y);
                        if ((b &  2) != 0) { mouse_event((int)(MouseEventFlags.MOUSEEVENTF_LEFTDOWN), x, y, 0, 0); }
                        if ((b &  4) != 0) { mouse_event((int)(MouseEventFlags.MOUSEEVENTF_LEFTUP), x, y, 0, 0); }
                        if ((b &  8) != 0) { mouse_event((int)(MouseEventFlags.MOUSEEVENTF_RIGHTDOWN), x, y, 0, 0); }
                        if ((b & 16) != 0) { mouse_event((int)(MouseEventFlags.MOUSEEVENTF_RIGHTUP), x, y, 0, 0); }
                        if ((b & 32) != 0) { mouse_event((int)(MouseEventFlags.MOUSEEVENTF_MIDDLEDOWN), x, y, 0, 0); }
                        if ((b & 64) != 0) { mouse_event((int)(MouseEventFlags.MOUSEEVENTF_MIDDLEUP), x, y, 0, 0); }
                        if (w != 0) { mouse_event((int)(MouseEventFlags.MOUSEEVENTF_WHEEL), x, y, (uint)w, 0); }
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
                        if (cmdlen >= 10) {
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
                        if ((parent.userRights != (long)MeshCentralTunnel.MeshRights.ADMIN) && ((parent.userRights & (long)MeshCentralTunnel.MeshRights.REMOTEVIEWONLY) != 0)) break;
                        if ((parent.userRights != (long)MeshCentralTunnel.MeshRights.ADMIN) && ((parent.userRights & (long)MeshCentralTunnel.MeshRights.DESKLIMITEDINPUT) != 0)) break;
                        // This is not supported since we are not running as a admin service
                        break;
                    }
                case 11: // Query displays
                    {
                        SendDisplays();
                        break;
                    }
                case 12: // Set display
                    {
                        if (cmdlen < 6) break;
                        int selectedDisplay = ((data[off + 4] << 8) + data[off + 5]);
                        if (selectedDisplay == 65535) {
                            currentDisplay = -1;
                        } else {
                            if (selectedDisplay < 1) break;
                            if (selectedDisplay > Screen.AllScreens.Length) break;
                            currentDisplay = (selectedDisplay - 1);
                        }                        
                        break;
                    }
                case 15: // Touch
                    {
                        // Check user rights. If view only, ignore this command
                        if ((parent.userRights != (long)MeshCentralTunnel.MeshRights.ADMIN) && ((parent.userRights & (long)MeshCentralTunnel.MeshRights.REMOTEVIEWONLY) != 0)) break;

                        break;
                    }
                case 85: // Unicode Key
                    {
                        if (cmdlen < 7) break;

                        // Check user rights. If view only, ignore this command
                        if ((parent.userRights != (long)MeshCentralTunnel.MeshRights.ADMIN) && ((parent.userRights & (long)MeshCentralTunnel.MeshRights.REMOTEVIEWONLY) != 0)) break;
                        bool limitedinput = false;
                        if ((parent.userRights != (long)MeshCentralTunnel.MeshRights.ADMIN) && ((parent.userRights & (long)MeshCentralTunnel.MeshRights.DESKLIMITEDINPUT) != 0)) { limitedinput = true; }

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

        private void SendDisplays()
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
            parent.WebSocket.SendBinary(buf, 0, buf.Length);
        }

        private void MainDesktopLoop()
        {
            while (true)
            {
                if (mainThread == null) return;
                Thread.Sleep(encoderFrameRate);
                if (mainThread == null) return;
                if (parent.WebSocket.PendingSendLength > 1024) continue; // If there is data pending in the outbound buffer, skip this round.

                try
                {
                    // Take a look at the mouse cursor
                    // var mouseCursors = ['default', 'progress', 'crosshair', 'pointer', 'help', 'text', 'no-drop', 'move', 'nesw-resize', 'ns-resize', 'nwse-resize', 'w-resize', 'alias', 'wait', 'none', 'not-allowed', 'col-resize', 'row-resize', 'copy', 'zoom-in', 'zoom-out'];
                    CURSORINFO pci;
                    pci.cbSize = Marshal.SizeOf(typeof(CURSORINFO));
                    GetCursorInfo(out pci);
                    int pointerType = 0;
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
                    if (mousePointer != pointerType) // Update the mouse pointer
                    {
                        byte[] mousePointerCmd = new byte[5];
                        mousePointerCmd[1] = 88;
                        mousePointerCmd[3] = 5;
                        mousePointerCmd[4] = (byte)pointerType;
                        parent.WebSocket.SendBinary(mousePointerCmd, 0, 5);
                        mousePointer = pointerType;
                    }

                    // Get the size and location of the currently selected screen
                    Size tscreensize = Size.Empty;
                    Point tscreenlocation = Point.Empty;
                    Screen[] screens = Screen.AllScreens;
                    if (currentDisplay == -1) // We are looking at all screens
                    {
                        tscreensize = Size.Empty;
                        int maxx = 0;
                        int maxy = 0;
                        foreach (Screen s in screens)
                        {
                            int xx = s.Bounds.Left + s.Bounds.Width;
                            int yy = s.Bounds.Top + s.Bounds.Height;
                            if (maxx < xx) { maxx = xx; }
                            if (maxy < yy) { maxy = yy; }
                        }
                        tscreensize = new Size(maxx, maxy);
                        tscreenlocation = Point.Empty;
                    } else if ((currentDisplay >= 0) && (currentDisplay < screens.Length)) { // We are looking at a specific screen
                        tscreensize = Screen.AllScreens[currentDisplay].Bounds.Size;
                        tscreenlocation = Screen.AllScreens[currentDisplay].Bounds.Location;
                    }

                    // If the size of the screen does not match the current client set size, update the client
                    if ((ScreenSize.Width != tscreensize.Width) || (ScreenSize.Height != tscreensize.Height) || (captureBitmap == null) || (encoderScalingChanged == true)) 
                    {
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
                        parent.WebSocket.SendBinary(screenSizeCmd);

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
                        oldcrcs = new long[tilesCount]; // 64 x 64 tiles
                        newcrcs = new long[tilesCount]; // 64 x 64 tiles
                    }

                    // Capture the screen & scale it if needed
                    Graphics captureGraphics = Graphics.FromImage(captureBitmap);
                    captureGraphics.CopyFromScreen(tscreenlocation.X, tscreenlocation.Y, 0, 0, ScreenSize);
                    Bitmap scaledCaptureBitmap = captureBitmap;
                    if (encoderScaling != 1024) { scaledCaptureBitmap = new Bitmap(captureBitmap, screenScaleWidth, screenScaleHeight); }

                    // Compute all tile CRC's
                    computeAllCRCs(scaledCaptureBitmap);

                    //  Send all changed tiles
                    int sendCount = 0;
                    for (int i = 0; i < tilesHigh; i++)
                    {
                        for (int j = 0; j < tilesWide; j++)
                        {
                            int tileNumber = (i * tilesWide) + j;
                            if (oldcrcs[tileNumber] != newcrcs[tileNumber]) {
                                SendSubBitmap(scaledCaptureBitmap, (j * 64), (i * 64), 64, 64);
                                oldcrcs[tileNumber] = newcrcs[tileNumber];
                                sendCount++;
                            }
                        }
                    }
                    Console.WriteLine(sendCount);
                }
                catch (Exception ex) {
                    int i = 5;
                }
            }
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
            int tileWidthRemainReads = (tilesRemainingWidth * 3) / 8;
            if (image.PixelFormat == PixelFormat.Format32bppArgb) { tileWidthReads = 32; tileWidthRemainReads = (tilesRemainingWidth * 4) / 8; }

            // Handle all of the full tiles for the height
            for (int i = 0; i < tilesFullHigh; i++) {
                for (int j = 0; j < 64; j++) {
                    for (int k = 0; k < tilesFullWide; k++) {
                        for (int l = 0; l < tileWidthReads; l++) {
                            newcrcs[(i * tilesWide) + k] = CRC(Marshal.ReadInt64(bitmapPtr, ptr), newcrcs[(i * tilesWide) + k]);
                            ptr += 8;
                        }
                    }
                    if (tilesRemainingWidth > 0)
                    {
                        // Handle the reminder of the width
                        for (int l = 0; l < tileWidthRemainReads; l++)
                        {
                            newcrcs[(i * tilesWide) + tilesFullWide] = CRC(Marshal.ReadInt64(bitmapPtr, ptr), newcrcs[(i * tilesWide) + tilesFullWide]);
                            ptr += 8;
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
                        for (int l = 0; l < tileWidthRemainReads; l++)
                        {
                            newcrcs[(tilesFullHigh * tilesWide) + tilesFullWide] = CRC(Marshal.ReadInt64(bitmapPtr, ptr), newcrcs[(tilesFullHigh * tilesWide) + tilesFullWide]);
                            ptr += 8;
                        }
                    }
                }
            }

            // Unlock the bits.
            image.UnlockBits(bmpData);
        }

        private void SendSubBitmap(Bitmap image, int x, int y, int w, int h)
        {
            //SendBitmap(x, y, image.Clone(new Rectangle(x, y, w, h), image.PixelFormat));
            SendBitmap(x, y, image.Clone(Rectangle.Intersect(screenRect, new Rectangle(x, y, w, h)), image.PixelFormat));
        }

        private void SendBitmap(int x, int y, Bitmap image)
        {
            memoryBuffer.SetLength(0);
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
                parent.WebSocket.SendBinary(imageCmd, 0, cmdlen + 8);
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
                parent.WebSocket.SendBinary(imageCmd, 8, cmdlen);
            }
        }

        private ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs) { if (codec.FormatID == format.Guid) { return codec; } }
            return null;
        }

        private long CRC(long a, long b)
        {
            // TODO
            return a + b;
        }

    }


    public class CRC32
    {
        private readonly uint[] ChecksumTable;
        private readonly uint Polynomial = 0xEDB88320;

        public CRC32()
        {
            ChecksumTable = new uint[0x100];
            for (uint index = 0; index < 0x100; ++index)
            {
                uint item = index;
                for (int bit = 0; bit < 8; ++bit) { item = ((item & 1) != 0) ? (Polynomial ^ (item >> 1)) : (item >> 1); }
                ChecksumTable[index] = item;
            }
        }

        public byte[] ComputeHash(byte[] data)
        {
            uint result = 0xFFFFFFFF;
            for (var i = 0; i < data.Length; i++) { result = ChecksumTable[(result & 0xFF) ^ (byte)data[i]] ^ (result >> 8); }
            byte[] hash = BitConverter.GetBytes(~result);
            Array.Reverse(hash);
            return hash;
        }

        public byte[] ComputeHash(Stream stream)
        {
            uint result = 0xFFFFFFFF;
            int current;
            while ((current = stream.ReadByte()) != -1) { result = ChecksumTable[(result & 0xFF) ^ (byte)current] ^ (result >> 8); }
            byte[] hash = BitConverter.GetBytes(~result);
            Array.Reverse(hash);
            return hash;
        }

    }
}

