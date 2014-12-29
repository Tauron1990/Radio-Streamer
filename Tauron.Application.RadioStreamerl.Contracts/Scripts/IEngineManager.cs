using System.Collections.Generic;
using System.IO;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Contracts.Scripts
{
    [PublicAPI]
    public interface IEngineManager
    {
        [NotNull]
        IEnumerable<string> FileNames { get; }

        [NotNull]
        string[] ScriptNames { get; }
        [NotNull]
        string[] EngineNames { get; }

        [NotNull]
        string[] Extensions { get; }

        void CopyFile([NotNull] string fileName, [NotNull] byte[] content);

        [CanBeNull]
        IScript SearchForScript([NotNull] string script);

        void PreCompile([NotNull] TextWriter logger);
    }
}