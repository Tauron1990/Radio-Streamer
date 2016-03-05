using System.ComponentModel;
using Tauron.Application.Commands;
using Tauron.Application.Ioc;
using Tauron.Application.Models;
using Tauron.Application.RadioStreamer.Contracts;
using Tauron.Application.RadioStreamer.Contracts.Core;
using Tauron.Application.RadioStreamer.Contracts.Core.Attributes;

namespace Tauron.Application.RadioStreamer.Views.Core
{
    [ExportViewModel(AppConstants.MainWindowsViewModelName)]
    public sealed class MainWindowViewModel : ViewModelBase
    {
        [Inject]
        IProgramManager _programManager;

        [InjectRadioEnviroment]
        IRadioEnvironment _radioEnvironment;

        [EventTarget]
        public void Closing(EventData data)
        {
            var args = data.EventArgs as CancelEventArgs;
            if (args == null)
                return;

            if (!_radioEnvironment.Settings.MinimizeInTray) return;

            _programManager.MainWindow.Hide();
            args.Cancel = true;
        }
    }
}
