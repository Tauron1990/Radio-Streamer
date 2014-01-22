using System;
using Tauron.Application.RadioStreamer.Contracts.Data.Enttitis;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Contracts.Player
{
    [PublicAPI]
    public sealed class PlayRadioEventArgs : EventArgs
    {
        public RadioEntry Radio { get; private set; }
        public RadioQuality Quality { get; private set; }

        public PlayRadioEventArgs(RadioQuality quality, RadioEntry radio)
        {
            Radio = radio;
            Quality = quality;
        }
    }
}