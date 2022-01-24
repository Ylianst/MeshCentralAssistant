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
    public partial class ConsoleForm : Form
    {
        public MainForm parent = null;
        public int pastCommandsIndex = -1;

        public ConsoleForm(MainForm parent)
        {
            this.parent = parent;
            InitializeComponent();
            Translate.TranslateControl(this);
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            mainTextBox.Clear();
        }

        private void sendButton_Click(object sender, EventArgs e)
        {
            if (parent.agent == null) return;
            if (parent.agent.SendConsoleCommand(sendTextBox.Text) == true)
            {
                parent.pastConsoleCommands.Insert(0, sendTextBox.Text);
                mainTextBox.AppendText("> " + sendTextBox.Text + "\r\n");
                sendTextBox.Clear();
                pastCommandsIndex = -1;
            }
        }

        private void ConsoleForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            parent.consoleForm = null;
        }

        private void sendTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13) { sendButton_Click(this, null); e.Handled = true; }
        }

        public void appendText(string str)
        {
            mainTextBox.AppendText(str.Replace("\r\n","\n").Replace("\n", "\r\n") + "\r\n");
        }

        private void sendTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up)
            {
                if (parent.pastConsoleCommands.Count > (pastCommandsIndex + 1))
                {
                    pastCommandsIndex++;
                    sendTextBox.Text = parent.pastConsoleCommands[pastCommandsIndex].ToString();
                    sendTextBox.SelectionStart = sendTextBox.Text.Length;
                    sendTextBox.SelectionLength = 0;
                    e.Handled = true;
                }
            }
            else if ((e.KeyCode == Keys.Down) && (pastCommandsIndex >= 0))
            {
                pastCommandsIndex--;
                if (pastCommandsIndex == -1) {
                    sendTextBox.Text = "";
                } else {
                    sendTextBox.Text = parent.pastConsoleCommands[pastCommandsIndex].ToString();
                    sendTextBox.SelectionStart = sendTextBox.Text.Length;
                    sendTextBox.SelectionLength = 0;
                }
            } else if (e.KeyCode == Keys.Escape) {
                sendTextBox.Text = "";
            }
            if ((e.KeyCode == Keys.Down) || (e.KeyCode == Keys.Up) || (e.KeyCode == Keys.Escape)) { e.Handled = true; }
        }
    }
}
