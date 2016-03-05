using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Contracts.Scripts
{
    [PublicAPI]
    public enum PictureStorage : byte
    {
        Internal,
        External,
    }
}