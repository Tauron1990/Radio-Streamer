using System.Collections.Generic;
using System.Linq;
using Tauron.Application.Ioc;
using Tauron.Application.RadioStreamer.Contracts.Data;
using Tauron.Application.RadioStreamer.Contracts.Player;
using Tauron.Application.RadioStreamer.Contracts.Player.Recording;
using Tauron.Application.RadioStreamer.Contracts.UI;
using Tauron.Application.RadioStreamer.Player.Encoder;
using Tauron.JetBrains.Annotations;
using Un4seen.Bass.Misc;

namespace Tauron.Application.RadioStreamer.Player
{
    [Export(typeof (IEncoderProvider))]
    public sealed class EncoderProvider : IEncoderProvider, INotifyBuildCompled
    {
        [Inject]
        private IEncoderFactory[] _encoderFactories;

        [Inject]
        private IUIOptionsManager _optionsManager;

        public IEnumerable<string> EncoderIds
        {
            get { return _encoderFactories.Select(ef => ef.Id); }
        }

        public IEncoder CreateEncoder(CommonProfile profile, IPlayerStream channel)
        {
            if (profile == null) return CreateDefault(channel);

            IEncoderFactory fac = _encoderFactories.FirstOrDefault(f => f.Id == profile.Id);
            if (fac == null) return CreateDefault(channel);

            var rec = fac.Create(profile, channel) ?? CreateDefault(channel);

            return rec;
        }

        [NotNull]
        private static IEncoder CreateDefault([NotNull] IPlayerStream channel)
        {
            var temp = new LameEncoder(channel)
            {
                InputFile = null,
                NoLimit = true,
                Bitrate = BaseEncoder.BITRATE.kbps_128,
                Mode = EncoderLAME.LAMEMode.Default,
                TargetSampleRate = (int) BaseEncoder.SAMPLERATE.Hz_44100,
                Quality = EncoderLAME.LAMEQuality.Quality
            };


            return temp;
        }

        public void BuildCompled()
        {
            foreach (var encoderFactory in _encoderFactories)
            {
                encoderFactory.RegisterOptions(_optionsManager);
            }
        }
    }
}
