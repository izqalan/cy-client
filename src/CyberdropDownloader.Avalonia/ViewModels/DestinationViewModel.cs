using Avalonia.Controls;
using System;

namespace CyberdropDownloader.Avalonia.ViewModels
{
    public class DestinationViewModel : ViewModelBase
    {
        private TextBox _destinationTextBox;
        private Window _mainWindow;

        public DestinationViewModel(Window mainWindow)
        {
            _mainWindow = mainWindow;
            _destinationTextBox = mainWindow.Find<TextBox>("DestinationTextBox");

            _destinationTextBox.PointerReleased += DestinationTextBox_Tapped;
        }

        private async void DestinationTextBox_Tapped(object? sender, global::Avalonia.Interactivity.RoutedEventArgs e)
        {
            var dialog = new OpenFolderDialog()
            {
                Directory = Environment.CurrentDirectory,
                Title = "Choose download destination"
            };

            string result = await dialog.ShowAsync(_mainWindow);

            if(result != string.Empty)
                _destinationTextBox.Text = result;
        }
    }
}
