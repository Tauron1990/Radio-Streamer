using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tauron.Application.RadioStreamer.Contracts.Scripts
{
    class TagInfo
    {
        public string title = string.Empty;
        public string artist = string.Empty;
        public string album = string.Empty;
        public string albumartist = string.Empty;
        public string year = string.Empty;
        public string comment = string.Empty;
        public string genre = string.Empty;
        public string track = string.Empty;
        public string disc = string.Empty;
        public string copyright = string.Empty;
        public string encodedby = string.Empty;
        public string composer = string.Empty;
        public string publisher = string.Empty;
        public string lyricist = string.Empty;
        public string remixer = string.Empty;
        public string producer = string.Empty;
        public string bpm = string.Empty;
        public string filename = string.Empty;
        private List<TagPicture> a = new List<TagPicture>();
        private List<string> b = new List<string>();
        public BASS_CHANNELINFO channelinfo = new BASS_CHANNELINFO();
        public BASSTag tagType = BASSTag.BASS_TAG_UNKNOWN;
        public float replaygain_track_gain = -100f;
        public float replaygain_track_peak = -1f;
        public string conductor = string.Empty;
        public string grouping = string.Empty;
        public string mood = string.Empty;
        public string rating = string.Empty;
        public string isrc = string.Empty;
        public double duration;
        public int bitrate;
    }
}
