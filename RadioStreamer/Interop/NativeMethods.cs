﻿using System;
using System.Runtime.InteropServices;

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
    }
}