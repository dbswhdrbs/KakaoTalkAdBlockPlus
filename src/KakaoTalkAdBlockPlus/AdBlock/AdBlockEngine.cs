using System.Linq;

namespace KakaoTalkAdBlockPlus.AdBlock
{
    /// <summary>
    /// 카카오톡 창에서 광고를 찾아 없앤다.
    /// 규칙은 원본 KakaoTalkAdBlock 최신판(2.2.4)의 일반(Win32) 클라이언트 규칙을 옮겼다.
    /// </summary>
    public sealed class AdBlockEngine
    {
        private readonly IWindowApi _windows;
        private readonly IProcessIdSource _kakaoTalkProcesses;

        public AdBlockEngine(IWindowApi windows, IProcessIdSource kakaoTalkProcesses)
        {
            _windows = windows;
            _kakaoTalkProcesses = kakaoTalkProcesses;
        }

        public AdBlockReport RunOnce()
        {
            foreach (var window in _windows.GetTopLevelWindows())
            {
                // 원본(#99 수정)과 같이 첫 번째 자식은 건너뛴다.
                foreach (var child in _windows.GetDescendantWindows(window).Skip(1))
                {
                    if (_windows.GetClassName(child) == "EVA_ChildWindow" && _windows.GetText(child).Length == 0)
                    {
                        _windows.Close(child);
                    }
                }
            }

            return new AdBlockReport(isKakaoTalkRunning: false);
        }
    }
}
