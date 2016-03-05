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
            handler?.Invoke();
        }

        protected virtual void OnChannelSwitched([NotNull] Channel channel, [CanBeNull] TAG_INFO info, bool newChannel)
        {
            var handler = ChannelSwitched;
            if (handler != null) Async.StartNew(() => handler(channel, info, newChannel));
        }

        public abstract double BufferPercentage { get; }
        public abstract void Initialize(BassEngine engine, Dictionary<int, string> plugins);
        public abstract Channel PlayChannel(string url, out TAG_INFO tags);
        public abstract void Free();
    }
}