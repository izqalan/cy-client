using CyberdropDownloader.Core.DataModels;
using HtmlAgilityPack;
using System;
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

        private Album _album;

        public WebScraper(string url)
        {
            _url = url;
        }

        public Album Album { get => _album; }

        public async Task Initialize()
        {
            await Task.Run(async () =>
            {
                _album = new Album(await FetchAlbumTitleAsync(), await FetchAlbumSizeAsync(), await FetchAlbumFilesAsync());
            });
        }

        public async Task<string> FetchAlbumTitleAsync()
        {
            string title = "";

            await Task.Run(() =>
            {
                title = _htmlDocument.DocumentNode.SelectNodes("//div/h1[@id='title']").First().Attributes["title"].Value;
            });

            return title;
        }

        public async Task<string> FetchAlbumSizeAsync()
        {
            string size = "";

            await Task.Run(() =>
            {
                size = _htmlDocument.DocumentNode.SelectNodes("//div/p[@class='title']")[1].InnerHtml;
            });

            return size;
        }

        public async Task<Queue<AlbumFile>> FetchAlbumFilesAsync()
        {
            Queue<AlbumFile> urls = new Queue<AlbumFile>();

            await Task.Run(() =>
            {
                HtmlNodeCollection nodes = _htmlDocument.DocumentNode.SelectNodes("//a[@class='image'][@href]");

                foreach (HtmlNode link in nodes)
                {
                    urls.Enqueue(new AlbumFile()
                    {
                        Name = ValidatePathAndFileName(link.Attributes["title"].Value),
                        Url = link.Attributes["href"].Value
                    });
                }

            });

            return urls;
        }

        public async Task<bool> LoadHtmlDocumenteAsync()
        {
            try
            {
                _htmlDocument = await new HtmlWeb().LoadFromWebAsync(_url);
            }
            catch (Exception)
            {
                _htmlDocument = null!;
            }

            if (_htmlDocument == null)
            {
                return false;
            }

            return true;
        }

        public string ValidatePathAndFileName(string data)
        {

            string regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());

            Regex regexResult = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));

            data = regexResult.Replace(data, "");

            data = data.Length == 0 ? "cy_album" : HttpUtility.HtmlDecode(data);

            return data;
        }
    }
}
