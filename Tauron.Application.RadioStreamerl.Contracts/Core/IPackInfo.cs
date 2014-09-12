using System;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Contracts.Core
{
    public interface IPackInfo
    {
        bool CanUnInstall { get; }

        void UnInstall();

        [NotNull]
        string Name { get; }

        [NotNull]
        string Description { get; }

        [NotNull]
        Version Version { get; }
    }
}