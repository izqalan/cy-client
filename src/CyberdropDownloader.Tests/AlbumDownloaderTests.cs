using CyberdropDownloader.Core;
using NUnit.Framework;

namespace CyberdropDownloader.Tests
{
    public class AlbumDownloaderTests
    {
        private AlbumDownloader _downloader;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _downloader = new AlbumDownloader();
        }

        [Test, Order(1)]
        public void Initialized()
        {
            Assert.IsNotNull(_downloader.DownloadClient);
        }
    }
}
