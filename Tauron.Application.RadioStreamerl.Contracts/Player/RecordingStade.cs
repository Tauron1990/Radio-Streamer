using System;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Contracts.Player
{
    public sealed class RecordingStade
    {
        public RecordingErrorStade ErrorStade { get; private set; }

        [CanBeNull]
        public Exception Exception { get; private set; }

        public RecordingStade(RecordingErrorStade errorStade, [CanBeNull] Exception exception)
        {
            ErrorStade = errorStade;
            Exception = exception;
        }
    }
}