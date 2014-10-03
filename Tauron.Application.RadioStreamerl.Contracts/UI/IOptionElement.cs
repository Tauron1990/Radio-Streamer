using System;
using Tauron.Application.RadioStreamer.Contracts.Core;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Contracts.UI
{
    public interface IOptionElement
    {
        [NotNull]
        string DisplayName { get; set; }

        bool Save([NotNull] IPropertyStore store);

        void Load([NotNull] IPropertyStore store);
    }
}