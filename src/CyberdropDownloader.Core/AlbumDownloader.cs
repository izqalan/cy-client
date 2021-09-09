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
        private bool _authorized;
        private bool _running;

        public AlbumDownloader(bool authorized)
        {
            // Setup and initialize HttpClient
            _downloadClient = new HttpClient(new HttpClientHandler()
            {
                AllowAutoRedirect = true,
                MaxRequestContentBufferSize = 20
            });

            _downloadClient.Timeout = Timeout.InfiniteTimeSpan;
            _authorized = authorized;
        }

        public HttpClient DownloadClient { get => _downloadClient; }
        public bool Authorized { get => _authorized; set => _authorized = value; }
        public bool Running { get => _running; }

        public delegate void EventHandler(string fileName);

        public event EventHandler FileDownloaded;
        public event EventHandler FileDownloading;
        public event EventHandler FileFailed;
        public event EventHandler FileExists;

        public async Task DownloadAsync(Album album, string path, CancellationTokenSource cancellationTokenSource)
        {
            // if not authorized throw exception
            if (!_authorized)
                throw new Exception("Not authorized");

            path = NormalizePath($"{path}\\{album.Title}");

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            while (album.Files.Count > 0)
            {
                cancellationTokenSource.Token.ThrowIfCancellationRequested();
                   
                _running = true;

                AlbumFile file = album.Files.Peek();
                string filePath = $"{path}\\{NormalizeFileName(file.Name)}";

                if (File.Exists(filePath))
                {
                    FileExists.Invoke(album.Files.Dequeue().Name);
                    _running = false;
                    continue;
                }

                try
                {
                    FileDownloading.Invoke(file.Name);

                    // Causing hang
                    HttpResponseMessage clientResponse = DownloadClient.GetAsync(file.Url).Result;

                    // Keeps redirecting until it reaches the actual file
                    while (clientResponse.ReasonPhrase == "Moved Temporarily")
                        clientResponse = DownloadClient.GetAsync(clientResponse.Headers.Location).Result;

                    await using (Stream fileStream = File.Create(filePath))
                        await clientResponse.Content.CopyToAsync(fileStream);

                    clientResponse.Dispose();
                    FileDownloaded.Invoke(album.Files.Dequeue().Name);
                }
                catch (Exception)
                {
                    File.Delete(filePath);
                    FileFailed.Invoke(file.Name);
                }

                _running = false;
            }
        }

        public void CancelDownload() 
        {
            _downloadClient.CancelPendingRequests();
            _running = false;
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
