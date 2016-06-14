using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Contracts.Scripts.Tags
{
    [PublicAPI]
    public interface ITagsProvider
    {
        ITagInfo CreateEmpty();
        ITagInfo GetFromCurrentPlay();
        ITagInfo GetFromFile(string file);

        IPicture CreatePicture();
        IPicture CreatePicture(string file);
    }
}