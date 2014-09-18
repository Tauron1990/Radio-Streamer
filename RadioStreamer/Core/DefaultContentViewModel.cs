using System;
using System.Collections.Generic;
using System.Windows.Media;
using Tauron.Application.Ioc;
using Tauron.Application.Models;
using Tauron.Application.RadioStreamer.Contracts;
using Tauron.Application.RadioStreamer.Contracts.Core.Attributes;
using Tauron.Application.RadioStreamer.Contracts.Player;
using Tauron.Application.RadioStreamer.Contracts.UI;
using Tauron.Application.RadioStreamer.Resources;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Core
{
	[ExportViewModel(AppConstants.DefaultContentViewModel)]
	public class DefaultContentViewModel : ViewModelBase, INotifyBuildCompled
	{
        [Inject]
	    private IEventAggregator _eventAggregator;

        [Inject]
	    private IStyleManager _styleManager;

        [Inject]
	    private IDialogFactory _factory;

		[StatusBarItemImport]
		private object[] _statusBarItems;
        
	    [NotNull]
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

        private string _currentTrack;

	    [NotNull]
	    public string CurrentTrack
	    {
	        get { return _currentTrack; }
	        set
	        {
	            if (Equals(_currentTrack, value)) return;

	            _currentTrack = value;

	            OnPropertyChanged();
	        }
	    }

        private ImageSource _toolTipImage;

	    [NotNull]
	    public ImageSource ToolTipImage
	    {
	        get { return _toolTipImage; }
	        set
	        {
	            if (Equals(_toolTipImage, value)) return;

	            _toolTipImage = value;

	            OnPropertyChanged();
	        }
	    }

	    [NotNull]
	    public IEnumerable<GenericMenuItem> NotifyContextMenu
	    {
	        get { return MenuItemService.GetMenu(MenuItemService.NotifyContextMenuName); }
	    }

	    public void BuildCompled()
	    {
	        CurrentTrack = RadioStreamerResources.UnkownString;

	        ToolTipImage = ImagesCache.ImageSources["StopImage"];
            
	        _eventAggregator.GetEvent<RadioPlayerTitleRecived, string>().Subscribe(str => CurrentTrack = str);
	        _eventAggregator.GetEvent<RadioPlayerPlay, EventArgs>()
	                        .Subscribe(e => ToolTipImage = ImagesCache.ImageSources["PlayImage"]);
	        _eventAggregator.GetEvent<RadioPlayerStop, EventArgs>()
	                        .Subscribe(e =>
	                        {
	                            CurrentTrack = RadioStreamerResources.RadioPlayerStadeStopped;
	                            ToolTipImage = ImagesCache.ImageSources["StopImage"];
	                        });
	    }
	}
}
