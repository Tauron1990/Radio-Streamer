using Tauron.Application.Ioc;
using Tauron.Application.RadioStreamer.Contracts.Player;
using Tauron.Application.RadioStreamer.Contracts.Scripts.Tags;
using Un4seen.Bass.AddOn.Tags;

namespace Tauron.Application.RadioStreamer.Player.Tags
{
    [Export(typeof(ITagsProvider))]
    public sealed class TagProvider : ITagsProvider, INotifyBuildCompled
    {
         [Inject]
        private IEventAggregator _eventAggregator;

        private ITagInfo _currentTag;

        public ITagInfo CreateEmpty()
        {
            return new BassTag();
        }

        public ITagInfo GetFromCurrentPlay()
        {
            return _currentTag ?? CreateEmpty();
        }

        public ITagInfo GetFromFile(string file)
        {
            return new BassTag(BassTags.BASS_TAG_GetFromFile(file));
        }

        public IPicture CreatePicture()
        {
            return new BassPicture();
        }

        public IPicture CreatePicture(string file)
        {
            return new BassPicture(new TagPicture(file));
        }

        public void BuildCompled()
        {
            _eventAggregator.GetEvent<RadioPlayerNewTagEvent, ITagInfo>().Subscribe(t => _currentTag = t);
            _eventAggregator = null;
        }
    }
}