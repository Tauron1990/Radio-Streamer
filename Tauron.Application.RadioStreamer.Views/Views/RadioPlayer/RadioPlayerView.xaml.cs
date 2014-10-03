#region Usings

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using Tauron.Application.Controls;
using Tauron.Application.RadioStreamer.Contracts;
using Tauron.Application.Views;

#endregion

namespace Tauron.Application.RadioStreamer.Views.RadioPlayer
{
    /// <summary>
    ///     Interaktionslogik für RadioPlayerView.xaml
    /// </summary>
    [ExportView(AppConstants.RadioPlayerViewModelName, Order = 200)]
    public partial class RadioPlayerView
    {
        private Window _window;

        public RadioPlayerView()
        {
            InitializeComponent();
        }

        private void RadioLoaded(object sender, RoutedEventArgs e)
        {
            _window = Window.GetWindow(this);
            _window.SizeChanged += MainWindowSizeChanged;

            InitAnimation();
        }

        private void MainWindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!e.WidthChanged) return;

            InitAnimation();
        }

        private void InitAnimation()
        {
            var animation = new DoubleAnimation
            {
                From = -100,
                To = _window.ActualWidth + 100,
                RepeatBehavior = RepeatBehavior.Forever
            };

            double duration = _window.ActualWidth/90;
            if (duration < 1)
                duration = 1;

            animation.Duration = new Duration(TimeSpan.FromSeconds((int) duration));

            var clock = animation.CreateClock();

            RadioTitle.ApplyAnimationClock(Canvas.LeftProperty, clock);

            clock.Controller.Begin();
        }
    }
}