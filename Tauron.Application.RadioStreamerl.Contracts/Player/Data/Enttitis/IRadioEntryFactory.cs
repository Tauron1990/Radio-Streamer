using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Contracts.Data.Enttitis
{
    [PublicAPI]
    public interface IRadioEntryFactory
    {
        void BeginChanging();

        RadioEntry AddOrGetEntry([NotNull] string name, out bool newEntry);

        void Compled();
    }
}