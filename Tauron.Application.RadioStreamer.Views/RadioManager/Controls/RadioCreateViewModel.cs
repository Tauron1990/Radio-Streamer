using System.Globalization;
using JetBrains.Annotations;
using Tauron.Application.Ioc;
using Tauron.Application.Models;
using Tauron.Application.RadioStreamer.Contracts;
using Tauron.Application.RadioStreamer.Contracts.Data;
using Tauron.Application.RadioStreamer.Contracts.Data.Enttitis;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Views.RadioManager.Controls
{
    [ExportViewModel(AppConstants.RadioCreateViewModel)]
    public class RadioCreateViewModel : ViewModelBase, INotifyBuildCompled
    {
        public class RadioEditModel : ObservableObject
        {
            private string _name;

            [CanBeNull]
            public string Name
            {
                get { return _name; }
                set
                {
                    _name = value;
                    OnPropertyChanged();
                }
            }

            private string _genre;

            [CanBeNull]
            public string Genre
            {
                get { return _genre; }
                set
                {
                    _genre = value;
                    OnPropertyChanged();
                }
            }

            private string _country;

            [CanBeNull]
            public string Country
            {
                get { return _country; }
                set
                {
                    _country = value;
                    OnPropertyChanged();
                }
            }

            private string _language;

            [CanBeNull]
            public string Language
            {
                get { return _language; }
                set
                {
                    _language = value;
                    OnPropertyChanged();
                }
            }

            private string _description;

            [CanBeNull]
            public string Description
            {
                get { return _description; }
                set
                {
                    _description = value;
                    OnPropertyChanged();
                }
            }
        }

        [Inject("RadioEntry")]
        private RadioEntry _entry;

        [Inject]
        private IRadioDatabase _radioDatabase;

        private RadioEditModel _model;

        [NotNull]
        public RadioEditModel Model
        {
            get { return _model; }
            set { _model = value; OnPropertyChanged();}
        }


        void INotifyBuildCompled.BuildCompled()
        {
            if (_entry.IsEmpty)
            {
                Model = new RadioEditModel {Language = CultureInfo.CurrentCulture.DisplayName};
            }

            Model = new RadioEditModel
            {
                Country = _entry.Country,
                Description = _entry.Description,
                Genre = _entry.Genre,
                Language = _entry.Language,
                Name = _entry.Name
            };
        }
    }
}