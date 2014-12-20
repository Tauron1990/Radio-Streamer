using System;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Tauron.Application.Ioc;
using Tauron.Application.Models;
using Tauron.Application.RadioStreamer.Contracts;
using Tauron.Application.RadioStreamer.Contracts.Data.Encoder;
using Tauron.Application.RadioStreamer.Contracts.UI;
using Tauron.Application.RadioStreamer.Resources;
using Tauron.JetBrains.Annotations;
using Un4seen.Bass.Misc;

namespace Tauron.Application.RadioStreamer.Views.EncodingOptions
{
    public class SimpleComboItem
    {
        [NotNull]
        public string Name { get; private set; }

        [NotNull]
        private string DisplayName { get; set; }

        public SimpleComboItem([NotNull] string name, [NotNull] string displayName)
        {
            if (name == null) throw new ArgumentNullException("name");
            if (displayName == null) throw new ArgumentNullException("displayName");
            
            Name = name;
            DisplayName = displayName;
        }

        public override string ToString()
        {
            return DisplayName;
        }
    }

    public class ComboItem<TEnum>
    {
        [NotNull]
        public TEnum Value { get; private set; }

        [NotNull]
        private string DisplayName { get; set; }

        public ComboItem([NotNull] TEnum value, [NotNull] string displayName)
        {
            if (displayName == null) throw new ArgumentNullException("displayName");

            Value = value;
            DisplayName = displayName;
        }

        public ComboItem(TEnum value)
            : this(value, value.ToString())
        {
        
        }

        public override string ToString()
        {
            return DisplayName;
        }
    }

    public enum ShortBlockUsage
    {
        None,
        UseShortBlock,
        UseOnlyShortBlocks,
        UseNoShortBlocks,
    }

    [ExportViewModel(LameEncoderProfile.LameId)]
    [NotShared]
    public sealed class LameEncodingEditorViewModel : ViewModelBase
    {
        public abstract class HelperBase<TType> : ObservableObject
        {
            public event Action<TType> ValueChanedEvent;

            protected void OnValueChanedEvent(TType e)
            {
                Action<TType> handler = ValueChanedEvent;
                if (handler != null) handler(e);
            }

            private Action<TType> _internalSetter;

            protected readonly Action<TType> Setter;
            protected readonly Func<TType> Getter;

            protected HelperBase([CanBeNull] Expression<Func<object>> name)
            {
                if (name == null)
                {
                    Getter = () => default(TType);
                    Setter = OnValueChanedEvent;
                    return;
                }

                var firstlevelbody = name.Body;
                var expression = firstlevelbody as MemberExpression ??
                                 ((MemberExpression) ((UnaryExpression) firstlevelbody).Operand);
                var firstmemberexp = (MemberExpression) expression.Expression;
                var targetExpression = (ConstantExpression) firstmemberexp.Expression;
                var target = firstmemberexp.Member.GetInvokeMember<object>(targetExpression.Value);

                var info = expression.Member as PropertyInfo;
                if(info == null)
                    throw new InvalidOperationException("No Property Found");
                
                Getter = () => (TType) info.GetValue(target);
                _internalSetter = b => info.SetValue(target, b);

                Setter = value =>
                {
                    _internalSetter(value);
                    OnValueChanedEvent(value);
                };
            }
        }

        public sealed class BoolHelper : HelperBase<bool>
        {
            public BoolHelper([NotNull] Expression<Func<object>> name) : base(name)
            {
            }

            public bool? Value
            {
                get { return Getter(); }
                set
                {
                    Setter(value == true);
                    OnPropertyChanged();
                }
            }
        }

        public abstract class TextHelperBase<TType> : HelperBase<TType>
        {
            private string _text;

            [CanBeNull]
            public string Text
            {
                get { return _text; }
                set
                {
                    TType realValue;
                    if (TryParse(value, out realValue))
                    {
                        _text = value;
                        Setter(realValue);
                    }
                    OnPropertyChanged();
                }
            }

            protected abstract bool TryParse([CanBeNull] string text, out TType value);
            [CanBeNull]
            protected abstract string ToString(TType value);

            protected TextHelperBase([NotNull] Expression<Func<object>> name) 
                : base(name)
            {
// ReSharper disable once DoNotCallOverridableMethodsInConstructor
                _text = ToString(Getter());
            }
        }

        public sealed class ComboboxHelper<TEnum> : HelperBase<TEnum>
        {
            private ComboItem<TEnum> _currentItem;

            [NotNull]
            public ComboItem<TEnum>[] Items { get; private set; }

            [CanBeNull]
            public ComboItem<TEnum> CurrentItem
            {
                get { return _currentItem; }
                set
                {
                    _currentItem = value;
                    Setter(value == null ? default(TEnum) : value.Value);
                    OnPropertyChanged();
                }
            }

            public ComboboxHelper([CanBeNull] Expression<Func<object>> name, [NotNull] params ComboItem<TEnum>[] items) 
                : base(name)
            {
                Items = items;
                var cont = Getter();
                CurrentItem = Items.FirstOrDefault(i => i.Value.Equals(cont));
            }
        }
        public sealed class SimpleComboboxHelper : HelperBase<string>
        {
            private SimpleComboItem _currentItem;

            [NotNull]
            public SimpleComboItem[] Items { get; private set; }

            [CanBeNull]
            public SimpleComboItem CurrentItem
            {
                get { return _currentItem; }
                set
                {
                    _currentItem = value;
                    Setter(value == null ? null : value.Name);
                    OnPropertyChanged();
                }
            }

            public SimpleComboboxHelper([NotNull] Expression<Func<object>> name, [NotNull] params SimpleComboItem[] items)
                : base(name)
            {
                Items = items;
                var cont = Getter();
                CurrentItem = Items.FirstOrDefault(i => i.Name == cont);
            }
        }

        public sealed class FloatTextHelper : TextHelperBase<float>
        {
            public FloatTextHelper([NotNull] Expression<Func<object>> name) 
                : base(name)
            {
            }

            protected override bool TryParse(string text, out float value)
            {
                if (!string.IsNullOrEmpty(text))
                    return float.TryParse(text, NumberStyles.Number, CultureInfo.CurrentUICulture, out value);
                value = 0;
                return true;
            }

            protected override string ToString(float value)
            {
                return value.ToString(CultureInfo.CurrentUICulture);
            }
        }
        
        public sealed class IntTextHelper : TextHelperBase<int>
        {
            public IntTextHelper([NotNull] Expression<Func<object>> name) : base(name)
            {
            }

            protected override bool TryParse(string text, out int value)
            {
                return int.TryParse(text, out value);
            }

            protected override string ToString(int value)
            {
                return value.ToString(CultureInfo.CurrentUICulture);
            }
        }

        [InjectModel(AppConstants.CommonEncoderUI)]
        private CommonEncodingEditorModel _editorModel;
        
        [NotNull]
        public LameEncoderProfile EncoderProfile { get; set; }

        [NotNull]
        public BoolHelper UseCustomOptionsOnly { get; private set; }

        public bool IsCustomOptionsDisabled
        {
            get { return UseCustomOptionsOnly.Value == false; }
        }

        [NotNull]
        public SimpleComboboxHelper LamePresents { get; private set; }
        
        public bool IsPresentDisabled
        {
            get { return LamePresents.CurrentItem != null && LamePresents.CurrentItem.Name == string.Empty; }
        }

        [NotNull]
        public FloatTextHelper Scale { get; private set; }

        [NotNull]
        public ComboboxHelper<EncoderLAME.LAMEMode> Modes { get; private set; }

        [NotNull]
        public ComboboxHelper<EncoderLAME.LAMEQuality> Qualitys { get; private set; }

        [NotNull]
        public BoolHelper EnforceCbr { get; private set; }

        [NotNull]
        public BoolHelper EnforceIso { get; private set; }

        [NotNull]
        public BoolHelper DisableAllFilters { get; private set; }

        [NotNull]
        public IntTextHelper TargetSampleRate { get; private set; }

        [NotNull]
        public IntTextHelper HighPassFrequency { get; private set; }

        [NotNull]
        public IntTextHelper HighPassFreqenzcyWidth { get; private set; }

        [NotNull]
        public IntTextHelper LowPassFreqency { get; private set; }

        [NotNull]
        public IntTextHelper LowPassFrecuencyWidth { get; private set; }

        public bool IsVariableBitrateEnabled { get { return UseVariableBitrate.Value == true && IsPresentDisabled; } }

        [NotNull]
        public BoolHelper UseVariableBitrate { get; private set; }

        [NotNull]
        public BoolHelper VbrDisableTag { get; private set; }

        [NotNull]
        public IntTextHelper AverageBitrate { get; private set; }

        [NotNull]
        public ComboboxHelper<EncoderLAME.LAMEVBRQuality> VbrQuality { get; private set; }

        [NotNull]
        public BoolHelper LimitVariableBitRate { get; private set; }

        [NotNull]
        public IntTextHelper MaxBitrate { get; private set; }

        [NotNull]
        public BoolHelper EnforceMinimalBitrate { get; private set; }

        [NotNull]
        public ComboboxHelper<ShortBlockUsage> ShortBlocks { get; private set; }

        [NotNull]
        public BoolHelper NoTemp { get; private set; }

        [NotNull]
        public BoolHelper SafeJoint { get; private set; }

        [NotNull]
        public ComboboxHelper<EncoderLAME.LAMEATH> AthControl { get; private set; }

        [NotNull]
        public ComboboxHelper<EncoderLAME.LAMEReplayGain> ReplayGain { get; private set; }

        [NotNull]
        public BoolHelper FreeFormat { get; private set; }

        [NotNull]
        public BoolHelper Copyright { get; private set; }

        [NotNull]
        public BoolHelper NonOriginal { get; private set; }

        [NotNull]
        public BoolHelper ErrorProtection { get; private set; }

        [NotNull]
        public BoolHelper DisableBitReservoir { get; private set; }

        [NotNull]
        public ComboboxHelper<EncoderLAME.LAMENOASM> NoAsm { get; private set; }

        public override void BuildCompled()
        {
            EncoderProfile = new LameEncoderProfile(_editorModel.CurrentProfile);
            
            UseCustomOptionsOnly = new BoolHelper(() => EncoderProfile.UseCustomOptionsOnly);
            Scale = new FloatTextHelper(() => EncoderProfile.Scale);
            EnforceCbr = new BoolHelper(() => EncoderProfile.EnforceCbr);
            EnforceIso = new BoolHelper(() => EncoderProfile.EnforceIso);
            DisableAllFilters = new BoolHelper(() => EncoderProfile.DisableAllFilters);
            TargetSampleRate = new IntTextHelper(() => EncoderProfile.TargetSampleRate);
            HighPassFrequency = new IntTextHelper(() => EncoderProfile.HighPassFreq);
            HighPassFreqenzcyWidth = new IntTextHelper(() => EncoderProfile.HighPassFreqWidth);
            LowPassFreqency = new IntTextHelper(() => EncoderProfile.LowPassFreq);
            LowPassFrecuencyWidth = new IntTextHelper(() => EncoderProfile.HighPassFreqWidth);
            UseVariableBitrate = new BoolHelper(() => EncoderProfile.UseVbr);
            VbrDisableTag = new BoolHelper(() => EncoderProfile.VbrDisableTag);
            AverageBitrate = new IntTextHelper(() => EncoderProfile.AbrBitrate);
            LimitVariableBitRate = new BoolHelper(() => EncoderProfile.LimitVbr);
            MaxBitrate = new IntTextHelper(() => EncoderProfile.VbrMaxBitrate);
            EnforceMinimalBitrate = new BoolHelper(() => EncoderProfile.VbrEnforceMinBitrate);
            NoTemp = new BoolHelper(() => EncoderProfile.PsYnoTemp);
            SafeJoint = new BoolHelper(() => EncoderProfile.PsYnsSafeJoint);
            FreeFormat = new BoolHelper(() => EncoderProfile.FreeFormat);
            Copyright = new BoolHelper(() => EncoderProfile.Copyright);
            NonOriginal = new BoolHelper(() => EncoderProfile.NonOriginal);
            ErrorProtection = new BoolHelper(() => EncoderProfile.Protect);
            DisableBitReservoir = new BoolHelper(() => EncoderProfile.DisableBitReservoir);
            NoAsm = new ComboboxHelper<EncoderLAME.LAMENOASM>(() => EncoderProfile.NoAsm,
                new ComboItem<EncoderLAME.LAMENOASM>(EncoderLAME.LAMENOASM.Default, RadioStreamerResources.LameNoAsmDefault),
                new ComboItem<EncoderLAME.LAMENOASM>(EncoderLAME.LAMENOASM.NO_3DNOW, RadioStreamerResources.LameNoAsmNo3DNow),
                new ComboItem<EncoderLAME.LAMENOASM>(EncoderLAME.LAMENOASM.NO_MMX, RadioStreamerResources.LameNoAsmNoMMX),
                new ComboItem<EncoderLAME.LAMENOASM>(EncoderLAME.LAMENOASM.NO_SSE, RadioStreamerResources.LameNoASMNoSSE));

            ReplayGain = new ComboboxHelper<EncoderLAME.LAMEReplayGain>(() => EncoderProfile.ReplayGain,
                new ComboItem<EncoderLAME.LAMEReplayGain>(EncoderLAME.LAMEReplayGain.None, RadioStreamerResources.LameReplayGainNone),
                new ComboItem<EncoderLAME.LAMEReplayGain>(EncoderLAME.LAMEReplayGain.Default, RadioStreamerResources.LameReplayGainDefault),
                new ComboItem<EncoderLAME.LAMEReplayGain>(EncoderLAME.LAMEReplayGain.Fast, RadioStreamerResources.LameReplayGainFast),
                new ComboItem<EncoderLAME.LAMEReplayGain>(EncoderLAME.LAMEReplayGain.Accurate, RadioStreamerResources.LameReplayGainAccurate));

            AthControl = new ComboboxHelper<EncoderLAME.LAMEATH>(() => EncoderProfile.AthControl,
                new ComboItem<EncoderLAME.LAMEATH>(EncoderLAME.LAMEATH.Default, RadioStreamerResources.LameATHDefault),
                new ComboItem<EncoderLAME.LAMEATH>(EncoderLAME.LAMEATH.Disable, RadioStreamerResources.LameATHNo),
                new ComboItem<EncoderLAME.LAMEATH>(EncoderLAME.LAMEATH.OnlyShortBlocks, RadioStreamerResources.LameATHOnlyShortBlocks),
                new ComboItem<EncoderLAME.LAMEATH>(EncoderLAME.LAMEATH.Only, RadioStreamerResources.LameATHOnly));

            ShortBlocks = new ComboboxHelper<ShortBlockUsage>(null,
                new ComboItem<ShortBlockUsage>(ShortBlockUsage.None, RadioStreamerResources.LameShortBlocksNone),
                new ComboItem<ShortBlockUsage>(ShortBlockUsage.UseNoShortBlocks, RadioStreamerResources.LameShortBlocksUseDont),
                new ComboItem<ShortBlockUsage>(ShortBlockUsage.UseShortBlock, RadioStreamerResources.LameShortBlocksUse),
                new ComboItem<ShortBlockUsage>(ShortBlockUsage.UseOnlyShortBlocks, RadioStreamerResources.LameShortBlockUseOnly));

            VbrQuality = new ComboboxHelper<EncoderLAME.LAMEVBRQuality>(() => EncoderProfile.VbrQuality,
                new ComboItem<EncoderLAME.LAMEVBRQuality>(EncoderLAME.LAMEVBRQuality.VBR_Q0, "Q0"),
                new ComboItem<EncoderLAME.LAMEVBRQuality>(EncoderLAME.LAMEVBRQuality.VBR_Q1, "Q1"),
                new ComboItem<EncoderLAME.LAMEVBRQuality>(EncoderLAME.LAMEVBRQuality.VBR_Q2, "Q2"),
                new ComboItem<EncoderLAME.LAMEVBRQuality>(EncoderLAME.LAMEVBRQuality.VBR_Q3, "Q3"),
                new ComboItem<EncoderLAME.LAMEVBRQuality>(EncoderLAME.LAMEVBRQuality.VBR_Q4, "Q4"),
                new ComboItem<EncoderLAME.LAMEVBRQuality>(EncoderLAME.LAMEVBRQuality.VBR_Q5, "Q5"),
                new ComboItem<EncoderLAME.LAMEVBRQuality>(EncoderLAME.LAMEVBRQuality.VBR_Q6, "Q6"),
                new ComboItem<EncoderLAME.LAMEVBRQuality>(EncoderLAME.LAMEVBRQuality.VBR_Q7, "Q7"),
                new ComboItem<EncoderLAME.LAMEVBRQuality>(EncoderLAME.LAMEVBRQuality.VBR_Q8, "Q8"),
                new ComboItem<EncoderLAME.LAMEVBRQuality>(EncoderLAME.LAMEVBRQuality.VBR_Q9, "Q9"));

            Modes =new ComboboxHelper<EncoderLAME.LAMEMode>(() => EncoderProfile.Mode,
                new ComboItem<EncoderLAME.LAMEMode>(EncoderLAME.LAMEMode.Default, RadioStreamerResources.LameDefaultMode),
                new ComboItem<EncoderLAME.LAMEMode>(EncoderLAME.LAMEMode.DualMono, RadioStreamerResources.LameDualMoneMode),
                new ComboItem<EncoderLAME.LAMEMode>(EncoderLAME.LAMEMode.ForcedJointStereo, RadioStreamerResources.LameForcedJointStereoMode),
                new ComboItem<EncoderLAME.LAMEMode>(EncoderLAME.LAMEMode.JointStereo, RadioStreamerResources.LameJointStereoMode),
                new ComboItem<EncoderLAME.LAMEMode>(EncoderLAME.LAMEMode.Mono, RadioStreamerResources.LameMonoMode),
                new ComboItem<EncoderLAME.LAMEMode>(EncoderLAME.LAMEMode.Stereo, RadioStreamerResources.LameStereoMode));

            Qualitys = new ComboboxHelper<EncoderLAME.LAMEQuality>(() => EncoderProfile.Quality,
                new ComboItem<EncoderLAME.LAMEQuality>(EncoderLAME.LAMEQuality.None, RadioStreamerResources.LameQualityNone),
                new ComboItem<EncoderLAME.LAMEQuality>(EncoderLAME.LAMEQuality.Q0),
                new ComboItem<EncoderLAME.LAMEQuality>(EncoderLAME.LAMEQuality.Q1),
                new ComboItem<EncoderLAME.LAMEQuality>(EncoderLAME.LAMEQuality.Q2),
                new ComboItem<EncoderLAME.LAMEQuality>(EncoderLAME.LAMEQuality.Q3),
                new ComboItem<EncoderLAME.LAMEQuality>(EncoderLAME.LAMEQuality.Q4),
                new ComboItem<EncoderLAME.LAMEQuality>(EncoderLAME.LAMEQuality.Q5),
                new ComboItem<EncoderLAME.LAMEQuality>(EncoderLAME.LAMEQuality.Q6),
                new ComboItem<EncoderLAME.LAMEQuality>(EncoderLAME.LAMEQuality.Q7),
                new ComboItem<EncoderLAME.LAMEQuality>(EncoderLAME.LAMEQuality.Q8),
                new ComboItem<EncoderLAME.LAMEQuality>(EncoderLAME.LAMEQuality.Q9),
                new ComboItem<EncoderLAME.LAMEQuality>(EncoderLAME.LAMEQuality.Quality, RadioStreamerResources.LameQualityQuality),
                new ComboItem<EncoderLAME.LAMEQuality>(EncoderLAME.LAMEQuality.Speed, RadioStreamerResources.LameQualitySpeed));

            LamePresents = new SimpleComboboxHelper(() => EncoderProfile.PresetName,
                new SimpleComboItem(string.Empty, RadioStreamerResources.LamePresentNo),
                new SimpleComboItem("medium", RadioStreamerResources.LamePresentMedium),
                new SimpleComboItem("standard", RadioStreamerResources.LamePresentStandart),
                new SimpleComboItem("extreme", RadioStreamerResources.LamePresentExtreme),
                new SimpleComboItem("insane", RadioStreamerResources.LamePresentInsane));

            UseVariableBitrate.ValueChanedEvent += v => OnPropertyChanged(() => IsVariableBitrateEnabled);
            LamePresents.ValueChanedEvent += v =>
            {
                OnPropertyChanged(() => IsPresentDisabled);
                OnPropertyChanged(() => IsVariableBitrateEnabled);
            };
            UseCustomOptionsOnly.ValueChanedEvent += v =>
            {
                OnPropertyChanged(() => IsPresentDisabled);
                OnPropertyChanged(() => IsCustomOptionsDisabled);
                OnPropertyChanged(() => IsVariableBitrateEnabled);
            };
            ShortBlocks.ValueChanedEvent += usage =>
            {
                switch (usage)
                {
                    case ShortBlockUsage.None:
                        EncoderProfile.PsYallShortBlocks = false;
                        EncoderProfile.PsYnoShortBlocks = false;
                        EncoderProfile.PsYuseShortBlocks = false;
                        break;
                    case ShortBlockUsage.UseShortBlock:
                        EncoderProfile.PsYallShortBlocks = false;
                        EncoderProfile.PsYnoShortBlocks = false;
                        EncoderProfile.PsYuseShortBlocks = true;
                        break;
                    case ShortBlockUsage.UseOnlyShortBlocks:
                        EncoderProfile.PsYallShortBlocks = true;
                        EncoderProfile.PsYnoShortBlocks = false;
                        EncoderProfile.PsYuseShortBlocks = false;
                        break;
                    case ShortBlockUsage.UseNoShortBlocks:
                        EncoderProfile.PsYallShortBlocks = false;
                        EncoderProfile.PsYnoShortBlocks = true;
                        EncoderProfile.PsYuseShortBlocks = false;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("usage");
                }
            };
        }
    }
}
