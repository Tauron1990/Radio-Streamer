using System.Windows.Controls;
using Tauron.Application.Controls;
using Tauron.Application.RadioStreamer.Contracts;
using Tauron.Application.RadioStreamer.Contracts.Core;
using Tauron.Application.RadioStreamer.Contracts.Player;
using Tauron.Application.Views;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Core
{
	/// <summary>
	/// Interaktionslogik für DefaultContent.xaml
	/// </summary>
    [ExportView(AppConstants.DefaultContentViewName)]
	public partial class DefaultContent
	{
		private readonly PlayerViewVisibleChanged _playerVisibleEvent;
		private bool _isPlayerVisible;

	    public DefaultContent()
		{
			IEventAggregator aggregator = EventAggregator.Aggregator;

			aggregator.GetEvent<PlayRadioEvent, PlayRadioEventArgs>().Subscribe(Play);
			_playerVisibleEvent = aggregator.GetEvent<PlayerViewVisibleChanged, bool>();
			InitializeComponent();
		}

		private void Play([NotNull] PlayRadioEventArgs obj)
		{
		    RadioSelector.SelectedIndex = 1;
		}

	    private void TabControlSelectionChanged([NotNull] object sender, [NotNull] SelectionChangedEventArgs e)
		{
		    var str = (string) ((IHeaderProvider) ((TabControl) sender).SelectedItem).Header;

		    if (str == AppConstants.RadioPlayerHeader && !_isPlayerVisible)
		    {
		        _playerVisibleEvent.Publish(true);
		        _isPlayerVisible = true;
		    }
		    else if (str != AppConstants.RadioPlayerHeader && _isPlayerVisible)
		    {
		        _playerVisibleEvent.Publish(false);
		        _isPlayerVisible = false;
		    }
		}
	}
}
