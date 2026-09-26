using KakaoTalkAdBlockPlus.Startup;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KakaoTalkAdBlockPlus.Tests.Startup
{
    [TestClass]
    public class StartupRegistrationTests
    {
        private const string AppName = "KakaoTalkAdBlockPlus";
        private const string ExecutablePath = @"C:\Apps\KakaoTalkAdBlockPlus\KakaoTalkAdBlockPlus.exe";
        private const string StartupApprovedKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\Run";

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

        [TestMethod]
        public void ShouldBeDisabledWhenTaskManagerDisabledIt()
        {
            var registration = CreateRegistration();
            registration.Enable();

            _registry.SetBinary(StartupApprovedKeyPath, AppName, TaskManagerFlag(0x03));

            Assert.IsFalse(registration.IsEnabled);
        }

        [TestMethod]
        public void ShouldStayEnabledWhenTaskManagerEnabledIt()
        {
            var registration = CreateRegistration();
            registration.Enable();

            _registry.SetBinary(StartupApprovedKeyPath, AppName, TaskManagerFlag(0x02));

            Assert.IsTrue(registration.IsEnabled);
        }

        /// <summary>작업 관리자가 기록하는 12바이트 값: 첫 바이트가 상태, 나머지는 시각.</summary>
        private static byte[] TaskManagerFlag(byte state) => new byte[] { state, 0, 0, 0, 0x5B, 0x2E, 0x1F, 0x83, 0x3A, 0x9D, 0xDA, 0x01 };
    }
}
