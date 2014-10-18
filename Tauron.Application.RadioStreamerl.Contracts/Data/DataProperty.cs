using System;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Contracts.Data
{
    public class DataProperty : IEquatable<DataProperty>
    {
        public bool Equals([CanBeNull] DataProperty other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Key, other.Key) && string.Equals(Value, other.Value);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Key.GetHashCode()*397) ^ Value.GetHashCode();
            }
        }

        public static bool operator ==(DataProperty left, DataProperty right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(DataProperty left, DataProperty right)
        {
            return !Equals(left, right);
        }

        [NotNull]
        public string Key { get; private set; }

        [NotNull]
        public string Value { get; set; }

        public DataProperty([NotNull] string key, [NotNull] string value)
        {
            if (key == null) throw new ArgumentNullException("key");
            if (value == null) throw new ArgumentNullException("value");

            Key = key;
            Value = value;
        }

        public override bool Equals([CanBeNull] object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((DataProperty) obj);
        }
    }
}