using System;
using Tauron.Application.BassLib.Misc;
using Tauron.JetBrains.Annotations;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Mix;
using Un4seen.Bass.AddOn.Tags;

namespace Tauron.Application.BassLib.Channels
{
    [PublicAPI]
    public enum MixSpeaker
    {
        Mono = 1,
        Stereo = 2,
        Quadrophonic = 4,
        Sourround51 = 6,
        Sourrounf71 = 8
    }

    [PublicAPI]
    public sealed class Mix : Channel
    {
        private Equalizer _equalizer;

        public Mix(int freq = 44100, BassMixFlags flags = BassMixFlags.Default, MixSpeaker speaker = MixSpeaker.Stereo)
            : base(BassMix.BASS_Mixer_StreamCreate(freq, (int) speaker, (BASSFlag) flags))
        {
            if(Handle == 0)
                throw new BassException();
        }

        [NotNull]
        public Equalizer Equalizer
        {
            get
            {
                if (_equalizer != null) return _equalizer;

                _equalizer = new Equalizer();
                _equalizer.Init(Handle);

                return _equalizer;
            }
            set
            {
                if (_equalizer != null) _equalizer.Free();

                _equalizer = value;
                _equalizer.Init(Handle);
            }
        }

        internal void Attach(int handle)
        {
            BassMix.BASS_Mixer_StreamAddChannel(Handle, handle, BASSFlag.BASS_DEFAULT).CheckBass();
        }

        public void Attach([NotNull] Channel channel)
        {
            if (channel == null) throw new ArgumentNullException("channel");

            channel.Mix = this;
        }

        internal void DeAttach(int handle)
        {
            if(Handle == 0) return;

            BassMix.BASS_Mixer_ChannelRemove(handle).CheckBass();
        }

        public void DeAttach([NotNull] Channel channel)
        {
            if (channel == null) throw new ArgumentNullException("channel");

            channel.Mix = null;
        }

        public override TAG_INFO Tag
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        protected override void Dispose(bool disposing)
        {
            Bass.BASS_StreamFree(HandleFree());
        }
    }
}