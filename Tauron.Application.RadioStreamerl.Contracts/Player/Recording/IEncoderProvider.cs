using Tauron.Application.BassLib;
using Tauron.Application.BassLib.Encoder;
using Tauron.Application.RadioStreamer.Contracts.Data;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Contracts.Player.Recording
{
    public interface IEncoderProvider
    {
        [NotNull]
        AudioEncoder CreateEncoder([CanBeNull] CommonProfile profile, [NotNull]Channel channel);
    }
}