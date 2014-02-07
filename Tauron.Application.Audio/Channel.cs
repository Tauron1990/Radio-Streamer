using System;
using Tauron.Application.BassLib.Channels;
using Tauron.JetBrains.Annotations;
using Un4seen.Bass;

namespace Tauron.Application.BassLib
{
    [PublicAPI]
    public abstract class Channel : IDisposable
    {
        private Mix _mix;

        [CanBeNull]
        public Mix Mix
        {
            get { return _mix; }
            set
            {
                if(_mix != null)
                    _mix.DeAttach(Handle);
                if(value != null)
                    value.Attach(Handle);
                _mix = value;
            }
        }

        protected int Handle { get; private set; }

        protected Channel(int handle)
        {
            Handle = handle;
        }

        public void Play(bool restart = false)
        {
            if (!Bass.BASS_ChannelPlay(Handle, restart))
                throw new BassException();
        }

        public void Pause()
        {
            if (!Bass.BASS_ChannelPause(Handle))
                throw  new BassException();
        }

        public void Stop()
        {
            if(!Bass.BASS_ChannelStop(Handle))
                throw new BassException();
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

        protected int HandleFree()
        {
            Mix = null;
            return Handle;
        }
    }
}