﻿using System;
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
    public sealed class BassMediaPlayer : IRadioPlayer, IDisposable
    {
        private static readonly string IllegalFileNamePattern = "[" + new String(Path.GetInvalidFileNameChars()) + "]";

        [Inject]
        private IEnumerable<Lazy<IPlaybackEngine, IPlaybackEngineMetadata>> _playbackEngines;
        [Inject] 
        private IEncoderProvider _encoderProvider;

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
        private bool _isPlaying;

        private readonly VisualHelper _visualHelper;
        private readonly MemoryManager _memoryManager;
        private readonly Equalizer _equalizer;

        private readonly RadioPlayerPlay _play;
        private readonly RadioPlayerStop _stop;
        private readonly RadioPlayerTitleRecived _titleRecived;

        [Inject]
        public BassMediaPlayer([NotNull] IEventAggregator aggregator)
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
            if(_playbackEngine != null)
                _playbackEngine.Free();
            if(_recorder != null)
                _recorder.Dispose();

            _nextChannel = null;
            _currentChannel = null;
            _plugins = null;
            _recorder = null;

            BassManager.Free();
        }

        public PlayerStade Play(RadioQuality radio, IScript script)
        {
            if(radio.IsEmpty || string.IsNullOrWhiteSpace(radio.Url))
                return new PlayerStade(PlayerErrorStade.NoQuality, null);

            try
            {
                _script = script;
                string url = radio.Url;
                var lazyEngine = _playbackEngines.FirstOrDefault(en => en.Metadata.Name == string.Empty);
                if (lazyEngine == null) return new PlayerStade(PlayerErrorStade.NoEngine, null);
            
                IPlaybackEngine engine = lazyEngine.Value;

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
            catch (BassException bassException)
            {
                return new PlayerStade(PlayerErrorStade.Error, bassException);
            }

            _isPlaying = true;
            return new PlayerStade(PlayerErrorStade.Success, null);
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

            if(!_isPlaying) return;

            _isPlaying = false;
            _mixer.Dispose();
            _currentChannel.Dispose();
            _nextChannel = null;
            _script = null;

            _stop.Publish(EventArgs.Empty);
        }

        public bool IsRecording
        {
            get
            {
                return _recorder != null && _recorder.IsRecording;
            }
        }

        public RecordingStade StartRecording(string location, CommonProfile profile)
        {
            if(!_isPlaying) return new RecordingStade(RecordingErrorStade.NotPlaying, null);

            try
            {
                _currentRecordingLocation = location;
                InitRecorder(profile);

                StartRecordingInternal();
            }
            catch (BassException exception)
            {
                return new RecordingStade(RecordingErrorStade.Error, exception);
            }

            return new RecordingStade(RecordingErrorStade.Sucess, null);
        }

        private void InitRecorder([CanBeNull] CommonProfile profile)
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

                // ReSharper disable once PossibleNullReferenceException
                _recorder.Encoder.OutputFile = VerifyPath(_tagInfo.title);
                _recorder.Encoder.Tags = _tagInfo;

                _recorder.Start();
            }
        }

        public void StopRecording()
        {
            StopRecordingInternal();

            if(_recorder == null) return;

            if (_recorder.Encoder != null) 
                _recorder.Encoder.Dispose();
            _recorder = null;
        }

        private void StopRecordingInternal()
        {
            if(_recorder == null) return;
            _recorder.Stop();
        }

        [NotNull]
        private string VerifyPath([NotNull] string name)
        {
            name = Regex.Replace(name, IllegalFileNamePattern, string.Empty).Trim();

            string location = Path.GetFullPath(Path.Combine(_currentRecordingLocation, name + ".mp3"));
            // ReSharper disable once AssignNullToNotNullAttribute
            var info = new DirectoryInfo(Path.GetDirectoryName(location));
            if (!info.Exists) info.Create();

            File.Delete(location);

            return location;
        }

        private bool NewRecordingTitle([NotNull] TAG_INFO info)
        {
            bool result = IsRecording;

            if(result)
                StopRecordingInternal();

            _tagInfo = info;

            if (_nextChannel != null && _recorder != null) _recorder.ChangeChannel(_nextChannel);

            return result;
        }

        public Bitmap CreateSprectrum(Spectrums playerCode, int width, int height)
        {
            _visualHelper.Width = width;
            _visualHelper.Height = height;

            return _visualHelper.CreateSpectrum(playerCode);
        }

        public double BufferPercentage
        {
            get
            {
                return _playbackEngine.BufferPercentage;
            }
        }

        public double Volume
        {
            set
            {
                _mixer.Volume = new Percentage(value, PercentMode.Default);
            }
            get
            {
                return (int)_mixer.Volume;
            }
        }

        public IEqualizer Equalizer
        {
            get
            {
                return _equalizer;
            }
        }

        public void Dispose()
        {
            Deactivate();
            _memoryManager.Dispose();
        }
    }
}
