namespace KakaoTalkAdBlockPlus.Settings
{
    /// <summary>재부팅 후에도 남는 설정 저장소.</summary>
    public interface ISettingsStore
    {
        AppSettings Load();

        void Save(AppSettings settings);
    }
}
