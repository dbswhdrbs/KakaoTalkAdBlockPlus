namespace KakaoTalkAdBlockPlus.AdBlock
{
    /// <summary>광고 제거 한 번의 결과.</summary>
    public sealed class AdBlockReport
    {
        public AdBlockReport(bool isKakaoTalkRunning)
        {
            IsKakaoTalkRunning = isKakaoTalkRunning;
        }

        public bool IsKakaoTalkRunning { get; }
    }
}
