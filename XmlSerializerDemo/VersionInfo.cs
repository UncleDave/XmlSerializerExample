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
        // Getters load the file every time, this may not be desirable in some cases, especially with large files.
        // In that case make Load and Save Public.

        public static Version LauncherVersion
        {
            get
            {
                Load();
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
                Load();
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
            EnsureFileExists();
        }

        private static void EnsureFileExists()
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
        }

        private static void Load()
        {
            EnsureFileExists();

            using (XmlReader reader = XmlReader.Create(filePath))
            {
                if (!serializer.CanDeserialize(reader))
                {
                    throw new XmlException("Referenced XML file was not in expected format. " + filePath);
                }

                _instance = (VersionInfo)serializer.Deserialize(reader);
                reader.Close();
            }
        }

        private static void Save()
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
