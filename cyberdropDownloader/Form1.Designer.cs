namespace cyberdropDownloader
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.urlBox = new System.Windows.Forms.TextBox();
            this.urlLabel = new System.Windows.Forms.Label();
            this.downloadBtn = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.filepath = new System.Windows.Forms.TextBox();
            this.destinationLabel = new System.Windows.Forms.Label();
            this.openFolderBtn = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.minimizeBtn = new System.Windows.Forms.Button();
            this.versionLabel = new System.Windows.Forms.Label();
            this.closeBtn = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.toolTip3 = new System.Windows.Forms.ToolTip(this.components);
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.issueUrlLabel = new System.Windows.Forms.LinkLabel();
            this.releasesUrlLabel = new System.Windows.Forms.LinkLabel();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // urlBox
            // 
            this.urlBox.AcceptsReturn = true;
            this.urlBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(34)))), ((int)(((byte)(37)))));
            this.urlBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.urlBox, "urlBox");
            this.urlBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(221)))), ((int)(((byte)(222)))));
            this.urlBox.Name = "urlBox";
            // 
            // urlLabel
            // 
            resources.ApplyResources(this.urlLabel, "urlLabel");
            this.urlLabel.ForeColor = System.Drawing.Color.White;
            this.urlLabel.Name = "urlLabel";
            // 
            // downloadBtn
            // 
            this.downloadBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(34)))), ((int)(((byte)(37)))));
            resources.ApplyResources(this.downloadBtn, "downloadBtn");
            this.downloadBtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.downloadBtn.FlatAppearance.BorderSize = 0;
            this.downloadBtn.ForeColor = System.Drawing.Color.Transparent;
            this.downloadBtn.Name = "downloadBtn";
            this.downloadBtn.UseVisualStyleBackColor = false;
            this.downloadBtn.Click += new System.EventHandler(this.downloadBtn_Click);
            // 
            // filepath
            // 
            this.filepath.AllowDrop = true;
            this.filepath.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(34)))), ((int)(((byte)(37)))));
            this.filepath.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.filepath.Cursor = System.Windows.Forms.Cursors.Hand;
            resources.ApplyResources(this.filepath, "filepath");
            this.filepath.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(221)))), ((int)(((byte)(222)))));
            this.filepath.Name = "filepath";
            this.filepath.Click += new System.EventHandler(this.filepath_Click);
            // 
            // destinationLabel
            // 
            resources.ApplyResources(this.destinationLabel, "destinationLabel");
            this.destinationLabel.ForeColor = System.Drawing.Color.White;
            this.destinationLabel.Name = "destinationLabel";
            // 
            // openFolderBtn
            // 
            resources.ApplyResources(this.openFolderBtn, "openFolderBtn");
            this.openFolderBtn.BackColor = System.Drawing.Color.Transparent;
            this.openFolderBtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.openFolderBtn.FlatAppearance.BorderSize = 0;
            this.openFolderBtn.ForeColor = System.Drawing.Color.Transparent;
            this.openFolderBtn.Image = global::cyberdropDownloader.Properties.Resources.folder;
            this.openFolderBtn.Name = "openFolderBtn";
            this.openFolderBtn.UseVisualStyleBackColor = false;
            this.openFolderBtn.Click += new System.EventHandler(this.openFolderBtn_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(34)))), ((int)(((byte)(37)))));
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Controls.Add(this.minimizeBtn);
            this.panel1.Controls.Add(this.versionLabel);
            this.panel1.Controls.Add(this.closeBtn);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            this.panel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseDown);
            this.panel1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseMove);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Image = global::cyberdropDownloader.Properties.Resources.duck15;
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // minimizeBtn
            // 
            resources.ApplyResources(this.minimizeBtn, "minimizeBtn");
            this.minimizeBtn.FlatAppearance.BorderSize = 0;
            this.minimizeBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(221)))), ((int)(((byte)(222)))));
            this.minimizeBtn.Name = "minimizeBtn";
            this.minimizeBtn.UseVisualStyleBackColor = true;
            this.minimizeBtn.Click += new System.EventHandler(this.minimizeBtn_Click);
            // 
            // versionLabel
            // 
            resources.ApplyResources(this.versionLabel, "versionLabel");
            this.versionLabel.BackColor = System.Drawing.Color.Transparent;
            this.versionLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(221)))), ((int)(((byte)(222)))));
            this.versionLabel.Name = "versionLabel";
            // 
            // closeBtn
            // 
            this.closeBtn.BackColor = System.Drawing.Color.Salmon;
            this.closeBtn.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.closeBtn, "closeBtn");
            this.closeBtn.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.closeBtn.Name = "closeBtn";
            this.closeBtn.UseVisualStyleBackColor = false;
            this.closeBtn.Click += new System.EventHandler(this.closeBtn_Click);
            // 
            // listBox1
            // 
            this.listBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(34)))), ((int)(((byte)(37)))));
            this.listBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listBox1.ForeColor = System.Drawing.Color.Chartreuse;
            this.listBox1.FormattingEnabled = true;
            resources.ApplyResources(this.listBox1, "listBox1");
            this.listBox1.Name = "listBox1";
            // 
            // toolTip3
            // 
            this.toolTip3.ToolTipTitle = "Choose destination folder";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.listBox1);
            this.groupBox1.ForeColor = System.Drawing.SystemColors.ControlDark;
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // issueUrlLabel
            // 
            this.issueUrlLabel.ActiveLinkColor = System.Drawing.Color.Lime;
            resources.ApplyResources(this.issueUrlLabel, "issueUrlLabel");
            this.issueUrlLabel.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.issueUrlLabel.LinkColor = System.Drawing.Color.Silver;
            this.issueUrlLabel.Name = "issueUrlLabel";
            this.issueUrlLabel.TabStop = true;
            this.issueUrlLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.issueUrlLabel_LinkClicked);
            // 
            // releasesUrlLabel
            // 
            this.releasesUrlLabel.ActiveLinkColor = System.Drawing.Color.Lime;
            resources.ApplyResources(this.releasesUrlLabel, "releasesUrlLabel");
            this.releasesUrlLabel.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.releasesUrlLabel.LinkColor = System.Drawing.Color.Silver;
            this.releasesUrlLabel.Name = "releasesUrlLabel";
            this.releasesUrlLabel.TabStop = true;
            this.releasesUrlLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.releasesUrlLabel_LinkClicked);
            // 
            // Form1
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(57)))), ((int)(((byte)(63)))));
            this.Controls.Add(this.releasesUrlLabel);
            this.Controls.Add(this.issueUrlLabel);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.openFolderBtn);
            this.Controls.Add(this.destinationLabel);
            this.Controls.Add(this.filepath);
            this.Controls.Add(this.downloadBtn);
            this.Controls.Add(this.urlLabel);
            this.Controls.Add(this.urlBox);
            this.Controls.Add(this.groupBox1);
            this.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseMove);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox urlBox;
        private System.Windows.Forms.Label urlLabel;
        private System.Windows.Forms.Button downloadBtn;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.TextBox filepath;
        private System.Windows.Forms.Label destinationLabel;
        private System.Windows.Forms.Button openFolderBtn;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button closeBtn;
        private System.Windows.Forms.Button minimizeBtn;
        private System.Windows.Forms.Label versionLabel;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.ToolTip toolTip3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.LinkLabel issueUrlLabel;
        private System.Windows.Forms.LinkLabel releasesUrlLabel;
    }
}

