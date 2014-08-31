using System;
using Tauron.Application.BassLib.Channels;
using Tauron.JetBrains.Annotations;
using Un4seen.Bass;

namespace Tauron.Application.BassLib
{
    [PublicAPI]
    public class BassEngine
    {
        public BassEngine()
        {
            if(!BassManager.IsInitialized)
                throw new BassException(BASSError.BASS_ERROR_INIT);
        }

        [NotNull]
        public WebStream CreateWeb([NotNull] string url, int offset = 0,
            WebStreamFlags flags = WebStreamFlags.Default, [CanBeNull] DownloadInterceptor download = null)
        {
            if (url == null) throw new ArgumentNullException("url");

            int handle = Bass.BASS_StreamCreateURL(url, offset, (BASSFlag) flags,
                download == null ? null : download.Downloadproc, IntPtr.Zero);
            return new WebStream(handle, download);
        }

        [NotNull]
        public FileChannel CreateFile([NotNull] string file, long offset = 0, long lenght = 0,
                                      FileFlags flags = FileFlags.Default)
        {
            if (file == null) throw new ArgumentNullException("file");

            int handle = Bass.BASS_StreamCreateFile(file, offset, lenght, (BASSFlag) flags);
            return new FileChannel(handle);
        }

        [NotNull]
        public string GetLastError()
        {
            return BassMessages.ResourceManager.GetString(Bass.BASS_ErrorGetCode().ToString()) ??
                   BassMessages.ErrorUnkown;
        }
    }
}