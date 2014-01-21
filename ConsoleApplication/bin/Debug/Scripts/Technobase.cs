using System;
using Tauron.Application.RadioStreamer.Database.Scripts.CSharp;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Tags;

namespace Tauron.Scripts
{
    public class Technobase : ScriptObject
    {
        protected override object Invoke()
        {
            string title;

            try
            {
                var contentShow = Document.GetElementbyId("Content_Show");
                var trackContent = contentShow.ChildNodes[3];

                title = trackContent.ChildNodes.Count > 2
                            ? trackContent.ChildNodes[1].InnerText
                            : trackContent.InnerText;

                title.Trim();
            }
            catch
            {
                title = "Unkown";
            }
            string[] names = title.Split(new[] { " - " }, StringSplitOptions.RemoveEmptyEntries);

            string artist;

            if (names.Length == 2)
            {
                artist = names[0].Trim();
                title = names[1].Trim();
            }
            else
            {
                artist = title;
                title = String.Empty;
            }

            return new TAG_INFO
            {
                tagType = BASSTag.BASS_TAG_ID3V2,
                title = title,
                artist = artist,
                bitrate = 128,
                albumartist = artist,
                filename = title
            };
        }
    }
}
