using Tauron.Application.BassLib;
using Tauron.Application.RadioStreamer.Contracts.Data.Enttitis;
using Tauron.Application.RadioStreamer.Contracts.Player;

namespace Tauron.Application.RadioStreamer.Player
{
    public sealed class InternalPlayerStream : IPlayerStream
    {
        private readonly CoreMediaPlayer _mediaPlayer;
        public Channel Channel { get; set; }

        public InternalPlayerStream(CoreMediaPlayer mediaPlayer)
        {
            _mediaPlayer = mediaPlayer;
        }

        public void Start(RadioQuality quality)
        {
            _mediaPlayer.Play(quality, null);
        }

        public void Stop()
        {
            _mediaPlayer.Stop();
        }
    }
}