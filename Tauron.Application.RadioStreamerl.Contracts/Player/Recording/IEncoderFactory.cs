using System;
using Tauron.Application.BassLib.Encoder;
using Tauron.Application.RadioStreamer.Contracts.Data;
using Tauron.Application.RadioStreamer.Contracts.UI;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Contracts.Player.Recording
{
    public interface IEncoderFactory
    {
        string ID { get; }

        void RegisterOptions([NotNull] IUIOptionsManager manager);

        AudioEncoder Create(CommonProfile profile);
    }
}