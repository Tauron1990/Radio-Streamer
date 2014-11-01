using Tauron.Application.BassLib;
using Tauron.Application.BassLib.Encoder;
using Tauron.Application.RadioStreamer.Contracts.Data;
using Tauron.Application.RadioStreamer.Contracts.UI;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Contracts.Player.Recording
{
    public abstract class EncoderFactoryBase : IEncoderFactory
    {
        public virtual string DisplayName { get { return ID; } }
        public abstract string ID { get; }
        public virtual void RegisterOptions(IUIOptionsManager manager)
        {
            
        }

        public AudioEncoder Create(CommonProfile profile, Channel channel)
        {
            if(profile == null) return null;
            return InternalCreate(profile, channel);
        }

        [CanBeNull]
        protected abstract AudioEncoder InternalCreate([NotNull] CommonProfile profile, [NotNull] Channel channel);
    }
}