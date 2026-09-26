using KakaoTalkAdBlockPlus.AdBlock;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KakaoTalkAdBlockPlus.Tests.AdBlock
{
    [TestClass]
    public class AdBlockEngineTests
    {
        private readonly FakeWindowApi _windows = new FakeWindowApi();
        private readonly FixedProcessIdSource _kakaoTalkProcesses = new FixedProcessIdSource();

        private AdBlockEngine CreateEngine() => new AdBlockEngine(_windows, _kakaoTalkProcesses);

        [TestMethod]
        public void ShouldReportNotRunningWhenNoKakaoTalkProcess()
        {
            var report = CreateEngine().RunOnce();

            Assert.IsFalse(report.IsKakaoTalkRunning);
        }
    }
}
