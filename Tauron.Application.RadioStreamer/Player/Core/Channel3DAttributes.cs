using System;
using Un4seen.Bass;

namespace Tauron.Application.RadioStreamer.Player.Core
{
    /// <summary>
    ///     Used with setting and getting Channel3DAttributes
    /// </summary>
    public class Channel3DAttributes
    {
        private int _iangle;
        private float _max;
        private float _min;
        private BASS3DMode _mode;
        private int _oangle;
        private int _outvol;

        public Channel3DAttributes()
        {
            _mode = BASS3DMode.BASS_3DMODE_OFF;
            _iangle = _oangle = -1;
            _outvol = 100;
            _min = _max = 0F;
        }

        /// <summary>
        ///     Used with setting and getting Channel3DAttributes
        /// </summary>
        /// <param name="mode">BASS_3DMODE_xxx mode (-1=leave current setting)</param>
        /// <param name="min">minimum distance, volume stops increasing within this distance (smaller than 0.0=leave current)</param>
        /// <param name="max">maximum distance, volume stops decreasing past this distance (smaller than 0.0=leave current)</param>
        /// <param name="iangle">angle of inside projection cone in degrees (360=omnidirectional, -1=leave current)</param>
        /// <param name="oangle">
        ///     angle of outside projection cone in degrees (-1=leave current)
        ///     NOTE: iangle & oangle must both be set in a single call
        /// </param>
        /// <param name="outvol">delta-volume outside the projection cone (0=silent, 100=same as inside)</param>
        /// <remarks>
        ///     The iangle/oangle angles decide how wide the sound is projected around the
        ///     orientation angle. Within the inside angle the volume level is the channel
        ///     level as set with BASS_ChannelSetAttributes, from the inside to the outside
        ///     angles the volume gradually changes by the "outvol" setting.
        /// </remarks>
        public Channel3DAttributes(BASS3DMode mode, float min, float max,
            int iangle, int oangle, int outvol)
        {
            _mode = mode;
            _min = min;
            _max = max;
            _iangle = iangle;
            _oangle = oangle;
            _outvol = outvol;
        }

        public int IAngle
        {
            get { return _iangle; }
            set { _iangle = value; }
        }

        public float Max
        {
            get { return _max; }
            set { _max = value; }
        }

        public float Min
        {
            get { return _min; }
            set { _min = value; }
        }

        public BASS3DMode Mode
        {
            get { return _mode; }
            set { _mode = value; }
        }

        public int Oangle
        {
            get { return _oangle; }
            set { _oangle = value; }
        }

        public int Outvol
        {
            get { return _outvol; }
            set { _outvol = value; }
        }

        public override string ToString()
        {
            return String.Format("Mode: {0} Min: {1} Max: {2} IAngle: {3} OAngle: {4} OutVol: {5}",
                _mode, _min, _max, _iangle, _oangle, _outvol);
        }
    }
}