using System.Windows;
using Tauron.Application.RadioStreamer.Contracts.Core;
using Tauron.Application.RadioStreamer.Contracts.UI;

namespace Tauron.Application.RadioStreamer.Views.Options.UiHelpers
{
    public abstract class OptionHelperBaseClass : ObservableObject, IOptionHelper
    {
        private string _oldValue;

        public abstract FrameworkElement LoadUI(Option option);

        protected abstract string GetCurrentValue();
        protected abstract void SetValue(string value);

        public virtual bool Serialize(IRadioEnvironment store, Option option)
        {
            var newValue = GetCurrentValue();
            if(newValue == _oldValue) return false;

            store.Settings.PropertyStore.SetValue(option.SettingKey, newValue);
            _oldValue = newValue;
            option.SettingValue = newValue;

            return true;
        }

        public virtual void Deserialize(IRadioEnvironment store, Option option, object defaultValue)
        {
            string currentValue = store.Settings.PropertyStore.GetValue(option.SettingKey, defaultValue?.ToString());

            _oldValue = currentValue;
            SetValue(_oldValue);
            option.SettingValue = currentValue;
        }

        public virtual void Reset(Option option)
        {
            SetValue(_oldValue);
        }
    }
}