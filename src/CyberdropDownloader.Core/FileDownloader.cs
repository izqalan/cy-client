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
            path = NormalizePath(path);
            string filePath = $"{path}\\{NormalizeFileName(fileName)}";
            
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            if (File.Exists(filePath))
                return DownloadResponse.FileExists;

            if (!EnoughSpaceCheck(path, ConvertAlbumSizeToByte(albumSize)))
                return DownloadResponse.NotEnoughSpace;

            DownloadResponse response = DownloadResponse.None;

            await Task.Run(async () =>
            {
                try
                {
                    HttpResponseMessage clientResponse = await DownloadClient.GetAsync(url);

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

                            await fileStream.DisposeAsync();
                        }

                        await stream.DisposeAsync();
                    }

                    clientResponse.Dispose();
                }
                catch (Exception)
                {
                    File.Delete(filePath);
                    response = DownloadResponse.Failed;
                }

                if(response != DownloadResponse.Failed)
                    response = DownloadResponse.Downloaded;
            });

            return response;
        }

        public static double ConvertAlbumSizeToByte(string albumSize)
        {
            decimal byteValue = 0;

            if (albumSize.Contains("KB"))
                byteValue = 1024;
            else if (albumSize.Contains("MB"))
                byteValue = 1048576;
            else if (albumSize.Contains("GB"))
                byteValue = 1073741824;
            else if (albumSize.Contains("TB"))
                byteValue = 1099511627776;

            decimal regexValue = Convert.ToDecimal(Regex.Replace(albumSize, @"[a-zA-Z]+", string.Empty));

            double roundedValue = Convert.ToDouble(decimal.Round(regexValue * byteValue, 0)); 

            return roundedValue;
        }

        public static string NormalizeFileName(string data)
        {
            string regexSearch = new string(Path.GetInvalidFileNameChars().ToString());

            Regex regexResult = new Regex($"[{Regex.Escape(regexSearch)}]");

            data = regexResult.Replace(data, string.Empty);

            if (data.Length == 0)
                data = "cy_album";

            return data;
        }

        public static string NormalizePath(string data)
        {
            string regexSearch = new string(Path.GetInvalidPathChars());

            Regex regexResult = new Regex($"[{Regex.Escape(regexSearch)}]");

            data = regexResult.Replace(data, string.Empty);

            return data;
        }

        private static bool EnoughSpaceCheck(string disk, double albumSize)
        {
            DriveInfo drive = new DriveInfo(disk);
             
            if (!drive.IsReady)
                return false;

            return drive.AvailableFreeSpace > albumSize;
        }
    }
}
