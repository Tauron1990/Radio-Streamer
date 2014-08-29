using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Contracts.Core
{
    public interface IPlugInManager
    {
        void LoadPakage([NotNull] string name);
        void LoadPlugIn([NotNull] string name);
    }
}