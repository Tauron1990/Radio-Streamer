using System;
using System.Runtime;
using System.Runtime.ExceptionServices;
using System.Security;
using NuGet;
using Tauron.Application.RadioStreamer.Interop;
using Tauron.Application.RadioStreamer.PlugIns;

namespace Tauron.Application.RadioStreamer
{
    public static class Programm
    {
        #if DEBUG
        private const string NugetUrl = @"C:\nuget\Packages";
        private const string MyGetUrl = @"C:\nuget\Plugins";
        #else
        private const string NugetUrl
        private const string MyGetUrl
        #endif

        private static readonly string[] AssemblysToInstall =
        {
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
            ProfileOptimization.StartProfile();

            AppDomain.CurrentDomain.SetPrincipalPolicy(System.Security.Principal.PrincipalPolicy.WindowsPrincipal);
            AppDomain.CurrentDomain.UnhandledException +=
                OnUnhandledException;

            LoadApplication();
            
            App.Setup();
        }

        private static void LoadApplication()
        {
            var progressDialog = NativeMethods.CreateProgressDialog();
;

            try
            {
                progressDialog.SetTitle("Loadeing Application");
                progressDialog.SetLine(1, "Prepare for Start up", false, IntPtr.Zero);
                progressDialog.StartProgressDialog(IntPtr.Zero, null, 32, IntPtr.Zero);

                IPackageRepository repository = new AggregateRepository(PackageRepositoryFactory.Default,
                    new[] {MyGetUrl, NugetUrl}, true);

                string targetPath = AppDomain.CurrentDomain.BaseDirectory;

                var packManager = new InternalPackageManager(repository, targetPath, false, targetPath, true);

                if (!packManager.IsVersionFilePresent)
                {
                    progressDialog.SetLine(2, "Installing Application Files", false, IntPtr.Zero);

                    foreach (var assembly in AssemblysToInstall)
                    {
                        packManager.Install(assembly);
                    }
                }

                progressDialog.SetLine(2, "Downloading Update on Necessary", false, IntPtr.Zero);

                packManager.CheckForUpdates();


                progressDialog.StopProgressDialog();
            }
            finally
            {
                NativeMethods.FreeObject(progressDialog);
            }
        }

        private static 

        [HandleProcessCorruptedStateExceptions, SecurityCritical]
        private static void OnUnhandledException([JetBrains.Annotations.NotNull] object sender, [JetBrains.Annotations.NotNull] UnhandledExceptionEventArgs args)
        {
            CommonConstants.LogCommon(true, args.ExceptionObject.ToString());
        }
    }
}
