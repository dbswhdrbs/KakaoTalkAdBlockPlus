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
