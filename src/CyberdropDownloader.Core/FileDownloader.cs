using CyberdropDownloader.Core.Enums;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using System.Net;

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

        public static async Task<DownloadResponse> DownloadFile(string url, string path, string fileName)
        {
            string filePath = $"{path}\\{fileName}";

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            if (File.Exists(filePath))
            {
                return DownloadResponse.FileExists;
            }

            // Check if enough space TODO


            DownloadResponse response = DownloadResponse.None;

            await Task.Run(async () =>
            {
                HttpResponseMessage clientResponse = await DownloadClient.GetAsync(url);

                try
                {
                    while(clientResponse.ReasonPhrase == "Moved Temporarily")
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

        private static bool isThereSpace(string disk, int tagetFileSize)
        {
            DriveInfo drive = new DriveInfo(disk);
            if (drive.IsReady)
            {
                return drive.AvailableFreeSpace > tagetFileSize;
            }
            return false;
        }
    }
}
