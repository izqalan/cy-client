using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace CyberdropDownloader.Avalonia.Views
{
    public partial class DownloadLog : UserControl
    {
        public DownloadLog()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
