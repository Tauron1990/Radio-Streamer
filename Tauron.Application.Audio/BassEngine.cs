using Tauron.Application.BassLib.Channels;
using Tauron.JetBrains.Annotations;
using Un4seen.Bass;

namespace Tauron.Application.BassLib
{
    [PublicAPI]
    public class BassEngine
    {
        public static WebStream CreateWeb(string url, int offset = 0, WebStreamFlags flags = WebStreamFlags.Default,
                                          DownloadInterceptor download = null)
        {
            //int handle = Bass.BASS_StreamCreateURL(url, offset, (BASSFlag)flags)
            return null;
        }
    }
}