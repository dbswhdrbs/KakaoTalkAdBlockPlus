namespace KakaoTalkAdBlockPlus.AdBlock
{
    /// <summary>엔진 보고서를 모아 현재 상태를 만든다.</summary>
    public sealed class AdBlockStatusTracker
    {
        public AdBlockStatus Current { get; private set; } = AdBlockStatus.Initial;

        public void Apply(AdBlockReport report) =>
            Current = new AdBlockStatus(report.IsKakaoTalkRunning, Current.RemovedAdCount + report.RemovedAds.Count);
    }
}
