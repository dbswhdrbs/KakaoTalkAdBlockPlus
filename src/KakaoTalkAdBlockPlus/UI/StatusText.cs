using KakaoTalkAdBlockPlus.AdBlock;

namespace KakaoTalkAdBlockPlus.UI
{
    /// <summary>설정창과 트레이 메뉴에 보여 줄 상태 문구.</summary>
    public static class StatusText
    {
        public static string Title(AdBlockStatus status) => "카카오톡 실행을 기다리는 중";

        public static string Detail(AdBlockStatus status) => "카카오톡을 켜면 자동으로 광고를 정리해요";
    }
}
