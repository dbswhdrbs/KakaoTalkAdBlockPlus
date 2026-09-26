namespace KakaoTalkAdBlockPlus.AdBlock
{
    /// <summary>화면에 보여 줄 현재 상태 (바뀌지 않는 스냅숏).</summary>
    public sealed class AdBlockStatus
    {
        public AdBlockStatus(bool isKakaoTalkRunning, int removedAdCount)
        {
            IsKakaoTalkRunning = isKakaoTalkRunning;
            RemovedAdCount = removedAdCount;
        }

        public static AdBlockStatus Initial { get; } = new AdBlockStatus(isKakaoTalkRunning: false, removedAdCount: 0);

        public bool IsKakaoTalkRunning { get; }

        /// <summary>이번 실행에서 없앤 광고 창 수.</summary>
        public int RemovedAdCount { get; }
    }
}
