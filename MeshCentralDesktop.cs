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
        private int encoderFrameRate = 100;

        [DllImport("user32.dll")]
        public static extern void mouse_event(uint dwFlags, int dx, int dy, uint dwData, int dwExtraInfo);

        [DllImport("user32")]
        public static extern int SetCursorPos(int x, int y);

        [DllImport("USER32.DLL")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        private static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

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
                        break;
                    }
                case 2: // Mouse
                    {
                        if (cmdlen < 10) break;
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
                            if (xencoderScaling != encoderScaling) { encoderScaling = xencoderScaling; ScreenSize = Size.Empty; }
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
                        break;
                    }
                case 85: // Unicode Key
                    {
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

                try
                {
                    // Take a look at the mouse cursor
                    //Cursor mouseCursor = Cursor.Current;

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
                    if ((ScreenSize.Width != tscreensize.Width) || (ScreenSize.Height != tscreensize.Height) || (captureBitmap == null))
                    {
                        ScreenSize = tscreensize;
                        byte[] screenSizeCmd = new byte[8];
                        screenSizeCmd[1] = 7; // Command 7, screen size
                        screenSizeCmd[3] = 8; // Command size, 8 bytes
                        screenSizeCmd[4] = (byte)(((encoderScaling * ScreenSize.Width) / 1024) >> 8);
                        screenSizeCmd[5] = (byte)(((encoderScaling * ScreenSize.Width) / 1024) & 0xFF);
                        screenSizeCmd[6] = (byte)(((encoderScaling * ScreenSize.Height) / 1024) >> 8);
                        screenSizeCmd[7] = (byte)(((encoderScaling * ScreenSize.Height) / 1024) & 0xFF);
                        parent.WebSocket.SendBinary(screenSizeCmd);

                        // TODO: Clear CRC's.
                        captureBitmap = new Bitmap(ScreenSize.Width, ScreenSize.Height, PixelFormat.Format24bppRgb);
                    }

                    // Capture the screen
                    Graphics captureGraphics = Graphics.FromImage(captureBitmap);
                    captureGraphics.CopyFromScreen(tscreenlocation.X, tscreenlocation.Y, 0, 0, ScreenSize);

                    memoryBuffer.SetLength(0);
                    memoryBuffer.Write(skipHeader, 0, 16); // Skip the first 16 bytes
                    if (encoderScaling == 1024) {
                        captureBitmap.Save(memoryBuffer, jgpEncoder, myEncoderParameters); // Write the JPEG image at 100% scale
                    } else {
                        Bitmap resized = new Bitmap(captureBitmap, (encoderScaling * ScreenSize.Width) / 1024, (encoderScaling * ScreenSize.Height) / 1024);
                        resized.Save(memoryBuffer, jgpEncoder, myEncoderParameters); // Write the scaled JPEG image
                    }
                    byte[] imageCmd = memoryBuffer.GetBuffer();
                    int cmdlen = (int)(memoryBuffer.Length - 8);
                    int x = 0;
                    int y = 0;

                    // Jumbo command
                    if (memoryBuffer.Length > 65000) {
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
                    } else {
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
                catch (Exception) { }
            }
        }

        private ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs) { if (codec.FormatID == format.Guid) { return codec; } }
            return null;
        }

    }
}
