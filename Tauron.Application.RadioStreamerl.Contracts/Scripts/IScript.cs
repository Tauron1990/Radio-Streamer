using Tauron.Application.RadioStreamer.Contracts.Scripts.Tags;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Contracts.Scripts
{
    [PublicAPI]
    public interface IScript
    {
        [CanBeNull]
        ITagInfo GetTitleInfo([NotNull] string url, [NotNull] ITagInfo meta, out string title);
    }
}