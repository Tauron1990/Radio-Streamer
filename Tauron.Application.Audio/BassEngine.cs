using System;
using Tauron.Application.BassLib.Channels;
using Tauron.JetBrains.Annotations;
using Un4seen.Bass;

namespace Tauron.Application.BassLib
{
    [PublicAPI]
    public class BassEngine
    {
        [NotNull]
        public static WebStream CreateWeb([NotNull] string url, int offset = 0,
            WebStreamFlags flags = WebStreamFlags.Default, [CanBeNull] DownloadInterceptor download = null)
        {
            if (url == null) throw new ArgumentNullException("url");

            int handle = Bass.BASS_StreamCreateURL(url, offset, (BASSFlag) flags,
                download == null ? null : download.Downloadproc, IntPtr.Zero);
            return new WebStream(handle, download);
        }
    }
}