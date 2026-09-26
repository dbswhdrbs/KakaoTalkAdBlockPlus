using System;
using System.Threading;

namespace KakaoTalkAdBlockPlus.Startup
{
    /// <summary>같은 사용자 세션에서 프로그램이 두 번 실행되지 않게 한다.</summary>
    public sealed class SingleInstanceGuard : IDisposable
    {
        private readonly Mutex _mutex;

        private SingleInstanceGuard(Mutex mutex, bool isFirstInstance)
        {
            _mutex = mutex;
            IsFirstInstance = isFirstInstance;
        }

        public bool IsFirstInstance { get; }

        public static SingleInstanceGuard Acquire(string name)
        {
            // 소유하지 않고 이름만 붙잡아 둔다 (Mutex 소유권은 스레드에 묶여 해제가 번거롭다).
            var mutex = new Mutex(initiallyOwned: false, @"Local\" + name, out var createdNew);
            return new SingleInstanceGuard(mutex, createdNew);
        }

        public void Dispose() => _mutex.Dispose();
    }
}
