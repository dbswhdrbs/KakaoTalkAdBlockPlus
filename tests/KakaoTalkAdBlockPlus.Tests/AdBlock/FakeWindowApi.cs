using System;
using System.Collections.Generic;
using System.Linq;
using KakaoTalkAdBlockPlus.AdBlock;

namespace KakaoTalkAdBlockPlus.Tests.AdBlock
{
    /// <summary>진짜 창 대신 메모리 속 창 트리를 흉내 내고, 엔진이 한 일을 기록한다.</summary>
    internal sealed class FakeWindowApi : IWindowApi
    {
        private readonly List<FakeWindow> _topLevelWindows = new List<FakeWindow>();
        private readonly Dictionary<IntPtr, FakeWindow> _windows = new Dictionary<IntPtr, FakeWindow>();
        private int _nextHandle = 0x1000;

        public List<IntPtr> Closed { get; } = new List<IntPtr>();

        public FakeWindow AddTopLevel(int processId, string className, string text)
        {
            var window = Register(new FakeWindow(NextHandle(), processId, className, text, parent: null));
            _topLevelWindows.Add(window);
            return window;
        }

        public FakeWindow AddChild(FakeWindow parent, string className, string text)
        {
            var window = Register(new FakeWindow(NextHandle(), parent.ProcessId, className, text, parent));
            parent.Children.Add(window);
            return window;
        }

        public IReadOnlyList<IntPtr> GetTopLevelWindows() => _topLevelWindows.Select(w => w.Handle).ToList();

        /// <summary>EnumChildWindows처럼 모든 자손을 전위 순서로 돌려준다.</summary>
        public IReadOnlyList<IntPtr> GetDescendantWindows(IntPtr parent) =>
            Descendants(_windows[parent]).Select(w => w.Handle).ToList();

        public int GetProcessId(IntPtr window) => _windows[window].ProcessId;

        public string GetClassName(IntPtr window) => _windows[window].ClassName;

        public string GetText(IntPtr window) => _windows[window].Text;

        public void Close(IntPtr window) => Closed.Add(window);

        private static IEnumerable<FakeWindow> Descendants(FakeWindow window) =>
            window.Children.SelectMany(child => new[] { child }.Concat(Descendants(child)));

        private IntPtr NextHandle() => new IntPtr(_nextHandle += 0x10);

        private FakeWindow Register(FakeWindow window)
        {
            _windows.Add(window.Handle, window);
            return window;
        }
    }

    internal sealed class FakeWindow
    {
        public FakeWindow(IntPtr handle, int processId, string className, string text, FakeWindow? parent)
        {
            Handle = handle;
            ProcessId = processId;
            ClassName = className;
            Text = text;
            Parent = parent;
        }

        public IntPtr Handle { get; }

        public int ProcessId { get; }

        public string ClassName { get; }

        public string Text { get; }

        public FakeWindow? Parent { get; }

        public List<FakeWindow> Children { get; } = new List<FakeWindow>();
    }
}
