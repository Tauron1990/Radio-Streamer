#region Usings

using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Threading;
using Tauron.Application.Ioc;
using Tauron.Application.Models;
using Tauron.Application.RadioStreamer.Contracts;
using Tauron.Application.RadioStreamer.Contracts.Core;
using Tauron.Application.RadioStreamer.Contracts.Core.Attributes;
using Tauron.Application.RadioStreamer.Contracts.Data.Enttitis;
using Tauron.Application.RadioStreamer.Contracts.Player;
using Tauron.Application.RadioStreamer.Contracts.Player.Misc;
using Tauron.Application.RadioStreamer.Contracts.Scripts;
using Tauron.Application.RadioStreamer.Contracts.UI;
using Tauron.Application.RadioStreamer.Resources;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application.RadioStreamer.Views.RadioPlayer
{
    [ExportViewModel(AppConstants.RadioPlayerViewModelName)]
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public class RadioPlayerViewModel : TabWorkspace, IDisposable
    {
        public enum PlayerStade
        {
            Playing,
            Stopped
        }

        private DispatcherTimer _bufferTimer;
        [Inject] 
        private IEngineManager _engineManager;
        [Inject] 
        private IEventAggregator _events;
        [InjectRadioPlayer] 
        private IRadioPlayer _player;
        [InjectRadioEnviroment] 
        private IRadioEnvironment _radioEnvironment;
        [Inject]
        private IDeviceManager _deviceManager;

        public void Dispose()
        {
            _bufferTimer.Stop();
            _player.Deactivate();
        }

        public override void BuildCompled()
        {
            CanClose = false;
            IsNotMuted = true;

            _events.GetEvent<PlayRadioEvent, PlayRadioEventArgs>().Subscribe(PlayRadioEventHandler);
            _events.GetEvent<RadioPlayerTitleRecived, string>().Subscribe(str => CurrentTitle.TrackName = str);

            CurrentTitle = new RadioTitle();
            _bufferTimer = new DispatcherTimer(TimeSpan.FromSeconds(1), DispatcherPriority.Normal, UpdateRadioStade,
                SystemDispatcher);
            _bufferTimer.Start();

            UpdateRecordingImage();
            ResetVolume();

            if(_radioEnvironment.Settings.PlayAfterStart)
                Play();
        }

        public RadioPlayerViewModel()
            : base(RadioStreamerResources.RadioPlayerTabTitle)
        {
            
        }

        #region RadioStade

        private RadioQuality _lastQuality;
        private RadioEntry _lastRadio;
        private double _percentCache = -2;
        private Stopwatch _time = new Stopwatch();

        [NotNull]
        public string RunTime
        {
            get
            {
                TimeSpan span = _time.Elapsed;
                return $"{RadioStreamerResources.ElapsedTime} {span.Minutes}:{span.Seconds}";
            }
        }

        [NotNull]
        public RadioTitle CurrentTitle { get; private set; }

        public double PercentageBuffer
        {
            get
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (_percentCache != -2) return _percentCache;

                if (CurrentTitle.State != PlayerStade.Playing) return 0;
                _percentCache = _player.BufferPercentage;

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
                        return PercentageBuffer.ToString("F") + "%";
                    default:
                        return RadioStreamerResources.BufferPercentageInvalid;
                }
            }
        }

        private void UpdateRadioStade([NotNull] object dender, [NotNull] EventArgs e)
        {
            _percentCache = -2;

            OnPropertyChangedExplicit(nameof(RunTime));
            OnPropertyChangedExplicit(nameof(PercentageBuffer));
            OnPropertyChangedExplicit(nameof(PercentageString));
        }

        #endregion

        #region Playing

        private bool _isInitialized;

        private bool SendError([NotNull] Exception ex)
        {
            if (ex.GetType().Name != "BassException")
                return false;
            const string caption = "Error";
            string text = string.Format(RadioStreamerResources.BassErrorMessage, ex.Message);

            Dialogs.ShowMessageBox(ViewManager.GetWindow(AppConstants.MainWindowName), text, caption, MsgBoxButton.Ok,
                MsgBoxImage.Error, null);

            return true;
        }

        private bool Initialize()
        {
            if (_isInitialized) return false;

            try
            {
                string name =_radioEnvironment.Settings.DefaultDevice;

                var dev = name != "null" ? _deviceManager.FirstOrDefault(d => d.Id == name) ??
                          _deviceManager.FirstOrDefault(d => d.Name == name) : null;

                _player.Device = dev;
                _player.Activate();
            }
            catch (Exception e)
            {
                if (e.GetType().Name != "BassException")
                    throw;

                const string caption = "Error";
                string text = string.Format(RadioStreamerResources.BassInitErrorMessage, e.Message);

                Dialogs.ShowMessageBox(ViewManager.GetWindow(AppConstants.MainWindowName), text, caption,
                    MsgBoxButton.Ok,
                    MsgBoxImage.Error, null);
            }
            _isInitialized = true;
            return _isInitialized;
        }

        private void Play(RadioQuality radio, [NotNull] string script)
        {
            if (!Initialize()) return;

            if (CurrentTitle.State == PlayerStade.Playing) Stop();

            try
            {
                _player.Play(radio, _engineManager.SearchForScript(script));
                ResetVolume();
                CurrentTitle.State = PlayerStade.Playing;
                _time.Start();
            }
            catch (Exception e)
            {
                if (!SendError(e))
                    throw;
            }
        }

        private void PlayRadioEventHandler([NotNull] PlayRadioEventArgs obj)
        {
            if (obj.Quality.IsEmpty) return;
            _lastRadio = obj.Radio;
            _lastQuality = obj.Quality;

            CurrentTitle.SetUnkown(true);
            CurrentTitle.CurrentRadioTitle = obj.Radio.Name;
            Play(obj.Quality, obj.Radio.Script);
        }

        [CommandTarget]
        private bool CanPlay()
        {
            return CurrentTitle.State != PlayerStade.Playing && !_lastRadio.IsEmpty;
        }

        [CommandTarget]
        private void Play()
        {
            Play(_lastQuality, _lastRadio.Script);
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

        private string _lastRecordImage;
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

        [CommandTarget]
        private bool CanRecord()
        {
            return CurrentTitle.State == PlayerStade.Playing;
        }

        [CommandTarget]
        private void Record()
        {
            string path =_radioEnvironment.Settings.RecodingPath;
            if(string.IsNullOrEmpty(path))
                path = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic).CombinePath("Radio Streamer");
            path.CreateDirectoryIfNotExis();

            if (_player.IsRecording)
                _player.StopRecording();
            else
                _player.StartRecording(path, _radioEnvironment.Settings.EncoderProfiles.Default.Item2);
            UpdateRecordingImage();
        }

        private void UpdateRecordingImage()
        {
            const string recordImage = "RecordImage";
            const string recordActiveImage = "RecordActiveImage";

            if (_player.IsRecording && _lastRecordImage != recordActiveImage)
            {
                _lastRecordImage = recordActiveImage;
                RecordingImage = ImagesCache.ImageSources[recordActiveImage];
            }
            else if (_lastRecordImage != recordImage)
            {
                _lastRecordImage = recordImage;
                RecordingImage = ImagesCache.ImageSources[recordImage];
            }
        }

        #endregion

        #region Volume

        private const string AudioVolumeHigh = "AudioVolumeHighImage";
        private const string AudioVolumeMedium = "AudioVolumeMediumImage";
        private const string AudioVolumeLow = "AudioVolumeLowImage";
        private const string AudioVolumeMuted = "AudioVolumeMutedImage";
        private bool _isNotMuted = true;
        private string _lastVolumeImage;
        private double _volume;
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

        public double Volume
        {
            get { return _volume; }
            set
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (_volume == value) return;

                UpdateVolume(value);
                _volume = value;
                OnPropertyChanged();
            }
        }

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

            _player.Volume = volume;
        }

        private void SetVolumeImage([NotNull] string image)
        {
            if (_lastVolumeImage == image) return;

            _lastVolumeImage = image;
            VolumeImage = ImagesCache.ImageSources[image];
        }

        [CommandTarget]
        private void MuteVolume()
        {
            if (!IsNotMuted)
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

        public sealed class RadioTitle : ObservableObject
        {
            private bool _isRecording;
            private string _radioTitle;
            private PlayerStade _stade;
            private string _trackName;

            public RadioTitle()
            {
                _stade = PlayerStade.Stopped;

                _radioTitle = RadioStreamerResources.UnkownString;
                _trackName = RadioStreamerResources.UnkownString;

                _isRecording = false;
            }

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

            public void SetUnkown(bool radioTitle)
            {
                if (radioTitle)
                    _radioTitle = RadioStreamerResources.UnkownString;
                TrackName = RadioStreamerResources.UnkownString;
            }
        }
    }
}