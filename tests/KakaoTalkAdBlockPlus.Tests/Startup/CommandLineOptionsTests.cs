using KakaoTalkAdBlockPlus.Startup;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KakaoTalkAdBlockPlus.Tests.Startup
{
    [TestClass]
    public class CommandLineOptionsTests
    {
        [DataTestMethod]
        [DataRow("--autostart")]
        [DataRow("--AutoStart")]
        public void ShouldDetectAutostartFlag(string argument)
        {
            Assert.IsTrue(CommandLineOptions.Parse(new[] { argument }).IsAutostart);
        }
    }
}
