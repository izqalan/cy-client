using CyberdropDownloader.Core.Enums;
using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace CyberdropDownloader.Core
{
    public static class FileDownloader
    {
        private static WebClient _downloadClient;

        public static WebClient DownloadClient { get => _downloadClient; }

        public static void Initialize()
        {
            _downloadClient = new WebClient();
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
                try
                {
                    await DownloadClient.DownloadFileTaskAsync(new Uri(url), filePath);
                    response = DownloadResponse.Downloaded;
                }
                catch (Exception)
                {
                    File.Delete(filePath);
                    response = DownloadResponse.Failed;
                }
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
