using System;
using System.Collections.Generic;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Contracts.UI
{
    [PublicAPI]
    public interface ITabManager
    {
        event Action<ViewEntry> ViewSelected; 

        [NotNull]
         IEnumerable<ViewEntry> Views { get; }

        void RegisterView([NotNull] ViewEntry viewEntry);

        void View([NotNull] string name);
    }
}
