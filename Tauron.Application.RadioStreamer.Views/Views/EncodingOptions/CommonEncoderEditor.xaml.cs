using System.Windows.Controls;
using Tauron.Application.RadioStreamer.Contracts;
using Tauron.Application.Views;

namespace Tauron.Application.RadioStreamer.Views.EncodingOptions
{
    /// <summary>
    /// Interaktionslogik für CommonEncoderEditor.xaml
    /// </summary>
    [ExportView(AppConstants.CommonEncoderUI)]
    public partial class CommonEncoderEditor : UserControl
    {
        public CommonEncoderEditor()
        {
            InitializeComponent();
        }
    }
}
