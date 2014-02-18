using System;
using System.Globalization;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.BassLib
{
    [PublicAPI]
    public struct Percentage
    {
        private int _value;

        public Percentage(int percent, PercentMode mode)
        {
            switch (mode)
            {
                case PercentMode.Default:
                    _value = percent;
                    break;
                case PercentMode.ZeroOne:
                    _value = percent*100;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("mode");
            }

            Validate();
        }

        public Percentage(float percent, PercentMode mode)
        {
            switch (mode)
            {
                case PercentMode.Default:
                    _value = (int)percent;
                    break;
                case PercentMode.ZeroOne:
                    _value = (int) (percent*100);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("mode");
            }
            Validate();
        }

        public Percentage(double percent, PercentMode mode)
            : this((float)percent, mode)
        {
        
        }

        private void Validate()
        {
            if (_value < 0)
                _value = 0;
            if (_value > 100)
                _value = 100;
        }

        public float ToFloat(PercentMode mode)
        {
            switch (mode)
            {
                case PercentMode.Default:
                    return _value;
                case PercentMode.ZeroOne:
                    return _value/100f;
                default:
                    throw new ArgumentOutOfRangeException("mode");
            }
        }

        public int ToInt(PercentMode mode)
        {
            switch (mode)
            {
                case PercentMode.Default:
                    return _value;
                case PercentMode.ZeroOne:
                    return _value / 100;
                default:
                    throw new ArgumentOutOfRangeException("mode");
            }
        }

        public static implicit operator Percentage(int val)
        {
            return new Percentage(val, PercentMode.Default);
        }

        public static implicit operator Percentage(float val)
        {
            return new Percentage(val, PercentMode.ZeroOne);
        }

        public static implicit operator int(Percentage p)
        {
            return p._value;
        }

        public static implicit operator float(Percentage p)
        {
            return p._value/100f;
        }

        public override string ToString()
        {
            return _value.ToString(CultureInfo.InvariantCulture);
        }
    }
}