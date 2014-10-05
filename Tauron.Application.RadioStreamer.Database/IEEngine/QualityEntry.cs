using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Database.IEEngine
{
    public class QualityEntry
    {
        [NotNull]
        public string Name { get; set; }

        [NotNull]
        public string Url { get; set; }
    }
}