using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Views.RadioPlayer.Equalizer
{
    public class EqManagerPresent : ObservableObject
    {
        public EqManagerPresent([NotNull] string name)
        {
            _name = name;
            OriginalName = name;
            PresetType = EqManagerPresetType.Preset;
        }

        private string _name;

        [NotNull]
        public string OriginalName { get; set; }

        [NotNull]
        public string Name
        {
            get { return _name; }
            set
            {
                if (Equals(_name, value)) return;

                _name = value;

                OnPropertyChanged();
            }
        }

        public EqManagerPresetType PresetType { get; set; }
    }
}