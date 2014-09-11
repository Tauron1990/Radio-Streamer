using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Contracts.Core
{
    public interface IPlugInManager
    {
        void LoadPakage([NotNull] string name);
        void InstallPlugIn([NotNull] string name);

        void Unistall([NotNull] string name, bool plugin);
    }
}