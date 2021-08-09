using Avalonia.Controls;
using ReactiveUI;
using System;
using System.Reactive;

namespace CyberdropDownloader.Avalonia.ViewModels
{
    public class DownloadViewModel : ViewModelBase
    {
        private Label _albumTitle = null!;
        private TextBlock _downloadLog = null!;
        private TextBox _urlInput = null!;

        public DownloadViewModel(Window mainWindow)
        {
            _albumTitle = mainWindow.Find<Label>("AlbumTitleLabel");
            _downloadLog = mainWindow.Find<TextBlock>("DownloadLogTextBlock");
            _urlInput = mainWindow.Find<TextBox>("UrlInputTextBox");

            DownloadCommand = ReactiveCommand.Create(FetchAndDownload);
        }

        public ReactiveCommand<Unit, Unit> DownloadCommand { get; }
        
        private void FetchAndDownload()
        {
            foreach(string entry in _urlInput.Text.Split(@"\n\r"))
            {
                // fetch url 
                // update album title
                // fetch files
                // download files
                // log result
            }
        }

        private void UpdateAlbumTitle(string title)
        {
            _albumTitle.Content = $"Downloading: {title}"; 
        }

        private void Log(string data)
        {
            _downloadLog.Text += $"{data}\n";
        }
    }
}
