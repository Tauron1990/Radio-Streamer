using System;
using Tauron.JetBrains.Annotations;
using Un4seen.Bass;

namespace Tauron.Application.BassLib.Channels
{
    [PublicAPI, Flags]
    public enum FileFlags
    {
        Default = BASSFlag.BASS_DEFAULT,
        Float = BASSFlag.BASS_SAMPLE_FLOAT,
        Mono = BASSFlag.BASS_SAMPLE_MONO,
        Software = BASSFlag.BASS_SAMPLE_SOFTWARE,
        Use3D = BASSFlag.BASS_SAMPLE_3D,
        Loop = BASSFlag.BASS_SAMPLE_LOOP,
        Fx = BASSFlag.BASS_SAMPLE_FX,
        PreScan = BASSFlag.BASS_STREAM_PRESCAN,
        AutoFree = BASSFlag.BASS_STREAM_AUTOFREE,
        Decode = BASSFlag.BASS_STREAM_DECODE,
        AsyncFile = BASSFlag.BASS_ASYNCFILE
    }
}