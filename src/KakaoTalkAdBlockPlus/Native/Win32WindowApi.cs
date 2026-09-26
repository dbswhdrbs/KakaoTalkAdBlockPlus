using System;
using System.Collections.Generic;
using System.Text;
using KakaoTalkAdBlockPlus.AdBlock;

namespace KakaoTalkAdBlockPlus.Native
{
    /// <summary>IWindowApi의 실제 구현 (user32.dll).</summary>
    public sealed class Win32WindowApi : IWindowApi
    {
        public IReadOnlyList<IntPtr> GetTopLevelWindows()
        {
            var windows = new List<IntPtr>();
            User32.EnumWindows((window, _) =>
            {
                windows.Add(window);
                return true;
            }, IntPtr.Zero);
            return windows;
        }

        public IReadOnlyList<IntPtr> GetDescendantWindows(IntPtr parent) => throw new NotImplementedException();

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

        public WindowRect GetRect(IntPtr window) => throw new NotImplementedException();

        public bool IsVisible(IntPtr window) => throw new NotImplementedException();

        public void Close(IntPtr window) => throw new NotImplementedException();

        public void Hide(IntPtr window) => throw new NotImplementedException();

        public void Resize(IntPtr window, int width, int height) => throw new NotImplementedException();
    }
}
