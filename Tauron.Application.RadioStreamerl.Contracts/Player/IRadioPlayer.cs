﻿using System.Drawing;
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

		void Play(RadioQuality radio, [CanBeNull] IScript script);
		void Stop();

		bool IsRecording { get; }

		void StartRecording([NotNull] string location, CommonProfile profile);
		void StopRecording();

        [CanBeNull]
        Bitmap CreateSprectrum(Spectrums playerCode, int width, int height);

        double BufferPercentage { get; }

        double Volume { set; get; }

        [NotNull]
        IEqualizer Equalizer { get; }
	}
}
