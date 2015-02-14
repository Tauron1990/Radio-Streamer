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
using Tauron.Application.RadioStreamer.Contracts.Scripts;
using Tauron.Application.RadioStreamer.Database.IEEngine.Content;
using Tauron.Application.RadioStreamer.Resources;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Database.IEEngine
{
    [Export(typeof (IDatabaseImportExportEngine))]
    public sealed class ImportExportEngine : IDatabaseImportExportEngine
    {
        private class StepInfo
        {
            [NotNull]
            public string Text { get; private set; }

            public int Amount { get; private set; }

            public StepInfo([NotNull] string text, int amount)
            {
                if (text == null) throw new ArgumentNullException("text");
                Text = text;
                Amount = amount;
            }
        }

        private enum ReportingStepType
        {
            Plugins,
            Radios,
            Scripts,
            Settings
        }

        private class ReportingHelper
        {
            private readonly IProgress<ActiveProgress> _reporter;

            private class ReportStep
            {
                private int _currentAmount;
                
                public int TotalAmount { get; private set; }

                public double Percent
                {
                    get { return 100d / TotalAmount * _currentAmount; }
                }

                [NotNull]
                public string Text { get; private set; }

                public void Step()
                {
                    _currentAmount++;
                }

                public ReportStep([NotNull] StepInfo info)
                {
                    TotalAmount = info.Amount;
                    Text = info.Text;
                }
            }

            private readonly Dictionary<ReportingStepType, ReportStep> _entries;
            private readonly int _maximumAmount;
            private int _currentAmount;
            private ReportStep _currentStep;

            private double OverallPercent
            {
                get { return 100d / _maximumAmount * _currentAmount; }
            }

            private double Percent
            {
                get { return _currentStep.Percent; }
            }

            public void Step([CanBeNull]string message = null)
            {
                string realMessage = string.IsNullOrWhiteSpace(message) ? _currentStep.Text : message;

                _currentAmount++;
                _currentStep.Step();

                // ReSharper disable once AssignNullToNotNullAttribute
                _reporter.Report(new ActiveProgress(realMessage, Percent, OverallPercent));
            }

            public void SwitchToStep(ReportingStepType set)
            {
                _currentStep = _entries[set];
            }

            public ReportingHelper([NotNull] ImportExportSettings settings, [NotNull] IProgress<ActiveProgress> reporter, [NotNull] Func<ReportingStepType, StepInfo> getAmount)
            {
                if (reporter == null) throw new ArgumentNullException("reporter");
                _reporter = reporter;

                _entries = new Dictionary<ReportingStepType, ReportStep>();
                var types = new List<ReportingStepType>();
                
                if(settings.ProcessPlugIns) types.Add(ReportingStepType.Plugins);
                if(settings.ProcessRadios) types.Add(ReportingStepType.Radios);
                if(settings.ProcessScripts) types.Add(ReportingStepType.Scripts);
                if(settings.ProcessSettings) types.Add(ReportingStepType.Settings);

                foreach (var reportingStepType in types)
                {
                    var info = getAmount(reportingStepType);
                    _entries[reportingStepType] = new ReportStep(info);
                    _maximumAmount += info.Amount;
                }
            }
        }

        [InjectRadioEnviroment]
        private IRadioEnvironment _radioEnvironment;

        [InjectRadioDatabase]
        private IRadioDatabase _radioDatabase;

        [Inject]
        private IPlugInManager _plugInManager;

        [Inject]
        private IEngineManager _scriptEngine;

        public string FileFilter
        {
            get
            {
                return RadioStreamerResources.FileFilterAllFiles + "|*.*|" + RadioStreamerResources.FileFilterRsdFile +
                       "|*.rsd";
            }
        }

        public string DefaultExtension { get { return "rsd"; } }

        public void ImportFiles(string file, bool merge, ImportExportSettings settings, IProgress<ActiveProgress> progress)
        {
            if (file == null) throw new ArgumentNullException("file");
            if (settings == null) throw new ArgumentNullException("settings");
            if (progress == null) throw new ArgumentNullException("progress");

            try
            {
                progress.Report(new ActiveProgress(RadioStreamerResources.SettingIEProcessingFile, 0, 0));

                RadioStreamerExport exported;
                
                using (var stream = new DeflateStream(new FileStream(file, FileMode.Open), CompressionMode.Decompress)) 
                    exported = (RadioStreamerExport) new BinaryFormatter().Deserialize(stream);

                var exportSettings = exported.ImportExportSettings.Merge(settings);

                var helper = new ReportingHelper(exportSettings, progress, type =>
                {
                    switch (type)
                    {
                        case ReportingStepType.Plugins:
                            return new StepInfo(RadioStreamerResources.SettingIEProcessingPlugIns, exported.PlugIns.Count);
                        case ReportingStepType.Radios:
                            return new StepInfo(RadioStreamerResources.SettingIEProcessingRadios, exported.Radios.Count + 1);
                        case ReportingStepType.Scripts:
                            return new StepInfo(RadioStreamerResources.SettingIEProcessingScripts, exported.Scripts.Count + 1);
                        case ReportingStepType.Settings:
                            return new StepInfo(RadioStreamerResources.SettingIEProcessingSettings, exported.RadioSettings.Count);
                        default:
                            throw new ArgumentOutOfRangeException("type");
                    }
                });

                if (exportSettings.ProcessSettings)
                {
                    helper.SwitchToStep(ReportingStepType.Settings);
                    var store = _radioEnvironment.Settings.PropertyStore;

                    foreach (var setting in exported.RadioSettings)
                    {
                        helper.Step();
                        store.SetName(setting.Key, setting.Value);
                    }

                    _radioEnvironment.Settings.Save();
                    _radioEnvironment.ReloadSetting();
                }

                if (exportSettings.ProcessRadios)
                {
                    helper.SwitchToStep(ReportingStepType.Radios);
                    if (!merge) _radioDatabase.Clear();

                    var fac = _radioDatabase.GetEntryFactory();

                    fac.BeginChanging();
                    try
                    {
                        foreach (var exportRadio in exported.Radios)
                        {
                            helper.Step();
                            var name = exportRadio.Entries.FirstOrDefault(e => e.Key == RadioEntry.MetaName);
                            if (name == null) continue;

                            bool cr;
                            var ent = fac.AddOrGetEntry(name.Value, out cr);
                            var meta = ent.Metadata;
                            if (meta == null) continue;

                            foreach (var metadataEntry in exportRadio.Entries) meta[metadataEntry.Key] = metadataEntry.Value;

                            var qfac = new RadioQualityFactory(ent);
                            var equality = exported.Qualities.FirstOrDefault(q => q.Name == ent.Name);
                            if (equality == null) continue;

                            foreach (var qualityEntry in equality.Qualitys)
                            {
                                name = qualityEntry.Entries.FirstOrDefault(e => e.Key == RadioQuality.MetaName);
                                if (name == null) continue;

                                var qua = qfac.Create(name.Value, string.Empty, string.Empty);
                                meta = qua.Metadata;
                                if (meta == null) continue;

                                foreach (var metadataEntry in qualityEntry.Entries) meta[metadataEntry.Key] = metadataEntry.Value;
                            }
                        }
                    }
                    finally
                    {
                        helper.Step(RadioStreamerResources.SettingIEProcessingDatabaseSave);
                        fac.Compled();
                    }
                }

                if (exportSettings.ProcessScripts)
                {
                    helper.SwitchToStep(ReportingStepType.Scripts);

                    foreach (var script in exported.Scripts)
                    {
                        helper.Step();
                        _scriptEngine.CopyFile(script.FileName, script.Content);
                    }

                    helper.Step(RadioStreamerResources.SettingIEProcessingPrecompile);
                    _scriptEngine.PreCompile(new StringWriter());
                }

                if (!exportSettings.ProcessPlugIns) return;

                helper.SwitchToStep(ReportingStepType.Plugins);
                var installedPlagIns = new List<string>(_plugInManager.GetInstalledAddIns().Select(p => p.Name));

                foreach (var plugIn in exported.PlugIns.Where(p => !installedPlagIns.Contains(p)))
                {
                    helper.Step();
                    _plugInManager.InstallPlugIn(plugIn);
                }
            }
            catch (Exception e)
            {
                if (CriticalExceptions.IsCriticalException(e)) throw;
            }
        }

        public void ExportFiles(string filename, ImportExportSettings settings, IProgress<ActiveProgress> progress)
        {
            if (filename == null) throw new ArgumentNullException("filename");
            if (settings == null) throw new ArgumentNullException("settings");
            if (progress == null) throw new ArgumentNullException("progress");

            var helper = new ReportingHelper(settings, progress, type =>
            {
                switch (type)
                {
                    case ReportingStepType.Plugins:
                        return new StepInfo(RadioStreamerResources.SettingIEProcessingPlugIns,
                                            _plugInManager.GetInstalledAddIns().Count());
                    case ReportingStepType.Radios:
                        return new StepInfo(RadioStreamerResources.SettingIEProcessingRadios, _radioDatabase.Count);
                    case ReportingStepType.Scripts:
                        return new StepInfo(RadioStreamerResources.SettingIEProcessingScripts, _scriptEngine.FileNames.Count());
                    case ReportingStepType.Settings:
                        return new StepInfo(RadioStreamerResources.SettingIEProcessingSettings, _radioEnvironment.Settings.PropertyStore.Count);
                    default:
                        throw new ArgumentOutOfRangeException("type");
                }
            });

            var export = new RadioStreamerExport(settings);

            var store = _radioEnvironment.Settings.PropertyStore;

            if (settings.ProcessSettings)
            {
                helper.SwitchToStep(ReportingStepType.Settings);
                foreach (var name in store)
                {
                    helper.Step();
                    string value = store.GetValue(name, null);
                    if (!string.IsNullOrEmpty(value)) export.RadioSettings[name] = value;
                }
            }

            if (settings.ProcessRadios)
            {
                helper.SwitchToStep(ReportingStepType.Radios);
                foreach (var radioEntry in _radioDatabase.GetRadios())
                {
                    helper.Step();
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
            }

            if (settings.ProcessPlugIns)
            {
                helper.SwitchToStep(ReportingStepType.Plugins);
                foreach (var installedPackInfo in _plugInManager.GetInstalledAddIns())
                {
                    helper.Step();
                    export.PlugIns.Add(installedPackInfo.Name);
                }
            }

            if (settings.ProcessScripts)
            {
                helper.SwitchToStep(ReportingStepType.Scripts);
                foreach (var scriptFile in _scriptEngine.FileNames)
                {
                    helper.Step();
                    export.Scripts.Add(new ExportScript(scriptFile.GetFileName(), scriptFile.ReadAllBytesIfExis()));
                }
            }

            progress.Report(new ActiveProgress(RadioStreamerResources.SettingIEProcessingFile, 0, 100));

            using (var stream = new DeflateStream(new FileStream(filename, FileMode.Create),CompressionLevel.Optimal))
            {
                new BinaryFormatter().Serialize(stream, export);
            }
        }

        [NotNull]
        private static IEnumerable<MetadataEntry> GetEntries([NotNull] Metadatascope scope)
        {
            return scope.Select(key => new MetadataEntry {Key = key, Value = scope[key]});
        }
    }
}
