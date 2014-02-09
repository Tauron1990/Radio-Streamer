using System.Collections.Generic;
using System.Linq;
using Tauron.Application.BassLib.Misc;
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
        //private class EqalizerTest : ObservableObject, IEqualizer
        //{
        //    private float _band0;
        //    private float _band1;
        //    private float _band2;
        //    private float _band3;
        //    private float _band4;
        //    private float _band5;
        //    private float _band6;
        //    private float _band7;
        //    private float _band8;
        //    private float _band9;
        //    private bool _enabled;

        //    public float Band0
        //    {
        //        get { return _band0; }
        //        set { _band0 = value; OnPropertyChanged();}
        //    }

        //    public float Band1
        //    {
        //        get { return _band1; }
        //        set { _band1 = value; OnPropertyChanged(); }
        //    }

        //    public float Band2
        //    {
        //        get { return _band2; }
        //        set { _band2 = value; OnPropertyChanged(); }
        //    }

        //    public float Band3
        //    {
        //        get { return _band3; }
        //        set { _band3 = value; OnPropertyChanged(); }
        //    }

        //    public float Band4
        //    {
        //        get { return _band4; }
        //        set { _band4 = value; OnPropertyChanged();}
        //    }

        //    public float Band5
        //    {
        //        get { return _band5; }
        //        set { _band5 = value; OnPropertyChanged();}
        //    }

        //    public float Band6
        //    {
        //        get { return _band6; }
        //        set { _band6 = value; OnPropertyChanged();}
        //    }

        //    public float Band7
        //    {
        //        get { return _band7; }
        //        set { _band7 = value; OnPropertyChanged(); }
        //    }

        //    public float Band8
        //    {
        //        get { return _band8; }
        //        set { _band8 = value; OnPropertyChanged(); }
        //    }

        //    public float Band9
        //    {
        //        get { return _band9; }
        //        set { _band9 = value; OnPropertyChanged(); }
        //    }

        //    public bool Enabled
        //    {
        //        get { return _enabled; }
        //        set { _enabled = value; OnPropertyChanged(); }
        //    }
        //}

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
                base.InsertItem(index,  _newPresent);
            }
        }

        private IEqualizerProfileDatabase _equalizerProfileDatabase;

        [Inject]
        public EqManagerViewModel([NotNull] IRadioEnvironment environment, [NotNull] IRadioPlayer player)
        {
            Equalizer = player.Equalizer;
            //Equalizer = new EqalizerTest();
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
        public UISyncObservableCollection<EqManagerPresent> Presets { get; private set; }

        private EqManagerPresent _currentPresent;

        [CanBeNull]
        public EqManagerPresent CurrentPresent
        {
            get { return _currentPresent; }
            set
            {
                if (Equals(_currentPresent, value)) return;

                UpdatePreset(value);
                _currentPresent = value;

                OnPropertyChanged();
            }
        }

        private void UpdatePreset([CanBeNull] EqManagerPresent value)
        {
            if (value == null || value.PresetType == EqManagerPresetType.Newlabel)
            {
                if (_currentPresent == null || CurrentPresentName != _currentPresent.Name) return;
                
                CurrentPresentName = string.Empty;
                Equalizer.Band0 = 0;
                Equalizer.Band1 = 0;
                Equalizer.Band2 = 0;
                Equalizer.Band3 = 0;
                Equalizer.Band4 = 0;
                Equalizer.Band5 = 0;
                Equalizer.Band6 = 0;
                Equalizer.Band7 = 0;
                Equalizer.Band8 = 0;
                Equalizer.Band9 = 0;
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
                    _equalizerProfileDatabase.DeleteProfile(CurrentPresent.OriginalName);
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
            {
                _equalizerProfileDatabase.DeleteProfile(CurrentPresent.Name);
                Presets.Remove(CurrentPresent);
            }
            CurrentPresent = null;
        }
    }
}