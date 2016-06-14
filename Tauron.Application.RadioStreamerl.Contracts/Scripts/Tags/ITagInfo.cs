using System.Collections.Generic;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Contracts.Scripts.Tags
{
    [PublicAPI]
    public interface ITagInfo
    {
        string Title { get; set; }
        string Artist { get; set; }
        string Album { get; set; }
        string Albumartist { get; set; }
        string Year { get; set; }
        string Comment { get; set; }
        string Genre { get; set; }
        string Track { get; set; }
        string Disc { get; set; }
        string Copyright { get; set; }
        string Encodedby { get; set; }
        string Composer { get; set; }
        string Publisher { get; set; }
        string Lyricist { get; set; }
        string Remixer { get; set; }
        string Producer { get; set; }
        string Bpm { get; set; }
        string Filename { get; set; }
        float ReplaygainTrackGain { get; set; }
        float ReplaygainTrackPeak { get; set; }
        string Conductor { get; set; }
        string Grouping { get; set; }
        string Mood { get; set; }
        string Rating { get; set; }
        string Isrc { get; set; }
        double Duration { get; set; }
        int Bitrate { get; set; }
        List<IPicture> Pictures { get; }
    }
}
