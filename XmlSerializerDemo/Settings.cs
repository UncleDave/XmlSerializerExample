using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace XmlSerializerDemo
{
    [XmlRoot("Settings")]
    public class Settings
    {
        public static Version LauncherVersion
        {
            get
            {
                return new Version(_instance.SLauncherVersion);
            }
            set
            {
                _instance.SLauncherVersion = value.ToString();
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
            }
        }

        // To add a new value add another Property like the ones above. Example:
        // If the type you want to store is not serializable it needs to be converted to something that is.
        //
        //public static string YourNewProperty
        //{
        //    get
        //    {
        //        Load();
        //        return _instance.YourNewProperty; If your type is not serializable do some processing here before returning it in the desired format.
        //    }
        //    set
        //    {
        //        _instance.YourNewProperty = value; If your type is not serializable do some processing here before saving it in a serialzable format.
        //        Save();
        //    }
        //}

        private static Settings _instance = new Settings();
        private const string filePath = "settings.xml";
        private static readonly XmlSerializer serializer = new XmlSerializer(typeof(Settings));

        // Defaults
        private static readonly Version defaultLauncherVersion = new Version("1.0.0.0");
        private const string defaultModVersion = "1.0";
        //private static string defaultYourNewProperty = "Hello";

        static Settings()
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
                LauncherVersion = defaultLauncherVersion;
                ModVersion = defaultModVersion;
                // YourNewProperty = defaultYourNewProperty;
            }
        }

        private static void Validate()
        {
            if (_instance.SLauncherVersion == null)
            {
                LauncherVersion = defaultLauncherVersion;
            }

            if (_instance.SModVersion == null)
            {
                ModVersion = defaultModVersion;
            }

            //if (YourNewProperty == String.Empty)
            //{
            //    YourNewProperty = defaultYourNewProperty;
            //}

            // Validate what is read, check for nulls, and whatever you wouldn't expect to have.
            // This should sort out versioning.
            // Always check the instance's properties rather than the static properties.
        }

        public static void Load()
        {
            EnsureFileExists();

            using (XmlReader reader = XmlReader.Create(filePath))
            {
                if (!serializer.CanDeserialize(reader))
                {
                    throw new XmlException("Referenced XML file was not in expected format. " + filePath);
                }

                _instance = (Settings)serializer.Deserialize(reader);
                reader.Close();
            }

            Validate();
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
        private Settings() { }

        [XmlElement("LauncherVersion")]
        public string SLauncherVersion { get; set; }
        [XmlElement("ModVersion")]
        public string SModVersion { get; set; }
        //[XmlElement("YourNewProperty")]
        //public string SYourNewProperty { get; set; }

        // This is your new property is serializable form.
    }
}