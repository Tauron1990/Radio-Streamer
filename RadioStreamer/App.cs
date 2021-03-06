﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using Tauron.Application.Implement;
using Tauron.Application.RadioStreamer.Contracts;
using Tauron.Application.RadioStreamer.Contracts.Core;
using Tauron.Application.RadioStreamer.Resources;
using Tauron.Application.Views;
#if !DEBUG
using Bugsense.WPF;
using System.Reflection;
#endif
using Tauron.JetBrains.Annotations;


namespace Tauron.Application.RadioStreamer
{
	internal class App : WpfApplication, ISingleInstanceApp
	{
	    private IProgramManager _programManager;

	    public App()
			: base(true)
		{
		}

        public static void Setup([NotNull] Mutex mutex, [NotNull] string channelName)
        {
            if (mutex == null) throw new ArgumentNullException(nameof(mutex));
            if (channelName == null) throw new ArgumentNullException(nameof(channelName));
#if !DEBUG
             var version = Assembly.GetEntryAssembly().GetName().Version.ToString();

            BugSense.Init("w8cd1a17", version);
            Run<App>(app => SingleInstance<App>.InitializeAsFirstInstance(mutex, channelName, app));
		    BugSense.DetachHandler();
#else
            Run<App>(app => SingleInstance<App>.InitializeAsFirstInstance(mutex, channelName, app), Properties.Settings.Default.Language);
            #endif
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
	        _programManager = Container.Resolve<IProgramManager>();
			var temp = ViewManager.Manager.CreateWindow(AppConstants.MainWindowName);

	        CurrentWpfApplication.Dispatcher.Invoke(() =>
	        {
	            Current.MainWindow = temp;
	            CurrentWpfApplication.MainWindow = (Window)temp.TranslateForTechnology();
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
            Container.Resolve<IStyleManager>().LoadResources();
            SimpleLocalize.Register(RadioStreamerResources.ResourceManager, GetType().Assembly);
		    //CurrentWpfApplication.Apply(Theme.Dark, AccentBrushes.Purple, null);
		}

	    protected override void MainWindowClosed(object sender, EventArgs e)
	    {
            if(_programManager.Shutdown)
                Shutdown();
            else if (!Container.Resolve<IRadioEnvironment>().Settings.MinimizeInTray)
	            base.MainWindowClosed(sender, e);
	    }

	    public bool SignalExternalCommandLineArgs(IList<string> args)
	    {
	        MainWindow?.Focus();
	        return true;
	    }

	}
}
