using Avalonia.Controls;
using Avalonia.Input;
using ReactiveUI;
using System.Reactive;

namespace CyberdropDownloader.Avalonia.ViewModels
{
    public class TitleBarViewModel : ViewModelBase
    {
        private readonly Window _mainWindow;
        private readonly DockPanel _titleBar;

        public TitleBarViewModel(Window mainWindow, DockPanel titleBar)
        {
            _mainWindow = mainWindow;
            _titleBar = titleBar;

            QuitCommand = ReactiveCommand.Create(CloseWindow);
            MinimizeCommand = ReactiveCommand.Create(MinimizeWindow);

            _titleBar.PointerPressed += TitleBar_PointerPressed;
            _titleBar.PointerMoved += TitleBar_PointerMoved;
            _titleBar.PointerReleased += TitleBar_PointerReleased;
        }

        public ReactiveCommand<Unit, Unit> QuitCommand { get; }
        public ReactiveCommand<Unit, Unit> MinimizeCommand { get; }

        private void CloseWindow() => _mainWindow.Close();
        private void MinimizeWindow() => _mainWindow.WindowState = WindowState.Minimized;

        private void TitleBar_PointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            throw new System.NotImplementedException();
        }
        private void TitleBar_PointerMoved(object? sender, PointerEventArgs e)
        {
            throw new System.NotImplementedException();
        }
        private void TitleBar_PointerPressed(object? sender, PointerPressedEventArgs e)
        {
            throw new System.NotImplementedException();
        }
    }
}
