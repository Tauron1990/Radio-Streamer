using System.ComponentModel;
using Tauron.Application.RadioStreamer.Contracts.Data;
using Tauron.Application.RadioStreamer.Contracts.Data.Enttitis;
using Tauron.Application.RadioStreamer.Contracts.Player.Misc;
using Tauron.Application.RadioStreamer.Contracts.Scripts;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Contracts.Player
{
    [PublicAPI]
    public interface IRadioPlayer : INotifyPropertyChanged
	{
        [CanBeNull]
        IDevice Device { get; set; }

		void Activate();
		void Deactivate();

		void Play(RadioQuality radio, [CanBeNull] IScript script);
		void Stop();

		bool IsRecording { get; }

		void StartRecording([NotNull] string location, [CanBeNull] CommonProfile profile);
		void StopRecording();

        double BufferPercentage { get; }

        double Volume { set; get; }

        bool Playing { get; }

        [NotNull]
        Equalizer Equalizer { get; }
        IPlayerStream PlayerStream { get; }
	}
}
