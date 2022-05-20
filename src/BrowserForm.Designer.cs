namespace MeshAssistant
{
    partial class BrowserForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BrowserForm));
            this.mainBrowser = new System.Windows.Forms.WebBrowser();
            this.SuspendLayout();
            // 
            // mainBrowser
            // 
            this.mainBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainBrowser.Location = new System.Drawing.Point(0, 0);
            this.mainBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.mainBrowser.Name = "mainBrowser";
            this.mainBrowser.Size = new System.Drawing.Size(554, 525);
            this.mainBrowser.TabIndex = 0;
            this.mainBrowser.Url = new System.Uri("", System.UriKind.Relative);
            // 
            // BrowserForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(554, 525);
            this.Controls.Add(this.mainBrowser);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "BrowserForm";
            this.Text = "Messenger";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BrowserForm_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.WebBrowser mainBrowser;
    }
}