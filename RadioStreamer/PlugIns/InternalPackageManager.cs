using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using NuGet;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.PlugIns
{
    [PublicAPI]
    public sealed class InternalPackageManager //: IDisposable
    {
        public static readonly string PluginPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins");
        public static readonly string PackPath = Path.Combine(AppDomain.CurrentDomain.DynamicDirectory, "Packs");

        [Serializable]
        private class CacheEntry
        {
            [NotNull]
            public string Path { get; set; }

            [NotNull]
            public SemanticVersion Version { get; set; }

            [NotNull]
            public string[] Dependencys { get; set; }
        }

        private class VersionFileSerializer : IEnumerable<IPackageName>
        {
            private const string VersionsFileName = "Packages.xml";

            private static readonly XmlSerializer Serializer = new XmlSerializer(
                typeof (Dictionary<string, CacheEntry>), new[] {typeof (CacheEntry)});

            private readonly string _location;

            private readonly Dictionary<string, CacheEntry> _versions;

            public bool IsFilePresent { get { return File.Exists(_location); } }

            public VersionFileSerializer([NotNull] string location)
            {
                _location = Path.Combine(location, VersionsFileName);

                try
                {
                    using (var stream = new FileStream(location, FileMode.Open))
                        _versions = (Dictionary<string, CacheEntry>) Serializer.Deserialize(stream);
                }
                catch (Exception)
                {
                    File.Delete(_location);
                    _versions = new Dictionary<string, CacheEntry>();
                }
            }

            public void SaveVersions()
            {
                using (var stream = new FileStream(_location, FileMode.Create))
                {
                    Serializer.Serialize(stream, _versions);
                }
            }

            public bool TryGetValue([NotNull] string name, out CacheEntry entry)
            {
                if (name == null) throw new ArgumentNullException("name");

                return _versions.TryGetValue(name, out entry);
            }

            public CacheEntry this[string name]
            {
                set { _versions[name] = value; }
            }

            public void Remove([NotNull] string name)
            {
                _versions.Remove(name);
                SaveVersions();
            }

            public IEnumerator<IPackageName> GetEnumerator()
            {
                return
                    _versions.Select(cacheEntry => new PackageName(cacheEntry.Key, cacheEntry.Value.Version))
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

            bool VersionCahce { get; }
            [NotNull]
            string Root { get; }

            void Add([NotNull] string name, [NotNull] IEnumerable<string> files, [NotNull] SemanticVersion version, [NotNull]IEnumerable<string> dependencys);
            void Remove([NotNull] string name);

            [CanBeNull]
            SemanticVersion GetVersion([NotNull] string name);

            [NotNull]
            IEnumerable<string> GetFiles([NotNull] string name);
        }
        private class NullCache : ICache
        {
            public bool IsFilePresent { get { return false; } }
            public bool VersionCahce { get { return false; } }
            public string Root { get { return String.Empty; } }

            public void Add(string name, IEnumerable<string> files, SemanticVersion version, IEnumerable<string> dependencys)
            {
            
            }

            public void Remove(string name)
            {
            
            }

            public SemanticVersion GetVersion(string name)
            {
                return null;
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
            private readonly VersionFileSerializer _versionsFile;

            public LocalCache([NotNull] string cachePath)
            {
                if (cachePath == null) throw new ArgumentNullException("cachePath");
                
                _cachePath = cachePath;
                if (!Directory.Exists(cachePath))
                    Directory.CreateDirectory(cachePath);

                _versionsFile = new VersionFileSerializer(cachePath);
            }

            public bool IsFilePresent { get { return _versionsFile.IsFilePresent; } }
            public bool VersionCahce { get { return false; } }
            public string Root { get { return _cachePath; } }

            public void Add(string name, IEnumerable<string> files, SemanticVersion version, IEnumerable<string> dependencys)
            {
                try
                {
                    CacheEntry entry;
                    var entryExist = _versionsFile.TryGetValue(name, out entry);
                    var path = entryExist ? entry.Path : Path.Combine(_cachePath, name);
                    if (Directory.Exists(path))
                    {
                        foreach (var directory in Directory.EnumerateDirectories(path))
                            Directory.Delete(directory, true);
                        foreach (var file in Directory.EnumerateFiles(path))
                            File.Delete(file);
                    }
                    else
                        Directory.CreateDirectory(path);

                    foreach (var file in files)
                    {
// ReSharper disable once AssignNullToNotNullAttribute
                        string newLoc = file.Replace(Path.GetDirectoryName(file), path);
                        File.Copy(file, newLoc);
                    }
                }
                finally
                {
                    var entry = new CacheEntry { Dependencys = dependencys.ToArray(), Path = Path.Combine(_cachePath, name), Version = version };
                    _versionsFile[name] = entry;
                    _versionsFile.SaveVersions();
                }
            }

            public void Remove(string name)
            {
                CacheEntry entry;
                if (!_versionsFile.TryGetValue(name, out entry)) return;

                Directory.Delete(entry.Path, true);
                _versionsFile.Remove(name);
            }

            public SemanticVersion GetVersion(string name)
            {
                CacheEntry entry;
                return !_versionsFile.TryGetValue(name, out entry) ? null : entry.Version;
            }

            public IEnumerable<string> GetFiles(string name)
            {
                CacheEntry entry;
                if(!_versionsFile.TryGetValue(name, out entry)) return Enumerable.Empty<string>();

                var files = new List<string>();

                foreach (var dependency in entry.Dependencys)
                {
                    files.AddRange(GetFiles(dependency));
                }

                files.AddRange(Directory.EnumerateFiles(entry.Path, "*.*", SearchOption.AllDirectories));

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

            public bool IsFilePresent { get { return _versionFile.IsFilePresent; } }
            public bool VersionCahce { get { return true; } }
            public string Root { get; private set; }

            public VersionCache([NotNull] string cachePath)
            {
                if (cachePath == null) throw new ArgumentNullException("cachePath");
                _versionFile = new VersionFileSerializer(cachePath);
                Root = cachePath;
            }

            public void Add(string name, IEnumerable<string> files, SemanticVersion version, IEnumerable<string> dependencys)
            {
                _versionFile[name] = new CacheEntry { Version = version, Dependencys =  dependencys.ToArray()};
            }

            public void Remove(string name)
            {
             _versionFile.Remove(name);
            }

            public SemanticVersion GetVersion(string name)
            {
                CacheEntry ent;
                return _versionFile.TryGetValue(name, out ent) ? ent.Version : null;
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

        private readonly ICache _cache;

        private readonly string _targetPath;
        private readonly IPackageRepository _packageRepository;

        public bool IsVersionFilePresent { get { return _cache.IsFilePresent; } }

        public InternalPackageManager([NotNull] string url, [CanBeNull] string targetPath,
            bool keepInCache = false, [CanBeNull] string cachePath = null, bool useVersionOnly = false)
            : this(PackageRepositoryFactory.Default.CreateRepository(url), targetPath, keepInCache, cachePath, useVersionOnly)
        { }

        public InternalPackageManager([NotNull] IPackageRepository repository, [CanBeNull] string targetPath,
            bool keepInCache = false, [CanBeNull] string cachePath = null, bool useVersionOnly = false)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            if (keepInCache && cachePath == null) throw new ArgumentNullException("cachePath");

            if (targetPath != null && !Directory.Exists(targetPath))
                Directory.CreateDirectory(targetPath);

            if (keepInCache) _cache = new LocalCache(cachePath);
            // ReSharper disable once AssignNullToNotNullAttribute
            if (useVersionOnly) _cache = new VersionCache(cachePath);
            else _cache = new NullCache();

            _targetPath = targetPath;
            _packageRepository = repository;
        }

        [NotNull]
        public IEnumerable<string> Install([NotNull] string name)
        {
            var files = new List<string>();

            Install(name, null, files);

            return files;
        }

        public void DeInstall([NotNull] string name)
        {
            if (name == null) throw new ArgumentNullException("name");

            string[] files = _cache.GetFiles(name).ToArray();
            if (files.Length == 0) return;

            string root = _cache.Root;
            foreach (var file in files)
            {
                File.Delete(file.Replace(root, _targetPath));
            }

            _cache.Remove(name);
        }

        private void Install([NotNull] string name, [CanBeNull] IVersionSpec spec, [NotNull] List<string> files)
        {
            if (name == null) throw new ArgumentNullException("name");

            var pack = _packageRepository.FindPackage(name, spec, null, true, true);
            if (pack == null) return;

            var cacheVer = _cache.GetVersion(name);
            if (!_cache.VersionCahce && cacheVer != null && cacheVer == pack.Version)
            {
                files.AddRange(_cache.GetFiles(name));
                return;
            }

            InstallPackage(pack, files);
        }

        private void InstallPackage([NotNull] IPackage pack, [NotNull] List<string> files)
        {
            Version[] versions = { new Version(4, 5), new Version(4, 0) };
            var frameworkNames = pack.GetSupportedFrameworks().ToArray();
            Version targetVersion = versions.FirstOrDefault(v => frameworkNames.Any(f => f.Version == v));
            if(targetVersion == null) return;

            var depSet = pack.DependencySets.FirstOrDefault(d => d.TargetFramework.Version == targetVersion);
            if (depSet != null)
            {
                foreach (var packageDependency in depSet.Dependencies)
                {
                    Install(packageDependency.Id, packageDependency.VersionSpec, files);
                }
            }

            string path = Path.Combine(Path.GetTempPath(),"Tauron");
            var tempfiles = new List<string>();
            string downlodPath = Path.Combine(path, "RSPM\\Downloads");

            try
            {
                if (!Directory.Exists(downlodPath))
                    Directory.CreateDirectory(downlodPath);

                foreach (var file in pack.GetLibFiles().Where(file => !file.EffectivePath.Contains("Design")))
                {
                    string copyPath = Path.Combine(downlodPath, pack.Id, file.EffectivePath);

                    using (var fileStream = new FileStream(copyPath, FileMode.Create))
                    {
                        using (var sourceStream = file.GetStream())
                        {
                            sourceStream.CopyTo(fileStream);
                        }
                    }

                    tempfiles.Add(copyPath);
                }

                _cache.Add(pack.Id, tempfiles, pack.Version,
                    depSet != null ? depSet.Dependencies.Select(dep => dep.Id) : Enumerable.Empty<string>());

                if (_targetPath != null)
                {
                    UpdateManager.PutUpdate(pack.Id, _targetPath, updatePath =>
                    {

                        if (!Directory.Exists(_targetPath))
                            Directory.Exists(_targetPath);
                        string copyPath = Path.Combine(downlodPath, pack.Id);

                        foreach (var tempfile in tempfiles)
                        {
                            var targetPath = tempfile.Replace(copyPath, updatePath);
                            files.Add(targetPath);
                            File.Move(tempfile, targetPath);
                        }
                    });
                }
                else
                    files.AddRange(_cache.GetFiles(pack.Id));
            }
            finally
            {
                Directory.Delete(path, true);
            }
        }

        public void CheckForUpdates()
        {
            if (_cache is NullCache) return;
            var files = new List<string>();

            foreach (var package in _packageRepository.GetUpdates(_cache, true, false))
            {
                InstallPackage(package, files);
            }
        }

        //public void Dispose()
        //{
        //    _downloadPath.DeleteDirectory();
            
        //}
    }
}
