using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Contracts.Scripts
{
    [PublicAPI]
    public interface IScriptEngine
    {
        [NotNull]
        string Name { get; }

        [NotNull]
        string[] Extensions { get; }

        [CanBeNull]
        Exception BuildUp([NotNull] string dir, [NotNull] Dictionary<string, Lazy<IScript>> scripts);

        void Init([NotNull] TextWriter logger, [NotNull] AssemblyBuilder builder, string cacheDll);

        void ReadScripts([NotNull] Assembly assembly, [NotNull] Dictionary<string, Lazy<IScript>> scripts);
    }
}