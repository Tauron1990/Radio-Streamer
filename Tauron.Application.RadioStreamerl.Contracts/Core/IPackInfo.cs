using System;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Contracts.Core
{
    public interface IPackInfo
    {
        [NotNull]
        string Name { get; }

        [NotNull]
        string Description { get; }

        [NotNull]
        Version Version { get; }
    }

    public interface IInstalledPackInfo : IPackInfo
    {
        bool CanUnInstall { get; }

        void UnInstall();
    }

    public interface IInstallablePackInfo : IPackInfo
    {
        void Install();
    }
}