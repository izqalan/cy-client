using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using CyberdropDownloader.Avalonia.Views;
using ReactiveUI;
using System.Reactive;

namespace CyberdropDownloader.Avalonia.ViewModels
{
    public class TitleBarViewModel : ViewModelBase
    {
        private readonly Window _mainWindow;
        private readonly TitleBar _titleBar;

        private bool _isPointerPressed;
        private PixelPoint _windowPosition;
        private Point _mouseOffset;

        public TitleBarViewModel(Window mainWindow)
        {
            _mainWindow = mainWindow;
            _titleBar = mainWindow.Find<TitleBar>("TitleBar");

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

        private void TitleBar_PointerPressed(object? sender, PointerPressedEventArgs e)
        {
            _mouseOffset = e.GetPosition(_titleBar);
            _windowPosition = _mainWindow.Position;
            _isPointerPressed = true;
        }

        private void TitleBar_PointerMoved(object? sender, PointerEventArgs e)
        {
            if (_isPointerPressed)
            {
                var tempPosition = e.GetPosition(_titleBar);
                _windowPosition = new PixelPoint((int)(_windowPosition.X + tempPosition.X - _mouseOffset.X), (int)(_windowPosition.Y + tempPosition.Y - _mouseOffset.Y));
                _mainWindow.Position = _windowPosition;
            }
        }

        private void TitleBar_PointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            _isPointerPressed = false;
        }
    }
}
