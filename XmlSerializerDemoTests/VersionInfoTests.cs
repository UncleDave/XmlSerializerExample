using System;
using System.IO;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XmlSerializerDemo;

namespace XmlSerializerDemoTests
{
    [TestClass]
    public class VersionInfoTests
    {
        // Tests will break when filePath is changed because I'm lazy.
        // Any project using this won't really need the tests anyway.

        [TestInitialize]
        public void Initialize()
        {
            if (File.Exists("versions.xml"))
            {
                File.Delete("versions.xml");
            }
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (File.Exists("versions.xml"))
            {
                File.Delete("versions.xml");
            }
        }

        [TestMethod]
        public void LoadTest()
        {
            using (XmlWriter writer = XmlWriter.Create("versions.xml"))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Versions");
                writer.WriteElementString("LauncherVersion", "2.0.0.0");
                writer.WriteElementString("ModVersion", "2.5");
                writer.WriteEndElement();
                writer.WriteEndDocument();

                writer.Close();
            }

            Assert.AreEqual("2.0.0.0", VersionInfo.LauncherVersion.ToString());
            Assert.AreEqual("2.5", VersionInfo.ModVersion);
        }

        [TestMethod]
        public void SaveShouldCreateNewFileWhenNonExists()
        {
            // Static initializers aren't triggered until the class is needed.
            string trash = VersionInfo.ModVersion;

            using (XmlReader reader = XmlReader.Create("versions.xml"))
            {
                reader.ReadToFollowing("LauncherVersion");
                Assert.AreEqual("1.0.0.0", reader.ReadElementContentAsString());
                // We don't need to ReadToFollowing the next element as ReadElementContentAsString advances the reader to the next element.
                Assert.AreEqual("1.0", reader.ReadElementContentAsString());

                reader.Close();
            }
        }

        [TestMethod]
        public void SaveShouldUpdateExistingFile()
        {
            VersionInfo.LauncherVersion = Version.Parse("5.0.0.5");
            VersionInfo.ModVersion = "Bagels";

            using (XmlReader reader = XmlReader.Create("versions.xml"))
            {
                reader.ReadToFollowing("LauncherVersion");
                Assert.AreEqual("5.0.0.5", reader.ReadElementContentAsString());
                // We don't need to ReadToFollowing the next element as ReadElementContentAsString advances the reader to the next element.
                Assert.AreEqual("Bagels", reader.ReadElementContentAsString());

                reader.Close();
            }
        }
    }
}