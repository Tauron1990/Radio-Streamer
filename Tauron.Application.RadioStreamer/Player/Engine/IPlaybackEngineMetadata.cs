using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Player.Engine
{
    public interface IPlaybackEngineMetadata
    {
        [NotNull]
        string Name { get; } 
    }
}