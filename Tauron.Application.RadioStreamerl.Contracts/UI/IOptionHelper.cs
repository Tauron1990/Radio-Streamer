using Tauron.Application.RadioStreamer.Contracts.Core;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Contracts.UI
{
    public interface IOptionHelper
    {
        [NotNull]
        object LoadUI([NotNull] Option option);

        bool Serialize([NotNull] IPropertyStore store, [NotNull] Option option);

        void Deserialize([NotNull] IPropertyStore store, [NotNull] Option option, [CanBeNull] object defaultValue);
    }
}