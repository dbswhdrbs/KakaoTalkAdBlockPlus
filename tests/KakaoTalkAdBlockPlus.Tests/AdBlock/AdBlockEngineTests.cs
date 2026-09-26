using KakaoTalkAdBlockPlus.AdBlock;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KakaoTalkAdBlockPlus.Tests.AdBlock
{
    [TestClass]
    public class AdBlockEngineTests
    {
        private const int KakaoTalkPid = 4242;
        private const int OtherPid = 7777;

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
            var scene = AddMainWindowWithBanner(KakaoTalkPid);

            CreateEngine().RunOnce();

            CollectionAssert.Contains(_windows.Closed, scene.Banner.Handle);
        }

        [TestMethod]
        public void ShouldNotCloseFirstChildOfMainWindow()
        {
            _kakaoTalkProcesses.Add(KakaoTalkPid);
            var scene = AddMainWindowWithBanner(KakaoTalkPid);

            CreateEngine().RunOnce();

            CollectionAssert.DoesNotContain(_windows.Closed, scene.FirstChild.Handle);
        }

        [TestMethod]
        public void ShouldIgnoreWindowsOfOtherProcesses()
        {
            _kakaoTalkProcesses.Add(KakaoTalkPid);
            AddMainWindowWithBanner(OtherPid);

            CreateEngine().RunOnce();

            Assert.AreEqual(0, _windows.Closed.Count);
        }

        [TestMethod]
        public void ShouldIgnoreWindowWithoutMainOrLockView()
        {
            _kakaoTalkProcesses.Add(KakaoTalkPid);
            var player = _windows.AddTopLevel(KakaoTalkPid, "EVA_Window_Dblclk", "동영상 플레이어");
            _windows.AddChild(player, "EVA_ChildWindow", "");
            _windows.AddChild(player, "EVA_ChildWindow", "");

            CreateEngine().RunOnce();

            Assert.AreEqual(0, _windows.Closed.Count);
        }

        [TestMethod]
        public void ShouldNotCloseChildContainingCustomScroll()
        {
            _kakaoTalkProcesses.Add(KakaoTalkPid);
            var scene = AddMainWindowWithBanner(KakaoTalkPid);
            var emoticonView = _windows.AddChild(scene.Main, "EVA_ChildWindow", "");
            var panel = _windows.AddChild(emoticonView, "EVA_VH_ListControl_Dblclk", "");
            _windows.AddChild(panel, "_EVA_CustomScrollCtrl", "");

            CreateEngine().RunOnce();

            CollectionAssert.DoesNotContain(_windows.Closed, emoticonView.Handle);
            CollectionAssert.Contains(_windows.Closed, scene.Banner.Handle);
        }

        [TestMethod]
        public void ShouldNotCloseNestedDescendants()
        {
            _kakaoTalkProcesses.Add(KakaoTalkPid);
            var scene = AddMainWindowWithBanner(KakaoTalkPid);
            var nested = _windows.AddChild(scene.MainView, "EVA_ChildWindow", "");

            CreateEngine().RunOnce();

            CollectionAssert.DoesNotContain(_windows.Closed, nested.Handle);
        }

        [TestMethod]
        public void ShouldNotCloseNamedChildren()
        {
            _kakaoTalkProcesses.Add(KakaoTalkPid);
            var scene = AddMainWindowWithBanner(KakaoTalkPid);
            var named = _windows.AddChild(scene.Main, "EVA_ChildWindow", "ChatRoomListView_0x0042");

            CreateEngine().RunOnce();

            CollectionAssert.DoesNotContain(_windows.Closed, named.Handle);
            CollectionAssert.DoesNotContain(_windows.Closed, scene.MainView.Handle);
        }

        [DataTestMethod]
        [DataRow("EVA_Window_Dblclk", "", false, DisplayName = "제목 없음")]
        [DataRow("EVA_Window_Dblclk", "카카오톡", true, DisplayName = "소유자 있음")]
        [DataRow("EVA_Window", "카카오톡", false, DisplayName = "다른 클래스")]
        public void ShouldIgnoreUntitledOrOwnedWindows(string className, string title, bool owned)
        {
            _kakaoTalkProcesses.Add(KakaoTalkPid);
            var owner = owned ? _windows.AddTopLevel(KakaoTalkPid, "EVA_Window_Dblclk", "카카오톡") : null;
            var window = _windows.AddTopLevel(KakaoTalkPid, className, title, owner);
            _windows.AddChild(window, "EVA_ChildWindow", "");
            _windows.AddChild(window, "EVA_ChildWindow", "OnlineMainView_0x00A1B2C3");
            var banner = _windows.AddChild(window, "EVA_ChildWindow", "");

            CreateEngine().RunOnce();

            CollectionAssert.DoesNotContain(_windows.Closed, banner.Handle);
        }

        [TestMethod]
        public void ShouldResizeOnlineMainViewOverBannerArea()
        {
            _kakaoTalkProcesses.Add(KakaoTalkPid);
            var scene = AddMainWindowWithBanner(KakaoTalkPid);
            scene.Main.Rect = new WindowRect(100, 100, 500, 700);

            CreateEngine().RunOnce();

            CollectionAssert.Contains(_windows.Resized, new ResizedWindow(scene.MainView.Handle, 398, 569));
        }

        [TestMethod]
        public void ShouldSkipOnlineMainViewResizeWhenWindowTooSmall()
        {
            _kakaoTalkProcesses.Add(KakaoTalkPid);
            var scene = AddMainWindowWithBanner(KakaoTalkPid);
            scene.Main.Rect = new WindowRect(0, 0, 160, 31);

            CreateEngine().RunOnce();

            Assert.AreEqual(0, _windows.Resized.Count);
        }

        [TestMethod]
        public void ShouldResizeLockModeViewToFullHeight()
        {
            _kakaoTalkProcesses.Add(KakaoTalkPid);
            var main = _windows.AddTopLevel(KakaoTalkPid, "EVA_Window_Dblclk", "카카오톡");
            main.Rect = new WindowRect(100, 100, 500, 700);
            _windows.AddChild(main, "EVA_ChildWindow", "");
            var lockView = _windows.AddChild(main, "EVA_ChildWindow", "LockModeView_0x00D4E5F6");

            CreateEngine().RunOnce();

            CollectionAssert.Contains(_windows.Resized, new ResizedWindow(lockView.Handle, 398, 600));
        }

        [TestMethod]
        public void ShouldHidePopupAdContainingChromeLegacyWindow()
        {
            _kakaoTalkProcesses.Add(KakaoTalkPid);
            var popup = _windows.AddTopLevel(KakaoTalkPid, "EVA_Window", "");
            var host = _windows.AddChild(popup, "Chrome_WidgetWin_0", "");
            _windows.AddChild(host, "Chrome_RenderWidgetHostHWND", "Chrome Legacy Window");

            CreateEngine().RunOnce();

            CollectionAssert.Contains(_windows.Hidden, popup.Handle);
        }

        [TestMethod]
        public void ShouldHideOwnedPopupAdOfMainWindow()
        {
            _kakaoTalkProcesses.Add(KakaoTalkPid);
            var scene = AddMainWindowWithBanner(KakaoTalkPid);
            var popup = _windows.AddTopLevel(KakaoTalkPid, "EVA_Window_Dblclk", "", owner: scene.Main);
            var host = _windows.AddChild(popup, "Chrome_WidgetWin_0", "");
            _windows.AddChild(host, "Chrome_RenderWidgetHostHWND", "Chrome Legacy Window");

            CreateEngine().RunOnce();

            CollectionAssert.Contains(_windows.Hidden, popup.Handle);
        }

        [TestMethod]
        public void ShouldNotHidePopupWithoutChromeLegacyWindow()
        {
            _kakaoTalkProcesses.Add(KakaoTalkPid);
            var scene = AddMainWindowWithBanner(KakaoTalkPid);
            var notification = _windows.AddTopLevel(KakaoTalkPid, "EVA_Window", "");
            _windows.AddChild(notification, "EVA_ChildWindow", "새 메시지");
            var ownedMenu = _windows.AddTopLevel(KakaoTalkPid, "EVA_Window_Dblclk", "", owner: scene.Main);
            _windows.AddChild(ownedMenu, "EVA_ChildWindow", "");

            CreateEngine().RunOnce();

            Assert.AreEqual(0, _windows.Hidden.Count);
        }

        [TestMethod]
        public void ShouldNotHideAlreadyHiddenPopup()
        {
            _kakaoTalkProcesses.Add(KakaoTalkPid);
            var popup = _windows.AddTopLevel(KakaoTalkPid, "EVA_Window", "");
            popup.IsVisible = false;
            var host = _windows.AddChild(popup, "Chrome_WidgetWin_0", "");
            _windows.AddChild(host, "Chrome_RenderWidgetHostHWND", "Chrome Legacy Window");

            CreateEngine().RunOnce();

            Assert.AreEqual(0, _windows.Hidden.Count);
        }

        /// <summary>
        /// 카카오톡 메인 창 구조:
        /// "카카오톡" EVA_Window_Dblclk ─┬─ "" EVA_ChildWindow (첫 번째 자식)
        ///                              ├─ "OnlineMainView_…" EVA_ChildWindow (친구/채팅 목록)
        ///                              └─ "" EVA_ChildWindow (하단 배너 광고)
        /// </summary>
        private MainWindowScene AddMainWindowWithBanner(int processId)
        {
            var main = _windows.AddTopLevel(processId, "EVA_Window_Dblclk", "카카오톡");
            return new MainWindowScene(
                main,
                firstChild: _windows.AddChild(main, "EVA_ChildWindow", ""),
                mainView: _windows.AddChild(main, "EVA_ChildWindow", "OnlineMainView_0x00A1B2C3"),
                banner: _windows.AddChild(main, "EVA_ChildWindow", ""));
        }

        private sealed class MainWindowScene
        {
            public MainWindowScene(FakeWindow main, FakeWindow firstChild, FakeWindow mainView, FakeWindow banner)
            {
                Main = main;
                FirstChild = firstChild;
                MainView = mainView;
                Banner = banner;
            }

            public FakeWindow Main { get; }

            public FakeWindow FirstChild { get; }

            public FakeWindow MainView { get; }

            public FakeWindow Banner { get; }
        }
    }
}
