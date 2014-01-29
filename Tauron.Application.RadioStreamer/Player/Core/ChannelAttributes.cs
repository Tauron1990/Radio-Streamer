using System;
using Un4seen.Bass;

namespace Tauron.Application.RadioStreamer.Player.Core
{
    /// <summary>
    ///     Used  with setting /getting ChannelAttributes
    /// </summary>
    public class ChannelAttributes //really a struct
    {
        private readonly int _handle;

        public ChannelAttributes(int handle)
        {
            _handle = handle;
        }

        /// <summary>
        ///     Playback rate (-1=leave current)
        /// </summary>
        public float Frequency
        {
            get { return GetValue(BASSAttribute.BASS_ATTRIB_FREQ); }
            set { SetValue(BASSAttribute.BASS_ATTRIB_FREQ, value); }
        }

        /// <summary>
        ///     Volume (-1=leave current, 0=silent, 1=max)
        /// </summary>
        public float Volume
        {
            get { return GetValue(BASSAttribute.BASS_ATTRIB_VOL); }
            set { SetValue(BASSAttribute.BASS_ATTRIB_VOL, value); }
        }

        /// <summary>
        ///     pan position(-1 = Left, 0 = middle, 1 = Right) panning has no effect on 3D channels
        /// </summary>
        public float Panning
        {
            get { return GetValue(BASSAttribute.BASS_ATTRIB_PAN); }
            set { SetValue(BASSAttribute.BASS_ATTRIB_PAN, value); }
        }

        private float GetValue(BASSAttribute attr)
        {
            float value = 0;
            if (!Bass.BASS_ChannelGetAttribute(_handle, BASSAttribute.BASS_ATTRIB_FREQ, ref value))
                throw new BASSException();
            return value;
        }

        private void SetValue(BASSAttribute attr, float value)
        {
            if (!Bass.BASS_ChannelSetAttribute(_handle, BASSAttribute.BASS_ATTRIB_FREQ, value))
                throw new BASSException();
        }

        public override string ToString()
        {
            return String.Format("Freq: {0} Vol: {1} Pan: {2}",
                Frequency, Volume, Panning);
        }
    }
}