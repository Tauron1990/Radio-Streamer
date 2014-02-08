using System;
using System.Collections;
using System.Collections.Generic;
using Tauron.Application.BassLib.Channels;
using Tauron.JetBrains.Annotations;
using Un4seen.Bass;

namespace Tauron.Application.BassLib
{
    [PublicAPI]
    public abstract class Channel : ObservableObject, IDisposable
    {
        protected class StreamDownloadDoneSync : SyncBase
        {
            private readonly Action _action;

            public StreamDownloadDoneSync([NotNull] Action action)
            {
                if (action == null) throw new ArgumentNullException("action");

                _action = action;
            }

            protected override void Syncimpl(int handle, int channel, int data, IntPtr user)
            {
                _action();
            }
        }

        protected class SyncManagerList : IEnumerable<SyncBase>
        {
            private readonly int _handle;
            private readonly List<SyncBase> _syncBases; 

            public SyncManagerList(int handle)
            {
                _handle = handle;
                _syncBases = new List<SyncBase>();
            }

            public void Register([NotNull] SyncBase syncBase, long data, BASSSync sync)
            {
                bool oneTime = sync.HasFlag(BASSSync.BASS_SYNC_ONETIME);

                syncBase.Handle = Bass.BASS_ChannelSetSync(_handle, sync, data, syncBase.Syncproc, IntPtr.Zero);
                if(syncBase.Handle == 0)
                    throw new BassException();

                syncBase.End += SyncBaseOnEnd;
                syncBase.OneTime = oneTime;
                _syncBases.Add(syncBase);
            }

            private void SyncBaseOnEnd([NotNull] SyncBase sync, bool b1)
            {
                _syncBases.Remove(sync);
                if (!b1 && sync.Handle != 0) Bass.BASS_ChannelRemoveSync(_handle, sync.Handle);
            }

            public void Dispose()
            {
                _syncBases.Clear();
            }

            public IEnumerator<SyncBase> GetEnumerator()
            {
                return _syncBases.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        private Mix _mix;
        private SyncManagerList _syncManager;

        [NotNull]
        protected SyncManagerList SyncManager
        {
            get { return _syncManager ?? (_syncManager = new SyncManagerList(Handle)); }
        }

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

        public Percentage Volume
        {
            get
            {
                float value = 0;
                Bass.BASS_ChannelGetAttribute(Handle, BASSAttribute.BASS_ATTRIB_VOL, ref value).CheckBass();

                return value;
            }
            set
            {
                Bass.BASS_ChannelSetAttribute(Handle, BASSAttribute.BASS_ATTRIB_VOL, value).CheckBass();
                OnPropertyChanged();
            }
        }

        public int Handle { get; private set; }

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
            if (_syncManager != null)
                _syncManager.Dispose();

            Mix = null;
            _syncManager = null;

            return Handle;
        }
    }
}