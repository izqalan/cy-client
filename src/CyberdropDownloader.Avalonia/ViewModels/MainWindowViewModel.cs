using Avalonia.Controls;
using ReactiveUI;

namespace CyberdropDownloader.Avalonia.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private TitleBarViewModel _titleBarViewModel = null!;
        private DestinationViewModel _destinationViewModel = null!;
        private OpenFolderViewModel _openFolderViewModel = null!;

        public MainWindowViewModel(Window mainWindow)
        {
            TitleBarViewModel = new TitleBarViewModel(mainWindow, mainWindow.Find<DockPanel>("TitleBar"));
            DestinationViewModel = new DestinationViewModel(mainWindow, mainWindow.Find<TextBox>("DestinationTextBox"));
            OpenFolderViewModel = new OpenFolderViewModel(mainWindow.Find<TextBox>("DestinationTextBox"));
        }

        public TitleBarViewModel TitleBarViewModel
        {
            get => _titleBarViewModel;
            set => this.RaiseAndSetIfChanged(ref _titleBarViewModel, value);
        }

        public DestinationViewModel DestinationViewModel
        {
            get => _destinationViewModel;
            set => this.RaiseAndSetIfChanged(ref _destinationViewModel, value);
        }

        public OpenFolderViewModel OpenFolderViewModel
        {
            get => _openFolderViewModel;
            set => this.RaiseAndSetIfChanged(ref _openFolderViewModel, value);
        }
    }
}
