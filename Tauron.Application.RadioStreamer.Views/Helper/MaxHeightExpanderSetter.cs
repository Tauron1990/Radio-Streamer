using System;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Threading;
using System.Xaml;

namespace Tauron.Application.RadioStreamer.Views.Helper
{
	[MarkupExtensionReturnType(typeof(double))]
	public sealed class MaxHeightExpanderSetter : MarkupExtension
	{
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			var provider = serviceProvider.GetService<IRootObjectProvider>();

			if (provider == null)
				return 300d;

			var root = provider.RootObject as FrameworkElement;
			if (root == null)
				return 300d;

			root.Loaded += Loaded;

			return 300d;
		}

		private FrameworkElement _targetElement;
		private DispatcherTimer _timer;
		private Window _win;
		private double _last;

		void Loaded(object sender, RoutedEventArgs e)
		{
			_last = 0;
			var ele = (FrameworkElement)e.OriginalSource;
			_win = Window.GetWindow(ele);

			_targetElement = ele;
			_timer = new DispatcherTimer(TimeSpan.FromMilliseconds(100), DispatcherPriority.Normal, Update, _win.Dispatcher);
		}

		private void Update(object sender, EventArgs e)
		{
			double curr = _win.ActualHeight - 150;
			if (_last == curr) return;

			_targetElement.MaxHeight = curr;
			_last = curr;
		}
	}
}
