using System;
using System.Collections;
using System.Collections.Generic;
using Tauron.Application.BassLib.Channels;
using Tauron.JetBrains.Annotations;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Tags;

namespace Tauron.Application.BassLib
{
    [PublicAPI]
    public abstract class Channel : ObservableObject, IDisposable
    {
        protected bool Equals([NotNull] Channel other)
        {
            return Handle == other.Handle;
        }

        public override int GetHashCode()
        {
            return Handle;
        }

        protected class EndSync : SyncBase
        {
            private readonly Action _action;

            public EndSync([NotNull] Action action)
            {
                if (action == null) throw new ArgumentNullException(nameof(action));

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
        protected SyncManagerList SyncManager => _syncManager ?? (_syncManager = new SyncManagerList(Handle));

        [CanBeNull]
        public Mix Mix
        {
            get { return _mix; }
            set
            {
                _mix?.DeAttach(Handle);
                value?.Attach(Handle);
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
                float vol = value;
                Bass.BASS_ChannelSetAttribute(Handle, BASSAttribute.BASS_ATTRIB_VOL, vol).CheckBass();
                OnPropertyChanged();
            }
        }

        public int Handle { get; }

        protected Channel(int handle)
        {
            if(handle == 0)
                throw  new BassException();
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

        [NotNull]
        public abstract TAG_INFO Tag
        {
            get;
        }

        public bool IsActive => Bass.BASS_ChannelIsActive(Handle) == BASSActive.BASS_ACTIVE_PLAYING;

        public long Position => Bass.BASS_ChannelGetPosition(Handle);

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
            _syncManager?.Dispose();

            Mix = null;
            _syncManager = null;

            return Handle;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Channel) obj);
        }
    }
}