using Tauron.Application.RadioStreamer.Contracts.Data;
using Tauron.Application.RadioStreamer.Contracts.UI;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Contracts.Player.Recording
{
    public interface IEncoderFactory
    {
        [NotNull]
        string DisplayName { get; }

        [NotNull]
        string Id { get; }

        void RegisterOptions([NotNull] IUIOptionsManager manager);

        [CanBeNull]
        IEncoder Create([CanBeNull] CommonProfile profile, [NotNull] IPlayerStream channel);
    }
}