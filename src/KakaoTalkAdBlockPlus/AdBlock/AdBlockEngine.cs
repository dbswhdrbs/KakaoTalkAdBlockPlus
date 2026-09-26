using System;
using System.Collections.Generic;
using System.Linq;
using static KakaoTalkAdBlockPlus.AdBlock.KakaoTalkWindowNames;

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
            var kakaoTalkProcessIds = _kakaoTalkProcesses.GetProcessIds();
            foreach (var window in _windows.GetTopLevelWindows())
            {
                if (!kakaoTalkProcessIds.Contains(_windows.GetProcessId(window))) continue;
                if (!IsMainWindowCandidate(window)) continue;

                var descendants = _windows.GetDescendantWindows(window);
                if (!HasMainOrLockView(descendants)) continue;

                RemoveMainWindowAds(window, descendants);
            }

            return new AdBlockReport(isKakaoTalkRunning: false);
        }

        /// <summary>
        /// 메인 창의 직계 자식을 살펴
        /// 이름 없는 EVA_ChildWindow(하단 배너 광고)에는 WM_CLOSE를 보내고,
        /// 친구/채팅 목록(OnlineMainView)은 배너가 있던 자리까지 늘린다.
        /// </summary>
        private void RemoveMainWindowAds(IntPtr mainWindow, IReadOnlyList<IntPtr> descendants)
        {
            var mainRect = _windows.GetRect(mainWindow);

            // 원본(#99 수정)과 같이 첫 번째 자식은 건너뛴다.
            foreach (var child in descendants.Skip(1))
            {
                if (_windows.GetParent(child) != mainWindow) continue;

                if (IsBannerAd(child)) _windows.Close(child);

                if (_windows.GetText(child).StartsWith(MainViewTextPrefix, StringComparison.Ordinal) &&
                    mainRect.Height - 31 >= 1)
                {
                    _windows.Resize(child, mainRect.Width - 2, mainRect.Height - 31);
                }
            }
        }

        private bool IsBannerAd(IntPtr child) =>
            _windows.GetClassName(child) == ChildWindowClass &&
            _windows.GetText(child).Length == 0 &&
            !HasCustomScroll(child);

        /// <summary>메인 창 후보: 제목이 있고 소유자가 없는 EVA_Window_Dblclk.</summary>
        private bool IsMainWindowCandidate(IntPtr window) =>
            _windows.GetClassName(window) == MainWindowClass &&
            _windows.GetText(window).Length > 0 &&
            _windows.GetParent(window) == IntPtr.Zero;

        /// <summary>카카오톡 자체 스크롤(_EVA_…)이 들어 있으면 광고가 아닌 화면이다 (이모티콘 화면 등, 원본 #97).</summary>
        private bool HasCustomScroll(IntPtr window) =>
            _windows.GetDescendantWindows(window).Any(child =>
                _windows.GetClassName(child).StartsWith(CustomControlClassPrefix, StringComparison.Ordinal));

        /// <summary>친구/채팅 목록(OnlineMainView)이나 잠금 화면(LockModeView)이 있어야 진짜 메인 창이다 (동영상 플레이어 등 제외, 원본 #99).</summary>
        private bool HasMainOrLockView(IEnumerable<IntPtr> descendants) =>
            descendants.Any(child =>
                _windows.GetClassName(child) == ChildWindowClass &&
                (_windows.GetText(child).StartsWith(MainViewTextPrefix, StringComparison.Ordinal) ||
                 _windows.GetText(child).StartsWith(LockViewTextPrefix, StringComparison.Ordinal)));
    }
}
