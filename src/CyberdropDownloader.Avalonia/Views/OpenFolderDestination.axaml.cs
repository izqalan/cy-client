using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace CyberdropDownloader.Avalonia.Views
{
    public partial class OpenFolderDestination : UserControl
    {
        public OpenFolderDestination()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
