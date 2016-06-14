using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Tauron.Application.BassLib.Misc;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.BassLib.Recording
{
    [PublicAPI]
    public abstract class AudioEncoderFactoryBase : ObservableObject
    {
        protected abstract class BaseMapper
        {
            public abstract string Set<TType>(TType value, [NotNull] string name);
            public abstract TType Get<TType>([NotNull] string name);

            [NotNull]
            public abstract AudioEncoder GetAudioEncoder([NotNull] Channel channel);
        }

        [PublicAPI]
        protected sealed class GenericMapper<TEncoder> : BaseMapper
            where  TEncoder : AudioEncoder
            
        {
            private interface IInternalConverter
            {
                [NotNull]
                string Convert([NotNull] object value);

                [NotNull]
                object ConvertBack([NotNull] string value);
            }
            private class FuncWrapper<TType> : IInternalConverter
            {
                private readonly Func<string, TType> _fromString;
                private readonly Func<TType, string> _toString;

                public FuncWrapper([CanBeNull] Func<string, TType> fromString, [CanBeNull] Func<TType, string> toString)
                {
                    _fromString = fromString;
                    _toString = toString;
                }

                public string Convert([CanBeNull] object value)
                {
                    if (_toString != null) return _toString((TType) value);
                    return value?.ToString() ?? string.Empty;
                }

                public object ConvertBack([CanBeNull] string value)
                {
                    if (value == null) return default(TType);
                    if (_fromString == null) return value;
                    return _fromString(value);
                }
            }
            
            private abstract class ActionWrapper
            {
                public abstract void Invoke([NotNull] TEncoder encoder, [CanBeNull] object value);
            }

            private class ActionWrapperImpl<TType> : ActionWrapper
            {
                private readonly Action<TEncoder, TType> _action;

                public ActionWrapperImpl([NotNull] Action<TEncoder, TType> action)
                {
                    if (action == null) throw new ArgumentNullException(nameof(action));
                    _action = action;
                }

                public override void Invoke(TEncoder encoder, object value)
                {
                    _action(encoder, (TType)value);
                }
            }

            private class MapperDataProperty : IEquatable<MapperDataProperty>
            {
                private object _value;

                public bool Equals([CanBeNull] MapperDataProperty other)
                {
                    if (ReferenceEquals(null, other)) return false;
                    return ReferenceEquals(this, other) || string.Equals(Name, other.Name);
                }

                public override int GetHashCode()
                {
                    return Name.GetHashCode();
                }

                public static bool operator ==(MapperDataProperty left, MapperDataProperty right)
                {
                    return Equals(left, right);
                }

                public static bool operator !=(MapperDataProperty left, MapperDataProperty right)
                {
                    return !Equals(left, right);
                }

                [NotNull]
                public string Name { get; set; }

                [NotNull]
                public IInternalConverter Converter { get; set; }

                [CanBeNull]
                public object DefaultValue { get; set; }


                [CanBeNull]
                public object Value
                {
                    get { return _value; }
                    set
                    {
                        IsSet = value != DefaultValue;
                        _value = value;
                    }
                }

                public bool IsSet { get; private set; }


                [NotNull]
                public ActionWrapper EncoderSetter { get; set; }

                public override bool Equals([CanBeNull] object obj)
                {
                    if (ReferenceEquals(null, obj)) return false;
                    if (ReferenceEquals(this, obj)) return true;
                    return obj.GetType() == GetType() && Equals((MapperDataProperty) obj);
                }
            }

            private readonly Dictionary<string, string> _profile;
            private readonly Func<Channel, TEncoder> _factory;
            private readonly Action<string> _propChanged;
            private readonly  HashSet<MapperDataProperty> _dataProperties = new HashSet<MapperDataProperty>();

            public GenericMapper([NotNull] Dictionary<string, string> profile, [NotNull] Func<Channel, TEncoder> factory,
                [NotNull] Action<string> propChanged)
            {
                if (profile == null) throw new ArgumentNullException(nameof(profile));
                if (factory == null) throw new ArgumentNullException(nameof(factory));
                if (propChanged == null) throw new ArgumentNullException(nameof(propChanged));

                _profile = profile;
                _factory = factory;
                _propChanged = propChanged;
            }

            [NotNull]
            public GenericMapper<TEncoder> CreateProperty<TType>([NotNull] Expression<Func<TType>> property,
                [NotNull] Action<TEncoder, TType> encoderSetter,
                [CanBeNull] Func<TType, string> toStringConverter,
                [CanBeNull] Func<string, TType> toTypeConverter,
                TType defaultValue = default(TType))
            {
                string name = PropertyHelper.ExtractPropertyName(property);

                if (!_dataProperties.Add(new MapperDataProperty
                {
                    Name = name,
                    Converter = new FuncWrapper<TType>(toTypeConverter, toStringConverter),
                    DefaultValue = defaultValue,
                    EncoderSetter = new ActionWrapperImpl<TType>(encoderSetter)
                }))
                    throw new ArgumentException("Property Duplicate");

                return this;
            }

            [NotNull]
            public GenericMapper<TEncoder> CreateProperty([NotNull] Expression<Func<int>> property,
                [NotNull] Action<TEncoder, int> encoderSetter,
                int defaultValue = 0)
            {
                return CreateProperty(property, encoderSetter, i => i.ToString(CultureInfo.InvariantCulture),
                    int.Parse, defaultValue);
            }

            [NotNull]
            public GenericMapper<TEncoder> CreateProperty([NotNull] Expression<Func<float>> property,
                [NotNull] Action<TEncoder, float> encoderSetter,
                float defaultValue = 0f)
            {
                return CreateProperty(property, encoderSetter, i => i.ToString(CultureInfo.InvariantCulture),
                    float.Parse, defaultValue);
            }
            [NotNull]
            public GenericMapper<TEncoder> CreateProperty([NotNull] Expression<Func<bool>> property,
                [NotNull] Action<TEncoder, bool> encoderSetter,
                bool defaultValue = false)
            {
                return CreateProperty(property, encoderSetter, i => i.ToString(),
                    bool.Parse, defaultValue);
            }

            [NotNull]
            public GenericMapper<TEncoder> CreateProperty([NotNull] Expression<Func<string>> property,
                [NotNull] Action<TEncoder, string> encoderSetter, [CanBeNull] string defaultValue = "")
            {
                return CreateProperty(property, encoderSetter, null,
                    null, defaultValue);
            }

            [NotNull]
            public GenericMapper<TEncoder> CreatePropertyEnum<TType>([NotNull] Expression<Func<TType>> property,
                [NotNull] Action<TEncoder, TType> encoderSetter,
                TType defaultValue = default(TType))
            {
                if (!typeof (Enum).IsAssignableFrom(typeof (TType)))
                    throw new InvalidOperationException("No Enum (TType)");

                return CreateProperty(property, encoderSetter, null, s => (TType)Enum.Parse(typeof (TType), s),
                    defaultValue);
            }


            public void Initialize()
            {
                foreach (var dataProperty in _profile)
                {
                    var mapperProp = _dataProperties.FirstOrDefault(mp => mp.Name == dataProperty.Key);
                    if(mapperProp == null) continue;

                    mapperProp.Value = mapperProp.Converter.ConvertBack(dataProperty.Value);
                }    
            }

            public override string Set<TType>(TType value, string name)
            {
                var mapperProp = _dataProperties.FirstOrDefault(mp => mp.Name == name);
                if (mapperProp == null) return null;

                mapperProp.Value = value;
                var temp = SetProperty(value, name, type => mapperProp.Converter.Convert(value));
                _propChanged(name);
                return temp;
            }

            private string SetProperty<TType>(TType value, string name, Func<TType, string> converter)
            {

                if (name == null) throw new ArgumentNullException(nameof(name));

                Func<TType, string> realConverter = converter;
                if (converter == null)
                    realConverter = type => type.ToString();

                var temp = realConverter(value);
                _profile[name] = temp;

                return temp;
            }

            public override TType Get<TType>(string name)
            {
                var mapperProp = _dataProperties.FirstOrDefault(mp => mp.Name == name);
                if (mapperProp == null) return default(TType);

                if (mapperProp.IsSet) return (TType) mapperProp.Value;

                return (TType) mapperProp.DefaultValue;
            }

            public override AudioEncoder GetAudioEncoder(Channel channel)
            {
                var enc = _factory(channel);

                foreach (var property in _dataProperties.Where(p => p.IsSet))
                {
                    property.EncoderSetter.Invoke(enc, property.Value);
                }

                return enc;
            }
        }

        private readonly string _id;
        private BaseMapper _baseMapper;
        private Dictionary<string, string> _profile;

        [NotNull]
        public AudioEncoder CreateEncoder([NotNull] Channel channel)
        {
            EnsureBaseMapper();

            return _baseMapper.GetAudioEncoder(channel);
        }

        protected AudioEncoderFactoryBase([NotNull] string id, [CanBeNull]Dictionary<string, string> profile)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
            _id = id;
// ReSharper disable once AssignNullToNotNullAttribute
            Profile = profile;
        }

        [NotNull]
        public Dictionary<string, string> Profile
        {
            get { return _profile ?? (_profile = new Dictionary<string, string>()); }
            set { _profile = value; }
        }

        [NotNull]
        protected GenericMapper<TEncoder> CreateMapping<TEncoder>([NotNull] Func<Channel, TEncoder> creator)
            where TEncoder : AudioEncoder
        {
            var mapper = new GenericMapper<TEncoder>(Profile, creator, OnPropertyChangedExplicit);
            _baseMapper = mapper;

            return mapper;
        }

        protected TType GetValue<TType>([CallerMemberName] [NotNull] string name = null)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            EnsureBaseMapper();
// ReSharper disable once AssignNullToNotNullAttribute
            return _baseMapper.Get<TType>(name);
        }

        protected void SetValue<TType>(TType value, [CallerMemberName] [NotNull] string name = null)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            EnsureBaseMapper();
// ReSharper disable once AssignNullToNotNullAttribute


            ValueChanged(name, _baseMapper.Set(value, name));
        }

        protected virtual void ValueChanged(string name, string value)
        { }

        private void EnsureBaseMapper()
        {
            if(_baseMapper != null) return;

            throw new InvalidOperationException("Not Mapping Created");
        }
    }
}