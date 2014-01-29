using System;
using Tauron.JetBrains.Annotations;
using Un4seen.Bass;

namespace Tauron.Application.RadioStreamer.Player.Core
{
    /// <summary>
    ///     Summary description for BASSException.
    /// </summary>
    public class BASSException : Exception
    {
        private readonly BASSError _err;

        public BASSException() : this(GetErrorCode())
        {
        }

        private BASSException(int code) : base(GetErrorDescription((BASSError) code))
        {
            _err = (BASSError)code;
        }

        /// <summary>
        ///     Get the error state
        /// </summary>
        public Error ErrorState
        {
            get { return _err; }
        }

        /// <summary>
        ///     Get an description for the error that occurred
        /// </summary>
        [NotNull]
        public string ErrorDescription
        {
            get { return GetErrorDescription(_err); }
        }

        // Get the BASS_ERROR_xxx error code. Use this function to get the reason for an error.
        //[DllImport("bass.dll", EntryPoint = "BASS_ErrorGetCode")]
        //static extern int _ErrorGetCode(); //OK

        protected static int GetErrorCode()
        {
            return (int) Bass.BASS_ErrorGetCode();
        }

        [NotNull]
        protected static string GetErrorDescription(BASSError error)
        {
            switch (error)
            {
                case Error.BASSOk:
                    return "All is OK";
                case Error.MEM:
                    return "Memory Error";
                case Error.FILEOPEN:
                    return "Can't Open the File";
                case Error.DRIVER:
                    return "Can't Find a Free Sound Driver";
                case Error.BUFLOST:
                    return "The Sample Buffer Was Lost - Please Report This!";
                case Error.HANDLE:
                    return "Invalid Handle";
                case Error.FORMAT:
                    return "Unsupported Format";
                case Error.POSITION:
                    return "Invalid Playback Position";
                case Error.INIT:
                    return "BASSEngine.Init Has Not Been Successfully Called";
                case Error.START:
                    return "BASS_Start Has Not Been Successfully Called";
                case Error.INITCD:
                    return "Can't Initialize CD";
                case Error.CDINIT:
                    return "BASS_CDInit Has Not Been Successfully Called";
                case Error.NOCD:
                    return "No CD in drive";
                case Error.CDTRACK:
                    return "Can't Play the Selected CD Track";
                case Error.ALREADY:
                    return "Already Initialized";
                case Error.CDVOL:
                    return "CD Has No Volume Control";
                case Error.NOPAUSE:
                    return "Not Paused";
                case Error.NOTAUDIO:
                    return "Not An Audio Track";
                case Error.NOCHAN:
                    return "Can't Get a Free Channel";
                case Error.ILLTYPE:
                    return "An Illegal Type Was Specified";
                case Error.ILLPARAM:
                    return "An Illegal Parameter Was Specified";
                case Error.NO3D:
                    return "No 3D Support";
                case Error.NOEAX:
                    return "No EAX Support";
                case Error.DEVICE:
                    return "Illegal Device Number";
                case Error.NOPLAY:
                    return "Not Playing";
                case Error.FREQ:
                    return "Illegal Sample Rate";
                case Error.NOA3D:
                    return "A3D.DLL is Not Installed";
                case Error.NOTFILE:
                    return "The Stream is Not a File Stream (WAV/MP3)";
                case Error.NOHW:
                    return "No Hardware Voices Available";
                case Error.EMPTY:
                    return "The MOD music has no sequence data";
                case Error.NONET:
                    return "No Internet connection could be opened";
                case Error.CREATE:
                    return "Couldn't create the file";
                case Error.NOFX:
                    return "Effects are not enabled";
                case Error.PLAYING:
                    return "The channel is playing";
                case Error.NOTAVAIL:
                    return "The requested data is not available";
                case Error.DECODE:
                    return "The channel is a 'decoding channel' ";
                case Error.WmaLicense:
                    return "the file is protected";
                case Error.UNKNOWN:
                    return "Some Other Mystery Error";
                default:
                    return "Unkown Error";
            }
        }

        // ***********************************************
        // * Error codes returned by BASS_GetErrorCode() *
        // ***********************************************
    }
}