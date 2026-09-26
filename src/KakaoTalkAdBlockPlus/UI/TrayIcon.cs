using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Threading;
using KakaoTalkAdBlockPlus.AdBlock;

namespace KakaoTalkAdBlockPlus.UI
{
    /// <summary>
    /// 알림 영역(트레이) 아이콘.
    /// 오른쪽 클릭: 상태 · 설정 열기 · 종료 메뉴, 왼쪽 두 번 클릭: 설정 열기.
    /// </summary>
    internal sealed class TrayIcon : IDisposable
    {
        private readonly NotifyIcon _notifyIcon;
        private readonly TrayMenu _menu;
        private readonly Func<AdBlockStatus> _status;
        private readonly DispatcherTimer _tooltipTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };

        public TrayIcon(Icon icon, ThemeManager theme, Func<AdBlockStatus> status, Action openSettings, Action exit)
        {
            _status = status;
            _menu = new TrayMenu(icon, theme, status, openSettings, exit);
            _notifyIcon = new NotifyIcon { Icon = icon, ContextMenuStrip = _menu, Text = AppInfo.DisplayName, Visible = true };
            _notifyIcon.MouseDoubleClick += (_, e) =>
            {
                if (e.Button == MouseButtons.Left) openSettings();
            };
            _notifyIcon.BalloonTipClicked += (_, _) => openSettings();

            _tooltipTimer.Tick += (_, _) => UpdateTooltip();
            _tooltipTimer.Start();
            UpdateTooltip();
        }

        /// <summary>직접 실행했을 때 어디에 있는지 알려 준다 (윈도우 시작 시 자동 실행이면 띄우지 않는다).</summary>
        public void ShowStartedBalloon() =>
            _notifyIcon.ShowBalloonTip(
                5000,
                "카카오톡 광고 차단이 시작됐어요",
                "알림 영역(트레이)에서 광고를 차단하고 있어요. 아이콘을 오른쪽 클릭하면 설정과 종료 메뉴가 나와요.",
                ToolTipIcon.None);

        public void Dispose()
        {
            _tooltipTimer.Stop();
            _notifyIcon.Visible = false;
            _notifyIcon.Dispose();
            _menu.Dispose();
        }

        private void UpdateTooltip() => _notifyIcon.Text = AppInfo.DisplayName + "\n" + StatusText.Title(_status());
    }
}
