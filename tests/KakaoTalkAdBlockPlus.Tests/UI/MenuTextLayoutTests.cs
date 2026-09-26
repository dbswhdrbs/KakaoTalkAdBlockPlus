using System;
using System.Drawing;
using System.Windows.Forms;
using KakaoTalkAdBlockPlus.UI;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KakaoTalkAdBlockPlus.Tests.UI
{
    /// <summary>실제 글꼴로 그려 보고, 칠해진 글자가 메뉴 항목의 위아래 가운데에 오는지 확인한다.</summary>
    [TestClass]
    public class MenuTextLayoutTests
    {
        private const int ItemHeight = 30;

        [DataTestMethod]
        [DataRow("설정 열기", false)]
        [DataRow("종료", false)]
        [DataRow("카카오톡 광고 차단", true)]
        [DataRow("광고를 차단하고 있어요 · 3개 정리", false)]
        public void ShouldCenterMenuTextInkVertically(string text, bool bold)
        {
            using var font = new Font("Malgun Gothic", 9f, bold ? FontStyle.Bold : FontStyle.Regular);

            var top = MenuTextLayout.CenteredTop(text, font, ItemHeight);

            using var bitmap = new Bitmap(320, ItemHeight);
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.Clear(Color.White);
                TextRenderer.DrawText(graphics, text, font, new Rectangle(0, top, 320, font.Height), Color.Black, MenuTextLayout.Flags);
            }

            var (inkTop, inkBottom) = InkRows(bitmap);
            var gapAbove = inkTop;
            var gapBelow = ItemHeight - 1 - inkBottom;
            Assert.IsTrue(Math.Abs(gapAbove - gapBelow) <= 1, $"위 여백 {gapAbove}px, 아래 여백 {gapBelow}px");
        }

        private static (int Top, int Bottom) InkRows(Bitmap bitmap)
        {
            int top = -1, bottom = -1;
            for (var y = 0; y < bitmap.Height; y++)
            {
                for (var x = 0; x < bitmap.Width; x++)
                {
                    if (bitmap.GetPixel(x, y).GetBrightness() > 0.6f) continue;

                    if (top < 0) top = y;
                    bottom = y;
                    break;
                }
            }

            Assert.IsTrue(top >= 0, "글자가 그려지지 않았다");
            return (top, bottom);
        }
    }
}
