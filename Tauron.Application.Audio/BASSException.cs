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
                    case BASSError.:
                        break;
                    case BASSError.BASS_ERROR_DECODE:
                        break;
                    case BASSError.BASS_ERROR_DX:
                        break;
                    case BASSError.BASS_ERROR_TIMEOUT:
                        break;
                    case BASSError.BASS_ERROR_FILEFORM:
                        break;
                    case BASSError.BASS_ERROR_SPEAKER:
                        break;
                    case BASSError.BASS_ERROR_VERSION:
                        break;
                    case BASSError.BASS_ERROR_CODEC:
                        break;
                    case BASSError.BASS_ERROR_ENDED:
                        break;
                    case BASSError.BASS_ERROR_BUSY:
                        break;
                    case BASSError.BASS_ERROR_UNKNOWN:
                        break;
                    case BASSError.BASS_ERROR_WMA_LICENSE:
                        break;
                    case BASSError.BASS_ERROR_WMA_WM9:
                        break;
                    case BASSError.BASS_ERROR_WMA_DENIED:
                        break;
                    case BASSError.BASS_ERROR_WMA_CODEC:
                        break;
                    case BASSError.BASS_ERROR_WMA_INDIVIDUAL:
                        break;
                    case BASSError.BASS_ERROR_ACM_CANCEL:
                        break;
                    case BASSError.BASS_ERROR_CAST_DENIED:
                        break;
                    case BASSError.BASS_VST_ERROR_NOINPUTS:
                        break;
                    case BASSError.BASS_VST_ERROR_NOOUTPUTS:
                        break;
                    case BASSError.BASS_VST_ERROR_NOREALTIME:
                        break;
                    case BASSError.BASS_ERROR_VIDEO_ABORT:
                        break;
                    case BASSError.BASS_ERROR_VIDEO_CANNOT_CONNECT:
                        break;
                    case BASSError.BASS_ERROR_VIDEO_CANNOT_READ:
                        break;
                    case BASSError.BASS_ERROR_VIDEO_CANNOT_WRITE:
                        break;
                    case BASSError.BASS_ERROR_VIDEO_FAILURE:
                        break;
                    case BASSError.BASS_ERROR_VIDEO_FILTER:
                        break;
                    case BASSError.BASS_ERROR_VIDEO_INVALID_CHAN:
                        break;
                    case BASSError.BASS_ERROR_VIDEO_INVALID_DLL:
                        break;
                    case BASSError.BASS_ERROR_VIDEO_INVALID_FORMAT:
                        break;
                    case BASSError.BASS_ERROR_VIDEO_INVALID_HANDLE:
                        break;
                    case BASSError.BASS_ERROR_VIDEO_INVALID_PARAMETER:
                        break;
                    case BASSError.BASS_ERROR_VIDEO_NO_AUDIO:
                        break;
                    case BASSError.BASS_ERROR_VIDEO_NO_EFFECT:
                        break;
                    case BASSError.BASS_ERROR_VIDEO_NO_INTERFACE:
                        break;
                    case BASSError.BASS_ERROR_VIDEO_NO_RENDERER:
                        break;
                    case BASSError.BASS_ERROR_VIDEO_NO_SUPPORT:
                        break;
                    case BASSError.BASS_ERROR_VIDEO_NO_VIDEO:
                        break;
                    case BASSError.BASS_ERROR_VIDEO_NOT_ALLOWED:
                        break;
                    case BASSError.BASS_ERROR_VIDEO_NOT_CONNECTED:
                        break;
                    case BASSError.BASS_ERROR_VIDEO_NOT_EXISTS:
                        break;
                    case BASSError.BASS_ERROR_VIDEO_NOT_FOUND:
                        break;
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