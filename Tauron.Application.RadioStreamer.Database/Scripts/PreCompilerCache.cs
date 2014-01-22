using System;

namespace Tauron.Application.RadioStreamer.Database.Scripts
{
    [Serializable]
    public sealed class PreCompilerCache
    {
        public DateTime CreationTime { get; set; }

        public PreCompilerCache(DateTime creationTime)
        {
            CreationTime = creationTime;
        }
    }
}
