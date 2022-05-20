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

namespace MeshAssistant
{
    public partial class RequestHelpForm : Form
    {
        public MainForm parent;
        public bool NoHelpRequestOk = false;

        public RequestHelpForm(MainForm parent)
        {
            this.parent = parent;
            InitializeComponent();
            Translate.TranslateControl(this);
        }

        public string helpRequestText { get { return mainTextBox.Text; } }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            parent.RequestHelp(mainTextBox.Text);
            Close();
        }

        private void mainTextBox_TextChanged(object sender, EventArgs e)
        {
            okButton.Enabled = ((mainTextBox.Text.Length > 0) || NoHelpRequestOk);
        }

        private void RequestHelpForm_Load(object sender, EventArgs e)
        {
            mainTextBox.Focus();
            mainTextBox_TextChanged(this, null);
        }

        private void RequestHelpForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            parent.requestHelpForm = null;
        }
    }
}
