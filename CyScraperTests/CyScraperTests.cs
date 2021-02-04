using cyberdropDownloader;
using HtmlAgilityPack;
using System.Collections.Generic;
using Xunit;

namespace CyClientTests
{
    public class CyScraperTests
    {
        CyScraper scraper = new CyScraper();
        HtmlWeb web = new HtmlWeb();

        [Fact]
        public void ConnectionEstablished()
        {
            bool connection =  false;
            try
            {
                using (var client = new System.Net.WebClient())
                {
                    using (client.OpenRead("https://cyberdrop.me/a/mowoshqo"))
                    {
                        connection = true;
                    }
                }
            } catch
            {
                connection = false;
            }
            Assert.True(connection);
        }
        [Fact]
        public void CheckIllegalChars()
        {
            string illegals = "\"M\"\\a/ry/ h**ad:>> a\\/:*?\"| li*tt|le|| la\"mb.?";
            illegals = scraper.CheckIllegalChars(illegals);
            Assert.Equal("Mary had a little lamb.", illegals);
        }

        [Fact]
        public void ScrapeKnownAlbumTitle()
        {
            var htmlDoc = web.Load("https://cyberdrop.me/a/mowoshqo");
            string title = scraper.GetTitle(htmlDoc);
            Assert.Equal("tests", title);
        }

        [Fact]
        public void RetrieveAlbumItems()
        {
            var htmlDoc = web.Load("https://cyberdrop.me/a/mowoshqo");
            List<string> urls = scraper.GetAlbumUrls(htmlDoc);
            Assert.NotEmpty(urls);
            Assert.Equal(3, urls.Count);
        }
        
    }
}
