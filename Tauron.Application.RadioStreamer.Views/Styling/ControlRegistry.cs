using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Tauron.Application.Ioc;
using Tauron.Application.Views;
using Tauron.Application.Views.Core;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Views.Styling
{
    public class ControlRegistry : CommonLocatorBase
    {
        private CommonLocatorBase _baseLocator;

        private Dictionary<string, string> _xaml; 

        public ControlRegistry()
        {
            _baseLocator = (CommonLocatorBase)ViewManager.Manager.ViewLocator;
            _xaml = new Dictionary<string, string>();
        }

        public void Register([NotNull] string name, [NotNull] string xaml)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (xaml == null) throw new ArgumentNullException(nameof(xaml));
            _xaml[name] = xaml;
        }

        public override DependencyObject Match(string name)
        {
            string value;
            if (_xaml.TryGetValue(name, out value)) return (DependencyObject)XamlReader.Parse(value);

            return _baseLocator.Match(name);
        }

        public override string GetName(Type model)
        {
            return _baseLocator.GetName(model);
        }

        public override DependencyObject Match(ISortableViewExportMetadata name)
        {
            return Match(name.Name);
        }

        public override IEnumerable<InstanceResolver<Control, ISortableViewExportMetadata>> GetAllViewsImpl(string name)
        {
            return _baseLocator.GetAllViewsImpl(name);
        }

        public override IWindow CreateWindowImpl(string name, object[] parameters)
        {
            WpfApplication.CurrentWpfApplication.Dispatcher.CheckAccess();
            string value;
            return _xaml.TryGetValue(name, out value) ? UiSynchronize.Synchronize.Invoke(() => new WpfWindow((Window) XamlReader.Parse(value))) : _baseLocator.CreateWindowImpl(name, parameters);
        }

        public override Type GetViewType(string name)
        {
            return _baseLocator.GetViewType(name);
        }
    }
}