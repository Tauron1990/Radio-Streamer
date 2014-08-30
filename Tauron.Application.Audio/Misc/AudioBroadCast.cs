using System;
using Tauron.JetBrains.Annotations;
using Un4seen.Bass.AddOn.Tags;
using Un4seen.Bass.Misc;

namespace Tauron.Application.BassLib.Misc
{
    [PublicAPI]
    public sealed class AudioBroadCast
    {
        private BroadCast _broadCast;

        [NotNull]
        public AudioStreaminServer Server { get; private set; }

        public AudioBroadCast([NotNull] AudioStreaminServer server)
        {
            if (server == null) throw new ArgumentNullException("server");

            Server = server;
            _broadCast = new BroadCast(server.Server);
        }

        public bool IsConnected
        {
            get { return Server.IsConnected; }
        }

        public bool IsStarted
        {
            get { return _broadCast.IsStarted; }
        }

        public BroadCast.BROADCASTSTATUS Status
        {
            get { return _broadCast.Status; }
        }

        public bool AutomaticMode
        {
            get { return _broadCast.AutomaticMode; }
        }
        public bool AutoReconnect
        {
            get { return _broadCast.AutoReconnect; }
            set { _broadCast.AutoReconnect = value; }
        }

        public int ReconnectTimeout
        {
            get { return _broadCast.ReconnectTimeout; }
            set { _broadCast.ReconnectTimeout = value; }
        }
        
        public long TotalBytesSend
        {
            get { return _broadCast.TotalBytesSend; }
        }

        public TimeSpan TotalConnectionTime
        {
            get { return _broadCast.TotalConnectionTime; }
        }

        public event BroadCastEventHandler Notification;

        public bool AutoConnect()
        {
            return _broadCast.AutoConnect();
        }

        public bool Connect()
        {
            return _broadCast.Connect();
        }

        public bool Disconnect()
        {
            return _broadCast.Disconnect();
        }

        public bool UpdateTitle([NotNull] string song, [NotNull] string url)
        {
            if (song == null) throw new ArgumentNullException("song");
            if (url == null) throw new ArgumentNullException("url");

            return _broadCast.UpdateTitle(song, url);
        }

        public bool UpdateTitle([NotNull] TAG_INFO tag, [NotNull] string url)
        {
            if (tag == null) throw new ArgumentNullException("tag");
            if (url == null) throw new ArgumentNullException("url");

            return _broadCast.UpdateTitle(tag, url);
        }

        public int GetListeners([NotNull] string password)
        {
            if (password == null) throw new ArgumentNullException("password");
            return _broadCast.GetListeners(password);
        }

        public string GetStats(string password)
        {
            return _broadCast.GetStats(password);
        }

    }
}
