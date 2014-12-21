namespace Tauron.Application.RadioStreamer.Views.EncodingOptions
{
    public interface IEncoderViewModel
    {
        void Serialize();
        void Deserialize();
        void Reset();
    }
}