using System;
using Tauron.JetBrains.Annotations;
using Un4seen.Bass;

namespace Tauron.Application.RadioStreamer.Player.Core
{
	/// <summary>
	/// Provides methods for playing audio samples.
	/// </summary>
	[PublicAPI]
	public sealed class Sample : IDisposable
	{
		private bool _disposed;
		int _handle;

		internal Sample(int handle)
		{
			_handle = handle;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

	    private void Dispose(bool disposing)
		{
		    if (_disposed) return;

		    // free unmanaged resources
		    Bass.BASS_SampleFree(_handle);

		    _disposed = true;
		}

		~Sample()
		{
			Dispose(false);
		}

		internal int Handle 
		{
			get
			{
				if (_disposed)
					throw new ObjectDisposedException(ToString());

				return _handle;
			}
		}
	
		#region Instance

        //// Free a sample//s resources.
        //// handle:    Sample handle
        //[DllImport("bass.dll", EntryPoint = "BASS_SampleFree")]
        //static extern void _Free(IntPtr handle);

        //[DllImport("bass.dll", EntryPoint = "BASS_SampleGetInfo")]
        //static extern int GetInfo(IntPtr handle, ref SampleInfo info); //return bool er code

        //[DllImport("bass.dll", EntryPoint = "BASS_SampleSetInfo")]
        //static extern int SetInfo(IntPtr handle, ref SampleInfo info);//return bool er code

		/// <summary>
		/// Sets/Retrieves a sample's current default attributes.
		/// </summary>
		/// <value>
		/// The SampleInfo struct
		/// </value>
		[NotNull]
		public BASS_SAMPLE Info 
		{
			get 
			{
				if (_disposed)
					throw new ObjectDisposedException("Sample");

                var si = Bass.BASS_SampleGetInfo(_handle);
				if (si == null) throw new BASSException();
				return si;
			}
			set
			{
			    if (_disposed)
			        throw new ObjectDisposedException("Sample");

			    if (!Bass.BASS_SampleSetInfo(_handle, value)) throw new BASSException();
			}
		}

        //[DllImport("bass.dll", EntryPoint = "BASS_SamplePlay")]
        //static extern IntPtr _Play(IntPtr handle); //return  HCHANNEL handle else err code

        /// <summary>
        /// Play a sample, using the sample's default attributes.
        /// </summary>
        /// <returns>A channel object</returns>
        [NotNull]
        public Channel Play()
        {
            var temp = GetChannel();
            temp.Play();
            return temp;
        }

	    [NotNull]
		public Channel GetChannel()
		{
            if (_disposed)
                throw new ObjectDisposedException("Sample");

		    int channel = Bass.BASS_SampleGetChannel(_handle, false);
		    if (channel == 0) throw new BASSException();
		    return new Channel(channel);
		}

	    /// <summary>
	    /// Play a sample, using specified attributes.
	    /// </summary>
	    /// <param name="freq">Playback Rate(-1 = Default)</param>
	    /// <param name="volume">Volume (-1=default, 0=silent, 100=max)</param>
	    /// <param name="panning">pan position(-101 = Default, -100 = Left, 0 = middle, 100 = Right)</param>
	    /// <param name="loop">1 = Loop sample (-1=default)</param>
	    /// <returns></returns>
	    [NotNull]
	    public Channel Play(int freq, float volume, float panning, bool loop)
	    {
	        if (_disposed)
	            throw new ObjectDisposedException("Sample");

	        var channel = GetChannel();

	        channel.Attributes.Frequency = freq;
	        channel.Attributes.Volume = volume;
	        channel.Attributes.Panning = panning;

	        channel.Play();

	        return channel;
	    }

	    /// <summary>
		/// Stops all instances of a sample. For example, if a sample is playing
		/// simultaneously 3 times, calling this function will stop all 3 of them,
		/// which is obviously simpler than calling Channel.Stop() 3 times.
		/// </summary>
		public void Stop()
		{
			if (_disposed)
				throw new ObjectDisposedException("Sample");

			if (!Bass.BASS_SampleStop(_handle)) throw new BASSException();
		}

		#endregion

	}


}
