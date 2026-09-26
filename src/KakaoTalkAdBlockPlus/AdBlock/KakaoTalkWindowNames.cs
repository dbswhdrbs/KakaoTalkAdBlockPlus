namespace KakaoTalkAdBlockPlus.AdBlock
{
    /// <summary>카카오톡 PC(일반 Win32 클라이언트)의 창 클래스 이름과 창 텍스트.</summary>
    internal static class KakaoTalkWindowNames
    {
        /// <summary>메인 창, 메인 창이 소유한 팝업.</summary>
        public const string MainWindowClass = "EVA_Window_Dblclk";

        /// <summary>팝업 창.</summary>
        public const string PopupWindowClass = "EVA_Window";

        /// <summary>광고 웹뷰(Chromium) 안쪽 창의 텍스트.</summary>
        public const string ChromeLegacyWindowText = "Chrome Legacy Window";

        /// <summary>메인 창 안의 화면 조각 (목록, 배너 광고 등).</summary>
        public const string ChildWindowClass = "EVA_ChildWindow";

        /// <summary>카카오톡 자체 컨트롤(스크롤 등) 클래스 이름 접두사.</summary>
        public const string CustomControlClassPrefix = "_EVA_";

        /// <summary>친구/채팅 목록 화면.</summary>
        public const string MainViewTextPrefix = "OnlineMainView";

        /// <summary>잠금 모드 화면.</summary>
        public const string LockViewTextPrefix = "LockModeView";

        /// <summary>메인 창 테두리 그림자 폭.</summary>
        public const int LayoutShadowPadding = 2;

        /// <summary>메인 창 위쪽 제목 영역 높이.</summary>
        public const int MainViewPadding = 31;
    }
}
