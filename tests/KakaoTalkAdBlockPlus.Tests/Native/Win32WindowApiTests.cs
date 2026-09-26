using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using KakaoTalkAdBlockPlus.AdBlock;
using KakaoTalkAdBlockPlus.Native;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KakaoTalkAdBlockPlus.Tests.Native
{
    /// <summary>테스트 프로세스 안에 만든 진짜 창으로 Win32WindowApi를 확인한다.</summary>
    [TestClass]
    public class Win32WindowApiTests
    {
        private readonly Win32WindowApi _api = new Win32WindowApi();

        [TestMethod]
        public void ShouldListTopLevelWindowWithProcessId()
        {
            using var factory = new TestWindowFactory();
            var window = factory.CreateTopLevel("KakaoTalkAdBlockPlus_TestTopLevel", "top");

            CollectionAssert.Contains(_api.GetTopLevelWindows().ToList(), window);
            using var current = Process.GetCurrentProcess();
            Assert.AreEqual(current.Id, _api.GetProcessId(window));
        }

        [TestMethod]
        public void ShouldReadClassNameTextAndParent()
        {
            using var factory = new TestWindowFactory();
            var main = factory.CreateTopLevel("EVA_Window_Dblclk", "카카오톡");
            var child = factory.CreateChild(main, "EVA_ChildWindow", "OnlineMainView_0x0001");
            var popup = factory.CreateTopLevel("EVA_Window_Dblclk", "", owner: main);

            Assert.AreEqual("EVA_ChildWindow", _api.GetClassName(child));
            Assert.AreEqual("카카오톡", _api.GetText(main));
            Assert.AreEqual("OnlineMainView_0x0001", _api.GetText(child));
            Assert.AreEqual(main, _api.GetParent(child));
            Assert.AreEqual(main, _api.GetParent(popup));
            Assert.AreEqual(IntPtr.Zero, _api.GetParent(main));
        }

        [TestMethod]
        public void ShouldListDescendantsInPreOrder()
        {
            using var factory = new TestWindowFactory();
            var main = factory.CreateTopLevel("EVA_Window_Dblclk", "카카오톡");
            var first = factory.CreateChild(main, "EVA_ChildWindow", "A");
            var nested = factory.CreateChild(first, "EVA_ChildWindow", "A-1");
            var second = factory.CreateChild(main, "EVA_ChildWindow", "B");

            CollectionAssert.AreEqual(new[] { first, nested, second }, _api.GetDescendantWindows(main).ToList());
        }

        [TestMethod]
        public void ShouldCloseWindow()
        {
            using var factory = new TestWindowFactory();
            var main = factory.CreateTopLevel("EVA_Window_Dblclk", "카카오톡");
            var banner = factory.CreateChild(main, "EVA_ChildWindow", "");

            _api.Close(banner);

            Assert.IsFalse(TestWindowFactory.Exists(banner));
            Assert.IsTrue(TestWindowFactory.Exists(main));
        }

        [TestMethod]
        public void ShouldHideWindow()
        {
            using var factory = new TestWindowFactory();
            var popup = factory.CreateTopLevel("EVA_Window", "", visible: true);
            Assert.IsTrue(_api.IsVisible(popup));

            _api.Hide(popup);

            Assert.IsFalse(_api.IsVisible(popup));
        }

        [TestMethod]
        public void ShouldResizeWindow()
        {
            using var factory = new TestWindowFactory();
            var main = factory.CreateTopLevel("EVA_Window_Dblclk", "카카오톡");
            var mainView = factory.CreateChild(main, "EVA_ChildWindow", "OnlineMainView_0x0001");

            _api.Resize(mainView, 321, 123);

            var rect = _api.GetRect(mainView);
            Assert.AreEqual(321, rect.Width);
            Assert.AreEqual(123, rect.Height);
        }

        [TestMethod]
        public void ShouldRemoveBannerFromSimulatedKakaoTalkWindow()
        {
            using var factory = new TestWindowFactory();
            var main = factory.CreateTopLevel("EVA_Window_Dblclk", "카카오톡");
            var first = factory.CreateChild(main, "EVA_ChildWindow", "");
            var mainView = factory.CreateChild(main, "EVA_ChildWindow", "OnlineMainView_0x0001");
            var banner = factory.CreateChild(main, "EVA_ChildWindow", "");
            using var current = Process.GetCurrentProcess();
            var engine = new AdBlockEngine(_api, new CurrentProcessOnly(current.Id));

            var report = engine.RunOnce();

            Assert.IsFalse(TestWindowFactory.Exists(banner), "배너가 닫혀야 한다");
            Assert.IsTrue(TestWindowFactory.Exists(first), "첫 번째 자식은 남아야 한다");
            Assert.AreEqual(600 - 31, _api.GetRect(mainView).Height, "목록 화면이 배너 자리까지 늘어나야 한다");
            CollectionAssert.AreEqual(new[] { banner }, report.RemovedAds.ToList());
        }

        private sealed class CurrentProcessOnly : IProcessIdSource
        {
            private readonly int[] _processIds;

            public CurrentProcessOnly(int processId)
            {
                _processIds = new[] { processId };
            }

            public IReadOnlyCollection<int> GetProcessIds() => _processIds;
        }
    }
}
