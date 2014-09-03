using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NuGet;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.PlugIns
{
    [PublicAPI]
    public sealed class InternalPackageManager //: IDisposable
    {
        private interface ICache
        {
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
            public string Root { get { return string.Empty; } }

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
        }
        private class LocalCache : ICache
        {
            private const string VersionsFileName = "Packages.bin";

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

            private readonly string _cachePath;
            private readonly string _versionPath;
            private readonly Dictionary<string, CacheEntry> _versions;

            public LocalCache([NotNull] string cachePath)
            {
                if (cachePath == null) throw new ArgumentNullException("cachePath");
                
                _cachePath = cachePath;
                _cachePath.CreateDirectoryIfNotExis();

                _versionPath = _cachePath.CombinePath(VersionsFileName);

                try
                {
                    _versions = _versionPath.Deserialize<Dictionary<string, CacheEntry>>();
                }
                catch (Exception)
                {
                    _versionPath.DeleteFile();
                    _versions = new Dictionary<string, CacheEntry>();
                }
            }

            private void SaveVersions()
            {
                _versionPath.DeleteFile();
                _versions.Serialize(_cachePath);
            }

            public string Root { get { return _cachePath; } }

            public void Add(string name, IEnumerable<string> files, SemanticVersion version, IEnumerable<string> dependencys)
            {
                try
                {
                    CacheEntry entry;
                    if (_versions.TryGetValue(name, out entry))
                    {
                        var path = entry.Path;
                        path.ClearDirectory();
                        path.CreateDirectoryIfNotExis();

                        foreach (var file in files)
                        {
                            file.CopyFileTo(path);
                        }
                        
                        return;
                    }

                    string newPath = _cachePath.CombinePath(name);
                    newPath.CreateDirectoryIfNotExis();

                    foreach (var file in files)
                    {
                        file.CopyFileTo(newPath);
                    }
                }
                finally
                {
                    var entry = new CacheEntry { Dependencys = dependencys.ToArray(), Path = _cachePath.CombinePath(name), Version = version};
                    _versions[name] = entry;
                    SaveVersions();
                }
            }

            public void Remove(string name)
            {
                CacheEntry entry;
                if (!_versions.TryGetValue(name, out entry)) return;

                entry.Path.DeleteDirectory();
                _versions.Remove(name);
            }

            public SemanticVersion GetVersion(string name)
            {
                CacheEntry entry;
                return !_versions.TryGetValue(name, out entry) ? null : entry.Version;
            }

            public IEnumerable<string> GetFiles(string name)
            {
                CacheEntry entry;
                if(!_versions.TryGetValue(name, out entry)) return Enumerable.Empty<string>();

                var files = new List<string>();

                foreach (var dependency in entry.Dependencys)
                {
                    files.AddRange(GetFiles(dependency));
                }

                files.AddRange(entry.Path.EnumerateAllFiles());

                return files;
            }
        }

        private readonly ICache _cache;

        private readonly string _targetPath;
        private readonly IPackageRepository _packageRepository;

        public InternalPackageManager([NotNull] string url, [CanBeNull] string targetPath,
            bool keepInCache = false, [NotNull] string cachePath = null)
        {
            if (url == null) throw new ArgumentNullException("url");
            if (keepInCache && cachePath == null) throw new ArgumentNullException("cachePath");

            targetPath.CreateDirectoryIfNotExis();

            if (keepInCache) _cache = new LocalCache(cachePath);
            else _cache = new NullCache();

            _targetPath = targetPath;
            _packageRepository = PackageRepositoryFactory.Default.CreateRepository(url);
        }

        [NotNull]
        public IEnumerable<string> Install([NotNull] string name)
        {
            var files = new List<string>();

            Install(name, null, files);

            return files;
        }

        public void DeInstall(string name)
        {
            string[] files = _cache.GetFiles(name).ToArray();
            if(files.Length == 0) return;

            string root = _cache.Root;
            foreach (var file in files)
            {
                file.Replace(root, _targetPath).DeleteFile();
            }

            _cache.Remove(name);
        }
 
       private void Install([NotNull] string name, [CanBeNull] IVersionSpec spec, [NotNull] List<string> files)
        {
            if (name == null) throw new ArgumentNullException("name");

            var pack = _packageRepository.FindPackage(name, spec, null, true, true);
            if (pack == null) return;

            var cacheVer = _cache.GetVersion(name);
            if (cacheVer != null && cacheVer == pack.Version)
            {
                files.AddRange(_cache.GetFiles(name));
            }

            Version[] versions = {new Version(4, 5), new Version(4, 0)};
            var frameworkNames = pack.GetSupportedFrameworks().ToArray();
            Version targetVersion = versions.FirstOrDefault(v => frameworkNames.Any(f => f.Version == v));

            var depSet = pack.DependencySets.FirstOrDefault(d => d.TargetFramework.Version == targetVersion);
            if (depSet != null)
            {
                foreach (var packageDependency in depSet.Dependencies)
                {
                    Install(packageDependency.Id, packageDependency.VersionSpec, files);
                }
            }

            string path = Path.GetTempPath().CombinePath("Tauron");
            var tempfiles = new List<string>();
            string downlodPath = path.CombinePath("RSPM\\Downloads");

            try
            {
                downlodPath.CreateDirectoryIfNotExis();

                foreach (var file in pack.GetLibFiles().Where(file => !file.EffectivePath.Contains("Design")))
                {
                    string copyPath = downlodPath.CombinePath(pack.Id, file.EffectivePath);

                    using (var fileStream = copyPath.OpenWrite())
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
                    _targetPath.CreateDirectoryIfNotExis();
                    string copyPath = downlodPath.CombinePath(pack.Id);

                    foreach (var tempfile in tempfiles)
                    {
                        var targetPath = tempfile.Replace(copyPath, _targetPath);
                        files.Add(targetPath);
                        tempfile.MoveTo(targetPath);
                    }
                }
                else 
                    files.AddRange(_cache.GetFiles(pack.Id));
            }
            finally
            {
                path.DeleteDirectory();
            }
        }

        //public void Dispose()
        //{
        //    _downloadPath.DeleteDirectory();
            
        //}
    }
}
