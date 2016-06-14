using Tauron.Application.BassLib.Encoder;
using Tauron.Application.RadioStreamer.Contracts.Player.Recording;
using Tauron.JetBrains.Annotations;
using Un4seen.Bass.Misc;

namespace Tauron.Application.RadioStreamer.Contracts.Data.Encoder
{
    public sealed class LameEncoderProfile : AudioEncoderFactoryBase
    {
        public const string LameId = "LameEncoder";

        public LameEncoderProfile([CanBeNull] CommonProfile profile) : base(LameId, profile)
        {
            CreateMapping(ch => new LameEncoder(ch))
                .CreateProperty(() => AbrBitrate, (encoder, i) => encoder.AbrBitrate = i)
                .CreatePropertyEnum(() => AthControl, (encoder, lameath) => encoder.AthControl = lameath)
                .CreatePropertyEnum(() => Bitrate, (encoder, bitrate) => encoder.Bitrate = bitrate,
                    BaseEncoder.BITRATE.kbps_128)
                .CreateProperty(() => Copyright, (encoder, b) => encoder.Copyright = b)
                .CreateProperty(() => DisableAllFilters, (encoder, b) => encoder.DisableAllFilters = b)
                .CreateProperty(() => DisableBitReservoir, (encoder, b) => encoder.DisableBitReservoir = b)
                .CreateProperty(() => CustomOptions, (encoder, s) => encoder.CustomOptions = s, string.Empty)
                .CreateProperty(() => EnforceCbr, (encoder, b) => encoder.EnforceCbr = b)
                .CreateProperty(() => EnforceIso, (encoder, b) => encoder.EnforceIso = b)
                .CreateProperty(() => FreeFormat, (encoder, b) => encoder.FreeFormat = b)
                .CreateProperty(() => HighPassFreq, (encoder, i) => encoder.HighPassFreqWidth = i)
                .CreateProperty(() => HighPassFreqWidth, (encoder, i) => encoder.HighPassFreqWidth = i)
                .CreateProperty(() => LimitVbr, (encoder, b) => encoder.LimitVbr = b)
                .CreateProperty(() => LowPassFreq, (encoder, i) => encoder.LowPassFreq = i)
                .CreateProperty(() => LowPassFreqWidth, (encoder, i) => encoder.LowPassFreqWidth = i)
                .CreatePropertyEnum(() => Mode, (encoder, mode) => encoder.Mode = mode)
                .CreatePropertyEnum(() => NoAsm, (encoder, lamenoasm) => encoder.NoAsm = lamenoasm)
                .CreateProperty(() => NonOriginal, (encoder, b) => encoder.NonOriginal = b)
                .CreateProperty(() => PresetName, (encoder, s) => encoder.PresetName = s, string.Empty)
                .CreateProperty(() => Protect, (encoder, b) => encoder.Protect = b)
                .CreateProperty(() => PsYallShortBlocks, (encoder, b) => encoder.PsYallShortBlocks = b)
                .CreateProperty(() => PsYnoShortBlocks, (encoder, b) => encoder.PsYnoShortBlocks = b)
                .CreateProperty(() => PsYnoTemp, (encoder, b) => encoder.PsYnoTemp = b)
                .CreateProperty(() => PsYnsSafeJoint, (encoder, b) => encoder.PsYnsSafeJoint = b)
                .CreateProperty(() => PsYuseShortBlocks, (encoder, b) => encoder.PsYuseShortBlocks = b)
                .CreatePropertyEnum(() => Quality, (encoder, quality) => encoder.Quality = quality,
                    EncoderLAME.LAMEQuality.Quality)
                .CreatePropertyEnum(() => ReplayGain, (encoder, gain) => encoder.ReplayGain = gain)
                .CreateProperty(() => Scale, (encoder, f) => encoder.Scale = f, 1f)
                .CreateProperty(() => TargetSampleRate, (encoder, i) => encoder.TargetSampleRate = i)
                .CreateProperty(() => UseCustomOptionsOnly, (encoder, b) => encoder.UseCustomOptionsOnly = b)
                .CreateProperty(() => UseVbr, (encoder, b) => encoder.UseVbr = b)
                .CreateProperty(() => VbrDisableTag, (encoder, b) => encoder.VbrDisableTag = b)
                .CreateProperty(() => VbrEnforceMinBitrate, (encoder, b) => encoder.VbrEnforceMinBitrate = b)
                .CreateProperty(() => VbrMaxBitrate, (encoder, i) => encoder.VbrMaxBitrate = i, 320)
                .CreatePropertyEnum(() => VbrQuality, (encoder, quality) => encoder.VbrQuality = quality,
                    EncoderLAME.LAMEVBRQuality.VBR_Q4)
                .Initialize();
        }

        public float Scale
        {
            get { return GetValue<float>(); }
            set { SetValue(value); }
        }

        [NotNull]
        public string CustomOptions
        {
            get { return GetValue<string>(); } 
            set { SetValue(value); }
        }

        [NotNull]
        public string PresetName
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public EncoderLAME.LAMEQuality Quality
        {
            get { return GetValue<EncoderLAME.LAMEQuality>(); }
            set { SetValue(value); }
        }

        public EncoderLAME.LAMEReplayGain ReplayGain
        {
            get { return GetValue<EncoderLAME.LAMEReplayGain>(); }
            set { SetValue(value); }
        }

        public BaseEncoder.BITRATE Bitrate
        {
            get { return GetValue<BaseEncoder.BITRATE>(); }
            set { SetValue(value); }
        }

        public EncoderLAME.LAMEVBRQuality VbrQuality
        {
            get { return GetValue<EncoderLAME.LAMEVBRQuality>(); }
            set { SetValue(value); }
        }

        public int VbrMaxBitrate
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }

        public bool UseCustomOptionsOnly
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        public EncoderLAME.LAMEMode Mode
        {
            get { return GetValue<EncoderLAME.LAMEMode>(); }
            set { SetValue(value); }
        }

        public bool FreeFormat
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        public int TargetSampleRate
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }

        public bool EnforceCbr
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        public int AbrBitrate
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }

        public bool UseVbr
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        public bool LimitVbr
        {
            get { return GetValue<bool>(); } 
            set { SetValue(value); }
        }

        public bool VbrDisableTag
        {
            get { return GetValue<bool>(); } 
            set { SetValue(value); }
        }

        public bool VbrEnforceMinBitrate
        {
            get { return GetValue<bool>(); } 
            set { SetValue(value); }
        }

        public bool Copyright
        {
            get { return GetValue<bool>(); } 
            set { SetValue(value); }
        }

        public bool NonOriginal
        {
            get { return GetValue<bool>(); } 
            set { SetValue(value); }
        }

        public bool Protect
        {
            get { return GetValue<bool>(); } 
            set { SetValue(value); }
        }

        public bool DisableBitReservoir
        {
            get { return GetValue<bool>(); } 
            set { SetValue(value); }
        }

        public bool EnforceIso
        {
            get { return GetValue<bool>(); } 
            set { SetValue(value); }
        }

        public bool DisableAllFilters
        {
            get { return GetValue<bool>(); } 
            set { SetValue(value); }
        }

        public bool PsYuseShortBlocks
        {
            get { return GetValue<bool>(); } 
            set { SetValue(value); }
        }

        public bool PsYnoShortBlocks
        {
            get { return GetValue<bool>(); } 
            set { SetValue(value); }
        }

        public bool PsYallShortBlocks
        {
            get { return GetValue<bool>(); } 
            set { SetValue(value); }
        }

        public bool PsYnoTemp
        {
            get { return GetValue<bool>(); } 
            set { SetValue(value); }
        }

        public bool PsYnsSafeJoint
        {
            get { return GetValue<bool>(); } 
            set { SetValue(value); }
        }

        public EncoderLAME.LAMENOASM NoAsm
        {
            get { return GetValue<EncoderLAME.LAMENOASM>(); } 
            set { SetValue(value); }
        }

        public int HighPassFreq
        {
            get { return GetValue<int>(); } 
            set { SetValue(value); }
        }

        public int HighPassFreqWidth
        {
            get { return GetValue<int>(); } 
            set { SetValue(value); }
        }

        public int LowPassFreq
        {
            get { return GetValue<int>(); } 
            set { SetValue(value); }
        }

        public int LowPassFreqWidth
        {
            get { return GetValue<int>(); } 
            set { SetValue(value); }
        }

        public EncoderLAME.LAMEATH AthControl
        {
            get { return GetValue<EncoderLAME.LAMEATH>(); } 
            set { SetValue(value); }
        }

    }
}
