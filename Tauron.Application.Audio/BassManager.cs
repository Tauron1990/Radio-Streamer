using System;
using Tauron.JetBrains.Annotations;
using Un4seen.Bass;

namespace Tauron.Application.BassLib
{
    [PublicAPI]
    public static class BassManager
    {
        public static readonly DeviceManager SoundDevice = new DeviceManager(false);

        public static bool IsInitialized { get; private set; }

        public static void IniBass([CanBeNull] BASS_DEVICEINFO deviceinfo = null, int frequency = 44100, BASSInit flags = BASSInit.BASS_DEVICE_DEFAULT)
        {
            if (IsInitialized) return;

            Bass.BASS_Init(deviceinfo == null ? -1 : SoundDevice.GetDevice(deviceinfo), frequency, flags, IntPtr.Zero)
                .CheckBass();

            IsInitialized = true;
        }

        public static void Register([NotNull] string email, [NotNull] string key)
        {
            BassNet.Registration(email, key);
        }

        public static void Free()
        {
            if(!IsInitialized) return;

            Bass.BASS_Free().CheckBass();

            IsInitialized = false;
        }
    }
}