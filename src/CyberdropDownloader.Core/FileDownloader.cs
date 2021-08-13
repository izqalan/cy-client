using CyberdropDownloader.Core.Enums;
using System;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace CyberdropDownloader.Core
{
    public static class FileDownloader
    {
        private static HttpClient _downloadClient;

        public static HttpClient DownloadClient { get => _downloadClient; }

        public static void Initialize()
        {
            _downloadClient = new HttpClient(new HttpClientHandler()
            {
                AllowAutoRedirect = true,
                MaxRequestContentBufferSize = 20
            });

            _downloadClient.Timeout = Timeout.InfiniteTimeSpan;
        }

        public static async Task<DownloadResponse> DownloadFile(string url, string path, string fileName, string albumSize)
        {
            string filePath = $"{path}\\{fileName}";

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            if (File.Exists(filePath))
                return DownloadResponse.FileExists;
            
            if (!EnoughSpaceCheck(path, ConvertAlbumSizeToByte(albumSize)))
                return DownloadResponse.NotEnoughSpace;

            DownloadResponse response = DownloadResponse.None;

            await Task.Run(async () =>
            {
                HttpResponseMessage clientResponse = await DownloadClient.GetAsync(url);

                try
                {
                    while (clientResponse.ReasonPhrase == "Moved Temporarily")
                    {
                        clientResponse = await DownloadClient.GetAsync(clientResponse.Headers.Location);
                    }

                    await using (Stream stream = await clientResponse.Content.ReadAsStreamAsync())
                    {
                        await using (Stream fileStream = File.Create(filePath))
                        {
                            stream.Seek(0, SeekOrigin.Begin);
                            await stream.CopyToAsync(fileStream);
                        }
                    }
                }
                catch (Exception)
                {
                    File.Delete(filePath);
                    response = DownloadResponse.Failed;
                }

                clientResponse.Dispose();
                response = DownloadResponse.Downloaded;
            });

            return response;
        }

        private static long ConvertAlbumSizeToByte(string albumSize)
        {
            long byteValue = 0;

            if (albumSize.Contains("KB"))
                byteValue = 1024;
            else if (albumSize.Contains("MB"))
                byteValue = 1048576;
            else if (albumSize.Contains("GB"))
                byteValue = 1073741824;
            else if (albumSize.Contains("TB"))
                byteValue = 1099511627776;

            return int.Parse(Regex.Replace(albumSize, "[^0-9]+", string.Empty)) * byteValue;
        }

        private static bool EnoughSpaceCheck(string disk, long albumSize)
        {
            DriveInfo drive = new DriveInfo(disk);

            if (!drive.IsReady)
                return false;

            return drive.AvailableFreeSpace > albumSize;
        }
    }
}
