using Tauron.JetBrains.Annotations;
using Un4seen.Bass.Misc;

namespace Tauron.Application.BassLib.Encoder
{
    [PublicAPI]
    public sealed class LameEncoder : AudioEncoder
    {
        public LameEncoder([NotNull] Channel channel) 
            : base(new EncoderLAME(channel.Handle), channel)
        {
        }

        [NotNull]
        private EncoderLAME Lame { get { return (EncoderLAME) BassEncoder; } }

        [NotNull]
        public string CustomOptions { get { return Lame.LAME_CustomOptions; } set { Lame.LAME_CustomOptions = value; }
        }
        public float Scale { get { return Lame.LAME_Scale; } set { Lame.LAME_Scale = value; } }
        [NotNull]
        public string PresetName { get { return Lame.LAME_PresetName; } set { Lame.LAME_PresetName = value; } }
        public EncoderLAME.LAMEQuality Quality { get { return Lame.LAME_Quality; } set { Lame.LAME_Quality = value; } }
        public EncoderLAME.LAMEReplayGain ReplayGain { get { return Lame.LAME_ReplayGain; } set { Lame.LAME_ReplayGain = value; } }
        public BaseEncoder.BITRATE Bitrate { get { return (BaseEncoder.BITRATE)Lame.LAME_Bitrate; } set { Lame.LAME_Bitrate = (int)value; } }
        public EncoderLAME.LAMEVBRQuality VbrQuality { get { return Lame.LAME_VBRQuality; } set { Lame.LAME_VBRQuality = value; } }
        public int VbrMaxBitrate { get { return Lame.LAME_VBRMaxBitrate; } set { Lame.LAME_VBRMaxBitrate = value; } }
        public bool UseCustomOptionsOnly { get { return Lame.LAME_UseCustomOptionsOnly; } set { Lame.LAME_UseCustomOptionsOnly = value; } }
        public EncoderLAME.LAMEMode Mode { get { return Lame.LAME_Mode; } set { Lame.LAME_Mode = value; } }
        public bool FreeFormat { get { return Lame.LAME_FreeFormat; } set { Lame.LAME_FreeFormat = value; } }
        public int TargetSampleRate { get { return Lame.LAME_TargetSampleRate; } set { Lame.LAME_TargetSampleRate = value; } }
        public bool EnforceCbr { get { return Lame.LAME_EnforceCBR; } set { Lame.LAME_EnforceCBR = value; } }
        public int AbrBitrate { get { return Lame.LAME_ABRBitrate; } set { Lame.LAME_ABRBitrate = value; } }
        public bool UseVbr { get { return Lame.LAME_UseVBR; } set { Lame.LAME_UseVBR = value; } }
        public bool LimitVbr { get { return Lame.LAME_LimitVBR; } set { Lame.LAME_LimitVBR = value; } }
        public bool VbrDisableTag { get { return Lame.LAME_VBRDisableTag; } set { Lame.LAME_VBRDisableTag = value; } }
        public bool VbrEnforceMinBitrate { get { return Lame.LAME_VBREnforceMinBitrate; } set { Lame.LAME_VBREnforceMinBitrate = value; } }
        public bool Copyright { get { return Lame.LAME_Copyright; } set { Lame.LAME_Copyright = value; } }
        public bool NonOriginal { get { return Lame.LAME_NonOriginal; } set { Lame.LAME_NonOriginal = value; } }
        public bool Protect { get { return Lame.LAME_Protect; } set { Lame.LAME_Protect = value; } }
        public bool DisableBitReservoir { get { return Lame.LAME_DisableBitReservoir; } set { Lame.LAME_DisableBitReservoir = value; } }
        public bool EnforceIso { get { return Lame.LAME_EnforceISO; } set { Lame.LAME_EnforceISO = value; } }
        public bool DisableAllFilters { get { return Lame.LAME_DisableAllFilters; } set { Lame.LAME_DisableAllFilters = value; } }
        public bool PsYuseShortBlocks { get { return Lame.LAME_PSYuseShortBlocks; } set { Lame.LAME_PSYuseShortBlocks = value; } }
        public bool PsYnoShortBlocks { get { return Lame.LAME_PSYnoShortBlocks; } set { Lame.LAME_PSYnoShortBlocks = value; } }
        public bool PsYallShortBlocks { get { return Lame.LAME_PSYallShortBlocks; } set { Lame.LAME_PSYallShortBlocks = value; } }
        public bool PsYnoTemp { get { return Lame.LAME_PSYnoTemp; } set { Lame.LAME_PSYnoTemp = value; } }
        public bool PsYnsSafeJoint { get { return Lame.LAME_PSYnsSafeJoint; } set { Lame.LAME_PSYnsSafeJoint = value; } }
        public EncoderLAME.LAMENOASM NoAsm { get { return Lame.LAME_NoASM; } set { Lame.LAME_NoASM = value; } }
        public int HighPassFreq { get { return Lame.LAME_HighPassFreq; } set { Lame.LAME_HighPassFreq = value; } }
        public int HighPassFreqWidth { get { return Lame.LAME_HighPassFreqWidth; } set { Lame.LAME_HighPassFreqWidth = value; } }
        public int LowPassFreq { get { return Lame.LAME_LowPassFreq; } set { Lame.LAME_LowPassFreq = value; } }
        public int LowPassFreqWidth { get { return Lame.LAME_LowPassFreqWidth; } set { Lame.LAME_LowPassFreqWidth = value; } }
        public EncoderLAME.LAMEATH AthControl { get { return Lame.LAME_ATHControl; } set { Lame.LAME_ATHControl = value; } }
        public bool EncoderExists { get { return Lame.EncoderExists; } }
    }
}