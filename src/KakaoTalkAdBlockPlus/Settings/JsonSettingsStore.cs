namespace KakaoTalkAdBlockPlus.Settings
{
    /// <summary>설정을 JSON 파일에 저장한다.</summary>
    public sealed class JsonSettingsStore
    {
        private readonly string _filePath;

        public JsonSettingsStore(string filePath)
        {
            _filePath = filePath;
        }

        public AppSettings Load() => AppSettings.Default;
    }
}
