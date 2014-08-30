using System;
using System.Collections.Generic;
using Tauron.Application.BassLib;
using Tauron.Application.BassLib.Channels;
using Tauron.Application.BassLib.Encoder;
using Tauron.Application.BassLib.Misc;
using Un4seen.Bass.Misc;

namespace TestInternetRadio
{
    static class Program
    {
        private const string Password = "testServer";
        
        private static Queue<string> _files = new Queue<string>();
        private static List<string> _allfiles = new List<string>();
        private static Random _random = new Random(unchecked(DateTime.Now.Year + DateTime.Now.DayOfYear));

        static void Main(string[] args)
        {
            Console.Title = @"Taurons Test Streaming Server";
            Console.Write(@"Initialize Core System...  ");

            BassManager.Register("Game-over-Alexander@web.de", "2X1533726322323");
            BassManager.InitBass();

            var engine = new BassEngine();

            var mix = new Mix(flags: BassMixFlags.Nonstop);
            var lame = new LameEncoder(mix)
            {
                InputFile = null,
                OutputFile = null,
                Bitrate = BaseEncoder.BITRATE.kbps_256
            };

            var castServer = new ShoutCastServer(lame)
            {
                ServerAddress = "localhost/TestServer",
                Port = 8000,
                Password = Password,
                PublicFlag = true
            };

            var broadCast = new AudioBroadCast(castServer) {AutoReconnect = true};
            broadCast.Notification += BroadCastOnNotification;

            Console.WriteLine(@"Done");
            Console.WriteLine(@"Waiting for Startup (press Enter)");
            Console.ReadKey();

            broadCast.AutoConnect();

            
        }

        private static void BroadCastOnNotification(object sender, BroadCastEventArgs broadCastEventArgs)
        {
        }
    }
}
