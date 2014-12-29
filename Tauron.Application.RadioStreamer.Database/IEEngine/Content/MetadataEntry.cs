using System;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Database.IEEngine.Content
{
    [Serializable]
    public class MetadataEntry
    {
        [NotNull]
        public string Key { get; set; }

        [NotNull]
        public string Value { get; set; }
    }
}