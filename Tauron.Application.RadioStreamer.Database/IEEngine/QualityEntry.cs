using System;
using System.Collections.Generic;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Database.IEEngine
{
    [Serializable]
    public class QualityEntry
    {
        [NotNull]
        public List<MetadataEntry> Entries { get; set; }

        public QualityEntry()
        {
            Entries = new List<MetadataEntry>();
        }
    }
}