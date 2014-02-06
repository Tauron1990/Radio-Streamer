using System;
using Tauron.JetBrains.Annotations;
using Un4seen.Bass;

namespace Tauron.Application.BassLib
{
    [PublicAPI]
    public class DeviceManager
    {
        private readonly bool _forRecord;

        [NotNull]
        public BASS_DEVICEINFO[] Deviceinfos { get; private set; }

        public DeviceManager(bool forRecord)
        {
            _forRecord = forRecord;
            Deviceinfos = forRecord ? Bass.BASS_RecordGetDeviceInfos() : Bass.BASS_GetDeviceInfos();
        }

        public void Switch([CanBeNull] BASS_DEVICEINFO device)
        {
            Bass.BASS_SetDevice(device == null ? 0 : GetDevice(device)).CheckBass();
        }

        internal int GetDevice([NotNull] BASS_DEVICEINFO deviceinfo)
        {
            return Array.IndexOf(Deviceinfos, deviceinfo);
        }
    }
}