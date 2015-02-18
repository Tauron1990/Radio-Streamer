using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using Tauron.Application.RadioStreamer.Contracts.Core;
using Tauron.Application.RadioStreamer.Contracts.UI;
using Tauron.Application.RadioStreamer.Resources;
using Tauron.Application.RadioStreamer.Views.Helper;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Views.Options.UiHelpers
{
    public class DefaultProfileHelper : IOptionHelper
    {
        private string _old;
        private CommonComboBoxItem _nullItem;

        [NotNull]
        public List<CommonComboBoxItem> Profiles { get; set; }

        [NotNull]
        public CommonComboBoxItem CurrentEncoder { get; set; }

        public object LoadUI(Option option)
        {
            var box = new ComboBox
            {
                ItemsSource = Profiles
            };

            box.SetBinding(Selector.SelectedItemProperty, new Binding("CurrentEncoder"));
            box.DataContext = this;

            return box;
        }

        public bool Serialize(IRadioEnvironment store, Option option)
        {
            store.Settings.EncoderProfiles.SetDefault(CurrentEncoder.Value as string);
            var returnVal = _old == CurrentEncoder.Value as string;
            _old = CurrentEncoder.Value as string;
            return returnVal;
        }

        public void Deserialize(IRadioEnvironment store, Option option, object defaultValue)
        {
            var encoder = store.Settings.EncoderProfiles.Default;
            _old = encoder.Item2 != null ? encoder.Item1 : null;

            Profiles = store.Settings.EncoderProfiles.Profiles.Select(s => new CommonComboBoxItem(s, s)).ToList();
            _nullItem = new CommonComboBoxItem(RadioStreamerResources.NoneString, null);


            Reset(option);
            if (Profiles != null) 
                Profiles.Add(_nullItem);
        }

        public void Reset(Option option)
        {
            var currentEncoder = Profiles.Find(i => i.Value as string == _old) ?? _nullItem;
            CurrentEncoder = currentEncoder;
        }
    }
}