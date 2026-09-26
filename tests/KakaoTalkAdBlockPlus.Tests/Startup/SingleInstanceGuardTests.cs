using System;
using KakaoTalkAdBlockPlus.Startup;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KakaoTalkAdBlockPlus.Tests.Startup
{
    [TestClass]
    public class SingleInstanceGuardTests
    {
        private readonly string _name = "KakaoTalkAdBlockPlus.Tests." + Guid.NewGuid().ToString("N");

        [TestMethod]
        public void ShouldBeFirstInstanceWhenNameIsFree()
        {
            using var guard = SingleInstanceGuard.Acquire(_name);

            Assert.IsTrue(guard.IsFirstInstance);
        }

        [TestMethod]
        public void ShouldNotBeFirstInstanceWhenAlreadyRunning()
        {
            using var first = SingleInstanceGuard.Acquire(_name);

            using var second = SingleInstanceGuard.Acquire(_name);

            Assert.IsFalse(second.IsFirstInstance);
        }
    }
}
