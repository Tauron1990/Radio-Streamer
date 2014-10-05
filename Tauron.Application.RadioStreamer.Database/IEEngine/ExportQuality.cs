using System;
using System.Collections.Generic;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Database.IEEngine
{
    [Serializable]
    public class ExportQuality
    {
        [NotNull]
        public string Name { get; set; }

        [NotNull]
        public List<QualityEntry> Qualitys { get; set; }

        public ExportQuality()
        {
            Qualitys = new List<QualityEntry>();
        }
    }
}