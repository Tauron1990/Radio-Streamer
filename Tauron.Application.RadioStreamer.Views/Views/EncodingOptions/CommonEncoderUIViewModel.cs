using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using Tauron.Application.Ioc;
using Tauron.Application.Models;
using Tauron.Application.RadioStreamer.Contracts;
using Tauron.Application.RadioStreamer.Contracts.Core;
using Tauron.Application.RadioStreamer.Contracts.Core.Attributes;
using Tauron.Application.RadioStreamer.Contracts.Data;
using Tauron.Application.RadioStreamer.Contracts.Player.Recording;
using Tauron.Application.RadioStreamer.Contracts.UI;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Views.EncodingOptions
{
    [ExportViewModel(AppConstants.CommonEncoderUI)]
    public class CommonEncoderUIViewModel : ViewModelBase, IEncoderViewModel
    {
        [InjectRadioEnviroment]
        private IRadioEnvironment _radioEnvironment;

        //[Inject(typeof(UserControl), ContractName = "EncodingEditor")]
        //private List<InstanceResolver<object, IEncodingEditorMetadata>> _encoderEditors;
        
        private List<Tuple<string, CommonProfile>> _modifed = new List<Tuple<string, CommonProfile>>();
        private string _currentProfile;

        [InjectModel(AppConstants.CommonEncoderUI)]
        private CommonEncodingEditorModel _editorModel;

        private object _currentEditor;

        [Inject(typeof(UserControl), ContractName = "EncodingEditor")]
        private List<InstanceResolver<UserControl, IEncodingEditorMetadata>> _editors;
       
        [NotNull]
        public UISyncObservableCollection<string> Profiles { get; private set; }

        [CanBeNull]
        public string CurrentProfile
        {
            get { return _currentProfile; }
            set
            {
                _currentProfile = value;
                OnPropertyChanged();
                SwitchProfile();
            }
        }

        private void SwitchProfile()
        {
            _editorModel.Name = _currentProfile;
            _editorModel.CurrentProfile = _radioEnvironment.Settings.EncoderProfiles.Deserialize(_currentProfile);

            if (_editorModel.CurrentProfile != null)
            {
                CurrentEditor =
                    Synchronize.Invoke(
                        () => _editors.First(e => e.Metadata.EncoderId == _editorModel.CurrentProfile.Id).Resolve());
                _modifed.Add(Tuple.Create(_editorModel.Name, _editorModel.CurrentProfile));
            }
            else CurrentEditor = null;
        }

        [CanBeNull]
        public object CurrentEditor
        {
            get { return _currentEditor; }
            set
            {
                _currentEditor = value;
                OnPropertyChanged();
            }
        }

        [CommandTarget]
        private bool CanDelete()
        {
            return !string.IsNullOrWhiteSpace(_currentProfile);
        }

        [CommandTarget]
        private void Delete()
        {
            _radioEnvironment.Settings.EncoderProfiles.Delete(_currentProfile);
            Profiles.Remove(_currentProfile);
            CurrentProfile = null;
        }

        [CommandTarget]
        private void New()
        {
            var view = ViewManager.CreateWindow(AppConstants.NewEncodingProfileView);
            view.ShowDialog(MainWindow).ContinueWith(t =>
            {
                if (view.DialogResult == false) return;
                var result = view.Result as Tuple<string, CommonProfile>;
                if (result == null) return;

                _radioEnvironment.Settings.EncoderProfiles.Serialize(result.Item1, result.Item2);
                Profiles.Add(result.Item1);
                CurrentProfile = result.Item1;
            });
        }

        public void Serialize()
        {
            foreach (var commonProfile in _modifed)
            {
                _radioEnvironment.Settings.EncoderProfiles.Serialize(commonProfile.Item1, commonProfile.Item2);
            }

            _modifed.Clear();
        }

        public void Deserialize()
        {
            Profiles = new UISyncObservableCollection<string>(_radioEnvironment.Settings.EncoderProfiles.Profiles);
        }

        public void Reset()
        {
            CurrentProfile = null;
        }
    }
}