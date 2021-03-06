using System;
using Tauron.JetBrains.Annotations;
using Un4seen.Bass;

namespace Tauron.Application.RadioStreamer.Player.CoreOld
{
	/// <summary>
	/// Summary description for Music.
	/// </summary>
	public class Music : AdvancedChannel
	{
		private bool _disposed;

		#region Construction / Desctruction
		internal Music(int handle) : base(handle) {}

		protected override void Dispose(bool disposing)
		{
		    if (_disposed) return;
		    try
		    {
		        // release any unmanaged resources
		        Free();

		        _disposed = true;
		    }
		    finally
		    {
		        base.Dispose(disposing);
		    }
		}

	    #endregion
		
		#region Unique Instance

		void Free()
		{
			Bass.BASS_MusicFree(Handle);
		}

        //[DllImport("bass.dll", EntryPoint = "BASS_MusicGetName")]
        //static extern IntPtr _GetName(IntPtr handle);//OK, return LPSTR

		/// <summary>
		/// Get the music's name
		/// </summary>
		[NotNull]
		public string Name 
		{
			get 
			{
				if (_disposed)
					throw new ObjectDisposedException("Music");

				string name = Bass.BASS_ChannelGetMusicName(Handle);
				if (name == null) throw new BASSException();
				return name;
			}
		}

        //[DllImport("bass.dll", EntryPoint = "BASS_MusicGetLength")]
        //static extern long _GetLength(IntPtr handle, int playlen);//OK, 2 param bool

		/// <summary>
		/// Retrieves the playback length in seconds. Must use MusicFlags.CalculateLength to function.
		/// </summary>
		public double PlaybackLength 
		{
			get 
			{
				if (_disposed)
					throw new ObjectDisposedException("Music");

				long len = Bass.BASS_ChannelGetLength( Handle, BASSMode.BASS_POS_BYTES);
				if (len == -1) throw new BASSException();
				return Bytes2Seconds(len);
			}
		}

		/// <summary>
		/// Retrieves the pattern length of the music file.
		/// </summary>
		public long PatternLength 
		{
			get 
			{
				if (_disposed)
					throw new ObjectDisposedException("Music");

				long len = Bass.BASS_ChannelGetLength(Handle, BASSMode.BASS_POS_MUSIC_ORDERS);
				if (len < 0) throw new BASSException();
				return len;
			}
		}

        //[DllImport("bass.dll", EntryPoint = "BASS_MusicPreBuf")]
        //static extern int _PreBuf(IntPtr handle);//OK return bool, error code

        ///// <summary>
        ///// Pre-buffer initial sample data ready for playback.
        ///// </summary>
        //public void PreBuffer()
        //{
        //    if (this._disposed)
        //        throw new ObjectDisposedException("Music");

        //    if (_PreBuf(base.Handle) == 0) throw new BASSException();
        //}

        //[DllImport("bass.dll", EntryPoint = "BASS_MusicPlay")]
        //static extern int _Play(IntPtr handle);//OK return bool, error code

        ///// <summary>
        ///// Play a music. Playback continues from where it was last stopped/paused.
        ///// Multiple musics may be played simultaneously.
        ///// </summary>
        //public void Play()
        //{
        //    if (_disposed)
        //        throw new ObjectDisposedException("Music");

        //    if (_Play(base.Handle) == 0) throw new BASSException();
        //    //start progress timer
        //    base.StartTimer();
        //}

        //[DllImport("bass.dll", EntryPoint = "BASS_MusicPlayEx")]
        //static extern int _PlayEx(IntPtr handle, int pos, int flags, int reset); //OK return bool, error code reset is bool

        ///// <summary>
        ///// Play a music, specifying start position and playback flags.
        ///// </summary>
        ///// <param name="pos">Position to start playback from, LOWORD=order HIWORD=row</param>
        ///// <param name="flags">MusicFlags</param>
        ///// <param name="reset">Stop all current playing notes and reset bpm/etc...</param>
        //public void Play(int pos, MusicFlags flags, bool reset)
        //{
        //    if (this._disposed)
        //        throw new ObjectDisposedException("Music");

        //    if (_PlayEx(base.Handle, pos, (int) flags, Helper.Bool2Int(reset)) == 0)
        //        throw new BASSException();
        //    //start progress timer
        //    base.StartTimer();
        //}

        //[DllImport("bass.dll", EntryPoint = "BASS_MusicSetAmplify(")]
        //static extern int _SetAmplify(IntPtr handle, int amp);//OK return bool, error code

		/// <summary>
		/// Set a music's amplification level (0 - 100).
		/// </summary>
		public float Amplification 
		{
			set 
			{
				if (_disposed)
					throw new ObjectDisposedException("Music");

			    if(!Bass.BASS_ChannelSetAttribute(Handle, BASSAttribute.BASS_ATTRIB_MUSIC_AMPLIFY, value))
                    throw new BASSException();
			}
		}

        //[DllImport("bass.dll", EntryPoint = "BASS_MusicSetPanSep")]
        //static extern int _SetPanSep(IntPtr handle, int pan);//OK return bool, error code

		/// <summary>
		/// Set a music's pan seperation (0 - 100, 50 = linear).
		/// </summary>
		public int PanningSeperation
		{
			set 
			{
				if (_disposed)
					throw new ObjectDisposedException("Music");

				if (!Bass.BASS_ChannelSetAttribute(Handle, BASSAttribute.BASS_ATTRIB_MUSIC_PANSEP, value)) throw new BASSException();
			}
		}

        //[DllImport("bass.dll", EntryPoint = "BASS_MusicSetPositionScaler")]
        //static extern int _SetPositionScaler(IntPtr handle, int pscale);//OK return bool, error code

		/// <summary>
		/// Set a music//s "GetPosition" scaler
		/// When you call BASS_ChannelGetPosition, the "row" (HIWORD) will be
		/// scaled by this value. By using a higher scaler, you can get a more
		/// precise position indication. The scaler(1 - 256).
		/// </summary>
		public int PositionScaler 
		{
			set 
			{
				if (_disposed)
					throw new ObjectDisposedException("Music");

				if (!Bass.BASS_ChannelSetAttribute(Handle, BASSAttribute.BASS_ATTRIB_MUSIC_PSCALER, value)) throw new BASSException();
			}
		}

        //[DllImport("bass.dll", EntryPoint = "BASS_MusicSetChannelVol")]
        //static extern int _SetChannelVol(IntPtr handle, int channel, int volume);//OK return bool, error code

        ///// <summary>
        ///// Set the volume level of a channel in a music
        ///// </summary>
        ///// <param name="channel">Channel number (0=first)</param>
        ///// <param name="volume"> Volume level (0-100)</param>
        //public void SetChannelVolume( int channel, int volume)
        //{
        //    if (_disposed)
        //        throw new ObjectDisposedException("Music");

        //    if (!Bass.mu) throw new BASSException();
        //}

    //    [DllImport("bass.dll", EntryPoint = "BASS_MusicGetChannelVol")]
    //    static extern int _GetChannelVol(IntPtr handle, int channel);//OK return bool, error code

    //    /// <summary>
    //    /// Get the volume level of a channel in a music
    //    /// </summary>
    //    /// <param name="channel">Channel number (0=first)</param>
    //    /// <returns> The channel's volume </returns>
    //    public int GetChannelVolume(int channel)
    //    {
    //        if (this._disposed)
    //            throw new ObjectDisposedException("Music");

    //        int vol = _GetChannelVol(base.Handle, channel);
    //        if (vol < 0) throw new BASSException();
    //        return vol;
    //    }

        #endregion

    }
}
