using System;
using System.Drawing;
using System.Windows.Forms;
using Downloader;
using Syroot.Windows.IO;
using System.Diagnostics;
using System.Reflection;

namespace cyberdropDownloader
{
    public partial class Form1 : Form
    {
        private String path;
        private DownloadService downloader = new DownloadService();
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
            filepath.Text = KnownFolders.Downloads.Path;
            this.path = filepath.Text;
            this.versionLabel.Text = $"cy client - v{version}";
            downloader.ChunkDownloadProgressChanged += OnChunkDownloadProgressChanged;
            toolTip1.AutoPopDelay = 5000;
            toolTip1.InitialDelay = 500;
            toolTip1.ReshowDelay = 500;
            toolTip1.ShowAlways = true;
            toolTip1.SetToolTip(this.openFolderBtn, "Open destination folder in explorer");
            toolTip1.SetToolTip(this.filepath, "Choose file destination");
        }

        private void OnChunkDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            Invoke(new MethodInvoker(delegate ()
            {
                //downloadProgressBar.Minimum = 0;
                //double receive = e.ReceivedBytesSize;
                //double total = e.TotalBytesToReceive;
                //double percentage = receive / total * 100;
                //progressLabel.Text = $"Downloading {string.Format("{0}", percentage)}%";
                // downloadProgressBar.Value = int.Parse(Math.Truncate(percentage).ToString());
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
                var scraper = new CyScraper(urlBox, this.path, this.downloader, listBox1);
                scraper.StartAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void filepath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.SelectedPath = KnownFolders.Downloads.Path;
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                filepath.Text = fbd.SelectedPath;
                this.path = filepath.Text;
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
