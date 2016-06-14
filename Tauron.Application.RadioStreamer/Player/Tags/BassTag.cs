using System.Collections.Generic;
using Tauron.Application.RadioStreamer.Contracts.Scripts.Tags;
using Un4seen.Bass.AddOn.Tags;

namespace Tauron.Application.RadioStreamer.Player.Tags
{
    public sealed class BassTag : ITagInfo
    {
        public BassTag()
        {
            Pictures = new List<IPicture>();
        }

        public BassTag(TAG_INFO info)
            : this()
        {
            Title = info.title;
            Artist = info.artist;
            Album = info.album;
            Albumartist = info.albumartist;
            Year = info.year;
            Comment = info.comment;
            Genre = info.genre;
            Track = info.track;
            Disc = info.disc;
            Copyright = info.copyright;
            Encodedby = info.encodedby;
            Composer = info.composer;
            Publisher = info.publisher;
            Lyricist = info.lyricist;
            Remixer = info.remixer;
            Producer = info.producer;
            Bpm = info.bpm;
            Filename = info.filename;
            ReplaygainTrackGain = info.replaygain_track_gain;
            ReplaygainTrackPeak = info.replaygain_track_peak;
            Conductor = info.conductor;
            Grouping = info.grouping;
            Mood = info.mood;
            Rating = info.rating;
            Isrc = info.isrc;
            Duration = info.duration;
            Bitrate = info.bitrate;

            for (int i = 0; i < info.PictureCount; i++)
            {
                var pic = info.PictureGet(i);

                Pictures.Add(new BassPicture(pic));
            }
        }

        public string Title { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
        public string Albumartist { get; set; }
        public string Year { get; set; }
        public string Comment { get; set; }
        public string Genre { get; set; }
        public string Track { get; set; }
        public string Disc { get; set; }
        public string Copyright { get; set; }
        public string Encodedby { get; set; }
        public string Composer { get; set; }
        public string Publisher { get; set; }
        public string Lyricist { get; set; }
        public string Remixer { get; set; }
        public string Producer { get; set; }
        public string Bpm { get; set; }
        public string Filename { get; set; }
        public float ReplaygainTrackGain { get; set; }
        public float ReplaygainTrackPeak { get; set; }
        public string Conductor { get; set; }
        public string Grouping { get; set; }
        public string Mood { get; set; }
        public string Rating { get; set; }
        public string Isrc { get; set; }
        public double Duration { get; set; }
        public int Bitrate { get; set; }
        public List<IPicture> Pictures { get; }


        internal static TAG_INFO GetInfo(ITagInfo intInfo)
        {
            var info = new TAG_INFO()
            {
                title = intInfo.Title,
                artist = intInfo.Artist,
                album = intInfo.Album,
                albumartist = intInfo.Albumartist,
                year = intInfo.Year,
                comment = intInfo.Comment,
                genre = intInfo.Genre,
                track = intInfo.Track,
                disc = intInfo.Disc,
                copyright = intInfo.Copyright,
                encodedby = intInfo.Encodedby,
                composer = intInfo.Composer,
                publisher = intInfo.Publisher,
                lyricist = intInfo.Lyricist,
                remixer = intInfo.Remixer,
                producer = intInfo.Producer,
                bpm = intInfo.Bpm,
                filename = intInfo.Filename,
                replaygain_track_gain = intInfo.ReplaygainTrackGain,
                replaygain_track_peak = intInfo.ReplaygainTrackPeak,
                conductor = intInfo.Conductor,
                grouping = intInfo.Grouping,
                mood = intInfo.Mood,
                rating = intInfo.Rating,
                isrc = intInfo.Isrc,
                duration = intInfo.Duration,
                bitrate = intInfo.Bitrate
            };

            foreach (var picture in intInfo.Pictures)
            {
                info.AddPicture(BassPicture.GeTagPicture(picture));
            }

            return info;
        }

    }
}