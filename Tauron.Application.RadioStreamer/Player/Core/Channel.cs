using System;
using System.ComponentModel;
using Tauron.JetBrains.Annotations;
using Un4seen.Bass;

namespace Tauron.Application.RadioStreamer.Player.Core
{
    /// <summary>
    ///     Provides common channel methods
    /// </summary>
    [PublicAPI]
    public class Channel : ChannelBase
    {
        private bool _disposed;

        internal Channel(int handle) : base(handle)
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

        #region Common Channel

        //OK return dword

        /// <summary>
        ///     Get some info about a channel.
        /// </summary>
        [NotNull]
        public string Flags
        {
            get
            {
                if (_disposed)
                    throw new ObjectDisposedException("Channel");

                var info = Bass.BASS_ChannelGetInfo(Handle);
                if (info == null) throw new BASSException();
                return Helper.PrintFlags(info.flags);
            }
        }

        /// <summary>
        ///     Gets/Sets a channel's 3D attributes.
        /// </summary>
        [NotNull]
        public Channel3DAttributes Attributes3D
        {
            get
            {
                if (_disposed)
                    throw new ObjectDisposedException("Channel");

                BASS3DMode mode = 0;
                float min = 0;
                float max = 0;
                int iangle = 0;
                int oangle = 0;
                int outvol = 0;

                if (!Bass.BASS_ChannelGet3DAttributes(Handle, ref mode, ref min, ref max,
                    ref iangle, ref oangle, ref outvol))
                    throw new BASSException();
                return new Channel3DAttributes(mode, min, max, iangle, oangle, outvol);
            }
            set
            {
                if (_disposed)
                    throw new ObjectDisposedException("Channel");

                if (!Bass.BASS_ChannelSet3DAttributes(Handle, value.Mode, value.Min, value.Max,
                    value.IAngle, value.Oangle, value.Outvol))
                    throw new BASSException();
            }
        }

        /// <summary>
        ///     Gets/Sets a channel's 3D Position
        /// </summary>
// ReSharper disable once VirtualMemberNeverOverriden.Global
        [NotNull]
        public virtual Channel3DPosition Position3D
        {
            get
            {
                if (_disposed)
                    throw new ObjectDisposedException("Channel");

                var pos = new BASS_3DVECTOR();
                var orient = new BASS_3DVECTOR();
                var vel = new BASS_3DVECTOR();
                if (!Bass.BASS_ChannelGet3DPosition(Handle ,pos, orient, vel))
                    throw new BASSException();
                return new Channel3DPosition(pos, orient, vel);
            }
            set
            {
                if (_disposed)
                    throw new ObjectDisposedException("Channel");

                BASS_3DVECTOR pos = value.Pos;
                BASS_3DVECTOR orient = value.Orient;
                BASS_3DVECTOR vel = value.Vel;
                if (!Bass.BASS_ChannelSet3DPosition(Handle, pos, orient, vel))
                    throw new BASSException();
            }
        }

        ///// <summary>
        /////     Set the wet(reverb)/dry(no reverb) mix ratio on the channel. By default
        /////     the distance of the sound from the listener is used to calculate the mix.
        ///// </summary>
        ///// <value>
        /////     The ratio (0.0=reverb off, 1.0=max reverb, -1.0=let EAX calculate
        /////     the reverb mix based on the distance)
        ///// </value>
        ///// <remarks>
        /////     The channel must have 3D functionality enabled for the EAX environment
        /////     to have any affect on it.
        ///// </remarks>
        //[ReadOnly(true)]
        //public float ChannelEAXMix
        //{
        //    get
        //    {
        //        if (_disposed)
        //            throw new ObjectDisposedException("Channel");
                
        //        Un4seen.Bass.Bass.BASS_GetEAXParameters()

        //        float output = 0;
        //        if (Bass.ea(base.Handle, ref output) == 0)
        //            throw new BASSException();
        //        return output;
        //    }
        //    set
        //    {
        //        if (_disposed)
        //            throw new ObjectDisposedException("Channel");

        //        if (_SetEAXMix(base.Handle, value) == 0)
        //            throw new BASSException();
        //    }
        //}

        //[DllImport("bass.dll", EntryPoint = "BASS_ChannelGetFlags")]
        //public static extern int _GetFlags(IntPtr handle);

        //[DllImport("bass.dll", EntryPoint = "BASS_ChannelSet3DAttributes")]
        //private static extern int _Set3DAttributes(IntPtr handle, int mode,
        //    float min, float max, int iangle, int oangle, int outvol); //OK handle is dword retun bool

        //[DllImport("bass.dll", EntryPoint = "BASS_ChannelGet3DAttributes")]
        //private static extern int _Get3DAttributes(IntPtr handle,
        //    ref int mode,
        //    ref float min,
        //    ref float max,
        //    ref int iangle,
        //    ref int oangle,
        //    ref int outvol); //OK return bool

        //[DllImport("bass.dll", EntryPoint = "BASS_ChannelSet3DPosition")]
        //private static extern int _Set3DPosition(IntPtr handle,
        //    ref Vector3D pos,
        //    ref Vector3D orient,
        //    ref Vector3D vel); //ok retun bool ,  how do we handle nulls???

        //[DllImport("bass.dll", EntryPoint = "BASS_ChannelGet3DPosition")]
        //private static extern int _Get3DPosition(IntPtr handle,
        //    out Vector3D pos,
        //    out Vector3D orient,
        //    out Vector3D vel); //ok retun bool

        //// 
        //// handle : channel handle(HCHANNEL / HSTREAM / HMUSIC)
        //// mix    : 
        //[DllImport("bass.dll", EntryPoint = "BASS_ChannelSetEAXMix")]
        //private static extern int _SetEAXMix(IntPtr handle, float mix); //OK return bool

        //// Get the wet(reverb)/dry(no reverb) mix ratio on the channel.
        //// handle:    channel handle(HCHANNEL / HSTREAM / HMUSIC)
        //// mix    : Pointer to store the ratio at
        //[DllImport("bass.dll", EntryPoint = "BASS_ChannelGetEAXMix")]
        //private static extern int _GetEAXMix(IntPtr handle, ref float mix); //OK retrun bool

        #endregion

        #region Overrides

        /// <summary>
        ///     Gets/Sets the current position
        /// </summary>
        [ReadOnly(true)]
        public override long Position
        {
            get
            {
                if (_disposed)
                    throw new ObjectDisposedException("Channel");

                long result = Bass.BASS_ChannelGetPosition(Handle);
                if (result < 0) throw new BASSException();
                return result;
            }
            set
            {
                if (_disposed)
                    throw new ObjectDisposedException("Channel");

                if (!Bass.BASS_ChannelSetPosition(Handle, value))
                    throw new BASSException();
            }
        }

        #endregion
    }
}