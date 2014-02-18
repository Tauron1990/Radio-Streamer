using System.Collections.Generic;
using Tauron.Application.BassLib;
using Tauron.Application.BassLib.Channels;
using Tauron.JetBrains.Annotations;
using Un4seen.Bass.AddOn.Tags;

namespace Tauron.Application.RadioStreamer.Player.Engine.Impl
{
    [ExportPlaybackEngine("")]
    public sealed class WebRadio : PlaybackEngineBase
    {
        private BassEngine _engine;
        private WebStream _stream;

        public override double BufferPercentage
        {
            get
            {
                return _stream.BufferPercentage;
            }
        }

        public override void Initialize(BassEngine engine, Dictionary<int, string> plugins)
        {
            _engine = engine;
        }

        public override Channel PlayChannel(string url, out TAG_INFO tags)
        {
            _stream = _engine.CreateWeb(url, flags: WebStreamFlags.Decode | WebStreamFlags.Fx | WebStreamFlags.Status);
            _stream.PreBufferFill = 10;
            
            tags = _stream.Tag;

            _stream.SetMetaUpdate(MetaSync);
            _stream.SetDownloadEnd(OnEnd);

            return _stream;
        }

        private void MetaSync(bool arg1, [CanBeNull] TAG_INFO arg2)
        {
            if (!arg1) arg2 = null;

            OnChannelSwitched(_stream, arg2, false);
        }

        public override void Free()
        {
            _engine = null;
            _stream = null;
        }
    }
}