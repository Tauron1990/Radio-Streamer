using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Contracts.Scripts
{
    [PublicAPI]
    public enum PictureType : byte
    {
        Unknown,
        Icon32,
        OtherIcon,
        FrontAlbumCover,
        BackAlbumCover,
        LeafletPage,
        Media,
        LeadArtist,
        Artists,
        Conductor,
        Orchestra,
        Composer,
        Writer,
        Location,
        RecordingSession,
        Performance,
        VideoCapture,
        ColoredFish,
        Illustration,
        BandLogo,
        PublisherLogo,
    }
}