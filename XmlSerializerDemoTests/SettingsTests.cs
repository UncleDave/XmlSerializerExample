using System;
using System.IO;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XmlSerializerDemo;

namespace XmlSerializerDemoTests
{
    [TestClass]
    public class SettingsTests
    {
        private readonly string filePath = "settings.xml";

        [TestInitialize]
        public void Initialize()
        {
            DeleteFileIfExists();
            SetDefaults();
        }

        [TestMethod]
        public void LoadTest()
        {
            WriteTestFile();
            Settings.Load();

            Assert.AreEqual("2.0.0.0", Settings.LauncherVersion.ToString());
            Assert.AreEqual("2.5", Settings.ModVersion);
        }

        [TestMethod]
        public void SaveShouldCreateNewFileWhenNonExists()
        {
            Settings.Save();

            using (XmlReader reader = XmlReader.Create(filePath))
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
            Settings.LauncherVersion = Version.Parse("5.0.0.5");
            Settings.ModVersion = "Bagels";
            Settings.Save();

            using (XmlReader reader = XmlReader.Create(filePath))
            {
                reader.ReadToFollowing("LauncherVersion");
                Assert.AreEqual("5.0.0.5", reader.ReadElementContentAsString());
                // We don't need to ReadToFollowing the next element as ReadElementContentAsString advances the reader to the next element.
                Assert.AreEqual("Bagels", reader.ReadElementContentAsString());

                reader.Close();
            }
        }

        [TestMethod]
        public void LoadShouldDealWithOutOfDateFile()
        {
            WriteMalformedTestFile();
            Settings.Load();

            Assert.AreEqual("1.0", Settings.ModVersion);
        }

        private void WriteTestFile()
        {
            using (XmlWriter writer = XmlWriter.Create(filePath))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Settings");
                writer.WriteElementString("LauncherVersion", "2.0.0.0");
                writer.WriteElementString("ModVersion", "2.5");
                writer.WriteEndElement();
                writer.WriteEndDocument();

                writer.Close();
            }
        }

        private void WriteMalformedTestFile()
        {
            using (XmlWriter writer = XmlWriter.Create(filePath))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Settings");
                writer.WriteElementString("LauncherVersion", "2.0.0.0");
                writer.WriteEndElement();
                writer.WriteEndDocument();

                writer.Close();
            }
        }

        private void DeleteFileIfExists()
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        private void SetDefaults()
        {
            Settings.LauncherVersion = new Version("1.0.0.0");
            Settings.ModVersion = "1.0";
        }
    }
}