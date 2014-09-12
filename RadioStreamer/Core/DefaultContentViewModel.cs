using System;
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
    public class ThemeInfo : ViewModelBase
    {
        private readonly IDialogFactory _factory;
        private string _name;

        public ThemeInfo([NotNull] string name, [NotNull] IStyleManager manager, [NotNull] IDialogFactory factory)
        {
            _factory = factory;
            _name = name;
            Manager = manager;
        }

        [NotNull]
        public string Name
        {
            get { return _name.GetFileNameWithoutExtension(); }
        }

        [NotNull]
        public IStyleManager Manager { get; private set; }

        public bool Enable { get { return Name != Manager.CurrentTheme; } }

        [EventTarget]
        public void Click()
        {
            Manager.SetTheme(_name);

            _factory.ShowMessageBox(MainWindow, RadioStreamerResources.ThemeSwichMessage, "Themes", MsgBoxButton.Ok,
                                    MsgBoxImage.Information, null);
        }
    }

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

	    private object[] _contextMenuEntrys;

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

        private UISyncObservableCollection<ThemeInfo> _theme;

	    [NotNull]
	    public UISyncObservableCollection<ThemeInfo> Themes
	    {
	        get { return _theme; }
	        set
	        {
	            if (Equals(_theme, value)) return;

	            _theme = value;

	            OnPropertyChanged();
	        }
	    }
        
	    public void BuildCompled()
	    {
	        CurrentTrack = RadioStreamerResources.UnkownString;

	        ToolTipImage = ImagesCache.ImageSources["StopImage"];

            Themes = new UISyncObservableCollection<ThemeInfo>();

	        foreach (var theme in _styleManager.Themes)
	        {
	            Themes.Add(new ThemeInfo(theme, _styleManager, _factory));
	        }

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
