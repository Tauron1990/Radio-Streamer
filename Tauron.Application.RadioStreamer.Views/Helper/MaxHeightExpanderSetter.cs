#region Usings

using System;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Threading;
using System.Xaml;

#endregion

namespace Tauron.Application.RadioStreamer.Views.Helper
{
    [MarkupExtensionReturnType(typeof (double))]
    public sealed class MaxHeightExpanderSetter : MarkupExtension
    {
        private double _last;
        private FrameworkElement _targetElement;
        private DispatcherTimer _timer;
        private Window _win;

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var provider = serviceProvider.GetService<IRootObjectProvider>();

            var root = provider?.RootObject as FrameworkElement;
            if (root == null)
                return 300d;

            root.Loaded += Loaded;

            return 300d;
        }

        private void Loaded(object sender, RoutedEventArgs e)
        {
            _last = 0;
            var ele = (FrameworkElement) e.OriginalSource;
            _win = Window.GetWindow(ele);

            _targetElement = ele;
            _timer = new DispatcherTimer(TimeSpan.FromMilliseconds(100), DispatcherPriority.Normal, Update,
                _win.Dispatcher);
        }

        private void Update(object sender, EventArgs e)
        {
            double curr = _win.ActualHeight - 150;
            if (_last == curr) return;

            if (curr < 0) curr = 0;

            _targetElement.MaxHeight = curr;
            _last = curr;
        }
    }
}