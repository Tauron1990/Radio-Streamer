using Tauron.Application.Ioc;
using Tauron.Application.Models;
using Tauron.Application.RadioStreamer.Contracts;
using Tauron.Application.RadioStreamer.Contracts.Data.Encoder;
using Tauron.Application.RadioStreamer.Contracts.UI;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Views.EncodingOptions
{
    [ExportViewModel(LameEncoderProfile.LameId)]
    [NotShared]
    public sealed class LameEncodingEditorViewModel : ViewModelBase
    {
        [InjectModel(AppConstants.CommonEncoderUI)]
        private CommonEncodingEditorModel _editorModel;

        private bool? _useCustomOptionsOnly;

        [NotNull]
        public LameEncoderProfile EncoderProfile { get; set; }

        public bool? UseCustomOptionsOnly
        {
            get { return _useCustomOptionsOnly; }
            set
            {
                EncoderProfile.UseCustomOptionsOnly = value == true;
                _useCustomOptionsOnly = value;
                OnPropertyChanged();
            }
        }

        public override void BuildCompled()
        {
            EncoderProfile = new LameEncoderProfile(_editorModel.CurrentProfile);
        }
    }
}
