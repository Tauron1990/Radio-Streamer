using System;
using Tauron.JetBrains.Annotations;
using Un4seen.Bass;

namespace Tauron.Application.RadioStreamer.Player.CoreOld
{
    /// <summary>
    ///     AdvancedChannel this class is not directly used
    /// </summary>
    [PublicAPI]
    public abstract class AdvancedChannel : Channel
    {
        private bool _disposed;

        internal AdvancedChannel(int handle) : base(handle)
        {
        }

        protected override void Dispose(bool disposing)
        {
            if (_disposed) return;
            try
            {
                _disposed = true;
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        #region Other Common

        //// Setup a user DSP function on a channel. When multiple DSP functions
        //// are used on a channel, they are called in the order that they were added.
        //// handle:  channel handle(HMUSIC / HSTREAM)
        //// proc   : User defined callback function
        //// user   : The //user// value passed to the callback function
        //// RETURN : DSP handle (NULL=error)
        //[DllImport("bass.dll", EntryPoint = "BASS_ChannelSetDSP")]
        //private static extern IntPtr _SetDSP(IntPtr handle, DSPCallBack proc, int user); // Make callback

        //// Remove a DSP function from a channel
        //// handle : channel handle(HMUSIC / HSTREAM)
        //// dsp    : Handle of DSP to remove */
        //// RETURN : BASSTRUE / BASSFALSE
        //[DllImport("bass.dll", EntryPoint = "BASS_ChannelRemoveDSP")]
        //private static extern int _RemoveDSP(IntPtr handle, IntPtr dsp); // OK,

        //// Setup a sync on a channel. Multiple syncs may be used per channel.
        //// handle : Channel handle (currently there are only HMUSIC/HSTREAM syncs)
        //// atype  : Sync type (BASS_SYNC_xxx type & flags)
        //// param  : Sync parameters (see the BASS_SYNC_xxx type description)
        //// proc   : User defined callback function (use AddressOf SYNCPROC)
        //// user   : The //user// value passed to the callback function
        //// Return : Sync handle(Null = Error)
        //[DllImport("bass.dll", EntryPoint = "BASS_ChannelSetSync")]
        //private static extern IntPtr _SetSync(IntPtr handle,
        //    int atype, long param, GetSyncCallBack proc, int user); // OK

        private void OnGetSyncCallBack(int handle, int channel, int data, IntPtr user) //internal
        {
            OnEnd();
        }

        private int _hsync;
        private SYNCPROC _getSync;

        [CanBeNull]
        private EventHandler _streamendstore;

        public virtual event EventHandler End
        {
            add
            {
                if (_disposed)
                    throw new ObjectDisposedException(ToString());

                if (_streamendstore == null) _streamendstore = value;
                else _streamendstore += value;

                if (_getSync != null) return;

                _getSync += OnGetSyncCallBack;
                _hsync = Bass.BASS_ChannelSetSync(Handle, BASSSync.BASS_SYNC_END, 0, _getSync, IntPtr.Zero);
            }
            remove
            {
                if (_disposed)
                    throw new ObjectDisposedException(ToString());

                if(_streamendstore != null)
// ReSharper disable once DelegateSubtraction
                    _streamendstore -= value;
            }
        }

        protected virtual void OnEnd()
        {
            if (_streamendstore != null) _streamendstore(this, null);
        }

        //// Remove a sync from a channel
        //// handle : channel handle(HMUSIC/HSTREAM)
        //// sync   : Handle of sync to remove
        //[DllImport("bass.dll", EntryPoint = "BASS_ChannelRemoveSync")]
        //private static extern int _RemoveSync(IntPtr handle, IntPtr sync); // OK retrun bool

        //// Retrieves upto "length" bytes of the channel//s current sample data. This is
        //// useful if you wish to "visualize" the sound.
        //// handle:  Channel handle(HMUSIC / HSTREAM, or RECORDCHAN)
        //// buffer : Location to write the data
        //// length : Number of bytes wanted, or a BASS_DATA_xxx flag
        //// RETURN : Number of bytes actually written to the buffer (-1=error)
        //[DllImport("bass.dll", EntryPoint = "BASS_ChannelGetData")]
        //private static extern int _GetData(IntPtr handle, float[] buffer, int Length); // OK return dword

        /// <summary>
        ///     Retrieves upto "length" bytes of the channel//s current sample data. This is
        ///     useful if you wish to "visualize" the sound.
        /// </summary>
        /// <param name="buffer">A buffer to place retrieved data</param>
        /// <param name="flags">ChannelDataFlags</param>
        public int GetData([NotNull] float[] buffer, ChannelDataFlags flags)
        {
            if (_disposed)
                throw new ObjectDisposedException(ToString());

            int output = Bass.BASS_ChannelGetData(Handle, buffer, (int) flags);
            if (output < 0) throw new BASSException();
            return output;
        }

        private static int GetDataLength(ChannelDataFlags flags)
        {
            switch (flags)
            {
                case ChannelDataFlags.FFT512:
                    return 256;
                case ChannelDataFlags.FFT1024:
                    return 512;
                case ChannelDataFlags.FFT2048:
                    return 1024;
                case ChannelDataFlags.SFFT512:
                    return 512;
                case ChannelDataFlags.SFFT1024:
                    return 1024;
                case ChannelDataFlags.SFFT2048:
                    return 2048;
                default:
                    return 1024;
            }
        }

        //[DllImport("bass.dll", EntryPoint = "BASS_ChannelGetData")]
        //private static extern int _GetData(IntPtr handle, [Out] byte[] buffer, int Length); // OK return dword

        /// <summary>
        ///     Retrieves upto "length" bytes of the channel//s current sample data. 8-bit data
        /// </summary>
        /// <param name="buffer">A buffer to place retrieved data</param>
        /// <param name="length">length in bytes</param>
        public int GetData([NotNull] byte[] buffer, int length)
        {
            if (_disposed)
                throw new ObjectDisposedException(ToString());

            int output = Bass.BASS_ChannelGetData(Handle, buffer, length);
            if (output < 0) throw new BASSException();
            return output;
        }

        //[DllImport("bass.dll", EntryPoint = "BASS_ChannelGetData")]
        //private static extern int _GetData(IntPtr handle, [Out] short[] buffer, int Length); // OK return dword

        /// <summary>
        ///     Retrieves upto "length" bytes of the channel//s current sample data. 16-bit data
        /// </summary>
        /// <param name="buffer">A buffer to place retrieved data</param>
        /// <param name="length">length in bytes</param>
        public int GetData([NotNull] short[] buffer, int length)
        {
            if (_disposed) throw new ObjectDisposedException(ToString());

            int output = Bass.BASS_ChannelGetData(Handle, buffer, length);
            if (output < 0) throw new BASSException();
            return output;
        }

        //// 
        //// handle : channel handle(HMUSIC / HSTREAM, or RECORDCHAN)
        //// RETURN : LOWORD=left level (0-128) HIWORD=right level (0-128) (-1=error)
        ////          Use LoWord and HiWord functions on return function.
        //[DllImport("bass.dll", EntryPoint = "BASS_ChannelGetLevel")]
        //private static extern int _GetLevel(IntPtr handle); //OK return dword

        /// <summary>
        ///     Calculate a channel's current left output level.
        /// </summary>
        public int LeftLevel
        {
            get
            {
                if (_disposed)
                    throw new ObjectDisposedException(ToString());

                int result = Bass.BASS_ChannelGetLevel(Handle);
                if (result < 0) return 0; /*throw new BASSException();*/
                if (result == -1) throw new BASSException();
                return Utils.LowWord32(result);
            }
        }

        /// <summary>
        ///     Calculate a channel's current right output level.
        /// </summary>
        public int RightLevel
        {
            get
            {
                if (_disposed)
                    throw new ObjectDisposedException(ToString());

                int result = Bass.BASS_ChannelGetLevel(Handle);
                return result < 0 ? 0 : Utils.HighWord32(result);
            }
        }

#if(true)
        /// <summary>
        ///     Setup a DX8 effect on a channel. Can only be used when the channel
        ///     is not playing. Use FX.Parameters to set the effect parameters.
        ///     Obviously requires DX8.
        /// </summary>
        /// <param name="chanfx">ChannelFX</param>
        /// <returns>An FX object</returns>
        [NotNull]
        public FX SetFX(BASSFXType chanfx, int priority)
        {
            if (_disposed) throw new ObjectDisposedException(ToString());

            int fx = Bass.BASS_ChannelSetFX(Handle, chanfx, priority);
            if (fx == 0) throw new BASSException();
            return new FX(fx, chanfx, priority);
        }

        /// <summary>
        ///     Remove a DX8 effect from a channel. Can only be used when the
        ///     channel is not playing.
        /// </summary>
        /// <param name="fx">The FX object to remove</param>
        public void RemoveFX([NotNull] FX fx)
        {
            if (_disposed) throw new ObjectDisposedException(ToString());

            if (!Bass.BASS_ChannelRemoveFX(Handle, fx.Handle)) throw new BASSException();
        }
#endif

        #endregion
    }
}