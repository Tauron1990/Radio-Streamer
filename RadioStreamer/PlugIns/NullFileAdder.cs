using System.IO;

namespace Tauron.Application.RadioStreamer.PlugIns
{
    public sealed class NullFileAdder : FileAdder
    {
        public override string AddFile(string effiectivePath, Stream stream)
        {
            return null;
        }

        public override void Dispose()
        {
        
        }
    }
}
