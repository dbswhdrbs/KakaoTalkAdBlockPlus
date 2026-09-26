using System;
using System.Security.AccessControl;
using System.Security.Principal;
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
        public void ShouldTreatInaccessibleInstanceAsAlreadyRunning()
        {
            // 관리자 권한으로 실행된 인스턴스처럼, 현재 사용자가 열 수 없는 이름을 먼저 만들어 둔다.
            var systemOnly = new MutexSecurity();
            systemOnly.AddAccessRule(new MutexAccessRule(
                new SecurityIdentifier(WellKnownSidType.LocalSystemSid, null), MutexRights.FullControl, AccessControlType.Allow));
            using var elevatedInstance = new Mutex(false, @"Local\" + _name, out _, systemOnly);

            using var guard = SingleInstanceGuard.Acquire(_name);

            Assert.IsFalse(guard.IsFirstInstance);
            Assert.IsFalse(guard.CanSignalFirstInstance);
            guard.SignalFirstInstance();
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
