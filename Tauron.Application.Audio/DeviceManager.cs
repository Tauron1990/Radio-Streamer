using Tauron.JetBrains.Annotations;
using Un4seen.Bass;

namespace Tauron.Application.RadioStreamer.Player.Core
{
    [PublicAPI]
    public class DeviceManager
    {
        private readonly bool _forRecord;

        [NotNull]
        public BASS_DEVICEINFO[] Deviceinfos { get; set; }

        public DeviceManager(bool forRecord)
        {
            _forRecord = forRecord;
            Deviceinfos = forRecord ? Bass.BASS_RecordGetDeviceInfos() : Bass.BASS_GetDeviceInfos();
        }

        public void Switch([NotNull] BASS_DEVICEINFO device)
        {

        }
    }
}