using System;
using System.Runtime.InteropServices;

namespace KakaoTalkAdBlockPlus.Native
{
    internal static class User32
    {
        public const uint WmClose = 0x0010;
        public const uint SmtoAbortIfHung = 0x0002;
        public const int SwHide = 0;
        public const uint SwpNoMove = 0x0002;
        public const uint SwpNoActivate = 0x0010;
        public const uint SwpAsyncWindowPos = 0x4000;
        public const int AsfwAny = -1;

        public delegate bool EnumWindowsProc(IntPtr window, IntPtr parameter);

        [StructLayout(LayoutKind.Sequential)]
        public struct Rect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumWindows(EnumWindowsProc callback, IntPtr parameter);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumChildWindows(IntPtr parent, EnumWindowsProc callback, IntPtr parameter);

        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr window, out uint processId);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, EntryPoint = "GetClassNameW")]
        public static extern int GetClassName(IntPtr window, [Out] char[] className, int maxCount);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, EntryPoint = "GetWindowTextW")]
        public static extern int GetWindowText(IntPtr window, [Out] char[] text, int maxCount);

        [DllImport("user32.dll")]
        public static extern IntPtr GetParent(IntPtr window);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindowVisible(IntPtr window);

        /// <summary>창 주인 스레드가 처리할 때까지 기다리지 않는다 (카카오톡이 응답 없음일 때 멈추지 않게).</summary>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ShowWindowAsync(IntPtr window, int command);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect(IntPtr window, out Rect rect);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr window, IntPtr insertAfter, int x, int y, int width, int height, uint flags);

        /// <summary>두 번째 실행이 첫 번째 인스턴스의 설정창을 앞으로 가져올 수 있게 허락한다.</summary>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool AllowSetForegroundWindow(int processId);

        [DllImport("user32.dll", EntryPoint = "SendMessageTimeoutW")]
        public static extern IntPtr SendMessageTimeout(
            IntPtr window, uint message, IntPtr wParam, IntPtr lParam, uint flags, uint timeoutMilliseconds, out IntPtr result);
    }
}
