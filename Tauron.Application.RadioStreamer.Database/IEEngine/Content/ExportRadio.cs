using System;
using System.Collections.Generic;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Database.IEEngine.Content
{
    [Serializable]
    public class ExportRadio
    {
        [NotNull]
        public List<MetadataEntry> Entries { get; set; }

        public ExportRadio()
        {
            Entries = new List<MetadataEntry>();
        }
    }
}