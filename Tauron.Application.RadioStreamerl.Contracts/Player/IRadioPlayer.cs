using System.Drawing;
using Tauron.Application.BassLib.Misc;
using Tauron.Application.RadioStreamer.Contracts.Data;
using Tauron.Application.RadioStreamer.Contracts.Data.Enttitis;
using Tauron.Application.RadioStreamer.Contracts.Scripts;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Contracts.Player
{
    [PublicAPI]
    public interface IRadioPlayer
	{
		void Activate();
		void Deactivate();

        [NotNull]
        PlayerStade Play(RadioQuality radio, [CanBeNull] IScript script);
		void Stop();

		bool IsRecording { get; }

        [NotNull]
        RecordingStade StartRecording([NotNull] string location, [CanBeNull] CommonProfile profile);
		void StopRecording();

        [CanBeNull]
        Bitmap CreateSprectrum(Spectrums playerCode, int width, int height);

        double BufferPercentage { get; }

        double Volume { set; get; }

        [NotNull]
        IEqualizer Equalizer { get; }
	}
}
