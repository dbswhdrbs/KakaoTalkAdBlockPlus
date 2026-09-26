namespace KakaoTalkAdBlockPlus.Startup
{
    /// <summary>현재 사용자(HKCU) 레지스트리 값 접근.</summary>
    public interface IRegistryStore
    {
        /// <summary>값이 없거나 문자열이 아니면 null.</summary>
        string? GetString(string keyPath, string valueName);

        /// <summary>키가 없으면 만든다.</summary>
        void SetString(string keyPath, string valueName, string value);
    }
}
