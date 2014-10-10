using System;

namespace Tauron.Application.RadioStreamer.Database.IEEngine
{
    [Serializable]
    public class MetadataEntry
    {
        public string Key { get; set; }

        public string Value { get; set; }
    }
}