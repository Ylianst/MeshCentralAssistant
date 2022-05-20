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
using System.Collections.Generic;

namespace MeshAssistant
{
    public partial class MeInfoForm : Form
    {
        private MainForm parent;

        public MeInfoForm(MainForm parent)
        {
            this.parent = parent;
            InitializeComponent();
            Translate.TranslateControl(this);
            Translate.TranslateListView(stateListView);
            stateListView.Items.Add(new ListViewItem(Translate.T(Properties.Resources.Loading)));
            versionsListView.Items.Add(new ListViewItem(Translate.T(Properties.Resources.Loading)));
            parent.agent.RequestIntelAmtState();
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void MeInfoForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            parent.meInfoForm = null;
        }

        public void updateInfo(Dictionary<string, object> state)
        {
            stateListView.Items.Clear();
            versionsListView.Items.Clear();
            if (state.ContainsKey("Versions"))
            {
                Dictionary<string, object> versions = (Dictionary<string, object>)state["Versions"];
                foreach (string k in versions.Keys)
                {
                    versionsListView.Items.Add(new ListViewItem(new string[] { k, versions[k].ToString() }));
                }
            }

            int flags = -1;
            int ProvisioningMode = -1;
            int ProvisioningState = -1;
            if (state.ContainsKey("Flags")) { flags = (int)state["Flags"]; }
            if (state.ContainsKey("ProvisioningMode")) { ProvisioningMode = (int)state["ProvisioningMode"]; }
            if (state.ContainsKey("ProvisioningState")) { ProvisioningState = (int)state["ProvisioningState"]; }
            string stateStr = Translate.T(Properties.Resources.Unknown);
            if (ProvisioningState == 0) { stateStr = Translate.T(Properties.Resources.NotActivatedPre); }
            else if (ProvisioningState == 1) { stateStr = Translate.T(Properties.Resources.NotActivatedIn); }
            else if (ProvisioningState == 2) {
                stateStr = Translate.T(Properties.Resources.Activated);
                if (flags >= 0) {
                    if ((flags & 2) != 0) { stateStr += ", CCM"; }
                    if ((flags & 4) != 0) { stateStr += ", ACM"; }
                }
            }
            stateListView.Items.Add(new ListViewItem(new string[] { Translate.T(Properties.Resources.State), stateStr }));

            if (state.ContainsKey("UUID")) { stateListView.Items.Add(new ListViewItem(new string[] { "UUID", state["UUID"].ToString() })); }

            if (state.ContainsKey("net0"))
            {
                Dictionary<string, object> net = (Dictionary<string, object>)state["net0"];
                if (net.ContainsKey("enabled"))
                {
                    int enabled = (int)net["enabled"];
                    stateListView.Items.Add(new ListViewItem(new string[] { "net0", ((enabled != 0)? Translate.T(Properties.Resources.Enabled): Translate.T(Properties.Resources.Disabled)) }));
                }
                if (net.ContainsKey("dhcpEnabled"))
                {
                    int dhcpEnabled = (int)net["dhcpEnabled"];
                    string x = ((dhcpEnabled != 0) ? Translate.T(Properties.Resources.Enabled) : Translate.T(Properties.Resources.Disabled));
                    if (net.ContainsKey("dhcpMode")) { x += ", " + (string)net["dhcpMode"]; }
                    stateListView.Items.Add(new ListViewItem(new string[] { "  DHCP", x }));
                }
                if (net.ContainsKey("mac"))
                {
                    string mac = (string)net["mac"];
                    stateListView.Items.Add(new ListViewItem(new string[] { "  MAC", mac }));
                }
                if (net.ContainsKey("address"))
                {
                    string address = (string)net["address"];
                    stateListView.Items.Add(new ListViewItem(new string[] { "  IP", address }));
                }
            }

        }
    }
}
