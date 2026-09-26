using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace KakaoTalkAdBlockPlus.UI
{
    /// <summary>트레이 우클릭 메뉴 색 (테마에서 가져온다).</summary>
    public sealed class TrayMenuPalette
    {
        public TrayMenuPalette(Color background, Color border, Color text, Color secondaryText, Color hover, Color separator)
        {
            Background = background;
            Border = border;
            Text = text;
            SecondaryText = secondaryText;
            Hover = hover;
            Separator = separator;
        }

        public Color Background { get; }

        public Color Border { get; }

        public Color Text { get; }

        public Color SecondaryText { get; }

        public Color Hover { get; }

        public Color Separator { get; }

        internal static TrayMenuPalette From(ThemeManager theme) => new TrayMenuPalette(
            FromRgb(theme.GetRgb("CardBackgroundBrush")),
            FromRgb(theme.GetRgb("CardBorderBrush")),
            FromRgb(theme.GetRgb("TextPrimaryBrush")),
            FromRgb(theme.GetRgb("TextSecondaryBrush")),
            FromRgb(theme.GetRgb("HoverBrush")),
            FromRgb(theme.GetRgb("DividerBrush")));

        private static Color FromRgb(uint rgb) => Color.FromArgb((int)(0xFF000000 | rgb));
    }

    /// <summary>회색 그라데이션 없는 평평한 메뉴: 둥근 선택 표시, 얇은 구분선.</summary>
    public sealed class TrayMenuRenderer : ToolStripProfessionalRenderer
    {
        private readonly TrayMenuPalette _palette;

        public TrayMenuRenderer(TrayMenuPalette palette)
        {
            _palette = palette;
            RoundedEdges = false;
        }

        /// <summary>Windows 11처럼 창 모서리를 DWM이 둥글게 그리면 테두리는 DWM에 맡긴다.</summary>
        public bool DrawBorder { get; set; } = true;

        protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e) =>
            e.Graphics.Clear(_palette.Background);

        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
            if (!DrawBorder) return;

            using var pen = new Pen(_palette.Border);
            var bounds = e.AffectedBounds;
            e.Graphics.DrawRectangle(pen, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
        }

        protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
        {
            // 왼쪽 회색 여백을 그리지 않는다.
        }

        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            if (!e.Item.Selected || !e.Item.Enabled) return;

            var scale = e.Graphics.DpiX / 96f;
            var inset = (int)(4 * scale);
            var bounds = new Rectangle(inset, 0, e.Item.Width - inset * 2, e.Item.Height);
            using var path = RoundedRectangle(bounds, (int)(6 * scale));
            using var brush = new SolidBrush(_palette.Hover);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.FillPath(brush, path);
        }

        /// <summary>
        /// 글자는 직접 그린다: WinForms는 위아래 여백이 있는 항목에서 글자 상자를 위쪽에 붙이고,
        /// 누를 수 없는 항목(제목/상태)은 시스템 회색으로 칠하기 때문이다.
        /// </summary>
        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            var color = e.Item.Enabled || TrayMenu.IsHeader(e.Item) ? _palette.Text : _palette.SecondaryText;
            var top = MenuTextLayout.CenteredTop(e.Text, e.TextFont, e.Item.Height);
            var bounds = new Rectangle(e.TextRectangle.X, top, e.Item.Width - e.TextRectangle.X, e.TextFont.Height);
            TextRenderer.DrawText(e.Graphics, e.Text, e.TextFont, bounds, color, MenuTextLayout.Flags);
        }

        protected override void OnRenderItemImage(ToolStripItemImageRenderEventArgs e)
        {
            // 누를 수 없는 제목/상태 항목의 아이콘도 흐리게 바꾸지 않는다.
            if (e.Image != null && (TrayMenu.IsHeader(e.Item) || TrayMenu.IsStatus(e.Item)))
            {
                e.Graphics.DrawImage(e.Image, e.ImageRectangle);
                return;
            }

            base.OnRenderItemImage(e);
        }

        protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
        {
            var scale = e.Graphics.DpiX / 96f;
            var margin = (int)(12 * scale);
            var y = e.Item.Height / 2;
            using var pen = new Pen(_palette.Separator);
            e.Graphics.DrawLine(pen, margin, y, e.Item.Width - margin, y);
        }

        private static GraphicsPath RoundedRectangle(Rectangle bounds, int radius)
        {
            var diameter = radius * 2;
            var path = new GraphicsPath();
            path.AddArc(bounds.Left, bounds.Top, diameter, diameter, 180, 90);
            path.AddArc(bounds.Right - diameter, bounds.Top, diameter, diameter, 270, 90);
            path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(bounds.Left, bounds.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
