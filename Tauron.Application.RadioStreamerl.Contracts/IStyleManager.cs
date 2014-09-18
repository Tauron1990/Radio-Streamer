using System;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Contracts
{
    public interface IStyleManager
    {
        event EventHandler ThemeChanged;

        [NotNull]
        string CurrentTheme { get; }

        [NotNull]
        string[] Themes { get; }

        void SetTheme([CanBeNull] string name);

        void LoadResources();
    }
}