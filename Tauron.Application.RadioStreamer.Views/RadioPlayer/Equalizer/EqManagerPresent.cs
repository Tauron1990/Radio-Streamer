#region Usings

using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application.RadioStreamer.Views.RadioPlayer.Equalizer
{
    public class EqManagerPresent : ObservableObject
    {
        private string _name;

        public EqManagerPresent([NotNull] string name)
        {
            _name = name;
            OriginalName = name;
            PresetType = EqManagerPresetType.Preset;
        }

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

        public override string ToString()
        {
            return OriginalName;
        }
    }
}