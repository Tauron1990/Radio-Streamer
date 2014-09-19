using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime;
using System.Runtime.ExceptionServices;
using System.Security;
using System.Security.Principal;
using System.Threading;
using Tauron.Application.Implement;
using Tauron.Application.RadioStreamer.Interop;
using Tauron.Application.RadioStreamer.PlugIns;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer
{
    public static class Programm
    {
        private static bool _isRestart;

        private static readonly string[] AssemblysToInstall =
        {
            "JetBrains.Annotations",
            "Tauron.Application.Common",
            "Tauron.Application.Common.Wpf",
            "Tauron.Application.Common.Wpf.Controls",
            "Tauron.RadioStreamer.Skins.ElysiumTheme",
            "Tauron.Application.Bass",
            "Tauron.Application.RadioStreamer",
            "Tauron.Application.RadioStreamer.Contracts",
            "Tauron.Application.Radiostreamer.Database",
            "Tauron.Application.RadioStreamer.Resources",
            "Tauron.Application.RadioStreamer.Views"
        };

        [STAThread]
        public static void Main()
        {
            bool first;
            string applicationIdentifier = "RadioStreamer" + Environment.UserName;

            string channelName = string.Concat(applicationIdentifier, ":", "SingeInstanceIPCChannel");
            var mutex = new Mutex(true, applicationIdentifier, out first);

            try
            {
                if (!first)
                    SignalFirstInstance(channelName, applicationIdentifier);

                string profileOptimizionPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Jitter");
                if (!Directory.Exists(profileOptimizionPath)) Directory.CreateDirectory(profileOptimizionPath);

                ProfileOptimization.SetProfileRoot(profileOptimizionPath);
                ProfileOptimization.StartProfile("Main.jit");

                var domain = AppDomain.CurrentDomain;
                domain.SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);
                domain.UnhandledException += OnUnhandledException;

                #if !DEBUG
                LoadApplication();
                #endif

                StartApp(mutex, channelName);
            }
            catch
            {
                _isRestart = false;
                throw;
            }
            finally
            {
                CleanUp();
            }

            if (_isRestart)
                Process.Start(Assembly.GetEntryAssembly().CodeBase);
        }

        private static void SignalFirstInstance([NotNull] string channelName, [NotNull] string applicationIdentifier)
        {
            if (channelName == null) throw new ArgumentNullException("channelName");
            if (applicationIdentifier == null) throw new ArgumentNullException("applicationIdentifier");

            SingleInstance<App>.SignalFirstInstance(channelName,
                SingleInstance<App>.GetCommandLineArgs(applicationIdentifier));
        }

        private static void CleanUp()
        {
            SingleInstance<App>.Cleanup();
        }


        [UsedImplicitly]
        private static void LoadApplication()
        {
            var progressDialog = NativeMethods.CreateProgressDialog();

            try
            {
                progressDialog.SetTitle("Loadeing Application");
                progressDialog.SetLine(1, "Prepare for Start up", false, IntPtr.Zero);
                progressDialog.StartProgressDialog(IntPtr.Zero, null, 32, IntPtr.Zero);

                var packManager = InternalPackageManager.BuildRootManager();

                if (!packManager.IsVersionFilePresent)
                {
                    progressDialog.SetLine(2, "Installing Application Files", false, IntPtr.Zero);

                    foreach (var assembly in AssemblysToInstall)
                    {
                        CheckCancel(progressDialog);
                        packManager.Install(assembly);
                    }
                }

                progressDialog.SetLine(2, "Check for Updates", false, IntPtr.Zero);

                var managers = new List<InternalPackageManager>();

                if(packManager.CheckForUpdates())
                    managers.Add(packManager);
                CheckCancel(progressDialog);
                packManager = InternalPackageManager.BuildPackManager();
                if(packManager.CheckForUpdates())
                    managers.Add(packManager);
                CheckCancel(progressDialog);
                packManager = InternalPackageManager.BuildPluginManager();
                if(packManager.CheckForUpdates())
                    managers.Add(packManager);
                CheckCancel(progressDialog);

                progressDialog.SetLine(2, "Downloading Updates", false, IntPtr.Zero);

                foreach (var manager in managers)
                    manager.InstallUpdates();
                CheckCancel(progressDialog);

                progressDialog.SetLine(2, "Install Updates", false, IntPtr.Zero);

                UpdateManager.InstallUpdates();

                progressDialog.StopProgressDialog();
            }
            finally
            {
                NativeMethods.FreeObject(progressDialog);
            }
        }

        [DebuggerStepThrough]
        private static void CheckCancel([NotNull] IWinProgressDialog dialog)
        {
            if (dialog.HasUserCancelled())
                Environment.Exit(-1);
        }

        private static void StartApp([NotNull] Mutex mutex, [NotNull] string channelName)
        {
            App.Setup(mutex, channelName);
        }

        [HandleProcessCorruptedStateExceptions, SecurityCritical]
        private static void OnUnhandledException([NotNull] object sender, [NotNull] UnhandledExceptionEventArgs args)
        {
            CommonConstants.LogCommon(true, args.ExceptionObject.ToString());
            _isRestart = false;
        }

        public static void Restart()
        {
            _isRestart = true;
            CommonApplication.Current.Shutdown();
        }
    }
}
