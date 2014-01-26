using Tauron.Application.Controls;
using Tauron.Application.RadioStreamer.Contracts;
using Tauron.Application.Views;

namespace Tauron.Application.RadioStreamer.Views.RadioManager
{
	/// <summary>
	/// Interaktionslogik für RadioManager.xaml
	/// </summary>
	[ExportView(AppConstants.DefaultContentTab, Order = 100)]
	public partial class RadioManagerView : IHeaderProvider
	{
        public RadioManagerView()
		{
			InitializeComponent();
		}

	    public object Header
	    {
	        get
	        {
	            return "Radio Manager";
	        }
	    }
	}
}
