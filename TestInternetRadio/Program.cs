using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tauron.Application.BassLib;
using Tauron.Application.BassLib.Channels;
using Un4seen.Bass;

namespace TestInternetRadio
{
    class Program
    {
        static void Main(string[] args)
        {
            BassManager.Register("Game-over-Alexander@web.de", "2X1533726322323");
            BassManager.IniBass();

            var engine = new BassEngine();

            var temp = new Mix();
            
        }
    }
}
