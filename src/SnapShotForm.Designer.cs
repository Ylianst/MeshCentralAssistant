namespace MeshAssistant
{
    partial class SnapShotForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SnapShotForm));
            this.mainTextBox = new System.Windows.Forms.TextBox();
            this.closeButton = new System.Windows.Forms.Button();
            this.mainTimer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // mainTextBox
            // 
            resources.ApplyResources(this.mainTextBox, "mainTextBox");
            this.mainTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(79)))), ((int)(((byte)(130)))));
            this.mainTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.mainTextBox.ForeColor = System.Drawing.Color.White;
            this.mainTextBox.Name = "mainTextBox";
            this.mainTextBox.ReadOnly = true;
            // 
            // closeButton
            // 
            resources.ApplyResources(this.closeButton, "closeButton");
            this.closeButton.Name = "closeButton";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // mainTimer
            // 
            this.mainTimer.Enabled = true;
            this.mainTimer.Interval = 1000;
            this.mainTimer.Tick += new System.EventHandler(this.mainTimer_Tick);
            // 
            // SnapShotForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(79)))), ((int)(((byte)(130)))));
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.mainTextBox);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SnapShotForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SnapShotForm_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox mainTextBox;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Timer mainTimer;
    }
}