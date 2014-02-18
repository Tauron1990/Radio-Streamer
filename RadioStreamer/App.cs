using System;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Security;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Elysium;
using Tauron.Application.Implement;
using Tauron.Application.RadioStreamer.Contracts;
using Tauron.Application.RadioStreamer.Contracts.Scripts;
using Tauron.Application.Views;

namespace Tauron.Application.RadioStreamer
{
	internal class App : WpfApplication
	{
		[STAThread]
		public static void Main()
		{
			AppDomain.CurrentDomain.SetPrincipalPolicy(System.Security.Principal.PrincipalPolicy.WindowsPrincipal);
		    AppDomain.CurrentDomain.UnhandledException +=
		        OnUnhandledException;
			Run<App>();
		}

        [HandleProcessCorruptedStateExceptions, SecurityCritical]
        private static void OnUnhandledException([JetBrains.Annotations.NotNull] object sender, [JetBrains.Annotations.NotNull] UnhandledExceptionEventArgs args)
	    {
	        CommonConstants.LogCommon(true, args.ExceptionObject.ToString());
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
			var temp = ViewManager.Manager.CreateWindow(AppConstants.MainWindowName);

	        CurrentWpfApplication.Dispatcher.Invoke(() =>
	        {
	            Current.MainWindow = temp;
	            CurrentWpfApplication.MainWindow = (Window) temp.TranslateForTechnology();
	        });
	        return temp;
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
			CurrentWpfApplication.Apply(Theme.Dark, AccentBrushes.Purple, null);
		}
	}
}
