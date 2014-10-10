using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Tauron.Application.RadioStreamer.Contracts.Data;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Database.Database.Formats
{
    public static class XmlSerializeHelper
    {
        private const string Extension = ".xml";

        [NotNull]
        public static IEnumerable<string> FindAllNames([NotNull] string location)
        {
            if (location == null) throw new ArgumentNullException("location");
            location.CreateDirectoryIfNotExis();

            return location.GetFiles().Select(s => s.GetFileNameWithoutExtension());
        }

        [CanBeNull]
        public static CommonProfile DeserializeProfile([NotNull] string name, [NotNull] string location)
        {
            if (name == null) throw new ArgumentNullException("name");
            if (location == null) throw new ArgumentNullException("location");

            string fullPath = location.CombinePath(name + Extension);

            if (!fullPath.ExisFile()) return null;

            try
            {
                XElement ele = XElement.Load(fullPath);
                var xElement = ele.Element("ID");
                CommonProfile profile = null;
                if (xElement != null)
                    profile = new CommonProfile(xElement.Value);

                if (profile == null) return null;

                foreach (var element in ele.Elements("Property"))
                {
                    string key = string.Empty;

                    var attr = element.Attribute("Key");
                    if (attr != null)
                        key = attr.Value;

                    profile.Properties.Add(new DataProperty(key, element.Value));
                }

                return profile;
            }
            catch (Exception e)
            {
                if (CriticalExceptions.IsCriticalException(e))
                    throw;

                return null;
            }
        }

        public static void Serialize([NotNull] string name, [NotNull] string location, [NotNull] CommonProfile profile)
        {
            if (name == null) throw new ArgumentNullException("name");
            if (location == null) throw new ArgumentNullException("location");
            if (profile == null) throw new ArgumentNullException("profile");

            var ele = new XElement("Profile", new XAttribute("ID", profile.Id));

            foreach (var dataProperty in profile.Properties)
                ele.Add(new XElement("Property", new XAttribute("Key", dataProperty.Key), dataProperty.Value));

            string fullPath = location.CombinePath(name + Extension);
            fullPath.CreateDirectoryIfNotExis();
            ele.Save(fullPath);
        }
    }
}