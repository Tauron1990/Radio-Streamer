using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Tauron.Application.Ioc;
using Tauron.Application.RadioStreamer.Contracts;
using Tauron.Application.RadioStreamer.Contracts.Scripts;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Database.Scripts
{
    [Export(typeof(IEngineManager))]
    public class EngineManager : IEngineManager, INotifyBuildCompled
    {
        private const string ScriptsPath = "Scripts";
        private const string Cachedll = "Cache.dll";
        [Inject]
        private List<IScriptEngine> _engines;

        [Inject] 
        private ITauronEnviroment _enviroment;

        private Dictionary<string, Lazy<IScript>> _scripts = new Dictionary<string, Lazy<IScript>>();

        public string[] ScriptNames { get; private set; }
        public string[] EngineNames { get; private set; }
        public string[] Extensions { get; private set; }

        public IScript SearchForScript(string script)
        {
            Lazy<IScript> scriptValue;
            return _scripts.TryGetValue(script, out scriptValue) ? scriptValue.Value : null;
        }

        public void PreCompile(TextWriter logger)
        {
            _scripts.Clear();
            string path = _enviroment.LocalApplicationData.CombinePath(AppConstants.AppName, "ScriptCache");

            path.CreateDirectoryIfNotExis();

            string dll = path.CombinePath(Cachedll);
            string info = path.CombinePath("Cache.bin");
            string scriptsPath = ScriptsPath.GetFullPath();

            scriptsPath.CreateDirectoryIfNotExis();

            if (!scriptsPath.EnumerateAllFiles().Any()) return;

            if (dll.ExisFile() && info.ExisFile())
            {
                try
                {
                    var cacheInfo = info.Deserialize<PreCompilerCache>();

                    
                    var temp = Directory.GetLastWriteTime(scriptsPath);
                    // ReSharper disable once InvertIf
                    if (cacheInfo.CreationTime > temp)
                    {
                        ExecuteLoad(dll);
                        return;
                    }
                }
                catch (Exception e)
                {
                    if (CriticalExceptions.IsCriticalApplicationException(e)) throw;
                }
            }

            ExecuteCompileStep(dll, logger, scriptsPath);
            new PreCompilerCache(DateTime.Now.AddMinutes(3)).Serialize(info);

            ScriptNames = _scripts.Keys.ToArray();
        }

        private void ExecuteCompileStep([NotNull] string dll, [NotNull] TextWriter logger, [NotNull]string scriptPath)
        {
            var name = new AssemblyName
            {
                CodeBase = dll,
                ContentType = AssemblyContentType.Default,
                Name = "Cache",
                ProcessorArchitecture = ProcessorArchitecture.X86,
                Version = new Version(1, 0)
            };

            AssemblyBuilder builder = AppDomain.CurrentDomain.DefineDynamicAssembly(name,
                                                                                    AssemblyBuilderAccess.RunAndSave,
                                                                                    dll.GetDirectoryName());

            foreach (var scriptEngine in _engines)
            {
                scriptEngine.Init(logger, builder, Cachedll);
                var temp = scriptEngine.BuildUp(scriptPath, _scripts);
                if (temp != null) throw temp;
            }

            builder.Save(Cachedll, PortableExecutableKinds.ILOnly, ImageFileMachine.I386);
        }

        private void ExecuteLoad([NotNull] string dll)
        {
            var asm = Assembly.LoadFile(dll);

            foreach (var scriptEngine in _engines) scriptEngine.ReadScripts(asm, _scripts);
            ScriptNames = _scripts.Keys.ToArray();
        }

        public void BuildCompled()
        {
            EngineNames = _engines.Select(e => e.Name).ToArray();
            Extensions = _engines.SelectMany(e => e.Extensions).ToArray();
        }
    }
}