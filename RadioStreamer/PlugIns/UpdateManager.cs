using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Xml;
using System.Xml.Serialization;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.PlugIns
{
    public static class UpdateManager
    {
        private const string UpdatePath = "Updates";
        private const string DeletePath = "Delete";
        private const string UpdateManifestFile = "manifest.conf";

        public class UpdateManifest
        {
            public string TargetLocation { get; set; } 
        }

        private static readonly XmlSerializer InstallSerializer = new XmlSerializer(typeof(UpdateManifest));
        private static readonly XmlSerializer DeleteSerializer = new XmlSerializer(typeof (List<string>));

        private static string _fullUpdatePath;
        private static string _fullDeletePath;

        [NotNull]
        public static string FullUpdatePath
        {
            private get { return _fullUpdatePath ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, UpdatePath); }
            set { _fullUpdatePath = Path.Combine(value, UpdatePath); }
        }

        [NotNull]
        public static string FullDeletePath
        {
            private get { return _fullDeletePath ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DeletePath); }
            set { _fullDeletePath = Path.Combine(value, DeletePath); }
        }

        public static void InstallUpdates()
        {
            if (Directory.Exists(FullUpdatePath))
            {
                foreach (var directory in Directory.EnumerateDirectories(UpdatePath))
                {
                    try
                    {
                        string manifestPath = Path.Combine(directory, UpdateManifestFile);
                        UpdateManifest manifest;

                        using (var stream = new FileStream(manifestPath, FileMode.Open))
                            manifest = (UpdateManifest) InstallSerializer.Deserialize(stream);

                        File.Delete(manifestPath);

                        foreach (var file in Directory.EnumerateFiles(directory, "*.*", SearchOption.AllDirectories))
                        {
                            var targetPath = file.Replace(directory, manifest.TargetLocation);

                            string copydic = Path.GetDirectoryName(targetPath);
                            if (copydic != null && !Directory.Exists(copydic)) Directory.CreateDirectory(copydic);

                            File.Copy(file, targetPath, true);
                        }
                    }
                    catch (InvalidOperationException)
                    {
                    }
                    catch (IOException)
                    {
                    }
                    catch (UnauthorizedAccessException)
                    {
                    }
                    catch (XmlException)
                    {
                    }
                    catch (SecurityException)
                    {
                    }
                    catch (ArgumentException)
                    {
                    }
                }

                Directory.Delete(FullUpdatePath, true);
            }

            if(!Directory.Exists(FullDeletePath)) return;

            try
            {
                using (var stream = new FileStream(Path.Combine(FullDeletePath, UpdateManifestFile), FileMode.Open))
                {
                    var files = (List<string>) DeleteSerializer.Deserialize(stream);

                    foreach (var file in files)
                    {
                        var info = new FileInfo(file);
                        var dic = info.Directory;

                        info.Delete();
                            
                        if(dic != null && dic.EnumerateFileSystemInfos().FirstOrDefault() == null)
                            dic.Delete();
                    }
                }
            }
            catch (IOException)
            {
            }
            catch (XmlException)
            {
            }
            finally
            {
                Directory.Delete(FullDeletePath, true);
            }
        }

        public static FileAdder PutUpdate([NotNull] string name, [NotNull] string targetPath)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (targetPath == null) throw new ArgumentNullException(nameof(targetPath));

            string updateLocation = Path.Combine(FullUpdatePath, name);

            Directory.CreateDirectory(updateLocation);
            
            using (var stream = new FileStream(Path.Combine(updateLocation, UpdateManifestFile), FileMode.Create))
                InstallSerializer.Serialize(stream, new UpdateManifest {TargetLocation = targetPath});

            return new PathFileAdder(updateLocation);
        }

        public static void AddFileToDelete(IEnumerable<string> files)
        {
            string path = Path.Combine(FullUpdatePath, UpdateManifestFile);

            List<string> filesList = null;

            if (File.Exists(path))
            {
                try
                {
                    using (var stream = new FileStream(path, FileMode.Open))
                    {
                        filesList = (List<string>) DeleteSerializer.Deserialize(stream);
                    }
                }
                finally
                {
                    if (filesList == null)
                    {
                        filesList = new List<string>();
                        File.Delete(path);
                    }
                }
            }
            else 
                filesList = new List<string>();

            filesList.AddRange(files);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                DeleteSerializer.Serialize(stream, filesList);
            }
        }
    }
}