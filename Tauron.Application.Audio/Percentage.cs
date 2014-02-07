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

        private void Validate()
        {
            if (_value < 0)
                _value = 0;
            if (_value < 100)
                _value = 100;
        }

        public static implicit operator Percentage(int val)
        {
            return new Percentage(val, val < 1 ? PercentMode.Default);
        }

        public override string ToString()
        {
            return _value.ToString(CultureInfo.InvariantCulture);
        }
    }
}