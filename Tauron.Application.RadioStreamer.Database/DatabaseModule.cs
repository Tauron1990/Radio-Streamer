using System.Reflection;
using Tauron.Application.Modules;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Database
{
    [ExportModule]
    public class DatabaseModule : IModule
    {
        [NotNull, AddinDescription]
        public AddinDescription GetDescription()
        {
            var asm = Assembly.GetExecutingAssembly();
            var name = asm.GetName();
            return new AddinDescription(name.Version,
                asm.GetCustomAttribute<AssemblyDescriptionAttribute>().Description,
                asm.GetCustomAttribute<AssemblyTitleAttribute>().Title);
        }

        public int Order { get { return 1000; } }
        public void Initialize(CommonApplication application)
        {
        
        }
    }
}
