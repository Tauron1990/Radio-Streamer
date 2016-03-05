using System;
using System.Collections.Generic;
using System.IO;
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
        private class InstalledPackInfo : IInstalledPackInfo
        {
            private static readonly IPackageRepository MachineCache = NuGet.MachineCache.Default;

            private readonly string _name;

            public InstalledPackInfo([NotNull] InternalPackageManager.CacheEntry cacheEntry,
                [NotNull] IPackageRepository repository)
            {
                if (cacheEntry == null) throw new ArgumentNullException(nameof(cacheEntry));
                if (repository == null) throw new ArgumentNullException(nameof(repository));

                var versionSpec = new VersionSpec(cacheEntry.SemanticVersion);
                var package = MachineCache.FindPackage(cacheEntry.Name, versionSpec, true, true)
                           ?? repository.FindPackage(cacheEntry.Name, versionSpec, true, true);

                CanUnInstall = true;
                
                _name = cacheEntry.Name;
                Name = package.Title;
                Description = package.Description;
                Version = package.Version.Version;
            }

            public bool CanUnInstall { get; }
            public void UnInstall()
            {
                InternalPackageManager.BuildPluginManager().UnInstall(_name);
            }

            public string Name { get; }
            public string Description { get; }
            public Version Version { get; }
        }
        private class InstallablePackInfo : IInstallablePackInfo
        {
            private readonly IPackage _package;
            private readonly InternalPackageManager _manager;

            public InstallablePackInfo([NotNull] IPackage package, [NotNull] InternalPackageManager manager)
            {
                if (package == null) throw new ArgumentNullException(nameof(package));
                if (manager == null) throw new ArgumentNullException(nameof(manager));

                _package = package;
                _manager = manager;
            }

            public string Name => _package.Title;
            public string Description => _package.Description;
            public Version Version => _package.Version.Version;

            public void Install()
            {
                _manager.InstallPackage(_package);
            }
        }

        private readonly InternalPackageManager _pluginManager;
        private readonly InternalPackageManager _packageManager;

        public PlugInManager()
        {
            _pluginManager = InternalPackageManager.BuildPluginManager();
            _packageManager = InternalPackageManager.BuildPackManager();
        }

        public IEnumerable<IInstallablePackInfo> GetAvailableAddIns()
        {
            return _pluginManager.GetAvailableAddIns().Select(p => new InstallablePackInfo(p, _pluginManager));
        }

        public IEnumerable<IInstalledPackInfo> GetInstalledAddIns()
        {
            return
                _pluginManager.GetEntries()
                    .Select(cacheEntry => new InstalledPackInfo(cacheEntry, _packageManager.PackageRepository));
        }

        public void LoadPakage(string name)
        {
            foreach (var file in _packageManager.Install(name, true).Where(f => f.EndsWith(".dll") || f.EndsWith(".exe")))
            {
                try
                {
                    Assembly.LoadFrom(file);
                }
                catch (IOException){}
                catch(BadImageFormatException){}
                catch(ArgumentException){}
            }
        }

        public void InstallPlugIn(string name)
        {
            _pluginManager.Install(name);
        }

        public void Unistall(string name, bool plugin)
        {
            if(plugin)
                _pluginManager.UnInstall(name);
            else 
                _packageManager.UnInstall(name);
        }
    }
}