using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using Tauron.Application.RadioStreamer.Contracts.UI;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Views.Options.UiHelpers
{
    public sealed class ComboboxHelperItem
    {
        public string Value { get; }

        [UsedImplicitly(ImplicitUseKindFlags.Access)]
        public string DisplyName { get; }

        public ComboboxHelperItem(string value, string displyName)
        {
            Value = value;
            DisplyName = displyName;
        }

        public override string ToString()
        {
            return DisplyName;
        }
    }

    public class ComboboxHelper : OptionHelperBaseClass
    {
        private string _old;

        private List<ComboboxHelperItem> _items;
        private ComboboxHelperItem _currentItem;

        [NotNull]
        public List<ComboboxHelperItem> Items
        {
            get { return _items; }
            set { _items = value; OnPropertyChanged();}
        }

        [NotNull]
        public ComboboxHelperItem CurrentItem
        {
            get { return _currentItem; }
            set { _currentItem = value; OnPropertyChanged();}
        }

        public ComboboxHelper(params ComboboxHelperItem[] items)
        {
            Items = new List<ComboboxHelperItem>(items);
        }

        public override FrameworkElement LoadUI(Option option)
        {
            var box = new ComboBox
            {
                MinWidth = 100,
                DataContext = this,
                ItemsSource = Items
            };

            box.SetBinding(Selector.SelectedItemProperty, new Binding(nameof(CurrentItem)));

            return box;
        }

        protected override string GetCurrentValue()
        {
            return CurrentItem.Value;
        }

        protected override void SetValue(string value)
        {
            CurrentItem = Items.FirstOrDefault(i => i.Value == value) ?? Items.First();
        }
    }
}