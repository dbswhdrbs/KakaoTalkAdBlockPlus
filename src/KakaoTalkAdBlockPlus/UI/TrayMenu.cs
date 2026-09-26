using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using KakaoTalkAdBlockPlus.AdBlock;
using KakaoTalkAdBlockPlus.Native;

namespace KakaoTalkAdBlockPlus.UI
{
    /// <summary>트레이 아이콘 오른쪽 클릭 메뉴: 앱 이름 · 상태 · 설정 열기 · 종료.</summary>
    internal sealed class TrayMenu : ContextMenuStrip
    {
        private const string HeaderTag = "header";
        private const string StatusTag = "status";

        private readonly ToolStripMenuItem _statusItem = new ToolStripMenuItem { Enabled = false, Tag = StatusTag };
        private readonly ToolStripMenuItem _settingsItem;
        private readonly ToolStripMenuItem _exitItem;
        private readonly ThemeManager _theme;
        private readonly Func<AdBlockStatus> _status;
        private readonly float _scale;

        public TrayMenu(Icon appIcon, ThemeManager theme, Func<AdBlockStatus> status, Action openSettings, Action exit)
        {
            _theme = theme;
            _status = status;
            using (var screen = Graphics.FromHwnd(IntPtr.Zero))
            {
                _scale = screen.DpiX / 96f;
            }

            // 제목과 상태는 누를 수 없는 항목으로 두어 다른 메뉴 항목과 같은 줄에 맞춘다.
            var header = new ToolStripMenuItem(AppInfo.DisplayName)
            {
                Enabled = false,
                Tag = HeaderTag,
                Font = new Font(SystemFonts.MenuFont, FontStyle.Bold),
                Image = new Icon(appIcon, Scaled(16), Scaled(16)).ToBitmap(),
            };
            _settingsItem = new ToolStripMenuItem("설정 열기", null, (_, _) => openSettings())
            {
                Font = new Font(SystemFonts.MenuFont, FontStyle.Bold),
            };
            _exitItem = new ToolStripMenuItem("종료", null, (_, _) => exit());

            Font = SystemFonts.MenuFont;
            ShowImageMargin = true;
            ImageScalingSize = new Size(Scaled(16), Scaled(16));
            Padding = new Padding(0, Scaled(6), 0, Scaled(6));
            Items.AddRange(new ToolStripItem[]
            {
                header, _statusItem, new ToolStripSeparator(), _settingsItem, new ToolStripSeparator(), _exitItem,
            });
            foreach (ToolStripItem item in Items)
            {
                if (item is ToolStripSeparator) continue;
                item.Padding = new Padding(Scaled(4), Scaled(5), Scaled(12), Scaled(5));
            }

            Prepare();
        }

        /// <summary>누를 수는 없지만 흐리게 보이지 않아야 하는 제목 항목.</summary>
        internal static bool IsHeader(ToolStripItem item) => item.Tag as string == HeaderTag;

        /// <summary>누를 수 없고 흐린 색으로 쓰는 상태 항목. 초록/회색 점은 그대로 보인다.</summary>
        internal static bool IsStatus(ToolStripItem item) => item.Tag as string == StatusTag;

        protected override void OnOpening(System.ComponentModel.CancelEventArgs e)
        {
            Prepare();
            base.OnOpening(e);
        }

        /// <summary>메뉴를 열 때마다 현재 테마 색과 상태를 반영한다.</summary>
        internal void Prepare()
        {
            var palette = TrayMenuPalette.From(_theme);
            var roundedByDwm = Dwmapi.TryRoundCorners(Handle, _theme.GetRgb("CardBorderBrush"));
            Renderer = new TrayMenuRenderer(palette) { DrawBorder = !roundedByDwm };

            var status = _status();
            _statusItem.Text = StatusText.Summary(status);
            ReplaceImage(_statusItem, StatusDot(_theme.GetRgb(status.IsKakaoTalkRunning ? "StatusRunningBrush" : "StatusWaitingBrush")));
            ReplaceImage(_settingsItem, MenuGlyphs.Render(MenuGlyphs.Settings, palette.Text, Scaled(16)));
            ReplaceImage(_exitItem, MenuGlyphs.Render(MenuGlyphs.Power, palette.Text, Scaled(16)));
        }

        private Bitmap StatusDot(uint rgb)
        {
            var size = Scaled(16);
            var bitmap = new Bitmap(size, size);
            using var graphics = Graphics.FromImage(bitmap);
            using var brush = new SolidBrush(Color.FromArgb((int)(0xFF000000 | rgb)));
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            var dot = size * 0.5f;
            graphics.FillEllipse(brush, (size - dot) / 2, (size - dot) / 2, dot, dot);
            return bitmap;
        }

        private static void ReplaceImage(ToolStripItem item, Image? image)
        {
            var previous = item.Image;
            item.Image = image;
            previous?.Dispose();
        }

        private int Scaled(int value) => (int)Math.Round(value * _scale);
    }
}
