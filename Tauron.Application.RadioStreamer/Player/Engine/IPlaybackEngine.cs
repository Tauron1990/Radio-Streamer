using System;
using System.Collections.Generic;
using Tauron.Application.BassLib;
using Tauron.JetBrains.Annotations;
using Un4seen.Bass.AddOn.Tags;

namespace Tauron.Application.RadioStreamer.Player.Engine
{
    public delegate void ChannelSwitch([NotNull] Channel channel, [CanBeNull] TAG_INFO info, bool newChannel);

    public interface IPlaybackEngine
    {
        event ChannelSwitch ChannelSwitched;
        event Action End;

        double BufferPercentage { get; }

        void Initialize([NotNull] BassEngine engine, [NotNull]Dictionary<int, string> plugins);

        [NotNull]
        Channel PlayChannel([NotNull] string url, out TAG_INFO tags);

        void Free();
    }
}