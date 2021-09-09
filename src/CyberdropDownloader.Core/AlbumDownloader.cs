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
        private static HttpClient _downloadClient;
        private bool _authorized;
        private bool _running;
        private bool _isCanceled;

        private CancellationTokenSource _cancellationTokenSource;

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
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public static HttpClient DownloadClient { get => _downloadClient; }
        public bool Authorized { get => _authorized; set => _authorized = value; }
        public bool Running { get => _running; }
        public bool IsCanceled { get => _isCanceled; }

        public delegate void EventHandler(string fileName);

        public event EventHandler FileDownloaded;
        public event EventHandler FileDownloading;
        public event EventHandler FileFailed;
        public event EventHandler FileExists;

        public async Task DownloadAsync(Album album, string path)
        {
            // if not authorized throw exception
            if (!_authorized)
                throw new Exception("Not authorized");

            try
            {
                if (_cancellationTokenSource == null)
                    _cancellationTokenSource = new CancellationTokenSource();

                _isCanceled = false;

                path = NormalizePath($"{path}\\{album.Title}");

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                await Task.Run(async () =>
                {
                    _running = true;

                    while (album.Files.Count > 0 && _running)
                    {
                        AlbumFile file = album.Files.Peek();
                        string filePath = $"{path}\\{NormalizeFileName(file.Name)}";

                        if (File.Exists(filePath))
                        {
                            FileExists.Invoke(album.Files.Dequeue().Name);
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
                    }
                }, _cancellationTokenSource.Token);
            }
            catch (Exception) 
            { 
                _isCanceled = true;
                _cancellationTokenSource = null;
            }
            _running = false;
        }

        public void CancelDownload() 
        {
            _cancellationTokenSource.Cancel();
            _downloadClient.CancelPendingRequests();
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
