using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Tauron.Application.BassLib.Misc;
using Tauron.Application.Ioc;
using Tauron.Application.RadioStreamer.Contracts.Player.Misc;
using Tauron.Application.RadioStreamer.Resources;

namespace Tauron.Application.RadioStreamer.Player
{
    [Export(typeof(ISpecrumProvider))]
    public sealed class SpectrumProvider : ISpecrumProvider
    {

        private readonly VisualHelper _visualHelper;

        public SpectrumProvider()
        {
            List<SpectrumEntry> entries = Enum.GetNames(typeof (Spectrums)).Select(name => new SpectrumEntry {ID = name, Name = SpectrumResources.ResourceManager.GetString(name)}).ToList();

            SpectrumEntries = entries;

            _visualHelper = new VisualHelper(null);
            CoreMediaPlayer.ChannelSwitchEvent += i => _visualHelper.Channel = i;
        }

        public IEnumerable<SpectrumEntry> SpectrumEntries { get; }

        public Bitmap CreateSprectrum(SpectrumEntry playerCode, int width, int height)
        {
            _visualHelper.Width = width;
            _visualHelper.Height = height;

            return _visualHelper.CreateSpectrum(playerCode.ID.ParseEnum<Spectrums>());
        }
    }
}