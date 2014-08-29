using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Contracts.UI
{
    [PublicAPI]
    public abstract class ThemeLoaderBase
    {
        public abstract void Load();
    }
}
