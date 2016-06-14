using System.IO;
using HtmlAgilityPack;
using Tauron.Application.RadioStreamer.Contracts.Scripts;
using Tauron.Application.RadioStreamer.Contracts.Scripts.Tags;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Database.Scripts
{
    public abstract class ScriptObject : ScriptingBase, IScript
    {
        public ITagInfo GetTitleInfo(string url, ITagInfo meta, out string title)
        {
            title = null;

            Url = url;
            Metadata = meta;
            Document = new HtmlDocument();
            Document.Load(new MemoryStream(WebClient.DownloadData(GetUrl)));

            object result = Invoke();

            var strResult = result as string;
            if (strResult != null)
            {
                title = strResult;
                var temp = TagsProvider.CreateEmpty();
                temp.Title = title;
                return temp;
            }

            var strArrResult = result as string[];
            if (strArrResult != null)
            {
                switch (strArrResult.Length)
                {
                    case 0:
                        return null;
                    case 1:
                        title = strArrResult[0];
                        var temp1 = TagsProvider.CreateEmpty();
                        temp1.Title = title;
                        return temp1;
                    default:
                        title = strArrResult[1] + strArrResult[0];
                        var temp2 = TagsProvider.CreateEmpty();
                        temp2.Title = strArrResult[0];
                        temp2.Artist = strArrResult[1];
                        return temp2;
                }
            }

            var tag = result as ITagInfo;
            if (tag == null) return null;

            title = tag.Artist + tag.Title;

            return tag;
        }

        [CanBeNull]
        protected abstract object Invoke();

        [NotNull]
        public virtual string GetUrl => Url;
    }
}