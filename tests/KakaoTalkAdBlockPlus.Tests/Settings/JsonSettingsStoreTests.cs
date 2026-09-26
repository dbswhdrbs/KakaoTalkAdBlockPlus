using System;
using System.IO;
using KakaoTalkAdBlockPlus.Settings;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KakaoTalkAdBlockPlus.Tests.Settings
{
    [TestClass]
    public class JsonSettingsStoreTests
    {
        private string _directory = string.Empty;

        private string SettingsPath => Path.Combine(_directory, "settings.json");

        [TestInitialize]
        public void CreateTemporaryDirectory()
        {
            _directory = Path.Combine(Path.GetTempPath(), "KakaoTalkAdBlockPlus.Tests", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_directory);
        }

        [TestCleanup]
        public void DeleteTemporaryDirectory()
        {
            if (Directory.Exists(_directory)) Directory.Delete(_directory, recursive: true);
        }

        [TestMethod]
        public void ShouldLoadDefaultsWhenFileDoesNotExist()
        {
            var settings = new JsonSettingsStore(SettingsPath).Load();

            Assert.AreEqual(CheckInterval.DefaultMilliseconds, settings.CheckInterval.Milliseconds);
        }

        [TestMethod]
        public void ShouldRoundTripCheckInterval()
        {
            new JsonSettingsStore(SettingsPath).Save(new AppSettings(CheckInterval.FromMilliseconds(750)));

            var loaded = new JsonSettingsStore(SettingsPath).Load();

            Assert.AreEqual(750, loaded.CheckInterval.Milliseconds);
        }

        [TestMethod]
        public void ShouldCreateDirectoryWhenSaving()
        {
            var path = Path.Combine(_directory, "not", "yet", "created", "settings.json");

            new JsonSettingsStore(path).Save(new AppSettings(CheckInterval.FromMilliseconds(300)));

            Assert.AreEqual(300, new JsonSettingsStore(path).Load().CheckInterval.Milliseconds);
        }

        [DataTestMethod]
        [DataRow("{ this is not json")]
        [DataRow("")]
        [DataRow("{ \"checkIntervalMs\": \"fast\" }")]
        public void ShouldLoadDefaultsWhenFileIsCorrupted(string content)
        {
            File.WriteAllText(SettingsPath, content);

            var settings = new JsonSettingsStore(SettingsPath).Load();

            Assert.AreEqual(CheckInterval.DefaultMilliseconds, settings.CheckInterval.Milliseconds);
        }
    }
}
