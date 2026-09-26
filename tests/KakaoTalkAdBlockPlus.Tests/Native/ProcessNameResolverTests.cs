using System.Diagnostics;
using System.IO;
using KakaoTalkAdBlockPlus.Native;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KakaoTalkAdBlockPlus.Tests.Native
{
    /// <summary>실제 프로세스(OpenProcess + QueryFullProcessImageName)를 쓰는 통합 테스트.</summary>
    [TestClass]
    public class ProcessNameResolverTests
    {
        [TestMethod]
        public void ShouldResolveCurrentProcessImageName()
        {
            using var current = Process.GetCurrentProcess();

            var name = new ProcessNameResolver().GetImageName(current.Id);

            Assert.AreEqual(Path.GetFileName(current.MainModule.FileName), name);
        }
    }
}
