using System;
using System.Collections.Generic;
using System.Text;
using KakaoTalkAdBlockPlus.AdBlock;

namespace KakaoTalkAdBlockPlus.Native
{
    /// <summary>IWindowApi의 실제 구현 (user32.dll).</summary>
    public sealed class Win32WindowApi : IWindowApi
    {
        private const uint CloseTimeoutMilliseconds = 1000;

        public IReadOnlyList<IntPtr> GetTopLevelWindows() => Collect(callback => User32.EnumWindows(callback, IntPtr.Zero));

        public IReadOnlyList<IntPtr> GetDescendantWindows(IntPtr parent) =>
            Collect(callback => User32.EnumChildWindows(parent, callback, IntPtr.Zero));

        public int GetProcessId(IntPtr window)
        {
            User32.GetWindowThreadProcessId(window, out var processId);
            return (int)processId;
        }

        public string GetClassName(IntPtr window)
        {
            var className = new StringBuilder(256);
            User32.GetClassName(window, className, className.Capacity);
            return className.ToString();
        }

        public string GetText(IntPtr window)
        {
            var text = new StringBuilder(256);
            User32.GetWindowText(window, text, text.Capacity);
            return text.ToString();
        }

        public IntPtr GetParent(IntPtr window) => User32.GetParent(window);

        public WindowRect GetRect(IntPtr window) =>
            User32.GetWindowRect(window, out var rect) ? new WindowRect(rect.Left, rect.Top, rect.Right, rect.Bottom) : default;

        public bool IsVisible(IntPtr window) => User32.IsWindowVisible(window);

        /// <summary>원본은 SendMessage였지만, 카카오톡이 응답하지 않을 때 멈추지 않도록 시간 제한을 둔다.</summary>
        public void Close(IntPtr window) =>
            User32.SendMessageTimeout(window, User32.WmClose, IntPtr.Zero, IntPtr.Zero, User32.SmtoAbortIfHung, CloseTimeoutMilliseconds, out _);

        public void Hide(IntPtr window) => User32.ShowWindow(window, User32.SwHide);

        /// <summary>원본 HideMainViewAdArea와 같이 UpdateWindow 후 SetWindowPos(HWND_TOP, SWP_NOMOVE).</summary>
        public void Resize(IntPtr window, int width, int height)
        {
            User32.UpdateWindow(window);
            User32.SetWindowPos(window, IntPtr.Zero, 0, 0, width, height, User32.SwpNoMove);
        }

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
