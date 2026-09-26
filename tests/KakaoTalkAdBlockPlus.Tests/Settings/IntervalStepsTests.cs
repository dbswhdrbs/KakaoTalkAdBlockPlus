using KakaoTalkAdBlockPlus.Settings;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KakaoTalkAdBlockPlus.Tests.Settings
{
    [TestClass]
    public class IntervalStepsTests
    {
        [TestMethod]
        public void ShouldReturnStepValueForIndex()
        {
            Assert.AreEqual(50, IntervalSteps.At(0).Milliseconds);
            Assert.AreEqual(100, IntervalSteps.At(1).Milliseconds);
            Assert.AreEqual(60_000, IntervalSteps.At(IntervalSteps.Count - 1).Milliseconds);
        }

        [DataTestMethod]
        [DataRow(50, 0)]
        [DataRow(100, 1)]
        [DataRow(120, 1)]
        [DataRow(180, 2)]
        [DataRow(400, 3)] // 300과 500 사이 한가운데면 짧은 쪽
        [DataRow(60_000, 11)]
        public void ShouldFindNearestStepIndex(int milliseconds, int expectedIndex)
        {
            Assert.AreEqual(expectedIndex, IntervalSteps.IndexOfNearest(CheckInterval.FromMilliseconds(milliseconds)));
        }
    }
}
