#define TESTING

using System;
using Tauron.JetBrains.Annotations;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Fx;

namespace Tauron.Application.RadioStreamer.Player.CoreOld
{
	/// <summary>
	/// Summary description for FX.
	/// </summary>
	[PublicAPI]
	public class FX
	{
	    public int Priority { get; set; }
	    private readonly int _handle;
		private readonly BASSFXType _type;

        internal FX(int handle, BASSFXType type, int priority)
		{
            Priority = priority;
            _handle = handle;
			_type = type;
		}

		/// <summary>
		/// Gets the _type of effect that has been applied
		/// </summary>
		public BASSFXType FXType 
		{
			get {return _type;}
		}

		internal int Handle 
		{
			get { return _handle;}
		}



		
        //// Set the parameters of a DX8 effect.
        //// _handle : FX _handle
        //// par    : Pointer to the parameter structure
        //[DllImport("bass.dll", EntryPoint = "BASS_FXSetParameters")]
        //static extern int _SetParameters(IntPtr handle, IntPtr fxparam);//return bool

        //// Retrieve the parameters of a DX8 effect.
        //// _handle : FX _handle
        //// par    : Pointer to the parameter structure
        //[DllImport("bass.dll", EntryPoint = "BASS_FXGetParameters")]
        //static extern int _GetParameters(IntPtr handle, IntPtr fxparam); //return bool

	    [CanBeNull]
	    public object Parameters 
		{
			get 
			{
				object param;

			    switch (FXType)
			    {
                    case BASSFXType.BASS_FX_DX8_CHORUS:
                        param = new BASS_DX8_CHORUS();
                        break;
                    case BASSFXType.BASS_FX_DX8_COMPRESSOR:
                        param = new BASS_DX8_COMPRESSOR();
                        break;
                    case BASSFXType.BASS_FX_DX8_DISTORTION:
                        param = new BASS_DX8_DISTORTION();
                        break;
                    case BASSFXType.BASS_FX_DX8_ECHO:
                        param = new BASS_DX8_ECHO();
                        break;
                    case BASSFXType.BASS_FX_DX8_FLANGER:
                        param = new BASS_DX8_FLANGER();
                        break;
                    case BASSFXType.BASS_FX_DX8_GARGLE:
                        param = new BASS_DX8_GARGLE();
                        break;
                    case BASSFXType.BASS_FX_DX8_I3DL2REVERB:
                        param = new BASS_DX8_I3DL2REVERB();
                        break;
                    case BASSFXType.BASS_FX_DX8_PARAMEQ:
                        param = new BASS_DX8_PARAMEQ();
                        break;
                    case BASSFXType.BASS_FX_DX8_REVERB:
                        param = new BASS_DX8_REVERB();
                        break;
			        case BASSFXType.BASS_FX_BFX_ROTATE:
			            param = new BASS_BFX_ROTATE();
			            break;
			        case BASSFXType.BASS_FX_BFX_VOLUME:
			            param = new BASS_BFX_VOLUME();
			            break;
			        case BASSFXType.BASS_FX_BFX_PEAKEQ:
			            param = new BASS_BFX_PEAKEQ();
			            break;
			        case BASSFXType.BASS_FX_BFX_MIX:
			            param = new BASS_BFX_MIX();
			            break;
			        case BASSFXType.BASS_FX_BFX_DAMP:
			            param = new BASS_BFX_DAMP();
			            break;
			        case BASSFXType.BASS_FX_BFX_AUTOWAH:
			            param = new BASS_BFX_AUTOWAH();
			            break;
			        case BASSFXType.BASS_FX_BFX_PHASER:
			            param = new BASS_BFX_PHASER();
			            break;
			        case BASSFXType.BASS_FX_BFX_CHORUS:
			            param = new BASS_BFX_CHORUS();
			            break;
			        case BASSFXType.BASS_FX_BFX_DISTORTION:
			            param = new BASS_BFX_DISTORTION();
			            break;
			        case BASSFXType.BASS_FX_BFX_COMPRESSOR2:
			            param = new BASS_BFX_COMPRESSOR2();
			            break;
			        case BASSFXType.BASS_FX_BFX_VOLUME_ENV:
			            param = new BASS_BFX_VOLUME_ENV();
			            break;
			        case BASSFXType.BASS_FX_BFX_BQF:
			            param = new BASS_BFX_BQF();
			            break;
			        case BASSFXType.BASS_FX_BFX_ECHO4:
			            param = new BASS_BFX_ECHO4();
			            break;
			        default:
			            throw new NotSupportedException();
			    }

			    if(!Bass.BASS_FXGetParameters(_handle, param))
                    throw new BASSException();

                //IntPtr alloc = Marshal.AllocHGlobal(Marshal.SizeOf(param));

                //Marshal.StructureToPtr(param, alloc, true);

                //if (_GetParameters( this._handle, alloc) == 0)
                //    throw new BASSException();

                //Marshal.PtrToStructure(alloc, param); // ????
                //Marshal.FreeHGlobal(alloc);
				return param;
			}
	        set
	        {
	            //IntPtr paramptr = Marshal.AllocHGlobal(Marshal.SizeOf(value));
	            //Marshal.StructureToPtr(value, paramptr, true);
	            //if (_SetParameters( this._handle, paramptr) == 0)
	            //{
	            //    Marshal.FreeHGlobal(paramptr);
	            //    throw new BASSException();
	            //} else Marshal.FreeHGlobal(paramptr);

	            if (!Bass.BASS_FXSetParameters(_handle, value)) throw new BASSException();
	        }
		}


	}

    //[StructLayout(LayoutKind.Sequential)]
    //public class FXCHORUS
    //{ // DSFXChorus
    //    public float fWetDryMix ;
    //    public float fDepth ;
    //    public float fFeedback ;
    //    public float fFrequency ;
    //    public uint lWaveform ; // 0=triangle, 1=sine
    //    public float fDelay ;
    //    public uint lPhase ; // BASS_FX_PHASE_xxx
    //}

    //[StructLayout(LayoutKind.Sequential)]
    //public class FXCOMPRESSOR 
    //{ // DSFXCompressor
    //    public float fGain ;
    //    public float fAttack ;
    //    public float fRelease ;
    //    public float fThreshold ;
    //    public float fRatio ;
    //    public float fPredelay ;
    //}

    //[StructLayout(LayoutKind.Sequential)]
    //public class FXDISTORTION
    //{ // DSFXDistortion
    //    public float fGain ;
    //    public float fEdge ;
    //    public float fPostEQCenterFrequency ;
    //    public float fPostEQBandwidth ;
    //    public float fPreLowpassCutoff ;
    //}

    //[StructLayout(LayoutKind.Sequential)]
    //public class FXECHO
    //{ // DSFXEcho
    //    public float fWetDryMix ;
    //    public float fFeedback ;
    //    public float fLeftDelay ;
    //    public float fRightDelay ;
    //    public int lPanDelay;
    //}

    //[StructLayout(LayoutKind.Sequential)]
    //public class FXFLANGER
    //{ // DSFXFlanger
    //    public float fWetDryMix ;
    //    public float fDepth ;
    //    public float fFeedback ;
    //    public float fFrequency ;
    //    public uint lWaveform ; // 0=triangle, 1=sine
    //    public float fDelay ;
    //    public uint lPhase ; // BASS_FX_PHASE_xxx
    //}

    //[StructLayout(LayoutKind.Sequential)]
    //public class FXGARGLE
    //{ // DSFXGargle
    //    public uint dwRateHz ; // Rate of modulation in hz
    //    public uint dwWaveShape ; // 0=triangle, 1=square
    //}

    //[StructLayout(LayoutKind.Sequential)]
    //public class FXI3DL2REVERB
    //{ // DSFXI3DL2Reverb
    //    public int lRoom ; // [-10000, 0]      default: -1000 mB
    //    public int lRoomHF ; // [-10000, 0]      default: 0 mB
    //    public float flRoomRolloffFactor ; // [0.0, 10.0]      default: 0.0
    //    public float flDecayTime ; // [0.1, 20.0]      default: 1.49s
    //    public float flDecayHFRatio ; // [0.1, 2.0]       default: 0.83
    //    public int lReflections ; // [-10000, 1000]   default: -2602 mB
    //    public float flReflectionsDelay ; // [0.0, 0.3]       default: 0.007 s
    //    public int lReverb ; // [-10000, 2000]   default: 200 mB
    //    public float flReverbDelay ; // [0.0, 0.1]       default: 0.011 s
    //    public float flDiffusion ; // [0.0, 100.0]     default: 100.0 %
    //    public float flDensity ; // [0.0, 100.0]     default: 100.0 %
    //    public float flHFReference ; // [20.0, 20000.0]  default: 5000.0 Hz
    //}

    //[StructLayout(LayoutKind.Sequential)]
    //public class FXPARAMEQ
    //{ // DSFXParamEq
    //    public float fCenter ;
    //    public float fBandwidth ;
    //    public float fGain ;
    //}

    //[StructLayout(LayoutKind.Sequential)]
    //public class FXREVERB
    //{ // DSFXWavesReverb
    //    public float fInGain ; // [-96.0,0.0]            default: 0.0 dB
    //    public float fReverbMix ; // [-96.0,0.0]            default: 0.0 db
    //    public float fReverbTime ; // [0.001,3000.0]         default: 1000.0 ms
    //    public float fHighFreqRTRatio ; // [0.001,0.999]          default: 0.001
    //}

}
