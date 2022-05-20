namespace MeshAssistant
{
    partial class GuestSharingForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GuestSharingForm));
            this.closeButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.createLinkButton = new System.Windows.Forms.Button();
            this.filesCheckBox = new System.Windows.Forms.CheckBox();
            this.terminalCheckBox = new System.Windows.Forms.CheckBox();
            this.desktopCheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.linkTextBox = new System.Windows.Forms.TextBox();
            this.copyLinkButton = new System.Windows.Forms.Button();
            this.cancelSharingButton = new System.Windows.Forms.Button();
            this.viewOnlyCheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // closeButton
            // 
            this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.closeButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.closeButton.Location = new System.Drawing.Point(279, 278);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(75, 23);
            this.closeButton.TabIndex = 0;
            this.closeButton.Text = "Close";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(342, 53);
            this.label1.TabIndex = 1;
            this.label1.Text = "You can share your computer with anyone by creating a sharing URL and giving it t" +
    "o a friend. Access is temporary and will require your confirmation before the se" +
    "ssion can start.";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.viewOnlyCheckBox);
            this.groupBox1.Controls.Add(this.createLinkButton);
            this.groupBox1.Controls.Add(this.filesCheckBox);
            this.groupBox1.Controls.Add(this.terminalCheckBox);
            this.groupBox1.Controls.Add(this.desktopCheckBox);
            this.groupBox1.Location = new System.Drawing.Point(15, 56);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(339, 125);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Sharing Settings";
            // 
            // createLinkButton
            // 
            this.createLinkButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.createLinkButton.Enabled = false;
            this.createLinkButton.Location = new System.Drawing.Point(224, 96);
            this.createLinkButton.Name = "createLinkButton";
            this.createLinkButton.Size = new System.Drawing.Size(99, 23);
            this.createLinkButton.TabIndex = 3;
            this.createLinkButton.Text = "Create Link";
            this.createLinkButton.UseVisualStyleBackColor = true;
            this.createLinkButton.Click += new System.EventHandler(this.createLinkButton_Click);
            // 
            // filesCheckBox
            // 
            this.filesCheckBox.AutoSize = true;
            this.filesCheckBox.Location = new System.Drawing.Point(16, 98);
            this.filesCheckBox.Name = "filesCheckBox";
            this.filesCheckBox.Size = new System.Drawing.Size(47, 17);
            this.filesCheckBox.TabIndex = 2;
            this.filesCheckBox.Text = "Files";
            this.filesCheckBox.UseVisualStyleBackColor = true;
            this.filesCheckBox.CheckedChanged += new System.EventHandler(this.desktopCheckBox_CheckedChanged);
            // 
            // terminalCheckBox
            // 
            this.terminalCheckBox.AutoSize = true;
            this.terminalCheckBox.Location = new System.Drawing.Point(16, 75);
            this.terminalCheckBox.Name = "terminalCheckBox";
            this.terminalCheckBox.Size = new System.Drawing.Size(66, 17);
            this.terminalCheckBox.TabIndex = 1;
            this.terminalCheckBox.Text = "Terminal";
            this.terminalCheckBox.UseVisualStyleBackColor = true;
            this.terminalCheckBox.CheckedChanged += new System.EventHandler(this.desktopCheckBox_CheckedChanged);
            // 
            // desktopCheckBox
            // 
            this.desktopCheckBox.AutoSize = true;
            this.desktopCheckBox.Location = new System.Drawing.Point(16, 28);
            this.desktopCheckBox.Name = "desktopCheckBox";
            this.desktopCheckBox.Size = new System.Drawing.Size(66, 17);
            this.desktopCheckBox.TabIndex = 0;
            this.desktopCheckBox.Text = "Desktop";
            this.desktopCheckBox.UseVisualStyleBackColor = true;
            this.desktopCheckBox.CheckedChanged += new System.EventHandler(this.desktopCheckBox_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.linkTextBox);
            this.groupBox2.Controls.Add(this.copyLinkButton);
            this.groupBox2.Controls.Add(this.cancelSharingButton);
            this.groupBox2.Location = new System.Drawing.Point(15, 187);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(339, 85);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Sharing Link";
            // 
            // linkTextBox
            // 
            this.linkTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.linkTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkTextBox.Location = new System.Drawing.Point(16, 22);
            this.linkTextBox.Name = "linkTextBox";
            this.linkTextBox.ReadOnly = true;
            this.linkTextBox.Size = new System.Drawing.Size(307, 26);
            this.linkTextBox.TabIndex = 5;
            // 
            // copyLinkButton
            // 
            this.copyLinkButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.copyLinkButton.Enabled = false;
            this.copyLinkButton.Location = new System.Drawing.Point(119, 56);
            this.copyLinkButton.Name = "copyLinkButton";
            this.copyLinkButton.Size = new System.Drawing.Size(99, 23);
            this.copyLinkButton.TabIndex = 4;
            this.copyLinkButton.Text = "Copy Link";
            this.copyLinkButton.UseVisualStyleBackColor = true;
            this.copyLinkButton.Click += new System.EventHandler(this.copyLinkButton_Click);
            // 
            // cancelSharingButton
            // 
            this.cancelSharingButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelSharingButton.Enabled = false;
            this.cancelSharingButton.Location = new System.Drawing.Point(224, 56);
            this.cancelSharingButton.Name = "cancelSharingButton";
            this.cancelSharingButton.Size = new System.Drawing.Size(99, 23);
            this.cancelSharingButton.TabIndex = 3;
            this.cancelSharingButton.Text = "Cancel Sharing";
            this.cancelSharingButton.UseVisualStyleBackColor = true;
            this.cancelSharingButton.Click += new System.EventHandler(this.cancelSharingButton_Click);
            // 
            // viewOnlyCheckBox
            // 
            this.viewOnlyCheckBox.AutoSize = true;
            this.viewOnlyCheckBox.Location = new System.Drawing.Point(32, 51);
            this.viewOnlyCheckBox.Name = "viewOnlyCheckBox";
            this.viewOnlyCheckBox.Size = new System.Drawing.Size(71, 17);
            this.viewOnlyCheckBox.TabIndex = 4;
            this.viewOnlyCheckBox.Text = "View-only";
            this.viewOnlyCheckBox.UseVisualStyleBackColor = true;
            // 
            // GuestSharingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.closeButton;
            this.ClientSize = new System.Drawing.Size(366, 313);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.closeButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GuestSharingForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Guest Sharing";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.GuestSharingForm_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button createLinkButton;
        private System.Windows.Forms.CheckBox filesCheckBox;
        private System.Windows.Forms.CheckBox terminalCheckBox;
        private System.Windows.Forms.CheckBox desktopCheckBox;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox linkTextBox;
        private System.Windows.Forms.Button copyLinkButton;
        private System.Windows.Forms.Button cancelSharingButton;
        private System.Windows.Forms.CheckBox viewOnlyCheckBox;
    }
}