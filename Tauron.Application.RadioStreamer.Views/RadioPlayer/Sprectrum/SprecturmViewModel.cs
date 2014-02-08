using System;
using System.Drawing;
using System.Windows;
using System.Windows.Threading;
using Tauron.Application.Ioc;
using Tauron.Application.Models;
using Tauron.Application.RadioStreamer.Contracts;
using Tauron.Application.RadioStreamer.Contracts.Core;
using Tauron.Application.RadioStreamer.Contracts.Player;
using Tauron.JetBrains.Annotations;
using SprectrumPictureBox = System.Windows.Forms.PictureBox;
using SprectrumPicture = System.Drawing.Bitmap;
using Picture = System.Drawing.Image;


namespace Tauron.Application.RadioStreamer.Views.RadioPlayer.Sprectrum
{
                //    "Graph",
                //"Balken",
                //"Punkt",
                //"Elipse",
                //"Linie",
                //"Linien-Spitze",
                //"Welle"

    [ExportViewModel(AppConstants.SprectrumViewModel)]
    public sealed class SprecturmViewModel : ViewModelBase, INotifyBuildCompled
    {
        private DispatcherTimer _sprectrumTimer;
        [Inject]
        private IRadioPlayer _player;

        [Inject] 
        private IEventAggregator _events;

        void INotifyBuildCompled.BuildCompled()
        {
            _events.GetEvent<PlayerViewVisibleChanged, bool>().Subscribe(PlayerVisibleChanged);
            _events.GetEvent<RadioPlayerPlay, EventArgs>().Subscribe(Play);
            _events.GetEvent<RadioPlayerStop, EventArgs>().Subscribe(Stop);
            
            _sprectrumTimer = new DispatcherTimer(TimeSpan.FromMilliseconds(250), DispatcherPriority.Normal, UpdateSprectrum, SystemDispatcher);
            _sprectrumTimer.Stop();
            CurrentDispatcher.Invoke(() =>
            {
                SprectrumPicture = new SprectrumPictureBox {BackColor = Color.Transparent};
            });
        }

        private void Stop([NotNull] EventArgs obj)
        {
            _sprectrumTimer.Stop();
        }

        private void Play([NotNull] EventArgs obj)
        {
            _sprectrumTimer.Start();
        }

        private System.Windows.Forms.Integration.WindowsFormsHost _sprectrumHost;
        [CanBeNull, ControlTarget]
        public System.Windows.Forms.Integration.WindowsFormsHost SprectrumHost
        {
            get { return _sprectrumHost; }
            set
            {
                if (_sprectrumPicture != null) if (value != null) value.Child = _sprectrumPicture;
                _sprectrumHost = value;
            }
        }


        private string _currentSpectrum = "Balken";

        private SprectrumPictureBox _sprectrumPicture;

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

        private void UpdateSprectrum([CanBeNull] object sender, [CanBeNull] EventArgs e)
        {
            Picture pic = _sprectrumPicture.Image;

            _sprectrumPicture.Image = IsVisble == Visibility.Visible
                                          ? _player.CreateSprectrum(_currentSpectrum, _sprectrumPicture.Width,
                                                                    _sprectrumPicture.Height)
                                          : null;

            if (pic != null) pic.Dispose();
        }

        private void PlayerVisibleChanged(bool obj)
        {
            IsVisble = obj ? Visibility.Visible : Visibility.Hidden;
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
    }
}
