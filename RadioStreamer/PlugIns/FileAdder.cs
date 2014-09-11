using System;
using System.IO;

namespace Tauron.Application.RadioStreamer.PlugIns
{
    public abstract class FileAdder : IDisposable
    {
        public abstract string AddFile(string effiectivePath, Stream stream);
        public abstract void Dispose();
    }
}
