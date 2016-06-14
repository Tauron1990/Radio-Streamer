using System.Collections.Generic;
using Tauron.Application.RadioStreamer.Contracts.Player.Misc;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Contracts.Core
{
    [PublicAPI]
	public interface IEqualizerProfileDatabase
	{
        [NotNull]
        IEnumerable<string> Profiles { get; }

		void SetProfil([NotNull] string name, [NotNull] Equalizer equlizer);
		void NewProfile([NotNull] string name, [NotNull] Equalizer equlizer);

		void DeleteProfile([NotNull] string name);
	}
}
