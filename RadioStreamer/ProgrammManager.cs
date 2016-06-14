using System.Globalization;
using System.Threading;
using Tauron.Application.Ioc;
using Tauron.Application.RadioStreamer.Contracts.Core;
using Tauron.Application.RadioStreamer.Properties;
using Tauron.Application.Views;

namespace Tauron.Application.RadioStreamer
{
    [Export(typeof (IProgramManager))]
    public sealed class ProgrammManager : IProgramManager
    {
        public IWindow MainWindow => CommonApplication.Current.MainWindow;

        public CultureInfo CultureInfo
        {
            get
            {
                var cul = Settings.Default.Language;
                if (cul == null || cul.Equals(CultureInfo.InvariantCulture))
                    return Thread.CurrentThread.CurrentCulture;

                return cul;
            }
            set
            {
                Settings.Default.Language = value;
                Settings.Default.Save();
            }
        }

        public bool AutoUpdate
        {
            get { return Settings.Default.AutoUpdate; }
            set
            {
                Settings.Default.AutoUpdate = value;
                Settings.Default.Save();
            }
        }

        public bool Shutdown { get; set; }

        public void Restart()
        {
            Programm.Restart();
        }

        public void ShowWindow(string name, bool modal)
        {
            var window = ViewManager.Manager.CreateWindow(name);
            if (modal)
                window.ShowDialogAsync(CommonApplication.Current.MainWindow);
            else 
                window.Show();
        }
    }
}
