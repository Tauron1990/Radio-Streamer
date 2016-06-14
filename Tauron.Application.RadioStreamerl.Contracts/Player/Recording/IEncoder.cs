using Tauron.Application.RadioStreamer.Contracts.Scripts.Tags;

namespace Tauron.Application.RadioStreamer.Contracts.Player.Recording
{
    public interface IEncoder
    {
        bool SupportsStdout { get; }
        IPlayerStream PlayerStream { get; set; }
        bool NoLimit { get; set; }
        string InputFile { get; set; }
        string OutputFile { get; set; }
        ITagInfo FileTags { get; set; }
        void Dispose();
        void Start(bool paused = false);
        void Stop();
    }
}