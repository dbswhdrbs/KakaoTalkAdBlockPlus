using System;
using System.Diagnostics;
using System.Linq;
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
    }
}
