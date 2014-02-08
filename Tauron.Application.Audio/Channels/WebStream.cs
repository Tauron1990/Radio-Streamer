using System;
using Tauron.JetBrains.Annotations;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Tags;

namespace Tauron.Application.BassLib.Channels
{
    public sealed class WebStream : Channel
    {
        private class MetaSync : SyncBase
        {
            private readonly Action<bool, TAG_INFO> _updateAction;

            public MetaSync([NotNull] Action<bool, TAG_INFO> updateAction)
            {
                _updateAction = updateAction;
            }

            protected override void Syncimpl(int handle, int channel, int data, IntPtr user)
            {
                var info = new TAG_INFO();
                bool ok = BassTags.BASS_TAG_GetFromURL(channel, info);

                _updateAction(ok, info);
            }
        } 

        [UsedImplicitly(ImplicitUseKindFlags.Access)]
        private DownloadInterceptor _interceptor;

        internal WebStream(int handle, [CanBeNull] DownloadInterceptor interceptor) : base(handle)
        {
            _interceptor = interceptor;
        }

        public double BufferPercentage
        {
            get
            {
                return BassHelper.GetBufferPercentage(Handle);
            }
        }

        public Percentage PreBufferFill
        {
            get
            {
                return Bass.BASS_GetConfig(BASSConfig.BASS_CONFIG_NET_PREBUF);
            }
            set
            {
                Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_NET_PREBUF, value).CheckBass();
                OnPropertyChanged();
            }
        }

        [NotNull]
        public IDisposable SetMetaUpdate([NotNull] Action<bool, TAG_INFO> action)
        {
            if (action == null) throw new ArgumentNullException("action");

            var sync = new MetaSync(action);
            SyncManager.Register(sync, 0, BASSSync.BASS_SYNC_META);
            return sync;
        }

        [NotNull]
        public IDisposable SetDownloadEnd([NotNull] Action action)
        {
            var sync = new StreamDownloadDoneSync(action);
            SyncManager.Register(sync, 0, BASSSync.BASS_SYNC_DOWNLOAD);
            return sync;
        }

        protected override void Dispose(bool disposing)
        {
            int handle = HandleFree();

            if(handle != 0)
                Bass.BASS_StreamFree(handle);

            _interceptor = null;
        }
    }
}