using Avalonia.Controls;
using ReactiveUI;

namespace CyberdropDownloader.Avalonia.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private TitleBarViewModel _titleBar = null!;

        public MainWindowViewModel(Window mainWindow)
        {
            TitleBar = new TitleBarViewModel(mainWindow, mainWindow.Find<DockPanel>("TitleBar"));
        }

        public TitleBarViewModel TitleBar
        {
            get => _titleBar;
            set => this.RaiseAndSetIfChanged(ref _titleBar, value);
        }
    }
}
