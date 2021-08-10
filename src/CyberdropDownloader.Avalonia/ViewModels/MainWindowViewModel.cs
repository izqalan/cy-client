using Avalonia.Controls;
using ReactiveUI;

namespace CyberdropDownloader.Avalonia.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private TitleBarViewModel _titleBarViewModel = null!;
        private FolderDestinationViewModel _folderDestinationViewModel = null!;
        private OpenFolderViewModel _openFolderDestinationViewModel = null!;
        private DownloadViewModel _downloadButtonViewModel = null!;

        public MainWindowViewModel(Window mainWindow)
        {
            TitleBarViewModel = new TitleBarViewModel(mainWindow);
            FolderDestinationViewModel = new FolderDestinationViewModel(mainWindow);
            OpenFolderDestinationViewModel = new OpenFolderViewModel(mainWindow);
            DownloadButtonViewModel = new DownloadViewModel(mainWindow);
        }

        public DownloadViewModel DownloadButtonViewModel
        {
            get => _downloadButtonViewModel;
            set => this.RaiseAndSetIfChanged(ref _downloadButtonViewModel, value);
        }

        public TitleBarViewModel TitleBarViewModel
        {
            get => _titleBarViewModel;
            set => this.RaiseAndSetIfChanged(ref _titleBarViewModel, value);
        }

        public FolderDestinationViewModel FolderDestinationViewModel
        {
            get => _folderDestinationViewModel;
            set => this.RaiseAndSetIfChanged(ref _folderDestinationViewModel, value);
        }

        public OpenFolderViewModel OpenFolderDestinationViewModel
        {
            get => _openFolderDestinationViewModel;
            set => this.RaiseAndSetIfChanged(ref _openFolderDestinationViewModel, value);
        }
    }
}
