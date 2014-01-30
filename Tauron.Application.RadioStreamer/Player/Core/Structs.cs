//#define RECORD

using System;
using System.Runtime.InteropServices;
using Tauron.JetBrains.Annotations;
using Un4seen.Bass;

namespace Tauron.Application.RadioStreamer.Player.Core
{
    /// <summary>
    ///     Used with getting and setting Channel3DPosition
    /// </summary>
    public class Channel3DPosition
    {
        private BASS_3DVECTOR _orient;
        private BASS_3DVECTOR _pos;
        private BASS_3DVECTOR _vel;

        /// <summary>
        ///     Used with getting and setting Channel3DPosition
        /// </summary>
        /// <param name="pos">position of the sound </param>
        /// <param name="orient">
        ///     orientation of the sound, this is irrelevant if it//s an
        ///     omnidirectional sound source
        /// </param>
        /// <param name="vel">velocity of the sound</param>
        public Channel3DPosition(BASS_3DVECTOR pos, BASS_3DVECTOR orient, BASS_3DVECTOR vel)
        {
            _pos = pos;
            _orient = orient;
            _vel = vel;
        }

        public BASS_3DVECTOR Orient
        {
            get { return _orient; }
            set { _orient = value; }
        }

        public BASS_3DVECTOR Pos
        {
            get { return _pos; }
            set { _pos = value; }
        }

        public BASS_3DVECTOR Vel
        {
            get { return _vel; }
            set { _vel = value; }
        }

        public override string ToString()
        {
            return String.Format("pos:{0}:orient:{1}:vel:{2}",
                _pos,
                _orient,
                _vel);
        }
    }

    /// <summary>
    ///     Use with setting and getting 3DPosition
    /// </summary>
    public class BASS3DPosition
    {
        private BASS_3DVECTOR _front;
        private BASS_3DVECTOR _pos;
        private BASS_3DVECTOR _top;
        private BASS_3DVECTOR _vel;

        /// <summary>
        ///     Use with setting and getting 3DPosition
        /// </summary>
        /// <param name="pos">Position of the listener </param>
        /// <param name="vel">listener's velocity, used to calculate doppler effect</param>
        /// <param name="top">Direction that listener's front is pointing</param>
        /// <param name="front">Direction that listener's top is pointing </param>
        public BASS3DPosition([NotNull] BASS_3DVECTOR pos, [NotNull] BASS_3DVECTOR vel, [NotNull] BASS_3DVECTOR top, [NotNull] BASS_3DVECTOR front)
        {
            _pos = pos;
            _vel = vel;
            _top = top;
            _front = front;
        }

        public BASS_3DVECTOR Front
        {
            get { return _front; }
            set { _front = value; }
        }

        public BASS_3DVECTOR Pos
        {
            get { return _pos; }
            set { _pos = value; }
        }

        public BASS_3DVECTOR Top
        {
            get { return _top; }
            set { _top = value; }
        }

        public BASS_3DVECTOR Vel
        {
            get { return _vel; }
            set { _vel = value; }
        }

        public override string ToString()
        {
            return String.Format("pos:{0}:vel:{1}:top:{2}:front:{3}",
                _pos, _vel, _top, _front);
        }
    }

    /// <summary>
    ///     Creates a structure to be used with getting and setting 3DFactors
    /// </summary>
    public class BASS3DFactors
    {
        private float _distf;
        private float _doppf;
        private float _rollf;

        /// <summary>
        ///     Creates a structure to be used with getting and setting 3DFactors
        /// </summary>
        /// <param name="distf">
        ///     Distance factor (0.0-10.0, 1.0=use meters, 0.3=use feet, smaller than 0.0=leave current)
        ///     By default BASSEngine measures distances in meters, you can change this
        ///     setting if you are using a different unit of measurement.
        /// </param>
        /// <param name="rollf">
        ///     Rolloff factor, how fast the sound quietens with distance
        ///     (0.0=no rolloff, 1.0=real world, 2.0=2x real... 10.0=max, smaller than 0.0=leave current)
        /// </param>
        /// <param name="doppf">
        ///     Doppler factor (0.0=no doppler, 1.0=real world, 2.0=2x real... 10.0=max, smaller than 0.0=leave current)
        ///     The doppler effect is the way a sound appears to change frequency when it is
        ///     moving towards or away from you. The listener and sound velocity settings are
        ///     used to calculate this effect, this "doppf" value can be used to lessen or
        ///     exaggerate the effect.
        /// </param>
        public BASS3DFactors(float distf, float rollf, float doppf)
        {
            _distf = distf;
            _rollf = rollf;
            _doppf = doppf;
        }

        public float Distf
        {
            get { return _distf; }
            set { _distf = value; }
        }

        public float Doppf
        {
            get { return _doppf; }
            set { _doppf = value; }
        }

        public float Rollf
        {
            get { return _rollf; }
            set { _rollf = value; }
        }

        public override string ToString()
        {
            return String.Format("Dist: {0} Roll: {1} Dopp: {2}",
                _distf, _rollf, _doppf);
        }
    }

    /// <summary>
    ///     Used with setting / getting EAXParameters
    /// </summary>
    public class BASSEAXParameters
    {
        private float _damp;
        private float _decay;
        private EAXEnvironment _env;
        private float _vol;

        /// <summary>
        ///     Used with setting / getting EAXParameters
        /// </summary>
        /// <param name="env">Reverb environment</param>
        /// <param name="vol">Reverb volume (0.0=off, 1.0=max, smaller than 0.0=leave current)</param>
        /// <param name="decay">Time in seconds it takes the reverb to diminish by 60dB (0.1-20.0, smaller than 0.0=leave current)</param>
        /// <param name="damp">
        ///     The damping, high or low frequencies decay faster (0.0=high decays quickest,
        ///     1.0=low/high decay equally, 2.0=low decays quickest, smaller than 0.0=leave current)
        /// </param>
        public BASSEAXParameters(EAXEnvironment env, float vol, float decay, float damp)
        {
            _env = env;
            _vol = vol;
            _decay = decay;
            _damp = damp;
        }

        public float Damp
        {
            get { return _damp; }
            set { _damp = value; }
        }

        public float Decay
        {
            get { return _decay; }
            set { _decay = value; }
        }

        public EAXEnvironment Env
        {
            get { return _env; }
            set { _env = value; }
        }

        public float Vol
        {
            get { return _vol; }
            set { _vol = value; }
        }

        public override string ToString()
        {
            return String.Format("Env: {0} Vol: {1} Decay: {2} Damp: {3}",
                ((EAXEnvironment) _env), _vol, _decay, _damp);
        }
    }

    #region API Structs

    [StructLayout(LayoutKind.Sequential)]
    public struct BASSInfo
    {
        private int size; //size of this struct (set this before calling the function)
        private readonly int flags; //device capabilities (DSCAPS_xxx flags)
        private readonly int hwsize; //size of total device hardware memory
        private readonly int hwfree; //size of free device hardware memory
        private readonly int freesam; //number of free sample slots in the hardware
        private readonly int free3d; //number of free 3D sample slots in the hardware
        private readonly int minrate; //min sample rate supported by the hardware
        private readonly int maxrate; //max sample rate supported by the hardware
        private readonly int eax; //device supports EAX? (always BASSFALSE if BASS_DEVICE_3D was not used) realy bool
        private readonly int a3d; //unused
        private readonly int dsver; //DirectSound version (use to check for DX5/7 functions)
        private readonly int latency; //delay (in ms) before start of playback (requires BASS_DEVICE_LATENCY)

//		public BASSInfo(bool dumb)
//		{
//			this.size = Marshal.SizeOf(typeof(BASSInfo));
//		}

        public int MaxSecondarySampleRate
        {
            get { return maxrate; }
        }

        public int MinSecondarySampleRate
        {
            get { return minrate; }
        }

        public int TotalHardwareMemBytes
        {
            get { return hwsize; }
        }

        public int FreeHardware3DChannels
        {
            get { return free3d; }
        }

        public int FreeHardwareMixingAllChannels
        {
            get { return freesam; }
        }

        public int FreeHardwareMemBytes
        {
            get { return hwfree; }
        }

        public int Size
        {
            set { size = value; }
        }

        #region DeviceCaps

        public string DeviceCapabilities
        {
            get { return Helper.PrintFlags((DeviceCaps) flags); }
        }

        public bool Certified
        {
            get { return Helper.Int2Bool((flags >> 6 & 1)); }
        }

        public bool ContinuousRate
        {
            get { return Helper.Int2Bool((flags >> 4 & 1)); }
        }

        public bool EmulateDriver
        {
            get { return Helper.Int2Bool((flags >> 5 & 1)); }
        }

        public bool Primary16Bit
        {
            get { return Helper.Int2Bool((flags >> 3 & 1)); }
        }

        public bool Primary8Bit
        {
            get { return Helper.Int2Bool((flags >> 2 & 1)); }
        }

        public bool PrimaryMono
        {
            get { return Helper.Int2Bool((flags & 1)); }
        }

        public bool PrimaryStereo
        {
            get { return Helper.Int2Bool((flags >> 1 & 1)); }
        }

        public bool Secondary16Bit
        {
            get { return Helper.Int2Bool((flags >> 11 & 1)); }
        }

        public bool Secondary8Bit
        {
            get { return Helper.Int2Bool((flags >> 10 & 1)); }
        }

        public bool SecondaryMono
        {
            get { return Helper.Int2Bool((flags >> 8 & 1)); }
        }

        public bool SecondaryStereo
        {
            get { return Helper.Int2Bool((flags >> 9 & 1)); }
        }

        public bool EAXSupport
        {
            get { return Helper.Int2Bool(eax); }
        }

        public int DXVersion
        {
            get { return dsver; }
        }

        public int Latency
        {
            get { return latency; }
        }

        #endregion
    }

    //*************************
    //* Sample info structure *
    //*************************
    [StructLayout(LayoutKind.Sequential)]
    public struct SampleInfo
    {
        public int freq; //default playback rate
        public int volume; //default volume (0-100)
        public int pan; //default pan (-100=left, 0=middle, 100=right)
        public int flags; //BASS_SAMPLE_xxx flags
        public int Length; //length (in samples, not bytes)
        public int max; // maximum simultaneous playbacks
        // The following are the sample; //s default 3D attributes (if the sample
        // is 3D, BASS_SAMPLE_3D is in flags) see BASS_ChannelSet3DAttributes
        public int mode3d; //BASS_3DMODE_xxx mode
        public float mindist; //minimum distance
        public float MAXDIST; //maximum distance
        public int iangle; //angle of inside projection cone
        public int oangle; //angle of outside projection cone
        public int outvol; //delta-volume outside the projection cone
        // The following are the defaults used if the sample uses the DirectX 7
        // voice allocation/management features.
        public int vam; //voice allocation/management flags (BASS_VAM_xxx)
        public int priority; //priority (0=lowest, 0xffffffff=highest)

        public string Flags
        {
            get { return Helper.PrintFlags((DeviceSetupFlags) flags); }
        }
    }

    public struct RecordInfo
    {
        public int flags; // device capabilities (DSCCAPS_xxx flags)
        public int formats; // supported standard formats (WAVE_FORMAT_xxx flags)
        public int inputs; // number of inputs
        public int singlein; // BASSTRUE = only 1 input can be set at a time
        public int size; // size of this struct (set this before calling the function)
    }

    #endregion
}