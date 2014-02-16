using System;
using System.IO;
using Un4seen.Bass;
using Un4seen.Bass.Misc;
using Un4seen.BassWasapi;

namespace ConsoleApplication
{
    internal class Program
    {
        private static BassWasapiHandler _wasapi;

        private static void Main(string[] args)
        {
            Console.WindowWidth = 150;



// not playing anything via BASS, so don't need an update thread
            Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_UPDATEPERIOD, 0);
// setup BASS - "no sound" device
            if (!Bass.BASS_Init(0, 48000, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero))
            {
                var err = Bass.BASS_ErrorGetCode();
            }

            int stream = Bass.BASS_StreamCreateFile(Path.GetFullPath("Music Is My Drug.mp3"), 0, 0,
                                                    BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_SAMPLE_FLOAT);
            if (stream != 0)
            {
                // assign WASAPI output in shared-mode
                _wasapi = new BassWasapiHandler(0, false, 48000, 2, 0f, 0f);
                // add the source channel
                _wasapi.AddOutputSource(stream, BASSFlag.BASS_DEFAULT);

                _wasapi.SetFullDuplex()
                // init and start WASAPI
                _wasapi.Init();
                _wasapi.Start();
            }

            Console.ReadLine();
        }
    }
}
