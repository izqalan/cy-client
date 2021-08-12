using CyberdropDownloader.Core.DataModels;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace CyberdropDownloader.Core
{
    public class WebScraper
    {
        private string _url;
        private HtmlDocument _htmlDocument;

        public WebScraper(string url)
        {
            _url = url;

            _htmlDocument = Task.Run(LoadHtmlDocumenteAsync).Result;
        }

        public string FetchAlbumTitle()
        {
            return _htmlDocument.DocumentNode.SelectNodes("//div/h1[@id='title']").First().Attributes["title"].Value;
        }

        public Queue<AlbumFile> FetchAlbumFiles()
        {
            Queue<AlbumFile> urls = new Queue<AlbumFile>();

            HtmlNodeCollection nodes = _htmlDocument.DocumentNode.SelectNodes("//a[@class='image'][@href]");

            foreach (HtmlNode link in nodes)
            {
                urls.Enqueue(new AlbumFile()
                {
                    Name = ValidatePathAndFileName(link.Attributes["title"].Value),
                    Url = link.Attributes["href"].Value
                });
            }

            return urls;
        }

        private async Task<HtmlDocument> LoadHtmlDocumenteAsync()
        {
            return await new HtmlWeb().LoadFromWebAsync(_url);
        }


        public string ValidatePathAndFileName(string data)
        {
            string regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            Regex r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
            data = r.Replace(data, "");
            data = data.Length == 0 ? "cy_album" : HttpUtility.HtmlDecode(data); ;
            return data;
        }
    }
}
