using System.Collections.Generic;
using System.Linq;
using Tauron.Application.Ioc;
using Tauron.Application.Models;
using Tauron.Application.Modules;
using Tauron.Application.RadioStreamer.Contracts;
using Tauron.Application.RadioStreamer.Contracts.Core;
using Tauron.Application.RadioStreamer.Resources;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Views.AddIns
{
    [ExportViewModel(AppConstants.AddInViewModel)]
    public sealed class AddInViewModel : ViewModelBase, INotifyBuildCompled
    {
        [Inject]
        private IPlugInManager _plugInManager;

        [Inject]
        private IDialogFactory _dialogFactory;

        [NotNull]
        public List<ModuleInfo> Modules { get; private set; }

        [NotNull]
        public UISyncObservableCollection<InternalAddInInfo> AddIns { get; private set; }

        [NotNull]
        public UISyncObservableCollection<InternalAddInInfo> ReadyAddIns { get; private set; }

        public void BuildCompled()
        {
            Modules = new List<ModuleInfo>(AddInListner.AddIns.Select(desc => new ModuleInfo(desc)));

            AddIns = new UISyncObservableCollection<InternalAddInInfo>();
            ReadyAddIns = new UISyncObservableCollection<InternalAddInInfo>();

            ResetAddIns();
        }

        private void ResetAddIns()
        {
            AddIns.Clear();
            ReadyAddIns.Clear();

            AddIns.AddRange(
                _plugInManager.GetInstalledAddIns()
                    .Select(
                        i =>
                            new InternalAddInInfo(i.CanUnInstall, Uninstall, i,
                                RadioStreamerResources.AddInViewUnistallButtonLabel)));

            ReadyAddIns.AddRange(
                _plugInManager.GetAvailableAddIns()
                    .Select(i => new InternalAddInInfo(true, Install, i, RadioStreamerResources.AddInViewInstallButton)));
        }

        private void Install([NotNull] IPackInfo obj)
        {
            string label = RadioStreamerResources.InstallingLabel;
            var diag = _dialogFactory.CreateProgressDialog(label, label, MainWindow, progress =>
            {
                ((IInstallablePackInfo) obj).Install();
                ResetAddIns();
            });

            diag.ProgressBarStyle = ProgressStyle.MarqueeProgressBar;
            diag.Start();
        }

        private void Uninstall([NotNull] IPackInfo obj)
        {
            ((IInstalledPackInfo) obj).UnInstall();
            ResetAddIns();
        }
    }
}
