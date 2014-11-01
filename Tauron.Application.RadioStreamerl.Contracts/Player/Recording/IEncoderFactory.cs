using Tauron.Application.BassLib;
using Tauron.Application.BassLib.Encoder;
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
        string ID { get; }

        void RegisterOptions([NotNull] IUIOptionsManager manager);

        [CanBeNull]
        AudioEncoder Create([CanBeNull] CommonProfile profile, [NotNull] Channel channel);
    }
}