using System.Windows;
using Tauron.Application.RadioStreamer.Contracts;
using Tauron.Application.Views;

namespace Tauron.Application.RadioStreamer.Views.Core
{
	/// <summary>
	/// Interaktionslogik für MainWindow.xaml
	/// </summary>
	[ExportWindow(AppConstants.MainWindowName)]
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}
	}
}
