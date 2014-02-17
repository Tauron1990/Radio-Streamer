using System.Drawing;
using Tauron.JetBrains.Annotations;
using Un4seen.Bass.Misc;

namespace Tauron.Application.BassLib.Misc
{
    [PublicAPI]
    public enum Spectrums
    {
        Graph,
        Bean,
        Dot,
        Ellipse,
        Line,
        LinePeak,
        Wave,
        Text
    }

    [PublicAPI]
    public class VisualHelper : ObservableObject
    {
        private readonly Visuals _visuals = new Visuals();

        private Color _base;
        private Color _peak;
        private Color _peakHold;
        private Color _background;
        private Spectrums _lastSpectrum;
        private int _width;
        private int _height;
        private int _genericLineTikness;
        private int _distance;
        private string _textContent;

        public VisualHelper()
        {
            _base = Color.DarkSlateBlue;
            _peak = Color.DeepPink;
            _peakHold = Color.DarkViolet;
            _background = Color.Black;

            _width = 600;
            _height = 600;
            _genericLineTikness = 7;
            _distance = 1;
            _textContent = "Unkown";
        }

        public Color Base
        {
            get { return _base; }
            set
            {
                _base = value;
                OnPropertyChanged();
            }
        }

        public Color Peak
        {
            get { return _peak; }
            set
            {
                if (Equals(_peak, value)) return;

                _peak = value;

                OnPropertyChanged();
            }
        }

        public Color PeakHold
        {
            get { return _peakHold; }
            set
            {
                if (Equals(_peakHold, value)) return;

                _peakHold = value;

                OnPropertyChanged();
            }
        }

        public Color Background
        {
            get { return _background; }
            set
            {
                _background = value;
                OnPropertyChanged();
            }
        }

        public int Width
        {
            get { return _width; }
            set
            {
                _width = value;
                OnPropertyChanged();
            }
        }

        public int Height
        {
            get { return _height; }
            set
            {
                _height = value;
                OnPropertyChanged();
            }
        }

        public int GenericLineTikness
        {
            get { return _genericLineTikness; }
            set
            {
                _genericLineTikness = value;
                OnPropertyChanged();
            }
        }

        public int Distance
        {
            get { return _distance; }
            set
            {
                _distance = value;
                OnPropertyChanged();
            }
        }

        [NotNull]
        public string TextContent
        {
            get { return _textContent; }
            set
            {
                if(string.IsNullOrEmpty(value)) return;

                _textContent = value;
                OnPropertyChanged();
            }
        }

        [CanBeNull]
        public Channel Channel { get; set; }

        public VisualHelper([NotNull] Channel channel)
            : this()
        {
            Channel = channel;
        }

        [CanBeNull]
        public Bitmap CreateSpectrum(Spectrums spectrum)
        {
            if(Channel == null) return null;

            if (_lastSpectrum != spectrum) _visuals.ClearPeaks();
            _lastSpectrum = spectrum;

            switch (spectrum)
            {
                case Spectrums.Graph:
                    return _visuals.CreateSpectrum(Channel.Handle, Width, Height, Base, Peak, Background, false, true,
                                                   true);
                case Spectrums.Bean:
                    return _visuals.CreateSpectrumBean(Channel.Handle, Width, Height, Base, Peak, Background,
                                                       _genericLineTikness, false, true, true);
                case Spectrums.Dot:
                    return _visuals.CreateSpectrumDot(Channel.Handle, Width, Height, Base, Peak, Background,
                                                      GenericLineTikness, Distance, false, true, true);
                case Spectrums.Ellipse:
                    return _visuals.CreateSpectrumEllipse(Channel.Handle, Width, Height, Base, Peak, Background,
                                                          GenericLineTikness, Distance, false, true, true);
                case Spectrums.Line:
                    return _visuals.CreateSpectrumLine(Channel.Handle, Width, Height, Base, Peak, Background,
                                                       GenericLineTikness, Distance, false, true, true);
                case Spectrums.LinePeak:
                    return _visuals.CreateSpectrumLinePeak(Channel.Handle, Width, Height, Base, Peak, PeakHold,
                                                           Background, GenericLineTikness, 2, Distance,
                                                           20, false, true, true);
                case Spectrums.Wave:
                    return _visuals.CreateSpectrumWave(Channel.Handle, Width, Height, Base, Peak, Background,
                                                       GenericLineTikness, false, true, true);
                default:
                    return _visuals.CreateSpectrumText(Channel.Handle, Width, Height, Base, Peak, Background,
                                                       TextContent, false, true, true);
            }
        }
    }
}