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
            this.intelMEStateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.showSessionsToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.intelAMTStateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pictureBoxUser = new System.Windows.Forms.PictureBox();
            this.pictureBoxRed = new System.Windows.Forms.PictureBox();
            this.pictureBoxQuestion = new System.Windows.Forms.PictureBox();
            this.pictureBoxGreen = new System.Windows.Forms.PictureBox();
            this.pictureBoxYellow = new System.Windows.Forms.PictureBox();
            this.pictureBoxCustom = new System.Windows.Forms.PictureBox();
            this.mainContextMenuStrip.SuspendLayout();
            this.dialogContextMenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxUser)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxRed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxQuestion)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxGreen)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxYellow)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCustom)).BeginInit();
            this.SuspendLayout();
            // 
            // stateLabel
            // 
            this.stateLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.stateLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.stateLabel.ForeColor = System.Drawing.Color.White;
            this.stateLabel.Location = new System.Drawing.Point(12, 11);
            this.stateLabel.Name = "stateLabel";
            this.stateLabel.Size = new System.Drawing.Size(174, 13);
            this.stateLabel.TabIndex = 4;
            this.stateLabel.Text = "Agent is missing";
            this.stateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // mainNotifyIcon
            // 
            this.mainNotifyIcon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Warning;
            this.mainNotifyIcon.BalloonTipText = "No remote sessions are active.";
            this.mainNotifyIcon.BalloonTipTitle = "MeshCentral Assistant";
            this.mainNotifyIcon.ContextMenuStrip = this.mainContextMenuStrip;
            this.mainNotifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("mainNotifyIcon.Icon")));
            this.mainNotifyIcon.Text = "MeshCentral Assistant";
            this.mainNotifyIcon.Visible = true;
            this.mainNotifyIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(this.mainNotifyIcon_MouseClick);
            // 
            // mainContextMenuStrip
            // 
            this.mainContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.closeToolStripMenuItem,
            this.openSiteToolStripMenuItem,
            this.showSessionsToolStripMenuItem,
            this.intelMEStateToolStripMenuItem,
            this.agentSelectToolStripMenuItem,
            this.toolStripMenuItem1,
            this.startAgentToolStripMenuItem,
            this.stopAgentToolStripMenuItem,
            this.toolStripMenuItem2,
            this.exitToolStripMenuItem});
            this.mainContextMenuStrip.Name = "mainContextMenuStrip";
            this.mainContextMenuStrip.Size = new System.Drawing.Size(167, 214);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.openToolStripMenuItem.Text = "&Open...";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.closeToolStripMenuItem.Text = "&Close";
            this.closeToolStripMenuItem.Visible = false;
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // openSiteToolStripMenuItem
            // 
            this.openSiteToolStripMenuItem.Name = "openSiteToolStripMenuItem";
            this.openSiteToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.openSiteToolStripMenuItem.Text = "O&pen Site...";
            this.openSiteToolStripMenuItem.Visible = false;
            this.openSiteToolStripMenuItem.Click += new System.EventHandler(this.openSiteToolStripMenuItem_Click);
            // 
            // showSessionsToolStripMenuItem
            // 
            this.showSessionsToolStripMenuItem.Name = "showSessionsToolStripMenuItem";
            this.showSessionsToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.showSessionsToolStripMenuItem.Text = "Show Sessions...";
            this.showSessionsToolStripMenuItem.Click += new System.EventHandler(this.remoteSessionsLabel_Click);
            // 
            // intelMEStateToolStripMenuItem
            // 
            this.intelMEStateToolStripMenuItem.Name = "intelMEStateToolStripMenuItem";
            this.intelMEStateToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.intelMEStateToolStripMenuItem.Text = "Intel® ME State...";
            this.intelMEStateToolStripMenuItem.Visible = false;
            this.intelMEStateToolStripMenuItem.Click += new System.EventHandler(this.intelAMTStateToolStripMenuItem_Click);
            // 
            // agentSelectToolStripMenuItem
            // 
            this.agentSelectToolStripMenuItem.Name = "agentSelectToolStripMenuItem";
            this.agentSelectToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.agentSelectToolStripMenuItem.Text = "Agent Select";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(163, 6);
            // 
            // startAgentToolStripMenuItem
            // 
            this.startAgentToolStripMenuItem.Name = "startAgentToolStripMenuItem";
            this.startAgentToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.startAgentToolStripMenuItem.Text = "&Start Agent";
            this.startAgentToolStripMenuItem.Click += new System.EventHandler(this.startAgentToolStripMenuItem_Click);
            // 
            // stopAgentToolStripMenuItem
            // 
            this.stopAgentToolStripMenuItem.Name = "stopAgentToolStripMenuItem";
            this.stopAgentToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.stopAgentToolStripMenuItem.Text = "S&top Agent";
            this.stopAgentToolStripMenuItem.Click += new System.EventHandler(this.stopAgentToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(163, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // connectionTimer
            // 
            this.connectionTimer.Enabled = true;
            this.connectionTimer.Interval = 1000;
            this.connectionTimer.Tick += new System.EventHandler(this.connectionTimer_Tick);
            // 
            // requestHelpButton
            // 
            this.requestHelpButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.requestHelpButton.Enabled = false;
            this.requestHelpButton.Location = new System.Drawing.Point(14, 182);
            this.requestHelpButton.Name = "requestHelpButton";
            this.requestHelpButton.Size = new System.Drawing.Size(174, 31);
            this.requestHelpButton.TabIndex = 6;
            this.requestHelpButton.Text = "Request Help";
            this.requestHelpButton.UseVisualStyleBackColor = true;
            this.requestHelpButton.Click += new System.EventHandler(this.requestHelpButton_Click);
            this.requestHelpButton.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            // 
            // remoteSessionsLabel
            // 
            this.remoteSessionsLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.remoteSessionsLabel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.remoteSessionsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.remoteSessionsLabel.ForeColor = System.Drawing.Color.White;
            this.remoteSessionsLabel.Location = new System.Drawing.Point(12, 162);
            this.remoteSessionsLabel.Name = "remoteSessionsLabel";
            this.remoteSessionsLabel.Size = new System.Drawing.Size(174, 13);
            this.remoteSessionsLabel.TabIndex = 11;
            this.remoteSessionsLabel.Text = "No remote sessions";
            this.remoteSessionsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.remoteSessionsLabel.Click += new System.EventHandler(this.remoteSessionsLabel_Click);
            // 
            // dialogContextMenuStrip
            // 
            this.dialogContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.requestHelpToolStripMenuItem,
            this.cancelHelpRequestToolStripMenuItem,
            this.toolStripMenuItem3,
            this.showSessionsToolStripMenuItem1,
            this.intelAMTStateToolStripMenuItem});
            this.dialogContextMenuStrip.Name = "dialogContextMenuStrip";
            this.dialogContextMenuStrip.Size = new System.Drawing.Size(184, 98);
            this.dialogContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.dialogContextMenuStrip_Opening);
            // 
            // requestHelpToolStripMenuItem
            // 
            this.requestHelpToolStripMenuItem.Name = "requestHelpToolStripMenuItem";
            this.requestHelpToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.requestHelpToolStripMenuItem.Text = "Request Help...";
            this.requestHelpToolStripMenuItem.Click += new System.EventHandler(this.requestHelpButton_Click);
            // 
            // cancelHelpRequestToolStripMenuItem
            // 
            this.cancelHelpRequestToolStripMenuItem.Name = "cancelHelpRequestToolStripMenuItem";
            this.cancelHelpRequestToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.cancelHelpRequestToolStripMenuItem.Text = "Cancel Help Request";
            this.cancelHelpRequestToolStripMenuItem.Click += new System.EventHandler(this.requestHelpButton_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(180, 6);
            // 
            // showSessionsToolStripMenuItem1
            // 
            this.showSessionsToolStripMenuItem1.Name = "showSessionsToolStripMenuItem1";
            this.showSessionsToolStripMenuItem1.Size = new System.Drawing.Size(183, 22);
            this.showSessionsToolStripMenuItem1.Text = "Remote Sessions...";
            this.showSessionsToolStripMenuItem1.Click += new System.EventHandler(this.remoteSessionsLabel_Click);
            // 
            // intelAMTStateToolStripMenuItem
            // 
            this.intelAMTStateToolStripMenuItem.Name = "intelAMTStateToolStripMenuItem";
            this.intelAMTStateToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.intelAMTStateToolStripMenuItem.Text = "Intel® ME State...";
            this.intelAMTStateToolStripMenuItem.Visible = false;
            this.intelAMTStateToolStripMenuItem.Click += new System.EventHandler(this.intelAMTStateToolStripMenuItem_Click);
            // 
            // pictureBoxUser
            // 
            this.pictureBoxUser.ContextMenuStrip = this.dialogContextMenuStrip;
            this.pictureBoxUser.Image = global::MeshAssistant.Properties.Resources.user;
            this.pictureBoxUser.Location = new System.Drawing.Point(12, 23);
            this.pictureBoxUser.Name = "pictureBoxUser";
            this.pictureBoxUser.Size = new System.Drawing.Size(178, 136);
            this.pictureBoxUser.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBoxUser.TabIndex = 12;
            this.pictureBoxUser.TabStop = false;
            // 
            // pictureBoxRed
            // 
            this.pictureBoxRed.ContextMenuStrip = this.dialogContextMenuStrip;
            this.pictureBoxRed.Image = global::MeshAssistant.Properties.Resources.MeshIconRed2;
            this.pictureBoxRed.Location = new System.Drawing.Point(12, 23);
            this.pictureBoxRed.Name = "pictureBoxRed";
            this.pictureBoxRed.Size = new System.Drawing.Size(178, 136);
            this.pictureBoxRed.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBoxRed.TabIndex = 8;
            this.pictureBoxRed.TabStop = false;
            // 
            // pictureBoxQuestion
            // 
            this.pictureBoxQuestion.ContextMenuStrip = this.dialogContextMenuStrip;
            this.pictureBoxQuestion.Image = global::MeshAssistant.Properties.Resources.MeshIconHelp2;
            this.pictureBoxQuestion.Location = new System.Drawing.Point(12, 23);
            this.pictureBoxQuestion.Name = "pictureBoxQuestion";
            this.pictureBoxQuestion.Size = new System.Drawing.Size(178, 136);
            this.pictureBoxQuestion.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBoxQuestion.TabIndex = 10;
            this.pictureBoxQuestion.TabStop = false;
            this.pictureBoxQuestion.Visible = false;
            // 
            // pictureBoxGreen
            // 
            this.pictureBoxGreen.ContextMenuStrip = this.dialogContextMenuStrip;
            this.pictureBoxGreen.Image = global::MeshAssistant.Properties.Resources.MeshIconGreen2;
            this.pictureBoxGreen.Location = new System.Drawing.Point(12, 23);
            this.pictureBoxGreen.Name = "pictureBoxGreen";
            this.pictureBoxGreen.Size = new System.Drawing.Size(178, 136);
            this.pictureBoxGreen.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBoxGreen.TabIndex = 7;
            this.pictureBoxGreen.TabStop = false;
            // 
            // pictureBoxYellow
            // 
            this.pictureBoxYellow.ContextMenuStrip = this.dialogContextMenuStrip;
            this.pictureBoxYellow.Image = global::MeshAssistant.Properties.Resources.MeshIconGray2;
            this.pictureBoxYellow.Location = new System.Drawing.Point(12, 23);
            this.pictureBoxYellow.Name = "pictureBoxYellow";
            this.pictureBoxYellow.Size = new System.Drawing.Size(178, 136);
            this.pictureBoxYellow.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBoxYellow.TabIndex = 9;
            this.pictureBoxYellow.TabStop = false;
            // 
            // pictureBoxCustom
            // 
            this.pictureBoxCustom.ContextMenuStrip = this.dialogContextMenuStrip;
            this.pictureBoxCustom.Location = new System.Drawing.Point(36, 28);
            this.pictureBoxCustom.Name = "pictureBoxCustom";
            this.pictureBoxCustom.Size = new System.Drawing.Size(130, 130);
            this.pictureBoxCustom.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxCustom.TabIndex = 13;
            this.pictureBoxCustom.TabStop = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(79)))), ((int)(((byte)(130)))));
            this.ClientSize = new System.Drawing.Size(200, 225);
            this.Controls.Add(this.pictureBoxCustom);
            this.Controls.Add(this.remoteSessionsLabel);
            this.Controls.Add(this.requestHelpButton);
            this.Controls.Add(this.stateLabel);
            this.Controls.Add(this.pictureBoxUser);
            this.Controls.Add(this.pictureBoxRed);
            this.Controls.Add(this.pictureBoxQuestion);
            this.Controls.Add(this.pictureBoxGreen);
            this.Controls.Add(this.pictureBoxYellow);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.Text = "MeshCentral Assistant";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            this.mainContextMenuStrip.ResumeLayout(false);
            this.dialogContextMenuStrip.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxUser)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxRed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxQuestion)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxGreen)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxYellow)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCustom)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

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
        private System.Windows.Forms.PictureBox pictureBoxGreen;
        private System.Windows.Forms.PictureBox pictureBoxRed;
        private System.Windows.Forms.PictureBox pictureBoxYellow;
        private System.Windows.Forms.PictureBox pictureBoxQuestion;
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
        private System.Windows.Forms.PictureBox pictureBoxUser;
        private System.Windows.Forms.PictureBox pictureBoxCustom;
    }
}

