using System.Windows.Controls;
using Tauron.Application.Controls;
using Tauron.Application.RadioStreamer.Contracts;
using Tauron.Application.RadioStreamer.Resources;
using Tauron.Application.Views;

namespace Tauron.Application.RadioStreamer.Views.RadioPlayer.ShutdownAutimatic
{
    /// <summary>
    /// Interaktionslogik für ShutdownAutomaticView.xaml
    /// </summary>
    [ExportView(AppConstants.RadioPlayerExtenionViews, Order = 1500)]
    public partial class ShutdownAutomaticView : IHeaderProvider
    {
        public ShutdownAutomaticView()
        {
            InitializeComponent();
        }

        public object Header => RadioStreamerResources.RadioPlayerShutdownTimerTitleLabel;
    }
}
