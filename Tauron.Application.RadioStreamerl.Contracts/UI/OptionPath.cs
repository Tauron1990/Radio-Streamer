using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Tauron.Application.RadioStreamer.Contracts.Core;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Contracts.UI
{
    public class OptionPath : IOptionElement
    {
        private sealed class SortingCollection : Collection<IOptionElement>, INotifyCollectionChanged
        {
            private class OptionElementComparer : IComparer<IOptionElement>
            {
                public int Compare([NotNull] IOptionElement x, [NotNull] IOptionElement y)
                {
                    if (x is OptionPath)
                    {
                        if (y is Option || y is OptionGroup)
                            return 1;
                        if (y is OptionPath)
                            return string.CompareOrdinal(x.DisplayName, y.DisplayName);
                        return -1;
                    }
                    if (x is Option)
                    {
                        if (y is OptionGroup)
                            return 1;
                        if (y is OptionPath)
                            return -1;
                        if (y is Option)
                            return string.CompareOrdinal(x.DisplayName, y.DisplayName);
                        return -1;
                    }
                    if (x is OptionGroup)
                    {
                        if (y is OptionPath || y is Option)
                            return -1;
                        if (y is OptionGroup)
                            return string.CompareOrdinal(x.DisplayName, y.DisplayName);
                        return -1;
                    }

                    if (y is Option || y is OptionGroup || y is OptionPath)
                        return -1;

                    return string.CompareOrdinal(x.DisplayName, y.DisplayName);
                }
            }

            private static readonly IComparer<IOptionElement> Comparer = new OptionElementComparer();

            private readonly List<IOptionElement> _list;

            public SortingCollection()
            {
                _list = (List<IOptionElement>)Items;
            }

            private void Sort()
            {
                _list.Sort(Comparer);
                OnCollectionChanged();
            }

            protected override void ClearItems()
            {
                base.ClearItems();
                Sort();
            }

            protected override void InsertItem(int index, [NotNull] IOptionElement item)
            {
                base.InsertItem(index, item);
                Sort();
            }

            protected override void RemoveItem(int index)
            {
                base.RemoveItem(index);
                Sort();
            }

            protected override void SetItem(int index, [NotNull] IOptionElement item)
            {
                base.SetItem(index, item);
                Sort();
            }

            public event NotifyCollectionChangedEventHandler CollectionChanged;

            private void OnCollectionChanged()
            {
                NotifyCollectionChangedEventHandler handler = CollectionChanged;
                if (handler != null)
                    handler(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }

        public string DisplayName { get; set; }

        [NotNull, UsedImplicitly]
        public ICollection<IOptionElement> Elements { get; private set; }

        public OptionPath()
        {
            Elements = new SortingCollection();
        }

        public bool Save(IPropertyStore store)
        {
            if (store == null) throw new ArgumentNullException("store");

            var temp = false;

// ReSharper disable once LoopCanBePartlyConvertedToQuery
            foreach (var optionPath in Elements)
            {
                var temp2 = optionPath.Save(store);
                if (temp2)
                    temp = true;
            }

            return temp;
        }

        public void Load(IPropertyStore store)
        {
            if (store == null) throw new ArgumentNullException("store");

            foreach (var optionPath in Elements)
                optionPath.Load(store);
        }

        public override string ToString()
        {
            return DisplayName;
        }
    }
}