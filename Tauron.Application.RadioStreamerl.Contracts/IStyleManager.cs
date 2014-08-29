using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Contracts
{
    public interface IStyleManager
    {
        [NotNull]
        string CurrentTheme { get; }

        [NotNull]
        string[] Themes { get; }

        void SetTheme([CanBeNull] string name);

        void LoadResources();
    }
}