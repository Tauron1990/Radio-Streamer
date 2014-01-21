using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using Tauron.Application.Ioc;
using Tauron.Application.RadioStreamer.Contracts.Scripts;

namespace Tauron.Application.RadioStreamer.Database.Scripts.CSharp
{
    [Export(typeof(IScriptEngine))]
    public class CSharpEngine : IScriptEngine
    {
        public string Name
        {
            get { return "CSharp"; }
        }

        public string[] Extensions
        {
            get { return new[] {"*.cs"}; }
        }

        private Compiler _compiler = new Compiler();

        public Exception BuildUp(string dir, Dictionary<string, Lazy<IScript>> scripts)
        {
            return _compiler.BuildUp(dir, scripts);
        }

        public void Init(TextWriter logger, AssemblyBuilder builder)
        {
            _compiler.Init(logger, builder);
        }

        public void ReadScripts(Assembly assembly, Dictionary<string, Lazy<IScript>> scripts)
        {
            Module module = assembly.GetModule(Compiler.ModuleName);
            if(module == null)
                return;

            Compiler.ReadScripts(module, scripts);
        }
    }
}