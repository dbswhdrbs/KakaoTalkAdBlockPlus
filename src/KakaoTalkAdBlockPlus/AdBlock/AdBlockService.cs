using System;
using System.Threading;
using KakaoTalkAdBlockPlus.Settings;

namespace KakaoTalkAdBlockPlus.AdBlock
{
    /// <summary>백그라운드 스레드에서 확인 주기마다 광고 제거 엔진을 실행한다.</summary>
    public sealed class AdBlockService : IDisposable
    {
        private readonly IAdBlockEngine _engine;
        private readonly CheckInterval _interval;
        private volatile bool _stopRequested;

        public AdBlockService(IAdBlockEngine engine, CheckInterval interval)
        {
            _engine = engine;
            _interval = interval;
        }

        public void Start() => new Thread(Run) { IsBackground = true, Name = "AdBlock" }.Start();

        public void Dispose() => _stopRequested = true;

        private void Run()
        {
            while (!_stopRequested)
            {
                _engine.RunOnce();
                Thread.Sleep(_interval.Milliseconds);
            }
        }
    }
}
