using System;
using Tauron.JetBrains.Annotations;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Mix;

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
        public Mix(int freq = 44100, BassMixFlags flags = BassMixFlags.Default, MixSpeaker speaker = MixSpeaker.Stereo)
            : base(BassMix.BASS_Mixer_StreamCreate(freq, (int) speaker, (BASSFlag) flags))
        {
            if(Handle == 0)
                throw new BassException();
        }

        internal void Attach(int handle)
        {
            if(!BassMix.BASS_Mixer_StreamAddChannel(Handle, handle, BASSFlag.BASS_DEFAULT))
                throw new BassException();
        }

        internal void DeAttach(int handle)
        {
            if(Handle == 0) return;

            if(!BassMix.BASS_Mixer_ChannelRemove(handle))
                throw new BassException();
        }

        protected override void Dispose(bool disposing)
        {
            Bass.BASS_StreamFree(HandleFree());
        }
    }
}