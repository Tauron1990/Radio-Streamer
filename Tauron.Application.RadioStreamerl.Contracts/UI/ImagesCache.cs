using System.Collections.Generic;
using System.Windows.Media;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Contracts.UI
{
    public static class ImagesCache
    {
        [NotNull]
        private static readonly Dictionary<string, ImageSource> _imageSources = new Dictionary<string, ImageSource>();

        [NotNull]
        public static Dictionary<string, ImageSource> ImageSources
        {
            get { return _imageSources; }
        }
    }
}