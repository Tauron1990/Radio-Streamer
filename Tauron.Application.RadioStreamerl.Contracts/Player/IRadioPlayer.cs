using System.Drawing;
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

		bool Activate();
		void Deactivate();

		bool Play(RadioQuality radio, IScript script);
		void Stop();

		bool SupportRecording { get; }
		bool IsRecording { get; }

		void StartRecording([NotNull] string location);
		void StopRecording();

        [NotNull]
        Bitmap CreateSprectrum([NotNull] string playerCode, int width, int height);

        [NotNull]
        string[] GetSpectrumCodes();
		double GetBufferPercentage();

		void SetVolume(double volume);
        [NotNull]
        IEqualizer GetEqualizer();
	}
}
