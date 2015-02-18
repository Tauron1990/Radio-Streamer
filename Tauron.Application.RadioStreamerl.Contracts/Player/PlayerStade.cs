using System;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Contracts.Player
{
    public sealed class PlayerStade
    {
        public PlayerErrorStade PlayerErrorStade { get; private set; }

        [CanBeNull]
        public Exception Exception { get; private set; }

        public PlayerStade(PlayerErrorStade playerErrorStade, [CanBeNull] Exception exception)
        {
            PlayerErrorStade = playerErrorStade;
            Exception = exception;
        }
    }
}