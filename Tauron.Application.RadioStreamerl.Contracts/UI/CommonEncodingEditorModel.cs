using System;
using Tauron.Application.Models;
using Tauron.Application.RadioStreamer.Contracts.Data;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Contracts.UI
{
    [ExportModel(AppConstants.CommonEncoderUI)]
    public sealed class CommonEncodingEditorModel : ModelBase
    {
        private CommonProfile _currentProfile;
        private string _name;

        public event EventHandler ProfileSwitched;

        private void OnProfileSwitched()
        {
            EventHandler handler = ProfileSwitched;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        [CanBeNull]
        public CommonProfile CurrentProfile
        {
            get { return _currentProfile; }
            set
            {
                _currentProfile = value;
                OnPropertyChanged();
                OnProfileSwitched();
            }
        }

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
    }
}
