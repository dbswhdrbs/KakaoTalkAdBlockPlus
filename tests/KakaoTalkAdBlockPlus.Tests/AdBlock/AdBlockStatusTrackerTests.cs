using KakaoTalkAdBlockPlus.AdBlock;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KakaoTalkAdBlockPlus.Tests.AdBlock
{
    [TestClass]
    public class AdBlockStatusTrackerTests
    {
        [TestMethod]
        public void ShouldStartWithNothingRemoved()
        {
            var status = new AdBlockStatusTracker().Current;

            Assert.IsFalse(status.IsKakaoTalkRunning);
            Assert.AreEqual(0, status.RemovedAdCount);
        }
    }
}
