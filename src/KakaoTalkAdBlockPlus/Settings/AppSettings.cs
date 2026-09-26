namespace KakaoTalkAdBlockPlus.Settings
{
    /// <summary>재부팅 후에도 유지되는 사용자 설정.</summary>
    public sealed class AppSettings
    {
        public AppSettings(CheckInterval checkInterval)
        {
            CheckInterval = checkInterval;
        }

        public static AppSettings Default => new AppSettings(CheckInterval.Default);

        public CheckInterval CheckInterval { get; }
    }
}
