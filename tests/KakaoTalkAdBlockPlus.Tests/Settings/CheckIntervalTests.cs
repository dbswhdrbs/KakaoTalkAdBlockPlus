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

        [TestMethod]
        public void ShouldClampAboveMaximumToSixtySeconds()
        {
            Assert.AreEqual(60_000, CheckInterval.FromMilliseconds(90_000).Milliseconds);
        }

        [DataTestMethod]
        [DataRow(100, "0.1")]
        [DataRow(50, "0.05")]
        [DataRow(125, "0.125")]
        [DataRow(1500, "1.5")]
        [DataRow(60_000, "60")]
        public void ShouldFormatAsSecondsText(int milliseconds, string expected)
        {
            Assert.AreEqual(expected, CheckInterval.FromMilliseconds(milliseconds).ToSecondsText());
        }
    }
}
