using System;
using Un4seen.Bass;

namespace Tauron.Application.BassLib
{
    public abstract class SyncBase : IDisposable
    {
        internal event Action<SyncBase, bool> End;

        private void OnEnd(bool oneTime)
        {
            var handler = End;
            if (handler != null) handler(this, oneTime);
        }

        internal readonly SYNCPROC Syncproc;

        protected internal bool OneTime { get; internal set; }

        protected internal int Handle { get; internal set; }

        protected SyncBase()
        {
            Syncproc = PrivateSyncimpl;  
        }

        private void PrivateSyncimpl(int handle, int channel, int data, IntPtr user)
        {
            Syncimpl(handle, channel, data, user);

            if(OneTime)
                OnEnd(true);
        }

        protected abstract void Syncimpl(int handle, int channel, int data, IntPtr user);

        public void Dispose()
        {
            OnEnd(false);
        }
    }
}