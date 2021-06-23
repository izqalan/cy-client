using System;
using System.Drawing;
using System.Windows.Forms;
using Downloader;
using Syroot.Windows.IO;
using System.Diagnostics;
using System.Reflection;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace cyberdropDownloader
{
    public partial class Form1 : Form
    {
        private string path;
        private DownloadService downloader = new DownloadService(new DownloadConfiguration() {
            ChunkCount = 8, // file parts to download
            MaxTryAgainOnFailover = 3,
            OnTheFlyDownload = false,
            ParallelDownload = true,
            RequestConfiguration = {
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.149 Safari/537.36"
            }
        });
        public Point mouseLocation;

        public Form1()
        {
            InitializeComponent();
            this.AcceptButton = downloadBtn;
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            mouseLocation = new Point(-e.X, -e.Y);
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point mousePos = Control.MousePosition;
                mousePos.Offset(mouseLocation.X, mouseLocation.Y);
                Location = mousePos;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ToolTip toolTip1 = new ToolTip();
            ToolTip toolTip3 = new ToolTip();
            string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            filepath.Text = Properties.Settings.Default["destinationPath"].ToString().Length > 0
                    ? Properties.Settings.Default["destinationPath"].ToString() : KnownFolders.Downloads.Path;
            this.path = filepath.Text;
            this.versionLabel.Text = $"cy client - v{version}";
            downloader.DownloadStarted += OnDownloadStarted;
            toolTip1.AutoPopDelay = 5000;
            toolTip1.InitialDelay = 500;
            toolTip1.ReshowDelay = 500;
            toolTip1.ShowAlways = true;
            toolTip1.SetToolTip(this.openFolderBtn, "Open destination folder in explorer");
            toolTip1.SetToolTip(this.filepath, "Choose file destination");
        }

        private void OnDownloadStarted(object sender, DownloadStartedEventArgs e)
        {
            Invoke(new MethodInvoker(delegate ()
            {
                listBox1.Items.Insert(0, "Downloading item: " + System.IO.Path.GetFileName(e.FileName));
            }));
        }

        private void downloadBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(urlBox.Text))
                {
                    throw new System.ArgumentException("URL cannot be null");
                }
                var scraper = new CyScraper(urlBox, this.path, this.downloader, listBox1, albumTitle);
                scraper.StartAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void filepath_Click(object sender, EventArgs e)
        {
            using (CommonOpenFileDialog dialog = new CommonOpenFileDialog())
            {
                dialog.InitialDirectory = this.path;
                dialog.IsFolderPicker = true;
                dialog.RestoreDirectory = true;
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    Properties.Settings.Default["destinationPath"] = dialog.FileName;
                    Properties.Settings.Default.Save();
                    filepath.Text = dialog.FileName;
                    this.path = filepath.Text;
                }
            }
        }

        private void openFolderBtn_Click(object sender, EventArgs e)
        {
            Process.Start(path);
        }

        private void closeBtn_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void minimizeBtn_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void issueUrlLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://github.com/izqalan/cy-client/issues");
        }

        private void releasesUrlLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://github.com/izqalan/cy-client/releases");
        }
    }
}
