using System;
using System.Collections.Generic;
using Tauron.Application.BassLib;
using Tauron.JetBrains.Annotations;
using Un4seen.Bass.AddOn.Tags;

namespace Tauron.Application.RadioStreamer.Player.Engine.Impl
{
    [PublicAPI]
    public abstract class PlaybackEngineBase : IPlaybackEngine
    {
        public event ChannelSwitch ChannelSwitched;
        public event Action End;

        protected virtual void OnEnd()
        {
            var handler = End;
            if (handler != null) handler();
        }

        protected virtual void OnChannelSwitched([NotNull] Channel channel, [CanBeNull] TAG_INFO info)
        {
            var handler = ChannelSwitched;
            if (handler != null) handler(channel, info);
        }

        public abstract double BufferPercentage { get; }
        public abstract void Initialize(BassEngine engine, Dictionary<int, string> plugins);
        public abstract Channel PlayChannel(string url, out TAG_INFO tags);
        public abstract void Free();
    }
}