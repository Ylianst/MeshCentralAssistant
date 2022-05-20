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
            this.stateListView = new System.Windows.Forms.ListView();
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.versionsTabPage = new System.Windows.Forms.TabPage();
            this.versionsListView = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.mainTabControl.SuspendLayout();
            this.stateTabPage.SuspendLayout();
            this.versionsTabPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // closeButton
            // 
            resources.ApplyResources(this.closeButton, "closeButton");
            this.closeButton.Name = "closeButton";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Name = "label1";
            // 
            // mainTabControl
            // 
            resources.ApplyResources(this.mainTabControl, "mainTabControl");
            this.mainTabControl.Controls.Add(this.stateTabPage);
            this.mainTabControl.Controls.Add(this.versionsTabPage);
            this.mainTabControl.Name = "mainTabControl";
            this.mainTabControl.SelectedIndex = 0;
            // 
            // stateTabPage
            // 
            this.stateTabPage.Controls.Add(this.stateListView);
            resources.ApplyResources(this.stateTabPage, "stateTabPage");
            this.stateTabPage.Name = "stateTabPage";
            this.stateTabPage.UseVisualStyleBackColor = true;
            // 
            // stateListView
            // 
            this.stateListView.BackColor = System.Drawing.Color.AliceBlue;
            this.stateListView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.stateListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader4,
            this.columnHeader5});
            resources.ApplyResources(this.stateListView, "stateListView");
            this.stateListView.ForeColor = System.Drawing.Color.Black;
            this.stateListView.FullRowSelect = true;
            this.stateListView.GridLines = true;
            this.stateListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.stateListView.Name = "stateListView";
            this.stateListView.UseCompatibleStateImageBehavior = false;
            this.stateListView.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader4
            // 
            resources.ApplyResources(this.columnHeader4, "columnHeader4");
            // 
            // columnHeader5
            // 
            resources.ApplyResources(this.columnHeader5, "columnHeader5");
            // 
            // versionsTabPage
            // 
            this.versionsTabPage.Controls.Add(this.versionsListView);
            resources.ApplyResources(this.versionsTabPage, "versionsTabPage");
            this.versionsTabPage.Name = "versionsTabPage";
            this.versionsTabPage.UseVisualStyleBackColor = true;
            // 
            // versionsListView
            // 
            this.versionsListView.BackColor = System.Drawing.Color.AliceBlue;
            this.versionsListView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.versionsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            resources.ApplyResources(this.versionsListView, "versionsListView");
            this.versionsListView.ForeColor = System.Drawing.Color.Black;
            this.versionsListView.FullRowSelect = true;
            this.versionsListView.GridLines = true;
            this.versionsListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.versionsListView.Name = "versionsListView";
            this.versionsListView.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.versionsListView.UseCompatibleStateImageBehavior = false;
            this.versionsListView.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            resources.ApplyResources(this.columnHeader1, "columnHeader1");
            // 
            // columnHeader2
            // 
            resources.ApplyResources(this.columnHeader2, "columnHeader2");
            // 
            // MeInfoForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(79)))), ((int)(((byte)(130)))));
            this.Controls.Add(this.mainTabControl);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.closeButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MeInfoForm";
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