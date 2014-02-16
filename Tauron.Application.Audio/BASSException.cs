using System;
using System.Runtime.Serialization;
using Tauron.JetBrains.Annotations;
using Un4seen.Bass;

namespace Tauron.Application.BassLib
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

        public BassException(BASSError error)
        {
            _error = error;
        }

        public override string Message
        {
            get
            {
                return BassMessages.ResourceManager.GetString(Error.ToString()) ?? BassMessages.ErrorUnkown;
            }
        }

        protected BassException([NotNull] SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}