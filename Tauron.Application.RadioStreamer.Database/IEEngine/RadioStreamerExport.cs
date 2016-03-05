using System;
using System.Collections.Generic;
using Tauron.Application.RadioStreamer.Contracts.Data;
using Tauron.Application.RadioStreamer.Database.IEEngine.Content;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Database.IEEngine
{
    [Serializable]
    public class RadioStreamerExport
    {
        [NotNull]
        public Dictionary<string, string> RadioSettings { get; private set; }

        [NotNull]
        public List<ExportRadio> Radios { get; private set; }

        [NotNull]
        public List<ExportQuality> Qualities { get; private set; }

        [NotNull]
        public List<ExportScript> Scripts { get; private set; }

        [NotNull]
        public ImportExportSettings ImportExportSettings { get; private set; }

        [NotNull]
        public List<string> PlugIns { get; private set; }

        public RadioStreamerExport([NotNull] ImportExportSettings importExportSettings)
        {
            if (importExportSettings == null) throw new ArgumentNullException(nameof(importExportSettings));

            ImportExportSettings = importExportSettings;
            
            RadioSettings = new Dictionary<string, string>();
            Radios = new List<ExportRadio>();
            Qualities = new List<ExportQuality>();
            Scripts = new List<ExportScript>();
            PlugIns = new List<string>();
        }
    }
}