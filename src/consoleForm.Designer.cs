namespace MeshAssistant
{
    partial class ConsoleForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConsoleForm));
            this.sendTextBox = new System.Windows.Forms.TextBox();
            this.clearButton = new System.Windows.Forms.Button();
            this.sendButton = new System.Windows.Forms.Button();
            this.mainTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // sendTextBox
            // 
            resources.ApplyResources(this.sendTextBox, "sendTextBox");
            this.sendTextBox.Name = "sendTextBox";
            this.sendTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.sendTextBox_KeyPress);
            this.sendTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.sendTextBox_KeyUp);
            // 
            // clearButton
            // 
            resources.ApplyResources(this.clearButton, "clearButton");
            this.clearButton.Name = "clearButton";
            this.clearButton.UseVisualStyleBackColor = true;
            this.clearButton.Click += new System.EventHandler(this.clearButton_Click);
            // 
            // sendButton
            // 
            resources.ApplyResources(this.sendButton, "sendButton");
            this.sendButton.Name = "sendButton";
            this.sendButton.UseVisualStyleBackColor = true;
            this.sendButton.Click += new System.EventHandler(this.sendButton_Click);
            // 
            // mainTextBox
            // 
            resources.ApplyResources(this.mainTextBox, "mainTextBox");
            this.mainTextBox.BackColor = System.Drawing.Color.Black;
            this.mainTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.mainTextBox.ForeColor = System.Drawing.Color.DarkGray;
            this.mainTextBox.Name = "mainTextBox";
            this.mainTextBox.ReadOnly = true;
            // 
            // ConsoleForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(79)))), ((int)(((byte)(130)))));
            this.Controls.Add(this.mainTextBox);
            this.Controls.Add(this.sendButton);
            this.Controls.Add(this.clearButton);
            this.Controls.Add(this.sendTextBox);
            this.Name = "ConsoleForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ConsoleForm_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox sendTextBox;
        private System.Windows.Forms.Button clearButton;
        private System.Windows.Forms.Button sendButton;
        private System.Windows.Forms.TextBox mainTextBox;
    }
}