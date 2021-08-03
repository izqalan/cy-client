using Avalonia.Controls;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive;

namespace CyberdropDownloader.Avalonia.ViewModels
{
    public class DownloadViewModel : ViewModelBase
    {
        private Label _albumTitle = null!;
        private TextBlock _downloadLog = null!;

        public DownloadViewModel(Label albumTitle, TextBox urlInput, TextBlock downloadLog)
        {
            _albumTitle = albumTitle;
            _downloadLog = downloadLog;

            DownloadCommand = ReactiveCommand.Create(DownloadFiles);
        }

        private List<string>? ParseUrlInput(string text)
        {
            throw new NotImplementedException();
        }

        public ReactiveCommand<Unit, Unit> DownloadCommand { get; }
        
        private void DownloadFiles()
        {
            // download files
            /* if _albumTitle.Text != empty
             * foreach line in _albumTitle.Text
             * download line
             */
        }
    }
}
