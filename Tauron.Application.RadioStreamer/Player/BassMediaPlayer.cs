using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using Tauron.Application.Ioc;
using Tauron.Application.RadioStreamer.Contracts.Core;
using Tauron.Application.RadioStreamer.Contracts.Core.Attributes;
using Tauron.Application.RadioStreamer.Contracts.Data.Enttitis;
using Tauron.Application.RadioStreamer.Contracts.Player;
using Tauron.Application.RadioStreamer.Contracts.Scripts;
using Tauron.JetBrains.Annotations;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Mix;
using Un4seen.Bass.AddOn.Tags;
using Un4seen.Bass.Misc;

namespace Tauron.Application.RadioStreamer.Player
{
    [ExportRadioPlayer]
    public sealed class BassMediaPlayer : IDisposable, IRadioPlayer
    {
        private SYNCPROC _downlodCompledDelegate;
        private MemoryManager _memoryManager;
        private SYNCPROC _newMetaDelegate;

        private RadioPlayerPlay _play;
        private RadioPlayerStop _stop;
        private RadioPlayerTitleRecived _titleRecived;

        [Inject]
        public BassMediaPlayer([NotNull] IEventAggregator aggregator)
        {
            _play = aggregator.GetEvent<RadioPlayerPlay, EventArgs>();
            _stop = aggregator.GetEvent<RadioPlayerStop, EventArgs>();
            _titleRecived = aggregator.GetEvent<RadioPlayerTitleRecived, string>();

            _memoryManager = new MemoryManager();

            _visuals = new Visuals();
            BassNet.Registration("Game-over-Alexander@web.de", "2X1533726322323");

            _newMetaDelegate = NewMeta;
            _downlodCompledDelegate = DownlodCompled;
        }

        #region Implementation of IRadioPlayer

        private readonly Visuals _visuals;
        private Equalizer _eq = new Equalizer();
        private int _handle;
        private int _mixer;
        private IScript _script;
        private string _sourceUrl;
        private bool _supportRecording = true;

        public bool Activate()
        {
            _memoryManager.Init();
            bool flag = Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);

            if (flag)
            {
                Bass.BASS_PluginLoadDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                    "DLL".CombinePath("PlugIns")));
            }

            return flag;
        }

        public void Deactivate()
        {
            StopRecording();
            Stop();

            Bass.BASS_Free();
        }

        public bool Play(RadioQuality url, [NotNull] IScript script)
        {
            if (_handle != 0) return true;

            _script = script;
            _sourceUrl = url.SourceUrl;

            _handle = Bass.BASS_StreamCreateURL(url.Url, 0,
                BASSFlag.BASS_SAMPLE_FX | BASSFlag.BASS_STREAM_DECODE |
                BASSFlag.BASS_STREAM_STATUS, null, IntPtr.Zero);
            _mixer = BassMix.BASS_Mixer_StreamCreate(44100, 2, BASSFlag.BASS_SAMPLE_SOFTWARE);
            bool ok = BassMix.BASS_Mixer_StreamAddChannel(_mixer, _handle, BASSFlag.BASS_SAMPLE_FX);

            if (!ok)
            {
                Bass.BASS_StreamFree(_handle);
                return false;
            }

            Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_NET_PREBUF, 10);

            bool flag = Bass.BASS_ChannelPlay(_mixer, false);

            if (flag)
            {
                Bass.BASS_ChannelSetSync(_handle, BASSSync.BASS_SYNC_META, 0, _newMetaDelegate, IntPtr.Zero);
                Bass.BASS_ChannelSetSync(_handle, BASSSync.BASS_SYNC_DOWNLOAD, 0, _downlodCompledDelegate, IntPtr.Zero);
            }

            _play.Publish(EventArgs.Empty);

            return flag;
        }

        public void Stop()
        {
            if (_handle == 0) return;

            StopRecording();
            Bass.BASS_StreamFree(_mixer);
            Bass.BASS_StreamFree(_handle);
            _handle = 0;
            _mixer = 0;

            _stop.Publish(EventArgs.Empty);
        }

        public double GetBufferPercentage()
        {
            double progress = Bass.BASS_StreamGetFilePosition(_handle, BASSStreamFilePosition.BASS_FILEPOS_WMA_BUFFER);
            if (progress == -1) // not a WMA stream, fallback to default...
                progress = Bass.BASS_StreamGetFilePosition(_handle, BASSStreamFilePosition.BASS_FILEPOS_BUFFER)
                           *100d/Bass.BASS_StreamGetFilePosition(_handle, BASSStreamFilePosition.BASS_FILEPOS_END);

            return progress;

            //return Bass.BASS_StreamGetFilePosition(_handle, BASSStreamFilePosition.BASS_FILEPOS_END) / 100d
            //* Bass.BASS_StreamGetFilePosition(_handle, BASSStreamFilePosition.BASS_FILEPOS_DOWNLOAD);
        }

        public void SetVolume(double volume)
        {
            if (_mixer >= 0) return;

            if (volume < 0) volume = 0;
            if (volume > 100) volume = 100;

            Bass.BASS_ChannelSetAttribute(_mixer, BASSAttribute.BASS_ATTRIB_VOL, (float) (volume/100));
        }

        public IEqualizer GetEqualizer()
        {
            return _eq;
        }

        public string GetLastError()
        {
            return Bass.BASS_ErrorGetCode().ToString();
        }

        private void DownlodCompled(int handle, int channel, int data, IntPtr user)
        {
            //bool breaked = _handle != 0;
            Stop();
        }

        #region Recording

        private static readonly Color Main = Color.Black;
        private static readonly Color Sub1 = Color.DarkRed;
        private static readonly Color Sub2 = Color.LightGray;
        private WebClient _client = new WebClient();
        private string _currentName;
        private string _ilegalFileNamePattern = "[" + new String(Path.GetInvalidFileNameChars()) + "]";
        private bool _isRecording;
        private bool _isRecordingEnabled;
        private BaseEncoder _lameencoder;

        private string _location;
        private string _recordingName;

        private TAG_INFO _tag;

        public bool SupportRecording
        {
            get { return _supportRecording; }
        }

        public bool IsRecording
        {
            get { return _isRecording; }
        }

        public void StartRecording(string location)
        {
            _location = location;
            _isRecordingEnabled = true;

            Switch();
        }

        public void StopRecording()
        {
            if (!_isRecordingEnabled) return;

            _isRecordingEnabled = false;
            _isRecording = false;

            _lameencoder.Stop(true);
        }

        public Bitmap CreateSprectrum(string playerCode, int width, int height)
        {
            switch (playerCode)
            {
                case "Graph":
                    return (_visuals.CreateSpectrum(_mixer, width, height, Main, Sub1, Sub2, false, true, true));
                case "Balken":
                    return (_visuals.CreateSpectrumBean(_mixer, width, height, Main, Sub1, Sub2, 5, false, true, true));
                case "Punkt":
                    return
                        (_visuals.CreateSpectrumDot(_mixer, width, height, Main, Sub1, Sub2, 5, 3, false, true, true));
                case "Elipse":
                    return
                        (_visuals.CreateSpectrumEllipse(_mixer, width, height, Main, Sub1, Sub2, 5, 3, false, true, true));
                case "Linie":
                    return
                        (_visuals.CreateSpectrumLine(_mixer, width, height, Main, Sub1, Sub2, 5, 3, false, true, true));
                case "Linien-Spitze":
                    return
                        (_visuals.CreateSpectrumLinePeak(_mixer, width, height, Main, Sub1, Color.DarkOrange, Sub2, 5, 3,
                            3, 3, false, true, true));
                case "Welle":
                    return (_visuals.CreateSpectrumWave(_mixer, width, height, Main, Sub1, Sub2, 5, false, true, true));
                default:
                    return
                        (_visuals.CreateSpectrumText(_mixer, width, height, Main, Sub1, Sub2, "Unknow", false, true,
                            true));
            }
        }

        public string[] GetSpectrumCodes()
        {
            return new[]
            {
                "Graph",
                "Balken",
                "Punkt",
                "Elipse",
                "Linie",
                "Linien-Spitze",
                "Welle"
            };
        }

        private void InitEncode()
        {
            Bass.BASS_RecordInit(-1);

            _lameencoder = new EncoderLAME(_handle)
            {
                InputFile = null,
                NoLimit = true,
                LAME_Bitrate = (int) BaseEncoder.BITRATE.kbps_128,
                LAME_Mode = EncoderLAME.LAMEMode.JointStereo,
                LAME_TargetSampleRate = (int) BaseEncoder.SAMPLERATE.Hz_44100,
                LAME_Quality = EncoderLAME.LAMEQuality.Quality
            };
        }

        private void Switch()
        {
            string[] data = Bass.BASS_ChannelGetTagsMETA(_handle);

            string title;

            try
            {
                if (_script != null)
                {
                    _tag = _script.GetTitleInfo(_sourceUrl, data, out title);
                }
                else
                {
                    title = "Unkown";
                    _tag = new TAG_INFO {title = "Unkown", artist = "Unkown"};
                }
            }
            catch (Exception e)
            {
                title = "Error: " + e.Message;
                _tag = new TAG_INFO {title = "Unkown", artist = "Unkown"};
            }

            _currentName = title;

            if (_tag == null)
                _tag = new TAG_INFO {title = title, artist = "Unkown"};

            _tag.comment = "Encoded by Bass.Net and Lame";
            _tag.encodedby = "Bass.Net and Lame";

            _titleRecived.Publish(title);

            if (_isRecordingEnabled) StartRecording();
        }

        private void StartRecording()
        {
            lock (this)
            {
                if (string.IsNullOrWhiteSpace(_location) && string.IsNullOrWhiteSpace(_currentName)) return;

                if (_lameencoder == null) InitEncode();

                if (_isRecording)
                {
                    if (_recordingName == _currentName) return;
                    // ReSharper disable once PossibleNullReferenceException
                    _lameencoder.Stop();
                }

                _recordingName = _currentName;

                // ReSharper disable once PossibleNullReferenceException
                _lameencoder.OutputFile = VerifyPath(_currentName);
                _lameencoder.TAGs = _tag;

                _isRecording = true;
                _lameencoder.Start(null, IntPtr.Zero, false);
            }
        }

        private string VerifyPath(string name)
        {
            name = Regex.Replace(name, _ilegalFileNamePattern, string.Empty).Trim();

            string location = Path.GetFullPath(Path.Combine(_location, name + ".mp3"));
            var info = new DirectoryInfo(Path.GetDirectoryName(location));
            if (!info.Exists) info.Create();

            File.Delete(location);

            return location;
        }

        private void NewMeta(int handle, int channel, int data, IntPtr user)
        {
            Switch();
        }

        #endregion

        #endregion

        #region Implementation of IDisposable

        public void Dispose()
        {
            _memoryManager.Dispose();
            try
            {
                Bass.BASS_Free();
            }
            catch (TypeInitializationException)
            {
            }
        }

        #endregion
    }
}