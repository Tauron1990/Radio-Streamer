using System;
using Tauron.JetBrains.Annotations;
using Un4seen.Bass.Misc;

namespace Tauron.Application.BassLib.Misc
{
    [PublicAPI]
    public abstract class AudioStreaminServer
    {
        private readonly IStreamingServer _server;

        [NotNull]
        public IStreamingServer Server {get { return _server; }}

        protected AudioStreaminServer([NotNull] IStreamingServer server)
        {
            if (server == null) throw new ArgumentNullException("server");
            _server = server;
        }

        public bool UseBASS { get { return _server.UseBASS; } }
        public bool IsConnected { get { return _server.IsConnected; } }
    }
}
