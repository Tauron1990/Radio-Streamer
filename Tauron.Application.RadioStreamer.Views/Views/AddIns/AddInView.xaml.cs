#region Usings

using Tauron.Application.RadioStreamer.Contracts;
using Tauron.Application.Views;

#endregion

namespace Tauron.Application.RadioStreamer.Views.AddIns
{
    /// <summary>
    ///     Interaktionslogik für AddInView.xaml
    /// </summary>
    [ExportWindow(AppConstants.AddInViewModel)]
    public partial class AddInView
    {
        public AddInView()
        {
            InitializeComponent();
        }
    }
}