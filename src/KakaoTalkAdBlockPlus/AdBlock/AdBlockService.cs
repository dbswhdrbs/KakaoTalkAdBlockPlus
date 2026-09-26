using System;
using System.Diagnostics;
using System.Threading;
using KakaoTalkAdBlockPlus.Settings;

namespace KakaoTalkAdBlockPlus.AdBlock
{
    /// <summary>백그라운드 스레드에서 확인 주기마다 광고 제거 엔진을 실행한다.</summary>
    public sealed class AdBlockService : IAdBlockService, IDisposable
    {
        private static readonly TimeSpan DefaultStopTimeout = TimeSpan.FromSeconds(2);

        private readonly IAdBlockEngine _engine;
        private readonly TimeSpan _stopTimeout;
        private readonly AutoResetEvent _wakeUp = new AutoResetEvent(initialState: false);
        private readonly AdBlockStatusTracker _tracker = new AdBlockStatusTracker();
        private volatile AdBlockStatus _status = AdBlockStatus.Initial;
        private volatile int _intervalMilliseconds;
        private volatile bool _stopRequested;
        private Thread? _thread;
        private bool _disposed;

        /// <param name="stopTimeout">멈출 때 진행 중인 검사를 기다리는 최대 시간 (카카오톡이 응답 없음이어도 종료가 멈추지 않게).</param>
        public AdBlockService(IAdBlockEngine engine, CheckInterval interval, TimeSpan? stopTimeout = null)
        {
            _engine = engine;
            _intervalMilliseconds = interval.Milliseconds;
            _stopTimeout = stopTimeout ?? DefaultStopTimeout;
        }

        /// <summary>바꾸면 기다리던 중이라도 바로 새 주기로 다시 시작한다.</summary>
        public CheckInterval Interval
        {
            get => CheckInterval.FromMilliseconds(_intervalMilliseconds);
            set
            {
                _intervalMilliseconds = value.Milliseconds;
                _wakeUp.Set();
            }
        }

        /// <summary>어느 스레드에서 읽어도 되는 최신 상태.</summary>
        public AdBlockStatus Status => _status;

        public void Start()
        {
            _thread = new Thread(Run) { IsBackground = true, Name = "AdBlock" };
            _thread.Start();
        }

        /// <summary>
        /// 실행 중인 검사가 끝날 때까지 (최대 stopTimeout) 기다린다. 돌아온 뒤에는 새 검사를 시작하지 않는다.
        /// </summary>
        public void Stop() => StopAndWait();

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            // 검사가 멈춰 스레드가 아직 끝나지 않았으면, 그 스레드가 쓸 대기 핸들은 남겨 둔다.
            if (StopAndWait()) _wakeUp.Dispose();
        }

        private bool StopAndWait()
        {
            _stopRequested = true;
            _wakeUp.Set();
            return _thread == null || _thread.Join(_stopTimeout);
        }

        private void Run()
        {
            while (!_stopRequested)
            {
                try
                {
                    _tracker.Apply(_engine.RunOnce());
                    _status = _tracker.Current;
                }
                catch (Exception exception)
                {
                    // 검사 도중 카카오톡 창이 사라지는 등 한 번의 실패로 차단을 멈추지 않는다.
                    Trace.TraceWarning("광고 검사 실패: {0}", exception);
                }

                if (_stopRequested) break;
                _wakeUp.WaitOne(_intervalMilliseconds);
            }
        }
    }
}
