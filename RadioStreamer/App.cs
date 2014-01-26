using System;
using System.Windows;
using System.Windows.Input;
using Elysium;
using Tauron.Application.Implement;
using Tauron.Application.RadioStreamer.Contracts;
using Tauron.Application.Views;

namespace Tauron.Application.RadioStreamer
{
	internal class App : WpfApplication
	{
		[STAThread]
		public static void Main()
		{
			AppDomain.CurrentDomain.SetPrincipalPolicy(System.Security.Principal.PrincipalPolicy.WindowsPrincipal);
			Run<App>();
		}

		public App()
			: base(true)
		{
		}

	    protected override void ConfigSplash()
	    {
            var dic = new Implementation.PackUriHelper().Load<ResourceDictionary>("StartResources.xaml");

	        CurrentWpfApplication.Resources = dic;

            var control = new System.Windows.Controls.ContentControl
            {
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                Height = 236,
                Width = 414,
                Content = dic["MainLabel"]
            };

	        SplashMessageListener.CurrentListner.SplashContent = control;
	        SplashMessageListener.CurrentListner.MainLabelForeground = "Black";
            SplashMessageListener.CurrentListner.MainLabelBackground = dic["MainLabelbackground"];
	    }

	    protected override IWindow DoStartup(CommandLineProcessor prcessor)
		{
			return ViewManager.Manager.CreateWindow(AppConstants.MainWindowName);
		}
		protected override void LoadCommands()
		{
			base.LoadCommands();
			CommandBinder.Register(MediaCommands.Play);
			CommandBinder.Register(MediaCommands.Stop);
			CommandBinder.Register(MediaCommands.Record);
			CommandBinder.Register(MediaCommands.MuteVolume);
		    CommandBinder.AutoRegister = true;
		}
		protected override void LoadResources()
		{
			CurrentWpfApplication.Apply(Theme.Dark, AccentBrushes.Violet, null);
		}
	}
}
