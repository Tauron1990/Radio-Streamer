using System;
using System.Runtime.Serialization;
using Tauron.JetBrains.Annotations;
using Un4seen.Bass;

namespace Tauron.Application.Audio
{
    [Serializable, PublicAPI]
    public class BassException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //
        private BASSError _error;

        public BASSError Error
        {
            get { return _error; }
        }

        public BassException()
        {
            _error = Bass.BASS_ErrorGetCode();
        }

        public override string Message
        {
            get
            {
                return BassMessages.ResourceManager.GetString(Error.ToString()) ?? BassMessages.ErrorUnkown;
                switch (_error)
                {
                    case BASSError.BASS_ERROR_VIDEO_NOT_READY:
                        break;
                    case BASSError.BASS_ERROR_VIDEO_NULL_DEVICE:
                        break;
                    case BASSError.BASS_ERROR_VIDEO_OPEN:
                        break;
                    case BASSError.BASS_ERROR_VIDEO_OUTOFMEMORY:
                        break;
                    case BASSError.BASS_ERROR_VIDEO_PARTIAL_RENDER:
                        break;
                    case BASSError.BASS_ERROR_VIDEO_TIME_OUT:
                        break;
                    case BASSError.BASS_ERROR_VIDEO_UNKNOWN_FILE_TYPE:
                        break;
                    case BASSError.BASS_ERROR_VIDEO_UNSUPPORT_STREAM:
                        break;
                    case BASSError.BASS_ERROR_VIDEO_VIDEO_FILTER:
                        break;
                    case BASSError.BASS_ERROR_WASAPI:
                        break;
                    default:
                        return BassMessages.ErrorUnkown;
                }

                return BassMessages.ErrorUnkown;
            }
        }

        protected BassException([NotNull] SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}