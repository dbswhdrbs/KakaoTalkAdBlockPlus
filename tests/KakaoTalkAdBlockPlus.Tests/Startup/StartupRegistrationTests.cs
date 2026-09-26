using KakaoTalkAdBlockPlus.Startup;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KakaoTalkAdBlockPlus.Tests.Startup
{
    [TestClass]
    public class StartupRegistrationTests
    {
        private const string AppName = "KakaoTalkAdBlockPlus";
        private const string ExecutablePath = @"C:\Apps\KakaoTalkAdBlockPlus\KakaoTalkAdBlockPlus.exe";

        private readonly FakeRegistryStore _registry = new FakeRegistryStore();

        private StartupRegistration CreateRegistration() => new StartupRegistration(_registry, AppName, ExecutablePath);

        [TestMethod]
        public void ShouldBeDisabledWhenRunValueIsMissing()
        {
            Assert.IsFalse(CreateRegistration().IsEnabled);
        }

        [TestMethod]
        public void ShouldWriteQuotedPathWithAutostartArgumentWhenEnabled()
        {
            CreateRegistration().Enable();

            Assert.AreEqual(
                "\"" + ExecutablePath + "\" --autostart",
                _registry.GetString(@"Software\Microsoft\Windows\CurrentVersion\Run", AppName));
        }

        [TestMethod]
        public void ShouldBeEnabledAfterEnable()
        {
            var registration = CreateRegistration();

            registration.Enable();

            Assert.IsTrue(registration.IsEnabled);
        }

        [TestMethod]
        public void ShouldDeleteRunValueWhenDisabled()
        {
            var registration = CreateRegistration();
            registration.Enable();

            registration.Disable();

            Assert.IsNull(_registry.GetString(StartupRegistration.RunKeyPath, AppName));
            Assert.IsFalse(registration.IsEnabled);
        }
    }
}
