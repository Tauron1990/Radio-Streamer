using System;
using Tauron.Application.RadioStreamer.Database.Scripts;

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

                title = title.Trim();
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

            var temp = TagsProvider.CreateEmpty();
            temp.Title = title;
            temp.Artist = artist;
            temp.Albumartist = artist;
            temp.Filename = title;

            return temp;
        }

        public override string GetUrl
        {
            get { return "http://www.technobase.fm/"; }
        }
    }
}
