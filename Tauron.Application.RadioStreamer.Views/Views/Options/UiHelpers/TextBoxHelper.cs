using System;
using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Tauron.Application.RadioStreamer.Contracts.Core;
using Tauron.Application.RadioStreamer.Contracts.UI;
using Tauron.Application.RadioStreamer.Resources;

namespace Tauron.Application.RadioStreamer.Views.Options.UiHelpers
{
    public sealed class TextBoxHelper : OptionHelperBaseClass, INotifyDataErrorInfo
    {
        private readonly bool _optionalPathButton;
        private readonly bool _isPath;
        private readonly IDialogFactory _factory;
        private FrameworkElement _textBox;
        private string _textContent;
        private bool _hasErrors;
        private string _lastValue;

        public TextBoxHelper(bool optionalPathButton, bool isPath = false, IDialogFactory factory = null)
        {
            if(optionalPathButton && factory == null)
                throw new ArgumentNullException(nameof(factory));

            _optionalPathButton = optionalPathButton;
            _isPath = isPath;
            _factory = factory;
        }

        public string TextContent
        {
            get { return _textContent; }
            set
            {
                _textContent = value;
                OnPropertyChanged();
                VerifyPath(value);
                OnErrorsChanged();
            }
        }

        public override FrameworkElement LoadUI(Option option)
        {
            if (_textBox != null) return _textBox;

            var textBox = new TextBox { MinWidth = 100};
            textBox.SetBinding(TextBox.TextProperty, new Binding(nameof(TextContent)) { Delay = 100, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged});

            if (_optionalPathButton)
            {
                var panel = new StackPanel { DataContext = this, Orientation = Orientation.Horizontal };

                var button = new Button {Content = "...", Margin = new Thickness(10, 0, 0, 0)};

                button.Click += ButtonOnClick;

                panel.Children.Add(textBox);
                panel.Children.Add(button);

                _textBox = panel;
            }
            else
            {
                textBox.DataContext = this;
                _textBox = textBox;
            }

            return _textBox;
        }

        protected override string GetCurrentValue()
        {
            return TextContent;
        }

        protected override void SetValue(string value)
        {
            TextContent = value;
        }

        private void ButtonOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            bool? result;
            var path = _factory.ShowOpenFolderDialog(CommonApplication.Current.MainWindow,
                RadioStreamerResources.PathSeeking, Environment.SpecialFolder.MyMusic, true, false, out result);

            if(result != true) return;

            TextContent = path;
        }

        private void VerifyPath(string value)
        {
            if (!_isPath)
            {
                HasErrors = false;
                return;
            }

            if (string.IsNullOrEmpty(value))
                HasErrors = false;
            else
                HasErrors = !value.ExisDirectory();
        }

        public IEnumerable GetErrors(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(TextContent):
                    yield return RadioStreamerResources.PathNotExisting;
                    break;

                default:
                    yield break;
            }
        }

        public bool HasErrors
        {
            get { return _hasErrors; }
            set { _hasErrors = value; OnPropertyChanged();}
        }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        private void OnErrorsChanged([CallerMemberName] string name = null)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(name));
        }
    }
}
