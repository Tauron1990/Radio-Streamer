using System;
using Tauron.Application.RadioStreamer.Contracts.Core;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Contracts.UI
{
    public class Option : ObservableObject, IOptionElement
    {
        private string _group;
        private IOptionHelper _helper;
        private string _settingKey;
        private readonly object _defaultValue;
        private object _settingValue;
        private string _displayName;

        [NotNull]
        public object UI
        {
            get { return Helper.LoadUI(this); }
        }

        [CanBeNull]
        public string Group
        {
            get { return _group; }
            private set
            {
                _group = value;
                OnPropertyChanged();
            }
        }

        [NotNull]
        public IOptionHelper Helper
        {
            get { return _helper; }
            private set
            {
                _helper = value;
                OnPropertyChanged();
            }
        }

        [NotNull]
        public string SettingKey
        {
            get { return _settingKey; }
            private set
            {
                _settingKey = value;
                OnPropertyChanged();
            }
        }

        [CanBeNull]
        public object SettingValue
        {
            get { return _settingValue; }
            set
            {
                _settingValue = value;
                OnPropertyChanged();
            }
        }

        public string DisplayName
        {
            get { return _displayName; }
            set
            {
                _displayName = value;
                OnPropertyChanged();
            }
        }

        public bool IsNameVisibly { get; set; }

        public Option([CanBeNull] string @group, [NotNull] IOptionHelper helper, [NotNull] string settingKey,
            [CanBeNull] object defaultValue, [NotNull] string displayName)
        {
            if (helper == null) throw new ArgumentNullException("helper");
            if (settingKey == null) throw new ArgumentNullException("settingKey");
            if (defaultValue == null) throw new ArgumentNullException("defaultValue");
            if (displayName == null) throw new ArgumentNullException("displayName");

            _group = @group;
            _helper = helper;
            _settingKey = settingKey;
            _defaultValue = defaultValue;
            _displayName = displayName;
        }

        public bool Save(IRadioEnvironment store)
        {
            if (store == null) throw new ArgumentNullException("store");

            return Helper.Serialize(store, this);
        }

        public void Load(IRadioEnvironment store)
        {
            Helper.Deserialize(store, this, _defaultValue);
        }

        public override string ToString()
        {
            return DisplayName;
        }
    }
}