using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Security;
using System.Security.Principal;
using System.Threading;
using Tauron.Application.Implement;
using Tauron.Application.RadioStreamer.Interop;
using Tauron.Application.RadioStreamer.PlugIns;
using Tauron.Application.RadioStreamer.Properties;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer
{
    public static class Programm
    {
        private static bool _isRestart;

        private const string UpdateCommadLine = "-Update"; 
        private static bool _installUpdatesSolo;

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

           if (ExecuteSoloUpdate())
            {
                return;
            }

            bool first;
            string applicationIdentifier = "RadioStreamer" + Environment.UserName;

            string channelName = string.Concat(applicationIdentifier, ":", "SingeInstanceIPCChannel");
            var mutex = new Mutex(true, applicationIdentifier, out first);

            try
            {
                if (!first)
                    SignalFirstInstance(channelName, applicationIdentifier);

                var domain = AppDomain.CurrentDomain;
                domain.SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);
                domain.UnhandledException += OnUnhandledException;

                #if !DEBUG
                LoadApplication();
                #endif

                if (_installUpdatesSolo)
                {
                    SheduleSoloUpdate();
                    mutex.Dispose();
                    return;
                }

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
            if (channelName == null) throw new ArgumentNullException(nameof(channelName));
            if (applicationIdentifier == null) throw new ArgumentNullException(nameof(applicationIdentifier));

            SingleInstance<App>.SignalFirstInstance(channelName,
                SingleInstance<App>.GetCommandLineArgs(applicationIdentifier));
        }

        private static void CleanUp()
        {
            SingleInstance<App>.Cleanup();
        }

        private static void SheduleSoloUpdate()
        {
            string file = Assembly.GetEntryAssembly().CodeBase;
            string path = Path.Combine(Path.GetTempPath(), "Tauron");
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            string newPath = file.Replace(AppDomain.CurrentDomain.BaseDirectory, path);
            File.Copy(file, newPath, true);

            Process.Start(newPath, UpdateCommadLine + AppDomain.CurrentDomain.BaseDirectory);
        }

        private static bool ExecuteSoloUpdate()
        {
            string[] args = Environment.GetCommandLineArgs();

            if(!args.Contains(UpdateCommadLine)) return false;

            bool found = false;
            string path = null;

            foreach (var s in args)
            {
                if (found)
                    path = s;

                if (s == UpdateCommadLine)
                    found = true;
            }

            if (string.IsNullOrEmpty(path) || !Directory.Exists(path)) return false;

            UpdateManager.FullUpdatePath = path;
            UpdateManager.FullDeletePath = path;
            UpdateManager.InstallUpdates();

            Process.Start(Path.Combine(path, Path.GetFileName(Assembly.GetEntryAssembly().CodeBase)));

            return true;
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

                    packManager.RegisterPack(
                        new InternalPackageManager.CacheEntry(
                            Assembly.GetExecutingAssembly().GetManifestResourceStream("RadioStreamer.nuspec")));
                }

                if(Settings.Default.AutoUpdate)
                {
                    progressDialog.SetLine(2, "Check for Updates", false, IntPtr.Zero);

                    var managers = new List<InternalPackageManager>();

                    bool needCopy;
                    if (packManager.CheckForUpdates(new[] { "RadioStreamer" }, out needCopy))
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

                    if (needCopy)
                    {
                        _installUpdatesSolo = true;
                        progressDialog.StopProgressDialog(); 
                        return;
                    }

                    UpdateManager.InstallUpdates();
                }

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
