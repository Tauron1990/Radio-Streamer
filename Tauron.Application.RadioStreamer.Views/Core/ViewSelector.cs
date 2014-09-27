using System;
using System.Collections.Generic;
using System.Linq;
using Tauron.Application.Ioc;
using Tauron.Application.RadioStreamer.Contracts.UI;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Core
{
    [Export(typeof (ITabManager))]
    public sealed class TabManager : ITabManager
    {
        private readonly HashSet<ViewEntry> _viewEntries = new HashSet<ViewEntry>();

        public event Action<ViewEntry> ViewSelected;

        private void OnViewSelected([NotNull] ViewEntry obj)
        {
            Action<ViewEntry> handler = ViewSelected;
            if (handler != null) handler(obj);
        }

        public IEnumerable<ViewEntry> Views { get { return new ReadOnlyEnumerator<ViewEntry>(_viewEntries); } }

        public void RegisterView(ViewEntry viewEntry)
        {
            if (viewEntry == null) throw new ArgumentNullException("viewEntry");
            _viewEntries.Add(viewEntry);
        }

        public void View(string name)
        {
            var val = _viewEntries.FirstOrDefault(e => e.Id == name);
            if(val == null) return;

            OnViewSelected(val);
        }
    }
}