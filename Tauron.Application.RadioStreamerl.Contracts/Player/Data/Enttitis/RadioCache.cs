using System.Collections.Generic;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Contracts.Data.Enttitis
{
    [PublicAPI]
    public static class RadioCache
    {
        [NotNull]
        public static List<RadioEntry> Cache { get; set; }
    }
}