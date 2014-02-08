using Un4seen.Bass;

namespace Tauron.Application.BassLib.Misc
{
    public class Equalizer : ObservableObject, IEqualizer
    {
        private BandValue[] _bands = new BandValue[10];

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
        private int _handle = -1;

        public Equalizer()
        {
            for (int i = 0; i < _bands.Length; i++)
            {
                _bands[i] = new BandValue();
            }
        }

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

        public float Band0
        {
            get { return GetGain(0); }
            set
            {
                UpdateGain(0, value);
                OnPropertyChanged();
            }
        }

        public float Band1
        {
            get { return GetGain(1); }
            set
            {
                UpdateGain(1, value);
                OnPropertyChanged();
            }
        }

        public float Band2
        {
            get { return GetGain(2); }
            set
            {
                UpdateGain(2, value);
                OnPropertyChanged();
            }
        }

        public float Band3
        {
            get { return GetGain(3); }
            set
            {
                UpdateGain(3, value);
                OnPropertyChanged();
            }
        }

        public float Band4
        {
            get { return GetGain(4); }
            set
            {
                UpdateGain(4, value);
                OnPropertyChanged();
            }
        }

        public float Band5
        {
            get { return GetGain(5); }
            set
            {
                UpdateGain(5, value);
                OnPropertyChanged();
            }
        }

        public float Band6
        {
            get { return GetGain(6); }
            set
            {
                UpdateGain(6, value);
                OnPropertyChanged();
            }
        }

        public float Band7
        {
            get { return GetGain(7); }
            set
            {
                UpdateGain(7, value);
                OnPropertyChanged();
            }
        }

        public float Band8
        {
            get { return GetGain(8); }
            set
            {
                UpdateGain(8, value);
                OnPropertyChanged();
            }
        }

        public float Band9
        {
            get { return GetGain(9); }
            set
            {
                UpdateGain(9, value);
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
                _bands[i].Handle = Bass.BASS_ChannelSetFX(_handle, BASSFXType.BASS_FX_DX8_PARAMEQ, 0);

                eq.fCenter = _cenders[i];

                Bass.BASS_FXSetParameters(_bands[i].Handle, eq);
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

            foreach (BandValue t in _bands)
            {
                Bass.BASS_ChannelRemoveFX(_handle, t.Handle);
            }
        }

        private void UpdateGain(int band, float gain)
        {
            var eq = new BASS_DX8_PARAMEQ();
            BandValue bandVal = _bands[band];
            bandVal.Gain = gain;

            if (!_enabled) return;

            int handle = bandVal.Handle;

            if (!Bass.BASS_FXGetParameters(handle, eq)) return;

            eq.fGain = gain;
            Bass.BASS_FXSetParameters(handle, eq);
        }

        private float GetGain(int band)
        {
            BandValue bandValue = _bands[band];

            if (!_enabled) return bandValue.Gain;

            var eq = new BASS_DX8_PARAMEQ();

            int handle = bandValue.Handle;
            return Bass.BASS_FXGetParameters(handle, eq) ? eq.fGain : bandValue.Gain;
        }

        private class BandValue
        {
            public int Handle { get; set; }
            public float Gain { get; set; }
        }
    }
}