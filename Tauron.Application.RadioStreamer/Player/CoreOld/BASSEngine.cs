using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Tauron.JetBrains.Annotations;
using Un4seen.Bass;

namespace Tauron.Application.RadioStreamer.Player.CoreOld
{
    /// <summary>
    ///     Provides methods for initializing the BASSEngine engine.
    /// </summary>
    [PublicAPI]
    public sealed class BASSEngine : IDisposable
    {
        private bool _disposed;
        private IWindow _window;

        #region Constructors / Destructor

        /// <summary>
        ///     Constructor for BASSEngine
        /// </summary>
        /// <param name="device">Device to use (0=first, -1=default, -2=no sound)</param>
        /// <param name="freq">Output sample rate</param>
        /// <param name="flags">DeviceSetupFlags for device initialization</param>
        /// <param name="window"> Owner window (0=current foreground window)</param>
        /// <remarks>
        ///     The "no sound" device (device=-2), allows loading
        ///     and "playing" of MOD musics only (all sample/stream
        ///     functions and most other functions fail). This is so
        ///     that you can still use the MOD musics as synchronizers
        ///     when there is no soundcard present. When using device -2,
        ///     you should still set the other arguments as you would do
        ///     normally.
        /// </remarks>
        public BASSEngine(int device, int freq, BASSInit flags, [CanBeNull] IWindow window)
        {
            Init(device, freq, flags, window != null ? window.Handle : IntPtr.Zero);
            _window = window;
        }

//		public BASSEngine()
//		{
//
//			if (BASSEngine.Version != "1.6") 
//				throw new NotSupportedException("BASSEngine version 1.6 required. You have BASSEngine version " + BASSEngine.Version);
//			Init(-1, 44100, DeviceSetupFlags.Default, new IntPtr(0));
//			//_window = (Form)Form.FromHandle(iwin32.Handle);
//		}

//		public BASSEngine(IWin32Window window): this(-1, 44100, DeviceSetupFlags.Default, window.Handle)
//		{
//			_window = (Form)Form.FromHandle(window.Handle);
//		}

        [CanBeNull]
        public IWindow ParentWindow
        {
            get { return _window; }
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
            _Free();

            _disposed = true;
        }

        ~BASSEngine()
        {
            Dispose(false);
        }

        #endregion

        #region Sample Construction

        /// <summary>
        ///     Load a WAV/MP3/MP2/MP1 sample. If you//re loading a sample with 3D
        ///     functionality, then you should use BASS_GetInfo and BASS_SetInfo to set
        ///     the default 3D parameters. You can also use these two functions to set
        ///     the sample//s default frequency/volume/pan/looping.
        /// </summary>
        /// <param name="mem">Load sample from memory</param>
        /// <param name="filename">Filename or memory location</param>
        /// <param name="offset">File offset to load the sample from </param>
        /// <param name="length">Data length</param>
        /// <param name="max">Maximum number of simultaneous playbacks (1-65535)</param>
        /// <param name="flags">SampleInfoFlags</param>
        /// <returns></returns>
        [NotNull]
        public Sample LoadSample(bool mem, [NotNull] string filename, int offset,
            int length, int max, SampleInfoFlags flags)
        {
            if (_disposed)
                throw new ObjectDisposedException("BASSEngine");

            int handle = Bass.BASS_SampleLoad(filename, offset, length, max, (BASSFlag) flags);
            if (handle == 0) throw new BASSException();
            return new Sample(handle);
        }

        /// <summary>
        ///     Create a sample. This function allows you to generate custom samples, or
        ///     load samples that are not in the WAV format. A pointer is returned to the
        ///     memory location at which you should write the sample's data. After writing
        ///     the data, call BASS_SampleCreateDone to get the new sample's handle.
        /// </summary>
        /// <param name="freq">default sample rate</param>
        /// <param name="max">Maximum number of simultaneous playbacks (1-65535)</param>
        /// <param name="flags">SampleInfoFlags</param>
        /// <returns></returns>
        public Sample CreateSample(short[] data, int freq, int max, SampleInfoFlags flags)
        {
            if (_disposed)
                throw new ObjectDisposedException("BASSEngine");

            int sample = Bass.BASS_SampleCreate(data.Length, freq, 2, max, (BASSFlag) flags);
            if(sample == 0) throw new BASSException();

            if(!Bass.BASS_SampleSetData(sample, data))
                throw new BASSException();

            return new Sample(sample);

            //IntPtr memloc = _CreateSample(data.Length, freq, max, (int) flags);
            //Marshal.Copy(data, 0, memloc, data.Length);
            //if (memloc == IntPtr.Zero) throw new BASSException();
            //IntPtr sample = _CreateSampleDone(); /// ???????????
            //if (sample == IntPtr.Zero) throw new BASSException();
            //return new Sample(sample);
        }

        //// Finished creating a new sample.
        //// Return: The New sample //s handle (NULL=error)
        //[DllImport("bass.dll", EntryPoint = "BASS_SampleCreateDone")]
        //private static extern IntPtr _CreateSampleDone();

        //how do we handle all the handles????
        //		public void CreateDone()
        //		{
        //			IntPtr sample = _CreateDone();
        //			if (sample == IntPtr.Zero) throw new BASSException();
        //		}

        #endregion

        #region Music Construction

        //[DllImport("bass.dll", EntryPoint = "BASS_MusicLoad")]
        //private static extern IntPtr _LoadMusic(int mem, IntPtr file, int offset, int Length, int flags);

        ////OK, bool mem, return point to HMUSIC

        /// <summary>
        ///     Load a music (MO3/XM/MOD/S3M/IT/MTM). The amplification and pan
        ///     seperation are initially set to 50, use BASS_MusicSetAmplify()
        ///     and BASS_MusicSetPanSep() to adjust them.
        /// </summary>
        /// <param name="mem">Load music from memory</param>
        /// <param name="filename">Filename or memory location </param>
        /// <param name="offset">File offset to load the music from </param>
        /// <param name="length">Data length ( 0=use to end of file)</param>
        /// <param name="flags">MusicFlags</param>
        /// <returns></returns>
        [NotNull]
        public Music LoadMusic(bool mem, [NotNull] string filename, int offset, int length, MusicFlags flags)
        {
            if (_disposed) throw new ObjectDisposedException("BASSEngine");

            int handle = Bass.BASS_MusicLoad(filename, offset, length, (BASSFlag) flags, 0);
            if(handle == 0) throw new BASSException();

            return new Music(handle) { Owner = this};

            //IntPtr pfile = Marshal.StringToHGlobalAnsi(filename); //must be like this
            //IntPtr mus = _LoadMusic(Helper.Bool2Int(mem), pfile,
            //    offset, length, (int) flags);
            //if (mus == IntPtr.Zero) throw new BASSException();
            //var output = new Music(mus);
            //output.Owner = this;
            //return output;
        }

        #endregion

        #region BassStream Construction

        // 
        // freq   : 
        // flags  : BASS_SAMPLE_xxx flags (only the 8BITS/MONO/3D flags are used)
        // proc   : User defined stream writing function pass using "AddressOf STREAMPROC"
        // user   : The //user// value passed to the callback function
        // RETURN : The created stream//s handle (NULL=error)
        [DllImport("bass.dll", EntryPoint = "BASS_StreamCreate")]
        private static extern IntPtr _CreateStream(int freq, int flags, StreamCallBack proc, int user); //OK

        //not properly implemented yet
        /// <summary>
        ///     Create a user sample stream.
        /// </summary>
        /// <param name="freq">BassStream playback rate</param>
        /// <param name="flags">StreamFlags</param>
        /// <param name="proc">StreamCallBack delegate</param>
        /// <param name="user">The "user" value passed to the callback function</param>
        /// <returns></returns>
        private BassStream CreateStream(int freq, StreamFlags flags, StreamCallBack proc, int user)
        {
            if (_disposed)
                throw new ObjectDisposedException("BASSEngine");

            IntPtr handle = _CreateStream(freq, (int) flags, proc, user);
            if (handle == IntPtr.Zero) throw new BASSException();
            var output = new BassStream(handle);
            output.Owner = this;
            return output;
        }

        [DllImport("bass.dll", EntryPoint = "BASS_StreamCreateFile")]
        private static extern IntPtr _CreateStreamFile(int mem, IntPtr file, int offset,
            int Length, int flags); //OK, mem is bool

        /// <summary>
        ///     Create a sample stream from an MP3/MP2/MP1/OGG or WAV file.
        /// </summary>
        /// <param name="mem">BassStream file from memory</param>
        /// <param name="filename">Filename or memory location</param>
        /// <param name="offset">File offset of the stream data</param>
        /// <param name="length">File length (0=use whole file)</param>
        /// <param name="flags">StreamFlags</param>
        /// <returns></returns>
        public BassStream LoadStream(bool mem, string filename, int offset,
            int length, StreamFlags flags)
        {
            if (_disposed)
                throw new ObjectDisposedException("BASSEngine");

            IntPtr handle = _CreateStreamFile(Helper.Bool2Int(mem), Marshal.StringToHGlobalAnsi(filename),
                offset, length, (int) flags);
            if (handle == IntPtr.Zero) throw new BASSException();
            var output = new BassStream(handle);
            output.Owner = this;
            return output;
        }

        [DllImport("bass.dll", EntryPoint = "BASS_StreamCreateURL")]
        private static extern IntPtr _CreateStreamURL(string url, int offset, int flags, string save); //OK

        /// <summary>
        ///     Create a sample stream from an MP3/MP2/MP1/OGG or WAV file on the internet.
        /// </summary>
        /// <param name="url">The URL (beginning with "http://" or "ftp://")</param>
        /// <param name="offset">File offset of start streaming from</param>
        /// <param name="flags">StreamFlags</param>
        /// <param name="savefile">Filename to save the streamed file as locally (""=don//t save)</param>
        /// <returns></returns>
        public BassStream CreateStreamFromURL(string url, int offset, StreamFlags flags, string savefile)
        {
            if (_disposed)
                throw new ObjectDisposedException("BASSEngine");

            IntPtr handle = _CreateStreamURL(url, offset, (int) flags, savefile);
            if (handle == IntPtr.Zero) throw new BASSException();
            var output = new BassStream(handle);
            output.Owner = this;
            return output;
        }

        #endregion

        #region Main Functions

        //OK

        public enum Curve
        {
            Linear = 0,
            Logarithmic = 1,
        }

        /// <summary>
        ///     Retrieve the version number of BASSEngine DLL that is used.
        /// </summary>
        public static string Version
        {
            get
            {
                int ver = _GetVersion();
                return String.Format("{0}.{1}", Helper.LoWord(ver), Helper.HiWord(ver));
            }
        }

        /// <summary>
        ///     Number of available devices
        /// </summary>
        public int DeviceCount
        {
            get
            {
                if (_disposed)
                    throw new ObjectDisposedException("BASSEngine");

                int count = 0;
                try
                {
                    while (_GetDeviceDescription(count++) != IntPtr.Zero)
                    {
                    }
                }
                catch (BASSException)
                {
                } // what is count or do i need to minus 1?
                return count - 1;
            }
        }

        /// <summary>
        ///     Gets/sets music volume
        /// </summary>
        public unsafe int MusicVolume
        {
            set
            {
                if (_disposed)
                    throw new ObjectDisposedException("BASSEngine");

                _SetGlobalVolumes(value, -1, -1);
            }
            get
            {
                if (_disposed)
                    throw new ObjectDisposedException("BASSEngine");

                int output = 0;
                _GetGlobalVolumes(&output, null, null);
                return output;
            }
        }

        /// <summary>
        ///     Gets/sets sample volume
        /// </summary>
        public unsafe int SampleVolume
        {
            set
            {
                if (_disposed)
                    throw new ObjectDisposedException("BASSEngine");

                _SetGlobalVolumes(-1, value, -1);
            }
            get
            {
                if (_disposed)
                    throw new ObjectDisposedException("BASSEngine");

                int output = 0;
                _GetGlobalVolumes(null, &output, null);
                return output;
            }
        }

        /// <summary>
        ///     Gets/sets stream volume
        /// </summary>
        public unsafe int StreamVolume
        {
            set
            {
                if (_disposed)
                    throw new ObjectDisposedException("BASSEngine");

                _SetGlobalVolumes(-1, -1, value);
            }
            get
            {
                if (_disposed)
                    throw new ObjectDisposedException("BASSEngine");

                int output = 0;
                _GetGlobalVolumes(null, null, &output);
                return output;
            }
        }

        /// <summary>
        ///     Set the class identifier of the object to create, that will be used
        ///     to initialize DirectSound.
        /// </summary>
        public Guid CLSID
        {
            set
            {
                if (_disposed)
                    throw new ObjectDisposedException("BASSEngine");

                _SetCLSID(ref value);
            }
        }

        /// <summary>
        ///     Retrieve some information on the device being used.
        /// </summary>
        [TypeConverter("System.ComponentModel.ExpandableObjectConverter")]
        public BASSInfo Info
        {
            get
            {
                if (_disposed)
                    throw new ObjectDisposedException("BASSEngine");

                var bi = new BASSInfo();
                bi.Size = Marshal.SizeOf(typeof (BASSInfo)); // workinto struct
                _GetInfo(ref bi);
                return bi;
            }
        }

        /// <summary>
        ///     Get the current CPU usage of BASSEngine. This includes the time taken to mix
        ///     the MOD musics and sample streams, and also the time taken by any user
        ///     DSP functions. It does not include plain sample mixing which is done by
        ///     the output device (hardware accelerated) or DirectSound (emulated). Audio
        ///     CD playback requires no CPU usage.
        /// </summary>
        public float CPUUsage
        {
            get { return _GetCPU(); }
        }

        /// <summary>
        ///     Gets/sets the digital output master volume.
        /// </summary>
        public int MasterVolume
        {
            get
            {
                if (_disposed)
                    throw new ObjectDisposedException("BASSEngine");

                int output = _GetVolume();
                if (output == -1) throw new BASSException();
                return output;
            }
            set
            {
                if (_disposed)
                    throw new ObjectDisposedException("BASSEngine");

                if (_SetVolume(value) == 0) throw new BASSException();
            }
        }

        [DllImport("bass.dll", EntryPoint = "BASS_GetVersion")]
        private static extern int _GetVersion();

        [DllImport("bass.dll", EntryPoint = "BASS_GetDeviceDescription")]
        private static extern IntPtr _GetDeviceDescription(int devnum);

        //OK, Ansi note, returns char *WINAPI == LPSTR???

        /// <summary>
        ///     Gets a text description of a device. This function can be used to enumerate the available devices.
        /// </summary>
        /// <param name="device">The device (0=First)</param>
        /// <returns>The text description of the device</returns>
        public string GetDeviceDescription(int device)
        {
            if (_disposed)
                throw new ObjectDisposedException("BASSEngine");

            IntPtr desc = _GetDeviceDescription(device);
            if (desc == IntPtr.Zero) throw new BASSException();
            return Marshal.PtrToStringAnsi(desc);
        }

        [DllImport("bass.dll", EntryPoint = "BASS_SetBufferLength")]
        private static extern float _SetBufferLength(float Length); //OK

        /// <summary>
        ///     Set the amount that BASSEngine mixes ahead new musics/streams.
        ///     Changing this setting does not affect musics/streams
        ///     that have already been loaded/created. Increasing the
        ///     buffer length, decreases the chance of the sound
        ///     possibly breaking-up on slower computers, but also
        ///     requires more memory. The default length is 0.5 secs.
        /// </summary>
        /// <param name="length">The buffer length in seconds</param>
        /// <returns>The actual new buffer length</returns>
        public float SetBufferLength(float length)
        {
            if (_disposed)
                throw new ObjectDisposedException("BASSEngine");

            return _SetBufferLength(length);
        }


        // Set the global music/sample/stream volume levels.
        // musvol : MOD music global volume level (0-100, -1=leave current)
        // samvol : Sample global volume level (0-100, -1=leave current)
        // strvol : BassStream global volume level (0-100, -1=leave current)
        [DllImport("bass.dll", EntryPoint = "BASS_SetGlobalVolumes")]
        private static extern void _SetGlobalVolumes(int musvol, int samvol, int strvol); //OK

        // Retrive the global music/sample/stream volume levels.
        // musvol : MOD music global volume level (NULL=don//t retrieve it)
        // samvol : Sample global volume level (NULL=don//t retrieve it)
        // strvol : BassStream global volume level (NULL=don//t retrieve it)
        [DllImport("bass.dll", EntryPoint = "BASS_GetGlobalVolumes")]
        private static extern unsafe void _GetGlobalVolumes(
            int* musvol,
            int* samvol,
            int* strvol); //OK


        [DllImport("bass.dll", EntryPoint = "BASS_SetLogCurves")]
        private static extern void _SetLogCurves(int volume, int pan); //OK, but want to change

        /// <summary>
        ///     Make the volume/panning values translate to a logarithmic curve,
        ///     or a linear "curve" (the default)
        /// </summary>
        /// <param name="volume">Volume Curve</param>
        /// <param name="panning">Panning Curve</param>
        public void SetLogCurves(Curve volume, Curve panning)
        {
            if (_disposed)
                throw new ObjectDisposedException("BASSEngine");

            _SetLogCurves((int) volume, (int) panning);
        }

        [DllImport("bass.dll", EntryPoint = "BASS_SetCLSID")]
        private static extern void _SetCLSID(ref Guid clsid); //OK


        private void Init(int device, int freq, BASSInit flags, IntPtr hwnd)
        {
            if (_disposed)
                throw new ObjectDisposedException("BASSEngine");

            if (!Bass.BASS_Init(device, freq, flags, hwnd)) throw new BASSException();
        }

        // Free all resources used by the digital output, including  all musics and samples.
        [DllImport("bass.dll", EntryPoint = "BASS_Free")]
        private static extern void _Free(); //OK

        private void Free()
        {
            _Free();
        }

        // Retrieve a pointer to a DirectSound interface. This can be used by
        // advanced users to "plugin" external functionality.
        // object : The interface to retrieve (BASS_OBJECT_xxx)
        // RETURN : A pointer to the requested interface (NULL=error)
        //UPGRADE_NOTE: object was upgraded to object_Renamed. Click for more: //ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="vbup1061"//
        [DllImport("bass.dll", EntryPoint = "BASS_GetDSoundObject")]
        private static extern IntPtr GetDSoundObject(uint dsobject); //???????? dsobject is handle

        [DllImport("bass.dll", EntryPoint = "BASS_GetInfo")]
        private static extern void _GetInfo(
            ref BASSInfo info); //OK

        [DllImport("bass.dll", EntryPoint = "BASS_Update")]
        private static extern int _Update(); //OK, rtruns bool, err code

        /// <summary>
        ///     Update the MUSIC/STREAM channel buffers.
        /// </summary>
        public void Update()
        {
            if (_disposed)
                throw new ObjectDisposedException("BASSEngine");

            if (_Update() == 0) throw new BASSException();
        }

        [DllImport("bass.dll", EntryPoint = "BASS_GetCPU")]
        private static extern float _GetCPU(); //OK

        [DllImport("bass.dll", EntryPoint = "BASS_Start")]
        private static extern int _Start(); //OK, return bool, err code

        /// <summary>
        ///     Start the digital output.
        /// </summary>
        public void Start()
        {
            if (_disposed)
                throw new ObjectDisposedException("BASSEngine");

            if (_Start() == 0) throw new BASSException();
        }

        [DllImport("bass.dll", EntryPoint = "BASS_Stop")]
        private static extern int _Stop(); //OK, return bool, err code

        /// <summary>
        ///     Stop the digital output, stopping all musics/samples/streams.
        /// </summary>
        public void Stop()
        {
            if (_disposed)
                throw new ObjectDisposedException("BASSEngine");

            if (_Stop() == 0) throw new BASSException();
        }

        [DllImport("bass.dll", EntryPoint = "BASS_Pause")]
        private static extern int _Pause(); //OK return bool, err code

        /// <summary>
        ///     Stop the digital output, pausing all musics/samples/
        ///     streams. Use <see cref="Start()" />Start to resume the digital output.
        /// </summary>
        public void Pause()
        {
            if (_disposed)
                throw new ObjectDisposedException("BASSEngine");

            if (_Pause() == 0) throw new BASSException();
        }

        [DllImport("bass.dll", EntryPoint = "BASS_SetVolume")]
        private static extern int _SetVolume(int volume); //OK return bool err code

        [DllImport("bass.dll", EntryPoint = "BASS_GetVolume")]
        private static extern int _GetVolume(); //OK

        #endregion

        #region BASS3D

        /// <summary>
        ///     This function is a workaround, because VB/C# doesn,t support multiple comma seperated
        ///     paramaters for each Global Const, simply pass the BASSEAXPreset value to this function
        ///     instead of BASS_SetEasParamaets as you would to in C++
        /// </summary>
        public EAXPreset EAXPreset
        {
            set
            {
                if (_disposed)
                    throw new ObjectDisposedException("BASSEngine");

                if (!SetEAXParametersEx(value)) throw new BASSException();
            }
        }

        /// <summary>
        ///     Set the 3D algorithm for software mixed 3D channels (does not affect
        ///     hardware mixed channels). Changing the mode only affects subsequently
        ///     created or loaded samples/streams/musics, not those that already exist.
        ///     Requires DirectX 7 or above.
        /// </summary>
        public BASS3DAlgorithm Algorithm3D
        {
            set
            {
                if (_disposed)
                    throw new ObjectDisposedException("BASSEngine");

                Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_3DALGORITHM, (int)value);
            }
        }

        /// <summary>
        ///     Gets/sets the factors that affect the calculations of 3D sound.
        /// </summary>
        public BASS3DFactors Factors3D
        {
            get
            {
                if (_disposed) throw new ObjectDisposedException("BASSEngine");

                float distf = 0;
                float rollf = 0;
                float doppf = 0;
                if (!Bass.BASS_Get3DFactors(ref distf, ref rollf, ref doppf)) throw new BASSException();
                return new BASS3DFactors(distf, rollf, doppf);
            }
            set
            {
                if (_disposed)
                    throw new ObjectDisposedException("BASSEngine");

                float distf = value.Distf;
                float rollf = value.Rollf;
                float doppf = value.Doppf;

                if (!Bass.BASS_Set3DFactors(distf, rollf, doppf))
                    throw new BASSException();
            }
        }

        /// <summary>
        ///     Gets/sets the position/velocity/orientation of the listener.
        /// </summary>
        public BASS3DPosition Position3D
        {
            get
            {
                if (_disposed)
                    throw new ObjectDisposedException("BASSEngine");

                var pos = new BASS_3DVECTOR();
                var vel = new BASS_3DVECTOR();
                var front = new BASS_3DVECTOR();
                var top = new BASS_3DVECTOR(); ;
                if (!Bass.BASS_Get3DPosition(pos, vel, front, top))
                    throw new BASSException();
                return new BASS3DPosition(pos, vel, top, front);
            }
            set
            {
                if (_disposed)
                    throw new ObjectDisposedException("BASSEngine");

                BASS_3DVECTOR pos = value.Pos;
                BASS_3DVECTOR vel = value.Vel;
                BASS_3DVECTOR front = value.Front;
                BASS_3DVECTOR top = value.Top;
                if (!Bass.BASS_Set3DPosition(pos, vel, front, top))
                    throw new BASSException();
            }
        }

        /// <summary>
        ///     Get/Set the current EAX parameters. Obviously, EAX functions
        ///     have no effect if no EAX supporting device (ie. SB Live) is used.
        /// </summary>
        public BASSEAXParameters EAXParameters
        {
            get
            {
                if (_disposed)
                    throw new ObjectDisposedException("BASSEngine");

                EAXEnvironment env = 0;
                float vol = 0;
                float decay = 0;
                float damp = 0;
                if (!Bass.BASS_GetEAXParameters(ref env, ref vol, ref decay, ref damp))
                    throw new BASSException();
                return new BASSEAXParameters(env, vol, decay, damp);
            }
            set
            {
                if (_disposed)
                    throw new ObjectDisposedException("BASSEngine");

                if (!Bass.BASS_SetEAXParameters(value.Env, value.Vol, value.Decay, value.Damp))
                    throw new BASSException();
            }
        }

        private bool SetEAXParametersEx(EAXPreset preset) //object refPreset
        {
            if (_disposed)
                throw new ObjectDisposedException("BASSEngine");

            Bass.BASS_SetEAXParameters(preset);
            //switch (preset)
            //{
            //    case EAXPreset.EAX_PRESET_GENERIC:
            //        return _SetEAXParameters((int) EAXEnvironment.Generic, 0.5F, 1.493F, 0.5F);
            //    case EAXPreset.EAX_PRESET_PADDEDCELL:
            //        return _SetEAXParameters((int) EAXEnvironment.PaddedCell, 0.25F, 0.1F, 0F);
            //    case EAXPreset.EAX_PRESET_ROOM:
            //        return _SetEAXParameters((int) EAXEnvironment.Room, 0.417F, 0.4F, 0.666F);
            //    case EAXPreset.EAX_PRESET_BATHROOM:
            //        return _SetEAXParameters((int) EAXEnvironment.Bathroom, 0.653F, 1.499F, 0.166F);
            //    case EAXPreset.EAX_PRESET_LIVINGROOM:
            //        return _SetEAXParameters((int) EAXEnvironment.LivingRoom, 0.208F, 0.478F, 0F);
            //    case BASSEAXPrese:
            //        return _SetEAXParameters((int) EAXEnvironment.StoneRoom, 0.5F, 2.309F, 0.888F);
            //    case BASSEAXPreset.Auditorium:
            //        return _SetEAXParameters((int) EAXEnvironment.Auditorium, 0.403F, 4.279F, 0.5F);
            //    case BASSEAXPreset.ConcertHall:
            //        return _SetEAXParameters((int) EAXEnvironment.ConcertHall, 0.5F, 3.961F, 0.5F);
            //    case BASSEAXPreset.Cave:
            //        return _SetEAXParameters((int) EAXEnvironment.Cave, 0.5F, 2.886F, 1.304F);
            //    case BASSEAXPreset.Arena:
            //        return _SetEAXParameters((int) EAXEnvironment.Arena, 0.361F, 7.284F, 0.332F);
            //    case BASSEAXPreset.Hangar:
            //        return _SetEAXParameters((int) EAXEnvironment.Hangar, 0.5F, 10F, 0.3F);
            //    case BASSEAXPreset.CarpetedHallway:
            //        return _SetEAXParameters((int) EAXEnvironment.CarpetedHallway, 0.153F, 0.259F, 2F);
            //    case BASSEAXPreset.Hallway:
            //        return _SetEAXParameters((int) EAXEnvironment.Hallway, 0.361F, 1.493F, 0F);
            //    case BASSEAXPreset.StoneCorridor:
            //        return _SetEAXParameters((int) EAXEnvironment.StoneCorridor, 0.444F, 2.697F, 0.638F);
            //    case BASSEAXPreset.Alley:
            //        return _SetEAXParameters((int) EAXEnvironment.Alley, 0.25F, 1.752F, 0.776F);
            //    case BASSEAXPreset.Forest:
            //        return _SetEAXParameters((int) EAXEnvironment.Forest, 0.111F, 3.145F, 0.472F);
            //    case BASSEAXPreset.City:
            //        return _SetEAXParameters((int) EAXEnvironment.City, 0.111F, 2.767F, 0.224F);
            //    case BASSEAXPreset.Mountains:
            //        return _SetEAXParameters((int) EAXEnvironment.Mountains, 0.194F, 7.841F, 0.472F);
            //    case BASSEAXPreset.Quarry:
            //        return _SetEAXParameters((int) EAXEnvironment.Quarry, 1F, 1.499F, 0.5F);
            //    case BASSEAXPreset.Plain:
            //        return _SetEAXParameters((int) EAXEnvironment.Plain, 0.097F, 2.767F, 0.224F);
            //    case BASSEAXPreset.ParkingLot:
            //        return _SetEAXParameters((int) EAXEnvironment.ParkingLot, 0.208F, 1.652F, 1.5F);
            //    case BASSEAXPreset.SewerPipe:
            //        return _SetEAXParameters((int) EAXEnvironment.SewerPipe, 0.652F, 2.886F, 0.25F);
            //    case BASSEAXPreset.Underwater:
            //        return _SetEAXParameters((int) EAXEnvironment.Underwater, 1F, 1.499F, 0F);
            //    case BASSEAXPreset.Drugged:
            //        return _SetEAXParameters((int) EAXEnvironment.Drugged, 0.875F, 8.392F, 1.388F);
            //    case BASSEAXPreset.Dizzy:
            //        return _SetEAXParameters((int) EAXEnvironment.Dizzy, 0.139F, 17.234F, 0.666F);
            //    case BASSEAXPreset.Psychotic:
            //        return _SetEAXParameters((int) EAXEnvironment.Psychotic, 0.486F, 7.563F, 0.806F);
            //    default:
            //        goto case BASSEAXPreset.Generic;
            //}
        }

        //[DllImport("bass.dll", EntryPoint = "BASS_Set3DAlgorithm")]
        //private static extern void _Set3DAlgorithm(int algo); //OK

        //[DllImport("bass.dll", EntryPoint = "BASS_Set3DFactors")]
        //private static extern int _Set3DFactors(
        //    ref float distf,
        //    ref float rollf,
        //    ref float doppf); //OK return bool err code

        //[DllImport("bass.dll", EntryPoint = "BASS_Get3DFactors")]
        //private static extern int _Get3DFactors(
        //    out float distf,
        //    out float rollf,
        //    out float doppf); //OK return bool err code

        //// NOTE   : front & top must both be set in a single call
        //[DllImport("bass.dll", EntryPoint = "BASS_Set3DPosition")]
        //private static extern int _Set3DPosition(
        //    ref Vector3D pos,
        //    ref Vector3D vel,
        //    ref Vector3D front,
        //    ref Vector3D top); //OK return bool err code

        //// NOTE   : front & top must both be retrieved in a single call
        //[DllImport("bass.dll", EntryPoint = "BASS_Get3DPosition")]
        //private static extern int _Get3DPosition(
        //    out Vector3D pos,
        //    out Vector3D vel,
        //    out Vector3D front,
        //    out Vector3D top); //OK return bool err code

        //[DllImport("bass.dll", EntryPoint = "BASS_Apply3D")]
        //private static extern int _Apply3D(); //OK return bool err code

        /// <summary>
        ///     Apply changes made to the 3D system. This must be called to apply any changes
        ///     made with BASSEngine.Factors3D, BASSEngine.Position3D, BASSEngine.Channel3DAttributes or
        ///     BASSEngine.Channel3DPosition. It improves performance to have DirectSound do all the
        ///     required recalculating at the same time like this, rather than recalculating after
        ///     every little change is made.
        /// </summary>
        /// <remarks>
        ///     This is automatically called when starting a 3D
        ///     sample with Sample.Play3D.
        /// </remarks>
        public void Apply3D()
        {
            if (_disposed)
                throw new ObjectDisposedException("BASSEngine");

            Bass.BASS_Apply3D();
        }

        //[DllImport("bass.dll", EntryPoint = "BASS_SetEAXParameters")]
        //private static extern int _SetEAXParameters(
        //    int env,
        //    float vol,
        //    float decay,
        //    float damp); //OK return bool err code

        //[DllImport("bass.dll", EntryPoint = "BASS_GetEAXParameters")]
        //private static extern int _GetEAXParameters(
        //    out int env,
        //    out float vol,
        //    out float decay,
        //    out float damp); //OK return bool err code

        #endregion
    }
}