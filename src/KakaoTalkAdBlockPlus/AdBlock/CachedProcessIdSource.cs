using System;
using System.Collections.Generic;

namespace KakaoTalkAdBlockPlus.AdBlock
{
    /// <summary>프로세스 목록 조회는 비싸므로 갱신 주기 동안 결과를 재사용한다.</summary>
    public sealed class CachedProcessIdSource : IProcessIdSource
    {
        private readonly IProcessIdSource _inner;
        private readonly TimeSpan _refreshPeriod;
        private readonly Func<TimeSpan> _clock;
        private IReadOnlyCollection<int>? _cached;
        private TimeSpan _cachedAt;

        /// <param name="clock">단조 증가하는 현재 시각 (예: Stopwatch.Elapsed).</param>
        public CachedProcessIdSource(IProcessIdSource inner, TimeSpan refreshPeriod, Func<TimeSpan> clock)
        {
            _inner = inner;
            _refreshPeriod = refreshPeriod;
            _clock = clock;
        }

        public IReadOnlyCollection<int> GetProcessIds()
        {
            var now = _clock();
            if (_cached == null || now - _cachedAt >= _refreshPeriod)
            {
                _cached = _inner.GetProcessIds();
                _cachedAt = now;
            }

            return _cached;
        }
    }
}
