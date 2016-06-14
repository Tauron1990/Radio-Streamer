using System;
using System.Collections.Generic;
using Un4seen.Bass;

namespace Tauron.Application.RadioStreamer.Player
{
    public class BassConfigurator : IDisposable
    {
        private static BassConfigurator _config;

        public static BassConfigurator Configurator => _config ?? (_config = new BassConfigurator());

        private readonly List<int> _handels;
        private bool _isInitialized;

        private BassConfigurator()
        {
            _handels = new List<int>();
        }


        public void Dispose()
        {
            foreach (int lib in _handels)
            {
                Utils.LIBFreeLibrary(lib);
            }
        }

        public void CheckStade()
        {
            lock (this)
            {
                if(_isInitialized) return;
                _isInitialized = true;
            }

            Init();
        }

        private void Init()
        {
            string[] basedlls =
            {
                "bass.dll",
                "bass_fx.dll",
                "bassenc.dll",
                "bassmix.dll",
                "OptimFROG.dll",
                "tags.dll"
            };

            string basePath = AppDomain.CurrentDomain.BaseDirectory.CombinePath("Data\\Dll");

            foreach (string dll in basedlls)
            {
                _handels.Add(Utils.LIBLoadLibrary(basePath.CombinePath(dll)));
            }

            BassNet.Registration("Game-over-Alexander@web.de", "2X1533726322323");
        }
    }
}