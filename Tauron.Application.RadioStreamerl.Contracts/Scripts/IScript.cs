using Tauron.JetBrains.Annotations;
using Un4seen.Bass.AddOn.Tags;

namespace Tauron.Application.RadioStreamer.Contracts.Scripts
{
    [PublicAPI]
    public interface IScript
    {
        [CanBeNull]
        TAG_INFO GetTitleInfo([NotNull] string url, [NotNull] TAG_INFO meta, out string title);
    }
}