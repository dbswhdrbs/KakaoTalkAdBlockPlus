using System;
using System.Collections.Generic;

namespace KakaoTalkAdBlockPlus.AdBlock
{
    /// <summary>광고 제거 한 번의 결과.</summary>
    public sealed class AdBlockReport
    {
        public AdBlockReport(bool isKakaoTalkRunning, IReadOnlyList<IntPtr> removedAds)
        {
            IsKakaoTalkRunning = isKakaoTalkRunning;
            RemovedAds = removedAds;
        }

        public bool IsKakaoTalkRunning { get; }

        /// <summary>이번에 닫거나 숨긴 광고 창.</summary>
        public IReadOnlyList<IntPtr> RemovedAds { get; }
    }
}
