using System;
using KakaoTalkAdBlockPlus.Startup;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Win32;

namespace KakaoTalkAdBlockPlus.Tests.Startup
{
    /// <summary>HKCU\Software\KakaoTalkAdBlockPlus.Tests\{임시} 아래에서만 읽고 쓴다.</summary>
    [TestClass]
    public class CurrentUserRegistryStoreTests
    {
        private const string TestRootPath = @"Software\KakaoTalkAdBlockPlus.Tests";

        private readonly CurrentUserRegistryStore _store = new CurrentUserRegistryStore();
        private string _keyPath = string.Empty;

        [TestInitialize]
        public void ChooseTemporaryKey()
        {
            _keyPath = TestRootPath + @"\" + Guid.NewGuid().ToString("N");
        }

        [TestCleanup]
        public void DeleteTemporaryKeys()
        {
            Registry.CurrentUser.DeleteSubKeyTree(TestRootPath, throwOnMissingSubKey: false);
        }

        [TestMethod]
        public void ShouldWriteReadAndDeleteStringValue()
        {
            _store.SetString(_keyPath, "App", "\"C:\\경로\\앱.exe\" --autostart");
            Assert.AreEqual("\"C:\\경로\\앱.exe\" --autostart", _store.GetString(_keyPath, "App"));

            _store.DeleteValue(_keyPath, "App");
            Assert.IsNull(_store.GetString(_keyPath, "App"));
        }

        [TestMethod]
        public void ShouldReturnNullForMissingValue()
        {
            _store.SetString(_keyPath, "Other", "value");

            Assert.IsNull(_store.GetString(_keyPath + @"\Missing", "App"));
            Assert.IsNull(_store.GetString(_keyPath, "App"));
            Assert.IsNull(_store.GetBinary(_keyPath, "App"));
            _store.DeleteValue(_keyPath + @"\Missing", "App");
            _store.DeleteValue(_keyPath, "App");
        }

        [TestMethod]
        public void ShouldReadBinaryValue()
        {
            using (var key = Registry.CurrentUser.CreateSubKey(_keyPath))
            {
                key.SetValue("App", new byte[] { 0x03, 0x00, 0x01 }, RegistryValueKind.Binary);
            }

            CollectionAssert.AreEqual(new byte[] { 0x03, 0x00, 0x01 }, _store.GetBinary(_keyPath, "App"));
            Assert.IsNull(_store.GetString(_keyPath, "App"));
        }
    }
}
