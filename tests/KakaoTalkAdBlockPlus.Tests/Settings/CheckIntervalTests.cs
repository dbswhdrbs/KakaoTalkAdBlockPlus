using KakaoTalkAdBlockPlus.Settings;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KakaoTalkAdBlockPlus.Tests.Settings
{
    [TestClass]
    public class CheckIntervalTests
    {
        [TestMethod]
        public void ShouldDefaultToOriginalHundredMilliseconds()
        {
            Assert.AreEqual(100, CheckInterval.Default.Milliseconds);
        }

        [TestMethod]
        public void ShouldClampBelowMinimumToFiftyMilliseconds()
        {
            Assert.AreEqual(50, CheckInterval.FromMilliseconds(10).Milliseconds);
        }
    }
}
