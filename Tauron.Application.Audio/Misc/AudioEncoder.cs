using System;
using Tauron.JetBrains.Annotations;
using Un4seen.Bass.AddOn.Tags;
using Un4seen.Bass.Misc;

namespace Tauron.Application.BassLib.Misc
{
    [PublicAPI]
    public abstract class AudioEncoder : IDisposable
    {
        private Channel _channel;

        protected AudioEncoder([NotNull] Func<Channel, BaseEncoder> encoder, [NotNull] Channel channel)
        {
            if (encoder == null) throw new ArgumentNullException(nameof(encoder));
            if (channel == null) throw new ArgumentNullException(nameof(channel));
            
            _channel = channel;
            BassEncoder = encoder(channel);
        }

        [NotNull]
        protected internal BaseEncoder BassEncoder { get; }

        public bool SupportsStdout => BassEncoder.SupportsSTDOUT;

        [NotNull]
        public Channel Channel
        {
            get { return _channel; }
            set
            {
                BassEncoder.ChannelHandle = value.Handle;

                _channel = value;
            }
        }

        public bool NoLimit { get { return BassEncoder.NoLimit; } set { BassEncoder.NoLimit = value; } }
        [CanBeNull]
        public string InputFile { get { return BassEncoder.InputFile; } set { BassEncoder.InputFile = value; } }
        [CanBeNull]
        public string OutputFile { get { return BassEncoder.OutputFile; } set { BassEncoder.OutputFile = value; } }

        [NotNull]
        public TAG_INFO Tags { get { return BassEncoder.TAGs; } set { BassEncoder.TAGs = value; } }

        public void Dispose()
        {
            BassEncoder.Dispose();
        }

        public void Start(bool paused = false)
        {
            BassEncoder.Start(null, IntPtr.Zero, paused).CheckBass();
        }

        public void Stop()
        {
            BassEncoder.Stop();
        }
    }
}
