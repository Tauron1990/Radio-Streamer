using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Media;
using Tauron.Application.Commands;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Contracts.UI
{
    [PublicAPI]
    public sealed class GenericMenuItem : ObservableObject
    {
        internal readonly List<Action<GenericMenuItem>> ClickActions;
        private string _id;
        private ImageSource _imageSource;
        private string _label;
        private bool _isEnabled;
        private ICommand _clickCommand;

        public GenericMenuItem([NotNull] params Action<GenericMenuItem>[] clickActions)
        {
            ClickActions = new List<Action<GenericMenuItem>>(clickActions);
            IsEnabled = true;
            ClickCommand = new SimpleCommand(Click);
        }

        [NotNull]
        public string Id
        {
            get { return _id ?? Label ?? string.Empty; }
            set { _id = value; }
        }

        [CanBeNull]
        public ImageSource ImageSource
        {
            get { return _imageSource; }
            set { _imageSource = value; OnPropertyChanged();}
        }

        [CanBeNull]
        public string Label
        {
            get { return _label; }
            set { _label = value; OnPropertyChanged();}
        }

        [CanBeNull]
        public List<GenericMenuItem> MenuItems { get; set; }

        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { _isEnabled = value; OnPropertyChanged(); }
        }

        [NotNull]
        public ICommand ClickCommand
        {
            get { return _clickCommand; }
            private set { _clickCommand = value; OnPropertyChanged();}
        }

        public void Click([CanBeNull] object temp)
        {
            foreach (var clickAction in ClickActions)
            {
                clickAction(this);
            }
        }
    }
}