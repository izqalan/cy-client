using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HtmlAgilityPack;
using Downloader;
using Syroot.Windows.IO;
using System.IO;

namespace cyberdropDownloader
{
    public partial class Form1 : Form
    {
        private String path;
        private DownloadService downloader = new DownloadService();
        CyScraper scraper;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            filepath.Text = KnownFolders.Downloads.Path;
            this.path = filepath.Text;
            downloader.ChunkDownloadProgressChanged += OnChunkDownloadProgressChanged;
        }

        private void OnChunkDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            Invoke(new MethodInvoker(delegate ()
            {
                downloadProgressBar.Minimum = 0;
                double receive = double.Parse(e.BytesReceived.ToString());
                double total = double.Parse(e.TotalBytesToReceive.ToString());
                double percentage = receive / total * 100;
                progressLabel.Text = $"Downloading {string.Format("{0:0}", percentage)}%";
                downloadProgressBar.Value = int.Parse(Math.Truncate(percentage).ToString());
            }));
        }

        private void downloadBtn_Click(object sender, EventArgs e)
        {
            var scraper = new CyScraper(urlBox.Text, this.path, this.downloader);
            scraper.StartAsync();
        }

        private void filepath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                filepath.Text = fbd.SelectedPath;
                this.path = filepath.Text;
            }
             
        }

    }

    public class CyScraper
    {
        private string url;
        private string dest;
        private DownloadService downloader;
        public string itemName;
        public CyScraper(String url, String path, DownloadService downloader)
        {
            this.url = url;
            this.dest = path;
            this.downloader = downloader;
        }

        private string title(HtmlAgilityPack.HtmlDocument htmlDoc)
        {
            var title = htmlDoc.DocumentNode.SelectNodes("//div/h1[@id='title']").First().Attributes["title"].Value;
            return title;
        }

        public async void StartAsync()
        {
            HtmlWeb web = new HtmlWeb();
            var htmlDoc = web.Load(url);
            string title = this.title(htmlDoc);

            // string dir = String.Format(@"{0}\\{1}\\", dest, title);
            // DirectoryInfo di = Directory.CreateDirectory(dir);

            foreach (HtmlNode link in htmlDoc.DocumentNode.SelectNodes("//a[@class='image'][@href]"))
            {
                string url = link.Attributes["href"].Value;
                itemName = link.Attributes["title"].Value;
                // download here
                string filepath = String.Format(@"{0}\\{1}\\{2}", dest, title, itemName);
                Console.WriteLine(filepath);
                await downloader.DownloadFileAsync(url, filepath);
            }
        }
    }


}
