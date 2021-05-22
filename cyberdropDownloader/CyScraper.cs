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

        public bool isThereSpace(string disk, int tagetFileSize)
        {
            DriveInfo drive = new DriveInfo(disk);
            if (drive.IsReady)
            {
                return drive.AvailableFreeSpace > tagetFileSize;
            }
            return false;
        }

        public string CheckIllegalChars(string s)
        {
            string regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            Regex r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
            s = r.Replace(s, "");
            s = s.Length == 0 ? "cy_album" : Mono.Web.HttpUtility.HtmlDecode(s); ;
            return s;
        }

        public virtual async Task GetUrlsAndDownload(HtmlAgilityPack.HtmlDocument htmlDoc)
        {
            try
            {
                string title = GetTitle(htmlDoc);
                int i = 0;
                int convertToByteValue = 1;
                title = CheckIllegalChars(title);

                // hacky way of getting album total file size from album
                // initially I wanted to use the hidden album size which shown in bytes but htmlagility doesn't seem to pick up that 
                var totalAlbumSizeNode = htmlDoc.DocumentNode.SelectNodes("//div/p[@class='title']");
                string totalAlbumSize = totalAlbumSizeNode[1].InnerHtml;

                if (totalAlbumSize != null)
                {
                    //https://stackoverflow.com/questions/44905/c-sharp-switch-statement-limitations-why
                    if (totalAlbumSize.Contains("KB"))
                    {
                        convertToByteValue = 1024;
                    }
                    else if (totalAlbumSize.Contains("MB"))
                    {
                        convertToByteValue = 1048576;
                    }
                    else if (totalAlbumSize.Contains("GB"))
                    {
                        convertToByteValue = 1073741824;
                    }

                    int totalAlbumSizeInt = Int32.Parse(Regex.Replace(totalAlbumSize, "[^0-9]+", string.Empty)) * convertToByteValue;

                    if (!isThereSpace(dest, totalAlbumSizeInt))
                    {
                        throw new Exception("Not enough space");
                    }
                }

                listBox.Items.Insert(0, "Album: " + title);
                foreach (HtmlNode link in htmlDoc.DocumentNode.SelectNodes("//a[@class='image'][@href]"))
                {
                    i++;
                    string url = Mono.Web.HttpUtility.HtmlDecode(link.Attributes["href"].Value);
                    itemName = CheckIllegalChars(link.Attributes["title"].Value);
                    string filepath = String.Format(@"{0}\{1}\{2}", dest, title, itemName);
                    if (!File.Exists(filepath))
                    {
                        // download here
                        await downloader.DownloadFileTaskAsync(url, filepath);
                    }
                    else
                    {
                        listBox.Items.Insert(0, "[File Existed] [SKIP]: " + itemName);
                    }
                }
                listBox.Items.Insert(0, "------Completed " + i + " Downloads------");
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public async Task StartAsync()
        {
            
            try
            {
                for (int i = 0; i < url.Lines.Length; i++)
                {
                    HtmlWeb web = new HtmlWeb();
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
