using Tauron.Application.RadioStreamer.Contracts.Scripts.Tags;
using Un4seen.Bass.AddOn.Tags;

namespace Tauron.Application.RadioStreamer.Player.Tags
{
    public sealed class BassPicture : IPicture
    {
        public BassPicture()
        {
            
        }

        public BassPicture(TagPicture picture)
        {
            AttributeIndex = picture.AttributeIndex;
            MimeType = picture.MIMEType;
            Type = (PictureType)picture.PictureType;
            Description = picture.Description;
            Data = picture.Data;
            Storage = (PictureStorage) picture.PictureStorage;
        }

        public int AttributeIndex { get; set; }
        public string MimeType { get; set; }
        public PictureType Type { get; set; }
        public string Description { get; set; }
        public byte[] Data { get; set; }
        public PictureStorage Storage { get; set; }

        internal static TagPicture GeTagPicture(IPicture picture)
        {
            return new TagPicture(picture.AttributeIndex, picture.MimeType, (TagPicture.PICTURE_TYPE) picture.Type,
                picture.Description, picture.Data) {PictureStorage = (TagPicture.PICTURE_STORAGE) picture.Storage};
        }
    }
}