using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Contracts.UI
{
    public interface IHeaderBinding
    {
        [CanBeNull]
        object Header { get; } 
    }
}