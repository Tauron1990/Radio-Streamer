using System;
using Tauron.JetBrains.Annotations;
using Un4seen.Bass;

namespace Tauron.Application.BassLib
{
    public abstract class Channel : IDisposable
    {
        protected int Handle { get; private set; }

        protected Channel(int handle)
        {
            Handle = handle;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected abstract void Dispose(bool disposing);

        ~Channel()
        {
            Dispose(false);
        }
    }
}