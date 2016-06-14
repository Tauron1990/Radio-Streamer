using System;
using Tauron.Application.BassLib.Misc;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.BassLib.Recording
{
    public sealed class Recorder : ObservableObject, IDisposable
    {
        private AudioEncoder _encoder;
        private bool _isRecording;

        [CanBeNull]
        public AudioEncoder Encoder
        {
            get { return _encoder; }
            set
            {
                _encoder = value;

                if(_encoder != null && !_encoder.SupportsStdout)
                    throw new InvalidOperationException("Only SupportsSTDOUT is Supported");

                if (_isRecording)
                    _encoder?.Dispose();

                IsRecording = false;
                OnPropertyChanged();
            }
        }

        public bool IsRecording
        {
            get
            {
                return _isRecording;
            }
            private set
            {
                _isRecording = value;
                OnPropertyChanged();
            }
        }

        public void Start(bool paused = false)
        {
            if(Encoder == null) return;

            IsRecording = true;
            Encoder.Start(paused);
        }

        public void Stop()
        {
            if(Encoder == null) return;

            Encoder.Stop();
            IsRecording = false;
        }

        public void ChangeChannel([NotNull] Channel channel)
        {
            if (channel == null) throw new ArgumentNullException(nameof(channel));
            if (Encoder == null) return;

            Encoder.Channel = channel;
        }

        public void Dispose()
        {
            DisposeImpl();
            GC.SuppressFinalize(this);
        }

        private void DisposeImpl()
        {
            _encoder.Dispose();
            IsRecording = false;
            _encoder = null;
        }

        ~Recorder()
        {
            DisposeImpl();
        }
    }
}