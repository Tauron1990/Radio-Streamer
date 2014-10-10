using System;
using System.Collections.Generic;
using Un4seen.Bass;

namespace Tauron.Application.RadioStreamer.Player
{
    public class MemoryManager : IDisposable
    {
        private readonly List<int> _handels;

        public MemoryManager()
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

        public void Init()
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
        }
    }
}