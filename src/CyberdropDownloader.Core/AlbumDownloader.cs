using CyberdropDownloader.Core.DataModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using CyberdropDownloader.Core.Exceptions;

namespace CyberdropDownloader.Core
{
	public class AlbumDownloader
	{
		private readonly HttpClient _downloadClient;
		private bool _authorized;
		private bool _running;
		private List<string> knownServers = new List<string>{"fs-01", "fs-02", "fs-03", "fs-04", "fs-05", "fs-06"};
		private string workingServer = null;

		public AlbumDownloader(bool authorized)
		{
			HttpClientHandler clientHandler = new HttpClientHandler();
			clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
			clientHandler.AllowAutoRedirect = true;
			
			// Setup and initialize HttpClient
			_downloadClient = new HttpClient(clientHandler);

			// Times out current download if it surpasses 5 minutes (might need tweaked)
			_downloadClient.Timeout = TimeSpan.FromMinutes(5);

			ServicePointManager.DefaultConnectionLimit = 100;
			ServicePointManager.Expect100Continue = false;

			_authorized = authorized;
		}

		public HttpClient DownloadClient => _downloadClient;
		public bool Authorized { get => _authorized; set => _authorized = value; }
		public bool Running => _running;

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

				// Checking if we can avoid useless server call 
				// in case we already know this precise server is down based on previous album downloads
				if (workingServer != null && workingServer != GetServerNameFromURL(file.Url))
				{
					file.Url = ReplaceServerName(file.Url, workingServer);
				}
				try
				{
					HttpResponseMessage response = await CheckAvailability(file);
					// Next block will be executed only if we didn't have name of the working server from previous albums
					if (response == null)
					{
						// If we couldn't get response from the server, then this server is down and should be removed from list
						knownServers.Remove(GetServerNameFromURL(file.Url));
						// Iterating through all known server names and trying to make a request.
						// If a precise server won't make a valid response, then its name will be deleted from server list
						foreach (string serverName in knownServers)
						{
							file.Url = ReplaceServerName(file.Url, serverName);
							response = await CheckAvailability(file);
							if (response != null)
							{
								workingServer = serverName;
								break;
							}
							else
							{
								knownServers.Remove(serverName);
								if (!knownServers.Any())
								{
									throw new AllServersAreUnreachable();
								}
							}
						}
					}
					// If response from server is good, then we should use this server for furure calls
					else
					{
						workingServer = GetServerNameFromURL(file.Url);
					}

					if (File.Exists(filePath))
					{
						byte[] fileData = await File.ReadAllBytesAsync(filePath);

						// if it's the same size, then continue to the next file, otherwise delete it.
						if (fileData.Length >= response.Content.Headers.ContentLength)
						{
							FileExists?.Invoke(this, album.Files.Dequeue().Name);
							_running = false;
							continue;
						}
						else File.Delete(filePath);
					}

					using (FileStream fileStream = File.OpenWrite(filePath))
					{
						FileDownloading?.Invoke(this, file.Name);

						for (int chunk = 0; chunk <= chunkCount;)
						{
							cancellationToken.Value.ThrowIfCancellationRequested();

							ProgressChanged?.Invoke(this, chunk);

							long chunkStart = chunk * (response.Content.Headers.ContentLength.Value / chunkCount);
							long chunkEnd = (chunk + 1) * (response.Content.Headers.ContentLength.Value / chunkCount);

							if (chunk == chunkCount)
								chunkEnd = response.Content.Headers.ContentLength.Value;

							try
							{
								using (HttpRequestMessage request = new HttpRequestMessage())
								{
									request.RequestUri = new Uri(file.Url);
									request.Headers.Range = new RangeHeaderValue(chunkStart, chunkEnd);

									using (HttpResponseMessage rangedResponse = await _downloadClient.SendAsync(request, cancellationToken.Value))
									{
										fileStream.Seek(chunkStart, SeekOrigin.Begin);
										await fileStream.WriteAsync(await rangedResponse.Content.ReadAsByteArrayAsync(), cancellationToken.Value);
									}
								}
							}
							catch (Exception) { continue; }

							chunk++;
						}

						FileDownloaded?.Invoke(this, album.Files.Dequeue().Name);
					}
				}
				catch (Exception) { FileFailed?.Invoke(this, album.Files.Dequeue().Name); }

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

		private static string GetServerNameFromURL(string url)
		{
			Regex regex = new Regex(@"(fs-\d{2})");
			string result = null;
			Match match = regex.Match(url);
			if (match.Success) 
			{
				result = match.Captures[0].ToString();
			}
			else
			{
				throw new UriFormatException();
			}
			return result;
		}

		private static string ReplaceServerName(string url, string newServerName)
		{
			Regex regex = new Regex(@"(fs-\d{2})");
			if (!regex.Match(url).Success)
			{
				throw new UriFormatException();
			}
			return regex.Replace(url, newServerName, 1);
		}

		private async Task<HttpResponseMessage> CheckAvailability(AlbumFile file) {
			HttpResponseMessage response = null;
			try
			{
				response = await _downloadClient.GetAsync(file.Url, HttpCompletionOption.ResponseHeadersRead);
				if (response.ReasonPhrase == "Bad Gateway")
					throw new Exception(response.ReasonPhrase);

				while (response.ReasonPhrase == "Moved Temporarily")
					response = await _downloadClient.GetAsync(response.Headers.Location, HttpCompletionOption.ResponseHeadersRead);
			} 
			catch (Exception) {}
			return response;
		}
	}
}
