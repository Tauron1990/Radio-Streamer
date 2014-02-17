using System;
using System.IO;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Mix;
using Un4seen.Bass.Misc;
using Un4seen.BassAsio;
using Un4seen.BassWasapi;

namespace ConsoleApplication
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WindowWidth = 150;

// not playing anything via BASS, so don't need an update thread
            //Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_UPDATEPERIOD, 0);

            var devices = Bass.BASS_GetDeviceInfos();
// setup BASS - "no sound" device
            if (!Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero))
            {
                var err = Bass.BASS_ErrorGetCode();
            }

            //var temp = Bass.BASS_RecordInit(-1);

            var fileInfo = new FileInfo(Path.GetFullPath("Music Is My Drug.mp3"));
            int stream = Bass.BASS_StreamCreateFile(fileInfo.FullName, 0, fileInfo.Length,
                                                     BASSFlag.BASS_STREAM_DECODE);
            if (stream != 0)
            {
                //// assign WASAPI output in shared-mode
                //var handler = new BassWasapiHandler(0, false, 48000, 2, 0f, 0f);
                //// add the source channel
                //handler.AddOutputSource(stream, BASSFlag.BASS_DEFAULT);

                ////_wasapi.SetFullDuplex()
                //// init and start WASAPI
                //handler.Init();
                //handler.Start()
                
                //var asio = new BassAsioHandler(-1, 0, stream);
                //asio.Start(0, 0);

                var encoder = new EncoderLAME(stream);
                encoder.OutputFile = "test.mp3";
                if(!encoder.Start(null, IntPtr.Zero, false))
                {
                    var err = Bass.BASS_ErrorGetCode();
                }

                var mixer = BassMix.BASS_Mixer_StreamCreate(4410, 2, BASSFlag.BASS_DEFAULT);
                if(mixer == 0)
                {
                    var err = Bass.BASS_ErrorGetCode();
                }
                if(!BassMix.BASS_Mixer_StreamAddChannel(mixer, stream, BASSFlag.BASS_DEFAULT))
                {
                    var err = Bass.BASS_ErrorGetCode();
                }
                if(!Bass.BASS_ChannelPlay(mixer, false))
                {
                    var err = Bass.BASS_ErrorGetCode();
                }
            }

            Console.ReadLine();
        }
    }
}
