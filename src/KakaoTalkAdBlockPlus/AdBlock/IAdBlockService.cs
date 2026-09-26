using KakaoTalkAdBlockPlus.Settings;

namespace KakaoTalkAdBlockPlus.AdBlock
{
    /// <summary>설정창과 트레이가 쓰는 광고 차단 서비스 기능.</summary>
    public interface IAdBlockService
    {
        CheckInterval Interval { get; set; }

        AdBlockStatus Status { get; }
    }
}
