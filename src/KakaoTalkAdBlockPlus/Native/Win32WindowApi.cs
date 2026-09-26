using System;
using System.Collections.Generic;
using KakaoTalkAdBlockPlus.AdBlock;

namespace KakaoTalkAdBlockPlus.Native
{
    /// <summary>
    /// IWindowApi의 실제 구현 (user32.dll).
    /// 창 이름을 읽는 버퍼를 재사용하므로 한 스레드(광고 검사 스레드)에서만 쓴다.
    /// </summary>
    public sealed class Win32WindowApi : IWindowApi
    {
        private const uint CloseTimeoutMilliseconds = 1000;

        private readonly char[] _nameBuffer = new char[256];

        public IReadOnlyList<IntPtr> GetTopLevelWindows() => Collect(callback => User32.EnumWindows(callback, IntPtr.Zero));

        public IReadOnlyList<IntPtr> GetDescendantWindows(IntPtr parent) =>
            Collect(callback => User32.EnumChildWindows(parent, callback, IntPtr.Zero));

        public int GetProcessId(IntPtr window)
        {
            User32.GetWindowThreadProcessId(window, out var processId);
            return (int)processId;
        }

        public string GetClassName(IntPtr window) =>
            ToName(User32.GetClassName(window, _nameBuffer, _nameBuffer.Length));

        public string GetText(IntPtr window) =>
            ToName(User32.GetWindowText(window, _nameBuffer, _nameBuffer.Length));

        public IntPtr GetParent(IntPtr window) => User32.GetParent(window);

        public WindowRect GetRect(IntPtr window) =>
            User32.GetWindowRect(window, out var rect) ? new WindowRect(rect.Left, rect.Top, rect.Right, rect.Bottom) : default;

        public bool IsVisible(IntPtr window) => User32.IsWindowVisible(window);

        /// <summary>원본은 SendMessage였지만, 카카오톡이 응답하지 않을 때 멈추지 않도록 시간 제한을 둔다.</summary>
        public void Close(IntPtr window) =>
            User32.SendMessageTimeout(window, User32.WmClose, IntPtr.Zero, IntPtr.Zero, User32.SmtoAbortIfHung, CloseTimeoutMilliseconds, out _);

        public void Hide(IntPtr window) => User32.ShowWindowAsync(window, User32.SwHide);

        /// <summary>
        /// 원본 HideMainViewAdArea와 같이 SetWindowPos(HWND_TOP, SWP_NOMOVE)로 크기를 바꾼다.
        /// 원본의 UpdateWindow는 빼고 요청만 보낸다: 카카오톡이 응답 없음이어도 기다리지 않는다.
        /// </summary>
        public void Resize(IntPtr window, int width, int height) =>
            User32.SetWindowPos(window, IntPtr.Zero, 0, 0, width, height,
                User32.SwpNoMove | User32.SwpNoActivate | User32.SwpAsyncWindowPos);

        private string ToName(int length) => length > 0 ? new string(_nameBuffer, 0, length) : string.Empty;

        private static IReadOnlyList<IntPtr> Collect(Action<User32.EnumWindowsProc> enumerate)
        {
            var windows = new List<IntPtr>();
            enumerate((window, _) =>
            {
                windows.Add(window);
                return true;
            });
            return windows;
        }
    }
}
