using System;
using Tauron.Application.BassLib.Encoder;
using Tauron.JetBrains.Annotations;
using Un4seen.Bass.Misc;

namespace Tauron.Application.BassLib.Misc
{
    public sealed class ShoutCastServer : AudioStreaminServer
    {
        public ShoutCastServer([NotNull] AudioEncoder encoder)
            : base(new SHOUTcast(encoder.BassEncoder))
        {
            if (encoder == null) throw new ArgumentNullException("encoder");
        }

        [CanBeNull]
        public string AdminPassword
        {
            get { return ShoutCast.AdminPassword; }
            set { ShoutCast.AdminPassword = value; }
        }

        [NotNull]
        private SHOUTcast ShoutCast
        {
            get { return (SHOUTcast) Server; }
        }

        [NotNull]
        public string ServerAddress
        {
            get { return ShoutCast.ServerAddress; }
            set { ShoutCast.ServerAddress = value; }

        }

        public int Port
        {
            get { return ShoutCast.ServerPort; }
            set { ShoutCast.ServerPort = value; }
        }

        [NotNull]
        public string Password
        {
            get { return ShoutCast.Password; }
            set { ShoutCast.Password = value; }
        }

        public bool PublicFlag
        {
            get { return ShoutCast.PublicFlag; }
            set { ShoutCast.PublicFlag = value; }
        }
    }
}
