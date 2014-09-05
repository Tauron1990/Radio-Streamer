using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NuGet;
using Tauron.Application.Ioc;
using Tauron.Application.RadioStreamer.Contracts.Core;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.PlugIns
{
    [Export(typeof (IPlugInManager))]
    public class PlugInManager : IPlugInManager
    {
        public void LoadPakage(string name)
        {
            InternalPackageManager.PackPath.CreateDirectoryIfNotExis();

            var repo = PackageRepositoryFactory.Default.CreateRepository(@"C:\nuget");
            var pack = repo.FindPackage(name);
            var man = new PackageManager(repo, InternalPackageManager.PackPath);
            man.InstallPackage(pack, false, false);
            LoadingPackage(pack);
        }

        private void LoadingPackage([NotNull] IPackage pack)
        {
            var set = (SearchSet(pack, new Version(4, 5)) ?? SearchSet(pack, new Version(4, 0))) ??
                      pack.PackageAssemblyReferences.FirstOrDefault();

            const string ver1 = "net45";
            const string ver2 = "net40";
            const string libstart = "lib";
            string[] versions = {libstart.CombinePath(ver1), libstart.CombinePath(ver2)};

            var files = new List<string>();

            foreach (var version in versions)
            {
                string version1 = version;
                files.AddRange(from assemblyReference in pack.AssemblyReferences
                               where assemblyReference.Path.StartsWith(version1)
                               select InternalPackageManager.PackPath.CombinePath(pack.Id + "." + pack.Version, assemblyReference.Path));
                if (files.Count > 0) break;
            }

            foreach (
                var file in
                    from reference in set.References from file in files where file.EndsWith(reference) select file) Assembly.LoadFrom(file);
        }

        private PackageReferenceSet SearchSet(IPackage pack, Version ver)
        {
            return
                pack.PackageAssemblyReferences.FirstOrDefault(
                    p =>
                    p.TargetFramework != null &&
                    (p.TargetFramework.Version == ver || p.SupportedFrameworks.Any(f => f.Version == ver)));
        }

        public void LoadPlugIn(string name)
        {
        
        }
    }
}