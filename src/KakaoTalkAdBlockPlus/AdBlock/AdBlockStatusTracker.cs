using System;
using System.Collections.Generic;

namespace KakaoTalkAdBlockPlus.AdBlock
{
    /// <summary>엔진 보고서를 모아 현재 상태를 만든다. 같은 광고 창은 한 번만 센다.</summary>
    public sealed class AdBlockStatusTracker
    {
        private readonly HashSet<IntPtr> _removedAds = new HashSet<IntPtr>();

        public AdBlockStatus Current { get; private set; } = AdBlockStatus.Initial;

        public void Apply(AdBlockReport report)
        {
            _removedAds.UnionWith(report.RemovedAds);
            Current = new AdBlockStatus(report.IsKakaoTalkRunning, _removedAds.Count);
        }
    }
}
