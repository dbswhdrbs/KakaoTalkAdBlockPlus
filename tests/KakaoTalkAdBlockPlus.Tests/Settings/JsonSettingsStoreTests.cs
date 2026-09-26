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
    }
}
