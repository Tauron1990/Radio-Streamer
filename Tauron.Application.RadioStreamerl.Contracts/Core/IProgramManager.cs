using System;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Contracts.Core
{
    public interface IProgramManager
    {
        IWindow MainWindow { get; }

        bool AutoUpdate { get; set; }

        bool Shutdown { get; set; }

        void Restart();

        void ShowWindow([NotNull] string name, bool modal);
    }
}