using System;
using Tauron.JetBrains.Annotations;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Tags;

namespace Tauron.Application.BassLib.Channels
{
    public class FileChannel : Channel
    {
        internal FileChannel(int handle) : base(handle)
        {
        }

        public override TAG_INFO Tag
        {
            get
            {
                var tags = new TAG_INFO();
                BassTags.BASS_TAG_GetFromFile(Handle, tags);
                return tags;
            }
        }

        public long Length => Bass.BASS_ChannelGetLength(Handle);

        public double Seconds => Bass.BASS_ChannelBytes2Seconds(Handle, Length);
 
        public double Progress => 100d/Length*Position;

        [NotNull]
        public IDisposable SetEntSync([NotNull] Action action)
        {
            var sync = new EndSync(action);
            SyncManager.Register(sync, 0, BASSSync.BASS_SYNC_END | BASSSync.BASS_SYNC_ONETIME);
            return sync;
        }

        protected override void Dispose(bool disposing)
        {
            Bass.BASS_StreamFree(HandleFree());
        }
    }
}