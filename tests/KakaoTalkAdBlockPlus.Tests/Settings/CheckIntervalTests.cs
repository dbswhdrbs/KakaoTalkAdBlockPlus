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

        [TestMethod]
        public void ShouldParseSecondsText()
        {
            var result = CheckInterval.ParseSeconds("0.25");

            Assert.AreEqual(IntervalParseStatus.Ok, result.Status);
            Assert.AreEqual(250, result.Interval.Milliseconds);
        }

        [DataTestMethod]
        [DataRow("0,5", 500)]
        [DataRow(" 1.5초 ", 1500)]
        [DataRow("0.3 초", 300)]
        [DataRow("2s", 2000)]
        [DataRow("1 S", 1000)]
        public void ShouldParseCommaDecimalAndUnitSuffix(string text, int expectedMilliseconds)
        {
            var result = CheckInterval.ParseSeconds(text);

            Assert.AreEqual(IntervalParseStatus.Ok, result.Status);
            Assert.AreEqual(expectedMilliseconds, result.Interval.Milliseconds);
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        [DataRow("초")]
        public void ShouldRejectEmptyText(string? text)
        {
            Assert.AreEqual(IntervalParseStatus.Empty, CheckInterval.ParseSeconds(text).Status);
        }
    }
}
