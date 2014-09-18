#region Usings

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Tauron.Application.Ioc;
using Tauron.Application.Models;
using Tauron.Application.RadioStreamer.Contracts;
using Tauron.Application.RadioStreamer.Contracts.Data;
using Tauron.Application.RadioStreamer.Contracts.Data.Enttitis;
using Tauron.Application.RadioStreamer.Contracts.Scripts;
using Tauron.Application.RadioStreamer.Resources;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application.RadioStreamer.Views.RadioManager.Controls
{
    [ExportViewModel(AppConstants.RadioCreateViewModel)]
    public class RadioCreateViewModel : ViewModelBase, INotifyBuildCompled
    {
        public enum ScriptType
        {
            None,
            Prepare,
            Script
        }

        [Inject] private IEngineManager _engineManager;

        private RadioEditModel _model;
        [Inject] private IRadioDatabase _radioDatabase;

        [WindowTarget("Window")] private IWindow _window;
        public static RadioEntry Entry { get; set; }

        [NotNull]
        public RadioEditModel Model
        {
            get { return _model; }
            set
            {
                _model = value;
                OnPropertyChanged();
            }
        }

        [NotNull]
        public RadioQualityManager QualityManager { get; private set; }

        [NotNull]
        public RadioScriptManager ScriptManager { get; private set; }

        void INotifyBuildCompled.BuildCompled()
        {
            if (Entry.IsEmpty) Model = new RadioEditModel {Language = CultureInfo.CurrentCulture.DisplayName};
            else
            {
                Model = new RadioEditModel
                {
                    Country = Entry.Country,
                    Description = Entry.Description,
                    Genre = Entry.Genre,
                    Language = Entry.Language,
                    Name = Entry.Name
                };
            }

            QualityManager = new RadioQualityManager(Entry);
            ScriptManager = new RadioScriptManager(_engineManager);
        }

        #region Main

        [CommandTarget]
        private void Close()
        {
            Entry = new RadioEntry();
            _window.Close();
        }

        [CommandTarget]
        private bool CanDelete()
        {
            return !Entry.IsEmpty;
        }

        [CommandTarget]
        private void Delete()
        {
            _radioDatabase.DeleteRadio(Entry.Name);
            _radioDatabase.Save();
            Entry = new RadioEntry();
            _window.Close();
        }

        [CommandTarget]
        private bool CanSave()
        {
            return !string.IsNullOrWhiteSpace(_model.Name);
        }

        [CommandTarget]
        private void Save()
        {
            bool newR;
            RadioEntry ent = _radioDatabase.GetEntryFactory().AddOrGetEntry(_model.Name, out newR);
            ent.Language = _model.Language;
            ent.Genre = _model.Genre;
            ent.Description = _model.Description;
            ent.Country = _model.Country;
            if (ScriptManager.ScriptModel != null && ScriptManager.ScriptModel.ScriptType == ScriptType.Script)
                ent.Script = ScriptManager.ScriptModel.InternalName;
            AddQualitys(ent);

            if (newR)
                RadioCache.Cache = new List<RadioEntry> {ent};
            _radioDatabase.Save();
            Entry = new RadioEntry();
            _window.Close();
        }

        #endregion

        [CommandTarget]
        private void AddQ()
        {
            string result = Dialogs.GetText(_window, RadioStreamerResources.RadioCreationAddQualityNameInstruction,
                string.Empty, "Name", true, null);

            if (string.IsNullOrWhiteSpace(result)) return;

            QualityManager.AddModel(result);
        }

        [CommandTarget]
        private bool CanRemoveQ()
        {
            return QualityManager.QualityModel != null;
        }

        [CommandTarget]
        private void RemoveQ()
        {
            QualityManager.Remove();
        }

        private void AddQualitys(RadioEntry entry)
        {
            var fac = new RadioQualityFactory(entry);

            if (entry.Qualitys != null)
                foreach (var source in entry.Qualitys.ToArray())
                {
                    fac.RemoveQuality(source.Name);
                }

            foreach (var quality in QualityManager)
            {
                fac.Create(quality.Name, quality.Url, string.Empty);
            }
        }

        public class RadioEditModel : ObservableObject
        {
            private string _country;
            private string _description;
            private string _genre;
            private string _language;
            private string _name;

            [NotNull]
            public string Name
            {
                get { return _name; }
                set
                {
                    _name = value;
                    OnPropertyChanged();
                }
            }

            [NotNull]
            public string Genre
            {
                get { return _genre; }
                set
                {
                    _genre = value;
                    OnPropertyChanged();
                }
            }

            [NotNull]
            public string Country
            {
                get { return _country; }
                set
                {
                    _country = value;
                    OnPropertyChanged();
                }
            }

            [NotNull]
            public string Language
            {
                get { return _language; }
                set
                {
                    _language = value;
                    OnPropertyChanged();
                }
            }

            [NotNull]
            public string Description
            {
                get { return _description; }
                set
                {
                    _description = value;
                    OnPropertyChanged();
                }
            }
        }

        public class RadioQualityManager : UISyncObservableCollection<RadioQualityModel>
        {
            private RadioQualityModel _qualityModel;

            public RadioQualityManager(RadioEntry entry)
            {
                if (entry.IsEmpty || entry.Qualitys == null) return;

                foreach (var quality in entry.Qualitys)
                {
                    var model = CreateQualityModel();
                    model.Name = quality.Name;
                    model.Url = quality.Url;
                    Add(model);
                }
            }

            [CanBeNull]
            public RadioQualityModel QualityModel
            {
                get { return _qualityModel; }
                set
                {
                    _qualityModel = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("QualityModel"));
                }
            }

            [NotNull]
            private RadioQualityModel CreateQualityModel()
            {
                return new RadioQualityModel(model => Remove(model));
            }

            public void AddModel([NotNull] string name)
            {
                var temp = CreateQualityModel();
                temp.Name = name;
                Add(temp);
            }

            public void Remove()
            {
                Remove(QualityModel);
            }
        }

        public class RadioQualityModel : ObservableObject
        {
            private readonly Action<RadioQualityModel> _deleteAction;

            private string _name;

            private string _url;

            public RadioQualityModel([NotNull] Action<RadioQualityModel> deleteAction)
            {
                _deleteAction = deleteAction;
            }

            [NotNull]
            public string Name
            {
                get { return _name; }
                set
                {
                    if (Equals(_name, value)) return;

                    _name = value;
                    if (string.IsNullOrWhiteSpace(value)) _deleteAction(this);

                    OnPropertyChanged();
                }
            }

            [NotNull]
            public string Url
            {
                get { return _url; }
                set
                {
                    if (Equals(_url, value)) return;

                    _url = value;

                    OnPropertyChanged();
                }
            }
        }

        public class RadioScriptManager : UISyncObservableCollection<RadioScriptModel>
        {
            private readonly IEngineManager _manager;
            private RadioScriptModel _scriptModel;

            public RadioScriptManager([NotNull] IEngineManager manager)
            {
                _manager = manager;
                Add(new RadioScriptModel(string.Empty, ScriptType.Prepare));
                Async.StartNew(BeginLoad);
            }

            [CanBeNull]
            public RadioScriptModel ScriptModel
            {
                get { return _scriptModel; }
                set
                {
                    _scriptModel = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("ScriptModel"));
                }
            }

            private void BeginLoad()
            {
                Clear();

                Add(new RadioScriptModel(string.Empty, ScriptType.None));

                foreach (var scriptName in _manager.ScriptNames)
                {
                    Add(new RadioScriptModel(scriptName, ScriptType.Script));
                }
            }
        }

        public class RadioScriptModel : ObservableObject
        {
            public RadioScriptModel([NotNull] string internalName, ScriptType scriptType)
            {
                if (internalName == null) throw new ArgumentNullException("internalName");

                InternalName = internalName;
                ScriptType = scriptType;
            }

            [NotNull]
            public string InternalName { get; private set; }

            public ScriptType ScriptType { get; private set; }

            [NotNull]
            public string Name
            {
                get
                {
                    switch (ScriptType)
                    {
                        case ScriptType.None:
                            return RadioStreamerResources.NoneString;
                        case ScriptType.Prepare:
                            return RadioStreamerResources.RadioPlayerStadeInitializing;
                        case ScriptType.Script:
                            return InternalName;
                        default:
                            return string.Empty;
                    }
                }
            }
        }
    }
}