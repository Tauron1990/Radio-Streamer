using Tauron.Application.Ioc;
using Tauron.Application.RadioStreamer.Contracts.Core;
using Tauron.Application.Views;

namespace Tauron.Application.RadioStreamer
{
    [Export(typeof (IProgramManager))]
    public sealed class ProgrammManager : IProgramManager
    {
        public void Restart()
        {
            Programm.Restart();
        }

        public void ShowWindow(string name, bool modal)
        {
            var window = ViewManager.Manager.CreateWindow(name);
            if (modal)
                window.ShowDialog(CommonApplication.Current.MainWindow);
            else 
                window.Show();
        }
    }
}
