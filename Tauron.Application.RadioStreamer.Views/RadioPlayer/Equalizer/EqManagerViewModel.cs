using System.Collections.Generic;
using System.Linq;
using Tauron.Application.Ioc;
using Tauron.Application.Models;
using Tauron.Application.RadioStreamer.Contracts;
using Tauron.Application.RadioStreamer.Contracts.Core;
using Tauron.Application.RadioStreamer.Contracts.Player;
using Tauron.Application.RadioStreamer.Resources;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Views.RadioPlayer.Equalizer
{
    [ExportViewModel(AppConstants.EqManagerViewModel)]
    public class EqManagerViewModel : ViewModelBase
    {
        public class PresetCollection : UISyncObservableCollection<EqManagerPresent>
        {
            private readonly EqManagerPresent _newPresent = new EqManagerPresent("<-" + RadioStreamerResources.NewLabel + "->")
            {
                OriginalName = RadioStreamerResources.NewLabel, 
                PresetType = EqManagerPresetType.Newlabel
            };

            public PresetCollection([NotNull] IEnumerable<EqManagerPresent> @select)
                : base(@select)
            {
                Items.Add(_newPresent);
            }

            protected override void InsertItem(int index, [CanBeNull] EqManagerPresent item)
            {
                if (item == null) return;
                if (item == _newPresent) return;

                if (Items.Count == index) Remove(_newPresent);
                base.InsertItem(index - 1, item);
                Add(_newPresent);
            }
        }

        private IEqualizerProfileDatabase _equalizerProfileDatabase;

        [Inject]
        public EqManagerViewModel([NotNull] IRadioEnvironment environment, [NotNull] IRadioPlayer player)
        {
            Equalizer = player.GetEqualizer();
            _equalizerProfileDatabase = environment.OpenSettings().EqualizerDatabase;

            Presets =
                new PresetCollection(
                    _equalizerProfileDatabase.Profiles.Select(str => new EqManagerPresent(str)));
        }

        [NotNull]
        public IEqualizer Equalizer { get; private set; }

        private string _currentPresentName;

        [NotNull]
        public string CurrentPresentName
        {
            get { return _currentPresentName; }
            set
            {
                if (Equals(_currentPresentName, value)) return;

                UpdateEqName(_currentPresentName, value);
                _currentPresentName = value;

                OnPropertyChanged();
            }
        }

        private void UpdateEqName([NotNull] string currentPresentName, [NotNull] string value)
        {
            if (CurrentPresent == null || CurrentPresent.PresetType == EqManagerPresetType.Newlabel) return;

            if (CurrentPresent.Name == currentPresentName) CurrentPresent.Name += "*";
            else CurrentPresent.Name = CurrentPresent.OriginalName;
        }

        [NotNull]
        public UISyncObservableCollection<EqManagerPresent> Presets
        {
            get { return _presets; }
            private set { _presets = value; }
        }

        private EqManagerPresent _currentPresent;
        private UISyncObservableCollection<EqManagerPresent> _presets;

        [CanBeNull]
        public EqManagerPresent CurrentPresent
        {
            get { return _currentPresent; }
            set
            {
                if (Equals(_currentPresent, value)) return;

                _currentPresent = value;

                UpdatePreset(value);

                OnPropertyChanged();
            }
        }

        private void UpdatePreset([CanBeNull] EqManagerPresent value)
        {
            if (value == null || value.PresetType == EqManagerPresetType.Newlabel)
            {
                CurrentPresentName = string.Empty;
                return;
            }

            CurrentPresentName = value.Name;
            _equalizerProfileDatabase.SetProfil(value.OriginalName, Equalizer);
        }

        [CommandTarget]
        private bool CanSave()
        {
            return CurrentPresent != null && !string.IsNullOrWhiteSpace(CurrentPresentName) &&
                   Presets.Where(pr => pr != CurrentPresent).All(pr => pr.Name != CurrentPresentName);
        }

        [CommandTarget]
        private void Save()
        {
            if(CurrentPresent == null) return;

            switch (CurrentPresent.PresetType)
            {
                case EqManagerPresetType.Preset:
                    _equalizerProfileDatabase.NewProfile(CurrentPresentName, Equalizer);
                    CurrentPresent.Name = CurrentPresentName;
                    CurrentPresent.OriginalName = CurrentPresentName;
                    break;
                case EqManagerPresetType.Newlabel:
                    _equalizerProfileDatabase.NewProfile(CurrentPresentName, Equalizer);

                    var preset = new EqManagerPresent(CurrentPresentName);
                    Presets.Add(preset);

                    CurrentPresent = preset;
                    break;
            }
        }

        [CommandTarget]
        private bool CanDelete()
        {
            return CurrentPresent != null && CurrentPresent.PresetType != EqManagerPresetType.Newlabel;
        }

        [CommandTarget]
        private void Delete()
        {
            if (CurrentPresent != null)   
                _equalizerProfileDatabase.DeleteProfile(CurrentPresent.Name);
            CurrentPresent = null;
        }
    }
}