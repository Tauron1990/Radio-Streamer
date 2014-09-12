using System.Collections.Generic;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Contracts.Core
{
    public interface IPlugInManager
    {
        [NotNull]
        IEnumerable<IPackInfo> GetPackInfos(); 

        void LoadPakage([NotNull] string name);
        void InstallPlugIn([NotNull] string name);
        
        void Unistall([NotNull] string name, bool plugin);
    }
}