using System;
using Tauron.JetBrains.Annotations;
using Un4seen.Bass.Misc;

namespace Tauron.Application.BassLib.Encoder
{
    public abstract class Encoder
    {
        private readonly BaseEncoder _encoder;

        protected Encoder([NotNull] BaseEncoder encoder)
        {
            if (encoder == null) throw new ArgumentNullException("encoder");
            _encoder = encoder;
        }

        [NotNull]
        internal BaseEncoder BassEncoder
        {
            get { return _encoder; }
        }
    }
}
