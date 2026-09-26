using System;
using System.Runtime.InteropServices;

namespace KakaoTalkAdBlockPlus.Native
{
    /// <summary>
    /// 창 테두리 모양 (Windows 11: 제목 표시줄 색, 둥근 모서리). 지원하지 않는 Windows에서는 조용히 무시된다.
    /// </summary>
    internal static class Dwmapi
    {
        private const int UseImmersiveDarkMode = 20;
        private const int WindowCornerPreference = 33;
        private const int BorderColor = 34;
        private const int CaptionColor = 35;
        private const int TextColor = 36;
        private const int CornerRoundSmall = 3;

        /// <summary>제목 표시줄을 창 배경과 같은 색으로 맞춘다.</summary>
        public static void StyleCaption(IntPtr window, bool dark, uint captionRgb, uint textRgb)
        {
            SetInt(window, UseImmersiveDarkMode, dark ? 1 : 0);
            SetInt(window, CaptionColor, ToColorRef(captionRgb));
            SetInt(window, TextColor, ToColorRef(textRgb));
        }

        /// <summary>팝업 메뉴를 둥근 모서리로 만든다. 성공하면 true (Windows 11).</summary>
        public static bool TryRoundCorners(IntPtr window, uint borderRgb) =>
            SetInt(window, WindowCornerPreference, CornerRoundSmall) && SetInt(window, BorderColor, ToColorRef(borderRgb));

        /// <summary>0xRRGGBB → COLORREF(0x00BBGGRR)</summary>
        private static int ToColorRef(uint rgb) => (int)(((rgb & 0xFF) << 16) | (rgb & 0xFF00) | ((rgb >> 16) & 0xFF));

        private static bool SetInt(IntPtr window, int attribute, int value)
        {
            try
            {
                return DwmSetWindowAttribute(window, attribute, ref value, sizeof(int)) >= 0;
            }
            catch (DllNotFoundException)
            {
                return false;
            }
        }

        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr window, int attribute, ref int value, int size);
    }
}
