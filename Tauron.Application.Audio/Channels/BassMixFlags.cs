using System;
using Tauron.JetBrains.Annotations;
using Un4seen.Bass;

namespace Tauron.Application.BassLib.Channels
{
    [PublicAPI, Flags]
    public enum BassMixFlags
    {
        Default = BASSFlag.BASS_DEFAULT,
        EightBits = BASSFlag.BASS_SAMPLE_8BITS,
        Float = BASSFlag.BASS_SAMPLE_FLOAT,
        Software = BASSFlag.BASS_SAMPLE_SOFTWARE,
        Use3D = BASSFlag.BASS_SAMPLE_3D,
        OldFx = BASSFlag.BASS_SAMPLE_FX,
        End = BASSFlag.BASS_MIXER_END,
        Nonstop = BASSFlag.BASS_MIXER_NONSTOP,
        PosEx = BASSFlag.BASS_MIXER_POSEX,
        AutoFree = BASSFlag.BASS_STREAM_AUTOFREE,
        Decode = BASSFlag.BASS_STREAM_DECODE
    }
}