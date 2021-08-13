using Avalonia.Controls;
using CyberdropDownloader.Avalonia.Views;
using ReactiveUI;
using System.Diagnostics;
using System.IO;
using System.Reactive;

namespace CyberdropDownloader.Avalonia.ViewModels
{
    public class OpenFolderViewModel
    {
        private TextBox _destinationTextBox = null!;
        private TextBlock _downloadLog = null!;

        public OpenFolderViewModel(Window mainWindow)
        {
            _destinationTextBox = mainWindow.Find<FolderDestination>("FolderDestination").Find<TextBox>("FolderDestinationTextBox");
            _downloadLog = mainWindow.Find<DownloadLog>("DownloadLog").Find<TextBlock>("DownloadLogTextBlock");

            OpenFolderDestinationCommand = ReactiveCommand.Create(OpenFolder);
        }

        public ReactiveCommand<Unit, Unit> OpenFolderDestinationCommand { get; }

        private void OpenFolder()
        {
            if (!Directory.Exists(_destinationTextBox.Text))
            {
                //TODO Report to log that path isn't real
                return;
            }

            Process.Start("explorer.exe", _destinationTextBox.Text);
        }
    }
}
