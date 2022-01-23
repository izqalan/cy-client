using CyberdropDownloader.Core.DataModels;
using CyberdropDownloader.Core.Exceptions;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CyberdropDownloader.Core
{
    public class WebScraper
    {
        private Album _album;

        public Album Album => _album;

        public async Task LoadAlbumAsync(string url)
        {
            await Task.Run(async () =>
            {
                // Load webpage
                HtmlDocument htmlDocument = await new HtmlWeb().LoadFromWebAsync(url);

                if(htmlDocument != null)
                {
                    (string title, string size, Queue<AlbumFile> files) albumData = FetchAlbumData(htmlDocument);

                    _album = new Album(albumData.title, albumData.size, albumData.files);
                }
            });
        }

        #region Load Album
        private (string title, string size, Queue<AlbumFile> files) FetchAlbumData(HtmlDocument htmlDocument)
        {
            return (FetchAlbumTitle(htmlDocument), FetchAlbumSize(htmlDocument), FetchAlbumFiles(htmlDocument));
        }

        private string FetchAlbumTitle(HtmlDocument htmlDocument)
        {
            string title;

            try
            {
                title = htmlDocument.DocumentNode.SelectNodes("//div/h1[@id='title']").First().Attributes["title"].Value;
            }
            catch(Exception ex)
            {
                switch(ex)
                {
                    case ArgumentNullException or NullReferenceException:
                        throw new NullAlbumTitleException();

                    default:
                        throw new Exception(ex.Message);
                }
            }

            return title;
        }

        private string FetchAlbumSize(HtmlDocument htmlDocument)
        {
            string size;

            try
            {
                size = htmlDocument.DocumentNode.SelectNodes("//div/p[@class='title']")[1].InnerHtml;
            }
            catch(Exception ex)
            {
                switch(ex)
                {
                    case ArgumentNullException or NullReferenceException:
                        throw new NullAlbumSizeException();

                    default:
                        throw new Exception(ex.Message);
                }
            }

            return size;
        }

        private Queue<AlbumFile> FetchAlbumFiles(HtmlDocument htmlDocument)
        {
            Queue<AlbumFile> urls = new Queue<AlbumFile>();

            try
            {
                HtmlNodeCollection files = htmlDocument.DocumentNode.SelectNodes("//a[@class='image'][@href]");
                
                foreach(HtmlNode link in files)
                {
                    urls.Enqueue(new AlbumFile()
                    {
                        Name = link.Attributes["title"].Value,
                        Url = link.Attributes["href"].Value
                    });
                }
            }
            catch(Exception ex)
            {
                switch(ex)
                {
                    case ArgumentNullException or NullReferenceException:
                        throw new NullAlbumFilesException();

                    default:
                        throw new Exception(ex.Message);
                }
            }

            return urls;
        }
        #endregion
    }
}
