using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Contracts.Core
{
    public interface IProgramManager
    {
        void Restart();

        void ShowWindow([NotNull] string name, bool modal);
    }
}