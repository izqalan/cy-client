using ReactiveUI;
using System.Diagnostics;
using System.Reactive;

namespace CyberdropDownloader.Avalonia.ViewModels
{
    public class HyperLinksViewModel : ViewModelBase
    {
        public HyperLinksViewModel()
        {
            ReactiveCommand.Create(OpenIssues);
            ReactiveCommand.Create(OpenReleases);
        }

        public ReactiveCommand<Unit, Unit>? ReleasesCommand { get; }
        public ReactiveCommand<Unit, Unit>? IssuesCommand { get; }

        private void OpenReleases()
        {
            var process = new ProcessStartInfo("https://github.com/izqalan/cy-client/releases")
            {
                UseShellExecute = true
            };

            Process.Start(process);
        }

        private void OpenIssues()
        {
            var process = new ProcessStartInfo("https://github.com/izqalan/cy-client/issues")
            {
                UseShellExecute = true
            };

            Process.Start(process);
        }
    }
}
