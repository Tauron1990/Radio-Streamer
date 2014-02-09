using System;
using Tauron.Application.Ioc;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Player.Engine
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false), BaseTypeRequired(typeof(IPlaybackEngine))]
    [PublicAPI]
    public class ExportPlaybackEngine : ExportAttribute
    {
        [NotNull]
        public string Name { get; private set; }

        public ExportPlaybackEngine([NotNull] string name)
            : base(typeof(IPlaybackEngine))
        {
            Name = name;
        }

        protected override bool HasMetadata
        {
            get { return true; }
        }
    }
}