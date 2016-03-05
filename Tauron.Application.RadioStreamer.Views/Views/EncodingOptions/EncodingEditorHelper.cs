using System.Windows;
using System.Windows.Controls;
using Tauron.Application.RadioStreamer.Contracts;
using Tauron.Application.RadioStreamer.Contracts.Core;
using Tauron.Application.RadioStreamer.Contracts.UI;
using Tauron.Application.Views;

namespace Tauron.Application.RadioStreamer.Views.EncodingOptions
{
    public class EncodingEditorHelper : IOptionHelper
    {
        public FrameworkElement LoadUI(Option option)
        {
            var temp = ViewManager.Manager.CreateView(AppConstants.CommonEncoderUI);
            new FrameworkObject(temp, false).DataContext = option.SettingValue;
            return (Control)temp;
        }

        public bool Serialize(IRadioEnvironment store, Option option)
        {
            var provider = option.SettingValue as IEncoderViewModel;

            if (provider == null) return false;

            provider.Serialize();

            return true;
        }

        public void Deserialize(IRadioEnvironment store, Option option, object defaultValue)
        {
            var provider = defaultValue as IEncoderViewModel;
            if(provider == null) return;
            option.SettingValue = defaultValue;

            provider.Deserialize();
        }

        public void Reset(Option option)
        {
            var provider = option.DefaultValue as IEncoderViewModel;
            if(provider == null) return;

            provider.Reset();
        }
    }
}