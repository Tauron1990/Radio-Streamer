using Tauron.Application.BassLib;
using Tauron.Application.BassLib.Encoder;
using Tauron.Application.Ioc;
using Tauron.Application.RadioStreamer.Contracts.Data;
using Tauron.Application.RadioStreamer.Contracts.Data.Encoder;
using Tauron.Application.RadioStreamer.Contracts.Player.Recording;

namespace Tauron.Application.RadioStreamer.Database.Encoder
{
    [Export(typeof (IEncoderFactory))]
    public sealed class LameEncoderFactory : EncoderFactoryBase
    {
        public override string DisplayName
        {
            get { return "Lame Encoder"; }
        }

        public override string ID
        {
            get { return LameEncoderProfile.LameId; }
        }

        protected override AudioEncoder InternalCreate(CommonProfile profile, Channel channel)
        {
            return new LameEncoderProfile(profile).CreateEncoder(channel);
        }
    }
}
