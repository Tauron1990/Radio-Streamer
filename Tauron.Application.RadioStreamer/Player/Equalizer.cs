﻿using Tauron.Application.RadioStreamer.Contracts.Core;
using Un4seen.Bass;

namespace Tauron.Application.RadioStreamer.Player
{
    public class Equalizer : ObservableObject, IEqualizer
    {
        private int _handle = -1;
        private int[] _bands = new int[10];
        private float[] _cenders =
        {
            31,
            63,
            125,
            250,
            500,
            1000,
            2000,
            4000,
            8000,
            16000
        };

        private bool _enabled;
        public bool Enabled
        {
            get { return _enabled; }
            set 
            {
                if (value && !_enabled)
                    Activate();
                else if (!value && _enabled)
                    Deactivate();

                _enabled = _handle != -1 && value;
                OnPropertyChanged();
            }
        }

        internal void Init(int handle)
        {
            _handle = handle;
            if (Enabled)
                Activate();
        }
        internal void Free()
        {
            _handle = -1;
            Enabled = false;
        }

        private void Activate()
        {
            if (_handle == -1) return;

            var eq = new BASS_DX8_PARAMEQ();

            for (int i = 0; i < _bands.Length; i++)
            {
                _bands[i] = Bass.BASS_ChannelSetFX(_handle, BASSFXType.BASS_FX_DX8_PARAMEQ, 0);

                eq.fCenter = _cenders[i];
				
                Bass.BASS_FXSetParameters(_bands[i], eq);
            }

            OnPropertyChangedExplicit("Band0");
            OnPropertyChangedExplicit("Band1");
            OnPropertyChangedExplicit("Band2");
            OnPropertyChangedExplicit("Band3");
            OnPropertyChangedExplicit("Band4");
            OnPropertyChangedExplicit("Band5");
            OnPropertyChangedExplicit("Band6");
            OnPropertyChangedExplicit("Band7");
            OnPropertyChangedExplicit("Band8");
            OnPropertyChangedExplicit("Band9");
        }
        private void Deactivate()
        {
            if (_handle == -1) return;

            foreach (int t in _bands)
            {
                Bass.BASS_ChannelRemoveFX(_handle, t);
            }
        }

        private void UpdateGain(int band, float gain)
        {
            var eq = new BASS_DX8_PARAMEQ();
            int handle = _bands[band];

            if (!Bass.BASS_FXGetParameters(handle, eq)) return;

            eq.fGain = gain;
            Bass.BASS_FXSetParameters(handle, eq);
        }
        private float GetGain(int band)
        {
            if (!_enabled) return 0;

            var eq = new BASS_DX8_PARAMEQ();
            int handle = _bands[band];
            return Bass.BASS_FXGetParameters(handle, eq) ? eq.fGain : 0;
        }

        public float Band0 
        {
            get { return GetGain(0); }
            set { UpdateGain(0, value); OnPropertyChanged(); } 
        }
        public float Band1
        {
            get { return GetGain(1); }
            set { UpdateGain(1, value); OnPropertyChanged(); }
        }
        public float Band2
        {
            get { return GetGain(2); }
            set { UpdateGain(2, value); OnPropertyChanged(); }
        }
        public float Band3
        {
            get { return GetGain(3); }
            set { UpdateGain(3, value); OnPropertyChanged(); }
        }
        public float Band4
        {
            get { return GetGain(4); }
            set { UpdateGain(4, value); OnPropertyChanged(); }
        }
        public float Band5
        {
            get { return GetGain(5); }
            set { UpdateGain(5, value); OnPropertyChanged(); }
        }
        public float Band6
        {
            get { return GetGain(6); }
            set { UpdateGain(6, value); OnPropertyChanged(); }
        }
        public float Band7
        {
            get { return GetGain(7); }
            set { UpdateGain(7, value); OnPropertyChanged(); }
        }
        public float Band8
        {
            get { return GetGain(8); }
            set { UpdateGain(8, value); OnPropertyChanged(); }
        }
        public float Band9
        {
            get { return GetGain(9); }
            set { UpdateGain(9, value); OnPropertyChanged(); }
        }
    }
}