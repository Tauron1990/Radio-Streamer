using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Database.Database.Formats
{
    public interface IChangedHandler
    {
        void Changed(ChangeType type,[NotNull]string key, [NotNull] string oldContent, [NotNull] string content);
    }
}