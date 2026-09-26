using System;
using System.Threading;
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

        [TestMethod]
        public void ShouldSignalFirstInstanceWhenSecondStarts()
        {
            using var signaled = new ManualResetEventSlim();
            using var first = SingleInstanceGuard.Acquire(_name);
            first.ListenForSignal(signaled.Set);

            using var second = SingleInstanceGuard.Acquire(_name);
            second.SignalFirstInstance();

            Assert.IsTrue(signaled.Wait(TimeSpan.FromSeconds(3)));
        }

        [TestMethod]
        public void ShouldReleaseNameOnDispose()
        {
            SingleInstanceGuard.Acquire(_name).Dispose();

            using var again = SingleInstanceGuard.Acquire(_name);

            Assert.IsTrue(again.IsFirstInstance);
        }
    }
}
