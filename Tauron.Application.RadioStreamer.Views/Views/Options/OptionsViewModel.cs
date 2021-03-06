﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Tauron.Application.Ioc;
using Tauron.Application.Models;
using Tauron.Application.RadioStreamer.Contracts;
using Tauron.Application.RadioStreamer.Contracts.Data;
using Tauron.Application.RadioStreamer.Contracts.UI;
using Tauron.Application.RadioStreamer.Resources;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Views.Options
{
    [ExportViewModel(AppConstants.OptionsViewModel)]
    public class OptionsViewModel : ViewModelBase
    {
        [Inject] 
        private IUIOptionsManager _model;
        
        [Inject]
        private IDatabaseImportExportEngine _exportEngine;


        [NotNull]
        public IEnumerable<OptionPath> OptionPaths => _model.Options;

        [WindowTarget("Window")]
        private IWindow _window;

        [NotNull]
        public UISyncObservableCollection<Option> SelectedOptions { get; }

        public OptionsViewModel()
        {
            SelectedOptions = new UISyncObservableCollection<Option>();
        }

        [EventTarget]
        private void NewItemSelect([NotNull] RoutedPropertyChangedEventArgs<object> newValue)
        {
            IEnumerable<Option> options;
            var path = newValue.NewValue as OptionPath;
            if (path != null)
                options = path.Elements.OfType<Option>();
            else
            {
                var group = newValue.NewValue as OptionGroup;
                options = @group?.Options ?? Enumerable.Empty<Option>();
            }

            SelectedOptions.Clear();
            SelectedOptions.AddRange(options);
        }

        [CommandTarget]
        private void Close()
        {
            _model.Reset();
            _window.Close();
        }

        [CommandTarget]
        private void Save()
        {
            _model.Save();
            _window.Close();
        }

        [NotNull]
        private string OpenFileDialog(bool forSave, out bool? ok)
        {
            string erg;

            if (forSave)
            {
                erg = Dialogs.ShowSaveFileDialog(_window, true, false, true, _exportEngine.DefaultExtension, true,
                    _exportEngine.FileFilter, true, true, RadioStreamerResources.SaveExportLabel,
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), out ok);
            }
            else
            {
                erg = Dialogs.ShowOpenFileDialog(_window, true, _exportEngine.DefaultExtension, true,
                    _exportEngine.FileFilter, false, RadioStreamerResources.LoadImportLabel, true, true, out ok)
                    .FirstOrDefault();
            }

            return erg ?? string.Empty;
        }

        [CommandTarget]
        private void Export()
        {
            bool? ok;
            string file = OpenFileDialog(true, out ok);

            if (ok != true) return;

            GetSettings().ContinueWith(t =>
            {
                var result = t.Result;
                if (result == null) return;

                StartProcessDialog(p => _exportEngine.ExportFiles(file, result, p));
            });
        }

        [CommandTarget]
        private void Import()
        {
            bool? ok;
            string file = OpenFileDialog(false, out ok);

            if(ok != true) return;

            GetSettings().ContinueWith(t =>
            {
                var result = t.Result;
                if(result == null) return;

                StartProcessDialog(p =>
                {
                    _exportEngine.ImportFiles(file, false, result, p);
                    _model.Deserialize();
                    _model.Reset();
                });
            });
        }

        [NotNull]
        private Task<ImportExportSettings> GetSettings()
        {
            var win = ViewManager.CreateWindow(AppConstants.ImportExportSettingsWindow);
            return win.ShowDialogAsync(MainWindow)
               .ContinueWith(t =>
               {
                   if (win.DialogResult == true) return (ImportExportSettings) win.Result;
                   return null;
               });
        }

        private void StartProcessDialog([NotNull] Action<IProgress<ActiveProgress>> worker)
        {
            if (worker == null) throw new ArgumentNullException("worker");

            string text = RadioStreamerResources.ImportExportProcessingTitle;
            var diag = Dialogs.CreateProgressDialog(text, text, MainWindow, worker);
            diag.Start();
        }
    }
}