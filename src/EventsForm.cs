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
using System.Collections.Generic;
using System.Windows.Forms;

namespace MeshAssistant
{
    public partial class EventsForm : Form
    {
        private MainForm parent;

        public EventsForm(MainForm parent, List<MainForm.LogEventStruct> events)
        {
            this.parent = parent;
            InitializeComponent();
            Translate.TranslateControl(this);
            Translate.TranslateListView(mainListView);
            mainListView.BeginUpdate();
            foreach (MainForm.LogEventStruct e in events)
            {
                mainListView.Items.Insert(0, new ListViewItem(new string[] { e.time.ToLongTimeString(), formatUserId(e.userid), e.msg }));
            }
            mainListView.EndUpdate();
        }

        private void EventsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            parent.closingEventForm();
        }

        private string formatUserId(string userid)
        {
            string[] s = userid.Split('/');
            if (s.Length == 3) return s[2];
            return userid;
        }

        public void addEvent(MainForm.LogEventStruct e)
        {
            mainListView.Items.Insert(0, new ListViewItem(new string[] { e.time.ToLongTimeString(), formatUserId(e.userid), e.msg }));
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
