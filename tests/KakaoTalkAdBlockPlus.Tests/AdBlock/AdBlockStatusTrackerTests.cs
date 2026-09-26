using System;
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

        [TestMethod]
        public void ShouldAccumulateRemovedAds()
        {
            var tracker = new AdBlockStatusTracker();

            tracker.Apply(new AdBlockReport(isKakaoTalkRunning: true, new[] { new IntPtr(1) }));
            tracker.Apply(new AdBlockReport(isKakaoTalkRunning: true, new[] { new IntPtr(2), new IntPtr(3) }));

            Assert.IsTrue(tracker.Current.IsKakaoTalkRunning);
            Assert.AreEqual(3, tracker.Current.RemovedAdCount);
        }
    }
}
