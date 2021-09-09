using Avalonia.Controls;
using Avalonia.Threading;
using CyberdropDownloader.Avalonia.ViewModels.Core;
using CyberdropDownloader.Core;
using ReactiveUI;
using System;
using System.Diagnostics;
using System.IO;
using System.Reactive;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace CyberdropDownloader.Avalonia.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly Window _mainWindow;
        private readonly TextBox _destinationInput;
        private readonly TextBlock _downloadLog;
        private readonly Label _albumTitle;
        private readonly TextBox _urlInput;
        private readonly DockPanel _titleBar;
        private readonly Label _applicationTitle;

        private readonly WebScraper _webScraper;
        private readonly AlbumDownloader _albumDownloader;
        private int _totalDownloaded;
        private string[]? _urls;
        private CancellationTokenSource? _cancellationTokenSource;

        public MainWindowViewModel(Window mainWindow)
        {
            _webScraper = new WebScraper();
            _albumDownloader = new AlbumDownloader(true);

            _mainWindow = mainWindow;

            _destinationInput = _mainWindow.Find<TextBox>("DestinationInput");
            _albumTitle = _mainWindow.Find<Label>("AlbumTitle");
            _downloadLog = _mainWindow.Find<TextBlock>("DownloadLog");
            _urlInput = _mainWindow.Find<TextBox>("UrlInput");
            _titleBar = _mainWindow.Find<DockPanel>("TitleBar");
            _applicationTitle = _mainWindow.Find<Label>("ApplicationTitle");

            ExitCommand = ReactiveCommand.Create(Exit);
            MinimizeCommand = ReactiveCommand.Create(Minimize);
            OpenFolderCommand = ReactiveCommand.Create(OpenFolder);
            ReleasesCommand = ReactiveCommand.Create(OpenReleases);
            IssuesCommand = ReactiveCommand.Create(OpenIssues);
            DownloadCommand = ReactiveCommand.Create(Download);

            _albumDownloader.FileDownloaded += AlbumDownloader_FileDownloaded; ;
            _albumDownloader.FileDownloading += AlbumDownloader_FileDownloading;
            _albumDownloader.FileExists += AlbumDownloader_FileExists;
            _albumDownloader.FileFailed += AlbumDownloader_FileFailed;
            _titleBar.PointerPressed += TitleBar_PointerPressed;
            _destinationInput.PointerReleased += DestinationInput_PointerReleased;

            _applicationTitle.Content = $"cy client - v{Assembly.GetExecutingAssembly().GetName().Version}";
            _destinationInput.Text = Environment.CurrentDirectory;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        #region Commands
        public ReactiveCommand<Unit, Unit>? OpenFolderCommand { get; }
        public ReactiveCommand<Unit, Unit>? DownloadCommand { get; }
        public ReactiveCommand<Unit, Unit>? ExitCommand { get; }
        public ReactiveCommand<Unit, Unit>? MinimizeCommand { get; }
        public ReactiveCommand<Unit, Unit>? ReleasesCommand { get; }
        public ReactiveCommand<Unit, Unit>? IssuesCommand { get; }

        private void Exit() => _mainWindow.Close();
        private void Minimize() => _mainWindow.WindowState = WindowState.Minimized;
        private void OpenReleases() => Process.Start(new ProcessStartInfo("https://github.com/izqalan/cy-client/releases") { UseShellExecute = true });
        private void OpenIssues() => Process.Start(new ProcessStartInfo("https://github.com/izqalan/cy-client/issues") { UseShellExecute = true });
        private async void Download()
        {
            if (_albumDownloader.Running)
            {
                _cancellationTokenSource?.Cancel();
                _albumDownloader.CancelDownload();
            }

            await Task.Run(async () =>
            {
                _urls = _urlInput.Text.Split(_urlInput.NewLine);

                try
                {
                    // each url in the url input box
                    foreach (string url in _urls)
                    {
                        if (_cancellationTokenSource?.IsCancellationRequested == true)
                            _cancellationTokenSource = new CancellationTokenSource();

                        // load the album
                        await _webScraper.LoadAlbumAsync(url).ConfigureAwait(false);

                        if (!_webScraper.Successful)
                        {
                            Log($"Invalid Url: {url}");
                            continue;
                        }

                        UpdateAlbumTitle(_webScraper.Album.Title);
                        Log($"Album: {_webScraper.Album.Title}");

                        // download album
                        await _albumDownloader.DownloadAsync(_webScraper.Album, _destinationInput.Text, _cancellationTokenSource).ConfigureAwait(false);
                    }

                    if (_totalDownloaded >= 1)
                    {
                        Log($"------Completed {_totalDownloaded} Downloads------");
                    }

                }
                catch (Exception) { ClearLog(); }
            }).ConfigureAwait(true);
        }
        private void OpenFolder()
        {
            if (!Directory.Exists(_destinationInput.Text))
            {
                Log("Directory doesn't exist.");
                return;
            }

            Process.Start("explorer.exe", _destinationInput.Text);
        }
        #endregion

        #region Events
        private void AlbumDownloader_FileDownloaded(string fileName)
        {
            _totalDownloaded += 1;
            Log($"Downloaded: {fileName}");
        }
        private void AlbumDownloader_FileDownloading(string fileName) => Log($"Downloading: {fileName}");
        private void AlbumDownloader_FileExists(string fileName) => Log($"[File Existed] [SKIP]: {fileName}");
        private void AlbumDownloader_FileFailed(string fileName) => Log($"[File Failed] [RETRYING]: {fileName}");
        private void TitleBar_PointerPressed(object? sender, global::Avalonia.Input.PointerPressedEventArgs e) => _mainWindow.BeginMoveDrag(e);
        private async void DestinationInput_PointerReleased(object? sender, global::Avalonia.Input.PointerReleasedEventArgs e)
        {
            var dialog = new OpenFolderDialog()
            {
                Directory = Environment.CurrentDirectory,
                Title = "Choose download destination"
            };

            string result = await dialog.ShowAsync(_mainWindow);

            if (result != string.Empty)
                _destinationInput.Text = result;
        }
        #endregion

        private async void Log(string data) => await Dispatcher.UIThread.InvokeAsync(() => _downloadLog.Text = $"{data}\n{_downloadLog.Text}");
        private async void ClearLog() => await Dispatcher.UIThread.InvokeAsync(() => _downloadLog.Text = "");
        private async void UpdateAlbumTitle(string title) => await Dispatcher.UIThread.InvokeAsync(() => _albumTitle.Content = $"Downloading: {title}");
    }
}
