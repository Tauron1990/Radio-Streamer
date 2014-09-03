using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tauron.Application.RadioStreamer.Interop;

namespace Tauron.Application.RadioStreamer
{
    public static class Programm
    {
        [STAThread]
        public static void Main()
        {
            AppDomain.CurrentDomain.SetPrincipalPolicy(System.Security.Principal.PrincipalPolicy.WindowsPrincipal);
            AppDomain.CurrentDomain.UnhandledException +=
                OnUnhandledException;

            LoadApplication();
            
            App.Setup();
        }

        private static void LoadApplication()
        {
            Guid test = new Guid("F8383852-FCD3-11d1-A6B9-006097DF5BD4");
            Type type = Type.GetTypeFromCLSID(test);
            object test2 = Activator.CreateInstance(type);
            var test3 = (IWinProgressDialog) test2;

            test3.SetTitle("Haalo Welt");
            test3.StartProgressDialog(IntPtr.Zero, null, 32, IntPtr.Zero);

            Thread.Sleep(new TimeSpan(0, 5, 0));

            test3.StopProgressDialog();
            Marshal.ReleaseComObject(test3);
        }

        [HandleProcessCorruptedStateExceptions, SecurityCritical]
        private static void OnUnhandledException([JetBrains.Annotations.NotNull] object sender, [JetBrains.Annotations.NotNull] UnhandledExceptionEventArgs args)
        {
            CommonConstants.LogCommon(true, args.ExceptionObject.ToString());
        }
    }
}
