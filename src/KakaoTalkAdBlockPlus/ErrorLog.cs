using System;
using System.IO;

namespace KakaoTalkAdBlockPlus
{
    /// <summary>예상하지 못한 오류를 %APPDATA%\KakaoTalkAdBlockPlus\error.log에 남긴다 (문제 해결용).</summary>
    internal static class ErrorLog
    {
        private const long MaxBytes = 512 * 1024;
        private static readonly object Gate = new object();

        public static void Write(Exception? exception)
        {
            if (exception == null) return;

            try
            {
                lock (Gate)
                {
                    Directory.CreateDirectory(AppInfo.DataDirectory);
                    var file = new FileInfo(AppInfo.ErrorLogFile);
                    if (file.Exists && file.Length > MaxBytes) file.Delete();
                    File.AppendAllText(AppInfo.ErrorLogFile, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {exception}{Environment.NewLine}");
                }
            }
            catch (Exception)
            {
                // 기록 실패로 프로그램을 멈추지 않는다.
            }
        }
    }
}
