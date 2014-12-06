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

    [ExportViewModel(LameEncoderProfile.LameId)]
    [NotShared]
    public sealed class LameEncodingEditorViewModel : ViewModelBase
    {
        public abstract class HelperBase<TType> : ObservableObject
        {
            public event Action ValueChanedEvent;

            protected void OnValueChanedEvent()
            {
                Action handler = ValueChanedEvent;
                if (handler != null) handler();
            }

            private Action<TType> _internalSetter;

            protected readonly Action<TType> Setter;
            protected readonly Func<TType> Getter;

            protected HelperBase([NotNull] Expression<Func<object>> name)
            {
                if (name == null) throw new ArgumentNullException("name");

                var expression = (MemberExpression)name.Body;
                var targetExpression = ((ConstantExpression) expression.Expression);
                var target = targetExpression.Value;

                var info = expression.Member as PropertyInfo;
                if(info == null)
                    throw new InvalidOperationException("No Property Found");
                
                Getter = () => (TType) info.GetValue(target);
                _internalSetter = b => info.SetValue(target, b);

                Setter = value =>
                {
                    _internalSetter(value);
                    OnValueChanedEvent();
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

            public ComboboxHelper([NotNull] Expression<Func<object>> name, [NotNull] params ComboItem<TEnum>[] items) 
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
                return value.ToString();
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
        public IntTextHelper TargetSampleRate { get; set; }

        public override void BuildCompled()
        {
            EncoderProfile = new LameEncoderProfile(_editorModel.CurrentProfile);

            UseCustomOptionsOnly = new BoolHelper(() => EncoderProfile.UseCustomOptionsOnly);
            Scale = new FloatTextHelper(() => EncoderProfile.Scale);
            EnforceCbr = new BoolHelper(() => EncoderProfile.EnforceCbr);
            EnforceIso = new BoolHelper(() => EncoderProfile.EnforceIso);
            DisableAllFilters = new BoolHelper(() => EncoderProfile.DisableAllFilters);
            TargetSampleRate = new IntTextHelper(() => EncoderProfile.TargetSampleRate);

            Modes =new ComboboxHelper<EncoderLAME.LAMEMode>(() => EncoderProfile.Mode,
                new ComboItem<EncoderLAME.LAMEMode>(EncoderLAME.LAMEMode.Default, RadioStreamerResources.LameDefaultMode),
                new ComboItem<EncoderLAME.LAMEMode>(EncoderLAME.LAMEMode.DualMono, RadioStreamerResources.LameDualMoneMode),
                new ComboItem<EncoderLAME.LAMEMode>(EncoderLAME.LAMEMode.ForcedJointStereo, RadioStreamerResources.LameForcedJointStereoMode),
                new ComboItem<EncoderLAME.LAMEMode>(EncoderLAME.LAMEMode.JointStereo, RadioStreamerResources.LameJointStereoMode),
                new ComboItem<EncoderLAME.LAMEMode>(EncoderLAME.LAMEMode.Mono, RadioStreamerResources.LameMonoMode),
                new ComboItem<EncoderLAME.LAMEMode>(EncoderLAME.LAMEMode.Stereo, RadioStreamerResources.LameStereoMode));

            Qualitys = new ComboboxHelper<EncoderLAME.LAMEQuality>(() => EncoderProfile,
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

            LamePresents.ValueChanedEvent += () => OnPropertyChanged(() => IsPresentDisabled);
        }
    }
}
