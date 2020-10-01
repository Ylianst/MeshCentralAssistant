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
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.startAgentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopAgentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.connectionTimer = new System.Windows.Forms.Timer(this.components);
            this.requestHelpButton = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.mainContextMenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
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
            this.stateLabel.Size = new System.Drawing.Size(178, 13);
            this.stateLabel.TabIndex = 4;
            this.stateLabel.Text = "Agent is missing";
            this.stateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // mainNotifyIcon
            // 
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
            this.toolStripMenuItem1,
            this.startAgentToolStripMenuItem,
            this.stopAgentToolStripMenuItem,
            this.toolStripMenuItem2,
            this.exitToolStripMenuItem});
            this.mainContextMenuStrip.Name = "mainContextMenuStrip";
            this.mainContextMenuStrip.Size = new System.Drawing.Size(135, 148);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this.openToolStripMenuItem.Text = "&Open...";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this.closeToolStripMenuItem.Text = "&Close";
            this.closeToolStripMenuItem.Visible = false;
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // openSiteToolStripMenuItem
            // 
            this.openSiteToolStripMenuItem.Name = "openSiteToolStripMenuItem";
            this.openSiteToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this.openSiteToolStripMenuItem.Text = "O&pen Site...";
            this.openSiteToolStripMenuItem.Visible = false;
            this.openSiteToolStripMenuItem.Click += new System.EventHandler(this.openSiteToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(131, 6);
            // 
            // startAgentToolStripMenuItem
            // 
            this.startAgentToolStripMenuItem.Name = "startAgentToolStripMenuItem";
            this.startAgentToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this.startAgentToolStripMenuItem.Text = "&Start Agent";
            this.startAgentToolStripMenuItem.Click += new System.EventHandler(this.startAgentToolStripMenuItem_Click);
            // 
            // stopAgentToolStripMenuItem
            // 
            this.stopAgentToolStripMenuItem.Name = "stopAgentToolStripMenuItem";
            this.stopAgentToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this.stopAgentToolStripMenuItem.Text = "S&top Agent";
            this.stopAgentToolStripMenuItem.Click += new System.EventHandler(this.stopAgentToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(131, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
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
            this.requestHelpButton.Location = new System.Drawing.Point(12, 160);
            this.requestHelpButton.Name = "requestHelpButton";
            this.requestHelpButton.Size = new System.Drawing.Size(178, 31);
            this.requestHelpButton.TabIndex = 6;
            this.requestHelpButton.Text = "Request Help";
            this.requestHelpButton.UseVisualStyleBackColor = true;
            this.requestHelpButton.Click += new System.EventHandler(this.requestHelpButton_Click);
            this.requestHelpButton.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::MeshAssistant.Properties.Resources.MeshIconGreen2;
            this.pictureBox1.Location = new System.Drawing.Point(12, 23);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(178, 136);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 7;
            this.pictureBox1.TabStop = false;
            // 
            // pictureBox3
            // 
            this.pictureBox3.Image = global::MeshAssistant.Properties.Resources.MeshIconGray2;
            this.pictureBox3.Location = new System.Drawing.Point(12, 23);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(178, 136);
            this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox3.TabIndex = 9;
            this.pictureBox3.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::MeshAssistant.Properties.Resources.MeshIconRed2;
            this.pictureBox2.Location = new System.Drawing.Point(12, 23);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(178, 136);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox2.TabIndex = 8;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox4
            // 
            this.pictureBox4.Image = global::MeshAssistant.Properties.Resources.MeshIconHelp2;
            this.pictureBox4.Location = new System.Drawing.Point(12, 23);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(178, 136);
            this.pictureBox4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox4.TabIndex = 10;
            this.pictureBox4.TabStop = false;
            this.pictureBox4.Visible = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(79)))), ((int)(((byte)(130)))));
            this.ClientSize = new System.Drawing.Size(204, 203);
            this.Controls.Add(this.pictureBox4);
            this.Controls.Add(this.requestHelpButton);
            this.Controls.Add(this.stateLabel);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.pictureBox3);
            this.Controls.Add(this.pictureBox2);
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
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
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
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.PictureBox pictureBox4;
    }
}

