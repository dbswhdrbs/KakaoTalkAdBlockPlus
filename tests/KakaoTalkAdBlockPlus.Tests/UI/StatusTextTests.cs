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
    }
}
