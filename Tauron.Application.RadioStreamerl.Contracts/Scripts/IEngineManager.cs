using System.IO;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Contracts.Scripts
{
    [PublicAPI]
    public interface IEngineManager
    {
        [NotNull]
        string[] ScriptNames { get; }
        [NotNull]
        string[] EngineNames { get; }

        [NotNull]
        string[] Extensions { get; }

        [CanBeNull]
        IScript SearchForScript([NotNull] string script);

        void PreCompile([NotNull] TextWriter logger);
    }
}