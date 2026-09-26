using KakaoTalkAdBlockPlus.AdBlock;

namespace KakaoTalkAdBlockPlus.UI
{
    /// <summary>설정창과 트레이 메뉴에 보여 줄 상태 문구.</summary>
    public static class StatusText
    {
        public static string Title(AdBlockStatus status) =>
            status.IsKakaoTalkRunning ? "광고를 차단하고 있어요" : "카카오톡 실행을 기다리는 중";

        public static string Detail(AdBlockStatus status)
        {
            if (status.RemovedAdCount > 0) return $"이번 실행에서 광고 {status.RemovedAdCount}개를 정리했어요";

            return status.IsKakaoTalkRunning ? "아직 정리한 광고가 없어요" : "카카오톡을 켜면 자동으로 광고를 정리해요";
        }

        /// <summary>트레이 메뉴처럼 좁은 곳에 쓰는 한 줄 요약.</summary>
        public static string Summary(AdBlockStatus status) =>
            status.IsKakaoTalkRunning && status.RemovedAdCount > 0
                ? $"{Title(status)} · {status.RemovedAdCount}개 정리"
                : Title(status);
    }
}
