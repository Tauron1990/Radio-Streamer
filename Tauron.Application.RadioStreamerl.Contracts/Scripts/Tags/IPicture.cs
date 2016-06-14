using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Contracts.Scripts.Tags
{
    [PublicAPI]
    public interface IPicture
    {
        int AttributeIndex { get; set; }
        string MimeType { get; set; }
        PictureType Type { get; set; }
        string Description { get; set; }
        byte[] Data { get; set; }
        PictureStorage Storage { get; set; }
    }
}