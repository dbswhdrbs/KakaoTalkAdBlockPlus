using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace KakaoTalkAdBlockPlus.Tests.Native
{
    /// <summary>
    /// 테스트 프로세스 안에 원하는 클래스 이름의 진짜 Win32 창을 만든다.
    /// 최상위 창은 화면 밖 작은 도구 창이라 사용자 화면에 보이지 않고 포커스도 빼앗지 않는다.
    /// 만든 스레드에서 Dispose해야 한다.
    /// </summary>
    internal sealed class TestWindowFactory : IDisposable
    {
        private const uint WsPopup = 0x80000000;
        private const uint WsChild = 0x40000000;
        private const uint WsVisible = 0x10000000;
        private const uint WsExToolWindow = 0x00000080;
        private const uint WsExNoActivate = 0x08000000;
        private const int SwShowNoActivate = 8;
        private const int ErrorClassAlreadyExists = 1410;
        private const uint PmRemove = 0x0001;

        private static readonly WndProc WindowProcedure = DefWindowProc;
        private static readonly HashSet<string> RegisteredClasses = new HashSet<string>();

        private readonly List<IntPtr> _topLevelWindows = new List<IntPtr>();

        /// <param name="owner">지정하면 WS_POPUP 소유 창이 되어 GetParent가 소유자를 돌려준다.</param>
        public IntPtr CreateTopLevel(string className, string text, IntPtr owner = default, bool visible = false)
        {
            var window = Create(className, text, WsPopup, WsExToolWindow | WsExNoActivate, owner, -32000, -32000, 400, 600);
            if (visible) ShowWindow(window, SwShowNoActivate);
            _topLevelWindows.Add(window);
            return window;
        }

        public IntPtr CreateChild(IntPtr parent, string className, string text) =>
            Create(className, text, WsChild | WsVisible, 0, parent, 0, 0, 100, 100);

        public static bool Exists(IntPtr window) => IsWindow(window);

        /// <summary>이 스레드의 메시지 큐를 비운다 (ShowWindowAsync처럼 게시된 요청을 처리시킨다).</summary>
        public static void PumpMessages()
        {
            while (PeekMessage(out var message, IntPtr.Zero, 0, 0, PmRemove))
            {
                TranslateMessage(ref message);
                DispatchMessage(ref message);
            }
        }

        public void Dispose()
        {
            // 소유된 창부터 닫히도록 나중에 만든 창부터 없앤다. 자식 창은 부모와 함께 없어진다.
            for (var i = _topLevelWindows.Count - 1; i >= 0; i--)
            {
                if (IsWindow(_topLevelWindows[i])) DestroyWindow(_topLevelWindows[i]);
            }
        }

        private static IntPtr Create(string className, string text, uint style, uint exStyle, IntPtr parent, int x, int y, int width, int height)
        {
            Register(className);
            var window = CreateWindowEx(exStyle, className, text, style, x, y, width, height, parent, IntPtr.Zero, GetModuleHandle(null), IntPtr.Zero);
            if (window == IntPtr.Zero) throw new Win32Exception(Marshal.GetLastWin32Error());
            return window;
        }

        private static void Register(string className)
        {
            if (!RegisteredClasses.Add(className)) return;

            var windowClass = new WndClassEx
            {
                Size = (uint)Marshal.SizeOf<WndClassEx>(),
                WindowProcedure = WindowProcedure,
                Instance = GetModuleHandle(null),
                ClassName = className,
            };
            if (RegisterClassEx(ref windowClass) == 0 && Marshal.GetLastWin32Error() != ErrorClassAlreadyExists)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }

        private delegate IntPtr WndProc(IntPtr window, uint message, IntPtr wParam, IntPtr lParam);

        [StructLayout(LayoutKind.Sequential)]
        private struct Message
        {
            public IntPtr Window;
            public uint Id;
            public IntPtr WParam;
            public IntPtr LParam;
            public uint Time;
            public int X;
            public int Y;
        }

        [DllImport("user32.dll", EntryPoint = "PeekMessageW")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool PeekMessage(out Message message, IntPtr window, uint filterMin, uint filterMax, uint remove);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool TranslateMessage(ref Message message);

        [DllImport("user32.dll", EntryPoint = "DispatchMessageW")]
        private static extern IntPtr DispatchMessage(ref Message message);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct WndClassEx
        {
            public uint Size;
            public uint Style;
            public WndProc WindowProcedure;
            public int ClassExtra;
            public int WindowExtra;
            public IntPtr Instance;
            public IntPtr Icon;
            public IntPtr Cursor;
            public IntPtr Background;
            public string? MenuName;
            public string ClassName;
            public IntPtr SmallIcon;
        }

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true, EntryPoint = "RegisterClassExW")]
        private static extern ushort RegisterClassEx(ref WndClassEx windowClass);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true, EntryPoint = "CreateWindowExW")]
        private static extern IntPtr CreateWindowEx(
            uint exStyle, string className, string windowName, uint style,
            int x, int y, int width, int height, IntPtr parent, IntPtr menu, IntPtr instance, IntPtr param);

        [DllImport("user32.dll", EntryPoint = "DefWindowProcW")]
        private static extern IntPtr DefWindowProc(IntPtr window, uint message, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DestroyWindow(IntPtr window);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsWindow(IntPtr window);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ShowWindow(IntPtr window, int command);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, EntryPoint = "GetModuleHandleW")]
        private static extern IntPtr GetModuleHandle(string? moduleName);
    }
}
