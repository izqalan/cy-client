using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using CyberdropDownloader.Avalonia.ViewModels;

namespace CyberdropDownloader.Avalonia.Views
{
    public partial class MainWindowView : Window
    {
        public MainWindowView()
        {
            InitializeComponent();

            DataContext = new MainWindowViewModel(this);
        }

        private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
    }
}