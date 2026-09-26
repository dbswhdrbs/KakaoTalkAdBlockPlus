using System;
using System.Threading;

namespace KakaoTalkAdBlockPlus.Startup
{
    /// <summary>
    /// 같은 사용자 세션에서 프로그램이 두 번 실행되지 않게 한다.
    /// 두 번째로 실행되면 첫 번째 인스턴스에 신호를 보내(설정창 열기) 알린다.
    /// </summary>
    public sealed class SingleInstanceGuard : IDisposable
    {
        private readonly Mutex _mutex;
        private readonly EventWaitHandle _signal;
        private RegisteredWaitHandle? _listener;

        private SingleInstanceGuard(Mutex mutex, EventWaitHandle signal, bool isFirstInstance)
        {
            _mutex = mutex;
            _signal = signal;
            IsFirstInstance = isFirstInstance;
        }

        public bool IsFirstInstance { get; }

        public static SingleInstanceGuard Acquire(string name)
        {
            // 소유하지 않고 이름만 붙잡아 둔다 (Mutex 소유권은 스레드에 묶여 해제가 번거롭다).
            var mutex = new Mutex(initiallyOwned: false, @"Local\" + name, out var createdNew);
            var signal = new EventWaitHandle(initialState: false, EventResetMode.AutoReset, @"Local\" + name + ".Signal");
            return new SingleInstanceGuard(mutex, signal, createdNew);
        }

        /// <summary>두 번째 인스턴스에서 부른다.</summary>
        public void SignalFirstInstance() => _signal.Set();

        /// <summary>첫 번째 인스턴스에서 부른다. 콜백은 스레드 풀에서 실행된다.</summary>
        public void ListenForSignal(Action onSignal) =>
            _listener = ThreadPool.RegisterWaitForSingleObject(
                _signal, (_, _) => onSignal(), state: null, Timeout.Infinite, executeOnlyOnce: false);

        public void Dispose()
        {
            _listener?.Unregister(null);
            _signal.Dispose();
            _mutex.Dispose();
        }
    }
}
