using Avalonia.Controls;

namespace CyberdropDownloader.Avalonia.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel(Window mainWindow)
        {
            mainWindow.Find<DockPanel>("TitleBar").DataContext = new TitleBarViewModel(mainWindow);
        }
    }
}
