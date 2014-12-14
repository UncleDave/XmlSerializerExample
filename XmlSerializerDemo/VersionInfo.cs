using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace XmlSerializerDemo
{
    [XmlRoot("Versions")]
    public class VersionInfo
    {
        // The values are stored inside _instance and setters automatically save the XML when a value is changed.

        public static Version LauncherVersion
        {
            get
            {
                return Version.Parse(_instance.SLauncherVersion);
            }
            set
            {
                _instance.SLauncherVersion = value.ToString();
                Save();
            }
        }
        public static string ModVersion
        {
            get
            {
                return _instance.SModVersion;
            }
            set
            {
                _instance.SModVersion = value;
                Save();
            }
        }

        private static VersionInfo _instance = new VersionInfo();
        private static string filePath = "versions.xml";
        private static XmlSerializer serializer = new XmlSerializer(typeof(VersionInfo));

        static VersionInfo()
        {
            Load();
        }

        public static void Load()
        {
            if (!File.Exists(filePath))
            {
                // Defaults, this should probably go elsewhere.
                // Ideally if no file exists get the actual current versions here.
                // Since this is a library we can't really do that.
                // If this class was copy pasta'd into another project however...
                LauncherVersion = new Version("1.0.0.0");
                ModVersion = "1.0";
            }

            using (XmlReader reader = XmlReader.Create(filePath))
            {
                if (!serializer.CanDeserialize(reader))
                {
                    throw new XmlException("Referenced XML file was not in expected format. " + filePath);
                }

                _instance = (VersionInfo)serializer.Deserialize(reader);
                reader.Close();

                LauncherVersion = Version.Parse(_instance.SLauncherVersion);
                ModVersion = _instance.SModVersion;
            }
        }

        public static void Save()
        {
            using (XmlWriter writer = XmlWriter.Create(filePath))
            {
                serializer.Serialize(writer, _instance);
                writer.Close();
            }
        }

        // Non-Static stuff below this line, should never be exposed due to the private constructor, despite being Public.
        // Fields/Properties must be Public and Non-Static to be Serialized.

        private VersionInfo() { }

        [XmlElement("LauncherVersion")]
        public string SLauncherVersion { get; set; }
        [XmlElement("ModVersion")]
        public string SModVersion { get; set; }
    }
}