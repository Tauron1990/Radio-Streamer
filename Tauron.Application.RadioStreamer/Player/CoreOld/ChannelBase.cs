using System;
using System.ComponentModel;
using System.Timers;
using Tauron.JetBrains.Annotations;
using Un4seen.Bass;

namespace Tauron.Application.RadioStreamer.Player.CoreOld
{
    /// <summary>
    ///     ChannelBase. The class is not used directly.
    /// </summary>
    [PublicAPI]
    public abstract class ChannelBase : IDisposable
    {
        private readonly int _handle;
        private readonly Timer _progresstimer;
        private bool _disposed;
        private BASSEngine _owner;

        protected ChannelBase(int handle)
        {
            _handle = handle;
            _progresstimer = new Timer(20);
            _progresstimer.Elapsed += ProgressTimerElapsed;
        }

        public double ProgressInterval
        {
            get
            {
                if (_disposed)
                    throw new ObjectDisposedException(ToString());

                return _progresstimer.Interval;
            }
            set
            {
                if (_disposed)
                    throw new ObjectDisposedException(ToString());

                _progresstimer.Interval = value;
            }
        }

        [NotNull]
        public BASSEngine Owner
        {
            get
            {
                if (_disposed)
                    throw new ObjectDisposedException(ToString());

                return _owner;
            }
            set
            {
                if (_disposed) throw new ObjectDisposedException(ToString());

                _owner = value;
                //progresstimer.SynchronizingObject = owner.ParentForm;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public int Handle
        {
            get
            {
                if (_disposed)
                    throw new ObjectDisposedException(ToString());

                return _handle;
            }
        }

        /// <summary>
        ///     Gets the objects current State
        /// </summary>
        public BASSActive ActivityState
        {
            get
            {
                if (_disposed)
                    throw new ObjectDisposedException(ToString());

                return Bass.BASS_ChannelIsActive(_handle);
            }
        }

        //[DllImport("bass.dll", EntryPoint = "BASS_ChannelSetAttributes")]
        //static extern int _SetAttributes(IntPtr handle, int freq, int volume, int pan);//OK handle is dword return bool

        //[DllImport("bass.dll", EntryPoint = "BASS_ChannelGetAttributes")]
        //static extern int _GetAttributes(IntPtr handle,
        //    ref int freq, 
        //    ref int volume, 
        //    ref int pan);//OK handle is dword return bool

        /// <summary>
        ///     Gets/Sets a channel's attributes. The actual setting may not be exactly
        ///     as specified, depending on the accuracy of the device and drivers.
        /// </summary>
        /// <remarks>
        ///     Only the volume can be adjusted for the CD "channel", but not all
        ///     soundcards allow controlling of the CD volume level.
        /// </remarks>
        [NotNull]
// ReSharper disable once VirtualMemberNeverOverriden.Global
        public virtual ChannelAttributes Attributes
        {
            get
            {
                if (_disposed)
                    throw new ObjectDisposedException(ToString());

                return new ChannelAttributes(_handle);
            }
            //set 
            //{
            //    if (this._disposed)
            //        throw new ObjectDisposedException(this.ToString());
            //    if (_SetAttributes( this._handle, value.freq, value.volume, value.panning) == 0)
            //        throw new BASSException();
            //}
        }

        /// <summary>
        ///     Gets/Sets the current playback position of a channel.
        /// </summary>
        public abstract long Position { get; set; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _disposed = true;
            }
        }

        ~ChannelBase()
        {
            Dispose(false);
        }

        private void ProgressTimerElapsed([NotNull] object sender, [NotNull] ElapsedEventArgs e)
        {
            OnProgress();
        }

        protected void StartTimer()
        {
            if (_disposed)
                throw new ObjectDisposedException(ToString());

            _progresstimer.Start();
        }

        public event BASSProgessHandler Progress;

        protected virtual void OnProgress()
        {
            if (Progress != null) Progress(this);
        }

        /// <summary>
        ///     Translate a byte position into time (seconds)
        /// </summary>
        /// <param name="pos">The position to translate</param>
        /// <returns>The millisecond position</returns>
        public double Bytes2Seconds(long pos)
        {
            if (_disposed)
                throw new ObjectDisposedException(ToString());

            double result = Bass.BASS_ChannelBytes2Seconds(_handle, pos); //_Bytes2Seconds(this._handle, pos);
            if (result < 0) throw new BASSException();
            return result;
        }

        //[DllImport("bass.dll", EntryPoint = "BASS_ChannelSeconds2Bytes")]
        //static extern long _Seconds2Bytes(IntPtr handle, float pos);//OK make decimal

        /// <summary>
        ///     Translate a time (seconds) position into bytes
        /// </summary>
        /// <param name="pos">The position to translate</param>
        /// <returns>The byte position</returns>
        public long Seconds2Bytes(double pos)
        {
            if (_disposed)
                throw new ObjectDisposedException(ToString());

            long result = Bass.BASS_ChannelSeconds2Bytes(_handle, pos); //_Seconds2Bytes(this._handle, pos);
            if (result < 0) throw new BASSException();
            return result;
        }

        //[DllImport("bass.dll", EntryPoint = "BASS_ChannelIsActive")]
        //static extern int _IsActive(IntPtr handle);//OK return dword

        //[DllImport("bass.dll", EntryPoint = "BASS_ChannelStop")]
        //static extern int _Stop(IntPtr handle);//OK return bool

        /// <summary>
        ///     Stop a channel.
        /// </summary>
        public virtual void Stop()
        {
            if (_disposed)
                throw new ObjectDisposedException(ToString());

            _progresstimer.Stop();
            if (!Bass.BASS_ChannelStop(_handle)) throw new BASSException();
        }

        //[DllImport("bass.dll", EntryPoint = "BASS_ChannelPause")]
        //static extern int _Pause(IntPtr handle);//OK return bool

        /// <summary>
        ///     Pause a channel.
        /// </summary>
        public void Pause()
        {
            if (_disposed)
                throw new ObjectDisposedException(ToString());

            if (!Bass.BASS_ChannelPause(_handle)) throw new BASSException();
        }

        //[DllImport("bass.dll", EntryPoint = "BASS_ChannelResume")]
        //static extern int _Resume(IntPtr handle);//OK return bool

        /// <summary>
        ///     Play a channel.
        /// </summary>
        public void Play()
        {
            if (_disposed)
                throw new ObjectDisposedException(ToString());

            if (!Bass.BASS_ChannelPlay(_handle, false)) throw new BASSException();

            StartTimer();
        }

        // handle : Channel handle (HCHANNEL/HMUSIC/HSTREAM, or CDCHANNEL)
        // pos    : the position
        //          if HCHANNEL: position in bytes
        //          if HMUSIC: LOWORD=order HIWORD=row ... use MAKELONG(order,row)
        //          if HSTREAM: position in bytes, file streams only
        //          if CDCHANNEL: position in milliseconds from start of track
        ///// <summary>
        /////     Used to override in derived classes
        ///// </summary>
        //[DllImport("bass.dll", EntryPoint = "BASS_ChannelSetPosition")]
        //protected static extern int _SetPosition(IntPtr handle, long pos); //OK retrn bool

        // Get the current playback position of a channel.
        // handle : Channel handle (HCHANNEL/HMUSIC/HSTREAM, or CDCHANNEL)
        // RETURN : the position (-1=error)
        //          if HCHANNEL: position in bytes
        //          if HMUSIC: LOWORD=order HIWORD=row (use GetLoWord(position), GetHiWord(Position))
        //          if HSTREAM: total bytes played since the stream was last flushed
        //          if CDCHANNEL: position in milliseconds from start of track
        ///// <summary>
        /////     Use to override in derived classes
        ///// </summary>
        //[DllImport("bass.dll", EntryPoint = "BASS_ChannelGetPosition")]
        //protected static extern long _GetPosition(IntPtr handle); //OK use decimal
    }
}