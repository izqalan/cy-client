using Avalonia.Controls;
using CyberdropDownloader.Avalonia.Views;
using System;

namespace CyberdropDownloader.Avalonia.ViewModels
{
    public class FolderDestinationViewModel : ViewModelBase
    {
        private TextBox _folderDestinationTextBox;
        private Window _mainWindow;

        public FolderDestinationViewModel(Window mainWindow)
        {
            _mainWindow = mainWindow;
            _folderDestinationTextBox = mainWindow.Find<FolderDestination>("FolderDestination").Find<TextBox>("FolderDestinationTextBox");

            _folderDestinationTextBox.PointerReleased += TextBox_Tapped;
        }

        private async void TextBox_Tapped(object? sender, global::Avalonia.Interactivity.RoutedEventArgs e)
        {
            var dialog = new OpenFolderDialog()
            {
                Directory = Environment.CurrentDirectory,
                Title = "Choose download destination"
            };

            string result = await dialog.ShowAsync(_mainWindow);

            if(result != string.Empty)
                _folderDestinationTextBox.Text = result;
        }
    }
}
