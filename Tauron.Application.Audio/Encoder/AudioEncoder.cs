using System;
using System.Windows.Markup;
using Tauron.JetBrains.Annotations;
using Un4seen.Bass.AddOn.Enc;
using Un4seen.Bass.AddOn.Tags;
using Un4seen.Bass.Misc;

namespace Tauron.Application.BassLib.Encoder
{
    [PublicAPI]
    public abstract class AudioEncoder : IDisposable
    {
        private readonly BaseEncoder _encoder;
        private Channel _channel;

        protected AudioEncoder([NotNull] BaseEncoder encoder, [NotNull] Channel channel)
        {
            if (encoder == null) throw new ArgumentNullException("encoder");
            if (channel == null) throw new ArgumentNullException("channel");
            
            _encoder = encoder;
            _channel = channel;
        }

        [NotNull]
        internal BaseEncoder BassEncoder
        {
            get { return _encoder; }
        }

        public bool SupportsStdout { get { return BassEncoder.SupportsSTDOUT; } }

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
