using System;
using System.IO;
using System.Security;
using System.Xml;
using System.Xml.Serialization;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.PlugIns
{
    public static class UpdateManager
    {
        private const string UpdatePath = "Updates";
        private const string UpdateManifestFile = "manifest.conf";

        public class UpdateManifest
        {
            public string TargetLocation { get; set; } 
        }

        private static readonly XmlSerializer Serializer = new XmlSerializer(typeof(UpdateManifest));

        [NotNull]
        public static string FullUpdatePath 
        {
            get
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, UpdatePath);
            } 
        }

        public static void InstallUpdates()
        {
            if(!Directory.Exists(FullUpdatePath)) return;

            foreach (var directory in Directory.EnumerateDirectories(UpdatePath))
            {
                try
                {
                    string manifestPath = Path.Combine(directory, UpdateManifestFile);
                    UpdateManifest manifest;

                    using (var stream = new FileStream(manifestPath, FileMode.Open))
                        manifest = (UpdateManifest) Serializer.Deserialize(stream);

                    File.Delete(manifestPath);

                    foreach (var file in Directory.EnumerateFiles(directory, "*.*", SearchOption.AllDirectories))
                    {
                        var targetPath = file.Replace(directory, manifest.TargetLocation);

                        string copydic = Path.GetDirectoryName(targetPath);
                        if (!Directory.Exists(copydic)) Directory.CreateDirectory(copydic);

                        File.Copy(file, targetPath, true);
                    }
                }
                catch (InvalidOperationException) { }
                catch(IOException){ }
                catch(UnauthorizedAccessException) { }
                catch(XmlException) { }
                catch(SecurityException) { }
                catch(ArgumentException) { }
            }

            Directory.Delete(FullUpdatePath, true);
        }

        public static void PutUpdate([NotNull] string name, [NotNull] string targetPath,
            [NotNull] Action<string> copyFiles)
        {
            if (name == null) throw new ArgumentNullException("name");
            if (targetPath == null) throw new ArgumentNullException("targetPath");
            if (copyFiles == null) throw new ArgumentNullException("copyFiles");

            string updateLocation = Path.Combine(FullUpdatePath, name);

            Directory.CreateDirectory(updateLocation);

            copyFiles(updateLocation);

            using (var stream = new FileStream(Path.Combine(updateLocation, UpdateManifestFile), FileMode.Create))
                Serializer.Serialize(stream, new UpdateManifest {TargetLocation = targetPath});
        }
    }
}