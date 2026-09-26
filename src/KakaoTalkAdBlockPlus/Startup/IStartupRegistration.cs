namespace KakaoTalkAdBlockPlus.Startup
{
    /// <summary>윈도우 시작 시 자동 실행 켜기/끄기.</summary>
    public interface IStartupRegistration
    {
        bool IsEnabled { get; }

        void Enable();

        void Disable();
    }
}
