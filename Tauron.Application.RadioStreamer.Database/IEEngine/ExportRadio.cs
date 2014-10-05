using System;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Database.IEEngine
{
    [Serializable]
    public class ExportRadio
    {
        [NotNull]
        public string Script { get; set; }

        [NotNull]
        public string Name { get; set; }

        [NotNull]
        public string Genre { get; set; }

        [NotNull]
        public string Country { get; set; }

        [NotNull]
        public string Language { get; set; }

        [NotNull]
        public string Description { get; set; }

        public bool Integrated { get; set; }
    }
}