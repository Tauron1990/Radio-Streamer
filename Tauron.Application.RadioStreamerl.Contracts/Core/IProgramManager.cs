using System;
using System.Globalization;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Contracts.Core
{
    public interface IProgramManager
    {
        IWindow MainWindow { get; }

        CultureInfo CultureInfo { get; set; }

        bool AutoUpdate { get; set; }

        bool Shutdown { get; set; }

        void Restart();

        void ShowWindow([NotNull] string name, bool modal);
    }
}