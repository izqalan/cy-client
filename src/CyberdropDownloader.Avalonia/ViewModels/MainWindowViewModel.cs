﻿using Avalonia.Controls;
using Avalonia.Threading;
using CyberdropDownloader.Avalonia.ViewModels.Core;
using CyberdropDownloader.Core;
using CyberdropDownloader.Core.Exceptions;
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
        private readonly Label _downloadProgess;

        private readonly WebScraper _webScraper;
        private readonly AlbumDownloader _albumDownloader;
        private int _totalDownloaded;
        private CancellationTokenSource _cancellationTokenSource;

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
            _downloadProgess = _mainWindow.Find<Label>("DownloadProgress");

            ExitCommand = ReactiveCommand.Create(Exit);
            MinimizeCommand = ReactiveCommand.Create(Minimize);
            OpenFolderCommand = ReactiveCommand.Create(OpenFolder);
            ReleasesCommand = ReactiveCommand.Create(OpenReleases);
            IssuesCommand = ReactiveCommand.Create(OpenIssues);
            DownloadCommand = ReactiveCommand.Create(Download);

            _albumDownloader.FileDownloaded += AlbumDownloader_FileDownloaded;
            _albumDownloader.FileDownloading += AlbumDownloader_FileDownloading;
            _albumDownloader.FileExists += AlbumDownloader_FileExists;
            _albumDownloader.FileFailed += AlbumDownloader_FileFailed;
            _albumDownloader.ProgressChanged += AlbumDownloader_ProgressChanged;
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

        private void Exit()
        {
            _mainWindow.Close();
        }

        private void Minimize()
        {
            _mainWindow.WindowState = WindowState.Minimized;
        }

        private void OpenReleases()
        {
            Process.Start(new ProcessStartInfo("https://github.com/izqalan/cy-client/releases") { UseShellExecute = true });
        }

        private void OpenIssues()
        {
            Process.Start(new ProcessStartInfo("https://github.com/izqalan/cy-client/issues") { UseShellExecute = true });
        }

        private async void Download()
        {
            // If the album downloader is currently downloading a file, then cancel the download and loop
            if(_albumDownloader.Running)
            {
                _cancellationTokenSource?.Cancel();
                _albumDownloader.CancelDownload();
            }

            // Open a new thread to take the load off the ui thread
            await Task.Run(async () =>
            {
                try
                {
                    // Each url in the url input box
                    foreach(string url in _urlInput.Text.Split(_urlInput.NewLine))
                    {
                        // If previously canceled, then create a new token
                        if(_cancellationTokenSource?.IsCancellationRequested == true)
                        {
                            _cancellationTokenSource = new CancellationTokenSource();
                        }

                        try
                        {
                            await _webScraper.LoadAlbumAsync(url);
                        }
                        catch(Exception ex)
                        {
                            switch(ex)
                            {
                                case NullAlbumTitleException:
                                    Log("Failed to fetch album title.");
                                    break;

                                case NullAlbumSizeException:
                                    Log("Failed to fetch album size.");
                                    break;

                                case NullAlbumFilesException:
                                    Log("Failed to fetch album files.");
                                    break;

                                case UriFormatException:
                                    Log("Invalid URL format.");
                                    continue;

                                default:
                                    Log($"Unknown webscraper error. Please report this to the github repository. {ex.Message}");
                                    continue;
                            }
                        }

                        // Update album title
                        UpdateAlbumTitle(_webScraper.Album.Title);

                        // Reset download progress
                        UpdateDownloadProgress(0);

                        // Log album title
                        Log($"Album: {_webScraper.Album.Title}");

                        try
                        {
                            DriveInfo driveInfo = new DriveInfo(_destinationInput.Text);

                            if(!driveInfo.IsReady)
                            {
                                Log($"{driveInfo.Name} is not ready.");
                                return;
                            }

                            if(driveInfo.AvailableFreeSpace < _webScraper.Album.Size)
                            {
                                Log($"{driveInfo.Name} lacks available free space for {_webScraper.Album.Title}.");
                                continue;
                            }
                        }
                        catch(Exception ex)
                        {
                            switch(ex)
                            {
                                // Drive letter doesn't exist or path doesn't exist
                                case ArgumentException:
                                    Log("Invalid destination path.");
                                    break;

                                // Has a disk error
                                case IOException:
                                    Log($"Destination path drive has unresolved errors.");
                                    break;

                                // Unauthorized to access
                                case UnauthorizedAccessException:
                                    Log("Unable to save to the destination path.");
                                    break;

                                default:
                                    Log($"Unknown error. Please report this to the github repository. {ex.Message}");
                                    break;
                            }
                        }

                        // Download album
                        await _albumDownloader.DownloadAsync(_webScraper.Album, _destinationInput.Text, _cancellationTokenSource?.Token, 100).ConfigureAwait(false);
                    }

                    // IF the total downloads are greater or equal to 1 then log the total downloads
                    if(_totalDownloaded >= 1)
                    {
                        Log($"------Completed {_totalDownloaded} Downloads------");
                    }
                } // Clear log if canceled
                catch(Exception) { ClearLog(); }
            });
        }

        private void OpenFolder()
        {
            // If directory doesn't exist, then log and exit out of method
            if(!Directory.Exists(_destinationInput.Text))
            {
                Log("Directory doesn't exist.");
                return;
            }

            // Start explorer with directory
            Process.Start("explorer.exe", _destinationInput.Text);
        }
        #endregion

        #region Events
        private void AlbumDownloader_FileDownloaded(object? sender, string fileName)
        {
            _totalDownloaded += 1;
        }

        private void AlbumDownloader_FileDownloading(object? sender, string fileName)
        {
            Log($"Downloading: {fileName}");
        }

        private void AlbumDownloader_FileExists(object? sender, string fileName)
        {
            Log($"[File Existed] [SKIP]: {fileName}");
        }

        private void AlbumDownloader_FileFailed(object? sender, string fileName)
        {
            Log($"[File Failed] [SKIP]: {fileName}");
        }

        private void AlbumDownloader_ProgressChanged(object? sender, int downloadPercent)
        {
            UpdateDownloadProgress(downloadPercent);
        }

        private void TitleBar_PointerPressed(object? sender, global::Avalonia.Input.PointerPressedEventArgs e)
        {
            _mainWindow.BeginMoveDrag(e);
        }

        private async void DestinationInput_PointerReleased(object? sender, global::Avalonia.Input.PointerReleasedEventArgs e)
        {
            OpenFolderDialog? dialog = new OpenFolderDialog()
            {
                Directory = Environment.CurrentDirectory,
                Title = "Choose download destination"
            };

            string result = await dialog.ShowAsync(_mainWindow);

            if(result != string.Empty)
            {
                _destinationInput.Text = result;
            }
        }
        #endregion

        private async void UpdateDownloadProgress(int progress)
        {
            await Dispatcher.UIThread.InvokeAsync(() => _downloadProgess.Content = $"{progress}%");
        }

        private async void Log(string data)
        {
            await Dispatcher.UIThread.InvokeAsync(() => _downloadLog.Text = $"{data}\n{_downloadLog.Text}");
        }

        private async void ClearLog()
        {
            await Dispatcher.UIThread.InvokeAsync(() => _downloadLog.Text = "");
        }

        private async void UpdateAlbumTitle(string title)
        {
            await Dispatcher.UIThread.InvokeAsync(() => _albumTitle.Content = $"Downloading: {title}");
        }
    }
}
