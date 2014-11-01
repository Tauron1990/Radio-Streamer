using Tauron.Application.RadioStreamer.Contracts.Data.Encoder;
using Tauron.Application.RadioStreamer.Contracts.Player.Recording;

namespace Tauron.Application.RadioStreamer.Views.EncodingOptions
{
    /// <summary>
    /// Interaktionslogik für LameEncodigEditor.xaml
    /// </summary>
    [ExportEncodingEditor(LameEncoderProfile.LameId)]
    public partial class LameEncodigEditor
    {
        public LameEncodigEditor()
        {
            InitializeComponent();
        }
    }
}
