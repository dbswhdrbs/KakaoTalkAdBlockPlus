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
        private Thread? _thread;

        public AdBlockService(IAdBlockEngine engine, CheckInterval interval)
        {
            _engine = engine;
            _interval = interval;
        }

        public void Start()
        {
            _thread = new Thread(Run) { IsBackground = true, Name = "AdBlock" };
            _thread.Start();
        }

        /// <summary>실행 중인 검사가 끝날 때까지 기다린다. 돌아온 뒤에는 엔진을 더 실행하지 않는다.</summary>
        public void Stop()
        {
            _stopRequested = true;
            _thread?.Join();
        }

        public void Dispose() => Stop();

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
