using Avalonia.Controls;
using System.Diagnostics;
using System.IO;
using ReactiveUI;
using System.Reactive;

namespace CyberdropDownloader.Avalonia.ViewModels
{
    public class OpenFolderViewModel
    {
        private TextBox _destinationTextBox;

        public OpenFolderViewModel(TextBox destinationTextBox)
        {
            _destinationTextBox = destinationTextBox;

            OpenFolderCommand = ReactiveCommand.Create(OpenFolder);
        }

        public ReactiveCommand<Unit, Unit> OpenFolderCommand { get; }

        private void OpenFolder()
        {
            if (Directory.Exists(_destinationTextBox.Text))
            {
                Process.Start("explorer.exe", _destinationTextBox.Text);
            }
            else
            {
                //TODO Report to log that path isn't real
            }
        }
    }
}
