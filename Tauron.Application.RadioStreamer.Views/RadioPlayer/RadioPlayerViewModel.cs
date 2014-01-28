using System;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Tauron.Application.Ioc;
using Tauron.Application.Models;
using Tauron.Application.RadioStreamer.Contracts;
using Tauron.Application.RadioStreamer.Contracts.Core.Attributes;
using Tauron.Application.RadioStreamer.Contracts.Data.Enttitis;
using Tauron.Application.RadioStreamer.Contracts.Player;
using Tauron.Application.RadioStreamer.Resources;
using Tauron.Application.RadioStreamer.Views.Helper;
using Tauron.JetBrains.Annotations;
using System.Diagnostics;

namespace Tauron.Application.RadioStreamer.Views.RadioPlayer
{
	[ExportViewModel(AppConstants.RadioViewModelName)]
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
	public class RadioPlayerViewModel : ViewModelBase, IDisposable, INotifyBuildCompled
	{
		public enum PlayerStade
		{
 			Playing,
			Stopped
		}
		public sealed class RadioTitle : ObservableObject
		{
			public RadioTitle()
			{
				_stade = PlayerStade.Stopped;

				_radioTitle = RadioStreamerResources.UnkownString;
				_trackName = RadioStreamerResources.UnkownString;
				
				_isRecording = false;
			}

			private PlayerStade _stade;
			public PlayerStade State
			{
				get { return _stade; }
				set
				{
					if (_stade == value) return;

					_stade = value;
					
					OnPropertyChanged();
					OnPropertyChangedExplicit("Title");
				}
			}

			private string _radioTitle;

		    [NotNull]
		    public string CurrentRadioTitle
			{
				get { return _radioTitle; }
				set
				{
				    if (_radioTitle == value) return;

				    _radioTitle = value;
				    OnPropertyChangedExplicit("RadioTitle");
				    OnPropertyChangedExplicit("Title");
				}
			}

			private string _trackName;

		    [NotNull]
		    public string TrackName
			{
				get { return _trackName; }
				set
				{
				    if (_trackName == value) return;

				    _trackName = value;
				    OnPropertyChanged();
				    OnPropertyChangedExplicit("Title");
				}
			}

			public void SetUnkown(bool radioTitle)
			{
				if (radioTitle)
					_radioTitle = RadioStreamerResources.UnkownString;
				TrackName = RadioStreamerResources.UnkownString;
			}

			private bool _isRecording;
			public bool IsRecording
			{
				get { return _isRecording; }
				set
				{
					if (_isRecording == value) return;

					_isRecording = value;
					OnPropertyChanged();
                    OnPropertyChangedExplicit("Title");
				}
			}

		    [NotNull]
		    public string Title
			{
				get
				{
					var builder = new StringBuilder();

					switch (_stade)
					{
						case PlayerStade.Playing:
							builder.Append(RadioStreamerResources.RadioPlayerStadePlaying);
							break;
						case PlayerStade.Stopped:
							builder.Append(RadioStreamerResources.RadioPlayerStadeStopped);
							break;
						default:
							builder.Append("Unkown; ");
							break;
					}

					builder.Append(CurrentRadioTitle).Append(" ");
					builder.AppendFormat(RadioStreamerResources.RadioTitleCurrentTrack, TrackName);

					if (_isRecording)
						builder.Append(RadioStreamerResources.RadioTitleIsRecording);

					return builder.ToString();
				}
			}
		}

        [InjectRadioPlayer]
		private IRadioPlayer _player;
		[Inject]
		private IEventAggregator _events;

		private DispatcherTimer _bufferTimer;

        private static readonly string ResourceAssembly = "Tauron.Application.RadioStreamer.Resources";

        void INotifyBuildCompled.BuildCompled()
		{
			_events.GetEvent<PlayRadioEvent, PlayRadioEventArgs>().Subscribe(PlayRadioEventHandler);

			CurrentTitle = new RadioTitle();
			_bufferTimer = new DispatcherTimer(TimeSpan.FromSeconds(1), DispatcherPriority.Normal, UpdateRadioStade, SystemDispatcher);
			_bufferTimer.Start();

			UpdateRecordingImage();
            ResetVolume();
		}

		#region RadioStade



	    private Stopwatch _time = new Stopwatch();

	    [NotNull]
	    public string RunTime 
		{ 
			get 
			{ 
				TimeSpan span = _time.Elapsed;
				return String.Format("{0} {1}:{2}",
					RadioStreamerResources.ElapsedTime,
					span.Minutes,
					span.Seconds);
			} 
		}

		private void UpdateRadioStade([NotNull] object dender, [NotNull] EventArgs e)
		{
		    _percentCache = -2;

		    OnPropertyChanged(() => RunTime);
		    OnPropertyChanged(() => PercentageBuffer);
		    OnPropertyChanged(() => PercentageString);
		}

	    [NotNull]
	    public RadioTitle CurrentTitle { get; private set; }

		private double _percentCache = -2;
		public double PercentageBuffer
		{
			get 
			{
			    if (_percentCache != -2) return _percentCache;

			    if (CurrentTitle.State != PlayerStade.Playing) return 0;
			    _percentCache = _player.GetBufferPercentage();

			    return _percentCache;
			}
		}

	    [NotNull]
	    public string PercentageString
		{
			get
			{
				switch (CurrentTitle.State)
				{
					case PlayerStade.Playing:
						return String.Format("{0}%", PercentageBuffer);
					default:
						return RadioStreamerResources.BufferPercentageInvalid;
				}
			}
		}

		private RadioEntry _lastRadio;
		private RadioQuality _lastQuality;
		#endregion

		#region Playing
		private bool _isInitialized;

		private void SendError()
		{
			const string caption = "Error";
			string text = String.Format(RadioStreamerResources.BassErrorMessage, _player.GetLastError());

		    Dialogs.ShowMessageBox(ViewManager.GetWindow(AppConstants.MainWindowName), text, caption, MsgBoxButton.Ok, MsgBoxImage.Error, null);
		}
		private bool Initialize()
		{
			if (_isInitialized) return false;

			_isInitialized = _player.Activate();

		    if (_isInitialized) return _isInitialized;

		    const string caption = "Error";
		    string text = String.Format(RadioStreamerResources.BassInitErrorMessage, _player.GetLastError());

		    Dialogs.ShowMessageBox(ViewManager.GetWindow(AppConstants.MainWindowName), text, caption, MsgBoxButton.Ok, MsgBoxImage.Error, null);

		    return _isInitialized;
		}
		private void Play(RadioQuality radio)
		{
			if (!Initialize()) return;

			if (CurrentTitle.State == PlayerStade.Playing)
				Stop();

			bool flag = _player.Play(radio, null);
			if (flag)
			{
				ResetVolume();
				CurrentTitle.State = PlayerStade.Playing;
				_time.Start();
			}
			else
				SendError();
		}
		
		private void PlayRadioEventHandler([NotNull] PlayRadioEventArgs obj)
		{
		    if (obj.Quality.IsEmpty) return;
		    _lastRadio = obj.Radio;
		    _lastQuality = obj.Quality;

		    CurrentTitle.SetUnkown(true);
		    CurrentTitle.CurrentRadioTitle = obj.Radio.Name;
		    Play(obj.Quality);
		}

	    [CommandTarget]
		private bool CanPlay()
		{
			return CurrentTitle.State != PlayerStade.Playing && !_lastRadio.IsEmpty;
		}
		[CommandTarget]
		private void Play()
		{
			Play(_lastQuality);
		}
		#endregion

		#region Stop
		[CommandTarget]
		private bool CanStop()
		{
			return CurrentTitle.State == PlayerStade.Playing;
		}
		[CommandTarget]
		private void Stop()
		{
			_time.Stop();

			_player.Stop();
			CurrentTitle.SetUnkown(false);
			CurrentTitle.State = PlayerStade.Stopped;
			UpdateRecordingImage();
		}

		#endregion

		#region Record

		[CommandTarget]
		private bool CanRecord()
		{
			return _player.SupportRecording && CurrentTitle.State == PlayerStade.Playing;
		}
		[CommandTarget]
		private void Record()
		{
			string path = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic).CombinePath("Radio Streamer");
			path.CreateDirectoryIfNotExis();

			if (_player.IsRecording)
				_player.StopRecording();
			else
				_player.StartRecording(path);
			UpdateRecordingImage();
		}

		private ImageSource _recordingImage;

	    [NotNull]
	    public ImageSource RecordingImage
		{
			get { return _recordingImage; }
			set
			{
			    if (Equals(_recordingImage, value)) return;

			    _recordingImage = value;
			    OnPropertyChanged();
			}
		}

        private static readonly Uri RecordImage = StaticPackUrihelper.GetUri("/Record.png", ResourceAssembly, true);
        private static readonly Uri RecordActiveImage = StaticPackUrihelper.GetUri("/RecordActive.png", ResourceAssembly, true);
		private Uri _lastRecordImage;

		private void UpdateRecordingImage()
		{
			if (_player.IsRecording && _lastRecordImage != RecordActiveImage)
			{
				_lastRecordImage = RecordActiveImage;
				RecordingImage = BitmapFrame.Create(RecordActiveImage);
			}
			else if (_lastRecordImage != RecordImage)
			{
				_lastRecordImage = RecordImage;
				RecordingImage = BitmapFrame.Create(RecordImage);
			}
		}

		#endregion

		#region Volume

        private static readonly Uri AudioVolumeHigh = StaticPackUrihelper.GetUri("/audio-volume-high.png", ResourceAssembly, true);
        private static readonly Uri AudioVolumeMedium = StaticPackUrihelper.GetUri("/audio-volume-medium.png", ResourceAssembly, true);
        private static readonly Uri AudioVolumeMuted = StaticPackUrihelper.GetUri("/audio-volume-muted.png", ResourceAssembly, true);
        private static readonly Uri AudioVolumeLow = StaticPackUrihelper.GetUri("/audio-volume-low.png", ResourceAssembly, true);

		private Uri _lastVolumeImage;

		private ImageSource _volumeImage;

	    [NotNull]
	    public ImageSource VolumeImage
		{
			get { return _volumeImage; }
			set
			{
			    _volumeImage = value;
			    OnPropertyChanged();
			}
		}

		private double _volume;
		public double Volume
		{
			get { return _volume; }
			set
			{
				if (_volume == value) return;

				UpdateVolume(value);
				_volume = value;
				OnPropertyChanged();
			}
		}

		private void ResetVolume()
		{
			SetVolumeImage(AudioVolumeHigh);
			_volume = 100;
			OnPropertyChanged(() => Volume);
		}
		private void UpdateVolume(double volume)
		{
			if (volume > 80)
				SetVolumeImage(AudioVolumeHigh);
			else if (volume > 40)
				SetVolumeImage(AudioVolumeMedium);
			else if (volume > 0.1)
				SetVolumeImage(AudioVolumeLow);
			else
				SetVolumeImage(AudioVolumeMuted);

			_player.SetVolume(volume);
		}
		private void SetVolumeImage([NotNull] Uri image)
		{
		    if (_lastVolumeImage == image) return;

		    _lastVolumeImage = image;
		    VolumeImage = BitmapFrame.Create(image);
		}

	    private bool _isNotMuted = true;
		public bool IsNotMuted
		{
			get { return _isNotMuted; }
			set 
			{
				if (_isNotMuted == value) return;

				_isNotMuted = value;
				OnPropertyChanged(() => IsNotMuted);
			}
		}

		[CommandTarget]
		private void MuteVolume()
		{
			if (IsNotMuted)
			{
				UpdateVolume(0);
				IsNotMuted = false;
			}
			else
			{
				UpdateVolume(_volume);
				IsNotMuted = true;
			}
		}
		#endregion

		public void Dispose()
		{
			_bufferTimer.Stop();
			_player.Deactivate();
		}
	}
}
