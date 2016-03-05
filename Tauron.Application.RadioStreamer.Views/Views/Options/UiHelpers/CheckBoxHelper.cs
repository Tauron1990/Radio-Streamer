using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Tauron.Application.RadioStreamer.Contracts.Core;
using Tauron.Application.RadioStreamer.Contracts.UI;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Views.Options.UiHelpers
{
    public sealed class CheckBoxHelper : OptionHelperBaseClass
    {
        private readonly Action<bool> _saveAction;
        private bool? _currentValue;
        private bool LastValue { get; set; }
        private CheckBox _checkBox;
        private object _content;

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

        public CheckBoxHelper([CanBeNull]Action<bool> saveAction, [CanBeNull] object content = null)
        {
            _saveAction = saveAction;
            _content = content;
        }

        public override FrameworkElement LoadUI(Option option)
        {
            if (_checkBox != null) return _checkBox;

            _checkBox = new CheckBox { DataContext = this, Content = _content ?? option.DisplayName };
            _checkBox.SetBinding(ToggleButton.IsCheckedProperty, "CurrentValue");

            return _checkBox;
        }

        protected override string GetCurrentValue()
        {
            return _currentValue.ToString();
        }

        protected override void SetValue(string value)
        {
            CurrentValue = bool.Parse(value);
        }


        public override bool Serialize(IRadioEnvironment store, Option option)
        {
            var temp = base.Serialize(store, option);
            _saveAction?.Invoke(CurrentValue ?? false);
            return temp;
        }
    }
}