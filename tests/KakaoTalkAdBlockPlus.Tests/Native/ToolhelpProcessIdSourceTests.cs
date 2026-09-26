using System.Diagnostics;
using System.IO;
using KakaoTalkAdBlockPlus.Native;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KakaoTalkAdBlockPlus.Tests.Native
{
    /// <summary>실제 프로세스 스냅숏(CreateToolhelp32Snapshot)을 쓰는 통합 테스트.</summary>
    [TestClass]
    public class ToolhelpProcessIdSourceTests
    {
        [TestMethod]
        public void ShouldFindCurrentProcessByImageName()
        {
            using var current = Process.GetCurrentProcess();
            var imageName = Path.GetFileName(current.MainModule.FileName).ToUpperInvariant();

            var ids = new ToolhelpProcessIdSource(imageName).GetProcessIds();

            CollectionAssert.Contains(new System.Collections.Generic.List<int>(ids), current.Id);
        }

        [TestMethod]
        public void ShouldReturnEmptyForUnknownImageName()
        {
            var ids = new ToolhelpProcessIdSource("no-such-process-" + System.Guid.NewGuid().ToString("N") + ".exe").GetProcessIds();

            Assert.AreEqual(0, ids.Count);
        }
    }
}
