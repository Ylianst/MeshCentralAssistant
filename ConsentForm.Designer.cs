namespace MeshAssistant
{
    partial class ConsentForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConsentForm));
            this.cancelButton = new System.Windows.Forms.Button();
            this.mainPictureBox = new System.Windows.Forms.PictureBox();
            this.okButton = new System.Windows.Forms.Button();
            this.mainLabel = new System.Windows.Forms.Label();
            this.nameLabel = new System.Windows.Forms.Label();
            this.autoConsentCheckBox = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.mainPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // cancelButton
            // 
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // mainPictureBox
            // 
            resources.ApplyResources(this.mainPictureBox, "mainPictureBox");
            this.mainPictureBox.Image = global::MeshAssistant.Properties.Resources.User;
            this.mainPictureBox.InitialImage = global::MeshAssistant.Properties.Resources.User;
            this.mainPictureBox.Name = "mainPictureBox";
            this.mainPictureBox.TabStop = false;
            // 
            // okButton
            // 
            resources.ApplyResources(this.okButton, "okButton");
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.okButton.Name = "okButton";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // mainLabel
            // 
            resources.ApplyResources(this.mainLabel, "mainLabel");
            this.mainLabel.ForeColor = System.Drawing.Color.White;
            this.mainLabel.Name = "mainLabel";
            // 
            // nameLabel
            // 
            resources.ApplyResources(this.nameLabel, "nameLabel");
            this.nameLabel.ForeColor = System.Drawing.Color.Gainsboro;
            this.nameLabel.Name = "nameLabel";
            // 
            // autoConsentCheckBox
            // 
            this.autoConsentCheckBox.ForeColor = System.Drawing.Color.White;
            resources.ApplyResources(this.autoConsentCheckBox, "autoConsentCheckBox");
            this.autoConsentCheckBox.Name = "autoConsentCheckBox";
            this.autoConsentCheckBox.UseVisualStyleBackColor = true;
            // 
            // ConsentForm
            // 
            this.AcceptButton = this.cancelButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(79)))), ((int)(((byte)(130)))));
            this.CancelButton = this.cancelButton;
            this.Controls.Add(this.autoConsentCheckBox);
            this.Controls.Add(this.nameLabel);
            this.Controls.Add(this.mainLabel);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.mainPictureBox);
            this.Controls.Add(this.cancelButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConsentForm";
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ConsentForm_FormClosed);
            this.Load += new System.EventHandler(this.ConsentForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.mainPictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.PictureBox mainPictureBox;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Label mainLabel;
        private System.Windows.Forms.Label nameLabel;
        private System.Windows.Forms.CheckBox autoConsentCheckBox;
    }
}