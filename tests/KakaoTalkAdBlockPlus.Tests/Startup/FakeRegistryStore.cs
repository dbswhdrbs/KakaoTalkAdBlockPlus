using System.Collections.Generic;
using KakaoTalkAdBlockPlus.Startup;

namespace KakaoTalkAdBlockPlus.Tests.Startup
{
    /// <summary>HKCU 대신 메모리에 값을 두는 가짜 레지스트리.</summary>
    internal sealed class FakeRegistryStore : IRegistryStore
    {
        private readonly Dictionary<string, object> _values = new Dictionary<string, object>();

        public string? GetString(string keyPath, string valueName) =>
            _values.TryGetValue(Key(keyPath, valueName), out var value) ? value as string : null;

        public void SetString(string keyPath, string valueName, string value) => _values[Key(keyPath, valueName)] = value;

        public void DeleteValue(string keyPath, string valueName) => _values.Remove(Key(keyPath, valueName));

        private static string Key(string keyPath, string valueName) => keyPath.ToLowerInvariant() + "|" + valueName.ToLowerInvariant();
    }
}
