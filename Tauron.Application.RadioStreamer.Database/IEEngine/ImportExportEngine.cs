using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Tauron.Application.Implement;
using Tauron.Application.Ioc;
using Tauron.Application.RadioStreamer.Contracts.Core;
using Tauron.Application.RadioStreamer.Contracts.Core.Attributes;
using Tauron.Application.RadioStreamer.Contracts.Data;
using Tauron.Application.RadioStreamer.Contracts.Data.Attributes;
using Tauron.Application.RadioStreamer.Contracts.Data.Enttitis;
using Tauron.Application.RadioStreamer.Database.Database;
using Tauron.Application.RadioStreamer.Resources;

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
                    bool cr;
                    var ent = fac.AddOrGetEntry(exportRadio.Name, out cr);

                    ent.Script = exportRadio.Script;
                    ent.Language = exportRadio.Language;
                    ent.Integrated = exportRadio.Integrated;
                    ent.Genre = exportRadio.Genre;
                    ent.Description = exportRadio.Description;
                    ent.Country = exportRadio.Country;

                    var qua = exported.Qualities.FirstOrDefault(q => q.Name == ent.Name);
                    if(qua == null) continue;

                }
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
                export.Radios.Add(new ExportRadio
                {
                    Country = radioEntry.Country,
                    Description = radioEntry.Description,
                    Genre = radioEntry.Genre,
                    Integrated = radioEntry.Integrated,
                    Language = radioEntry.Language,
                    Name = radioEntry.Name,
                    Script = radioEntry.Script
                });
            }

            foreach (var quality in _radioDatabase.GetQualitys())
            {
                var q = new ExportQuality {Name = quality.RadioName};

                foreach (var name in quality.Qualitys) 
                    q.Qualitys.Add(new QualityEntry {Name = name, Url = quality.Qualitys.Read(name)});

                export.Qualities.Add(q);
            }

            using (var stream = new DeflateStream(new FileStream(filename, FileMode.Create),CompressionLevel.Optimal))
            {
                new BinaryFormatter().Serialize(stream, export);
            }
        }
    }
}
