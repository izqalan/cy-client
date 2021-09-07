using CyberdropDownloader.Core.DataModels;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CyberdropDownloader.Core
{
    public class WebScraper
    {
        private string _url;
        private HtmlDocument _htmlDocument;

        private Album _album;
        private bool _successful;

        public Album Album { get => _album; }
        public bool Successful { get => _successful; }

        public async Task LoadAlbumAsync(string url)
        {
            _url = url;
            _successful = false;

            await Task.Run(async () =>
            {
                await LoadHtmlDocumenteAsync();

                if (_htmlDocument != null)
                {
                    try
                    {
                        // Insatiate new album with title, size, and files.
                        _album = new Album(await FetchAlbumTitleAsync(), await FetchAlbumSizeAsync(), await FetchAlbumFilesAsync());
                        _successful = true;
                    }
                    catch
                    {
                        _successful = false;
                    }
                }
            });
        }

        #region Load Album
        private async Task<string> FetchAlbumTitleAsync()
        {
            string title = null;

            await Task.Run(() =>
            {
                title = _htmlDocument.DocumentNode.SelectNodes("//div/h1[@id='title']").First().Attributes["title"].Value;
            });

            return title;
        }

        private async Task<string> FetchAlbumSizeAsync()
        {
            string size = null;

            await Task.Run(() =>
            {
                size = _htmlDocument.DocumentNode.SelectNodes("//div/p[@class='title']")[1].InnerHtml;
            });

            return size;
        }

        private async Task<Queue<AlbumFile>> FetchAlbumFilesAsync()
        {
            Queue<AlbumFile> urls = new Queue<AlbumFile>();

            await Task.Run(() =>
            {
                HtmlNodeCollection nodes = _htmlDocument.DocumentNode.SelectNodes("//a[@class='image'][@href]");

                foreach (HtmlNode link in nodes)
                {
                    urls.Enqueue(new AlbumFile()
                    {
                        Name = link.Attributes["title"].Value,
                        Url = link.Attributes["href"].Value
                    });
                }

            });

            return urls;
        }

        private async Task LoadHtmlDocumenteAsync()
        {
            try
            {
                _htmlDocument = await new HtmlWeb().LoadFromWebAsync(_url);
            }
            catch (Exception)
            {
                _htmlDocument = null!;
            }
        }
        #endregion
    }
}
