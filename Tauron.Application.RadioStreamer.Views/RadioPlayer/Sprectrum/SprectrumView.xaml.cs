using Tauron.Application.Controls;
using Tauron.Application.RadioStreamer.Contracts;
using Tauron.Application.RadioStreamer.Resources;
using Tauron.Application.Views;

namespace Tauron.Application.RadioStreamer.Views.RadioPlayer.Sprectrum
{
    /// <summary>
    /// Interaktionslogik für SprectrumView.xaml
    /// </summary>
    [ExportView(AppConstants.RadioPlayerExtenionViews, Order = 1000)]
    public partial class SprectrumView : IHeaderProvider
    {
        public SprectrumView()
        {
            InitializeComponent();
        }

        public object Header
        {
            get
            {
                return RadioStreamerResources.SprectumHeaderLabel;
            }
        }
    }
}
