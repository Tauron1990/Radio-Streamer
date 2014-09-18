using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Xml.Serialization;
using NuGet;

namespace Tauron.Application.RadioStreamer.PlugIns
{
    // ReSharper disable CodeAnnotationAnalyzer
    public sealed class InternalPackageManager //: IDisposable
    {
        private static readonly string PluginPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins");
        private static readonly string PackPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Packs");

#if DEBUG
        private const string NugetUrl = @"C:\nuget\Packages";
        private const string MyGetUrl = @"C:\nuget\Plugins";
#else
        private const string NugetUrl
        private const string MyGetUrl
#endif

        private static readonly FrameworkName MyFrameWorkVersion = new FrameworkName(".NETFramework", new Version(4, 5));

        //public static readonly IPackageRepository DefaultRepository =
        //    new AggregateRepository(PackageRepositoryFactory.Default, new[] {MyGetUrl, NugetUrl}, true);

        private static readonly IPackageRepository NugetRepository =
            PackageRepositoryFactory.Default.CreateRepository(NugetUrl);

        private static readonly IPackageRepository MyGetRepository =
            PackageRepositoryFactory.Default.CreateRepository(MyGetUrl);

        private static readonly ICache PackCache = new LocalCache(PackPath, false);

        public static InternalPackageManager BuildRootManager()
        {
            string targetPath = AppDomain.CurrentDomain.BaseDirectory;

            return new InternalPackageManager(MyGetRepository, targetPath, false, targetPath, true);
        }

        public static InternalPackageManager BuildPackManager()
        {
            return new InternalPackageManager(NugetRepository, null, PackCache);
        }

        public static InternalPackageManager BuildPluginManager()
        {
            return new InternalPackageManager(MyGetUrl, PluginPath, false, PluginPath, true);
        }

        [Serializable]
// ReSharper disable once MemberCanBePrivate.Global
        public class CacheEntry
        {
            [XmlIgnore]
            public SemanticVersion SemanticVersion
            {
                get { return new SemanticVersion(Version); }
                set { Version = value.ToString(); }
            }

            public string Name { get; set; }

            public string Path { get; set; }

// ReSharper disable once MemberCanBePrivate.Global
            public string Version { get; set; }

            public string[] Dependencys { get; set; }

            public string[] Files { get; set; }
        }

        private class VersionFileSerializer : IEnumerable<IPackageName>, IEnumerable<CacheEntry>
        {
            private const string VersionsFileName = "Packages.xml";

            private static readonly XmlSerializer Serializer = new XmlSerializer(
                typeof (List<CacheEntry>), new[] {typeof (CacheEntry)});

            private readonly string _location;

            private readonly List<CacheEntry> _versions;

            public bool IsFilePresent
            {
                get { return File.Exists(_location); }
            }

            public VersionFileSerializer(string location)
            {
                _location = Path.Combine(location, VersionsFileName);

                try
                {
                    using (var stream = new FileStream(_location, FileMode.Open)) _versions = (List<CacheEntry>) Serializer.Deserialize(stream);
                }
                catch (Exception)
                {
                    File.Delete(_location);
                    _versions = new List<CacheEntry>();
                }
            }

            public void SaveVersions()
            {
                using (var stream = new FileStream(_location, FileMode.Create))
                {
                    Serializer.Serialize(stream, _versions);
                }
            }

            public bool TryGetValue(string name, out CacheEntry entry)
            {
                if (name == null) throw new ArgumentNullException("name");

                var ent = _versions.Find(e => e.Name == name);

                entry = ent;

                return ent != null;
            }

            public CacheEntry this[string name]
            {
                set
                {
                    var ent = _versions.FindIndex(e => e.Name == name);
                    if(ent != -1)
                        _versions.RemoveAt(ent);
                    _versions.Add(value);
                }
            }

            public void Remove(string name)
            {
                var ent = _versions.FindIndex(e => e.Name == name);
                _versions.RemoveAt(ent);
                SaveVersions();
            }

            IEnumerator<CacheEntry> IEnumerable<CacheEntry>.GetEnumerator()
            {
                return _versions.GetEnumerator();
            }

            public IEnumerator<IPackageName> GetEnumerator()
            {
                return
                    _versions.Select(cacheEntry => new PackageName(cacheEntry.Name, cacheEntry.SemanticVersion))
                             .Cast<IPackageName>()
                             .GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        private interface ICache : IEnumerable<IPackageName>
        {
            bool IsFilePresent { get; }

            bool IsVersionCache { get; }

            string Root { get; }

            FileAdder Add(string name, SemanticVersion version, IEnumerable<string> dependencys);
            void Remove(string name);


            SemanticVersion GetVersion(string name);

            IEnumerable<CacheEntry> GetEntries();
            IEnumerable<string> GetFiles(string name);
        }

        private class NullCache : ICache
        {
            public bool IsFilePresent
            {
                get { return false; }
            }

            public bool IsVersionCache
            {
                get { return false; }
            }

            public string Root
            {
                get { return String.Empty; }
            }

            public FileAdder Add(string name, SemanticVersion version, IEnumerable<string> dependencys)
            {
                return new NullFileAdder();
            }

            public void Remove(string name)
            {

            }

            public SemanticVersion GetVersion(string name)
            {
                return null;
            }

            public IEnumerable<CacheEntry> GetEntries()
            {
                return Enumerable.Empty<CacheEntry>();
            }

            public IEnumerable<string> GetFiles(string name)
            {
                return Enumerable.Empty<string>();
            }

            public IEnumerator<IPackageName> GetEnumerator()
            {
                return Enumerable.Empty<IPackageName>().GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        private class LocalCache : ICache
        {
            private readonly string _cachePath;
            private readonly bool _createSubdirectory;
            private readonly VersionFileSerializer _versionsFile;

            public LocalCache(string cachePath, bool createSubdirectory = true)
            {
                if (cachePath == null) throw new ArgumentNullException("cachePath");

                _cachePath = cachePath;
                _createSubdirectory = createSubdirectory;
                if (!Directory.Exists(cachePath)) Directory.CreateDirectory(cachePath);

                _versionsFile = new VersionFileSerializer(cachePath);
            }

            public bool IsFilePresent
            {
                get { return _versionsFile.IsFilePresent; }
            }

            public bool IsVersionCache
            {
                get { return false; }
            }

            public string Root
            {
                get { return _cachePath; }
            }

            public FileAdder Add(string name, SemanticVersion version, IEnumerable<string> dependencys)
            {
                CacheEntry entry;
                var entryExist = _versionsFile.TryGetValue(name, out entry);
                var path = entryExist
                    ? entry.Path
                    : _createSubdirectory ? Path.Combine(_cachePath, name) : _cachePath;

                if (entryExist)
                    foreach (var file in entry.Files)
                        File.Delete(file);
                else if (!Directory.Exists(path)) Directory.CreateDirectory(path);

                return new PathFileAdder(path, add =>
                {
                    entry = new CacheEntry
                    {
                        Dependencys = dependencys.ToArray(),
                        Path = Path.Combine(_cachePath, name),
                        SemanticVersion = version,
                        Name = name,
                        Files =  add.Files.ToArray()
                    };

                    _versionsFile[name] = entry;
                    _versionsFile.SaveVersions();
                });
            }

            public void Remove(string name)
            {
                CacheEntry entry;
                if (!_versionsFile.TryGetValue(name, out entry)) return;

                UpdateManager.AddFileToDelete(entry.Files);

                _versionsFile.Remove(name);
            }

            public SemanticVersion GetVersion(string name)
            {
                CacheEntry entry;
                return !_versionsFile.TryGetValue(name, out entry) ? null : entry.SemanticVersion;
            }

            public IEnumerable<CacheEntry> GetEntries()
            {
                return _versionsFile;
            }

            public IEnumerable<string> GetFiles(string name)
            {
                CacheEntry entry;
                if (!_versionsFile.TryGetValue(name, out entry)) return Enumerable.Empty<string>();

                var files = new List<string>();

                foreach (var dependency in entry.Dependencys)
                {
                    files.AddRange(GetFiles(dependency));
                }

                files.AddRange(entry.Files);

                return files;
            }

            public IEnumerator<IPackageName> GetEnumerator()
            {
                return _versionsFile.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        private class VersionCache : ICache
        {
            private readonly VersionFileSerializer _versionFile;

            public bool IsFilePresent
            {
                get { return _versionFile.IsFilePresent; }
            }

            public bool IsVersionCache
            {
                get { return true; }
            }

            public string Root { get; private set; }

            public VersionCache(string cachePath)
            {
                if (cachePath == null) throw new ArgumentNullException("cachePath");
                _versionFile = new VersionFileSerializer(cachePath);
                Root = cachePath;
            }

            public FileAdder Add(string name, SemanticVersion version, IEnumerable<string> dependencys)
            {
                _versionFile[name] = new CacheEntry { Name = name, SemanticVersion = version, Dependencys = dependencys.ToArray() };
                _versionFile.SaveVersions();

                return new NullFileAdder();
            }

            public void Remove(string name)
            {
                _versionFile.Remove(name);
                _versionFile.SaveVersions();
            }

            public SemanticVersion GetVersion(string name)
            {
                CacheEntry ent;
                return _versionFile.TryGetValue(name, out ent) ? ent.SemanticVersion : null;
            }

            public IEnumerable<CacheEntry> GetEntries()
            {
                return _versionFile;
            }

            public IEnumerable<string> GetFiles(string name)
            {
                return Enumerable.Empty<string>();
            }

            public IEnumerator<IPackageName> GetEnumerator()
            {
                return _versionFile.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        private readonly string _targetPath;
        private readonly IPackageRepository _packageRepository;

        private readonly InternalPackageManager _parent;

        public bool IsVersionFilePresent
        {
            get { return Cache.IsFilePresent; }
        }

        private ICache Cache { get; set; }

        public IPackageRepository PackageRepository
        {
            get { return _packageRepository; }
        }

        private InternalPackageManager(string url, string targetPath,
                                      bool keepInCache = false, string cachePath = null, bool useVersionOnly = false)
            : this(
                PackageRepositoryFactory.Default.CreateRepository(url), targetPath, keepInCache, cachePath,
                useVersionOnly)
        {
        }

        private InternalPackageManager(IPackageRepository repository, string targetPath,ICache cache)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            if (cache == null) throw new ArgumentNullException("cache");

            if (targetPath != null && !Directory.Exists(targetPath)) Directory.CreateDirectory(targetPath);

            Cache = cache;

            _targetPath = targetPath;
            _packageRepository = repository;

            if (repository == NugetRepository) return;

            _parent = BuildPackManager();
        }

        private InternalPackageManager(IPackageRepository repository, string targetPath,
                                      bool keepInCache = false, string cachePath = null, bool useVersionOnly = false)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            if (keepInCache && cachePath == null) throw new ArgumentNullException("cachePath");

            if (targetPath != null && !Directory.Exists(targetPath)) Directory.CreateDirectory(targetPath);

            if (keepInCache) Cache = new LocalCache(cachePath);
            // ReSharper disable once AssignNullToNotNullAttribute
            else if (useVersionOnly) Cache = new VersionCache(cachePath);
            else Cache = new NullCache();

            _targetPath = targetPath;
            _packageRepository = repository;

            if (repository == NugetRepository) return;
            
            _parent = BuildPackManager();
        }

        public IEnumerable<CacheEntry> GetEntries()
        {
            return Cache.GetEntries();
        }

// ReSharper disable once UnusedMethodReturnValue.Global
        public IEnumerable<string> Install(string name,bool forceCopy = false)
        {
            var files = new HashSet<string>();

            Install(name, null, files, forceCopy);

            return files;
        }

        public void UnInstall(string name)
        {
            if (name == null) throw new ArgumentNullException("name");

            string[] files = Cache.GetFiles(name).ToArray();
            if (files.Length == 0) return;

            string cacheRoot = Cache.Root;
            if(!string.IsNullOrEmpty(_targetPath))
                UpdateManager.AddFileToDelete(files.Select(s => s.Replace(cacheRoot, _targetPath)));

            Cache.Remove(name);
        }

        private void Install(string name, IVersionSpec spec, HashSet<string> files, bool forceCopy)
        {
            if (name == null) throw new ArgumentNullException("name");

            var pack = PackageRepository.FindPackage(name, spec, null, true, true);
            if (pack == null)
            {
                if(_parent == null)
                    return;

                _parent.Install(name, spec, files, forceCopy);

                return;
            }

            var cacheVer = Cache.GetVersion(name);
            if (cacheVer != null && cacheVer >= pack.Version)
            {
                if (Cache.IsVersionCache)
                {
                    if(_targetPath == null) return;

                    foreach (var packageFile in SelectFiles(pack, SelectCorrectVersion(pack)))
                    {
// ReSharper disable once AssignNullToNotNullAttribute
                        files.Add(Path.Combine(_targetPath, packageFile.EffectivePath));
                    }
                }
                if (files != null) files.AddRange(Cache.GetFiles(name));
                return;
            }

            InstallPackagePrivate(pack, files, forceCopy);
        }

        public void InstallPackage(IPackage package)
        {
            InstallPackagePrivate(package);
        }

        private void InstallPackagePrivate(IPackage pack, HashSet<string> files = null, bool forceCopy = false)
        {
            FrameworkName targetVersion = SelectCorrectVersion(pack);
            if (targetVersion == null) return;

            var framework = MyFrameWorkVersion;
            var depset = new List<PackageDependency>();

            foreach (var packageDependency in pack.GetCompatiblePackageDependencies(framework))
            {
                Install(packageDependency.Id, packageDependency.VersionSpec, files, forceCopy);
                depset.Add(packageDependency);
            }
            var cacheAdder =
                Cache.Add(pack.Id, pack.Version,
                    depset.Select(dep => dep.Id));

            var fileAdder = new AggregareFileAdder(cacheAdder);

            if (!string.IsNullOrWhiteSpace(_targetPath))
                fileAdder.FileAdder.Add(forceCopy
                    ? new PathFileAdder(_targetPath)
                    : UpdateManager.PutUpdate(pack.Id, _targetPath));

            using (fileAdder)
            {
                foreach (var file in SelectFiles(pack, targetVersion))
                {
                    using (var stream = new MemoryStream())
                    {
                        using (var fileStream = file.GetStream())
                        {
                            fileStream.CopyTo(stream);
                        }

                        stream.Position = 0;
                        string newFilePath = fileAdder.AddFile(file.EffectivePath, stream);
                        if (files != null)
                            files.Add(newFilePath);
                    }
                }
            }
        }

        private FrameworkName SelectCorrectVersion(IPackage pack)
        {
            var targetFramework = new FrameworkName(".NETFramework", new Version(4, 5));

            var frameworkNames = pack.GetSupportedFrameworks().ToArray();
            return frameworkNames.OrderByDescending(n => n.Version).FirstOrDefault(n => VersionUtility.IsCompatible(targetFramework, new [] { n }));
        }

        private IEnumerable<IPackageFile> SelectFiles(IPackage pack, FrameworkName targetVersion)
        {
            return
                pack.GetLibFiles()
                    .Where(
                        file =>
                            !file.EffectivePath.Contains("Design") && file.TargetFramework == targetVersion);
        }

        private List<IPackage> _updates;

        public bool CheckForUpdates()
        {
            if (Cache is NullCache) return false;
            _updates = new List<IPackage>();

            foreach (var package in PackageRepository.GetUpdates(Cache, true, false))
            {
                _updates.Add(package);
            }

            var flag = _parent != null && _parent.CheckForUpdates();

            return _updates.Count < 0 || flag;
        }

        public void InstallUpdates()
        {
            foreach (var package in _updates)
            {
                InstallPackagePrivate(package);
            }

            _parent.InstallUpdates();

            _updates.Clear();
        }

        //public void Dispose()
        //{
        //    _downloadPath.DeleteDirectory();

        //}
        public IEnumerable<IPackage> GetAvailableAddIns()
        {
            var installed = new List<string>(Cache.Select(p => p.Id));

            return _packageRepository.GetPackages().Where(p => p.Tags.Contains("AddIn") && !installed.Contains(p.Id));
        }
    }
}
// ReSharper restore CodeAnnotationAnalyzer
