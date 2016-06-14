#region Usings

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Forms.Integration;
using System.Windows.Threading;
using Ookii.Dialogs.Wpf;
using Tauron.Application.Ioc;
using Tauron.Application.Models;
using Tauron.Application.RadioStreamer.Contracts;
using Tauron.Application.RadioStreamer.Contracts.Core;
using Tauron.Application.RadioStreamer.Contracts.Player;
using Tauron.Application.RadioStreamer.Contracts.Player.Misc;
using Tauron.Application.RadioStreamer.Resources;
using Tauron.JetBrains.Annotations;
using SprectrumPictureBox = System.Windows.Forms.PictureBox;
using SprectrumPicture = System.Drawing.Bitmap;
using Picture = System.Drawing.Image;

#endregion

namespace Tauron.Application.RadioStreamer.Views.RadioPlayer.Sprectrum
{
    [ExportViewModel(AppConstants.SprectrumViewModel)]
    public sealed class SprecturmViewModel : ViewModelBase
    {
        private SpectrumEntry _currentSpectrum;
        [Inject]
        private IEventAggregator _events;
        [Inject]
        private IRadioPlayer _player;
        [Inject]
        private IRadioEnvironment _radioEnvironment;
        [Inject]
        private ISpecrumProvider _specrumProvider;

        private WindowsFormsHost _sprectrumHost;
        private SprectrumPictureBox _sprectrumPicture;
        private DispatcherTimer _sprectrumTimer;

        [CanBeNull, ControlTarget]
        public WindowsFormsHost SprectrumHost
        {
            get { return _sprectrumHost; }
            set
            {
                if (_sprectrumPicture != null) if (value != null) value.Child = _sprectrumPicture;
                _sprectrumHost = value;
            }
        }

        [NotNull]
        public SprectrumPictureBox SprectrumPicture
        {
            get { return _sprectrumPicture; }
            set
            {
                if (_sprectrumPicture == value) return;

                _sprectrumPicture = value;
                if (SprectrumHost != null) SprectrumHost.Child = value;
                OnPropertyChanged();
            }
        }

        public Visibility IsVisble
        {
            get { return _sprectrumHost.Visibility; }
            set
            {
                _sprectrumHost.Visibility = value;
                UpdateSprectrum(null, null);
            }
        }

        public override void BuildCompled()
        {
            _events.GetEvent<PlayerViewVisibleChanged, bool>().Subscribe(PlayerVisibleChanged);
            _events.GetEvent<RadioPlayerPlay, EventArgs>().Subscribe(Play);
            _events.GetEvent<RadioPlayerStop, EventArgs>().Subscribe(Stop);

            _currentSpectrum = _specrumProvider.SpectrumEntries.FirstOrDefault(e => e.Name == _radioEnvironment.Settings.LastSprecturm) ??
                               _specrumProvider.SpectrumEntries.First();

            _sprectrumTimer = new DispatcherTimer(TimeSpan.FromMilliseconds(50), DispatcherPriority.Normal,
                UpdateSprectrum, SystemDispatcher);
            _sprectrumTimer.Stop();
            CurrentDispatcher.Invoke(() =>
            {
                SprectrumPicture = new SprectrumPictureBox {BackColor = Color.Black};
                SprectrumPicture.Click += SprectrumPictureOnClick;
            });
        }

        private void SprectrumPictureOnClick([NotNull] object sender, [NotNull] EventArgs eventArgs)
        {
            _currentSpectrum = new SpectumChoiceBox(_currentSpectrum, _specrumProvider).Show(
                System.Windows.Application.Current.MainWindow);

            _radioEnvironment.Settings.LastSprecturm = _currentSpectrum.ToString();
        }

        private void Stop([NotNull] EventArgs obj)
        {
            _sprectrumTimer.Stop();
        }

        private void Play([NotNull] EventArgs obj)
        {
            _sprectrumTimer.Start();
        }

        private void UpdateSprectrum([CanBeNull] object sender, [CanBeNull] EventArgs e)
        {
            Picture pic = _sprectrumPicture.Image;

            _sprectrumPicture.Image = IsVisble == Visibility.Visible
                ? _specrumProvider.CreateSprectrum(_currentSpectrum, _sprectrumPicture.Width - 20,
                    _sprectrumPicture.Height - 20)
                : null;

            pic?.Dispose();
        }

        private void PlayerVisibleChanged(bool obj)
        {
            IsVisble = obj ? Visibility.Visible : Visibility.Hidden;
        }

        private class SpectumChoiceBox
        {
            private readonly SpectrumEntry _currentSpectrums;
            private readonly TaskDialog _dialog;

            private readonly TaskDialogButton _okButton;

            private readonly Dictionary<TaskDialogRadioButton, SpectrumEntry> _spectrumMapping =
                new Dictionary<TaskDialogRadioButton, SpectrumEntry>();

            private SpectrumEntry _choice;

            public SpectumChoiceBox(SpectrumEntry currentSpectrum, ISpecrumProvider provider)
            {
                _currentSpectrums = currentSpectrum;
                _choice = currentSpectrum;

                var icon = RadioStreamerResources.RadioIcon;

                _dialog = new TaskDialog
                {
                    WindowIcon = icon,
                    MainIcon = TaskDialogIcon.Information,
                    WindowTitle = RadioStreamerResources.SpectrumChoiceWindowLabel,
                    MainInstruction = RadioStreamerResources.SpectrumChoiceMainInstruction
                };
                _okButton = new TaskDialogButton(ButtonType.Ok);

                _dialog.Buttons.Add(_okButton);
                _dialog.Buttons.Add(new TaskDialogButton(ButtonType.Close));

                foreach (var name in provider.SpectrumEntries)
                {
                    var btn = new TaskDialogRadioButton {Text = name.Name};

                    _dialog.RadioButtons.Add(btn);

                    _spectrumMapping[btn] = name;
                    if (currentSpectrum.ID == name.ID) btn.Checked = true;
                }

                _dialog.RadioButtonClicked += DialogOnRadioButtonClicked;
            }

            private void DialogOnRadioButtonClicked([NotNull] object sender,
                [NotNull] TaskDialogItemClickedEventArgs taskDialogItemClickedEventArgs)
            {
                var btn = taskDialogItemClickedEventArgs.Item as TaskDialogRadioButton;
                if (btn == null) return;

                _choice = _spectrumMapping[btn];
            }

            public SpectrumEntry Show([NotNull] Window window)
            {
                var result = _dialog.ShowDialog(window) == _okButton ? _choice : _currentSpectrums;
                _dialog.Dispose();
                return result;
            }
        }
    }
}