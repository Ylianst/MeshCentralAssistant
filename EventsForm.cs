using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
