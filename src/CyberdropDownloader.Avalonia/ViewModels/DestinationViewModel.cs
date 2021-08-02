using Avalonia;
using Avalonia.Controls;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace CyberdropDownloader.Avalonia.ViewModels
{
    public class DestinationViewModel : ViewModelBase
    {
        private TextBox _destinationTextBox;
        private Window _mainWindow;

        public DestinationViewModel(Window mainWindow, TextBox destinationTextbox)
        {
            _mainWindow = mainWindow;
            _destinationTextBox = destinationTextbox;

            _destinationTextBox.PointerReleased += DestinationTextBox_Tapped;
        }

        private async void DestinationTextBox_Tapped(object? sender, global::Avalonia.Interactivity.RoutedEventArgs e)
        {
            var dialog = new OpenFolderDialog()
            {
                Directory = Environment.CurrentDirectory,
                Title = "Choose download destination"
            };
            
            _destinationTextBox.Text = await dialog.ShowAsync(_mainWindow);
        }
    }
}
