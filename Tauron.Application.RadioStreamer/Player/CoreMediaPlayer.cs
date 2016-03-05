using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Tauron.Application.BassLib;
using Tauron.Application.BassLib.Channels;
using Tauron.Application.BassLib.Misc;
using Tauron.Application.BassLib.Recording;
using Tauron.Application.Ioc;
using Tauron.Application.RadioStreamer.Contracts.Core;
using Tauron.Application.RadioStreamer.Contracts.Core.Attributes;
using Tauron.Application.RadioStreamer.Contracts.Data;
using Tauron.Application.RadioStreamer.Contracts.Data.Enttitis;
using Tauron.Application.RadioStreamer.Contracts.Player;
using Tauron.Application.RadioStreamer.Contracts.Player.Recording;
using Tauron.Application.RadioStreamer.Contracts.Scripts;
using Tauron.Application.RadioStreamer.Player.Engine;
using Tauron.JetBrains.Annotations;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Tags;

namespace Tauron.Application.RadioStreamer.Player
{
    [ExportRadioPlayer]
    public sealed class CoreMediaPlayer : ObservableObject, IRadioPlayer, IDisposable
    {
        private static readonly string IllegalFileNamePattern = "[" + new String(Path.GetInvalidFileNameChars()) + "]";

        [Inject]
        private IEnumerable<Lazy<IPlaybackEngine, IPlaybackEngineMetadata>> _playbackEngines;
        [Inject] 
        private IEncoderProvider _encoderProvider;
        [InjectRadioEnviroment]
        private IRadioEnvironment _radioEnvironment;

        private Channel _currentChannel;
        private Channel _nextChannel;
        private Mix _mixer;
        private Recorder _recorder;
        private BassEngine _bassEngine;
        private Dictionary<int, string> _plugins;
        private IPlaybackEngine _playbackEngine;
        private IScript _script;
        private string _url;
        private string _currentRecordingLocation;
        private TAG_INFO _tagInfo;
        private CommonProfile _currentProfile;

        private readonly VisualHelper _visualHelper;
        private readonly MemoryManager _memoryManager;
        private readonly Equalizer _equalizer;

        private readonly RadioPlayerPlay _play;
        private readonly RadioPlayerStop _stop;
        private readonly RadioPlayerTitleRecived _titleRecived;

        [Inject]
        public CoreMediaPlayer([NotNull] IEventAggregator aggregator)
        {
            _visualHelper = new VisualHelper();
            _memoryManager = new MemoryManager();
            _equalizer = new Equalizer();

            _play = aggregator.GetEvent<RadioPlayerPlay, EventArgs>();
            _stop = aggregator.GetEvent<RadioPlayerStop, EventArgs>();
            _titleRecived = aggregator.GetEvent<RadioPlayerTitleRecived, string>();

            _memoryManager.Init();

            BassNet.Registration("Game-over-Alexander@web.de", "2X1533726322323");
        }

        public void Activate()
        {
            BassManager.InitBass();
            //BassManager.InitRecord();

            _plugins = Bass.BASS_PluginLoadDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                           "Data\\DLL".CombinePath("PlugIns")));
            _bassEngine = new BassEngine();
        }

        public void Deactivate()
        {
            if(_currentChannel == null) return;

            _currentChannel.Dispose();

            _bassEngine = null;
            _playbackEngine?.Free();
            _recorder?.Dispose();

            _nextChannel = null;
            _currentChannel = null;
            _plugins = null;
            _recorder = null;

            BassManager.Free();
        }

        public void Play(RadioQuality radio, IScript script)
        {
            _script = script;
            string url = radio.Url;
            IPlaybackEngine engine = _playbackEngines.First(en => en.Metadata.Name == string.Empty).Value;

            if (url[0] == '[')
            {
                int index = url.IndexOf(']');
                if (index != -1)
                {
                    string[] values = url.Split(new[] {']'}, 2, StringSplitOptions.RemoveEmptyEntries);
                    values[0] = values[0].Substring(1);

                    var tempEngine = _playbackEngines.FirstOrDefault(en => en.Metadata.Name == values[0]);
                    if (tempEngine != null)
                    {
                        try
                        {
                            engine = tempEngine.Value;
                            url = values[1];
                        }
                        catch (Exception)
                        {
                            url = radio.Url;
                        }
                    }
                }
            }

            BeginPlayback(url, engine);
        }

        private void BeginPlayback([NotNull] string url, [NotNull] IPlaybackEngine playbackEngine)
        {
            if (_currentChannel != null && _currentChannel.IsActive)
                Stop();

            _url = url;

            _playbackEngine = playbackEngine;
            _playbackEngine.ChannelSwitched += PlaybackEngineOnChannelSwitched;
            _playbackEngine.End += PlaybackEngineOnEnd;

            _playbackEngine.Initialize(_bassEngine, _plugins);

            TAG_INFO tags;
            _currentChannel = _playbackEngine.PlayChannel(url, out tags);
            _mixer = new Mix(flags:BassMixFlags.Software | BassMixFlags.Nonstop) {Equalizer = _equalizer};

            _currentChannel.Mix = _mixer;

            _visualHelper.Channel = _mixer;

            _mixer.Play();
            OnPropertyChanged(() => Playing);
            _play.Publish(EventArgs.Empty);
            _tagInfo = PublishTitle(tags);
        }

        private void PlaybackEngineOnEnd()
        {
            Stop();
        }

        [NotNull]
        private TAG_INFO PublishTitle([NotNull] TAG_INFO info)
        {
            string newTitle;

            if (_script != null)
                // ReSharper disable once AssignNullToNotNullAttribute
                info = _script.GetTitleInfo(_url, info, out newTitle);
            else newTitle = info.title;

            if (info == null)
                info = new TAG_INFO {title = "Unkown", artist = "Unkown"};

            if (string.IsNullOrWhiteSpace(newTitle)) newTitle = "Unkown";

            Async.StartNew(() => _titleRecived.Publish(newTitle));
            return info;
        }

        private void PlaybackEngineOnChannelSwitched([NotNull] Channel channel, [NotNull] TAG_INFO info, bool newChannel)
        {
            _nextChannel = channel;
            if (!newChannel)
                _nextChannel = null;

            info = PublishTitle(info);

            bool startRecording = NewRecordingTitle(info);

            if (_nextChannel != null)
            {
                _currentChannel.Mix = null;
                _nextChannel.Mix = _mixer;

                _currentChannel.Dispose();
                _currentChannel = _nextChannel;

                _nextChannel = null;
            }

            if (startRecording) StartRecordingInternal();
        }

        public void Stop()
        {
            _playbackEngine.ChannelSwitched -= PlaybackEngineOnChannelSwitched;
            _playbackEngine.End -= PlaybackEngineOnEnd;
            _playbackEngine.Free();

            if(IsRecording)
                StopRecording();

            _mixer.Dispose();
            OnPropertyChanged(() => Playing);
            _currentChannel.Dispose();
            _nextChannel = null;
            _script = null;

            _stop.Publish(EventArgs.Empty);
        }

        public bool IsRecording => _recorder != null && _recorder.IsRecording;

        public void StartRecording(string location, CommonProfile profile)
        {
            if (profile == null)
                return;

            _currentRecordingLocation = location;
            InitRecorder(profile);

            StartRecordingInternal();
        }

        private void InitRecorder([NotNull] CommonProfile profile)
        {
            if (_currentChannel == null) return;

            if(profile == _currentProfile && _recorder != null) return;

            if(_recorder == null)
                _recorder = new Recorder();

            _recorder.Encoder = _encoderProvider.CreateEncoder(profile, _currentChannel);
        }

        private void StartRecordingInternal()
        {
            lock (this)
            {
                if(_recorder == null) return;

                if (string.IsNullOrWhiteSpace(_currentRecordingLocation) && string.IsNullOrWhiteSpace(_tagInfo.title)) return;

                bool ok;
                string path = VerifyPath(_tagInfo.title, out ok);
                if(!ok) return;

                // ReSharper disable once PossibleNullReferenceException
                _recorder.Encoder.OutputFile = path;
                _recorder.Encoder.Tags = _tagInfo;

                _recorder.Start();
                OnPropertyChangedExplicit(nameof(IsRecording));
            }
        }

        public void StopRecording()
        {
            StopRecordingInternal();

            if(_recorder == null) return;

            _recorder.Encoder?.Dispose();
            _recorder = null;

            OnPropertyChangedExplicit(nameof(IsRecording));
        }

        private void StopRecordingInternal()
        {
            _recorder?.Stop();
        }

        [NotNull]
        private string VerifyPath([NotNull] string name, out bool ok)
        {
            name = Regex.Replace(name, IllegalFileNamePattern, string.Empty).Trim();

            string location = Path.GetFullPath(Path.Combine(_currentRecordingLocation, name + ".mp3"));
            // ReSharper disable once AssignNullToNotNullAttribute
            var info = new DirectoryInfo(Path.GetDirectoryName(location));
            if (!info.Exists) info.Create();

            if (location.ExisFile())
            {
                switch (_radioEnvironment.Settings.FileExisBehavior)
                {
                    case FileExisBehavior.Override:
                        location.DeleteFile();
                        ok = true;
                        break;
                    case FileExisBehavior.Skip:
                        ok = false;
                        break;
                    case FileExisBehavior.ReName:
                        int nameNum = 1;
                        string newLoc;
                        while (true)
                        {
                            newLoc = location + nameNum;
                            if (!newLoc.ExisFile()) break;

                            nameNum++;
                        }

                        location = newLoc;
                        ok = true;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else
                ok = true;

            return location;
        }

        private bool NewRecordingTitle([NotNull] TAG_INFO info)
        {
            bool result = IsRecording;

            if (result)
                StopRecordingInternal();

            _tagInfo = info;

            if (_nextChannel != null) _recorder?.ChangeChannel(_nextChannel);

            return result;
        }

        public Bitmap CreateSprectrum(Spectrums playerCode, int width, int height)
        {
            _visualHelper.Width = width;
            _visualHelper.Height = height;

            return _visualHelper.CreateSpectrum(playerCode);
        }

        public double BufferPercentage => _playbackEngine.BufferPercentage;

        public double Volume
        {
            set
            {
                _mixer.Volume = new Percentage(value, PercentMode.Default);
                OnPropertyChanged();
            }
            get { return (int) _mixer.Volume; }
        }

        public IEqualizer Equalizer => _equalizer;

        public bool Playing => _currentChannel != null && _currentChannel.IsActive;

        public void Dispose()
        {
            Deactivate();
            _memoryManager.Dispose();
            _recorder?.Dispose();
        }
    }
}
