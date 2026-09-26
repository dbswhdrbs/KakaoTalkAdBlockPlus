using Microsoft.Win32;

namespace KakaoTalkAdBlockPlus.Startup
{
    /// <summary>실제 HKCU 레지스트리. 관리자 권한이 필요 없다.</summary>
    public sealed class CurrentUserRegistryStore : IRegistryStore
    {
        public string? GetString(string keyPath, string valueName) => GetValue(keyPath, valueName) as string;

        public byte[]? GetBinary(string keyPath, string valueName) => GetValue(keyPath, valueName) as byte[];

        public void SetString(string keyPath, string valueName, string value)
        {
            using var key = Registry.CurrentUser.CreateSubKey(keyPath);
            key.SetValue(valueName, value, RegistryValueKind.String);
        }

        public void DeleteValue(string keyPath, string valueName)
        {
            using var key = Registry.CurrentUser.OpenSubKey(keyPath, writable: true);
            key?.DeleteValue(valueName, throwOnMissingValue: false);
        }

        private static object? GetValue(string keyPath, string valueName)
        {
            using var key = Registry.CurrentUser.OpenSubKey(keyPath);
            return key?.GetValue(valueName);
        }
    }
}
