#region Usings

using Tauron.Application.Controls;
using Tauron.Application.RadioStreamer.Contracts;
using Tauron.Application.Views;

#endregion

namespace Tauron.Application.RadioStreamer.Views.RadioManager
{
    /// <summary>
    ///     Interaktionslogik für RadioManager.xaml
    /// </summary>
    [ExportView(AppConstants.RadioManagerViewModelName, Order = 100)]
    public partial class RadioManagerView
    {
        public RadioManagerView()
        {
            InitializeComponent();
        }
    }
}