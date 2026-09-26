using System;
using System.Threading;
using KakaoTalkAdBlockPlus.AdBlock;
using KakaoTalkAdBlockPlus.Settings;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KakaoTalkAdBlockPlus.Tests.AdBlock
{
    [TestClass]
    public class AdBlockServiceTests
    {
        private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(3);
        private static readonly CheckInterval Fastest = CheckInterval.FromMilliseconds(CheckInterval.MinMilliseconds);

        private readonly CountingEngine _engine = new CountingEngine();

        [TestMethod]
        public void ShouldRunEngineRepeatedlyAfterStart()
        {
            using var service = new AdBlockService(_engine, Fastest);

            service.Start();

            Assert.IsTrue(_engine.WaitForCalls(3, Timeout), $"엔진 실행 횟수: {_engine.Calls}");
        }

        [TestMethod]
        public void ShouldStopRunningAfterStop()
        {
            var service = new AdBlockService(_engine, Fastest);
            service.Start();
            Assert.IsTrue(_engine.WaitForCalls(1, Timeout));

            service.Stop();
            var callsAfterStop = _engine.Calls;
            Thread.Sleep(Fastest.Milliseconds * 4);

            Assert.AreEqual(callsAfterStop, _engine.Calls);
        }

        [TestMethod]
        public void ShouldApplyNewIntervalWithoutWaitingForOldOne()
        {
            using var service = new AdBlockService(_engine, CheckInterval.FromMilliseconds(CheckInterval.MaxMilliseconds));
            service.Start();
            Assert.IsTrue(_engine.WaitForCalls(1, Timeout));

            service.Interval = Fastest;

            Assert.IsTrue(_engine.WaitForCalls(3, Timeout), $"엔진 실행 횟수: {_engine.Calls}");
        }

        [TestMethod]
        public void ShouldKeepRunningWhenEngineThrows()
        {
            _engine.Behavior = () => throw new InvalidOperationException("카카오톡 창이 사라짐");
            using var service = new AdBlockService(_engine, Fastest);

            service.Start();

            Assert.IsTrue(_engine.WaitForCalls(3, Timeout), $"엔진 실행 횟수: {_engine.Calls}");
        }

        private sealed class CountingEngine : IAdBlockEngine
        {
            private int _calls;

            public int Calls => Volatile.Read(ref _calls);

            public Func<AdBlockReport> Behavior { get; set; } = () => new AdBlockReport(isKakaoTalkRunning: false, new IntPtr[0]);

            public AdBlockReport RunOnce()
            {
                Interlocked.Increment(ref _calls);
                return Behavior();
            }

            public bool WaitForCalls(int count, TimeSpan timeout) => SpinWait.SpinUntil(() => Calls >= count, timeout);
        }
    }
}
