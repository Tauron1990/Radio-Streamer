using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Contracts.Scripts
{
    [PublicAPI]
    public sealed class Picture
    {
        public int AttributeIndex { get; set; } = -1;
        public string MimeType { get; set; }
        public PictureType Type { get; set; }
        public string Description { get; set; }
        public byte[] Data { get; set; }
        public PictureStorage Storage { get; set; }
    }
}