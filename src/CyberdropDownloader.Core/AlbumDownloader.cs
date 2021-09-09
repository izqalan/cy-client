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
                AllowAutoRedirect = true
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
                // If canceled through ui, stop iterating through album files.
                cancellationTokenSource.Token.ThrowIfCancellationRequested();
                
                // Indicate that a file is being downloaded
                _running = true;

                // Return the first object of the queue without removing it
                AlbumFile file = album.Files.Peek();
                // Normalize the filename and create file path
                string filePath = $"{path}\\{NormalizeFileName(file.Name)}";

                // If the file exists, then skip over it
                if (File.Exists(filePath))
                {
                    // Invoke file exists event to notify the ui, then remove the file from the queue.
                    FileExists.Invoke(album.Files.Dequeue().Name);
                    _running = false;
                    continue;
                }

                try
                {
                    // Invokes file downloading event to notify the ui
                    FileDownloading.Invoke(file.Name);

                    // Causing hang, but is solved with a timeout. Most downloads don't actually timeout they just take a really long time and are better off restarted.
                    HttpResponseMessage clientResponse = DownloadClient.GetAsync(file.Url).Result;

                    // Keeps redirecting until it reaches the actual file. Should only be once or twice at max, but will keep redirecting until it hits the final destination.
                    while (clientResponse.ReasonPhrase == "Moved Temporarily")
                        clientResponse = DownloadClient.GetAsync(clientResponse.Headers.Location).Result;

                    // Creates a file stream and copies file from memory into local file
                    await using (Stream fileStream = File.Create(filePath))
                        await clientResponse.Content.CopyToAsync(fileStream);

                    // Disposes of file to free up resources
                    clientResponse.Dispose();

                    // Invokes file downloaded event to notfy the ui
                    FileDownloaded.Invoke(album.Files.Dequeue().Name);
                }
                catch (Exception)
                {
                    // Deletes failed file to prevent corruption
                    File.Delete(filePath);

                    // Invokes failed file event to notify the ui
                    FileFailed.Invoke(file.Name);
                }

                // Indicates that no file is currently being downloaded
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
