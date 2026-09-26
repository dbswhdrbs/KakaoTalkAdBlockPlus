using System.Drawing;
using System.Windows.Forms;

namespace KakaoTalkAdBlockPlus.UI
{
    /// <summary>트레이 메뉴 글자를 항목의 위아래 가운데에 놓는다.</summary>
    public static class MenuTextLayout
    {
        /// <summary>글자를 재고 그릴 때 함께 쓰는 설정: 한 줄, 위쪽 기준.</summary>
        public const TextFormatFlags Flags =
            TextFormatFlags.Left | TextFormatFlags.Top | TextFormatFlags.SingleLine | TextFormatFlags.NoPrefix;

        /// <summary>areaHeight 높이의 영역에서 글자 줄의 위쪽 y.</summary>
        public static int CenteredTop(string text, Font font, int areaHeight) => (areaHeight - font.Height) / 2;
    }
}
