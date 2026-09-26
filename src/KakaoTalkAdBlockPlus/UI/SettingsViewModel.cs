using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using KakaoTalkAdBlockPlus.AdBlock;
using KakaoTalkAdBlockPlus.Settings;
using KakaoTalkAdBlockPlus.Startup;

namespace KakaoTalkAdBlockPlus.UI
{
    /// <summary>설정창: 확인 주기, 윈도우 시작 시 자동 실행, 현재 상태.</summary>
    public sealed class SettingsViewModel : INotifyPropertyChanged
    {
        private readonly ISettingsStore _store;
        private readonly IAdBlockService _service;
        private readonly IStartupRegistration _startup;
        private int _intervalStepIndex;
        private CheckInterval _interval;
        private string _intervalText;
        private string? _intervalError;
        private string? _startupError;
        private AdBlockStatus _status;

        public SettingsViewModel(ISettingsStore store, IAdBlockService service, IStartupRegistration startup)
        {
            _store = store;
            _service = service;
            _startup = startup;
            _interval = store.Load().CheckInterval;
            _intervalText = _interval.ToSecondsText();
            _intervalStepIndex = IntervalSteps.IndexOfNearest(_interval);
            _status = service.Status;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public bool IsKakaoTalkRunning => _status.IsKakaoTalkRunning;

        public string StatusTitle => StatusText.Title(_status);

        public string StatusDetail => StatusText.Detail(_status);

        /// <summary>확인 주기 입력칸 (초 단위).</summary>
        public string IntervalText
        {
            get => _intervalText;
            set => SetField(ref _intervalText, value);
        }

        /// <summary>슬라이더 위치 (IntervalSteps 인덱스). 바꾸면 그 단계의 주기를 바로 적용한다.</summary>
        public int IntervalStepIndex
        {
            get => _intervalStepIndex;
            set => ApplyInterval(IntervalSteps.At(value));
        }

        /// <summary>기본값이면 [기본값으로] 버튼을 감춘다.</summary>
        public bool IsDefaultInterval => _interval.Milliseconds == CheckInterval.DefaultMilliseconds;

        /// <summary>윈도우 시작 시 자동 실행 스위치.</summary>
        public bool StartWithWindows
        {
            get => _startup.IsEnabled;
            set
            {
                try
                {
                    if (value) _startup.Enable();
                    else _startup.Disable();
                    StartupError = null;
                }
                catch (Exception exception)
                {
                    Trace.TraceWarning("자동 실행 설정 실패: {0}", exception);
                    StartupError = "윈도우 시작 설정을 바꾸지 못했어요. 잠시 후 다시 시도해 주세요.";
                }

                // 실패했으면 스위치가 실제 상태로 돌아가도록 알린다.
                OnPropertyChanged(nameof(StartWithWindows));
            }
        }

        /// <summary>자동 실행을 바꾸지 못했을 때 보여 줄 문구. 문제가 없으면 null.</summary>
        public string? StartupError
        {
            get => _startupError;
            private set => SetField(ref _startupError, value);
        }

        /// <summary>입력칸 값이 잘못됐을 때 보여 줄 문구. 문제가 없으면 null.</summary>
        public string? IntervalError
        {
            get => _intervalError;
            private set => SetField(ref _intervalError, value);
        }

        /// <summary>입력칸에서 Enter를 누르거나 포커스가 떠날 때 부른다.</summary>
        public void ApplyIntervalText()
        {
            var result = CheckInterval.ParseSeconds(IntervalText);
            if (result.Status != IntervalParseStatus.Ok)
            {
                IntervalError = ErrorMessageFor(result.Status);
                return;
            }

            ApplyInterval(result.Interval);
        }

        /// <summary>설정창이 열려 있는 동안 주기적으로 불러 상태 문구를 새로 고친다.</summary>
        public void RefreshStatus()
        {
            _status = _service.Status;
            OnPropertyChanged(nameof(IsKakaoTalkRunning));
            OnPropertyChanged(nameof(StatusTitle));
            OnPropertyChanged(nameof(StatusDetail));
        }

        /// <summary>원본과 같은 기본 주기(0.1초)로 되돌린다.</summary>
        public void ResetInterval() => ApplyInterval(CheckInterval.Default);

        private void ApplyInterval(CheckInterval interval)
        {
            _interval = interval;
            IntervalError = null;
            _service.Interval = interval;
            try
            {
                _store.Save(new AppSettings(interval));
            }
            catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
            {
                Trace.TraceWarning("설정 저장 실패: {0}", exception);
                IntervalError = "설정을 저장하지 못했어요. 다음에 실행할 때는 이전 값으로 시작해요.";
            }

            IntervalText = interval.ToSecondsText();
            _intervalStepIndex = IntervalSteps.IndexOfNearest(interval);
            OnPropertyChanged(nameof(IntervalStepIndex));
            OnPropertyChanged(nameof(IsDefaultInterval));
        }

        private void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private void SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return;

            field = value;
            OnPropertyChanged(propertyName);
        }

        private static string ErrorMessageFor(IntervalParseStatus status) =>
            status is IntervalParseStatus.TooSmall or IntervalParseStatus.TooLarge
                ? $"{CheckInterval.FromMilliseconds(CheckInterval.MinMilliseconds).ToSecondsText()}초 ~ " +
                  $"{CheckInterval.FromMilliseconds(CheckInterval.MaxMilliseconds).ToSecondsText()}초 사이로 입력해 주세요"
                : "숫자로 입력해 주세요 (예: 0.5)";
    }
}
