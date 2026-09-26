using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Linq;

namespace KakaoTalkAdBlockPlus.UI
{
    /// <summary>윈도우 기본 아이콘 글꼴(Segoe Fluent Icons / Segoe MDL2 Assets)로 메뉴 아이콘을 그린다.</summary>
    internal static class MenuGlyphs
    {
        public const string Settings = "";
        public const string Power = "";

        private static readonly string? IconFontName = FindIconFont();

        /// <summary>아이콘 글꼴이 없으면 null (아이콘 없이 글자만 보인다).</summary>
        public static Bitmap? Render(string glyph, Color color, int size)
        {
            if (IconFontName == null) return null;

            var bitmap = new Bitmap(size, size, PixelFormat.Format32bppArgb);
            using var graphics = Graphics.FromImage(bitmap);
            using var font = new Font(IconFontName, size * 0.78f, GraphicsUnit.Pixel);
            using var brush = new SolidBrush(color);
            using var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            graphics.DrawString(glyph, font, brush, new RectangleF(0, 0, size, size), format);
            return bitmap;
        }

        private static string? FindIconFont()
        {
            using var fonts = new InstalledFontCollection();
            var installed = fonts.Families.Select(family => family.Name).ToList();
            return new[] { "Segoe Fluent Icons", "Segoe MDL2 Assets" }.FirstOrDefault(installed.Contains);
        }
    }
}
