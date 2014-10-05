using System;
using System.Collections.Generic;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Database.IEEngine
{
    [Serializable]
    public class RadioStreamerExport
    {
        [NotNull]
        public Dictionary<string, string> Settings { get; set; }

        [NotNull]
        public List<ExportRadio> Radios { get; set; }

        [NotNull]
        public List<ExportQuality> Qualities { get; set; }

        public RadioStreamerExport()
        {
            Settings = new Dictionary<string, string>();
            Radios = new List<ExportRadio>();
            Qualities = new List<ExportQuality>();
        }
    }
}