using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Downloader;
using HtmlAgilityPack;
using System.Windows.Forms;

namespace cyberdropDownloader
{
    public class CyScraper
    {
        private TextBox url;
        private string dest;
        private DownloadService downloader;
        public string itemName;
        ListBox listBox;
        public CyScraper(TextBox url, String path, DownloadService downloader, ListBox listBox)
        {
            this.url = url;
            this.dest = path;
            this.downloader = downloader;
            this.listBox = listBox;
        }

        public CyScraper()
        {
        }

        public string GetTitle(HtmlAgilityPack.HtmlDocument htmlDoc)
        {
            var title = htmlDoc.DocumentNode.SelectNodes("//div/h1[@id='title']").First().Attributes["title"].Value;
            return title;
        }

        public List<string> GetAlbumUrls(HtmlAgilityPack.HtmlDocument htmlDoc)
        {
            List<string> urls = new List<string>();

            foreach (HtmlNode link in htmlDoc.DocumentNode.SelectNodes("//a[@class='image'][@href]"))
            {
                string url = link.Attributes["href"].Value;
                urls.Add(url);
            }
            return urls;
        }

        private async void GetUrlsAndDownload(HtmlAgilityPack.HtmlDocument htmlDoc)
        {
            string title = this.GetTitle(htmlDoc);
            // scuffed af; having form controls in business logic
            listBox.Items.Insert(0, "Album: " + title);
            foreach (HtmlNode link in htmlDoc.DocumentNode.SelectNodes("//a[@class='image'][@href]"))
            {
                string url = link.Attributes["href"].Value;
                itemName = link.Attributes["title"].Value;
                // scuffed af; having form controls in business logic
                listBox.Items.Insert(0, "Downloading item: " + itemName);
                // download here
                string filepath = String.Format(@"{0}\{1}\{2}", dest, title, itemName);
                await downloader.DownloadFileAsync(url, filepath);
            }
        }

        public void StartAsync()
        {
            try
            {
                for (int i = 0; i < url.Lines.Length; i++)
                {
                    HtmlWeb web = new HtmlWeb();
                    var htmlDoc = web.Load(url.Lines[i]);
                    GetUrlsAndDownload(htmlDoc);
                }
                
            }
            catch (Exception e)
            {
                Console.WriteLine("error StartAsync");
                Console.WriteLine(e);
                MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
    }
}
