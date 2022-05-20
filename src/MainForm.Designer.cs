namespace MeshAssistant
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.stateLabel = new System.Windows.Forms.Label();
            this.mainNotifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.mainContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openSiteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showSessionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showEventsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.intelMEStateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.updateSoftwareToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.agentSelectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.startAgentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopAgentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.connectionTimer = new System.Windows.Forms.Timer(this.components);
            this.requestHelpButton = new System.Windows.Forms.Button();
            this.remoteSessionsLabel = new System.Windows.Forms.Label();
            this.dialogContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.requestHelpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cancelHelpRequestToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.guestSharingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.showSessionsToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.showEventsToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.intelAMTStateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.updateSoftwareToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.mainPictureBox = new System.Windows.Forms.PictureBox();
            this.pictureBoxCustom = new System.Windows.Forms.PictureBox();
            this.mainContextMenuStrip.SuspendLayout();
            this.dialogContextMenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mainPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCustom)).BeginInit();
            this.SuspendLayout();
            // 
            // stateLabel
            // 
            resources.ApplyResources(this.stateLabel, "stateLabel");
            this.stateLabel.ForeColor = System.Drawing.Color.White;
            this.stateLabel.Name = "stateLabel";
            // 
            // mainNotifyIcon
            // 
            this.mainNotifyIcon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Warning;
            resources.ApplyResources(this.mainNotifyIcon, "mainNotifyIcon");
            this.mainNotifyIcon.ContextMenuStrip = this.mainContextMenuStrip;
            this.mainNotifyIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(this.mainNotifyIcon_MouseClick);
            // 
            // mainContextMenuStrip
            // 
            this.mainContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.closeToolStripMenuItem,
            this.openSiteToolStripMenuItem,
            this.showSessionsToolStripMenuItem,
            this.showEventsToolStripMenuItem,
            this.intelMEStateToolStripMenuItem,
            this.updateSoftwareToolStripMenuItem,
            this.agentSelectToolStripMenuItem,
            this.toolStripMenuItem1,
            this.startAgentToolStripMenuItem,
            this.stopAgentToolStripMenuItem,
            this.toolStripMenuItem2,
            this.exitToolStripMenuItem});
            this.mainContextMenuStrip.Name = "mainContextMenuStrip";
            resources.ApplyResources(this.mainContextMenuStrip, "mainContextMenuStrip");
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            resources.ApplyResources(this.openToolStripMenuItem, "openToolStripMenuItem");
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            resources.ApplyResources(this.closeToolStripMenuItem, "closeToolStripMenuItem");
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // openSiteToolStripMenuItem
            // 
            this.openSiteToolStripMenuItem.Name = "openSiteToolStripMenuItem";
            resources.ApplyResources(this.openSiteToolStripMenuItem, "openSiteToolStripMenuItem");
            this.openSiteToolStripMenuItem.Click += new System.EventHandler(this.openSiteToolStripMenuItem_Click);
            // 
            // showSessionsToolStripMenuItem
            // 
            this.showSessionsToolStripMenuItem.Name = "showSessionsToolStripMenuItem";
            resources.ApplyResources(this.showSessionsToolStripMenuItem, "showSessionsToolStripMenuItem");
            this.showSessionsToolStripMenuItem.Click += new System.EventHandler(this.remoteSessionsLabel_Click);
            // 
            // showEventsToolStripMenuItem
            // 
            this.showEventsToolStripMenuItem.Name = "showEventsToolStripMenuItem";
            resources.ApplyResources(this.showEventsToolStripMenuItem, "showEventsToolStripMenuItem");
            this.showEventsToolStripMenuItem.Click += new System.EventHandler(this.showEventsToolStripMenuItem_Click);
            // 
            // intelMEStateToolStripMenuItem
            // 
            this.intelMEStateToolStripMenuItem.Name = "intelMEStateToolStripMenuItem";
            resources.ApplyResources(this.intelMEStateToolStripMenuItem, "intelMEStateToolStripMenuItem");
            this.intelMEStateToolStripMenuItem.Click += new System.EventHandler(this.intelAMTStateToolStripMenuItem_Click);
            // 
            // updateSoftwareToolStripMenuItem
            // 
            this.updateSoftwareToolStripMenuItem.Name = "updateSoftwareToolStripMenuItem";
            resources.ApplyResources(this.updateSoftwareToolStripMenuItem, "updateSoftwareToolStripMenuItem");
            this.updateSoftwareToolStripMenuItem.Click += new System.EventHandler(this.updateSoftwareToolStripMenuItem_Click);
            // 
            // agentSelectToolStripMenuItem
            // 
            this.agentSelectToolStripMenuItem.Name = "agentSelectToolStripMenuItem";
            resources.ApplyResources(this.agentSelectToolStripMenuItem, "agentSelectToolStripMenuItem");
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            resources.ApplyResources(this.toolStripMenuItem1, "toolStripMenuItem1");
            // 
            // startAgentToolStripMenuItem
            // 
            this.startAgentToolStripMenuItem.Name = "startAgentToolStripMenuItem";
            resources.ApplyResources(this.startAgentToolStripMenuItem, "startAgentToolStripMenuItem");
            this.startAgentToolStripMenuItem.Click += new System.EventHandler(this.startAgentToolStripMenuItem_Click);
            // 
            // stopAgentToolStripMenuItem
            // 
            this.stopAgentToolStripMenuItem.Name = "stopAgentToolStripMenuItem";
            resources.ApplyResources(this.stopAgentToolStripMenuItem, "stopAgentToolStripMenuItem");
            this.stopAgentToolStripMenuItem.Click += new System.EventHandler(this.stopAgentToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            resources.ApplyResources(this.toolStripMenuItem2, "toolStripMenuItem2");
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            resources.ApplyResources(this.exitToolStripMenuItem, "exitToolStripMenuItem");
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // connectionTimer
            // 
            this.connectionTimer.Interval = 1000;
            this.connectionTimer.Tick += new System.EventHandler(this.connectionTimer_Tick);
            // 
            // requestHelpButton
            // 
            resources.ApplyResources(this.requestHelpButton, "requestHelpButton");
            this.requestHelpButton.Name = "requestHelpButton";
            this.requestHelpButton.UseVisualStyleBackColor = true;
            this.requestHelpButton.Click += new System.EventHandler(this.requestHelpButton_Click);
            this.requestHelpButton.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            // 
            // remoteSessionsLabel
            // 
            resources.ApplyResources(this.remoteSessionsLabel, "remoteSessionsLabel");
            this.remoteSessionsLabel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.remoteSessionsLabel.ForeColor = System.Drawing.Color.White;
            this.remoteSessionsLabel.Name = "remoteSessionsLabel";
            this.remoteSessionsLabel.Click += new System.EventHandler(this.remoteSessionsLabel_Click);
            // 
            // dialogContextMenuStrip
            // 
            this.dialogContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.requestHelpToolStripMenuItem,
            this.cancelHelpRequestToolStripMenuItem,
            this.guestSharingToolStripMenuItem,
            this.toolStripMenuItem3,
            this.showSessionsToolStripMenuItem1,
            this.showEventsToolStripMenuItem1,
            this.intelAMTStateToolStripMenuItem,
            this.updateSoftwareToolStripMenuItem1});
            this.dialogContextMenuStrip.Name = "dialogContextMenuStrip";
            resources.ApplyResources(this.dialogContextMenuStrip, "dialogContextMenuStrip");
            this.dialogContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.dialogContextMenuStrip_Opening);
            // 
            // requestHelpToolStripMenuItem
            // 
            this.requestHelpToolStripMenuItem.Name = "requestHelpToolStripMenuItem";
            resources.ApplyResources(this.requestHelpToolStripMenuItem, "requestHelpToolStripMenuItem");
            this.requestHelpToolStripMenuItem.Click += new System.EventHandler(this.requestHelpButton_Click);
            // 
            // cancelHelpRequestToolStripMenuItem
            // 
            this.cancelHelpRequestToolStripMenuItem.Name = "cancelHelpRequestToolStripMenuItem";
            resources.ApplyResources(this.cancelHelpRequestToolStripMenuItem, "cancelHelpRequestToolStripMenuItem");
            this.cancelHelpRequestToolStripMenuItem.Click += new System.EventHandler(this.requestHelpButton_Click);
            // 
            // guestSharingToolStripMenuItem
            // 
            this.guestSharingToolStripMenuItem.Name = "guestSharingToolStripMenuItem";
            resources.ApplyResources(this.guestSharingToolStripMenuItem, "guestSharingToolStripMenuItem");
            this.guestSharingToolStripMenuItem.Click += new System.EventHandler(this.guestSharingToolStripMenuItem_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            resources.ApplyResources(this.toolStripMenuItem3, "toolStripMenuItem3");
            // 
            // showSessionsToolStripMenuItem1
            // 
            this.showSessionsToolStripMenuItem1.Name = "showSessionsToolStripMenuItem1";
            resources.ApplyResources(this.showSessionsToolStripMenuItem1, "showSessionsToolStripMenuItem1");
            this.showSessionsToolStripMenuItem1.Click += new System.EventHandler(this.remoteSessionsLabel_Click);
            // 
            // showEventsToolStripMenuItem1
            // 
            this.showEventsToolStripMenuItem1.Name = "showEventsToolStripMenuItem1";
            resources.ApplyResources(this.showEventsToolStripMenuItem1, "showEventsToolStripMenuItem1");
            this.showEventsToolStripMenuItem1.Click += new System.EventHandler(this.showEventsToolStripMenuItem_Click);
            // 
            // intelAMTStateToolStripMenuItem
            // 
            this.intelAMTStateToolStripMenuItem.Name = "intelAMTStateToolStripMenuItem";
            resources.ApplyResources(this.intelAMTStateToolStripMenuItem, "intelAMTStateToolStripMenuItem");
            this.intelAMTStateToolStripMenuItem.Click += new System.EventHandler(this.intelAMTStateToolStripMenuItem_Click);
            // 
            // updateSoftwareToolStripMenuItem1
            // 
            this.updateSoftwareToolStripMenuItem1.Name = "updateSoftwareToolStripMenuItem1";
            resources.ApplyResources(this.updateSoftwareToolStripMenuItem1, "updateSoftwareToolStripMenuItem1");
            this.updateSoftwareToolStripMenuItem1.Click += new System.EventHandler(this.updateSoftwareToolStripMenuItem_Click);
            // 
            // mainPictureBox
            // 
            resources.ApplyResources(this.mainPictureBox, "mainPictureBox");
            this.mainPictureBox.ContextMenuStrip = this.dialogContextMenuStrip;
            this.mainPictureBox.Name = "mainPictureBox";
            this.mainPictureBox.TabStop = false;
            // 
            // pictureBoxCustom
            // 
            resources.ApplyResources(this.pictureBoxCustom, "pictureBoxCustom");
            this.pictureBoxCustom.Name = "pictureBoxCustom";
            this.pictureBoxCustom.TabStop = false;
            // 
            // MainForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(79)))), ((int)(((byte)(130)))));
            this.Controls.Add(this.remoteSessionsLabel);
            this.Controls.Add(this.requestHelpButton);
            this.Controls.Add(this.stateLabel);
            this.Controls.Add(this.mainPictureBox);
            this.Controls.Add(this.pictureBoxCustom);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            this.mainContextMenuStrip.ResumeLayout(false);
            this.dialogContextMenuStrip.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mainPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCustom)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label stateLabel;
        private System.Windows.Forms.NotifyIcon mainNotifyIcon;
        private System.Windows.Forms.ContextMenuStrip mainContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.Timer connectionTimer;
        private System.Windows.Forms.ToolStripMenuItem openSiteToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem startAgentToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stopAgentToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.Button requestHelpButton;
        private System.Windows.Forms.PictureBox mainPictureBox;
        private System.Windows.Forms.Label remoteSessionsLabel;
        private System.Windows.Forms.ToolStripMenuItem showSessionsToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip dialogContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem requestHelpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cancelHelpRequestToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showSessionsToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem intelAMTStateToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem intelMEStateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem agentSelectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem updateSoftwareToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showEventsToolStripMenuItem;
        private System.Windows.Forms.PictureBox pictureBoxCustom;
        private System.Windows.Forms.ToolStripMenuItem showEventsToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem updateSoftwareToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem guestSharingToolStripMenuItem;
    }
}

