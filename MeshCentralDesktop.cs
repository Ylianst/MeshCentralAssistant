using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

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

        public MeshCentralDesktop(MeshCentralTunnel parent)
        {
            this.parent = parent;

            // Setup the JPEG encoder
            jgpEncoder = GetEncoder(ImageFormat.Jpeg);
            myEncoderParameters = new EncoderParameters(1);
            EncoderParameter myEncoderParameter = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 0L);
            myEncoderParameters.Param[0] = myEncoderParameter;

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

            switch(cmd)
            {
                case 1: // Key
                    {
                        break;
                    }
                case 2: // Mouse
                    {
                        break;
                    }
                case 10: // Ctrl-Alt-Del
                    {
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

        private void MainDesktopLoop()
        {
            while (true)
            {
                if (mainThread == null) return;
                Thread.Sleep(1000);
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
                            int x = s.Bounds.Left + s.Bounds.Width;
                            int y = s.Bounds.Top + s.Bounds.Height;
                            if (maxx < x) { maxx = x; }
                            if (maxy < y) { maxy = y; }
                        }
                        tscreensize = new Size(maxx, maxy);
                        tscreenlocation = Point.Empty;
                    } else if ((currentDisplay >= 0) && (currentDisplay < screens.Length)) { // We are looking at a specific screen
                        tscreensize = Screen.AllScreens[currentDisplay].Bounds.Size;
                        tscreenlocation = Screen.AllScreens[currentDisplay].Bounds.Location;
                    }

                    //tscreensize = new Size(256, 256);// TEST

                    // If the size of the screen does not match the current client set size, update the client
                    if ((ScreenSize.Width != tscreensize.Width) || (ScreenSize.Height != tscreensize.Height))
                    {
                        ScreenSize = tscreensize;
                        byte[] screenSizeCmd = new byte[8];
                        screenSizeCmd[1] = 7; // Command 7, screen size
                        screenSizeCmd[3] = 8; // Command size, 8 bytes
                        screenSizeCmd[4] = (byte)(ScreenSize.Width >> 8);
                        screenSizeCmd[5] = (byte)(ScreenSize.Width & 0xFF);
                        screenSizeCmd[6] = (byte)(ScreenSize.Height >> 8);
                        screenSizeCmd[7] = (byte)(ScreenSize.Height & 0xFF);
                        parent.WebSocket.SendBinary(screenSizeCmd);

                        // TODO: Clear CRC's.
                    }

                    // Capture the screen
                    Bitmap captureBitmap = new Bitmap(ScreenSize.Width, ScreenSize.Height, PixelFormat.Format32bppArgb);
                    Graphics captureGraphics = Graphics.FromImage(captureBitmap);
                    captureGraphics.CopyFromScreen(tscreenlocation.X, tscreenlocation.Y, 0, 0, ScreenSize);
                    
                    memoryBuffer.SetLength(0);
                    memoryBuffer.Write(skipHeader, 0, 16); // Skip the first 16 bytes
                    captureBitmap.Save(memoryBuffer, jgpEncoder, myEncoderParameters); // Write the JPEG image
                    byte[] imageCmd = memoryBuffer.GetBuffer();
                    int cmdlen = (int)(memoryBuffer.Length - 8);

                    // Jumbo command
                    if (memoryBuffer.Length > 65000) {
                        imageCmd[1] = 27; // Command 27, JUMBO
                        imageCmd[3] = 8; // Command size, 8 bytes
                        imageCmd[4] = (byte)((cmdlen >> 24) & 0xFF);
                        imageCmd[5] = (byte)((cmdlen >> 16) & 0xFF);
                        imageCmd[6] = (byte)((cmdlen >> 8) & 0xFF);
                        imageCmd[7] = (byte)((cmdlen) & 0xFF);

                        // Tile command
                        imageCmd[9] = 3; // Command 3, tile
                        imageCmd[12] = (byte)(0 >> 8);   // X
                        imageCmd[13] = (byte)(0 & 0xFF); // X
                        imageCmd[14] = (byte)(0 >> 8);   // Y
                        imageCmd[15] = (byte)(0 & 0xFF); // Y

                        // Send with JUMBO command
                        parent.WebSocket.SendBinary(imageCmd, 0, cmdlen + 8);
                    } else {
                        // Tile command
                        imageCmd[9] = 3; // Command 3, tile
                        imageCmd[10] = (byte)(cmdlen >> 8);    // Command size, 8 bytes + image
                        imageCmd[11] = (byte)(cmdlen & 0xFF); // Command size, 8 bytes + image
                        imageCmd[12] = (byte)(0 >> 8);   // X
                        imageCmd[13] = (byte)(0 & 0xFF); // X
                        imageCmd[14] = (byte)(0 >> 8);   // Y
                        imageCmd[15] = (byte)(0 & 0xFF); // Y

                        // Send normal command
                        parent.WebSocket.SendBinary(imageCmd, 8, cmdlen);
                    }
                }
                catch (Exception ex) { }
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
