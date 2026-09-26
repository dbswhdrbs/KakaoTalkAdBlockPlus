using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows;
using System.Windows.Threading;
using KakaoTalkAdBlockPlus.AdBlock;
using KakaoTalkAdBlockPlus.Native;
using KakaoTalkAdBlockPlus.Settings;
using KakaoTalkAdBlockPlus.Startup;
using KakaoTalkAdBlockPlus.UI;

namespace KakaoTalkAdBlockPlus
{
    /// <summary>
    /// 프로그램 조립: 실행되면 창 없이 트레이에 머물며 광고 차단을 시작한다.
    /// 설정창은 트레이 메뉴나 두 번째 실행으로만 연다.
    /// </summary>
    public partial class App : Application
    {
        private static readonly TimeSpan ProcessListRefreshPeriod = TimeSpan.FromSeconds(1);

        private readonly CommandLineOptions _options;
        private readonly SingleInstanceGuard _instanceGuard;
        private ThemeManager? _theme;
        private ISettingsStore? _settingsStore;
        private IStartupRegistration? _startup;
        private AdBlockService? _service;
        private TrayIcon? _tray;
        private SettingsWindow? _settingsWindow;

        internal App(CommandLineOptions options, SingleInstanceGuard instanceGuard)
        {
            _options = options;
            _instanceGuard = instanceGuard;
            InitializeComponent();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            DispatcherUnhandledException += OnDispatcherUnhandledException;

            _theme = new ThemeManager(this);
            _settingsStore = new JsonSettingsStore(AppInfo.SettingsFile);
            _startup = CreateStartupRegistration();

            _service = new AdBlockService(CreateEngine(), _settingsStore.Load().CheckInterval);
            _service.Start();

            _tray = new TrayIcon(LoadTrayIcon(), _theme, () => _service.Status, ShowSettings, ExitApplication);
            _instanceGuard.ListenForSignal(() => Dispatcher.BeginInvoke(new Action(ShowSettings)));

            if (!_options.IsAutostart) _tray.ShowStartedBalloon();
        }

        private static AdBlockEngine CreateEngine()
        {
            var clock = Stopwatch.StartNew();
            var kakaoTalkProcesses = new CachedProcessIdSource(
                new ToolhelpProcessIdSource(AppInfo.KakaoTalkExecutable), ProcessListRefreshPeriod, () => clock.Elapsed);
            return new AdBlockEngine(new Win32WindowApi(), kakaoTalkProcesses);
        }

        private static IStartupRegistration CreateStartupRegistration()
        {
            var startup = new StartupRegistration(new CurrentUserRegistryStore(), AppInfo.Name, AppInfo.ExecutablePath);
            try
            {
                // 실행 파일을 옮겼어도 다음 로그인 때 제대로 시작되게 한다.
                startup.RepairIfEnabled();
            }
            catch (Exception exception)
            {
                ErrorLog.Write(exception);
            }

            return startup;
        }

        private static Icon LoadTrayIcon()
        {
            var resource = GetResourceStream(new Uri("pack://application:,,,/KakaoTalkAdBlockPlus;component/Assets/AppIcon.ico"));
            using var stream = resource.Stream;
            return new Icon(stream, System.Windows.Forms.SystemInformation.SmallIconSize);
        }

        private void ShowSettings()
        {
            if (_settingsWindow != null)
            {
                if (_settingsWindow.WindowState == WindowState.Minimized) _settingsWindow.WindowState = WindowState.Normal;
                _settingsWindow.Activate();
                return;
            }

            _settingsWindow = new SettingsWindow(new SettingsViewModel(_settingsStore!, _service!, _startup!), _theme!);
            _settingsWindow.Closed += (_, _) => _settingsWindow = null;
            _settingsWindow.Show();
            _settingsWindow.Activate();
        }

        private void ExitApplication()
        {
            _settingsWindow?.Close();
            _tray?.Dispose();
            _service?.Dispose();
            _theme?.Dispose();
            Shutdown();
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            // 화면 쪽 오류 하나로 광고 차단까지 멈추지 않는다.
            ErrorLog.Write(e.Exception);
            e.Handled = true;
        }
    }
}
