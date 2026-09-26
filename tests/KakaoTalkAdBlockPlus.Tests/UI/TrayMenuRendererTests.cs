using System;
using System.Drawing;
using System.Windows.Forms;
using KakaoTalkAdBlockPlus.UI;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KakaoTalkAdBlockPlus.Tests.UI
{
    /// <summary>실제 메뉴를 비트맵으로 그려, 항목 글자가 위아래 가운데에 오는지 확인한다.</summary>
    [TestClass]
    public class TrayMenuRendererTests
    {
        private static readonly TrayMenuPalette Palette = new TrayMenuPalette(
            background: Color.White, border: Color.Gray, text: Color.Black,
            secondaryText: Color.DimGray, hover: Color.LightGray, separator: Color.Silver);

        [DataTestMethod]
        [DataRow("설정 열기")]
        [DataRow("종료")]
        public void ShouldDrawMenuItemTextVerticallyCentered(string text)
        {
            using var menu = new ContextMenuStrip
            {
                Renderer = new TrayMenuRenderer(Palette),
                Font = new Font("Malgun Gothic", 9f),
                ShowImageMargin = true,
            };
            // TrayMenu와 같은 여백 (96 DPI 기준)
            var item = new ToolStripMenuItem(text) { Padding = new Padding(4, 5, 12, 5) };
            menu.Items.Add(item);
            menu.PerformLayout();
            menu.Size = menu.GetPreferredSize(Size.Empty);

            using var bitmap = new Bitmap(menu.Width, menu.Height);
            menu.DrawToBitmap(bitmap, new Rectangle(Point.Empty, menu.Size));

            var bounds = item.Bounds;
            var (inkTop, inkBottom) = InkRows(bitmap, bounds);
            var gapAbove = inkTop - bounds.Top;
            var gapBelow = bounds.Bottom - 1 - inkBottom;
            Assert.IsTrue(Math.Abs(gapAbove - gapBelow) <= 1, $"항목 {bounds.Top}~{bounds.Bottom - 1}: 위 여백 {gapAbove}px, 아래 여백 {gapBelow}px");
        }

        private static (int Top, int Bottom) InkRows(Bitmap bitmap, Rectangle bounds)
        {
            int top = -1, bottom = -1;
            for (var y = bounds.Top; y < bounds.Bottom; y++)
            {
                // 가장자리 테두리 선은 빼고 본다.
                for (var x = 3; x < bitmap.Width - 3; x++)
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
