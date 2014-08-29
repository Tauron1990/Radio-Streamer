using System;
using System.IO;
using System.Reflection.Emit;
using System.Text;
using Mono.CSharp;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Styling
{
    public class StyleScriptCompiler
    {
        private sealed class StyleExplicitDynamicAsseblyDefinition : AssemblyDefinitionDynamic
        {
            [NotNull]
            private ModuleBuilder Module { get; set; }

            public StyleExplicitDynamicAsseblyDefinition([NotNull] ModuleContainer module, [NotNull] string name,
                                                    [NotNull] AssemblyBuilder builder)
                : base(module, name)
            {
                ResolveAssemblySecurityAttributes();
                Builder = builder;

                Module = builder.DefineDynamicModule(name, true); //CreateModuleBuilder();
                this.module.Create(this, Module);
            }
        }
        private class ObjectBuilder
        {
            private readonly Type _type;

            public ObjectBuilder([NotNull] Type type)
            {
                _type = type;
            }

            [NotNull]
            public object CreateObject()
            {
                var constructorInfo = _type.GetConstructor(Type.EmptyTypes);
                if (constructorInfo != null)
                    return constructorInfo.Invoke(null);
                throw new InvalidOperationException();
            }
        }

        //private const string ModuleName = "CSharpEngine";

        private TextWriter _logger;
        private AssemblyBuilder _builder;
        private Report _report;
        private string _moduleName;

        public void BuildUp([NotNull] string source, [NotNull] string typeName)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (typeName == null) throw new ArgumentNullException("typeName");
            
            var context = CreateContext(source);
            var container = CreateModuleContainer(context);
            ParseFiles(container);

            StyleExplicitDynamicAsseblyDefinition definition;

            if (!PrepareCompiler(container, out definition)) throw new InvalidDataException("Definition Check Failed");
            if (!ExecuteCompiler(container, definition))
                throw new InvalidOperationException("StyleScriptCompiler Execution Faild");
        }

        [NotNull]
        private CompilerContext CreateContext([NotNull] string source)
        {
            var settings = new CompilerSettings
            {
                Target = Target.Library,
                TargetExt = ".dll",
                LoadDefaultReferences = true,
                ShowFullPaths = false,
                StdLib = true,
            };

            var context = new CompilerContext(settings, new ConsoleReportPrinter(_logger));
            context.SourceFiles.Add(new SourceFile("Source", source, 0));
            
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
                LocatedTokens = new LocatedToken[15000]
            };

            foreach (var sourceFile in container.Compiler.SourceFiles)
            {
                var compilationSourceFile = new CompilationSourceFile(container, sourceFile);
                container.AddTypeContainer(compilationSourceFile);
                using (
                    var seekStream =
                        new SeekableStreamReader(new MemoryStream(Encoding.UTF8.GetBytes(sourceFile.FullPathName)),
                            Encoding.UTF8))
                {
                    new CSharpParser(seekStream, compilationSourceFile, _report, session).parse();
                }
            }
        }

        private bool PrepareCompiler([NotNull] ModuleContainer container, out StyleExplicitDynamicAsseblyDefinition assemblyDefinitionDynamic)
        {
            assemblyDefinitionDynamic = new StyleExplicitDynamicAsseblyDefinition(container, _moduleName, _builder);
            container.SetDeclaringAssembly(assemblyDefinitionDynamic);

            var importer = new ReflectionImporter(container, container.Compiler.BuiltinTypes);
            assemblyDefinitionDynamic.Importer = importer;

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) importer.ImportAssembly(assembly, container.GlobalRootNamespace);

            return container.Compiler.BuiltinTypes.CheckDefinitions(container);
        }

        private bool ExecuteCompiler([NotNull] ModuleContainer container, [NotNull] StyleExplicitDynamicAsseblyDefinition definition)
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

        public void Init([NotNull] TextWriter logger, [NotNull] AssemblyBuilder builder, [NotNull] string moduleName)
        {
            if (logger == null) throw new ArgumentNullException("logger");
            if (builder == null) throw new ArgumentNullException("builder");
            if (moduleName == null) throw new ArgumentNullException("moduleName");

            _logger = logger;
            _builder = builder;
            _moduleName = moduleName;
        }
    }
}