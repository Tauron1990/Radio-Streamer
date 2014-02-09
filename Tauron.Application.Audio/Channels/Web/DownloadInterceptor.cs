using System;
using System.Runtime.InteropServices;
using Tauron.JetBrains.Annotations;
using Un4seen.Bass;

namespace Tauron.Application.BassLib.Channels
{
    public abstract class DownloadInterceptor
    {
        [NotNull]
        internal DOWNLOADPROC Downloadproc { get; private set; }

        protected DownloadInterceptor()
        {
            Downloadproc = DataReceived;
        }

        private void DataReceived(IntPtr buffer, int length, IntPtr user)
        {
            if(length <= 0) return;

            var arr = new byte[length];
            Marshal.Copy(buffer, arr, 0, length);
            DataReceived(arr);
        }

        protected abstract void DataReceived([NotNull] byte[] data);
    }
}