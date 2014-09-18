#region Usings

using Tauron.Application.RadioStreamer.Contracts;
using Tauron.Application.Views;

#endregion

namespace Tauron.Application.RadioStreamer.Views.RadioManager.Controls
{
    /// <summary>
    ///     Interaktionslogik für RadioCreateView.xaml
    /// </summary>
    [ExportWindow(AppConstants.RadioCreateViewModel)]
    public partial class RadioCreateView
    {
        public RadioCreateView()
        {
            InitializeComponent();
        }
    }
}