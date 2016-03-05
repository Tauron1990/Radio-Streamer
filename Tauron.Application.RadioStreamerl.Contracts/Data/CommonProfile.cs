using System;
using System.Collections.Generic;
using System.Linq;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Contracts.Data
{
    public sealed class CommonProfile : IEquatable<CommonProfile>
    {
        private static bool CompareLists([NotNull] List<DataProperty> prop, [NotNull] List<DataProperty> prop2)
        {
            if (prop.Count != prop2.Count) return false;

            return !prop.Where((t, i) => t != prop2[i]).Any();
        }

        public bool Equals([CanBeNull] CommonProfile other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Id, other.Id) && CompareLists(Properties, other.Properties);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Id.GetHashCode()*397) ^ Properties.GetHashCode();
            }
        }

        public static bool operator ==(CommonProfile left, CommonProfile right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CommonProfile left, CommonProfile right)
        {
            return !Equals(left, right);
        }

        [NotNull]
        public string Id { get; private set; }

        public CommonProfile([NotNull] string id)
        {
            if (id == null) throw new ArgumentNullException("id");
            
            Id = id;
            Properties = new List<DataProperty>();
        }

        [NotNull]
        public List<DataProperty> Properties { get; private set; }

        public void TryGetProperty<TType>([NotNull] Func<string, TType> converter, [NotNull] Action<TType> getter,
            [NotNull] string name)
        {
            if (converter == null) throw new ArgumentNullException("converter");
            if (getter == null) throw new ArgumentNullException("getter");
            if (name == null) throw new ArgumentNullException("name");

            DataProperty prop = Properties.Find(p => p.Key == name);
            if (prop == null) return;

            getter(converter(prop.Value));
        }

        public TType TryGetProperty<TType>([NotNull] Func<string, TType> converter,[NotNull] string name, out bool found)
        {
            if (converter == null) throw new ArgumentNullException("converter");
            if (name == null) throw new ArgumentNullException("name");

            DataProperty prop = Properties.Find(p => p.Key == name);
            if (prop == null)
            {
                found = false;
                return default(TType);
            }
            
            found = true;
            return converter(prop.Value);
        }

        public void SetProperty<TType>([NotNull]TType vaule, [NotNull] string name, [CanBeNull]Func<TType, string> converter = null)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            Func<TType, string> realConverter = converter;
            if (converter == null)
                realConverter = type => type.ToString();

            DataProperty prop = Properties.Find(p => p.Key == name);
            if (prop == null)
            {
                prop = new DataProperty(name, realConverter(vaule));
                Properties.Add(prop);
            }
            else
                prop.Value = realConverter(vaule);

        }


        public override bool Equals([CanBeNull] object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is CommonProfile && Equals((CommonProfile) obj);
        }
    }
}