using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Threading;
using KakaoTalkAdBlockPlus.Native;
using KakaoTalkAdBlockPlus.Settings;

namespace KakaoTalkAdBlockPlus.UI
{
    public partial class SettingsWindow : Window
    {
        private readonly SettingsViewModel _viewModel;
        private readonly ThemeManager _theme;
        private readonly DispatcherTimer _statusTimer;

        internal SettingsWindow(SettingsViewModel viewModel, ThemeManager theme)
        {
            InitializeComponent();
            _viewModel = viewModel;
            _theme = theme;
            DataContext = viewModel;
            IntervalSlider.Maximum = IntervalSteps.Count - 1;

            _statusTimer = new DispatcherTimer(DispatcherPriority.Background) { Interval = TimeSpan.FromSeconds(1) };
            _statusTimer.Tick += (_, _) => _viewModel.RefreshStatus();

            SourceInitialized += (_, _) => StyleWindowFrame();
            Loaded += (_, _) =>
            {
                _viewModel.RefreshStatus();
                _statusTimer.Start();
            };
            _theme.ThemeChanged += OnThemeChanged;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            // Esc로 닫을 때도 입력 중인 값을 놓치지 않는다.
            if (IntervalBox.IsKeyboardFocused) _viewModel.ApplyIntervalText();
            base.OnClosing(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            _statusTimer.Stop();
            _theme.ThemeChanged -= OnThemeChanged;
            base.OnClosed(e);
        }

        private void OnThemeChanged(object? sender, EventArgs e) => StyleWindowFrame();

        /// <summary>Windows 11에서 제목 표시줄을 창 배경색에 맞춘다.</summary>
        private void StyleWindowFrame()
        {
            var handle = new WindowInteropHelper(this).Handle;
            if (handle == IntPtr.Zero) return;

            Dwmapi.StyleCaption(handle, _theme.IsDark, _theme.GetRgb("WindowBackgroundBrush"), _theme.GetRgb("TextPrimaryBrush"));
        }

        private void OnIntervalKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;

            _viewModel.ApplyIntervalText();
            IntervalBox.SelectAll();
            e.Handled = true;
        }

        private void OnIntervalLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e) => _viewModel.ApplyIntervalText();

        private void OnResetIntervalClick(object sender, RoutedEventArgs e) => _viewModel.ResetInterval();

        private void OnCloseClick(object sender, RoutedEventArgs e) => Close();
    }
}
