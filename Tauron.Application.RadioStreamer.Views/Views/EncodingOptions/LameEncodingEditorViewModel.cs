using System;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using Tauron.Application.Ioc;
using Tauron.Application.Models;
using Tauron.Application.RadioStreamer.Contracts;
using Tauron.Application.RadioStreamer.Contracts.Data;
using Tauron.Application.RadioStreamer.Contracts.UI;
using Tauron.Application.RadioStreamer.Resources;
using Tauron.JetBrains.Annotations;
// ReSharper disable InconsistentNaming

namespace Tauron.Application.RadioStreamer.Views.EncodingOptions
{
    public class SimpleComboItem
    {
        [NotNull]
        public string Name { get; }

        [NotNull]
        private string DisplayName { get; }

        public SimpleComboItem([NotNull] string name, [NotNull] string displayName)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (displayName == null) throw new ArgumentNullException(nameof(displayName));
            
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
        public TEnum Value { get; }

        [NotNull, UsedImplicitly]
        public string DisplayName { get; }

        public ComboItem([NotNull] TEnum value, [NotNull] string displayName)
        {
            if (displayName == null) throw new ArgumentNullException(nameof(displayName));

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

    [ExportViewModel(AppConstants.LameId)]
    [NotShared]
    public sealed class LameEncodingEditorViewModel : ViewModelBase
    {
        public enum Lameath
        {
            Default,
            Only,
            Disable,
            OnlyShortBlocks,
        }

        public enum Lamenoasm
        {
            Default,
            NO_MMX,
            NO_3DNOW,
            NO_SSE,
        }

        public enum LamevbrQuality
        {
            VBR_Q0,
            VBR_Q1,
            VBR_Q2,
            VBR_Q3,
            VBR_Q4,
            VBR_Q5,
            VBR_Q6,
            VBR_Q7,
            VBR_Q8,
            VBR_Q9,
        }

        public enum LameReplayGain
        {
            Default,
            Fast,
            Accurate,
            None,
        }

        public enum LameQuality
        {
            Q0,
            Q1,
            Q2,
            Q3,
            Q4,
            Q5,
            Q6,
            Q7,
            Q8,
            Q9,
            None,
            Speed,
            Quality,
        }
        
        public enum LameMode
        {
            Default,
            Stereo,
            JointStereo,
            ForcedJointStereo,
            Mono,
            DualMono,
        }

        [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
        public sealed class LameEncoderProfile
        {
            private readonly CommonProfile _profile;

            public LameEncoderProfile([CanBeNull] CommonProfile profile)
            {
                _profile = profile;
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

            public LameQuality Quality
            {
                get { return GetValue<LameQuality>(); }
                set { SetValue(value); }
            }

            public LameReplayGain ReplayGain
            {
                get { return GetValue<LameReplayGain>(); }
                set { SetValue(value); }
            }

            public LamevbrQuality VbrQuality
            {
                get { return GetValue<LamevbrQuality>(); }
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

            public LameMode Mode
            {
                get { return GetValue<LameMode>(); }
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

            public Lamenoasm NoAsm
            {
                get { return GetValue<Lamenoasm>(); }
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

            public Lameath AthControl
            {
                get { return GetValue<Lameath>(); }
                set { SetValue(value); }
            }

            private TType GetValue<TType>([CallerMemberName] string name = null)
            {
                bool temp;
                // ReSharper disable once AssignNullToNotNullAttribute
                return _profile.TryGetProperty(ValueConverter<TType>, name, out temp);
            }

            private TType ValueConverter<TType>(string arg)
            {
                var type = typeof (TType);
                if (type.IsEnum)
                    return (TType) Enum.Parse(type, arg);
                if (type.IsValueType)
                    return (TType)Convert.ChangeType(arg, type);
                if (type == typeof (string))
                    return (TType)(object)arg;

                throw new InvalidCastException();
            }

            private void SetValue<TType>(TType value, [CallerMemberName] string name = null)
            {
                // ReSharper disable once AssignNullToNotNullAttribute
                _profile.SetProperty(value, name);
            }
        }

        public abstract class HelperBase<TType> : ObservableObject
        {
            public event Action<TType> ValueChanedEvent;

            protected void OnValueChanedEvent(TType e)
            {
                Action<TType> handler = ValueChanedEvent;
                handler?.Invoke(e);
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
                    Setter(value?.Name);
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
                if (!String.IsNullOrEmpty(text))
                    return Single.TryParse(text, NumberStyles.Number, CultureInfo.CurrentUICulture, out value);
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
                return Int32.TryParse(text, out value);
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

        public bool IsCustomOptionsDisabled => UseCustomOptionsOnly.Value == false;

        [NotNull]
        public SimpleComboboxHelper LamePresents { get; private set; }
        
        public bool IsPresentDisabled => LamePresents.CurrentItem != null && LamePresents.CurrentItem.Name == string.Empty;

        [NotNull]
        public FloatTextHelper Scale { get; private set; }

        [NotNull]
        public ComboboxHelper<LameMode> Modes { get; private set; }

        [NotNull]
        public ComboboxHelper<LameQuality> Qualitys { get; private set; }

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

        public bool IsVariableBitrateEnabled => UseVariableBitrate.Value == true && IsPresentDisabled;

        [NotNull]
        public BoolHelper UseVariableBitrate { get; private set; }

        [NotNull]
        public BoolHelper VbrDisableTag { get; private set; }

        [NotNull]
        public IntTextHelper AverageBitrate { get; private set; }

        [NotNull]
        public ComboboxHelper<LamevbrQuality> VbrQuality { get; private set; }

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
        public ComboboxHelper<Lameath> AthControl { get; private set; }

        [NotNull]
        public ComboboxHelper<LameReplayGain> ReplayGain { get; private set; }

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
        public ComboboxHelper<Lamenoasm> NoAsm { get; private set; }

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
            NoAsm = new ComboboxHelper<Lamenoasm>(() => EncoderProfile.NoAsm,
                new ComboItem<Lamenoasm>(Lamenoasm.Default, RadioStreamerResources.LameNoAsmDefault),
                new ComboItem<Lamenoasm>(Lamenoasm.NO_3DNOW, RadioStreamerResources.LameNoAsmNo3DNow),
                new ComboItem<Lamenoasm>(Lamenoasm.NO_MMX, RadioStreamerResources.LameNoAsmNoMMX),
                new ComboItem<Lamenoasm>(Lamenoasm.NO_SSE, RadioStreamerResources.LameNoASMNoSSE));

            ReplayGain = new ComboboxHelper<LameReplayGain>(() => EncoderProfile.ReplayGain,
                new ComboItem<LameReplayGain>(LameReplayGain.None, RadioStreamerResources.LameReplayGainNone),
                new ComboItem<LameReplayGain>(LameReplayGain.Default, RadioStreamerResources.LameReplayGainDefault),
                new ComboItem<LameReplayGain>(LameReplayGain.Fast, RadioStreamerResources.LameReplayGainFast),
                new ComboItem<LameReplayGain>(LameReplayGain.Accurate, RadioStreamerResources.LameReplayGainAccurate));

            AthControl = new ComboboxHelper<Lameath>(() => EncoderProfile.AthControl,
                new ComboItem<Lameath>(Lameath.Default, RadioStreamerResources.LameATHDefault),
                new ComboItem<Lameath>(Lameath.Disable, RadioStreamerResources.LameATHNo),
                new ComboItem<Lameath>(Lameath.OnlyShortBlocks, RadioStreamerResources.LameATHOnlyShortBlocks),
                new ComboItem<Lameath>(Lameath.Only, RadioStreamerResources.LameATHOnly));

            ShortBlocks = new ComboboxHelper<ShortBlockUsage>(null,
                new ComboItem<ShortBlockUsage>(ShortBlockUsage.None, RadioStreamerResources.LameShortBlocksNone),
                new ComboItem<ShortBlockUsage>(ShortBlockUsage.UseNoShortBlocks, RadioStreamerResources.LameShortBlocksUseDont),
                new ComboItem<ShortBlockUsage>(ShortBlockUsage.UseShortBlock, RadioStreamerResources.LameShortBlocksUse),
                new ComboItem<ShortBlockUsage>(ShortBlockUsage.UseOnlyShortBlocks, RadioStreamerResources.LameShortBlockUseOnly));

            VbrQuality = new ComboboxHelper<LamevbrQuality>(() => EncoderProfile.VbrQuality,
                new ComboItem<LamevbrQuality>(LamevbrQuality.VBR_Q0, "Q0"),
                new ComboItem<LamevbrQuality>(LamevbrQuality.VBR_Q1, "Q1"),
                new ComboItem<LamevbrQuality>(LamevbrQuality.VBR_Q2, "Q2"),
                new ComboItem<LamevbrQuality>(LamevbrQuality.VBR_Q3, "Q3"),
                new ComboItem<LamevbrQuality>(LamevbrQuality.VBR_Q4, "Q4"),
                new ComboItem<LamevbrQuality>(LamevbrQuality.VBR_Q5, "Q5"),
                new ComboItem<LamevbrQuality>(LamevbrQuality.VBR_Q6, "Q6"),
                new ComboItem<LamevbrQuality>(LamevbrQuality.VBR_Q7, "Q7"),
                new ComboItem<LamevbrQuality>(LamevbrQuality.VBR_Q8, "Q8"),
                new ComboItem<LamevbrQuality>(LamevbrQuality.VBR_Q9, "Q9"));

            Modes =new ComboboxHelper<LameMode>(() => EncoderProfile.Mode,
                new ComboItem<LameMode>(LameMode.Default, RadioStreamerResources.LameDefaultMode),
                new ComboItem<LameMode>(LameMode.DualMono, RadioStreamerResources.LameDualMoneMode),
                new ComboItem<LameMode>(LameMode.ForcedJointStereo, RadioStreamerResources.LameForcedJointStereoMode),
                new ComboItem<LameMode>(LameMode.JointStereo, RadioStreamerResources.LameJointStereoMode),
                new ComboItem<LameMode>(LameMode.Mono, RadioStreamerResources.LameMonoMode),
                new ComboItem<LameMode>(LameMode.Stereo, RadioStreamerResources.LameStereoMode));

            Qualitys = new ComboboxHelper<LameQuality>(() => EncoderProfile.Quality,
                new ComboItem<LameQuality>(LameQuality.None, RadioStreamerResources.LameQualityNone),
                new ComboItem<LameQuality>(LameQuality.Q0),
                new ComboItem<LameQuality>(LameQuality.Q1),
                new ComboItem<LameQuality>(LameQuality.Q2),
                new ComboItem<LameQuality>(LameQuality.Q3),
                new ComboItem<LameQuality>(LameQuality.Q4),
                new ComboItem<LameQuality>(LameQuality.Q5),
                new ComboItem<LameQuality>(LameQuality.Q6),
                new ComboItem<LameQuality>(LameQuality.Q7),
                new ComboItem<LameQuality>(LameQuality.Q8),
                new ComboItem<LameQuality>(LameQuality.Q9),
                new ComboItem<LameQuality>(LameQuality.Quality, RadioStreamerResources.LameQualityQuality),
                new ComboItem<LameQuality>(LameQuality.Speed, RadioStreamerResources.LameQualitySpeed));

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
                        throw new ArgumentOutOfRangeException(nameof(usage));
                }
            };
        }
    }
}
