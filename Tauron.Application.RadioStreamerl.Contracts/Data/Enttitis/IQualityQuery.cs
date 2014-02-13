using System.Collections.Generic;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Contracts.Data.Enttitis
{
    [PublicAPI]
	public interface IQualityQuery : IEnumerable<RadioQuality>
	{
		int Count { get; }
		bool IsCompled { get; }

        void Reset();
	}
}
