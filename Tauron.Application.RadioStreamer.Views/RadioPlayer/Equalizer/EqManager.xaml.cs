using Tauron.Application.Controls;
using Tauron.Application.RadioStreamer.Contracts;
using Tauron.Application.RadioStreamer.Resources;
using Tauron.Application.Views;

namespace Tauron.Application.RadioStreamer.Views.RadioPlayer.Equalizer
{
    /// <summary>
    /// Interaktionslogik für EqManager.xaml
    /// </summary>
    [ExportView(AppConstants.RadioPlayerExtenionViews, Order = 100)]
    public partial class EqManager : IHeaderProvider 
    {
        public EqManager()
        {
            InitializeComponent();
        }

        public object Header { get { return RadioStreamerResources.EqualizerLabel; } }
    }
}
