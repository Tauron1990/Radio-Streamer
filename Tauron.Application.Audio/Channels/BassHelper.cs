using Un4seen.Bass;

namespace Tauron.Application.BassLib.Channels
{
    internal static class BassHelper
    {
        public static double GetBufferPercentage(int handle)
        {
            double progress = Bass.BASS_StreamGetFilePosition(handle, BASSStreamFilePosition.BASS_FILEPOS_WMA_BUFFER);
            if (progress == -1) // not a WMA stream, fallback to default...
                progress = Bass.BASS_StreamGetFilePosition(handle, BASSStreamFilePosition.BASS_FILEPOS_BUFFER)
                           * 100d / Bass.BASS_StreamGetFilePosition(handle, BASSStreamFilePosition.BASS_FILEPOS_END);

            return progress;

            //return Bass.BASS_StreamGetFilePosition(_handle, BASSStreamFilePosition.BASS_FILEPOS_END) / 100d
            //* Bass.BASS_StreamGetFilePosition(_handle, BASSStreamFilePosition.BASS_FILEPOS_DOWNLOAD);
        }
    }
}