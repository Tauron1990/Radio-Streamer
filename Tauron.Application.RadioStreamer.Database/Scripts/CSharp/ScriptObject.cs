using System.IO;
using HtmlAgilityPack;
using Tauron.Application.RadioStreamer.Contracts.Scripts;
using Tauron.JetBrains.Annotations;
using Un4seen.Bass.AddOn.Tags;

namespace Tauron.Application.RadioStreamer.Database.Scripts.CSharp
{
    public abstract class ScriptObject : ScriptingBase, IScript
    {
        public TAG_INFO GetTitleInfo(string url, TAG_INFO meta, out string title)
        {
            title = null;

            Url = url;
            Metadata = meta;
            Document = new HtmlDocument();
            Document.Load(new MemoryStream(WebClient.DownloadData(url)));

            object result = Invoke();

            if (result == null)
                return null;

            var strResult = result as string;
            if (strResult != null)
            {
                title = strResult;
                return new TAG_INFO { title = strResult };
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
                        return new TAG_INFO { title = strArrResult[0] };
                    default:
                        title = strArrResult[1] + strArrResult[0];
                        return new TAG_INFO { title = strArrResult[0], artist = strArrResult[1] };
                }
            }

            var tag = result as TAG_INFO;
            if (tag == null) return null;

            title = tag.artist + tag.title;

            return tag;
        }

        [CanBeNull]
        protected abstract object Invoke();
    }
}