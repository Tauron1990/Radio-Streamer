using Tauron.JetBrains.Annotations;
using Un4seen.Bass;

namespace Tauron.Application.BassLib.Channels
{
    public sealed class WebStream : Channel
    {
        [UsedImplicitly(ImplicitUseKindFlags.Access)]
        private DownloadInterceptor _interceptor;

        internal WebStream(int handle, [CanBeNull] DownloadInterceptor interceptor) : base(handle)
        {
            _interceptor = interceptor;
        }

        public int 

        protected override void Dispose(bool disposing)
        {
            int handle = HandleFree();

            if(handle != 0)
                Bass.BASS_StreamFree(handle);

            _interceptor = null;
        }
    }
}