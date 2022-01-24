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
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace MeshAssistant
{
    public partial class ConsentForm : Form
    {
        private MainForm parent;
        private string orgtitle;
        private string message = "";
        public static Dictionary<string, DateTime> autoConsent = new Dictionary<string, DateTime>();

        public ConsentForm(MainForm parent)
        {
            this.parent = parent;
            InitializeComponent();
            Translate.TranslateControl(this);
            this.orgtitle = this.Text;
        }

        public string userid;
        public MeshCentralTunnel tunnel;

        public string Message { set { message = value; updateInfo(); } }
        public string UserName { set { nameLabel.Text = value; updateInfo(); } }
        public string Title { set { this.Text = string.Format(Translate.T(Properties.Resources.TitleMerge), orgtitle, value); } }
        public Image UserImage { set { if (value == null) { mainPictureBox.Image = mainPictureBox.InitialImage; } else { mainPictureBox.Image = value; } } }

        public bool AutoAccept { get { return autoConsentCheckBox.Checked; } }

        private void updateInfo()
        {
            mainLabel.Text = string.Format(message, nameLabel.Text);
        }

        private void closeButton_Click(object sender, System.EventArgs e)
        {
            tunnel.ConsentRejected();
            Close();
        }

        private void ConsentForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            parent.consentForm = null;
        }

        private void okButton_Click(object sender, System.EventArgs e)
        {
            if ((userid != null) && (autoConsentCheckBox.Checked == true)) { autoConsent.Add(userid, DateTime.Now.AddMinutes(5)); }
            tunnel.ConsentAccepted();
            Close();
        }

        private void ConsentForm_Load(object sender, System.EventArgs e)
        {
            autoConsentCheckBox.Visible = (userid != null);
        }
    }
}
