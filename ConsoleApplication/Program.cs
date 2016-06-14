using System;
using System.DirectoryServices;
using System.IO;
using Un4seen.Bass;

namespace ConsoleApplication
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WindowWidth = 150;
            Console.Title = "Test Application";

            BassNet.Registration("Game-over-Alexander@web.de", "2X1533726322323");
            bool temp  = Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);

            var plugins = Bass.BASS_PluginLoadDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "plugins"));

            //var ch = Bass.BASS_StreamCreateURL()
        }
    }
}
