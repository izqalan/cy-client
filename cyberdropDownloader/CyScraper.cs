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

        private string Title(HtmlAgilityPack.HtmlDocument htmlDoc)
        {
            var title = htmlDoc.DocumentNode.SelectNodes("//div/h1[@id='title']").First().Attributes["title"].Value;
            return title;
        }

        public async void StartAsync()
        {
            try
            {
                for (int i = 0; i < url.Lines.Length; i++)
                {
                    HtmlWeb web = new HtmlWeb();
                    var htmlDoc = web.Load(url.Lines[i]);
                    string title = this.Title(htmlDoc);
                    listBox.Items.Insert(0, "Album: " + title);
                    //listBox.Items.Add("Downloading album: " + title);
                    foreach (HtmlNode link in htmlDoc.DocumentNode.SelectNodes("//a[@class='image'][@href]"))
                    {
                        string url = link.Attributes["href"].Value;
                        itemName = link.Attributes["title"].Value;
                        listBox.Items.Insert(0, "Downloading item: " + itemName);
                        //listBox.Items.Add("Downloading item: " + itemName);
                        // download here
                        string filepath = String.Format(@"{0}\{1}\{2}", dest, title, itemName);
                        await downloader.DownloadFileAsync(url, filepath);
                    }
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
