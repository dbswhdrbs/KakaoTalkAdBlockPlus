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
    }
}
