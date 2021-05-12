using System;
using System.Collections.Generic;
using System.Linq;
using Downloader;
using HtmlAgilityPack;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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

        public string CheckIllegalChars(string s)
        {
            string regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            Regex r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
            s = r.Replace(s, "");
            s = s.Length == 0 ? "cy_album" : s; 
            return s;
        }

        public virtual async Task GetUrlsAndDownload(HtmlAgilityPack.HtmlDocument htmlDoc)
        {
            string title = GetTitle(htmlDoc);
            int i = 0;
            // check for illegal chars
            title = CheckIllegalChars(title);
            // scuffed af; having form controls in business logic
            listBox.Items.Insert(0, "Album: " + title);
            foreach (HtmlNode link in htmlDoc.DocumentNode.SelectNodes("//a[@class='image'][@href]"))
            {
                i++;
                string url = link.Attributes["href"].Value;
                itemName = CheckIllegalChars(link.Attributes["title"].Value);
                // scuffed af; having form controls in business logic
                listBox.Items.Insert(0, "Downloading item: " + itemName);
                // download here
                string filepath = String.Format(@"{0}\{1}\{2}", dest, title, itemName);
                Console.WriteLine(url);
                await downloader.DownloadFileAsync(url, filepath);
            }
            listBox.Items.Insert(0, "------Completed " + i + " Downloads------");
        }

        public async Task StartAsync()
        {
            var userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.149 Safari/537.36";
            try
            {
                for (int i = 0; i < url.Lines.Length; i++)
                {
                    HtmlWeb web = new HtmlWeb();
                    web.UserAgent = userAgent;
                    var htmlDoc = web.Load(url.Lines[i]);
                    await GetUrlsAndDownload(htmlDoc);
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
