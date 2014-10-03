using System;
using System.Linq;
using Tauron.Application.Implement;
using Tauron.Application.Ioc;
using Tauron.Application.RadioStreamer.Contracts.Core;
using Tauron.Application.RadioStreamer.Contracts.Core.Attributes;
using Tauron.Application.RadioStreamer.Contracts.Data;
using Tauron.Application.RadioStreamer.Contracts.Data.Attributes;
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
        ???
        }

        public void ExportFiles(string filename)
        {
            if (filename == null) throw new ArgumentNullException("filename");
            
            var export = new RadioStreamerExport();

            var store = _radioEnvironment.Settings.PropertyStore;
        }
    }
}
