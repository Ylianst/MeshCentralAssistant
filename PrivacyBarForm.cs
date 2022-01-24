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
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace MeshAssistant
{
    public partial class PrivacyBarForm : Form
    {
        private MainForm parent;
        private const int CS_DROPSHADOW = 0x20000;
        private const int WM_WINDOWPOSCHANGING = 0x46;
        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HT_CAPTION = 0x2;
        private Screen displayedScreen;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        public string privacyText {
            set { mainLabel.Text = value; }
        }

        public PrivacyBarForm(MainForm parent, Screen screen)
        {
            this.parent = parent;
            displayedScreen = screen;
            InitializeComponent();
            Translate.TranslateControl(this);
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ClassStyle |= CS_DROPSHADOW;
                return cp;
            }
        }

        private void PrivacyBarForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void PrivacyBarForm_Load(object sender, EventArgs e)
        {
            this.Width = 600;
            this.Height = 28;
            this.Left = displayedScreen.WorkingArea.Left + ((displayedScreen.WorkingArea.Width / 2) - (this.Width / 2));
            this.Top = displayedScreen.WorkingArea.Top;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct WINDOWPOS
        {
            public IntPtr hwnd;
            public IntPtr hwndInsertAfter;
            public int x;
            public int y;
            public int cx;
            public int cy;
            public uint flags;
        }

        protected override void WndProc(ref Message message)
        {
            switch (message.Msg)
            {
                case WM_WINDOWPOSCHANGING:
                    if (displayedScreen != null)
                    {
                        WINDOWPOS wp = (WINDOWPOS)Marshal.PtrToStructure(message.LParam, typeof(WINDOWPOS));
                        if (wp.x < displayedScreen.WorkingArea.Left) { wp.x = displayedScreen.WorkingArea.Left; }
                        if ((wp.x + this.Width) > displayedScreen.WorkingArea.Right) { wp.x = (displayedScreen.WorkingArea.Right - this.Width); }
                        wp.y = displayedScreen.WorkingArea.Top;
                        Marshal.StructureToPtr(wp, message.LParam, false);
                    }
                    break;
            }

            base.WndProc(ref message);
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            parent.PrivacyBarClose();
        }

    }
}
