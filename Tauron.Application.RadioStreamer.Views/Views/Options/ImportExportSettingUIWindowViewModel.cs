using Tauron.Application.Models;
using Tauron.Application.RadioStreamer.Contracts;
using Tauron.Application.RadioStreamer.Contracts.Data;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Views.Options
{
    [ExportViewModel(AppConstants.ImportExportSettingsWindow)]
    public sealed class ImportExportSettingUIWindowViewModel : ViewModelBase, IResultProvider
    {
        public object Result
        {
            get { return new ImportExportSettings(ExportRadios, ExportSettings, ExportPlugIns, ExportScripts); }
        }

        public bool ExportSettings { get; set; }

        public bool ExportRadios { get; set; }

        public bool ExportScripts { get; set; }

        public bool ExportPlugIns { get; set; }

        public ImportExportSettingUIWindowViewModel()
        {
            ExportPlugIns = true;
            ExportRadios = true;
            ExportScripts = true;
            ExportSettings = true;
        }

        [NotNull,WindowTarget]
        public IWindow Window { get; set; }

        [CommandTarget]
        public void Ok()
        {
            Window.DialogResult = true;
        }

        [CommandTarget]
        public void Abort()
        {
            Window.DialogResult = false;
        }
    }
}
