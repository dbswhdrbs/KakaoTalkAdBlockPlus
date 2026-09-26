using System;
using System.Windows;
using System.Windows.Media;
using Microsoft.Win32;

namespace KakaoTalkAdBlockPlus.UI
{
    /// <summary>
    /// 윈도우 앱 테마(라이트/다크)를 따라 색 사전(Light.xaml / Dark.xaml)을 바꿔 끼운다.
    /// 색 사전은 Application 리소스의 첫 번째 병합 사전이다.
    /// </summary>
    internal sealed class ThemeManager : IDisposable
    {
        private const string PersonalizeKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";

        private static readonly Uri LightPalette = new Uri("pack://application:,,,/KakaoTalkAdBlockPlus;component/UI/Themes/Light.xaml");
        private static readonly Uri DarkPalette = new Uri("pack://application:,,,/KakaoTalkAdBlockPlus;component/UI/Themes/Dark.xaml");

        private readonly Application _application;

        public ThemeManager(Application application)
        {
            _application = application;
            IsDark = ReadSystemPrefersDark();
            ApplyPalette();
            SystemEvents.UserPreferenceChanged += OnUserPreferenceChanged;
        }

        public event EventHandler? ThemeChanged;

        public bool IsDark { get; private set; }

        /// <summary>현재 색 사전의 브러시 색 (0xRRGGBB). 트레이 메뉴처럼 WPF가 아닌 곳에서 쓴다.</summary>
        public uint GetRgb(string brushKey)
        {
            var color = ((SolidColorBrush)_application.Resources.MergedDictionaries[0][brushKey]).Color;
            return ((uint)color.R << 16) | ((uint)color.G << 8) | color.B;
        }

        public void Dispose() => SystemEvents.UserPreferenceChanged -= OnUserPreferenceChanged;

        private static bool ReadSystemPrefersDark()
        {
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(PersonalizeKeyPath);
                return key?.GetValue("AppsUseLightTheme") is int useLightTheme && useLightTheme == 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void ApplyPalette() =>
            _application.Resources.MergedDictionaries[0] = new ResourceDictionary { Source = IsDark ? DarkPalette : LightPalette };

        private void OnUserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
        {
            if (e.Category != UserPreferenceCategory.General) return;

            _application.Dispatcher.BeginInvoke(new Action(() =>
            {
                var dark = ReadSystemPrefersDark();
                if (dark == IsDark) return;

                IsDark = dark;
                ApplyPalette();
                ThemeChanged?.Invoke(this, EventArgs.Empty);
            }));
        }
    }
}
