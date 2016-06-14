using System.Net;
using HtmlAgilityPack;
using Mono.CSharp;
using Tauron.Application.RadioStreamer.Contracts.Scripts.Tags;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Database.Scripts
{
    [PublicAPI]
    public class ScriptingBase : InteractiveBase
    {
        public static ITagsProvider TagsProvider { get; } = CommonApplication.Current.Container.Resolve<ITagsProvider>();

        public static readonly WebClient WebClient = new WebClient();
        [NotNull]
        public static string Url { get; internal set; }

        [NotNull]
        public static ITagInfo Metadata { get; internal set; }

        [NotNull]
        public static HtmlDocument Document { get; internal set; }
    }
}
