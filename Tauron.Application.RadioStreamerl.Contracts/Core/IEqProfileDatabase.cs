using System.Collections.Generic;
using Tauron.Application.BassLib.Misc;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Contracts.Core
{
    [PublicAPI]
	public interface IEqualizerProfileDatabase
	{
        [NotNull]
        IEnumerable<string> Profiles { get; }

		void SetProfil([NotNull] string name, [NotNull] IEqualizer equlizer);
		void NewProfile([NotNull] string name, [NotNull] IEqualizer equlizer);

		void DeleteProfile([NotNull] string name);
	}
}
