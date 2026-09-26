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
    public sealed class AdBlockEngine : IAdBlockEngine
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
            if (kakaoTalkProcessIds.Count == 0) return new AdBlockReport(isKakaoTalkRunning: false, new IntPtr[0]);

            var kakaoTalkWindows = _windows.GetTopLevelWindows()
                .Where(window => kakaoTalkProcessIds.Contains(_windows.GetProcessId(window)))
                .ToList();

            var removedAds = new List<IntPtr>();
            var mainWindows = kakaoTalkWindows.Where(IsMainWindowCandidate).ToList();
            foreach (var window in mainWindows)
            {
                var descendants = _windows.GetDescendantWindows(window);
                if (!HasMainOrLockView(descendants)) continue;

                RemoveMainWindowAds(window, descendants, removedAds);
            }

            HidePopupAds(kakaoTalkWindows, mainWindows, removedAds);

            return new AdBlockReport(isKakaoTalkRunning: true, removedAds);
        }

        /// <summary>
        /// 메인 창의 직계 자식을 살펴
        /// 이름 없는 EVA_ChildWindow(하단 배너 광고)에는 WM_CLOSE를 보내고,
        /// 친구/채팅 목록(OnlineMainView)은 배너가 있던 자리까지 늘린다.
        /// </summary>
        private void RemoveMainWindowAds(IntPtr mainWindow, IReadOnlyList<IntPtr> descendants, List<IntPtr> removedAds)
        {
            var mainRect = _windows.GetRect(mainWindow);

            // 원본(#99 수정)과 같이 첫 번째 자식은 건너뛴다.
            foreach (var child in descendants.Skip(1))
            {
                if (_windows.GetParent(child) != mainWindow) continue;

                if (IsBannerAd(child))
                {
                    _windows.Close(child);
                    removedAds.Add(child);
                }

                ExpandViewOverAdArea(child, mainRect);
            }
        }

        /// <summary>광고 웹뷰를 품은 팝업 창을 숨긴다.</summary>
        private void HidePopupAds(IEnumerable<IntPtr> kakaoTalkWindows, ICollection<IntPtr> mainWindows, List<IntPtr> removedAds)
        {
            foreach (var popup in kakaoTalkWindows.Where(window => IsPopupAdCandidate(window, mainWindows)))
            {
                if (!_windows.IsVisible(popup) || !ContainsChromeLegacyWindow(popup)) continue;

                _windows.Hide(popup);
                removedAds.Add(popup);
            }
        }

        /// <summary>목록 화면은 배너 자리까지, 잠금 화면은 창 높이 전체로 늘린다 (원본 HideMainViewAdArea/HideLockScreenAdArea).</summary>
        private void ExpandViewOverAdArea(IntPtr view, WindowRect mainRect)
        {
            var text = _windows.GetText(view);
            var width = mainRect.Width - LayoutShadowPadding;

            if (text.StartsWith(MainViewTextPrefix, StringComparison.Ordinal))
            {
                var height = mainRect.Height - MainViewPadding;
                if (height >= 1) ResizeIfNeeded(view, width, height);
            }
            else if (text.StartsWith(LockViewTextPrefix, StringComparison.Ordinal))
            {
                ResizeIfNeeded(view, width, mainRect.Height);
            }
        }

        /// <summary>
        /// 원본은 매번 크기를 다시 맞췄지만, 이미 맞으면 건너뛴다:
        /// 카카오톡에 검사마다 창 메시지를 보내지 않고, 카카오톡이 멈췄을 때 기다릴 일도 줄인다.
        /// </summary>
        private void ResizeIfNeeded(IntPtr view, int width, int height)
        {
            var current = _windows.GetRect(view);
            if (current.Width == width && current.Height == height) return;

            _windows.Resize(view, width, height);
        }

        private bool IsBannerAd(IntPtr child) =>
            _windows.GetClassName(child) == ChildWindowClass &&
            _windows.GetText(child).Length == 0 &&
            !HasCustomScroll(child);

        /// <summary>
        /// 팝업 광고 후보: 제목 없는 창 중
        /// 소유자가 없는 EVA_Window, 또는 메인 창이 소유한 EVA_Window_Dblclk.
        /// </summary>
        private bool IsPopupAdCandidate(IntPtr window, ICollection<IntPtr> mainWindows)
        {
            if (_windows.GetText(window).Length != 0) return false;

            var className = _windows.GetClassName(window);
            var owner = _windows.GetParent(window);
            return (className == PopupWindowClass && owner == IntPtr.Zero) ||
                   (className == MainWindowClass && mainWindows.Contains(owner));
        }

        /// <summary>광고 웹뷰가 들어 있는지.</summary>
        private bool ContainsChromeLegacyWindow(IntPtr window) =>
            _windows.GetDescendantWindows(window).Any(child => _windows.GetText(child) == ChromeLegacyWindowText);

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
