using CyberdropDownloader.Core;
using NUnit.Framework;

namespace CyberdropDownloader.Tests
{
    public class FileDownloaderTests
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            FileDownloader.Initialize();
        }

        [Test, Order(1)]
        public void Initialized()
        {
            Assert.IsNotNull(FileDownloader.DownloadClient);
        }

        [Test, Order(1)]
        public void ConvertAlbumSizeToByte()
        {
            long response = FileDownloader.ConvertAlbumSizeToByte("10 MB");

            Assert.IsTrue(response == 10485760);
        }
    }
}
