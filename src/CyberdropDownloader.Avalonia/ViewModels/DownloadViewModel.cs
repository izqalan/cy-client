using Avalonia.Controls;
using ReactiveUI;
using System.Reactive;
using CyberdropDownloader.Avalonia.Views;
using CyberdropDownloader.Core;
using System.Collections.Generic;
using CyberdropDownloader.Core.DataModels;
using CyberdropDownloader.Core.Enums;

namespace CyberdropDownloader.Avalonia.ViewModels
{
    public class DownloadViewModel : ViewModelBase
    {
        private Label _albumTitle = null!;
        private TextBlock _downloadLog = null!;
        private TextBox _urlInput = null!;
        private TextBox _folderDestination = null!;

        private int _totalDownloaded = 0;

        public DownloadViewModel(Window mainWindow)
        {
            _albumTitle = mainWindow.Find<AlbumTitle>("AlbumTitle").Find<Label>("AlbumTitleLabel");
            _downloadLog = mainWindow.Find<DownloadLog>("DownloadLog").Find<TextBlock>("DownloadLogTextBlock");
            _urlInput = mainWindow.Find<UrlInput>("UrlInput").Find<TextBox>("UrlInputTextBox");
            _folderDestination = mainWindow.Find<FolderDestination>("FolderDestination").Find<TextBox>("FolderDestinationTextBox");

            DownloadCommand = ReactiveCommand.Create(FetchAndDownload);
        }

        public ReactiveCommand<Unit, Unit> DownloadCommand { get; }

        private async void FetchAndDownload()
        {
            string[] entries = _urlInput.Text.Split('\n', '\r');

            foreach (string entry in entries)
            {
                WebScraper webScraper = null!;

                try
                {
                    webScraper = new WebScraper(entry);
                }
                catch
                {
                    Log($"Invalid Url: {entry}");
                    break;
                }

                string albumTitle = webScraper.FetchAlbumTitle();
                Queue<AlbumFile> files = webScraper.FetchAlbumFiles();

                UpdateAlbumTitle(albumTitle);

                while (files.Count > 0)
                {
                    AlbumFile file = files.Peek();

                    Log($"Downloading item: {file.Name}");

                    DownloadResponse response = await FileDownloader.DownloadFile(file.Url, $"{_folderDestination.Text}\\{albumTitle}", file.Name);

                    switch (response)
                    {
                        case DownloadResponse.Downloaded:
                            files.Dequeue();
                            _totalDownloaded += 1;
                            continue;

                        case DownloadResponse.Failed:
                            Log($"[File Failed] [RETRYING]: {file.Name}");
                            continue;

                        case DownloadResponse.FileExists:
                            Log($"[File Existed] [SKIP]: {file.Name}");
                            files.Dequeue();
                            continue;

                        case DownloadResponse.NotEnoughSpace:
                            Log($"[Not Enough Storage] [SKIP]: {file.Name}");
                            files.Dequeue();
                            continue;
                    }
                }
            }

            if (_totalDownloaded >= 1)
            {
                Log($"------Completed {_totalDownloaded} Downloads------");
            }
        }

        private void UpdateAlbumTitle(string title)
        {
            _albumTitle.Content = $"Downloading: {title}";
            Log($"Album: {title}");
        }

        private void Log(string data)
        {
            _downloadLog.Text = $"{data}\n{_downloadLog.Text}";
        }
    }
}
