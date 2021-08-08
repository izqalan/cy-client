using Avalonia.Controls;
using ReactiveUI;

namespace CyberdropDownloader.Avalonia.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private TitleBarViewModel _titleBarViewModel = null!;
        private DestinationViewModel _destinationViewModel = null!;
        private OpenFolderViewModel _openFolderViewModel = null!;
        private DownloadViewModel _downloadViewModel = null!;

        public MainWindowViewModel(Window mainWindow)
        {
            TitleBarViewModel = new TitleBarViewModel(mainWindow);
            DestinationViewModel = new DestinationViewModel(mainWindow);
            OpenFolderViewModel = new OpenFolderViewModel(mainWindow);
            DownloadViewModel = new DownloadViewModel(mainWindow);
        }

        public DownloadViewModel DownloadViewModel
        {
            get => _downloadViewModel;
            set => this.RaiseAndSetIfChanged(ref _downloadViewModel, value);
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
