using System;
using System.Windows;
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
                NotifyRunningInstance(instanceGuard, options);
                return 0;
            }

            AppDomain.CurrentDomain.UnhandledException += (_, e) => ErrorLog.Write(e.ExceptionObject as Exception);

            // 트레이 메뉴(WinForms)에서 난 예외도 오류 대화상자 대신 기록만 한다.
            System.Windows.Forms.Application.SetUnhandledExceptionMode(System.Windows.Forms.UnhandledExceptionMode.CatchException);
            System.Windows.Forms.Application.ThreadException += (_, e) => ErrorLog.Write(e.Exception);

            return new App(options, instanceGuard).Run();
        }

        /// <summary>이미 트레이에서 실행 중이면 그쪽 설정창을 띄우고 끝낸다.</summary>
        private static void NotifyRunningInstance(SingleInstanceGuard instanceGuard, CommandLineOptions options)
        {
            if (instanceGuard.CanSignalFirstInstance)
            {
                User32.AllowSetForegroundWindow(User32.AsfwAny);
                instanceGuard.SignalFirstInstance();
            }
            else if (!options.IsAutostart)
            {
                MessageBox.Show(
                    "카카오톡 광고 차단이 이미 관리자 권한으로 실행 중이에요.\n알림 영역(트레이) 아이콘에서 설정을 열어 주세요.",
                    AppInfo.DisplayName,
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        }
    }
}
