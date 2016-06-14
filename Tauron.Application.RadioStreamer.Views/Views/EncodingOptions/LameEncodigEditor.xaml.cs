using Tauron.Application.RadioStreamer.Contracts;
using Tauron.Application.RadioStreamer.Contracts.Player.Recording;

namespace Tauron.Application.RadioStreamer.Views.EncodingOptions
{
    /// <summary>
    /// Interaktionslogik für LameEncodigEditor.xaml
    /// </summary>
    [ExportEncodingEditor(AppConstants.LameId)]
    public partial class LameEncodigEditor
    {
        public LameEncodigEditor()
        {
            InitializeComponent();
        }
    }
}
