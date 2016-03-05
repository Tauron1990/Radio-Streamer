using System;
using System.Globalization;
using System.Windows.Threading;
using Tauron.Application.Ioc;
using Tauron.Application.Models;
using Tauron.Application.Models.Rules;
using Tauron.Application.RadioStreamer.Contracts;
using Tauron.Application.RadioStreamer.Contracts.Core;
using Tauron.Application.RadioStreamer.Contracts.Core.Attributes;
using Tauron.Application.RadioStreamer.Contracts.Player;
using Tauron.Application.RadioStreamer.Resources;

namespace Tauron.Application.RadioStreamer.Views.RadioPlayer.ShutdownAutimatic
{
    [ExportViewModel(AppConstants.ShutdownAutomaticViewModelName)]
    public class ShutdownAutomaticViewModel : ViewModelBase
    {
        private ObservableProperty _timeStringProperty;
        private DateTime _targetTime;
        private DispatcherTimer _dispatcherTimer;
        private bool _isActive;
        private bool _isRunning;
        private bool _canActivate;
        private LinkedProperty _linkedPlaying;

        [InjectRadioPlayer]
        private IRadioPlayer _player;
        [Inject(typeof (IProgramManager))]
        private IProgramManager _programManager;

        private bool? _shutDownPc;
        private string _timeLeftValue;

        public ShutdownAutomaticViewModel()
        {
            RegisterProperty("TimeString", typeof (ShutdownAutomaticViewModel), typeof (string),
                new ObservablePropertyMetadata(TimeStringChanged).SetValidationRules(new RequiredRule(), new TimeSpanParsingRule()));

            _dispatcherTimer = new DispatcherTimer(TimeSpan.FromSeconds(1), DispatcherPriority.Background, HandleTimer, SystemDispatcher);
            _dispatcherTimer.Stop();
            UpdateTimeLeftLabel();
        }

        public bool IsActive
        {
            get { return _isActive; }
            set { _isActive = value; OnPropertyChanged(); UpdateTimer();}
        }

        public bool CanActivate
        {
            get { return _canActivate; }
            set { _canActivate = value; OnPropertyChanged(); UpdateTimer(); }
        }

        public bool? ShutDownPc
        {
            get { return _shutDownPc; }
            set { _shutDownPc = value; OnPropertyChanged();}
        }

        public string TimeLeftValue
        {
            get { return _timeLeftValue; }
            set { _timeLeftValue = value; OnPropertyChanged();}
        }

        private void TimeStringChanged(ObservableProperty prop, ModelBase model, object value)
        {
            var val = value as string;

            if(string.IsNullOrWhiteSpace(val)) return;

            try
            {
                var time = TimeSpan.Parse(val, CultureInfo.CurrentUICulture);
                if(time.Ticks < 0) return;

                _targetTime = DateTime.Now + time;
                UpdateTimer();
            }
            catch (Exception e) when(e is OverflowException || e is BadImageFormatException || e is ArgumentException)
            {
            }
        }

        private void UpdateTimer()
        {
            if (!CanActivate && IsActive)
                IsActive = false;

            if (IsActive)
            {
                if(_isRunning) return;

                _dispatcherTimer.Start();
                _isRunning = true;
            }
            else
            {
                if(!_isRunning) return;

                _dispatcherTimer.Stop();
                _isRunning = false;
            }
        }

        private void HandleTimer(object sender, EventArgs e)
        {
            if (DateTime.Now > _targetTime)
                TriggerStop();
        }

        private void TriggerStop()
        {
            if (_player.Playing)
                _player.Stop();

            if (!ShutDownPc == true) return;

            System.Diagnostics.Process.Start(@"C:\WINDOWS\system32\Shutdown", "-s -f -t 30");

            _programManager.Shutdown = true;
            _programManager.MainWindow.Close();
        }

        private void UpdateTimeLeftLabel()
        {
            if (!IsActive)
                TimeLeftValue = RadioStreamerResources.RadioPlayerShutdownTimerTimeLeftValueOff;

            var temp = _targetTime - DateTime.Now;

            if(temp.Ticks < 0)
                temp = TimeSpan.Zero;

            TimeLeftValue = $"{temp:dd\\.hh\\:mm\\:ss}";
        }

        public override void BuildCompled()
        {
            ShutDownPc = false;
            _linkedPlaying = LinkProperty(nameof(_player.Playing), _player, nameof(CanActivate));
        }
    }
}