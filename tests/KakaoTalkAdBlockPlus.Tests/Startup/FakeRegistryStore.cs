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

        public byte[]? GetBinary(string keyPath, string valueName) =>
            _values.TryGetValue(Key(keyPath, valueName), out var value) ? value as byte[] : null;

        /// <summary>테스트 준비용: 작업 관리자처럼 이진 값을 기록한다.</summary>
        public void SetBinary(string keyPath, string valueName, byte[] value) => _values[Key(keyPath, valueName)] = value;

        private static string Key(string keyPath, string valueName) => keyPath.ToLowerInvariant() + "|" + valueName.ToLowerInvariant();
    }
}
