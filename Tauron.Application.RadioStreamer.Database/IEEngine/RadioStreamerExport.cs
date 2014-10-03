using System;
using System.Collections.Generic;

namespace Tauron.Application.RadioStreamer.Database.IEEngine
{
    [Serializable]
    public class RadioStreamerExport
    {
        public Dictionary<string, string> Settings { get; private set; }

    }
}