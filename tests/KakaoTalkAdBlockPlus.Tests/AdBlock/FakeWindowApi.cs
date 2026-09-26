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

        public List<ResizedWindow> Resized { get; } = new List<ResizedWindow>();

        public FakeWindow AddTopLevel(int processId, string className, string text, FakeWindow? owner = null)
        {
            var window = Register(new FakeWindow(NextHandle(), processId, className, text, parent: null) { Owner = owner });
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

        public IntPtr GetParent(IntPtr window) =>
            _windows[window].Parent?.Handle ?? _windows[window].Owner?.Handle ?? IntPtr.Zero;

        public WindowRect GetRect(IntPtr window) => _windows[window].Rect;

        public void Close(IntPtr window) => Closed.Add(window);

        public void Resize(IntPtr window, int width, int height) => Resized.Add(new ResizedWindow(window, width, height));

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

        /// <summary>최상위 창의 소유자 (GetParent가 돌려주는 값).</summary>
        public FakeWindow? Owner { get; set; }

        public List<FakeWindow> Children { get; } = new List<FakeWindow>();

        public WindowRect Rect { get; set; } = new WindowRect(0, 0, 400, 600);
    }

    /// <summary>엔진이 Resize를 부른 기록.</summary>
    internal readonly struct ResizedWindow : IEquatable<ResizedWindow>
    {
        public ResizedWindow(IntPtr window, int width, int height)
        {
            Window = window;
            Width = width;
            Height = height;
        }

        public IntPtr Window { get; }

        public int Width { get; }

        public int Height { get; }

        public bool Equals(ResizedWindow other) => Window == other.Window && Width == other.Width && Height == other.Height;

        public override bool Equals(object? obj) => obj is ResizedWindow other && Equals(other);

        public override int GetHashCode() => (Window, Width, Height).GetHashCode();

        public override string ToString() => $"0x{Window.ToInt64():X} → {Width}x{Height}";
    }
}
