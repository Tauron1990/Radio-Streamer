using System;
using System.ComponentModel;
using System.Windows.Data;
using Tauron.Application.Composition;
using Tauron.Application.RadioStreamer.Contracts.Core;

namespace Tauron.Application.RadioStreamer.Views.RadioPlayer.Controls
{
    public sealed class RadioEqProfileManagerViewModel : ObservableObject
    {
        private class DummyEqualizer : ObservableObject, IEqualizer
        {
            private float _band0;
            public float Band0
            {
                get { return _band0; }
                set
                {
                    if (_band0 == value) return;

                    _band0 = value;
                    OnPropertyChanged();
                }
            }
            private float _band1;
            public float Band1
            {
                get { return _band1; }
                set
                {
                    if (_band1 == value) return;

                    _band1 = value;
                    OnPropertyChanged();
                }
            }
            private float _band2;
            public float Band2
            {
                get { return _band2; }
                set
                {
                    if (_band2 == value) return;

                    _band2 = value;
                    OnPropertyChanged();
                }
            }
            private float _band3;
            public float Band3
            {
                get { return _band3; }
                set
                {
                    if (_band3 == value) return;

                    _band3 = value;
                    OnPropertyChanged();
                }
            }
            private float _band4;
            public float Band4
            {
                get { return _band4; }
                set
                {
                    if (_band4 == value) return;

                    _band4 = value;
                    OnPropertyChanged();
                }
            }
            private float _band5;
            public float Band5
            {
                get { return _band5; }
                set
                {
                    if (_band5 == value) return;

                    _band5 = value;
                    OnPropertyChanged();
                }
            }
            private float _band6;
            public float Band6
            {
                get { return _band6; }
                set
                {
                    if (_band6 == value) return;

                    _band6 = value;
                    OnPropertyChanged();
                }
            }
            private float _band7;
            public float Band7
            {
                get { return _band7; }
                set
                {
                    if (_band7 == value) return;

                    _band7 = value;
                    OnPropertyChanged();
                }
            }
            private float _band8;
            public float Band8
            {
                get { return _band8; }
                set
                {
                    if (_band8 == value) return;

                    _band8 = value;
                    OnPropertyChanged();
                }
            }
            private float _band9;
            public float Band9
            {
                get { return _band9; }
                set
                {
                    if (_band9 == value) return;

                    _band9 = value;
                    OnPropertyChanged();
                }
            }

            public bool Enabled
            {
                get
                {
                    return true;
                }
                set
                {
                }
            }
        }

        private IEqualizerProfileDatabase _database;

        public RadioEqProfileManagerViewModel()
        {

            _database = CompositionServices.Container.Resolve<IRadioEnvironment>()
                                           .OpenSettings().EqualizerDatabase;
            _profiles = CurrentDispatcher.Invoke(() => CollectionViewSource.GetDefaultView(_database.Profiles));

            CurrentSaveLabel = Resources.RadioStreamerResources.NewLabel;
            _equalizer = new DummyEqualizer();
        }

        [EventTarget]
        private void CurrentChanged()
        {
            ProfileName = _profiles.CurrentItem as string;
            if (ProfileName == null)
            {
                CurrentSaveLabel = Resources.RadioStreamerResources.NewLabel;
                ClearEqualizer();
            }
            else
            {
                CurrentSaveLabel = Resources.RadioStreamerResources.SaveLabel;
                _database.SetProfil(ProfileName, _equalizer);
            }
        }

        private ICollectionView _profiles;
        public ICollectionView Profiles
        {
            get { return _profiles; }
            set
            {
                if (_profiles == value) return;

                _profiles = value;
                OnPropertyChanged();
            }
        }

        private IEqualizer _equalizer;
        public IEqualizer Equalizer
        {
            get { return _equalizer; }
            set
            {
                if (_equalizer == value) return;

                _equalizer = value;
                OnPropertyChanged();
            }
        }

        private string _profileName;
        public string ProfileName
        {
            get { return _profileName; }
            set
            {
                if (_profileName == value) return;

                _profileName = value;
                OnPropertyChanged();
            }
        }

        private string _currentSaveLabel;
        public string CurrentSaveLabel
        {
            get { return _currentSaveLabel; }
            set
            {
                if (_currentSaveLabel == value) return;

                _currentSaveLabel = value;
                OnPropertyChanged();
            }
        }

        private string _currentItem;
        public string CurrentItem
        {
            get { return _currentItem; }
            set
            {
                if (_currentItem == value) return;

                CurrentChanged();

                _currentItem = value;
                OnPropertyChanged();
            }
        }


        [CommandTarget]
        private bool CanSave()
        {
            return !String.IsNullOrWhiteSpace(ProfileName);
        }
        [CommandTarget(Synchronize = true)]
        private void Save()
        {
            _database.NewProfile(ProfileName, _equalizer);
            CurrentItem = null;
            ProfileName = null;
            ClearEqualizer();

        }

        private void ClearEqualizer()
        {
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
        }
    }
}