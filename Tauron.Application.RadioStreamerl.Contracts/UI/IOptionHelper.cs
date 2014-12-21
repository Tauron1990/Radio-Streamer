using Tauron.Application.RadioStreamer.Contracts.Core;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Contracts.UI
{
    public interface IOptionHelper
    {
        [NotNull]
        object LoadUI([NotNull] Option option);

        bool Serialize([NotNull] IRadioEnvironment store, [NotNull] Option option);

        void Deserialize([NotNull] IRadioEnvironment store, [NotNull] Option option, [CanBeNull] object defaultValue);

        void Reset([NotNull] Option option);
    }
}