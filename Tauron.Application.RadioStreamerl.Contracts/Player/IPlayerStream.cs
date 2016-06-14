using Tauron.Application.RadioStreamer.Contracts.Data.Enttitis;

namespace Tauron.Application.RadioStreamer.Contracts.Player
{
    public interface IPlayerStream
    {
        void Start(RadioQuality quality);
        void Stop();
    }
}