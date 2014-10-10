using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Tauron.Application.Ioc;
using Tauron.Application.RadioStreamer.Contracts.Core;
using Tauron.Application.RadioStreamer.Contracts.Core.Attributes;
using Tauron.Application.RadioStreamer.Contracts.Data;
using Tauron.Application.RadioStreamer.Contracts.Data.Attributes;
using Tauron.Application.RadioStreamer.Contracts.Data.Enttitis;
using Tauron.Application.RadioStreamer.Resources;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Database.IEEngine
{
    [Export(typeof (IDatabaseImportExportEngine))]
    public sealed class ImportExportEngine : IDatabaseImportExportEngine
    {
        [InjectRadioEnviroment]
        private IRadioEnvironment _radioEnvironment;

        [InjectRadioDatabase]
        private IRadioDatabase _radioDatabase;

        public string FileFilter
        {
            get
            {
                return RadioStreamerResources.FileFilterAllFiles + "|*.*|" + RadioStreamerResources.FileFilterRsdFile +
                       "|*.rsd";
            }
        }

        public string DefaultExtension { get { return ".rsd"; } }

        public void ImportFiles(string file, bool merge)
        {
            if (file == null) throw new ArgumentNullException("file");

            try
            {
                RadioStreamerExport exported;

                using (var stream = new DeflateStream(new FileStream(file, FileMode.Open), CompressionMode.Decompress)) 
                    exported = (RadioStreamerExport) new BinaryFormatter().Deserialize(stream);

                var store = _radioEnvironment.Settings.PropertyStore;

                foreach (var setting in exported.Settings) store.SetName(setting.Key, setting.Value);

                if(!merge)
                    _radioDatabase.Clear();

                var fac = _radioDatabase.GetEntryFactory();

                foreach (var exportRadio in exported.Radios)
                {
                    var name = exportRadio.Entries.FirstOrDefault(e => e.Key == RadioEntry.MetaName);
                    if(name == null) continue;

                    bool cr;
                    var ent = fac.AddOrGetEntry(name.Value, out cr);
                    var meta = ent.Metadata;
                    if (meta == null) continue;

                    foreach (var metadataEntry in exportRadio.Entries)
                        meta[metadataEntry.Key] = metadataEntry.Value;

                    var qfac = new RadioQualityFactory(ent);
                    var equality = exported.Qualities.FirstOrDefault(q => q.Name == ent.Name);
                    if(equality == null) continue;

                    foreach (var qualityEntry in equality.Qualitys)
                    {
                        name = qualityEntry.Entries.FirstOrDefault(e => e.Key == RadioQuality.MetaName);
                        if(name == null) continue;

                        var qua = qfac.Create(name.Value, string.Empty, string.Empty);
                        meta = qua.Metadata;
                        if(meta == null) continue;

                        foreach (var metadataEntry in qualityEntry.Entries)
                            meta[metadataEntry.Key] = metadataEntry.Value;
                    }
                }

                _radioEnvironment.Settings.Save();
                _radioDatabase.Save();
            }
            catch (Exception e)
            {
                if (CriticalExceptions.IsCriticalException(e)) throw;
            }
        }

        public void ExportFiles(string filename)
        {
            if (filename == null) throw new ArgumentNullException("filename");
            
            var export = new RadioStreamerExport();

            var store = _radioEnvironment.Settings.PropertyStore;

            foreach (var name in store)
            {
                string value = store.GetValue(name, null);
                if (!string.IsNullOrEmpty(value)) 
                    export.Settings[name] = value;
            }

            foreach (var radioEntry in _radioDatabase.GetRadios())
            {
                var entry = new ExportRadio();
                var qentry = new ExportQuality {Name = radioEntry.Name};

                if (radioEntry.Metadata == null) continue;

                entry.Entries.AddRange(GetEntries(radioEntry.Metadata));

                foreach (var quality in radioEntry.Metadata.GetQualitys())
                {
                    var qent = new QualityEntry();
                    qent.Entries.AddRange(GetEntries(radioEntry.Metadata.GetQuality(quality)));
                    qentry.Qualitys.Add(qent);
                }

                export.Qualities.Add(qentry);
                export.Radios.Add(entry);
            }

            using (var stream = new DeflateStream(new FileStream(filename, FileMode.Create),CompressionLevel.Optimal))
            {
                new BinaryFormatter().Serialize(stream, export);
            }
        }

        [NotNull]
        private IEnumerable<MetadataEntry> GetEntries([NotNull] Metadatascope scope)
        {
            return scope.Select(key => new MetadataEntry {Key = key, Value = scope[key]});
        }
    }
}
