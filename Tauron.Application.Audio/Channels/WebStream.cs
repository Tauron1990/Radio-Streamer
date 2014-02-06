using Tauron.JetBrains.Annotations;
using Un4seen.Bass;

namespace Tauron.Application.BassLib.Channels
{
    public class WebStream : Channel
    {
        [UsedImplicitly(ImplicitUseKindFlags.Access)]
        private DownloadInterceptor _interceptor;

        internal WebStream(int handle, DownloadInterceptor interceptor) : base(handle)
        {
            _interceptor = interceptor;
        }

        protected override void Dispose(bool disposing)
        {
            Bass.BASS_StreamFree(Handle);
            _interceptor = null;
        }
    }
}