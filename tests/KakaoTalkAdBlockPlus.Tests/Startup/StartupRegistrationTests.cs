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
    }
}
