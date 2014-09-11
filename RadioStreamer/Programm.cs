using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime;
using System.Runtime.ExceptionServices;
using System.Security;
using Tauron.Application.RadioStreamer.Interop;
using Tauron.Application.RadioStreamer.PlugIns;

namespace Tauron.Application.RadioStreamer
{
    public static class Programm
    {
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
            string profileOptimizionPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Jitter");
            if (!Directory.Exists(profileOptimizionPath)) Directory.CreateDirectory(profileOptimizionPath);

            ProfileOptimization.SetProfileRoot(profileOptimizionPath);
            ProfileOptimization.StartProfile("Main.jit");

            var domain = AppDomain.CurrentDomain;
            domain.SetPrincipalPolicy(System.Security.Principal.PrincipalPolicy.WindowsPrincipal);
            domain.UnhandledException += OnUnhandledException;
           
            #if !DEBUG
            LoadApplication();
            #endif
   
            StartApp();
        }


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
        private static void CheckCancel(IWinProgressDialog dialog)
        {
            if(dialog.HasUserCancelled())
                Environment.Exit(-1);
        }

        private static void StartApp()
        {
            App.Setup();
        }

        [HandleProcessCorruptedStateExceptions, SecurityCritical]
        private static void OnUnhandledException([JetBrains.Annotations.NotNull] object sender, [JetBrains.Annotations.NotNull] UnhandledExceptionEventArgs args)
        {
            CommonConstants.LogCommon(true, args.ExceptionObject.ToString());
        }
    }
}
