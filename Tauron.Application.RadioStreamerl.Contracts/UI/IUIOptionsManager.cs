using System;
using System.Collections.Generic;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Contracts.UI
{
    [PublicAPI]
    public interface IUIOptionsManager
    {
        event EventHandler<OptionsChangedEventArgs> OptionsChanged;

        [NotNull]
        IEnumerable<OptionPath> Options { get; }

        void RegisterOption([NotNull]string path, [NotNull] Option option);

        void Save();
    }
}
