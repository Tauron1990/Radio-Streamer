﻿#region Usings

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Forms.Integration;
using System.Windows.Threading;
using Ookii.Dialogs.Wpf;
using Tauron.Application.BassLib.Misc;
using Tauron.Application.Ioc;
using Tauron.Application.Models;
using Tauron.Application.RadioStreamer.Contracts;
using Tauron.Application.RadioStreamer.Contracts.Core;
using Tauron.Application.RadioStreamer.Contracts.Player;
using Tauron.Application.RadioStreamer.Resources;
using Tauron.JetBrains.Annotations;
using SprectrumPictureBox = System.Windows.Forms.PictureBox;
using SprectrumPicture = System.Drawing.Bitmap;
using Picture = System.Drawing.Image;

#endregion

namespace Tauron.Application.RadioStreamer.Views.RadioPlayer.Sprectrum
{
    [ExportViewModel(AppConstants.SprectrumViewModel)]
    public sealed class SprecturmViewModel : ViewModelBase, INotifyBuildCompled
    {
        private Spectrums _currentSpectrum;
        [Inject] private IEventAggregator _events;
        [Inject] private IRadioPlayer _player;

        [Inject] private IRadioEnvironment _radioEnvironment;
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

        void INotifyBuildCompled.BuildCompled()
        {
            _events.GetEvent<PlayerViewVisibleChanged, bool>().Subscribe(PlayerVisibleChanged);
            _events.GetEvent<RadioPlayerPlay, EventArgs>().Subscribe(Play);
            _events.GetEvent<RadioPlayerStop, EventArgs>().Subscribe(Stop);

            _currentSpectrum = _radioEnvironment.Settings.LastSprecturm.ParseEnum<Spectrums>();

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
            _currentSpectrum = new SpectumChoiceBox(_currentSpectrum).Show(
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
                ? _player.CreateSprectrum(_currentSpectrum, _sprectrumPicture.Width - 20,
                    _sprectrumPicture.Height - 20)
                : null;

            if (pic != null) pic.Dispose();
        }

        private void PlayerVisibleChanged(bool obj)
        {
            IsVisble = obj ? Visibility.Visible : Visibility.Hidden;
        }

        private class SpectumChoiceBox
        {
            private readonly Spectrums _currentSpectrums;
            private readonly TaskDialog _dialog;

            private readonly TaskDialogButton _okButton;

            private readonly Dictionary<TaskDialogRadioButton, string> _spectrumMapping =
                new Dictionary<TaskDialogRadioButton, string>();

            private Spectrums _choice;

            public SpectumChoiceBox(Spectrums currentSpectrums)
            {
                _currentSpectrums = currentSpectrums;
                _choice = currentSpectrums;

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

                foreach (var name in Enum.GetNames(typeof (Spectrums)))
                {
                    var btn = new TaskDialogRadioButton {Text = SpectrumResources.ResourceManager.GetString(name)};

                    _dialog.RadioButtons.Add(btn);

                    _spectrumMapping[btn] = name;
                    if (currentSpectrums.ToString() == name) btn.Checked = true;
                }

                _dialog.RadioButtonClicked += DialogOnRadioButtonClicked;
            }

            private void DialogOnRadioButtonClicked([NotNull] object sender,
                [NotNull] TaskDialogItemClickedEventArgs taskDialogItemClickedEventArgs)
            {
                var btn = taskDialogItemClickedEventArgs.Item as TaskDialogRadioButton;
                if (btn == null) return;

                _choice = _spectrumMapping[btn].ParseEnum<Spectrums>();
            }

            public Spectrums Show([NotNull] Window window)
            {
                var result = _dialog.ShowDialog(window) == _okButton ? _choice : _currentSpectrums;
                _dialog.Dispose();
                return result;
            }
        }
    }
}