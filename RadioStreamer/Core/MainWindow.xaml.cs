using Tauron.Application.RadioStreamer.Contracts;
using Tauron.Application.Views;

namespace Tauron.Application.RadioStreamer.Core
{
	/// <summary>
	/// Interaktionslogik für MainWindow.xaml
	/// </summary>
	[ExportWindow(AppConstants.MainWindowName)]
	public partial class MainWindow
	{
		public MainWindow()
		{
			InitializeComponent();
		}
	}
}
