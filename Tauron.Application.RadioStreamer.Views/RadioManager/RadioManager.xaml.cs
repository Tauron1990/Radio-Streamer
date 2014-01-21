using System.Windows.Controls;
using Tauron.Application.RadioStreamer.Contracts;
using Tauron.Application.Views;

namespace Tauron.Application.RadioStreamer.Views.RadioManager
{
	/// <summary>
	/// Interaktionslogik für RadioManager.xaml
	/// </summary>
	[ExportView(AppConstants.RadioManagerView)]
	public partial class RadioManagerView : UserControl
	{
        public RadioManagerView()
		{
			InitializeComponent();
		}
	}
}
