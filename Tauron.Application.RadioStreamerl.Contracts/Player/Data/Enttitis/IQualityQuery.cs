using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Contracts.Data.Enttitis
{
    [PublicAPI]
	public interface IQualityQuery : IEnumerable<RadioQuality>, INotifyCollectionChanged
    {
        event Action ValueChangedEvent;

		int Count { get; }
		bool IsCompled { get; }

        void Reset();
	}
}
