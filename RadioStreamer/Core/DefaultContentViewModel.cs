using Tauron.Application.Models;
using Tauron.Application.RadioStreamer.Contracts;
using Tauron.Application.RadioStreamer.Contracts.Core.Attributes;

namespace Tauron.Application.RadioStreamer.Core
{
	[ExportViewModel(AppConstants.DefaultContentViewModel)]
	public class DefaultContentViewModel : ViewModelBase
	{
		[StatusBarItemImport]
		private object[] _statusBarItems;

		public object[] StatusBarItems
		{
			get { return _statusBarItems; }
			set
			{
				if (_statusBarItems == value) return;

				_statusBarItems = value;
				OnPropertyChanged();
			}
		}
	}
}
