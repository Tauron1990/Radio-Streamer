using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace Tauron.Application.RadioStreamer.Player.Core
{
    public enum WMAStreamFlags
    {
        Default = SampleInfoFlags.Default,
        Loop = SampleInfoFlags.Loop,
        ThreeDee = SampleInfoFlags.ThreeDee,
        FX = SampleInfoFlags.FX,
        //Decode,
        //AutoFree,
    }

    /// <summary>
    ///     Summary description for WMAStream.
    /// </summary>
    public class WMAStream : AdvancedChannel
    {
        private bool disposed;

        internal WMAStream(IntPtr handle) : base(handle)
        {
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                try
                {
                    if (disposing)
                    {
                        // free managed resources
                    }

                    // free unmanaged resources
                    _Free(base.Handle);

                    disposed = true;
                }
                finally
                {
                    base.Dispose(disposing);
                }
            }
        }

        #region DllImports

        private IntPtr HSYNC;
        private GetSyncCallBack getSync;

        private EventHandler streamendstore;

        /// <summary>
        ///     Retrieve the playback length (in bytes) of a WMA stream.
        /// </summary>
        public long Length
        {
            get
            {
                if (disposed)
                    throw new ObjectDisposedException(ToString());

                long output = _GetLength(base.Handle);
                if (output < 0) throw new WMAException();
                return output;
            }
        }

        /// <summary>
        ///     Get/Set the current playback position of a WMA channel in bytes
        /// </summary>
        public override long Position
        {
            get
            {
                if (disposed)
                    throw new ObjectDisposedException(ToString());

                long output = _GetWMAPosition(base.Handle);
                if (output < 0) throw new WMAException();
                return output;
            }
            set
            {
                if (disposed)
                    throw new ObjectDisposedException(ToString());

                if (_SetWMAPosition(base.Handle, value) == 0)
                    throw new WMAException();
            }
        }

        [DllImport("basswma.dll", EntryPoint = "BASS_WMA_StreamFree")]
        private static extern void _Free(IntPtr hwma);

        [DllImport("basswma.dll", EntryPoint = "BASS_WMA_StreamGetLength")]
        private static extern long _GetLength(IntPtr handle);

        [DllImport("basswma.dll", EntryPoint = "BASS_WMA_StreamGetTags")]
        private static extern IntPtr _GetTags(IntPtr handle, int tags);

        /// <summary>
        ///     Retrieve the WMA tags, if available.
        /// </summary>
        /// <returns>An string array containing the tags</returns>
        public string[] GetTags()
        {
            if (disposed)
                throw new ObjectDisposedException(ToString());

            var tags = new ArrayList();
            IntPtr ptag = _GetTags(base.Handle, 0);
            do
            {
                string tag = Marshal.PtrToStringAnsi(ptag);
                if (tag == "") break;
                tags.Add(tag);
                ptag = new IntPtr(ptag.ToInt32() + (tag.Length + 1));
            } while (true);

            var output = new string[tags.Count];
            for (int i = 0; i < output.Length; i++)
                output[i] = (string) tags[i];
            return output;
        }

        [DllImport("basswma.dll", EntryPoint = "BASS_WMA_StreamPlay")]
        private static extern int _Play(IntPtr handle, int flush, int flags);

        /// <summary>
        ///     Play a WMA stream.
        /// </summary>
        /// <param name="flush">restart from the beginning.</param>
        /// <param name="loop">loop the file</param>
        public void Play(bool flush, bool loop)
        {
            if (disposed)
                throw new ObjectDisposedException(ToString());

            var flags = (int) WMAStreamFlags.Loop;
            if (!loop) flags = 0;
            if (_Play(base.Handle, Helper.Bool2Int(flush), flags) == 0)
                throw new WMAException();
            base.StartTimer();
        }

        public override void Stop()
        {
            if (disposed)
                throw new ObjectDisposedException(ToString());

            base.Stop();
            Position = 0;
        }

        [DllImport("basswma.dll", EntryPoint = "BASS_WMA_ChannelSetPosition")]
        private static extern int _SetWMAPosition(IntPtr handle, long pos);

        [DllImport("basswma.dll", EntryPoint = "BASS_WMA_ChannelGetPosition")]
        private static extern long _GetWMAPosition(IntPtr handle);

        [DllImport("basswma.dll", EntryPoint = "BASS_WMA_ChannelSetSync")]
        private static extern IntPtr _SetSync(IntPtr handle,
            int atype, long param, GetSyncCallBack proc, int user); //TODO: OK

        private void OnGetSyncCallBack(IntPtr handle, IntPtr channel, int data, int user) //internal
        {
            OnEnd();
        }

        public override event EventHandler End
        {
            add
            {
                streamendstore += value;
                getSync += OnGetSyncCallBack;
                HSYNC = _SetSync(base.Handle, 2, 0, getSync, 0);
            }
            remove
            {
                streamendstore -= value;
                getSync -= OnGetSyncCallBack;
                _RemoveSync(base.Handle, HSYNC);
            }
        }

        protected override void OnEnd()
        {
            if (streamendstore != null) streamendstore(this, null);
        }

        // Remove a sync from a channel
        // handle : channel handle(HMUSIC/HSTREAM)
        // sync   : Handle of sync to remove
        [DllImport("basswma.dll", EntryPoint = "BASS_WMA_ChannelRemoveSync")]
        private static extern int _RemoveSync(IntPtr handle, IntPtr sync); //TODO: OK retrun bool

        //TODO: IWMReader
        //void *BASSWMADEF(BASS_WMA_GetIWMReader)(HSTREAM handle);
        /* Retrieve the IWMReader interface of a WMA stream. This allows direct
		access to the WMFSDK functions.
		handle : Channel handle
		RETURN : Pointer to the IWMReader object interface (NULL=error) */

        #endregion
    }
}