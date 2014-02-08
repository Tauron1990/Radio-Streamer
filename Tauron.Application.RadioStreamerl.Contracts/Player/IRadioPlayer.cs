using System.Drawing;
using Tauron.Application.BassLib;
using Tauron.Application.BassLib.Misc;
using Tauron.Application.RadioStreamer.Contracts.Core;
using Tauron.Application.RadioStreamer.Contracts.Data.Enttitis;
using Tauron.Application.RadioStreamer.Contracts.Scripts;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Contracts.Player
{
    [PublicAPI]
    public interface IRadioPlayer
	{
        [NotNull]
        string GetLastError();

		void Activate();
		void Deactivate();

		void Play(RadioQuality radio, IScript script);
		void Stop();

		bool IsRecording { get; }

		void StartRecording([NotNull] string location);
		void StopRecording();

        [NotNull]
        Bitmap CreateSprectrum(Spectrums playerCode, int width, int height);

		double GetBufferPercentage();

		void SetVolume(double volume);

        [NotNull]
        IEqualizer GetEqualizer();
	}
}
