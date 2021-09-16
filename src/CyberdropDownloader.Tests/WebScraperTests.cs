using CyberdropDownloader.Core;
using CyberdropDownloader.Core.DataModels;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CyberdropDownloader.Tests
{

    public class WebScraperTests
    {
        private WebScraper _webScraper;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            _webScraper = new WebScraper();
            await _webScraper.LoadAlbumAsync("https://cyberdrop.me/a/0ezUfnys");
        }

        [Test, Order(1)]
        public void LoadPage()
        {
            bool result = _webScraper.Successful;

            Assert.True(result);
        }

        [Test, Order(2)]
        public void FetchAlbumTitle()
        {
            string result = _webScraper.Album.Title;

            Assert.IsTrue(string.Equals("tests", result));
        }

        [Test, Order(3)]
        public void FetchAlbumSize()
        {
            double result = _webScraper.Album.Size;

            Assert.IsTrue(Equals("4802478", result.ToString()));
        }

        [Test, Order(4)]
        public void FetchAlbumFiles()
        {
            Queue<AlbumFile> result = _webScraper.Album.Files;

            Assert.IsTrue(result.Count == 12);
        }
    }
}
