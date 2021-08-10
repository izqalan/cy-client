using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace CyberdropDownloader.Avalonia.Views
{
    public partial class TitleBar : UserControl
    {
        public TitleBar()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
