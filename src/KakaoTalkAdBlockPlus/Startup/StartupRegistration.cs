namespace KakaoTalkAdBlockPlus.Startup
{
    /// <summary>윈도우 시작 시 자동 실행 (HKCU\...\CurrentVersion\Run).</summary>
    public sealed class StartupRegistration
    {
        public const string RunKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";

        private readonly IRegistryStore _registry;
        private readonly string _appName;
        private readonly string _executablePath;

        public StartupRegistration(IRegistryStore registry, string appName, string executablePath)
        {
            _registry = registry;
            _appName = appName;
            _executablePath = executablePath;
        }

        public bool IsEnabled => false;

        public void Enable() => _registry.SetString(RunKeyPath, _appName, "\"" + _executablePath + "\" --autostart");
    }
}
