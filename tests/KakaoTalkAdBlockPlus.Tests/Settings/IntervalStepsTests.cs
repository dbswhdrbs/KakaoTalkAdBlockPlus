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
    }
}
