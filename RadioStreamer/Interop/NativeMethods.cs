using System;
using System.Runtime.InteropServices;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Interop
{
    [Guid("EBBC7C04-315E-11d2-B62F-006097DF5BD4"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComImport]
    internal interface IWinProgressDialog
    {
        [PreserveSig]
        void StartProgressDialog(IntPtr hwndParent, [MarshalAs(UnmanagedType.IUnknown)] object punkEnableModless, uint dwFlags, IntPtr pvResevered);
        [PreserveSig]
        void StopProgressDialog();
        [PreserveSig]
        void SetTitle([MarshalAs(UnmanagedType.LPWStr)] string pwzTitle);
        [PreserveSig]
        void SetAnimation(IntPtr hInstAnimation, ushort idAnimation);
        [PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        bool HasUserCancelled();
        [PreserveSig]
        void SetProgress(uint dwCompleted, uint dwTotal);
        [PreserveSig]
        void SetProgress64(ulong ullCompleted, ulong ullTotal);
        [PreserveSig]
        void SetLine(uint dwLineNum, [MarshalAs(UnmanagedType.LPWStr)] string pwzString, [MarshalAs(UnmanagedType.VariantBool)] bool fCompactPath, IntPtr pvResevered);
        [PreserveSig]
        void SetCancelMsg([MarshalAs(UnmanagedType.LPWStr)] string pwzCancelMsg, object pvResevered);
        [PreserveSig]
        void Timer(uint dwTimerAction, object pvResevered);
    }
    internal static class NativeMethods
    {
        private static readonly Guid ProgressDialog = new Guid("F8383852-FCD3-11d1-A6B9-006097DF5BD4");

        [NotNull]
        public static IWinProgressDialog CreateProgressDialog()
        {
            return (IWinProgressDialog)
                Activator.CreateInstance(Type.GetTypeFromCLSID(ProgressDialog));
        }

        public static void FreeObject([NotNull] object obj)
        {
            Marshal.ReleaseComObject(obj);
        }
    }
}
