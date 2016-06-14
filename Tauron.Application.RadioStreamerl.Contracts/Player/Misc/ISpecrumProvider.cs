using System.Collections.Generic;
using System.Drawing;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Contracts.Player.Misc
{
    public interface ISpecrumProvider
    {
        IEnumerable<SpectrumEntry> SpectrumEntries { get; }

        [CanBeNull]
        Bitmap CreateSprectrum(SpectrumEntry playerCode, int width, int height);
    }
}