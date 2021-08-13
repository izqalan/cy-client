using CyberdropDownloader.Core;
using CyberdropDownloader.Core.DataModels;
using System.Collections.Generic;
using Xunit;

namespace CyberdropDownloader.Tests
{
    public class WebScraperTests
    {
        WebScraper webScraper = new WebScraper("https://cyberdrop.me/a/mowoshqo");

        [Fact]
        public async void LoadPage()
        {
            bool result = await webScraper.LoadHtmlDocumenteAsync();

            Assert.True(result);
        }

        [Fact]
        public async void FetchAlbumTitle()
        {
            string result = await webScraper.FetchAlbumTitleAsync();

            Assert.Equal("tests", result);
        }

        [Fact]
        public async void FetchAlbumSize()
        {
            string result = await webScraper.FetchAlbumSizeAsync();

            Assert.Equal("4.58 MB", result);
        }

        [Fact]
        public async void FetchAlbumFiles()
        {
            Queue<AlbumFile> result = await webScraper.FetchAlbumFilesAsync();

            Assert.Equal(3, result.Count);
        }
    }
}
