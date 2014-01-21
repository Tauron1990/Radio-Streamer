using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Mono.CSharp;
using Tauron.Application.RadioStreamer.Contracts.Scripts;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Database.Scripts.CSharp
{
    public class Compiler
    {
        private class ScriptBuilder
        {
            private readonly Type _type;

            public ScriptBuilder([NotNull] Type type)
            {
                _type = type;
            }

            [NotNull]
            public IScript CreateScript()
            {
                return (IScript) _type.GetConstructor(Type.EmptyTypes).Invoke(null);
            }
        }

        public const string ModuleName = "CSharpEngine";

        private TextWriter _logger;
        private AssemblyBuilder _builder;
        private Report _report;

        [CanBeNull]
        public Exception BuildUp([NotNull] string dir, [NotNull] Dictionary<string, Lazy<IScript>> scripts)
        {
            try
            {
                var context = CreateContext(dir);
                var container = CreateModuleContainer(context);
                ParseFiles(container);

                ExplicitDynamicAsseblyDefinition definition;

                if (PrepareCompiler(container, out definition)) return new InvalidDataException("Definition Check Failed");
                if (ExecuteCompiler(container, definition)) return new InvalidOperationException("Compiler Execution Faild");

                ReadScripts(definition.Module, scripts);
            }
            catch (Exception e)
            {
                return e;
            }

            return null;
        }

        public static void ReadScripts([NotNull] Module module, [NotNull] Dictionary<string, Lazy<IScript>> scripts)
        {
            foreach (var type in module.GetTypes()
                                       .Where(type => type.BaseType == typeof (ScriptObject))) scripts[type.Name] = new Lazy<IScript>(new ScriptBuilder(type).CreateScript);
        }

        [NotNull]
        private CompilerContext CreateContext([NotNull] string dir)
        {
            var settings = new CompilerSettings
            {
                Target = Target.Library,
                TargetExt = ".dll",
                LoadDefaultReferences = true,
                ShowFullPaths = false,
                StdLib = true,
            };

            var context = new CompilerContext(settings, new StreamReportPrinter(_logger));

            int index = 0;
            foreach (var file in dir.EnumerateFiles("*.cs"))
            {
                context.SourceFiles.Add(new SourceFile(file.GetFileNameWithoutExtension(), file, index));
                index++;
            }

            _report = new Report(context, new ConsoleReportPrinter(_logger));

            return context;
        }

        [NotNull]
        private static ModuleContainer CreateModuleContainer([NotNull] CompilerContext context)
        {
            var container = new ModuleContainer(context);
            RootContext.ToplevelTypes = container;

            return container;
        }

        private void ParseFiles([NotNull] ModuleContainer container)
        {
            Location.Initialize(container.Compiler.SourceFiles);
            var session = new ParserSession
            {
                UseJayGlobalArrays = true,
                LocatedTokens = new Tokenizer.LocatedToken[15000]
            };

            foreach (var sourceFile in container.Compiler.SourceFiles)
            {
                using (var stream = new FileStream(sourceFile.FullPathName, FileMode.Open))
                {
                    var compilationSourceFile = new CompilationSourceFile(container, sourceFile);
                    container.AddTypeContainer(compilationSourceFile);
                    using (var seekStream = new SeekableStreamReader(stream, Encoding.UTF8))
                    {
                        new CSharpParser(seekStream, compilationSourceFile, _report, session).parse();
                    }
                }
            }
        }

        private bool PrepareCompiler([NotNull] ModuleContainer container, out ExplicitDynamicAsseblyDefinition assemblyDefinitionDynamic)
        {
            assemblyDefinitionDynamic = new ExplicitDynamicAsseblyDefinition(container, ModuleName, _builder);
            container.SetDeclaringAssembly(assemblyDefinitionDynamic);

            var importer = new ReflectionImporter(container, container.Compiler.BuiltinTypes);
            assemblyDefinitionDynamic.Importer = importer;

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) importer.ImportAssembly(assembly, container.GlobalRootNamespace);

            return container.Compiler.BuiltinTypes.CheckDefinitions(container);
        }

        private bool ExecuteCompiler([NotNull] ModuleContainer container, [NotNull] ExplicitDynamicAsseblyDefinition definition)
        {
            container.CreateContainer();
            container.InitializePredefinedTypes();
            container.Define();
            if (_report.Errors > 0) return false;

            definition.Resolve();
            if (_report.Errors > 0) return false;

            definition.Emit();

            if (_report.Errors > 0) return false;

            container.CloseContainer();

            return _report.Errors == 0;
        }

        public void Init([NotNull] TextWriter logger, [NotNull] AssemblyBuilder builder)
        {
            _logger = logger;
            _builder = builder;
        }
    }
}