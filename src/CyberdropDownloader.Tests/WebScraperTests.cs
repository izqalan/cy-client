using CyberdropDownloader.Core;
using CyberdropDownloader.Core.DataModels;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CyberdropDownloader.Tests
{

    public class WebScraperTests
    {
        WebScraper webScraper = new WebScraper("https://cyberdrop.me/a/mowoshqo");

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            await webScraper.InitializeAsync();
        }

        [Test, Order(1)]
        public void LoadPage()
        {
            bool result = webScraper.Successful;

            Assert.True(result);
        }

        [Test, Order(2)]
        public void FetchAlbumTitle()
        {
            string result = webScraper.Album.Title;

            Assert.IsTrue(string.Equals("tests", result));
        }

        [Test, Order(3)]
        public void FetchAlbumSize()
        {
            string result = webScraper.Album.Size;

            Assert.IsTrue(string.Equals("4.58 MB", result));
        }

        [Test, Order(4)]
        public void FetchAlbumFiles()
        {
            Queue<AlbumFile> result = webScraper.Album.Files;

            Assert.IsTrue(result.Count == 3);
        }
    }
}
