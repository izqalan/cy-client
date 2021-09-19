using CyberdropDownloader.Core.DataModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;

namespace CyberdropDownloader.Core
{
    public class AlbumDownloader
    {
        private HttpClient _downloadClient;
        private bool _authorized;
        private bool _running;

        private List<Chunk> _chunks;
        private Chunk _currentChunk;

        public AlbumDownloader(bool authorized)
        {
            // Setup and initialize HttpClient
            _downloadClient = new HttpClient(new SocketsHttpHandler()
            {
                AllowAutoRedirect = true,
                KeepAlivePingTimeout = Timeout.InfiniteTimeSpan
            });

            // Times out current download if it surpasses 5 minutes (might need tweaked)
            _downloadClient.Timeout = TimeSpan.FromMinutes(5);

            ServicePointManager.DefaultConnectionLimit = 100;
            ServicePointManager.Expect100Continue = false;

            _authorized = authorized;
        }

        public HttpClient DownloadClient { get => _downloadClient; }
        public bool Authorized { get => _authorized; set => _authorized = value; }
        public bool Running { get => _running; }

        public event EventHandler<string> FileDownloaded;
        public event EventHandler<string> FileDownloading;
        public event EventHandler<string> FileFailed;
        public event EventHandler<string> FileExists;
        public event EventHandler<int> ProgressChanged;

        public async Task DownloadAsync(Album album, string path, CancellationToken? cancellationToken, int chunkCount = 1)
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
                cancellationToken.Value.ThrowIfCancellationRequested();

                _running = true;

                AlbumFile file = album.Files.Peek();

                string filePath = $"{path}\\{NormalizeFileName(file.Name)}";

                try
                {
                    HttpResponseMessage response = await _downloadClient.GetAsync(file.Url, HttpCompletionOption.ResponseHeadersRead);

                    if (response.ReasonPhrase == "Bad Gateway")
                        throw new Exception(response.ReasonPhrase);

                    while (response.ReasonPhrase == "Moved Temporarily")
                        response = await _downloadClient.GetAsync(response.Headers.Location, HttpCompletionOption.ResponseHeadersRead);

                    if (File.Exists(filePath))
                    {
                        long fileLength = new FileInfo(filePath).Length;

                        if (fileLength == response.Content.Headers.ContentLength)
                        {
                            FileExists?.Invoke(this, album.Files.Dequeue().Name);
                            _running = false;
                            continue;
                        }
                        else File.Delete(filePath);
                    }

                    _chunks = new List<Chunk>();

                    for (int chunk = 0; chunk <= chunkCount; chunk++)
                    {
                        _chunks.Add(new Chunk()
                        {
                            Start = chunk * (response.Content.Headers.ContentLength.Value / chunkCount),
                            End = (chunk + 1) * (response.Content.Headers.ContentLength.Value / chunkCount)
                        });
                    }

                    _chunks.LastOrDefault().End = response.Content.Headers.ContentLength.Value;

                    FileStream fileStream = File.OpenWrite(filePath);

                    FileDownloading?.Invoke(this, file.Name);

                    while (_chunks.Count > 0)
                    {
                        cancellationToken.Value.ThrowIfCancellationRequested();

                        _currentChunk = _chunks[0];

                        try
                        {
                            using (HttpRequestMessage request = new HttpRequestMessage())
                            {
                                request.RequestUri = new Uri(file.Url);
                                request.Headers.Range = new RangeHeaderValue(_currentChunk.Start, _currentChunk.End);

                                using (HttpResponseMessage rangedResponse = await _downloadClient.SendAsync(request, cancellationToken.Value))
                                {
                                    fileStream.Seek(_currentChunk.Start, SeekOrigin.Begin);
                                    await fileStream.WriteAsync(await rangedResponse.Content.ReadAsByteArrayAsync(), cancellationToken.Value);
                                }
                            }
                        }
                        catch (Exception) { continue; }

                        _chunks.RemoveAt(0);
                        ProgressChanged?.Invoke(this, chunkCount - _chunks.Count);
                    }

                    await fileStream.DisposeAsync();
                    FileDownloaded?.Invoke(this, album.Files.Dequeue().Name);
                }
                catch (Exception ex) 
                {
                    string message = ex.Message;

                    FileFailed?.Invoke(this, album.Files.Dequeue().Name); 
                }

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
