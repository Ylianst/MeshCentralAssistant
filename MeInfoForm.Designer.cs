namespace MeshAssistant
{
    partial class MeInfoForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MeInfoForm));
            this.closeButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.mainTabControl = new System.Windows.Forms.TabControl();
            this.stateTabPage = new System.Windows.Forms.TabPage();
            this.versionsTabPage = new System.Windows.Forms.TabPage();
            this.versionsListView = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.stateListView = new System.Windows.Forms.ListView();
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.mainTabControl.SuspendLayout();
            this.stateTabPage.SuspendLayout();
            this.versionsTabPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // closeButton
            // 
            this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.closeButton.Location = new System.Drawing.Point(271, 324);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(75, 23);
            this.closeButton.TabIndex = 7;
            this.closeButton.Text = "Close";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(334, 18);
            this.label1.TabIndex = 8;
            this.label1.Text = "Intel® Management Engine state for this computer.";
            // 
            // mainTabControl
            // 
            this.mainTabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mainTabControl.Controls.Add(this.stateTabPage);
            this.mainTabControl.Controls.Add(this.versionsTabPage);
            this.mainTabControl.Location = new System.Drawing.Point(12, 30);
            this.mainTabControl.Name = "mainTabControl";
            this.mainTabControl.SelectedIndex = 0;
            this.mainTabControl.Size = new System.Drawing.Size(334, 288);
            this.mainTabControl.TabIndex = 9;
            // 
            // stateTabPage
            // 
            this.stateTabPage.Controls.Add(this.stateListView);
            this.stateTabPage.Location = new System.Drawing.Point(4, 22);
            this.stateTabPage.Name = "stateTabPage";
            this.stateTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.stateTabPage.Size = new System.Drawing.Size(326, 262);
            this.stateTabPage.TabIndex = 0;
            this.stateTabPage.Text = "State";
            this.stateTabPage.UseVisualStyleBackColor = true;
            // 
            // versionsTabPage
            // 
            this.versionsTabPage.Controls.Add(this.versionsListView);
            this.versionsTabPage.Location = new System.Drawing.Point(4, 22);
            this.versionsTabPage.Name = "versionsTabPage";
            this.versionsTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.versionsTabPage.Size = new System.Drawing.Size(326, 262);
            this.versionsTabPage.TabIndex = 1;
            this.versionsTabPage.Text = "Versions";
            this.versionsTabPage.UseVisualStyleBackColor = true;
            // 
            // versionsListView
            // 
            this.versionsListView.BackColor = System.Drawing.Color.AliceBlue;
            this.versionsListView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.versionsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.versionsListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.versionsListView.ForeColor = System.Drawing.Color.Black;
            this.versionsListView.FullRowSelect = true;
            this.versionsListView.GridLines = true;
            this.versionsListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.versionsListView.Location = new System.Drawing.Point(3, 3);
            this.versionsListView.Name = "versionsListView";
            this.versionsListView.Size = new System.Drawing.Size(320, 256);
            this.versionsListView.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.versionsListView.TabIndex = 8;
            this.versionsListView.UseCompatibleStateImageBehavior = false;
            this.versionsListView.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Item";
            this.columnHeader1.Width = 130;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Value";
            this.columnHeader2.Width = 170;
            // 
            // stateListView
            // 
            this.stateListView.BackColor = System.Drawing.Color.AliceBlue;
            this.stateListView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.stateListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader4,
            this.columnHeader5});
            this.stateListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stateListView.ForeColor = System.Drawing.Color.Black;
            this.stateListView.FullRowSelect = true;
            this.stateListView.GridLines = true;
            this.stateListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.stateListView.Location = new System.Drawing.Point(3, 3);
            this.stateListView.Name = "stateListView";
            this.stateListView.Size = new System.Drawing.Size(320, 256);
            this.stateListView.TabIndex = 8;
            this.stateListView.UseCompatibleStateImageBehavior = false;
            this.stateListView.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Item";
            this.columnHeader4.Width = 130;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Value";
            this.columnHeader5.Width = 170;
            // 
            // MeInfoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(79)))), ((int)(((byte)(130)))));
            this.ClientSize = new System.Drawing.Size(358, 359);
            this.Controls.Add(this.mainTabControl);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.closeButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MeInfoForm";
            this.Text = "Intel® Management Engine";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MeInfoForm_FormClosing);
            this.mainTabControl.ResumeLayout(false);
            this.stateTabPage.ResumeLayout(false);
            this.versionsTabPage.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabControl mainTabControl;
        private System.Windows.Forms.TabPage stateTabPage;
        private System.Windows.Forms.TabPage versionsTabPage;
        private System.Windows.Forms.ListView stateListView;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ListView versionsListView;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
    }
}