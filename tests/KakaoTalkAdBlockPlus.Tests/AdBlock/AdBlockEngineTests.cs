using KakaoTalkAdBlockPlus.AdBlock;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KakaoTalkAdBlockPlus.Tests.AdBlock
{
    [TestClass]
    public class AdBlockEngineTests
    {
        private const int KakaoTalkPid = 4242;

        private readonly FakeWindowApi _windows = new FakeWindowApi();
        private readonly FixedProcessIdSource _kakaoTalkProcesses = new FixedProcessIdSource();

        private AdBlockEngine CreateEngine() => new AdBlockEngine(_windows, _kakaoTalkProcesses);

        [TestMethod]
        public void ShouldReportNotRunningWhenNoKakaoTalkProcess()
        {
            var report = CreateEngine().RunOnce();

            Assert.IsFalse(report.IsKakaoTalkRunning);
        }

        [TestMethod]
        public void ShouldCloseUnnamedBannerChildOfMainWindow()
        {
            _kakaoTalkProcesses.Add(KakaoTalkPid);
            var main = _windows.AddTopLevel(KakaoTalkPid, "EVA_Window_Dblclk", "카카오톡");
            _windows.AddChild(main, "EVA_ChildWindow", "");
            _windows.AddChild(main, "EVA_ChildWindow", "OnlineMainView_0x00A1B2C3");
            var banner = _windows.AddChild(main, "EVA_ChildWindow", "");

            CreateEngine().RunOnce();

            CollectionAssert.Contains(_windows.Closed, banner.Handle);
        }
    }
}
