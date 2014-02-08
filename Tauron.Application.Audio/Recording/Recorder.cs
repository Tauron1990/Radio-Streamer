using System;
using Tauron.JetBrains.Annotations;
using Un4seen.Bass.Misc;

namespace Tauron.Application.BassLib.Recording
{
    public sealed class Recorder : ObservableObject, IDisposable
    {
        [NotNull]
        public static EncoderLAME CreateLame([NotNull] Channel channel)
        {
            return new EncoderLAME(channel.Handle);
        }

        private BaseEncoder _encoder;
        private bool _isRecording;

        [CanBeNull]
        public BaseEncoder Encoder
        {
            get { return _encoder; }
            set
            {
                if(!_encoder.SupportsSTDOUT)
                    throw new InvalidOperationException("Only SupportsSTDOUT is Supported");

                if(_isRecording)
                    _encoder.Dispose();

                _encoder = value;
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

            Encoder.Start(null, IntPtr.Zero, paused).CheckBass();
        }

        public void Stop()
        {
            if(Encoder == null) return;

            Encoder.Stop();
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