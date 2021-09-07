using CyberdropDownloader.Core.DataModels;
using System;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace CyberdropDownloader.Core
{
    public class AlbumDownloader
    {
        private HttpClient _downloadClient;

        public AlbumDownloader()
        {
            // Setup and initialize HttpClient
            _downloadClient = new HttpClient(new HttpClientHandler()
            {
                AllowAutoRedirect = true,
                MaxRequestContentBufferSize = 20
            });

            _downloadClient.Timeout = Timeout.InfiniteTimeSpan;
        }

        public HttpClient DownloadClient { get => _downloadClient; }

        public delegate void EventHandler(string fileName);

        public event EventHandler FileDownloaded;
        public event EventHandler FileDownloading;
        public event EventHandler FileFailed;
        public event EventHandler FileExists;

        public async Task DownloadAsync(Album album, string path)
        {
            path = NormalizePath($"{path}\\{album.Title}");

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            while (album.Files.Count > 0)
            {
                AlbumFile file = album.Files.Peek();
                string filePath = $"{path}\\{NormalizeFileName(file.Name)}";

                if (File.Exists(filePath))
                {
                    FileExists.Invoke(album.Files.Dequeue().Name);
                    continue;
                }

                await Task.Run(async () =>
                {
                    try
                    {
                        DownloadClient.CancelPendingRequests();

                        HttpResponseMessage clientResponse = await DownloadClient.GetAsync(file.Url);

                        FileDownloading.Invoke(file.Name);

                        // Keeps redirecting until it reaches the actual file
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
                        FileDownloaded.Invoke(album.Files.Dequeue().Name);
                    }
                    catch (Exception)
                    {
                        File.Delete(filePath);
                        FileFailed.Invoke(file.Name);
                    }
                });
            }
        }

        private static string NormalizeFileName(string data)
        {
            string regexSearch = new string(Path.GetInvalidFileNameChars().ToString());

            Regex regexResult = new Regex($"[{Regex.Escape(regexSearch)}]");

            data = regexResult.Replace(data, string.Empty);

            if (data.Length == 0)
                data = "cy_album";

            return data;
        }

        private static string NormalizePath(string data)
        {
            string regexSearch = new string(Path.GetInvalidPathChars());

            Regex regexResult = new Regex($"[{Regex.Escape(regexSearch)}]");

            data = regexResult.Replace(data, string.Empty);

            return data;
        }

        /**
        private static bool EnoughSpaceCheck(string disk, double albumSize)
        {
            DriveInfo drive = new DriveInfo(disk);

            if (!drive.IsReady)
                return false;

            return drive.AvailableFreeSpace > albumSize;
        }**/
    }
}
