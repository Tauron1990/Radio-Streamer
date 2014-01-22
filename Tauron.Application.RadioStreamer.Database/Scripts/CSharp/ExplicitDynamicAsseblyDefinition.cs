using System.Reflection.Emit;
using Mono.CSharp;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Database.Scripts.CSharp
{
    public sealed class ExplicitDynamicAsseblyDefinition : AssemblyDefinitionDynamic
    {
        [NotNull]
        public ModuleBuilder Module { get; private set; }

        public ExplicitDynamicAsseblyDefinition([NotNull] ModuleContainer module, [NotNull] string name,
                                                [NotNull] AssemblyBuilder builder)
            : base(module, name)
        {
            ResolveAssemblySecurityAttributes();
            Builder = builder;

            Module = CreateModuleBuilder();
            this.module.Create(this, Module);
         }
    }
}