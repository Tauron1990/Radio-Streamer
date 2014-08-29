using Elysium;
using Tauron.Application;
using Tauron.Application.RadioStreamer.Contracts.UI;

namespace ElysiumTheme
{
    public class Init : ThemeLoaderBase
    {
        public override void Load()
        {
            WpfApplication.CurrentWpfApplication.Apply(Theme.Dark, AccentBrushes.Violet, null);
        }
    }
}
