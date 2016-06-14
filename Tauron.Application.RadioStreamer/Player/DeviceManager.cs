using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tauron.Application.Ioc;
using Tauron.Application.RadioStreamer.Contracts.Player.Misc;
using Un4seen.Bass;

namespace Tauron.Application.RadioStreamer.Player
{
    [Export(typeof (IDeviceManager))]
    public sealed class DeviceManager : IDeviceManager
    {
        private readonly List<Device> _devices = new List<Device>();
        private BassConfigurator _configurator;

        public DeviceManager()
        {
            _configurator = BassConfigurator.Configurator;
            _configurator.CheckStade();

            foreach (var info in Bass.BASS_GetDeviceInfos().Where(info => info.IsEnabled))
            {
                _devices.Add(new Device(info));
            }
        }

        public IEnumerator<IDevice> GetEnumerator()
        {
            return _devices.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
