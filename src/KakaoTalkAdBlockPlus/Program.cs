using System;
using KakaoTalkAdBlockPlus.Native;
using KakaoTalkAdBlockPlus.Startup;

namespace KakaoTalkAdBlockPlus
{
    internal static class Program
    {
        [STAThread]
        private static int Main(string[] args)
        {
            var options = CommandLineOptions.Parse(args);
            using var instanceGuard = SingleInstanceGuard.Acquire(AppInfo.SingleInstanceName);
            if (!instanceGuard.IsFirstInstance)
            {
                // 이미 트레이에서 실행 중이면 그쪽 설정창을 띄우고 끝낸다.
                User32.AllowSetForegroundWindow(User32.AsfwAny);
                instanceGuard.SignalFirstInstance();
                return 0;
            }

            AppDomain.CurrentDomain.UnhandledException += (_, e) => ErrorLog.Write(e.ExceptionObject as Exception);
            return new App(options, instanceGuard).Run();
        }
    }
}
