using System;
using System.Collections.Generic;
using System.Linq;
using Tauron.Application.BassLib;
using Tauron.Application.BassLib.Channels;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Tags;

namespace Tauron.Application.RadioStreamer.Player.Engine.Impl
{
    [ExportPlaybackEngine("GenericOffline")]
    public sealed class FilePlayer : PlaybackEngineBase
    {
        private static readonly FileFlags DefaultFileFlags = FileFlags.Decode | FileFlags.Fx | FileFlags.Software;

        private Queue<string> _files; 
        private BassEngine _bassEngine;
        private string[] _supportetExtensions;
        private FileChannel _currentChannel;

        public override double BufferPercentage
        {
            get
            {
                return _currentChannel.Progress;
            }
        }

        public override void Initialize(BassEngine engine, Dictionary<int, string> plugins)
        {
            _bassEngine = engine;
            _files = new Queue<string>();
            _supportetExtensions = Utils.BASSAddOnGetSupportedFileExtensions(plugins, true)
                                        .Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < _supportetExtensions.Length; i++)
            {
                _supportetExtensions[i] = _supportetExtensions[i].Remove(0, 1);
            }
        }

        public override Channel PlayChannel(string url, out TAG_INFO tags)
        {
            if(url.ExisFile())
                _files.Enqueue(url);
            else if (url.ExisDirectory())
            {
                foreach (
                    var file in
                        url.EnumerateAllFiles().Where(file => _supportetExtensions.Contains(file.GetExtension())))
                {
                    _files.Enqueue(file);
                }
            }
            else throw new BassException(BASSError.BASS_ERROR_FILEOPEN);

            string first = _files.Dequeue();

            _currentChannel = _bassEngine.CreateFile(first, flags: DefaultFileFlags);
            _currentChannel.SetEntSync(FileEnd);

            tags = _currentChannel.Tag;
            return _currentChannel;
        }

        private void FileEnd()
        {
            if(_files == null) return;
            if (_files.Count == 0)
            {
                OnEnd();
                return;
            }
            string first = _files.Dequeue();

            _currentChannel = _bassEngine.CreateFile(first, flags: DefaultFileFlags);
            _currentChannel.SetEntSync(FileEnd);

            var tags = _currentChannel.Tag;
            OnChannelSwitched(_currentChannel, tags);
        }

        public override void Free()
        {
            _bassEngine = null;
            _files = null;
        }
    }
}