using System;
using Tauron.Application.BassLib;
using Tauron.Application.BassLib.Misc;
using Tauron.Application.RadioStreamer.Contracts.Player;
using Tauron.Application.RadioStreamer.Contracts.Player.Recording;
using Tauron.Application.RadioStreamer.Contracts.Scripts.Tags;
using Tauron.Application.RadioStreamer.Player.Tags;
using Tauron.JetBrains.Annotations;
using Un4seen.Bass.Misc;

namespace Tauron.Application.RadioStreamer.Player.Encoder
{
    public abstract class AudioEncoderBase : AudioEncoder, IEncoder
    {
        private IPlayerStream _playerStream;

        protected AudioEncoderBase([NotNull] Func<Channel, BaseEncoder> encoder, [NotNull] IPlayerStream stream) : base(encoder, GetChannel(stream))
        {
            _playerStream = stream;
        }

        public IPlayerStream PlayerStream
        {
            get { return _playerStream; }
            set
            {
                _playerStream = value;
                Channel = GetChannel(value);
            }
        }

        public ITagInfo FileTags
        {
            get { return new BassTag(Tags); }
            set { Tags = BassTag.GetInfo(value); }
        }

        protected static Channel GetChannel(IPlayerStream stream)
        {
            var temp = stream as InternalPlayerStream;
            if (temp == null) throw new InvalidOperationException("The Type must be InternalPlayerStream.");

            return temp.Channel;
        }
    }
}