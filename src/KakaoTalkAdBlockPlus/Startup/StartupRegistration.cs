namespace KakaoTalkAdBlockPlus.Startup
{
    /// <summary>윈도우 시작 시 자동 실행 (HKCU\...\CurrentVersion\Run).</summary>
    public sealed class StartupRegistration
    {
        public const string RunKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";

        /// <summary>작업 관리자 [시작 앱] 탭의 사용/사용 안 함 상태가 기록되는 곳.</summary>
        public const string StartupApprovedKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\Run";

        private readonly IRegistryStore _registry;
        private readonly string _appName;
        private readonly string _executablePath;

        public StartupRegistration(IRegistryStore registry, string appName, string executablePath)
        {
            _registry = registry;
            _appName = appName;
            _executablePath = executablePath;
        }

        public bool IsEnabled =>
            _registry.GetString(RunKeyPath, _appName) != null && !IsDisabledInTaskManager;

        /// <summary>첫 바이트가 홀수(0x03 등)면 작업 관리자에서 "사용 안 함"으로 바꾼 상태다.</summary>
        private bool IsDisabledInTaskManager =>
            _registry.GetBinary(StartupApprovedKeyPath, _appName) is { Length: > 0 } flag && (flag[0] & 1) == 1;

        public void Enable() => _registry.SetString(RunKeyPath, _appName, "\"" + _executablePath + "\" --autostart");

        public void Disable() => _registry.DeleteValue(RunKeyPath, _appName);
    }
}
