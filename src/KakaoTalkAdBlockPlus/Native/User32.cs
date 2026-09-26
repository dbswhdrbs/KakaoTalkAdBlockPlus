using System;
using System.Runtime.InteropServices;
using System.Text;

namespace KakaoTalkAdBlockPlus.Native
{
    internal static class User32
    {
        public const uint WmClose = 0x0010;
        public const uint SmtoAbortIfHung = 0x0002;
        public const int SwHide = 0;

        public delegate bool EnumWindowsProc(IntPtr window, IntPtr parameter);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumWindows(EnumWindowsProc callback, IntPtr parameter);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumChildWindows(IntPtr parent, EnumWindowsProc callback, IntPtr parameter);

        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr window, out uint processId);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, EntryPoint = "GetClassNameW")]
        public static extern int GetClassName(IntPtr window, StringBuilder className, int maxCount);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, EntryPoint = "GetWindowTextW")]
        public static extern int GetWindowText(IntPtr window, StringBuilder text, int maxCount);

        [DllImport("user32.dll")]
        public static extern IntPtr GetParent(IntPtr window);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindowVisible(IntPtr window);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ShowWindow(IntPtr window, int command);

        [DllImport("user32.dll", EntryPoint = "SendMessageTimeoutW")]
        public static extern IntPtr SendMessageTimeout(
            IntPtr window, uint message, IntPtr wParam, IntPtr lParam, uint flags, uint timeoutMilliseconds, out IntPtr result);
    }
}
