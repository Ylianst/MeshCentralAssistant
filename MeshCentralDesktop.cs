using System;
using System.IO;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Drawing.Imaging;

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
