using System.Collections.Generic;
using System.Linq;
using Tauron.Application.BassLib;
using Tauron.Application.BassLib.Encoder;
using Tauron.Application.BassLib.Recording;
using Tauron.Application.Ioc;
using Tauron.Application.RadioStreamer.Contracts.Data;
using Tauron.Application.RadioStreamer.Contracts.Player.Recording;
using Tauron.Application.RadioStreamer.Contracts.UI;
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
            get { return _encoderFactories.Select(ef => ef.ID); }
        }

        public AudioEncoder CreateEncoder(CommonProfile profile,Channel channel)
        {
            if (profile == null) return CreateDefault(channel);

            IEncoderFactory fac = _encoderFactories.FirstOrDefault(f => f.ID == profile.Id);
            if (fac == null) return CreateDefault(channel);

            var rec = fac.Create(profile, channel) ?? CreateDefault(channel);

            return rec;
        }

        [NotNull]
        private static AudioEncoder CreateDefault([NotNull] Channel channel)
        {
            var temp = Recorder.CreateLame(channel);

            temp.InputFile = null;
            temp.NoLimit = true;
            temp.Bitrate = BaseEncoder.BITRATE.kbps_128;
            temp.Mode = EncoderLAME.LAMEMode.Default;
            temp.TargetSampleRate = (int) BaseEncoder.SAMPLERATE.Hz_44100;
            temp.Quality = EncoderLAME.LAMEQuality.Quality;

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
