namespace KakaoTalkAdBlockPlus.AdBlock
{
    /// <summary>엔진 보고서를 모아 현재 상태를 만든다.</summary>
    public sealed class AdBlockStatusTracker
    {
        public AdBlockStatus Current { get; } = AdBlockStatus.Initial;
    }
}
