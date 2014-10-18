using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Contracts.Player.Recording
{
    public interface IEncodingEditorMetadata
    {
        [NotNull]
        string EncoderId { get; } 
    }
}