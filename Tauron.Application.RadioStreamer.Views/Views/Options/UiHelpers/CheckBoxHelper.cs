using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Tauron.Application.RadioStreamer.Contracts.Core;
using Tauron.Application.RadioStreamer.Contracts.UI;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Views.Options.UiHelpers
{
    public sealed class CheckBoxHelper : ObservableObject, IOptionHelper
    {
        private bool? _currentValue;
        private bool LastValue { get; set; }

        [UsedImplicitly(ImplicitUseKindFlags.Default)]
        public bool? CurrentValue
        {
            get { return _currentValue; }
            set
            {
                _currentValue = value;
                OnPropertyChanged();
            }
        }

        public object LoadUI(Option option)
        {
            var temp = new CheckBox {DataContext = this, Content = option};

            temp.SetBinding(ToggleButton.IsCheckedProperty, "CurrentValue");

            return temp;
        }

        public bool Serialize(IRadioEnvironment store, Option option)
        {
            option.SettingValue = CurrentValue;
            store.Settings.PropertyStore.SetName(option.SettingKey, option.SettingString);
            return true;
        }

        public void Deserialize(IRadioEnvironment store, Option option, object defaultValue)
        {
            if (defaultValue == null)
                defaultValue = bool.FalseString;
            LastValue = bool.Parse(store.Settings.PropertyStore.GetValue(option.SettingKey, defaultValue.ToString()));
            CurrentValue = LastValue;
            option.SettingValue = CurrentValue;
        }

        public void Reset(Option option)
        {
            CurrentValue = LastValue;
            option.SettingValue = CurrentValue;
        }
    }
}