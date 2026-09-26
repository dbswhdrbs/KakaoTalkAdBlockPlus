using KakaoTalkAdBlockPlus.AdBlock;
using KakaoTalkAdBlockPlus.UI;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KakaoTalkAdBlockPlus.Tests.UI
{
    [TestClass]
    public class StatusTextTests
    {
        [TestMethod]
        public void ShouldDescribeWaitingForKakaoTalk()
        {
            var status = new AdBlockStatus(isKakaoTalkRunning: false, removedAdCount: 0);

            Assert.AreEqual("카카오톡 실행을 기다리는 중", StatusText.Title(status));
            Assert.AreEqual("카카오톡을 켜면 자동으로 광고를 정리해요", StatusText.Detail(status));
        }

        [TestMethod]
        public void ShouldDescribeBlockingWithCount()
        {
            var noAdsYet = new AdBlockStatus(isKakaoTalkRunning: true, removedAdCount: 0);
            var someAds = new AdBlockStatus(isKakaoTalkRunning: true, removedAdCount: 3);

            Assert.AreEqual("광고를 차단하고 있어요", StatusText.Title(noAdsYet));
            Assert.AreEqual("아직 정리한 광고가 없어요", StatusText.Detail(noAdsYet));
            Assert.AreEqual("이번 실행에서 광고 3개를 정리했어요", StatusText.Detail(someAds));
        }
    }
}
