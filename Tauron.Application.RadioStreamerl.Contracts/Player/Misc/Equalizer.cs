namespace Tauron.Application.RadioStreamer.Contracts.Player.Misc
{
    public sealed class Equalizer : ObservableObject
    {
        private float _band0;
        private float _band1;
        private float _band2;
        private float _band3;
        private float _band4;
        private float _band5;
        private float _band6;
        private float _band7;
        private float _band8;
        private float _band9;
        private bool _enabled;

        public float Band0
        {
            get { return _band0; }
            set { _band0 = value; OnPropertyChanged();}
        }

        public float Band1
        {
            get { return _band1; }
            set { _band1 = value; OnPropertyChanged();}
        }

        public float Band2
        {
            get { return _band2; }
            set { _band2 = value; OnPropertyChanged();}
        }

        public float Band3
        {
            get { return _band3; }
            set { _band3 = value; OnPropertyChanged();}
        }

        public float Band4
        {
            get { return _band4; }
            set { _band4 = value; OnPropertyChanged();}
        }

        public float Band5
        {
            get { return _band5; }
            set { _band5 = value; OnPropertyChanged();}
        }

        public float Band6
        {
            get { return _band6; }
            set { _band6 = value; OnPropertyChanged();}
        }

        public float Band7
        {
            get { return _band7; }
            set { _band7 = value; OnPropertyChanged();}
        }

        public float Band8
        {
            get { return _band8; }
            set { _band8 = value; OnPropertyChanged();}
        }

        public float Band9
        {
            get { return _band9; }
            set { _band9 = value; OnPropertyChanged();}
        }

        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; OnPropertyChanged();}
        }
    }
}