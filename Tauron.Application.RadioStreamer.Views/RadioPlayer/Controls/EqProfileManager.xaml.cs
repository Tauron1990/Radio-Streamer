using System;
using System.Windows.Controls;

namespace Tauron.Application.RadioStreamer.Views.RadioPlayer.Controls
{
    /// <summary>
	/// Interaktionslogik für EqProfileManager.xaml
	/// </summary>
	public partial class EqProfileManager
	{
		public EqProfileManager()
		{
			InitializeComponent();
		}

		private void RadioProfileManager_Initialized_1(object sender, EventArgs e)
		{
			if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)) return;

			DataContext = new RadioEqProfileManagerViewModel();
		}
	}
}
