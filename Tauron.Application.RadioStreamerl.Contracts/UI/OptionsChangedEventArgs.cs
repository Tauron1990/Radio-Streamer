using System;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Contracts.UI
{
    public sealed class OptionsChangedEventArgs : EventArgs
    {
        [NotNull]
        public Option[] ChangedOptions { get; private set; }

        public OptionsChangedEventArgs([NotNull] Option[] changedOptions)
        {
            if (changedOptions == null) throw new ArgumentNullException("changedOption");
            ChangedOptions = changedOptions;
        }
    }
}