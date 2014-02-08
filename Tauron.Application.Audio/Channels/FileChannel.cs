using Un4seen.Bass;

namespace Tauron.Application.BassLib.Channels
{
    public class FileChannel : Channel
    {
        internal FileChannel(int handle) : base(handle)
        {
        }

        protected override void Dispose(bool disposing)
        {
            Bass.BASS_StreamFree(HandleFree());
        }
    }
}