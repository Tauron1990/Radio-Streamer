using Tauron.Application.RadioStreamer.Contracts;
using Tauron.Application.Views;

namespace Tauron.Application.RadioStreamer.Views.Options
{
    /// <summary>
    /// Interaktionslogik für OptionsWindow.xaml
    /// </summary>
    [ExportWindow(AppConstants.OptionsViewModel)]
    public partial class OptionsWindow
    {
        public OptionsWindow()
        {
            InitializeComponent();
        }
    }
}
