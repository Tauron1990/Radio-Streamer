using System;
using Tauron.JetBrains.Annotations;
using Un4seen.Bass;

namespace Tauron.Application.BassLib.Channels
{
    [Flags, PublicAPI]
    public enum WebStreamFlags
    {
        Default = BASSFlag.BASS_DEFAULT,
        Float = BASSFlag.BASS_SAMPLE_FLOAT,
        Mono = BASSFlag.BASS_SAMPLE_MONO,
        Software = BASSFlag.BASS_SAMPLE_SOFTWARE,
        Use3D = BASSFlag.BASS_SAMPLE_3D, 
        Loop = BASSFlag.BASS_SAMPLE_LOOP,
        Fx = BASSFlag.BASS_SAMPLE_FX,
        Restrate = BASSFlag.BASS_STREAM_RESTRATE,
        Blook = BASSFlag.BASS_STREAM_BLOCK,
        Status = BASSFlag.BASS_STREAM_STATUS,
        Autofree = BASSFlag.BASS_STREAM_AUTOFREE,
        Decode = BASSFlag.BASS_STREAM_DECODE
    }
}