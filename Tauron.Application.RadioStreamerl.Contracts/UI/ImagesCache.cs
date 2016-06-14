using System.Collections.Generic;
using System.Windows.Media;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Contracts.UI
{
    public static class ImagesCache
    {
        [NotNull]
        public static Dictionary<string, ImageSource> ImageSources { get; } = new Dictionary<string, ImageSource>();
    }
}