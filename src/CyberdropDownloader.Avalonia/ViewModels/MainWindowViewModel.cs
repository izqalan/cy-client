using Avalonia.Controls;
using ReactiveUI;
using System.Reactive;

namespace CyberdropDownloader.Avalonia.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private Window _mainWindow;

        public MainWindowViewModel(Window mainWindow)
        {
            _mainWindow = mainWindow;

            QuitCommand = ReactiveCommand.Create(CloseWindow);
            MinimizeCommand = ReactiveCommand.Create(MinimizeWindow);
        }

        public ReactiveCommand<Unit, Unit> QuitCommand { get; }
        public ReactiveCommand<Unit, Unit> MinimizeCommand { get; }

        private void CloseWindow() => _mainWindow.Close();
        private void MinimizeWindow() => _mainWindow.WindowState = WindowState.Minimized;
    }
}
