using System;
using Tauron.Application.BassLib.Encoder;
using Tauron.Application.RadioStreamer.Contracts.Data;
using Tauron.Application.RadioStreamer.Contracts.Player.Recording;
using Tauron.JetBrains.Annotations;
using Un4seen.Bass.Misc;

namespace Tauron.Application.RadioStreamer.Views.Helper.Encoder
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
                .CreateProperty(() => PsYnsSafeJoint, (encoder, b) => encoder.PsYnsSafeJoint = b):
        }

        [NotNull]
        public string CustomOptions { get; set; }

        [NotNull]
        public string PresetName
        {
            get { return GetProperty(s => s, "PresentName", string.Empty); }
            set { Profile.SetProperty(value, "PresentName"); }
        }

        public EncoderLAME.LAMEQuality Quality
        {
            get { return GetProperty(s => s.ParseEnum<EncoderLAME.LAMEQuality>(), "Quality", EncoderLAME.LAMEQuality.Quality); }
            set { _profile.SetProperty(value, "Quality"); }
        }

        public EncoderLAME.LAMEReplayGain ReplayGain
        {
            get
            {
                return GetProperty(s => s.ParseEnum<EncoderLAME.LAMEReplayGain>(), "ReplayGain",
                    EncoderLAME.LAMEReplayGain.None);
            }
            set { _profile.SetProperty(value, "ReplayGain"); }
        }

        public BaseEncoder.BITRATE Bitrate
        {
            get
            {
                return GetProperty(s => s.ParseEnum<BaseEncoder.BITRATE>(), "Bitrate", BaseEncoder.BITRATE.kbps_128);
            }
            set { _profile.SetProperty(value, "Bitrate"); }
        }

        public EncoderLAME.LAMEVBRQuality VbrQuality
        {
            get
            {
                return GetProperty(s => s.ParseEnum<EncoderLAME.LAMEVBRQuality>(), "VbrQuality",
                    EncoderLAME.LAMEVBRQuality.VBR_Q4);
            }
            set { _profile.SetProperty(value, "VbrQuality"); }
        }

        public int VbrMaxBitrate
        {
            get { return GetProperty(int.Parse, "VbrMaxBitrate", 320); }
            set { _profile.SetProperty(value, "VbrMaxBitrate"); }
        }

        public bool UseCustomOptionsOnly
        {
            get { return GetProperty(bool.Parse, "UseCustomOptionsOnly", false); }
            set { _profile.SetProperty(value, "UseCustomOptionsOnly"); }
        }

        public EncoderLAME.LAMEMode Mode
        {
            get { return GetProperty(s => s.ParseEnum<EncoderLAME.LAMEMode>(), "Mode", EncoderLAME.LAMEMode.Default); }
            set { _profile.SetProperty(value, "Mode"); }
        }

        public bool FreeFormat
        {
            get { return GetProperty(bool.Parse, "FreeFormat", false); }
            set { _profile.SetProperty(value, "FreeFormat"); }
        }

        public int TargetSampleRate
        {
            get { return GetProperty(int.Parse, "TargetSampleRate", 0); }
            set { _profile.SetProperty(value, "TargetSampleRate"); }
        }

        public bool EnforceCbr
        {
            get { return GetProperty(bool.Parse, "EnforceCbr", false); }
            set { _profile.SetProperty(value, "EnforceCbr"); }
        }

        public int AbrBitrate
        {
            get { return GetProperty(int.Parse, "AbrBitrate", 0); }
            set { _profile.SetProperty(value, "AbrBitrate"); }
        }

        public bool UseVbr
        {
            get { return Lame.LAME_UseVBR; }
            set { Lame.LAME_UseVBR = value; }
        }
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

    }
}
