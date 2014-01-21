using System.Net;
using HtmlAgilityPack;
using Mono.CSharp;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Database.Scripts
{
    [PublicAPI]
    public class ScriptingBase : InteractiveBase
    {
        public static readonly WebClient WebClient = new WebClient();
        [NotNull]
        public static string Url { get; internal set; }

        [NotNull]
        public static string[] Metadata { get; internal set; }

        [NotNull]
        public static HtmlDocument Document { get; internal set; }
    }
}
