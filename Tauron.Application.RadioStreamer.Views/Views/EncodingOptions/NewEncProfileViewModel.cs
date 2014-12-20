using System;
using System.Collections.Generic;
using System.Linq;
using Tauron.Application.Ioc;
using Tauron.Application.Models;
using Tauron.Application.Models.Rules;
using Tauron.Application.RadioStreamer.Contracts;
using Tauron.Application.RadioStreamer.Contracts.Core;
using Tauron.Application.RadioStreamer.Contracts.Core.Attributes;
using Tauron.Application.RadioStreamer.Contracts.Data;
using Tauron.Application.RadioStreamer.Contracts.Player.Recording;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Views.EncodingOptions
{
    internal sealed class EncoderProfileNameRule : ModelRule
    {
        public const string ContextName = "EncoderNames";

        public override bool IsValidValue(object obj, ValidatorContext context)
        {
            var str = obj as string;

            if(string.IsNullOrEmpty(str)) return true;

            IEnumerable<string> names = GetNames(context);
            if(names == null) return true;

            return !names.Contains(str);
        }

        [CanBeNull]
        private static IEnumerable<string> GetNames([NotNull] ValidatorContext context)
        {
            object val;
            if (!context.Items.TryGetValue(ContextName, out val)) return null;
            return val as IEnumerable<string>;
        }
    }

    [ExportViewModel(AppConstants.NewEncodingProfileView)]
    public sealed class NewEncProfileViewModel : ViewModelBase, IResultProvider
    {
        private static readonly ObservableProperty ProfileNameProperty =
            RegisterProperty("ProfileName", typeof (NewEncProfileViewModel), typeof (string),
                new ObservablePropertyMetadata().SetValidationRules(new RequiredRule(), new EncoderProfileNameRule()));

        private static readonly ObservableProperty CurrentEncTypeProperty = RegisterProperty("CurrentEncType",
            typeof (NewEncProfileViewModel), typeof (string),
            new ObservablePropertyMetadata().SetValidationRules(new RequiredRule()));

        [InjectRadioEnviroment]
        private IRadioEnvironment _radioEnvironment;

        private string _profileName;
        [Inject] private IEncoderProvider _encoderProvider;

        private string _currentEncType;

        public object Result
        {
            get
            {
                return Tuple.Create((string) GetValue(ProfileNameProperty),
                    new CommonProfile((string) GetValue(CurrentEncTypeProperty)));
            }
        }

        [NotNull, WindowTarget]
        public IWindow CurrentWindow { get; set; }

        //[CanBeNull]
        //public string ProfileName
        //{
        //    get { return _profileName; }
        //    set
        //    {
        //        _profileName = value;
        //        OnPropertyChanged();
        //    }
        //}

        [NotNull]
        public IEnumerable<string> EncTypes
        {
            get { return _encoderProvider.EncoderIds; }
        }

        //[NotNull]
        //public string CurrentEncType
        //{
        //    get { return _currentEncType; }
        //    set
        //    {
        //        _currentEncType = value;
        //        OnPropertyChanged();
        //    }
        //}

        [CommandTarget]
        public bool CanOk()
        {
            return !HasErrors;
        }

        [CommandTarget]
        public void Ok()
        {
            CurrentWindow.DialogResult = true;
        }

        [CommandTarget]
        public void Abort()
        {
            CurrentWindow.DialogResult = false;
        }

        [EventTarget]
        public void Initialized()
        {
            ValidateAll();
        }

        public override void BuildCompled()
        {
            ValidatorContext.Items[EncoderProfileNameRule.ContextName] =
                _radioEnvironment.Settings.EncoderProfiles.Profiles.ToArray();
        }
    }
}
