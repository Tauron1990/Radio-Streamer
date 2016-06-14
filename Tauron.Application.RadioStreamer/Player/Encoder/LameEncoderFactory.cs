using Tauron.Application.Ioc;
using Tauron.Application.RadioStreamer.Contracts;
using Tauron.Application.RadioStreamer.Contracts.Data;
using Tauron.Application.RadioStreamer.Contracts.Player;
using Tauron.Application.RadioStreamer.Contracts.Player.Recording;

namespace Tauron.Application.RadioStreamer.Player.Encoder
{
    [Export(typeof (IEncoderFactory))]
    public sealed class LameEncoderFactory : EncoderFactoryBase
    {
        public override string DisplayName => "Lame Encoder";

        public override string Id => AppConstants.LameId;

        protected override IEncoder InternalCreate(CommonProfile profile, IPlayerStream channel)
        {
            return new LameEncoderProfile(profile).CreateEncoder(channel);
        }
    }
}
