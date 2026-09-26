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
        private readonly Mutex? _mutex;
        private readonly EventWaitHandle? _signal;
        private RegisteredWaitHandle? _listener;

        private SingleInstanceGuard(Mutex? mutex, EventWaitHandle? signal, bool isFirstInstance)
        {
            _mutex = mutex;
            _signal = signal;
            IsFirstInstance = isFirstInstance;
        }

        public bool IsFirstInstance { get; }

        /// <summary>
        /// 다른 권한(관리자)으로 실행 중인 인스턴스의 이름은 열 수 없어 신호를 보낼 수 없다.
        /// </summary>
        public bool CanSignalFirstInstance => _signal != null;

        public static SingleInstanceGuard Acquire(string name)
        {
            Mutex? mutex = null;
            try
            {
                // 소유하지 않고 이름만 붙잡아 둔다 (Mutex 소유권은 스레드에 묶여 해제가 번거롭다).
                mutex = new Mutex(initiallyOwned: false, @"Local\" + name, out var createdNew);
                var signal = new EventWaitHandle(initialState: false, EventResetMode.AutoReset, @"Local\" + name + ".Signal");
                return new SingleInstanceGuard(mutex, signal, createdNew);
            }
            catch (UnauthorizedAccessException)
            {
                // 관리자 권한으로 먼저 실행된 인스턴스가 이름을 갖고 있다.
                mutex?.Dispose();
                return new SingleInstanceGuard(mutex: null, signal: null, isFirstInstance: false);
            }
        }

        /// <summary>두 번째 인스턴스에서 부른다. 보낼 수 없으면 아무 일도 하지 않는다.</summary>
        public void SignalFirstInstance() => _signal?.Set();

        /// <summary>첫 번째 인스턴스에서 부른다. 콜백은 스레드 풀에서 실행된다.</summary>
        public void ListenForSignal(Action onSignal)
        {
            if (_signal == null) return;

            _listener = ThreadPool.RegisterWaitForSingleObject(
                _signal, (_, _) => onSignal(), state: null, Timeout.Infinite, executeOnlyOnce: false);
        }

        public void Dispose()
        {
            _listener?.Unregister(null);
            _signal?.Dispose();
            _mutex?.Dispose();
        }
    }
}
