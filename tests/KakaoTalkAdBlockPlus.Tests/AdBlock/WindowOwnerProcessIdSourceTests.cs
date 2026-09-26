using System.Collections.Generic;
using KakaoTalkAdBlockPlus.AdBlock;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KakaoTalkAdBlockPlus.Tests.AdBlock
{
    [TestClass]
    public class WindowOwnerProcessIdSourceTests
    {
        private readonly FakeWindowApi _windows = new FakeWindowApi();
        private readonly FakeProcessNames _names = new FakeProcessNames();

        private WindowOwnerProcessIdSource CreateSource() => new WindowOwnerProcessIdSource(_windows, _names, "kakaotalk.exe");

        [TestMethod]
        public void ShouldFindWindowOwnersByImageName()
        {
            _names.Add(10, "KakaoTalk.exe");
            _names.Add(20, "explorer.exe");
            _windows.AddTopLevel(10, "EVA_Window_Dblclk", "카카오톡");
            _windows.AddTopLevel(10, "EVA_Window", "");
            _windows.AddTopLevel(20, "Shell_TrayWnd", "");

            var ids = CreateSource().GetProcessIds();

            CollectionAssert.AreEquivalent(new[] { 10 }, new List<int>(ids));
        }

        [TestMethod]
        public void ShouldResolveEachProcessNameOnce()
        {
            _names.Add(10, "KakaoTalk.exe");
            _names.Add(20, "explorer.exe");
            _windows.AddTopLevel(10, "EVA_Window_Dblclk", "카카오톡");
            _windows.AddTopLevel(20, "Shell_TrayWnd", "");
            var source = CreateSource();

            source.GetProcessIds();
            source.GetProcessIds();

            CollectionAssert.AreEquivalent(new[] { 10, 20 }, _names.Resolved);
        }

        [TestMethod]
        public void ShouldForgetProcessesWithoutWindows()
        {
            _names.Add(10, "KakaoTalk.exe");
            var window = _windows.AddTopLevel(10, "EVA_Window_Dblclk", "카카오톡");
            var source = CreateSource();
            source.GetProcessIds();

            // 카카오톡이 끝나고 창이 사라진 뒤, 같은 PID를 다른 프로그램이 받았다.
            _windows.Remove(window);
            source.GetProcessIds();
            _names.Add(10, "notepad.exe");
            _windows.AddTopLevel(10, "Notepad", "메모장");

            CollectionAssert.AreEquivalent(new int[0], new List<int>(source.GetProcessIds()));
        }

        private sealed class FakeProcessNames : IProcessNameResolver
        {
            private readonly Dictionary<int, string> _names = new Dictionary<int, string>();

            public List<int> Resolved { get; } = new List<int>();

            public void Add(int processId, string imageName) => _names[processId] = imageName;

            public string? GetImageName(int processId)
            {
                Resolved.Add(processId);
                return _names.TryGetValue(processId, out var name) ? name : null;
            }
        }
    }
}
