using Tauron.Application.RadioStreamer.Contracts.Player.Misc;
using Un4seen.Bass;

namespace Tauron.Application.RadioStreamer.Player
{
    public sealed class Device : IDevice
    {
        internal readonly BASS_DEVICEINFO Info;

        public Device(BASS_DEVICEINFO info)
        {
            Info = info;
        }

        public string Name => Info.name;
        public string Id => Info.id;
    }
}
