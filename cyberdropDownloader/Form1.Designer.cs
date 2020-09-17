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
            this.downloadProgressBar = new System.Windows.Forms.ProgressBar();
            this.progressLabel = new System.Windows.Forms.Label();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.filepath = new System.Windows.Forms.TextBox();
            this.destinationLabel = new System.Windows.Forms.Label();
            this.openFolderBtn = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // urlBox
            // 
            this.urlBox.AcceptsReturn = true;
            resources.ApplyResources(this.urlBox, "urlBox");
            this.urlBox.Name = "urlBox";
            // 
            // urlLabel
            // 
            resources.ApplyResources(this.urlLabel, "urlLabel");
            this.urlLabel.Name = "urlLabel";
            // 
            // downloadBtn
            // 
            resources.ApplyResources(this.downloadBtn, "downloadBtn");
            this.downloadBtn.Name = "downloadBtn";
            this.downloadBtn.UseVisualStyleBackColor = true;
            this.downloadBtn.Click += new System.EventHandler(this.downloadBtn_Click);
            // 
            // downloadProgressBar
            // 
            resources.ApplyResources(this.downloadProgressBar, "downloadProgressBar");
            this.downloadProgressBar.Name = "downloadProgressBar";
            // 
            // progressLabel
            // 
            resources.ApplyResources(this.progressLabel, "progressLabel");
            this.progressLabel.Name = "progressLabel";
            // 
            // filepath
            // 
            this.filepath.AllowDrop = true;
            this.filepath.Cursor = System.Windows.Forms.Cursors.Hand;
            resources.ApplyResources(this.filepath, "filepath");
            this.filepath.Name = "filepath";
            this.filepath.Click += new System.EventHandler(this.filepath_Click);
            // 
            // destinationLabel
            // 
            resources.ApplyResources(this.destinationLabel, "destinationLabel");
            this.destinationLabel.Name = "destinationLabel";
            // 
            // openFolderBtn
            // 
            resources.ApplyResources(this.openFolderBtn, "openFolderBtn");
            this.openFolderBtn.BackColor = System.Drawing.Color.Transparent;
            this.openFolderBtn.Image = global::cyberdropDownloader.Properties.Resources.folder_1_;
            this.openFolderBtn.Name = "openFolderBtn";
            this.openFolderBtn.UseVisualStyleBackColor = false;
            this.openFolderBtn.Click += new System.EventHandler(this.openFolderBtn_Click);
            // 
            // Form1
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.openFolderBtn);
            this.Controls.Add(this.destinationLabel);
            this.Controls.Add(this.filepath);
            this.Controls.Add(this.progressLabel);
            this.Controls.Add(this.downloadProgressBar);
            this.Controls.Add(this.downloadBtn);
            this.Controls.Add(this.urlLabel);
            this.Controls.Add(this.urlBox);
            this.Cursor = System.Windows.Forms.Cursors.Hand;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox urlBox;
        private System.Windows.Forms.Label urlLabel;
        private System.Windows.Forms.Button downloadBtn;
        private System.Windows.Forms.ProgressBar downloadProgressBar;
        private System.Windows.Forms.Label progressLabel;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.TextBox filepath;
        private System.Windows.Forms.Label destinationLabel;
        private System.Windows.Forms.Button openFolderBtn;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}

