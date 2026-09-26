using System;
using System.IO;
using System.Reflection;

namespace KakaoTalkAdBlockPlus
{
    internal static class AppInfo
    {
        /// <summary>레지스트리 Run 값 이름, 설정 폴더 이름.</summary>
        public const string Name = "KakaoTalkAdBlockPlus";

        public const string DisplayName = "카카오톡 광고 차단";

        /// <summary>감시할 카카오톡 PC 실행 파일.</summary>
        public const string KakaoTalkExecutable = "KakaoTalk.exe";

        /// <summary>같은 사용자 세션에서 한 번만 실행되도록 쓰는 이름.</summary>
        public const string SingleInstanceName = Name + ".SingleInstance";

        /// <summary>%APPDATA%\KakaoTalkAdBlockPlus: 재부팅해도 남는다.</summary>
        public static string DataDirectory =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Name);

        public static string SettingsFile => Path.Combine(DataDirectory, "settings.json");

        public static string ErrorLogFile => Path.Combine(DataDirectory, "error.log");

        public static string ExecutablePath => Assembly.GetExecutingAssembly().Location;
    }
}
