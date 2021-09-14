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
            _downloadClient = new HttpClient(new SocketsHttpHandler()
            {
                AllowAutoRedirect = true,
                KeepAlivePingTimeout = Timeout.InfiniteTimeSpan
            });

            // Times out current download if it surpasses 30 minutes (might need tweaked)
            _downloadClient.Timeout = TimeSpan.FromMinutes(30);

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
            // If not authorized throw exception
            if (!_authorized)
                throw new Exception("Not authorized");

            // Normalize the download path
            path = NormalizePath($"{path}\\{album.Title}");

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            // Iterate through all album files until there are none left
            while (album.Files.Count > 0)
            {
                cancellationTokenSource.Token.ThrowIfCancellationRequested();
                
                _running = true;

                AlbumFile file = album.Files.Peek();

                string filePath = $"{path}\\{NormalizeFileName(file.Name)}";

                try
                {
                    FileDownloading.Invoke(file.Name);

                    HttpResponseMessage response = await _downloadClient.GetAsync(file.Url, HttpCompletionOption.ResponseHeadersRead);

                    while (response.ReasonPhrase == "Moved Temporarily")
                        response = await _downloadClient.GetAsync(response.Headers.Location, HttpCompletionOption.ResponseHeadersRead);

                    using (Stream dataStream = await response.Content.ReadAsStreamAsync(cancellationTokenSource.Token))
                    {
                        if (File.Exists(filePath))
                        {
                            long fileLength = new FileInfo(filePath).Length;

                            if (fileLength == response.Content.Headers.ContentLength)
                            {
                                FileExists.Invoke(album.Files.Dequeue().Name);
                                _running = false;
                                continue;
                            }
                            else File.Delete(filePath);
                        }

                        using (Stream fileStream = File.Open(filePath, FileMode.Create, FileAccess.Write))
                        {
                            await dataStream.CopyToAsync(fileStream, cancellationTokenSource.Token);
                        }

                        response.Dispose();

                        FileDownloaded.Invoke(album.Files.Dequeue().Name);
                    }
                }
                catch (Exception) { FileFailed.Invoke(file.Name); }

                _running = false;
            }
        }

        public void CancelDownload() 
        {
            // Cancels pending get requests
            _downloadClient.CancelPendingRequests();

            // Indicates that no file i currently being downloaded
            _running = false;
        } 

        private static string NormalizeFileName(string data)
        {
            // Create string containing all invalid file name characters
            string regexSearch = new string(Path.GetInvalidFileNameChars());

            // Create regex from regexSearch
            Regex regexResult = new Regex($"[{Regex.Escape(regexSearch)}]");

            // Replace invalid character with empty string
            data = regexResult.Replace(data, string.Empty);

            // If the file name was completely erased, then replace it with cy_file
            if (data.Length == 0)
                data = "cy_file";

            return data;
        }

        private static string NormalizePath(string data)
        {
            // Create string containing all invalid path characters
            string regexSearch = new string(Path.GetInvalidPathChars());

            // Create regex from regexSearch
            Regex regexResult = new Regex($"[{Regex.Escape(regexSearch)}]");

            // Replace invalid characters with empty string
            data = regexResult.Replace(data, string.Empty);

            // Odd bug with folders that end with '.' not being found.
            data = data.Trim('.');

            // If the album name was completely erased, then replace it with cy_album
            if (data.Length == 0)
                data = "cy_album";

            return data;
        }
    }
}
