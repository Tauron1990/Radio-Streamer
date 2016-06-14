using Tauron.Application.RadioStreamer.Contracts.Data;
using Tauron.Application.RadioStreamer.Contracts.Player;
using Tauron.Application.RadioStreamer.Contracts.Player.Recording;
using Tauron.Application.RadioStreamer.Contracts.UI;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Player.Encoder
{
    public abstract class EncoderFactoryBase : IEncoderFactory
    {
        public virtual string DisplayName => Id;
        public abstract string Id { get; }
        public virtual void RegisterOptions(IUIOptionsManager manager)
        {
            
        }

        public IEncoder Create(CommonProfile profile, IPlayerStream channel)
        {
            if(profile == null) return null;
            return InternalCreate(profile, channel);
        }

        [CanBeNull]
        protected abstract IEncoder InternalCreate([NotNull] CommonProfile profile, [NotNull] IPlayerStream channel);
    }
}